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
using Microsoft.Pex.Framework;
using NUnit.Framework;
using Microsoft.Pex.Framework.Generated;

namespace Linq2Rest.Provider.Writers
{
	public partial class RationalValueWriterTests
	{
[Test]
[PexGeneratedBy(typeof(RationalValueWriterTests))]
public void HandlesGet200()
{
    Type type;
    DecimalValueWriter s0 = new DecimalValueWriter();
    type = this.HandlesGet((RationalValueWriter)s0);
    PexAssert.IsNotNull((object)type);
}
[Test]
[PexGeneratedBy(typeof(RationalValueWriterTests))]
public void HandlesGet295()
{
    Type type;
    SingleValueWriter s0 = new SingleValueWriter();
    type = this.HandlesGet((RationalValueWriter)s0);
    PexAssert.IsNotNull((object)type);
}
[Test]
[PexGeneratedBy(typeof(RationalValueWriterTests))]
public void HandlesGet142()
{
    Type type;
    DoubleValueWriter s0 = new DoubleValueWriter();
    type = this.HandlesGet((RationalValueWriter)s0);
    PexAssert.IsNotNull((object)type);
}
	}
}
