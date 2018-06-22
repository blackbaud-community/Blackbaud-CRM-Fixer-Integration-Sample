using System;
using System.Collections.Generic;

namespace Blackbaud.CustomFx.Currency.Catalog.Fixer.Response
{
    internal class ExchangeRates
    {
        public bool Success;
        public Error Error;
        public bool Historical;
        public long Timestamp;
        public string Base;
        public DateTime Date;
        public IDictionary<string, decimal> Rates;
    }
}
