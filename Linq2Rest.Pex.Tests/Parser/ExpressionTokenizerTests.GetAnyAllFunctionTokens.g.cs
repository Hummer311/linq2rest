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

namespace Linq2Rest.Parser
{
	public partial class ExpressionTokenizerTests
	{
[Test]
[PexGeneratedBy(typeof(ExpressionTokenizerTests))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void GetAnyAllFunctionTokensThrowsContractException619()
{
    try
    {
      TokenSet tokenSet;
      tokenSet = this.GetAnyAllFunctionTokens((string)null);
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
[PexGeneratedBy(typeof(ExpressionTokenizerTests))]
public void GetAnyAllFunctionTokens404()
{
    TokenSet tokenSet;
    tokenSet = this.GetAnyAllFunctionTokens("");
    PexAssert.IsNull((object)tokenSet);
}
[Test]
[PexGeneratedBy(typeof(ExpressionTokenizerTests))]
public void GetAnyAllFunctionTokens730()
{
    TokenSet tokenSet;
    tokenSet = this.GetAnyAllFunctionTokens("\0");
    PexAssert.IsNull((object)tokenSet);
}
	}
}
