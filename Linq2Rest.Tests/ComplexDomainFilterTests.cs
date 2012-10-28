// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexDomainFilterTests.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ComplexDomainFilterTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Specialized;
using System.Linq;
using Linq2Rest.Tests.Fakes.ComplexDomain;

namespace Linq2Rest.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ComplexDomainFilterTests
	{
		private TypeInstanceData[] _model;

		[SetUp]
		public void Setup()
		{
			_model = new[]
				         {
					         new TypeInstanceData
						         {
							         UserDefinedId = "OneInstance",
							         DefinitionFullName = "All\\Hardware\\CPU\\SNB",
							         Properties = new[]
								                      {
									                      CreatePropertyInstanceData("Status", "Approved"),
									                      CreatePropertyInstanceData("Frequency", "20"),
								                      },
							         IsDeleted = false
						         },
					         new TypeInstanceData
						         {
							         UserDefinedId = "OneInstance",
							         DefinitionFullName = "All\\Hardware\\CPU\\SNB",
							         Properties = new[]
								                      {
									                      CreatePropertyInstanceData("Status", "Approved"),
									                      CreatePropertyInstanceData("Frequency", "30"),
								                      },
							         IsDeleted = false
						         },
					         new TypeInstanceData
						         {
							         UserDefinedId = "SecondInstance",
							         DefinitionFullName = "All\\Hardware\\CPU\\SNB",
							         Properties = new[]
								                      {
									                      CreatePropertyInstanceData("Status", "Not Approved")
								                      },
							         IsDeleted = false
						         }
				         };
		}

		[TestCase("Properties/any(pi: pi/DefinitionName eq 'Status' and pi/DefinitionName eq 'Approved')", 0)]
		[TestCase("Properties/any(pi: pi/Values/any(v: v/StringNonUnicodeValue eq 'AAAAAApproved'))", 0)]
		[TestCase("Properties/any(pi: pi/DefinitionName eq 'Status' and pi/Values/any(c: c/StringNonUnicodeValue eq 'Approved'))", 2)]
		[TestCase("Properties/any(pi: pi/DefinitionName eq 'Status' and pi/Values/any(c: c/StringNonUnicodeValue eq 'Approved')) and Properties/any(pi: pi/DefinitionName eq 'Frequency' and pi/Values/any(c: c/StringNonUnicodeValue eq '20'))", 1)]
		[TestCase("Properties/any(pi: pi/Values/any(c: c/StringNonUnicodeValue eq 'Approved') and pi/DefinitionName eq 'Status')", 2)]
		public void DomainTest(string filter, int result)
		{
			var nv = new NameValueCollection { { "$filter", filter } };
			var list = _model.AsQueryable().Filter(nv).ToList();

			Assert.AreEqual(result, list.Count);
		}

		private PropertyInstanceData CreatePropertyInstanceData(string definitionName, string value)
		{
			return new PropertyInstanceData
				       {
					       DefinitionName = definitionName,
					       Values = new[]
						                {
							                new PropertyInstanceValueData()
								                {
									                ValueType = ValueTypeDefinitionData.StringNonUnicode,
									                StringNonUnicodeValue = value
								                }
						                }
				       };
		}
	}
}