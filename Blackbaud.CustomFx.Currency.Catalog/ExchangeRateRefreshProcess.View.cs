using Blackbaud.AppFx.Server.AppCatalog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Blackbaud.CustomFx.Currency.Catalog
{
    public sealed class ExchangeRateRefreshProcessViewDataForm : AppViewDataForm
    {
        public string Name;
        public string Description;
        public string Date;

        public override AppViewDataFormLoadResult Load()
        {
            AppViewDataFormLoadResult result = new AppViewDataFormLoadResult();
            
            using (SqlConnection conn = this.RequestContext.OpenAppDBConnection())
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = this.RequestContext.ClientAppInfo.TimeOutSeconds;
                    cmd.CommandText = GetSelectSql();

                    cmd.Parameters.AddWithValue("@ID", new Guid(this.ProcessContext.RecordID));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.Name = reader.GetString(0);
                            this.Description = reader.GetString(1);

                            switch (reader.GetByte(2))
                            {
                                case (byte)Common.DateCode.Latest:
                                    this.Date = "Latest";
                                    break;
                                case (byte)Common.DateCode.Historical:
                                    this.Date = "Historical";
                                    break;
                                default:
                                    this.Date = string.Empty;
                                    break;
                            }

                            result.DataLoaded = true;
                        }
                        else
                        {
                            result.DataLoaded = false;
                        }
                    }
                }
            }

            return result;
        }

        private static string GetSelectSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select");
            sql.AppendLine("  NAME,");
            sql.AppendLine("  DESCRIPTION,");
            sql.AppendLine("  DATECODE");
            sql.AppendLine("from dbo.USR_EXCHANGERATEREFRESHPROCESS");
            sql.AppendLine("where ID = @ID;");

            return sql.ToString();
        }
    }
}