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
using System.Linq.Expressions;
using Microsoft.Pex.Framework;
using NUnit.Framework;
using Microsoft.Pex.Framework.Generated;

namespace Linq2Rest
{
	public partial class MethodProviderTests
	{
[Test]
[PexGeneratedBy(typeof(MethodProviderTests))]
public void IgnoreCaseExpressionGet639()
{
    ConstantExpression constantExpression;
    constantExpression = this.IgnoreCaseExpressionGet();
    PexAssert.IsNotNull((object)constantExpression);
    PexAssert.AreEqual<ExpressionType>
        (ExpressionType.Constant, constantExpression.NodeType);
    PexAssert.IsNotNull(constantExpression.Value);
    PexAssert.AreEqual<StringComparison>(StringComparison.OrdinalIgnoreCase, 
                                         (StringComparison)(constantExpression.Value));
}
	}
}
