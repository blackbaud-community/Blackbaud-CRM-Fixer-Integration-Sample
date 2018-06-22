using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Blackbaud.CustomFx.Currency.Catalog.Fixer
{
    internal sealed class Service
    {
        private string _accessKey;

        // The Free Edition of Fixer does not come with secure encryption, so we must use the HTTP version of the API
        private const string BASE_URL = "http://data.fixer.io/api/";

        public Service(string accessKey)
        {
            if (string.IsNullOrEmpty(accessKey))
            {
                throw new ArgumentNullException("accessKey");
            }

            _accessKey = accessKey;
        }

        public Response.SupportedSymbols GetSupportedCurrencies()
        {
            // Get response from Fixer service
            string content = GetResponse(BASE_URL + "symbols?access_key=" + _accessKey);

            // Substitute to cut down on API calls
            //string content = "{\"success\":true,\"symbols\":{\"AED\":\"United Arab Emirates Dirham\",\"AFN\":\"Afghan Afghani\",\"ALL\":\"Albanian Lek\",\"AMD\":\"Armenian Dram\",\"ANG\":\"Netherlands Antillean Guilder\",\"AOA\":\"Angolan Kwanza\",\"ARS\":\"Argentine Peso\",\"AUD\":\"Australian Dollar\",\"AWG\":\"Aruban Florin\",\"AZN\":\"Azerbaijani Manat\",\"BAM\":\"Bosnia-Herzegovina Convertible Mark\",\"BBD\":\"Barbadian Dollar\",\"BDT\":\"Bangladeshi Taka\",\"BGN\":\"Bulgarian Lev\",\"BHD\":\"Bahraini Dinar\",\"BIF\":\"Burundian Franc\",\"BMD\":\"Bermudan Dollar\",\"BND\":\"Brunei Dollar\",\"BOB\":\"Bolivian Boliviano\",\"BRL\":\"Brazilian Real\",\"BSD\":\"Bahamian Dollar\",\"BTC\":\"Bitcoin\",\"BTN\":\"Bhutanese Ngultrum\",\"BWP\":\"Botswanan Pula\",\"BYN\":\"New Belarusian Ruble\",\"BYR\":\"Belarusian Ruble\",\"BZD\":\"Belize Dollar\",\"CAD\":\"Canadian Dollar\",\"CDF\":\"Congolese Franc\",\"CHF\":\"Swiss Franc\",\"CLF\":\"Chilean Unit of Account (UF)\",\"CLP\":\"Chilean Peso\",\"CNY\":\"Chinese Yuan\",\"COP\":\"Colombian Peso\",\"CRC\":\"Costa Rican Col\u00f3n\",\"CUC\":\"Cuban Convertible Peso\",\"CUP\":\"Cuban Peso\",\"CVE\":\"Cape Verdean Escudo\",\"CZK\":\"Czech Republic Koruna\",\"DJF\":\"Djiboutian Franc\",\"DKK\":\"Danish Krone\",\"DOP\":\"Dominican Peso\",\"DZD\":\"Algerian Dinar\",\"EGP\":\"Egyptian Pound\",\"ERN\":\"Eritrean Nakfa\",\"ETB\":\"Ethiopian Birr\",\"EUR\":\"Euro\",\"FJD\":\"Fijian Dollar\",\"FKP\":\"Falkland Islands Pound\",\"GBP\":\"British Pound Sterling\",\"GEL\":\"Georgian Lari\",\"GGP\":\"Guernsey Pound\",\"GHS\":\"Ghanaian Cedi\",\"GIP\":\"Gibraltar Pound\",\"GMD\":\"Gambian Dalasi\",\"GNF\":\"Guinean Franc\",\"GTQ\":\"Guatemalan Quetzal\",\"GYD\":\"Guyanaese Dollar\",\"HKD\":\"Hong Kong Dollar\",\"HNL\":\"Honduran Lempira\",\"HRK\":\"Croatian Kuna\",\"HTG\":\"Haitian Gourde\",\"HUF\":\"Hungarian Forint\",\"IDR\":\"Indonesian Rupiah\",\"ILS\":\"Israeli New Sheqel\",\"IMP\":\"Manx pound\",\"INR\":\"Indian Rupee\",\"IQD\":\"Iraqi Dinar\",\"IRR\":\"Iranian Rial\",\"ISK\":\"Icelandic Kr\u00f3na\",\"JEP\":\"Jersey Pound\",\"JMD\":\"Jamaican Dollar\",\"JOD\":\"Jordanian Dinar\",\"JPY\":\"Japanese Yen\",\"KES\":\"Kenyan Shilling\",\"KGS\":\"Kyrgystani Som\",\"KHR\":\"Cambodian Riel\",\"KMF\":\"Comorian Franc\",\"KPW\":\"North Korean Won\",\"KRW\":\"South Korean Won\",\"KWD\":\"Kuwaiti Dinar\",\"KYD\":\"Cayman Islands Dollar\",\"KZT\":\"Kazakhstani Tenge\",\"LAK\":\"Laotian Kip\",\"LBP\":\"Lebanese Pound\",\"LKR\":\"Sri Lankan Rupee\",\"LRD\":\"Liberian Dollar\",\"LSL\":\"Lesotho Loti\",\"LTL\":\"Lithuanian Litas\",\"LVL\":\"Latvian Lats\",\"LYD\":\"Libyan Dinar\",\"MAD\":\"Moroccan Dirham\",\"MDL\":\"Moldovan Leu\",\"MGA\":\"Malagasy Ariary\",\"MKD\":\"Macedonian Denar\",\"MMK\":\"Myanma Kyat\",\"MNT\":\"Mongolian Tugrik\",\"MOP\":\"Macanese Pataca\",\"MRO\":\"Mauritanian Ouguiya\",\"MUR\":\"Mauritian Rupee\",\"MVR\":\"Maldivian Rufiyaa\",\"MWK\":\"Malawian Kwacha\",\"MXN\":\"Mexican Peso\",\"MYR\":\"Malaysian Ringgit\",\"MZN\":\"Mozambican Metical\",\"NAD\":\"Namibian Dollar\",\"NGN\":\"Nigerian Naira\",\"NIO\":\"Nicaraguan C\u00f3rdoba\",\"NOK\":\"Norwegian Krone\",\"NPR\":\"Nepalese Rupee\",\"NZD\":\"New Zealand Dollar\",\"OMR\":\"Omani Rial\",\"PAB\":\"Panamanian Balboa\",\"PEN\":\"Peruvian Nuevo Sol\",\"PGK\":\"Papua New Guinean Kina\",\"PHP\":\"Philippine Peso\",\"PKR\":\"Pakistani Rupee\",\"PLN\":\"Polish Zloty\",\"PYG\":\"Paraguayan Guarani\",\"QAR\":\"Qatari Rial\",\"RON\":\"Romanian Leu\",\"RSD\":\"Serbian Dinar\",\"RUB\":\"Russian Ruble\",\"RWF\":\"Rwandan Franc\",\"SAR\":\"Saudi Riyal\",\"SBD\":\"Solomon Islands Dollar\",\"SCR\":\"Seychellois Rupee\",\"SDG\":\"Sudanese Pound\",\"SEK\":\"Swedish Krona\",\"SGD\":\"Singapore Dollar\",\"SHP\":\"Saint Helena Pound\",\"SLL\":\"Sierra Leonean Leone\",\"SOS\":\"Somali Shilling\",\"SRD\":\"Surinamese Dollar\",\"STD\":\"S\u00e3o Tom\u00e9 and Pr\u00edncipe Dobra\",\"SVC\":\"Salvadoran Col\u00f3n\",\"SYP\":\"Syrian Pound\",\"SZL\":\"Swazi Lilangeni\",\"THB\":\"Thai Baht\",\"TJS\":\"Tajikistani Somoni\",\"TMT\":\"Turkmenistani Manat\",\"TND\":\"Tunisian Dinar\",\"TOP\":\"Tongan Pa\u02bbanga\",\"TRY\":\"Turkish Lira\",\"TTD\":\"Trinidad and Tobago Dollar\",\"TWD\":\"New Taiwan Dollar\",\"TZS\":\"Tanzanian Shilling\",\"UAH\":\"Ukrainian Hryvnia\",\"UGX\":\"Ugandan Shilling\",\"USD\":\"United States Dollar\",\"UYU\":\"Uruguayan Peso\",\"UZS\":\"Uzbekistan Som\",\"VEF\":\"Venezuelan Bol\u00edvar Fuerte\",\"VND\":\"Vietnamese Dong\",\"VUV\":\"Vanuatu Vatu\",\"WST\":\"Samoan Tala\",\"XAF\":\"CFA Franc BEAC\",\"XAG\":\"Silver (troy ounce)\",\"XAU\":\"Gold (troy ounce)\",\"XCD\":\"East Caribbean Dollar\",\"XDR\":\"Special Drawing Rights\",\"XOF\":\"CFA Franc BCEAO\",\"XPF\":\"CFP Franc\",\"YER\":\"Yemeni Rial\",\"ZAR\":\"South African Rand\",\"ZMK\":\"Zambian Kwacha (pre-2013)\",\"ZMW\":\"Zambian Kwacha\",\"ZWL\":\"Zimbabwean Dollar\"}}";
            //string content = "{\"success\":false,\"error\":{\"code\":101,\"type\":\"missing_access_key\",\"info\":\"You have not supplied an API Access Key. [Required format: access_key=YOUR_ACCESS_KEY]\"}}";

            // Convert JSON to .NET object
            Response.SupportedSymbols symbols = JsonConvert.DeserializeObject<Response.SupportedSymbols>(content);

            // Return results
            if (symbols.Success)
            {
                return symbols;
            }
            else
            {
                throw new Exception(symbols.Error.Info);
            }
        }

        public Response.ExchangeRates GetLatestRates(IList<string> symbols)
        {
            // Initialize URL
            string url = BASE_URL + "latest?access_key=" + _accessKey;

            // Add symbols, if provided
            if (symbols != null && symbols.Count > 0)
            {
                url += ("&symbols=" + string.Join(",", symbols));
            }

            // Get response from Fixer service
            string content = GetResponse(url);

            // Substitute to cut down on API calls
            //string content = "{\"success\":true,\"timestamp\":1528382707,\"base\":\"EUR\",\"date\":\"2018-06-07\",\"rates\":{\"CAD\":1.534346,\"EUR\":1,\"GBP\":0.882646,\"MXN\":24.17604,\"USD\":1.182174}}";
            //string content = "{\"success\":true,\"timestamp\":1526585287,\"base\":\"EUR\",\"date\":\"2018-05-17\",\"rates\":{\"AED\":4.334055,\"AFN\":83.950361,\"ALL\":126.98761,\"AMD\":569.80982,\"ANG\":2.112138,\"AOA\":274.619412,\"ARS\":28.675762,\"AUD\":1.5715,\"AWG\":2.100529,\"AZN\":2.005536,\"BAM\":1.959152,\"BBD\":2.360145,\"BDT\":98.547862,\"BGN\":1.95514,\"BHD\":0.444768,\"BIF\":2066.283431,\"BMD\":1.180073,\"BND\":1.567847,\"BOB\":8.09494,\"BRL\":4.370276,\"BSD\":1.180073,\"BTC\":0.000145,\"BTN\":80.067921,\"BWP\":11.677949,\"BYN\":2.348078,\"BYR\":23129.422215,\"BZD\":2.357546,\"CAD\":1.510835,\"CDF\":1847.401391,\"CHF\":1.181678,\"CLF\":0.027283,\"CLP\":746.254293,\"CNY\":7.510451,\"COP\":3425.86877,\"CRC\":663.059177,\"CUC\":1.180073,\"CUP\":31.271923,\"CVE\":110.313187,\"CZK\":25.595775,\"DJF\":209.168068,\"DKK\":7.450199,\"DOP\":58.307018,\"DZD\":136.575699,\"EGP\":20.958414,\"ERN\":17.689783,\"ETB\":32.097975,\"EUR\":1,\"FJD\":2.430597,\"FKP\":0.873727,\"GBP\":0.873348,\"GEL\":2.857549,\"GGP\":0.873396,\"GHS\":5.441899,\"GIP\":0.873958,\"GMD\":55.31,\"GNF\":10620.653185,\"GTQ\":8.657064,\"GYD\":244.84145,\"HKD\":9.262944,\"HNL\":28.046839,\"HRK\":7.375925,\"HTG\":74.427251,\"HUF\":317.215303,\"IDR\":16576.479278,\"ILS\":4.236817,\"IMP\":0.873396,\"INR\":80.027326,\"IQD\":1397.205913,\"IRR\":49563.047436,\"ISK\":123.081572,\"JEP\":0.873396,\"JMD\":146.753825,\"JOD\":0.836082,\"JPY\":130.74139,\"KES\":118.184269,\"KGS\":80.812436,\"KHR\":4779.293944,\"KMF\":490.709558,\"KPW\":1062.065336,\"KRW\":1275.693876,\"KWD\":0.356024,\"KYD\":0.967587,\"KZT\":386.686169,\"LAK\":9811.123545,\"LBP\":1776.835199,\"LKR\":186.215454,\"LRD\":157.610489,\"LSL\":14.876998,\"LTL\":3.597682,\"LVL\":0.732294,\"LYD\":1.598405,\"MAD\":11.122773,\"MDL\":19.565648,\"MGA\":3788.032687,\"MKD\":61.245768,\"MMK\":1589.5581,\"MNT\":2825.094036,\"MOP\":9.540419,\"MRO\":417.745517,\"MUR\":40.417388,\"MVR\":18.373812,\"MWK\":841.922784,\"MXN\":23.260381,\"MYR\":4.681385,\"MZN\":70.237524,\"NAD\":14.880957,\"NGN\":422.465865,\"NIO\":37.006487,\"NOK\":9.58783,\"NPR\":127.38883,\"NZD\":1.716058,\"OMR\":0.454088,\"PAB\":1.180073,\"PEN\":3.861785,\"PGK\":3.84715,\"PHP\":61.681794,\"PKR\":136.310184,\"PLN\":4.297115,\"PYG\":6623.039131,\"QAR\":4.295229,\"RON\":4.636151,\"RSD\":117.563901,\"RUB\":73.379743,\"RWF\":998.825185,\"SAR\":4.425306,\"SBD\":9.230763,\"SCR\":15.756312,\"SDG\":21.188439,\"SEK\":10.316808,\"SGD\":1.583917,\"SHP\":0.873962,\"SLL\":9204.566045,\"SOS\":664.380494,\"SRD\":8.732651,\"STD\":24511.877222,\"SVC\":10.325347,\"SYP\":607.713744,\"SZL\":14.879301,\"THB\":37.844519,\"TJS\":10.586197,\"TMT\":4.012247,\"TND\":2.972136,\"TOP\":2.686315,\"TRY\":5.264068,\"TTD\":7.846891,\"TWD\":35.334914,\"TZS\":2684.665146,\"UAH\":30.864799,\"UGX\":4378.068963,\"USD\":1.180073,\"UYU\":36.251828,\"UZS\":9440.580787,\"VEF\":82398.566938,\"VND\":26872.612382,\"VUV\":126.704395,\"WST\":3.032199,\"XAF\":655.565745,\"XAG\":0.071796,\"XAU\":0.000914,\"XCD\":3.186538,\"XDR\":0.824027,\"XOF\":655.565745,\"XPF\":119.369641,\"YER\":294.900126,\"ZAR\":14.881421,\"ZMK\":10622.075213,\"ZMW\":11.930429,\"ZWL\":380.402304}}";
            //string content = "{\"success\":false,\"error\":{\"code\":101,\"type\":\"missing_access_key\",\"info\":\"You have not supplied an API Access Key. [Required format: access_key=YOUR_ACCESS_KEY]\"}}";

            // Convert JSON to .NET object
            Response.ExchangeRates rates = JsonConvert.DeserializeObject<Response.ExchangeRates>(content);

            // Return results
            if (rates.Success)
            {
                return rates;
            }
            else
            {
                throw new Exception(rates.Error.Info);
            }
        }

        public Response.ExchangeRates GetHistoricalRates(DateTime date, IList<string> symbols)
        {
            // Initialize URL
            string url = BASE_URL + date.ToString("yyyy-MM-dd") + "?access_key=" + _accessKey;

            // Add symbols, if provided
            if (symbols != null && symbols.Count > 0)
            {
                url += ("&symbols=" + string.Join(",", symbols));
            }

            // Get response from Fixer service
            string content = GetResponse(url);

            // Substitute to cut down on API calls
            //string content = "{\"success\":true,\"timestamp\":1495065599,\"historical\":true,\"base\":\"EUR\",\"date\":\"2017-05-17\",\"rates\":{\"AED\":4.097632,\"AFN\":75.954236,\"ALL\":135.047807,\"AMD\":539.343194,\"ANG\":1.997818,\"AOA\":184.208644,\"ARS\":17.392328,\"AUD\":1.500668,\"AWG\":1.997319,\"AZN\":1.898796,\"BAM\":1.958934,\"BBD\":2.231642,\"BDT\":89.957488,\"BGN\":1.963063,\"BHD\":0.41977,\"BIF\":1899.015688,\"BMD\":1.115821,\"BND\":1.551436,\"BOB\":7.688357,\"BRL\":3.499662,\"BSD\":1.115821,\"BTC\":0.00062,\"BTN\":71.468335,\"BWP\":11.485927,\"BYN\":2.10869,\"BYR\":22338.735259,\"BZD\":2.22908,\"CAD\":1.517338,\"CDF\":1573.195973,\"CHF\":1.09249,\"CLF\":0.027862,\"CLP\":748.648905,\"CNY\":7.673726,\"COP\":3227.735184,\"CRC\":628.5866,\"CUC\":1.115821,\"CUP\":1.115564,\"CVE\":110.298898,\"CZK\":26.491822,\"DJF\":198.259057,\"DKK\":7.439189,\"DOP\":52.510535,\"DZD\":120.795432,\"EGP\":20.140567,\"ERN\":17.060737,\"ETB\":25.573393,\"EUR\":1,\"FJD\":2.349941,\"FKP\":0.860407,\"GBP\":0.860476,\"GEL\":2.679642,\"GGP\":0.860318,\"GHS\":4.809079,\"GIP\":0.860634,\"GMD\":50.078045,\"GNF\":10127.972162,\"GTQ\":8.188451,\"GYD\":228.196535,\"HKD\":8.687333,\"HNL\":26.054471,\"HRK\":7.399904,\"HTG\":74.72653,\"HUF\":308.792276,\"IDR\":14851.576738,\"ILS\":4.022556,\"IMP\":0.860318,\"INR\":71.736129,\"IQD\":1317.784533,\"IRR\":36201.694357,\"ISK\":113.501307,\"JEP\":0.860318,\"JMD\":143.639625,\"JOD\":0.790562,\"JPY\":124.001178,\"KES\":115.07461,\"KGS\":75.509831,\"KHR\":4476.004017,\"KMF\":502.28679,\"KPW\":1004.239287,\"KRW\":1251.359679,\"KWD\":0.338759,\"KYD\":0.91449,\"KZT\":349.497437,\"LAK\":9139.465954,\"LBP\":1679.979983,\"LKR\":170.251962,\"LRD\":101.539736,\"LSL\":14.717695,\"LTL\":3.401802,\"LVL\":0.692423,\"LYD\":1.56159,\"MAD\":10.907482,\"MDL\":20.44742,\"MGA\":3456.813044,\"MKD\":61.24741,\"MMK\":1514.168876,\"MNT\":2691.360044,\"MOP\":8.947439,\"MRO\":398.415023,\"MUR\":38.607402,\"MVR\":17.3286,\"MWK\":800.791196,\"MXN\":21.141684,\"MYR\":4.825701,\"MZN\":78.598429,\"NAD\":14.793536,\"NGN\":351.483408,\"NIO\":32.805597,\"NOK\":9.382024,\"NPR\":114.036897,\"NZD\":1.606676,\"OMR\":0.429036,\"PAB\":1.115821,\"PEN\":3.652085,\"PGK\":3.545188,\"PHP\":55.545565,\"PKR\":116.759502,\"PLN\":4.191141,\"PYG\":6199.501449,\"QAR\":4.062813,\"RON\":4.56304,\"RSD\":123.430881,\"RUB\":63.692176,\"RWF\":914.136307,\"SAR\":4.183881,\"SBD\":8.852947,\"SCR\":15.030432,\"SDG\":7.431146,\"SEK\":9.746607,\"SGD\":1.551806,\"SHP\":0.860635,\"SLL\":8290.549414,\"SOS\":612.585287,\"SRD\":8.335458,\"STD\":24506.997642,\"SVC\":9.732415,\"SYP\":574.625446,\"SZL\":14.740075,\"THB\":38.428872,\"TJS\":9.482802,\"TMT\":3.804949,\"TND\":2.67741,\"TOP\":2.578884,\"TRY\":3.99263,\"TTD\":7.475445,\"TWD\":33.60185,\"TZS\":2488.280979,\"UAH\":29.446514,\"UGX\":4030.345281,\"USD\":1.115821,\"UYU\":31.276462,\"UZS\":4212.223844,\"VEF\":11.12975,\"VND\":25249.912096,\"VUV\":120.051171,\"WST\":2.888634,\"XAF\":655.522465,\"XAG\":0.065929,\"XAU\":0.000886,\"XCD\":3.014453,\"XDR\":0.810122,\"XOF\":657.999623,\"XPF\":119.203388,\"YER\":278.899441,\"ZAR\":14.789984,\"ZMK\":10.271102,\"ZMW\":10.22073,\"ZWL\":359.690472}}";
            //string content = "{\"success\":false,\"error\":{\"code\":101,\"type\":\"missing_access_key\",\"info\":\"You have not supplied an API Access Key. [Required format: access_key=YOUR_ACCESS_KEY]\"}}";

            // Convert JSON to .NET object
            Response.ExchangeRates rates = JsonConvert.DeserializeObject<Response.ExchangeRates>(content);

            // Return results
            if (rates.Success)
            {
                return rates;
            }
            else
            {
                throw new Exception(rates.Error.Info);
            }
        }

        private static string GetResponse(string url)
        {
            // Send request to Fixer service
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Read and parse response
            string content = string.Empty;
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
            {
                content = sr.ReadToEnd();
            }

            return content;
        }
    }
}
