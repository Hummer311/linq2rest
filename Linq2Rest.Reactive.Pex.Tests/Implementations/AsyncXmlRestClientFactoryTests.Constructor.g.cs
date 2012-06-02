// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using NUnit.Framework;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Pex.Framework;

namespace Linq2Rest.Reactive.Implementations
{
	public partial class AsyncXmlRestClientFactoryTests
	{
[Test]
[PexGeneratedBy(typeof(AsyncXmlRestClientFactoryTests))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException692()
{
    AsyncXmlRestClientFactory asyncXmlRestClientFactory;
    asyncXmlRestClientFactory = this.Constructor((Uri)null);
}
[Test]
[PexGeneratedBy(typeof(AsyncXmlRestClientFactoryTests))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException939()
{
    Uri uri;
    AsyncXmlRestClientFactory asyncXmlRestClientFactory;
    uri = new Uri("  \\\\-");
    asyncXmlRestClientFactory = this.Constructor(uri);
}
[Test]
[PexGeneratedBy(typeof(AsyncXmlRestClientFactoryTests))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException13()
{
    Uri uri;
    AsyncXmlRestClientFactory asyncXmlRestClientFactory;
    uri = new Uri("  \\\\0");
    asyncXmlRestClientFactory = this.Constructor(uri);
}
[Test]
[PexGeneratedBy(typeof(AsyncXmlRestClientFactoryTests))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException896()
{
    Uri uri;
    AsyncXmlRestClientFactory asyncXmlRestClientFactory;
    uri = new Uri("  aa:");
    asyncXmlRestClientFactory = this.Constructor(uri);
}
[Test]
[PexGeneratedBy(typeof(AsyncXmlRestClientFactoryTests))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException398()
{
    Uri uri;
    AsyncXmlRestClientFactory asyncXmlRestClientFactory;
    uri = new Uri("  /\\\u00a1");
    asyncXmlRestClientFactory = this.Constructor(uri);
}
	}
}
