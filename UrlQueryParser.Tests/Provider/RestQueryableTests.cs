﻿// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
// All other rights reserved.

namespace UrlQueryParser.Tests.Provider
{
	using System.Web.Script.Serialization;

	using Moq;

	using NUnit.Framework;

	using UrlQueryParser.Provider;

	public class RestQueryableTests
	{
		[Test]
		public void ElementTypeIsSameAsGenericParameter()
		{
			var queryable = new RestQueryable<FakeItem>(new Mock<IRestClient>().Object, new JavaScriptSerializer());

			Assert.AreEqual(typeof(FakeItem), queryable.ElementType);
		}
	}
}
