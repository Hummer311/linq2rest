// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
// All other rights reserved.

namespace Linq2Rest.Provider
{
	using System;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Threading;

	internal static class ExpressionProcessor
	{
		private static readonly ExpressionType[] _compositeExpressionTypes = new[] { ExpressionType.Or, ExpressionType.OrElse, ExpressionType.And, ExpressionType.AndAlso };

		public static string ProcessExpression(this Expression expression)
		{
			Contract.Requires(expression != null);

			if (expression is LambdaExpression)
			{
				return ProcessExpression((expression as LambdaExpression).Body);
			}

			if (expression is MemberExpression)
			{
				var memberExpression = expression as MemberExpression;
				if (!memberExpression.IsMemberOfParameter())
				{
					var collapsedExpression = CollapseCapturedOuterVariables(memberExpression);
					if (!(collapsedExpression is MemberExpression))
					{
						return ProcessExpression(collapsedExpression);
					}

					memberExpression = (MemberExpression)collapsedExpression;
				}

				var memberCall = GetMemberCall(memberExpression);

				return string.IsNullOrWhiteSpace(memberCall)
						? memberExpression.Member.Name
						: string.Format("{0}({1})", memberCall, ProcessExpression(memberExpression.Expression));
			}

			if (expression is ConstantExpression)
			{
				var value = (expression as ConstantExpression).Value;
				return string.Format(
					Thread.CurrentThread.CurrentCulture,
					"{0}{1}{0}",
					value is string ? "'" : string.Empty,
					value ?? "null");
			}

			if (expression is UnaryExpression)
			{
				var unaryExpression = expression as UnaryExpression;
				var operand = unaryExpression.Operand;
				switch (unaryExpression.NodeType)
				{
					case ExpressionType.Not:
					case ExpressionType.IsFalse:
						return string.Format("not({0})", ProcessExpression(operand));
					default:
						return ProcessExpression(operand);
				}
			}

			if (expression is BinaryExpression)
			{
				var binaryExpression = expression as BinaryExpression;
				var operation = GetOperation(binaryExpression);

				var isLeftComposite = _compositeExpressionTypes.Any(x => x == binaryExpression.Left.NodeType);
				var isRightComposite = _compositeExpressionTypes.Any(x => x == binaryExpression.Right.NodeType);
				return string.Format(
					"{0} {1} {2}",
					string.Format(isLeftComposite ? "({0})" : "{0}", ProcessExpression(binaryExpression.Left)),
					operation,
					string.Format(isRightComposite ? "({0})" : "{0}", ProcessExpression(binaryExpression.Right)));
			}

			if (expression is MethodCallExpression)
			{
				return GetMethodCall(expression as MethodCallExpression);
			}

			if (expression is NewExpression)
			{
				return expression.ToString();
			}

			throw new InvalidOperationException("Expression is not recognized or supported");
		}

		public static object GetExpressionValue(this Expression expression)
		{
			if (expression is UnaryExpression)
			{
				return (expression as UnaryExpression).Operand;
			}

			if (expression is ConstantExpression)
			{
				return (expression as ConstantExpression).Value;
			}

			return null;
		}

		private static Expression CollapseCapturedOuterVariables(MemberExpression input)
		{
			if (input == null || input.NodeType != ExpressionType.MemberAccess)
			{
				return input;
			}

			if (input.Expression is MemberExpression)
			{
				var objectMember = Expression.Convert(input, typeof(object));

				var getterLambda = Expression.Lambda<Func<object>>(objectMember).Compile();

				return Expression.Constant(getterLambda());
			}

			if (input.Expression is ConstantExpression)
			{
				object obj = ((ConstantExpression)input.Expression).Value;
				if (obj == null)
				{
					return input;
				}

				Type t = obj.GetType();
				if (!t.IsNestedPrivate || !t.Name.StartsWith("<>"))
				{
					return input;
				}

				var fi = (FieldInfo)input.Member;
				object result = fi.GetValue(obj);
				return result is Expression ? (Expression)result : Expression.Constant(result);
			}

			return input;
		}

