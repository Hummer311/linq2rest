// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SelectExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests
{
	using System.Linq;
	using Linq2Rest;
	using Linq2Rest.Parser;
	using NUnit.Framework;

	public class SelectExpressionFactoryTests
	{
		private FakeItem[] _items;
		private SelectExpressionFactory<FakeItem> _factory;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			var memberNameResolver = new MemberNameResolver();
			this._factory = new SelectExpressionFactory<FakeItem>(memberNameResolver, new RuntimeTypeProvider(memberNameResolver));

			this._items = new[]
				{
					new FakeItem { IntValue = 2, DoubleValue = 5 },
					new FakeItem { IntValue = 1, DoubleValue = 4 },
					new FakeItem { IntValue = 3, DoubleValue = 4 }
				};
		}

		[Test]
		public void WhenApplyingSelectionThenReturnsObjectWithOnlySelectedPropertiesAsFields()
		{
			var expression = this._factory.Create("Number").Compile();

			var selection = this._items.Select(expression);

			Assert.True(selection.All(x => x.GetType().GetProperty("Number") != null));
		}
	}
}