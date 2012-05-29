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
using Microsoft.Pex.Engine.Exceptions;
using Microsoft.Pex.Framework;

namespace Linq2Rest.Provider.Writers
{
	public partial class TimeSpanValueWriterTests
	{
[Test]
[PexGeneratedBy(typeof(TimeSpanValueWriterTests))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void WriteThrowsContractException498()
{
    try
    {
      string s;
      TimeSpanValueWriter s0 = new TimeSpanValueWriter();
      s = this.Write(s0, (object)null);
      throw 
        new AssertionException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(TimeSpanValueWriterTests))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void WriteThrowsContractException59()
{
    try
    {
      string s;
      TimeSpanValueWriter s0 = new TimeSpanValueWriter();
      object s1 = new object();
      s = this.Write(s0, s1);
      throw 
        new AssertionException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[Test]
[PexGeneratedBy(typeof(TimeSpanValueWriterTests))]
public void Write908()
{
    string s;
    TimeSpanValueWriter s0 = new TimeSpanValueWriter();
    object box = (object)(default(TimeSpan));
    s = this.Write(s0, box);
    PexAssert.AreEqual<string>("time\'PT0S\'", s);
    PexAssert.IsNotNull((object)s0);
}
	}
}