		private static bool IsMemberOfParameter(this MemberExpression input)
		{
			if (input.Expression == null)
			{
				return false;
			}

			var nodeType = input.Expression.NodeType;
			var tempExpression = input.Expression as MemberExpression;
			while (nodeType == ExpressionType.MemberAccess)
			{
				nodeType = tempExpression.Expression.NodeType;
				tempExpression = tempExpression.Expression as MemberExpression;
			}

			return nodeType == ExpressionType.Parameter;
		}

		private static string GetMemberCall(MemberExpression memberExpression)
		{
			Contract.Requires(memberExpression != null);

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

		private static string GetMethodCall(MethodCallExpression expression)
		{
			Contract.Requires(expression != null);

			var methodName = expression.Method.Name;
			var declaringType = expression.Method.DeclaringType;
			if (declaringType == typeof(string))
			{
				switch (methodName)
				{
					case "Replace":
						{
							Contract.Assume(expression.Arguments.Count > 1);

							var firstArgument = expression.Arguments[0];
							var secondArgument = expression.Arguments[1];

							return string.Format(
								"replace({0}, {1}, {2})",
								ProcessExpression(expression.Object),
								ProcessExpression(firstArgument),
								ProcessExpression(secondArgument));
						}

					case "Trim":
						return string.Format("trim({0})", ProcessExpression(expression.Object));
					case "ToLower":
					case "ToLowerInvariant":
						return string.Format("tolower({0})", ProcessExpression(expression.Object));
					case "ToUpper":
					case "ToUpperInvariant":
						return string.Format("toupper({0})", ProcessExpression(expression.Object));
					case "Substring":
						{
							Contract.Assume(expression.Arguments.Count > 0);

							if (expression.Arguments.Count == 1)
							{
								var argumentExpression = expression.Arguments[0];
								return string.Format(
									"substring({0}, {1})", ProcessExpression(expression.Object), ProcessExpression(argumentExpression));
							}

							var firstArgument = expression.Arguments[0];
							var secondArgument = expression.Arguments[1];
							return string.Format(
								"substring({0}, {1}, {2})",
								ProcessExpression(expression.Object),
								ProcessExpression(firstArgument),
								ProcessExpression(secondArgument));
						}

					case "IndexOf":
						{
							Contract.Assume(expression.Arguments.Count > 0);

							var argumentExpression = expression.Arguments[0];
							return string.Format(
								"indexof({0}, {1})", ProcessExpression(expression.Object), ProcessExpression(argumentExpression));
						}

					case "EndsWith":
						{
							Contract.Assume(expression.Arguments.Count > 0);

							var argumentExpression = expression.Arguments[0];
							return string.Format(
								"endswith({0}, {1})", ProcessExpression(expression.Object), ProcessExpression(argumentExpression));
						}

					case "StartsWith":
						{
							Contract.Assume(expression.Arguments.Count > 0);

							var argumentExpression = expression.Arguments[0];
							return string.Format(
								"startswith({0}, {1})", ProcessExpression(expression.Object), ProcessExpression(argumentExpression));
						}
				}
			}
			else if (declaringType == typeof(Math))
			{
				Contract.Assume(expression.Arguments.Count > 0);

				var mathArgument = expression.Arguments[0];

				switch (methodName)
				{
					case "Round":
						return string.Format("round({0})", ProcessExpression(mathArgument));
					case "Floor":
						return string.Format("floor({0})", ProcessExpression(mathArgument));
					case "Ceiling":
						return string.Format("ceiling({0})", ProcessExpression(mathArgument));
				}
			}

			if (expression.Method.IsStatic)
			{
				return expression.ToString();
			}

			return string.Empty;
		}

		private static string GetOperation(Expression expression)
		{
			Contract.Requires(expression != null);

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
	}
}