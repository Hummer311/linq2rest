// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Parser
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Web.UI.WebControls;

	/// <summary>
	/// Defines the SortExpressionFactory´.
	/// </summary>
	public class SortExpressionFactory : ISortExpressionFactory
	{
		/// <summary>
		/// Creates an enumeration of sort descriptions from its string representation.
		/// </summary>
		/// <param name="filter">The string representation of the sort descriptions.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
		public IEnumerable<SortDescription<T>> Create<T>(string filter)
		{
			if (string.IsNullOrWhiteSpace(filter))
			{
				return new SortDescription<T>[0];
			}

			var parameterExpression = Expression.Parameter(typeof(T), "x");

			var sortTokens = filter.Split(',');
			return from sortToken in sortTokens
				   select sortToken.Split(' ')
					   into sort
					   let property = GetPropertyExpression<T>(sort.First(), parameterExpression)
					   where property != null
					   let direction = sort.ElementAtOrDefault(1) == "desc" ? SortDirection.Descending : SortDirection.Ascending
					   select new SortDescription<T>(property, direction);
		}

		private static Expression GetPropertyExpression<T>(string propertyToken, ParameterExpression parameter)
		{
			if(string.IsNullOrWhiteSpace(propertyToken))
			{
				return null;
			}

			var parentType = typeof(T);
			Expression propertyExpression = null;
			var propertyChain = propertyToken.Split('/');
			foreach (var propertyName in propertyChain)
			{
				var property = parentType.GetProperty(propertyName);
				if (property != null)
				{
					parentType = property.PropertyType;
					propertyExpression = propertyExpression == null
											? Expression.Property(parameter, property)
											: Expression.Property(propertyExpression, property);
				}
			}

			var funcType = typeof(Func<,>).MakeGenericType(typeof(T), parentType);

			Contract.Assume(propertyExpression != null);

			var lambda = Expression.Lambda(funcType, propertyExpression, parameter);
			return propertyExpression == null ? null : lambda;
		}
	}
}