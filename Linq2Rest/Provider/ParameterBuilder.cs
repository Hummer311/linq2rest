// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

namespace Linq2Rest.Provider
{
    using System;
    using System.Collections.Generic;
#if !SILVERLIGHT
    using System.Diagnostics.Contracts;
#endif
    using System.Linq;
#if !NETFX_CORE && !SILVERLIGHT
	using System.Web;
#endif

	internal class ParameterBuilder
    {
        private readonly Uri _serviceBase;

        public ParameterBuilder(Uri serviceBase)
        {
#if !SILVERLIGHT
            Contract.Requires(serviceBase != null);
#endif
            _serviceBase = serviceBase;
            OrderByParameter = new List<string>();
        }

        public string FilterParameter { get; set; }

        public IList<string> OrderByParameter { get; private set; }

        public string SelectParameter { get; set; }

        public string SkipParameter { get; set; }

        public string TakeParameter { get; set; }

        public Uri GetFullUri()
        {
            var parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(FilterParameter))
			{
#if !SILVERLIGHT && !NETFX_CORE
				parameters.Add(BuildParameter(StringConstants.FilterParameter, HttpUtility.UrlEncode(FilterParameter)));
#else
				parameters.Add(BuildParameter(StringConstants.FilterParameter, FilterParameter));
#endif
            }

            if (!string.IsNullOrWhiteSpace(SelectParameter))
            {
                parameters.Add(BuildParameter(StringConstants.SelectParameter, SelectParameter));
            }

            if (!string.IsNullOrWhiteSpace(SkipParameter))
            {
                parameters.Add(BuildParameter(StringConstants.SkipParameter, SkipParameter));
            }

            if (!string.IsNullOrWhiteSpace(TakeParameter))
            {
                parameters.Add(BuildParameter(StringConstants.TopParameter, TakeParameter));
            }

            if (OrderByParameter.Any())
            {
                parameters.Add(BuildParameter(StringConstants.OrderByParameter, string.Join(",", OrderByParameter)));
            }

            var builder = new UriBuilder(_serviceBase);
            builder.Query = (string.IsNullOrEmpty(builder.Query) ? string.Empty : "&") + string.Join("&", parameters);

            return builder.Uri;
        }

        private string BuildParameter(string name, string value)
        {
            return string.Format(name + "=" + value);
        }

#if !SILVERLIGHT
        [ContractInvariantMethod]
        private void Invariants()
        {
            Contract.Invariant(_serviceBase != null);
            Contract.Invariant(OrderByParameter != null);
        }
#endif
    }
}