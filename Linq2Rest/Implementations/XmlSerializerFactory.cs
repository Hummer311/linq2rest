// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Implementations
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using Linq2Rest.Provider;

	/// <summary>
	/// Defines the XmlSerializer factory.
	/// </summary>
	public class XmlSerializerFactory : ISerializerFactory
	{
		private readonly IEnumerable<Type> _knownTypes;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlSerializerFactory"/> class.
		/// </summary>
		/// <param name="knownTypes">A number of known types for serialization resolution.</param>
		public XmlSerializerFactory(IEnumerable<Type> knownTypes)
		{
			Contract.Requires<ArgumentNullException>(knownTypes != null);

			_knownTypes = knownTypes;
		}

		/// <summary>
		/// Creates an instance of an <see cref="ISerializer{T}"/>.
		/// </summary>
		/// <typeparam name="T">The item type for the serializer.</typeparam>
		/// <returns>An instance of an <see cref="ISerializer{T}"/>.</returns>
		public ISerializer<T> Create<T>()
		{
			return new XmlSerializer<T>(_knownTypes);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			Contract.Invariant(_knownTypes != null);
		}

		private class XmlSerializer<T> : ISerializer<T>
		{
			private readonly XmlSerializer _serializer;
			private readonly XmlSerializer _listSerializer;

			public XmlSerializer(IEnumerable<Type> knownTypes)
			{
				Contract.Requires(knownTypes != null);

				var array = knownTypes.ToArray();
				_serializer = new XmlSerializer(typeof(T), array);
				_listSerializer = new XmlSerializer(typeof(List<T>), array);
			}

			public T Deserialize(string input)
			{
				using (var reader = new StringReader(input))
				{
					var result = (T)_serializer.Deserialize(XmlReader.Create(reader));

					return result;
				}
			}

			public IList<T> DeserializeList(string input)
			{
				using (var reader = new StringReader(input))
				{
					var result = (List<T>)_listSerializer.Deserialize(XmlReader.Create(reader));

					return result;
				}
			}

			[ContractInvariantMethod]
			private void Invariants()
			{
				Contract.Invariant(_serializer != null);
				Contract.Invariant(_listSerializer != null);
			}
		}
	}
}