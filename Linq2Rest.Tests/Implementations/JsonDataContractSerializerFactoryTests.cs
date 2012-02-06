﻿namespace Linq2Rest.Tests.Implementations
{
	using System;
	using Linq2Rest.Implementations;
	using NUnit.Framework;

	[TestFixture]
	public class JsonDataContractSerializerFactoryTests
	{
		private JsonDataContractSerializerFactory _factory;

		[SetUp]
		public void Setup()
		{
			_factory = new JsonDataContractSerializerFactory(Type.EmptyTypes);
		}

		[Test]
		public void WhenCreatingSerializerThenDoesNotReturnNull()
		{
			Assert.NotNull(_factory.Create<SimpleContractItem>());
		}

		[Test]
		public void CreatedSerializerCanDeserializeDataContractType()
		{
			const string Json = "{\"Value\": 2, \"Text\":\"test\"}";

			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.Deserialize(Json);

			Assert.AreEqual(2, deserializedResult.Value);
			Assert.AreEqual("test", deserializedResult.SomeString);
		}

		[Test]
		public void CreatedSerializerCanDeserializeListOfDataContractType()
		{
			const string Json = "[{\"Value\": 2, \"Text\":\"test\"}]";

			var serializer = _factory.Create<SimpleContractItem>();

			var deserializedResult = serializer.DeserializeList(Json);

			Assert.AreEqual(1, deserializedResult.Count);
		}
	}
}