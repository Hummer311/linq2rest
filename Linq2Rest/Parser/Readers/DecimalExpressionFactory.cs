// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Globalization;
	using System.Linq.Expressions;

	internal class DecimalExpressionFactory : IValueExpressionFactory
	{
		public Type Handles
		{
			get
			{
				return typeof(decimal);
			}
		}

		public ConstantExpression Convert(string token)
		{
			var number = decimal.Parse(token.Trim('M', 'm'), NumberStyles.Any);
			return Expression.Constant(number);
		}
	}
}