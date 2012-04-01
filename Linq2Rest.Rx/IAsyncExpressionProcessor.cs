namespace Linq2Rest.Rx
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using Linq2Rest.Provider;

	internal interface IAsyncExpressionProcessor
	{
		Task<IList<T>> ProcessMethodCall<T>(
			MethodCallExpression methodCall,
			ParameterBuilder builder,
			Func<ParameterBuilder, Task<IList<T>>> resultLoader,
			Func<Type, ParameterBuilder, Task<IEnumerable>> intermediateResultLoader);
	}
}