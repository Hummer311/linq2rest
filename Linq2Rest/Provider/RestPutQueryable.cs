// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestPutQueryable.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the RestPutQueryable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq.Expressions;
	using Linq2Rest.Provider.Writers;

	internal class RestPutQueryable<T> : RestQueryableBase<T>
	{
		private readonly RestPutQueryProvider<T> _restPutQueryProvider;

		public RestPutQueryable(IRestClient client, ISerializerFactory serializerFactory, Expression expression, Stream inputData, Type sourceType)
			: this(client, serializerFactory, new MemberNameResolver(), new IntValueWriter[0], expression, inputData, sourceType)
		{
		}

		public RestPutQueryable(IRestClient client, ISerializerFactory serializerFactory, IMemberNameResolver memberNameResolver, IEnumerable<IValueWriter> valueWriters, Expression expression, Stream inputData, Type sourceType)
			: base(client, serializerFactory, memberNameResolver, valueWriters)
		{
			Contract.Requires<ArgumentNullException>(client != null);
			Contract.Requires<ArgumentNullException>(serializerFactory != null);
			Contract.Requires<ArgumentNullException>(memberNameResolver != null);
			Contract.Requires<ArgumentNullException>(expression != null);
			Contract.Requires<ArgumentNullException>(valueWriters != null);
			Contract.Requires<ArgumentNullException>(inputData != null);

			_restPutQueryProvider = new RestPutQueryProvider<T>(
				client,
				serializerFactory,
				new ExpressionProcessor(new ExpressionWriter(memberNameResolver, ValueWriters), memberNameResolver),
				memberNameResolver,
				ValueWriters,
				inputData,
				sourceType);
			Provider = _restPutQueryProvider;
			Expression = expression;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					_restPutQueryProvider.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_restPutQueryProvider != null);
		}
	}
}