﻿// -----------------------------------------------------------------------
// <copyright file="GuidExpressionFactoryTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Linq2Rest.Tests.Parser.Readers
{
	using System;
	using System.Linq.Expressions;
	using Linq2Rest.Parser.Readers;
	using NUnit.Framework;

	[TestFixture]
	public class GuidExpressionFactoryTests
	{
		private GuidExpressionFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new GuidExpressionFactory();
		}

		[Test]
		public void WhenFilterGuidIsIncorrectFormatThenThrows()
		{
			const string Parameter = "guid'blah'";

			Assert.Throws<InvalidOperationException>(() => _factory.Convert(Parameter));
		}

		[Test]
		public void WhenFilterIncludesGuidParameterThenReturnedExpressionContainsGuid()
		{
			var guid = Guid.NewGuid();
			var parameter = string.Format("guid'{0}'", guid);

			var expression = _factory.Convert(parameter);

			Assert.IsAssignableFrom<Guid>(expression.Value);
		}

		[Test]
		public void WhenFilterIncludesGuidParameterWithNoDashesThenReturnedExpressionContainsGuid()
		{
			var guid = Guid.NewGuid();
			var parameter = string.Format("guid'{0}'", guid.ToString("N"));

			var expression = _factory.Convert(parameter);

			Assert.IsAssignableFrom<Guid>(expression.Value);
		}

		[Test]
		public void WhenFilterIncludesGuidParameterInDoubleQuotesThenReturnedExpressionContainsGuid()
		{
			var guid = Guid.NewGuid();
			var parameter = string.Format("guid\"{0}\"", guid);

			var expression = _factory.Convert(parameter);

			Assert.IsAssignableFrom<Guid>(expression.Value);
		}

		[Test]
		public void WhenFilterIncludesGuidParameterWithNoDashesInDoubleQuotesThenReturnedExpressionContainsGuid()
		{
			var guid = Guid.NewGuid();
			var parameter = string.Format("guid\"{0}\"", guid.ToString("N"));

			var expression = _factory.Convert(parameter);

			Assert.IsAssignableFrom<Guid>(expression.Value);
		}
	}
}
