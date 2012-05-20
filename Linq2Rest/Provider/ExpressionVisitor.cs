// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
#if !WINDOWS_PHONE
	using System.Diagnostics.Contracts;
#endif
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Linq2Rest.Provider.Writers;

	internal class ExpressionVisitor : IExpressionVisitor
	{
		private static readonly ExpressionType[] _compositeExpressionTypes = new[] { ExpressionType.Or, ExpressionType.OrElse, ExpressionType.And, ExpressionType.AndAlso };

		public string Visit(Expression expression)
		{
			return expression == null ? null : Visit(expression, expression.Type, GetRootParameterName(expression));
		}

		private static Type GetUnconvertedType(Expression expression)
		{
#if !WINDOWS_PHONE
			Contract.Requires(expression != null);
#endif

			switch (expression.NodeType)
			{
				case ExpressionType.Convert:
					var unaryExpression = expression as UnaryExpression;

#if !WINDOWS_PHONE
					Contract.Assume(unaryExpression != null, "Matches node type.");
#endif

					return unaryExpression.Operand.Type;
				default:
					return expression.Type;
			}
		}

		private static string GetMemberCall(MemberExpression memberExpression)
		{
#if !WINDOWS_PHONE
			Contract.Requires(memberExpression != null);
			Contract.Ensures(Contract.Result<string>() != null);
#endif

			var declaringType = memberExpression.Member.DeclaringType;
			var name = memberExpression.Member.Name;

			if (declaringType == typeof(string))
			{
				if (name == "Length")
				{
					return name.ToLowerInvariant();
				}
			}
			else if (declaringType == typeof(DateTime))
			{
				switch (name)
				{
					case "Hour":
					case "Minute":
					case "Second":
					case "Day":
					case "Month":
					case "Year":
						return name.ToLowerInvariant();
				}
			}

			return string.Empty;
		}

		private static Expression CollapseCapturedOuterVariables(MemberExpression input)
		{
			if (input == null || input.NodeType != ExpressionType.MemberAccess)
			{
				return input;
			}

			if (input.Expression is MemberExpression)
			{
				var value = GetValue(input);
				return Expression.Constant(value);
			}

			var constantExpression = input.Expression as ConstantExpression;
			if (constantExpression != null)
			{
				var obj = constantExpression.Value;
				if (obj == null)
				{
					return input;
				}

				var fieldInfo = input.Member as FieldInfo;
				if (fieldInfo != null)
				{
					var result = fieldInfo.GetValue(obj);
					return result is Expression ? (Expression)result : Expression.Constant(result);
				}

				var propertyInfo = input.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					var result = propertyInfo.GetValue(obj, null);
					return result is Expression ? (Expression)result : Expression.Constant(result);
				}
			}

			return input;
		}

		private static object GetValue(Expression input)
		{
#if !WINDOWS_PHONE
			Contract.Requires(input != null);
#endif

			var objectMember = Expression.Convert(input, typeof(object));
			var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

			return getterLambda();
		}

		private static bool IsMemberOfParameter(MemberExpression input)
		{
#if !WINDOWS_PHONE
			Contract.Requires(input != null);
#endif

			if (input.Expression == null)
			{
				return false;
			}

			var nodeType = input.Expression.NodeType;
			var tempExpression = input.Expression as MemberExpression;
			while (nodeType == ExpressionType.MemberAccess)
			{
#if !WINDOWS_PHONE
				Contract.Assume(tempExpression != null, "It's a member access");
#endif

				nodeType = tempExpression.Expression.NodeType;
				tempExpression = tempExpression.Expression as MemberExpression;
			}

			return nodeType == ExpressionType.Parameter;
		}

		private static string GetOperation(Expression expression)
		{
#if !WINDOWS_PHONE
			Contract.Requires(expression != null);
#endif

			switch (expression.NodeType)
			{
				case ExpressionType.Add:
					return "add";
				case ExpressionType.AddChecked:
					break;
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					return "and";
				case ExpressionType.Divide:
					return "div";
				case ExpressionType.Equal:
					return "eq";
				case ExpressionType.GreaterThan:
					return "gt";
				case ExpressionType.GreaterThanOrEqual:
					return "ge";
				case ExpressionType.LessThan:
					return "lt";
				case ExpressionType.LessThanOrEqual:
					return "le";
				case ExpressionType.Modulo:
					return "mod";
				case ExpressionType.Multiply:
					return "mul";
				case ExpressionType.Not:
					return "not";
				case ExpressionType.NotEqual:
					return "ne";
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					return "or";
				case ExpressionType.Subtract:
					return "sub";
			}

			return string.Empty;
		}

		private static string GetRootParameterName(Expression expression)
		{
			if (expression is UnaryExpression)
			{
				expression = ((UnaryExpression)expression).Operand;
			}

			if (expression is LambdaExpression && ((LambdaExpression)expression).Parameters.Count() > 0)
			{
				return ((LambdaExpression)expression).Parameters.First().Name;
			}

			return null;
		}

		private string Visit(Expression expression, string rootParameterName)
		{
			return expression == null ? null : Visit(expression, expression.Type, rootParameterName);
		}

		private string GetMethodCall(MethodCallExpression expression, string rootParameterName)
		{
#if !WINDOWS_PHONE
			Contract.Requires(expression != null);
#endif

			var methodName = expression.Method.Name;
			var declaringType = expression.Method.DeclaringType;

			if (methodName == "Equals")
			{
				return string.Format(
									 "{0} eq {1}",
									 Visit(expression.Object, rootParameterName),
									 Visit(expression.Arguments[0], rootParameterName));
			}

			if (declaringType == typeof(string))
			{
				var obj = expression.Object;
#if !WINDOWS_PHONE
				Contract.Assume(obj != null);
#endif

				switch (methodName)
				{
					case "Replace":
						{
#if !WINDOWS_PHONE
							Contract.Assume(expression.Arguments.Count > 1);
#endif

							var firstArgument = expression.Arguments[0];
							var secondArgument = expression.Arguments[1];

#if !WINDOWS_PHONE
							Contract.Assume(firstArgument != null);
							Contract.Assume(secondArgument != null);
#endif

							return string.Format(
								"replace({0}, {1}, {2})",
								Visit(obj, rootParameterName),
								Visit(firstArgument, rootParameterName),
								Visit(secondArgument, rootParameterName));
						}

					case "Trim":
						return string.Format("trim({0})", Visit(obj, rootParameterName));
					case "ToLower":
					case "ToLowerInvariant":
						return string.Format("tolower({0})", Visit(obj, rootParameterName));
					case "ToUpper":
					case "ToUpperInvariant":
						return string.Format("toupper({0})", Visit(obj, rootParameterName));
					case "Substring":
						{
#if !WINDOWS_PHONE
							Contract.Assume(expression.Arguments.Count > 0);
#endif

							if (expression.Arguments.Count == 1)
							{
								var argumentExpression = expression.Arguments[0];

#if !WINDOWS_PHONE
								Contract.Assume(argumentExpression != null);
#endif

								return string.Format(
									"substring({0}, {1})", Visit(obj, rootParameterName), Visit(argumentExpression, rootParameterName));
							}

							var firstArgument = expression.Arguments[0];
							var secondArgument = expression.Arguments[1];

#if !WINDOWS_PHONE
							Contract.Assume(firstArgument != null);
							Contract.Assume(secondArgument != null);
#endif

							return string.Format(
								"substring({0}, {1}, {2})",
								Visit(obj, rootParameterName),
								Visit(firstArgument, rootParameterName),
								Visit(secondArgument, rootParameterName));
						}

					case "Contains":
						{
							var argumentExpression = expression.Arguments[0];

#if !WINDOWS_PHONE
							Contract.Assume(argumentExpression != null);
#endif

							return string.Format(
								"substringof({0}, {1})",
								Visit(argumentExpression, rootParameterName),
								Visit(obj, rootParameterName));
						}

					case "IndexOf":
						{
#if !WINDOWS_PHONE
							Contract.Assume(expression.Arguments.Count > 0);
#endif

							var argumentExpression = expression.Arguments[0];

#if !WINDOWS_PHONE
							Contract.Assume(argumentExpression != null);
#endif

							return string.Format("indexof({0}, {1})", Visit(obj, rootParameterName), Visit(argumentExpression, rootParameterName));
						}

					case "EndsWith":
						{
#if !WINDOWS_PHONE
							Contract.Assume(expression.Arguments.Count > 0);
#endif

							var argumentExpression = expression.Arguments[0];

#if !WINDOWS_PHONE
							Contract.Assume(argumentExpression != null);
#endif

							return string.Format("endswith({0}, {1})", Visit(obj, rootParameterName), Visit(argumentExpression, rootParameterName));
						}

					case "StartsWith":
						{
#if !WINDOWS_PHONE
							Contract.Assume(expression.Arguments.Count > 0);
#endif

							var argumentExpression = expression.Arguments[0];

#if !WINDOWS_PHONE
							Contract.Assume(argumentExpression != null);
#endif

							return string.Format("startswith({0}, {1})", Visit(obj, rootParameterName), Visit(argumentExpression, rootParameterName));
						}
				}
			}
			else if (declaringType == typeof(Math))
			{
#if !WINDOWS_PHONE
				Contract.Assume(expression.Arguments.Count > 0);
#endif

				var mathArgument = expression.Arguments[0];

#if !WINDOWS_PHONE
				Contract.Assume(mathArgument != null);
#endif

				switch (methodName)
				{
					case "Round":
						return string.Format("round({0})", Visit(mathArgument, rootParameterName));
					case "Floor":
						return string.Format("floor({0})", Visit(mathArgument, rootParameterName));
					case "Ceiling":
						return string.Format("ceiling({0})", Visit(mathArgument, rootParameterName));
				}
			}

			if (expression.Method.Name == "Any" || expression.Method.Name == "All")
			{
#if !WINDOWS_PHONE
				Contract.Assume(expression.Arguments.Count > 1);
#endif

				return string.Format("{0}/{1}({2}: {3})", Visit(expression.Arguments[0], rootParameterName), expression.Method.Name.ToLowerInvariant(), expression.Arguments[1] is LambdaExpression ? (expression.Arguments[1] as LambdaExpression).Parameters.First().Name : null, Visit(expression.Arguments[1], rootParameterName));
			}

			return ParameterValueWriter.Write(GetValue(expression));
		}

		private string Visit(Expression expression, Type type, string rootParameterName)
		{
#if !WINDOWS_PHONE
			Contract.Requires(expression != null);
			Contract.Requires(type != null);
#endif
			switch (expression.NodeType)
			{
				case ExpressionType.Constant:
					{
						var value = GetValue(Expression.Convert(expression, type));

#if !WINDOWS_PHONE
						Contract.Assume(type != null);
#endif

						return ParameterValueWriter.Write(value);
					}

				case ExpressionType.Add:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Divide:
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.NotEqual:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.Subtract:
					{
						var binaryExpression = expression as BinaryExpression;

#if !WINDOWS_PHONE
						Contract.Assume(binaryExpression != null);
#endif

						var operation = GetOperation(binaryExpression);

						var isLeftComposite = _compositeExpressionTypes.Any(x => x == binaryExpression.Left.NodeType);
						var isRightComposite = _compositeExpressionTypes.Any(x => x == binaryExpression.Right.NodeType);

						var leftType = GetUnconvertedType(binaryExpression.Left);
						var leftString = Visit(binaryExpression.Left, rootParameterName);
						var rightString = Visit(binaryExpression.Right, leftType, rootParameterName);

						return string.Format(
							"{0} {1} {2}",
							string.Format(isLeftComposite ? "({0})" : "{0}", leftString),
							operation,
							string.Format(isRightComposite ? "({0})" : "{0}", rightString));
					}

				case ExpressionType.Negate:
					{
						var unaryExpression = expression as UnaryExpression;

#if !WINDOWS_PHONE
						Contract.Assume(unaryExpression != null);
#endif

						var operand = unaryExpression.Operand;

						return string.Format("-{0}", Visit(operand, rootParameterName));
					}

				case ExpressionType.Not:
#if !SILVERLIGHT
				case ExpressionType.IsFalse:
#endif
					{
						var unaryExpression = expression as UnaryExpression;

#if !WINDOWS_PHONE
						Contract.Assume(unaryExpression != null);
#endif

						var operand = unaryExpression.Operand;

						return string.Format("not({0})", Visit(operand, rootParameterName));
					}
#if !SILVERLIGHT
				case ExpressionType.IsTrue:
					{
						var unaryExpression = expression as UnaryExpression;

						Contract.Assume(unaryExpression != null);

						var operand = unaryExpression.Operand;

						Contract.Assume(operand != null);

						return Visit(operand, rootParameterName);
					}
#endif

				case ExpressionType.Convert:
				case ExpressionType.Quote:
					{
						var unaryExpression = expression as UnaryExpression;

#if !WINDOWS_PHONE
						Contract.Assume(unaryExpression != null);
#endif

						var operand = unaryExpression.Operand;
						return Visit(operand, rootParameterName);
					}

				case ExpressionType.MemberAccess:
					{
						var memberExpression = expression as MemberExpression;
#if !WINDOWS_PHONE
						Contract.Assume(memberExpression != null);
#endif

						var pathPrefixes = new List<string>();

						var currentMemberExpression = memberExpression;
						while (currentMemberExpression != null)
						{
							pathPrefixes.Add(currentMemberExpression.Member.Name);
							if (currentMemberExpression.Expression is ParameterExpression &&
							    ((ParameterExpression) currentMemberExpression.Expression).Name != rootParameterName)
							{
								pathPrefixes.Add(((ParameterExpression) currentMemberExpression.Expression).Name);
							}

							currentMemberExpression = currentMemberExpression.Expression as MemberExpression;
						}

						pathPrefixes.Reverse();
						var prefix = string.Join("/", pathPrefixes);

						if (!IsMemberOfParameter(memberExpression))
						{
							var collapsedExpression = CollapseCapturedOuterVariables(memberExpression);
							if (!(collapsedExpression is MemberExpression))
							{
#if !WINDOWS_PHONE
								Contract.Assume(collapsedExpression != null);
#endif

								return Visit(collapsedExpression, rootParameterName);
							}

							memberExpression = (MemberExpression) collapsedExpression;
						}

						var memberCall = GetMemberCall(memberExpression);

						var innerExpression = memberExpression.Expression;

#if !WINDOWS_PHONE
						Contract.Assume(innerExpression != null);
#endif

						return string.IsNullOrWhiteSpace(memberCall)
						       	? prefix
						       	: string.Format("{0}({1})", memberCall, Visit(innerExpression, rootParameterName));
					}
					
				case ExpressionType.Call:
					var methodCallExpression = expression as MethodCallExpression;
#if !WINDOWS_PHONE
					Contract.Assume(methodCallExpression != null);
#endif

					return GetMethodCall(methodCallExpression, rootParameterName);
				case ExpressionType.New:
					var newValue = GetValue(expression);
					return ParameterValueWriter.Write(newValue);
				case ExpressionType.Lambda:
					var lambdaExpression = expression as LambdaExpression;

#if !WINDOWS_PHONE
					Contract.Assume(lambdaExpression != null);
#endif

					var body = lambdaExpression.Body;
					return Visit(body, rootParameterName);
				default:
					throw new InvalidOperationException("Expression is not recognized or supported");
			}
		}
	}
}