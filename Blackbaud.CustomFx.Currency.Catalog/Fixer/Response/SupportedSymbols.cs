using System.Collections.Generic;

namespace Blackbaud.CustomFx.Currency.Catalog.Fixer.Response
{
    internal class SupportedSymbols
    {
        public bool Success;
        public Error Error;
        public IDictionary<string, string> Symbols;
    }
}
