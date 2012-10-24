// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleExpressionFactoryTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2011
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SingleExpressionFactoryTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Tests.Parser.Readers
{
	using System;
	using Linq2Rest.Parser.Readers;
	using NUnit.Framework;

	[TestFixture]
	public class SingleExpressionFactoryTests
	{
		private SingleExpressionFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new SingleExpressionFactory();
		}

		[Test]
		public void WhenFilterIsIncorrectFormatThenThrows()
		{
			const string Parameter = "blah";

			Assert.Throws<FormatException>(() => _factory.Convert(Parameter));
		}

		[Test]
		public void WhenFilterIncludesSingleParameterThenReturnedExpressionContainsSingle()
		{
			var expression = _factory.Convert("1.23");

			Assert.IsAssignableFrom<float>(expression.Value);
		}

		[Test]
		public void WhenFilterIncludesSingleParameterWithTrailingUpperCaseMThenReturnedExpressionContainsSingle()
		{
			var expression = _factory.Convert("1.23F");

			Assert.IsAssignableFrom<float>(expression.Value);
		}

		[Test]
		public void WhenFilterIncludesSingleParameterWithTrailingLowerCaseMThenReturnedExpressionContainsSingle()
		{
			var expression = _factory.Convert("1.23f");

			Assert.IsAssignableFrom<float>(expression.Value);
		}
	}
}