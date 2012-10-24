// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsignedShortExpressionFactory.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2011
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnsignedShortExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Linq.Expressions;

	internal class UnsignedShortExpressionFactory : IValueExpressionFactory
	{
		public Type Handles
		{
			get
			{
				return typeof(ushort);
			}
		}

		public ConstantExpression Convert(string token)
		{
			ushort number;
			if (ushort.TryParse(token, out number))
			{
				return Expression.Constant(number);
			}

			throw new FormatException("Could not read " + token + " as Unsigned Short.");
		}
	}
}