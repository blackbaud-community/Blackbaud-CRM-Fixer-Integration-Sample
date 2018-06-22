using Blackbaud.AppFx.Server;
using Blackbaud.AppFx.Server.AppCatalog;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Blackbaud.CustomFx.Currency.Catalog.Fixer;
using Blackbaud.CustomFx.Currency.Catalog.Fixer.Response;

namespace Blackbaud.CustomFx.Currency.Catalog
{
    public sealed class ExchangeRateRefreshBusinessProcess : AppBusinessProcess
    {
        private const string EUR_SYMBOL = "EUR";
        
        private Service _service;

        private string _accessKey;
        private Common.DateCode _dateCode;
        private DateTime _date;

        public override AppBusinessProcessResult StartBusinessProcess()
        {
            AppBusinessProcessResult result = new AppBusinessProcessResult()
            {
                NumberSuccessfullyProcessed = 0,
                NumberOfExceptions = 0
            };
            
            using (SqlConnection conn = this.RequestContext.OpenAppDBConnection(RequestContext.ConnectionContext.BusinessProcess))
            {
                this.UpdateProcessStatus("Retrieving list of active currencies...");
                Dictionary<string, Currency> currencies = (Dictionary<string, Currency>)LoadActiveCurrencies(conn);

                // There are no active currencies in CRM, therefore there is nothing to do
                if (currencies.Count == 0)
                {
                    return result;
                }

                // The Free Edition of Fixer only supports the Euro as the base currency. If the Euro is not present, there is nothing to do.
                if (!currencies.ContainsKey(EUR_SYMBOL))
                {
                    return result;
                }
                
                this.UpdateProcessStatus("Retrieving process properties...");
                LoadProperties(conn);

                // Initialize third party service
                _service = new Fixer.Service(_accessKey);

                this.UpdateProcessStatus("Retrieving list of currencies supported by the rate provider...");
                Dictionary<string, string> supportedCurrencies = (Dictionary<string, string>)_service.GetSupportedCurrencies().Symbols;

                // Remove currencies not supported by the rate provider
                foreach (var r in currencies.Where(c => !supportedCurrencies.ContainsKey(c.Key)).ToList())
                {
                    currencies.Remove(r.Key);
                }

                // There are no supported currencies in CRM, therefore there is nothing to do
                if (currencies.Count == 0)
                {
                    return result;
                }

                // Get exchange rates
                ExchangeRates rateResponse;
                this.UpdateProcessStatus("Retrieving currency exchange rates from the rate provider...");
                switch (_dateCode)
                {
                    case Common.DateCode.Latest:
                        rateResponse = _service.GetLatestRates(currencies.Keys.ToList<string>());
                        break;
                    case Common.DateCode.Historical:
                        rateResponse = _service.GetHistoricalRates(_date, currencies.Keys.ToList<string>());
                        break;
                    default:
                        throw new ServiceException("Invalid process date code encountered.");
                }

                // Prepare to add the rates
                Guid baseCurrencyId = currencies[rateResponse.Base].Id;
                DateTime date = rateResponse.Date.AddTicks(rateResponse.Timestamp);
                Guid timeZoneId = GetTimeZoneId(conn);

                // Add rates, except the one that goes from base to base. Fixer includes this in the output.
                foreach(var r in rateResponse.Rates.Where(r => currencies[r.Key].Id != baseCurrencyId))
                {
                    // Create request
                    DataFormSaveRequest request = new DataFormSaveRequest()
                    {
                        FormID = new Guid("9b16843e-c20a-4ddf-b31b-1d179380476e"), // CurrencyExchangeRate.Add.xml
                        SecurityContext = new RequestSecurityContext()
                        {
                            SecurityFeatureID = new Guid("37e7492f-9a86-4029-b125-d133f330bf90"), // ExchangeRateRefresh.BusinessProcess.xml
                            SecurityFeatureType = SecurityFeatureType.BusinessProcess
                        },
                        DataFormItem = new AppFx.XmlTypes.DataForms.DataFormItem()
                    };

                    // Fill out form
                    request.DataFormItem.SetValue("FROMCURRENCYID", baseCurrencyId);
                    request.DataFormItem.SetValue("TOCURRENCYID", currencies[r.Key].Id);
                    request.DataFormItem.SetValue("TYPECODE", 1); // Daily
                    request.DataFormItem.SetValue("RATE", r.Value);
                    request.DataFormItem.SetValue("ASOFDATETIME", date);
                    request.DataFormItem.SetValue("TIMEZONEENTRYID", timeZoneId);

                    // Save the form
                    try
                    {
                        ServiceMethods.DataFormSave(request, this.RequestContext);
                        result.NumberSuccessfullyProcessed++;
                    }
                    catch
                    {
                        result.NumberOfExceptions++;
                    }
                }
            }

            return result;
        }

        private void LoadProperties(SqlConnection conn)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select");
            sql.AppendLine("  FIXERAPIACCESSKEY,");
            sql.AppendLine("  DATECODE,");
            sql.AppendLine("  HISTORICALDATE");
            sql.AppendLine("from dbo.USR_EXCHANGERATEREFRESHPROCESS");
            sql.AppendLine("where ID = @ID;");

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = this.ProcessCommandTimeout;
                cmd.CommandText = sql.ToString();

                cmd.Parameters.AddWithValue("@ID", this.ProcessContext.ParameterSetID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        _accessKey = reader.GetString(0);
                        _dateCode = (Common.DateCode)reader.GetByte(1);

                        if (_dateCode == Common.DateCode.Historical)
                        {
                            _date = reader.GetDateTime(2);
                        }
                    }
                    else
                    {
                        throw new ServiceException("Could not retrieve process properties.");
                    }
                }
            }
        }

        private Guid GetTimeZoneId(SqlConnection conn)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select ID");
            sql.AppendLine("from dbo.TIMEZONEENTRY");
            sql.AppendLine("where NAME = 'UTC';");

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = this.ProcessCommandTimeout;
                cmd.CommandText = sql.ToString();

                return (Guid)cmd.ExecuteScalar();
            }
        }

        private IDictionary<string, Currency> LoadActiveCurrencies(SqlConnection conn)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select");
            sql.AppendLine("  ID,");
            sql.AppendLine("  ISO4217");
            sql.AppendLine("from dbo.CURRENCY");
            sql.AppendLine("where INACTIVE = 0;");

            Dictionary<string, Currency> currencies = new Dictionary<string, Currency>();

            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = this.ProcessCommandTimeout;
                cmd.CommandText = sql.ToString();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Guid id = reader.GetGuid(0);
                        string iso = reader.GetString(1);

                        currencies.Add(iso, new Currency { Id = id });
                    }
                }
            }

            return currencies;
        }

        private struct Currency
        {
            public Guid Id;
        }
    }
}