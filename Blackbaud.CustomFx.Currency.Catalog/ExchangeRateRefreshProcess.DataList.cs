using Blackbaud.AppFx.Server;
using Blackbaud.AppFx.Server.AppCatalog;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Blackbaud.CustomFx.Currency.Catalog
{
    public sealed class ExchangeRateRefreshProcessDataList : AppDataList
    {
        public override AppDataListResult GetListResults()
        {
            List<DataListResultRow> rows = new List<DataListResultRow>();

            using (SqlConnection conn = this.RequestContext.OpenAppDBConnection())
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = this.RequestContext.ClientAppInfo.TimeOutSeconds;
                    cmd.CommandText = GetSelectSql();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] values = new string[5]
                            {
                                reader.GetGuid(0).ToString(),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                string.Empty
                            };

                            switch (reader.GetByte(4))
                            {
                                case (byte)Common.DateCode.Latest:
                                    values[4] = "Latest";
                                    break;
                                case (byte)Common.DateCode.Historical:
                                    values[4] = "Historical";
                                    break;
                            }

                            rows.Add(new DataListResultRow() { Values = values });
                        }
                    }
                }
            }

            return new AppDataListResult(rows.ToArray());
        }

        private static string GetSelectSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select");
            sql.AppendLine("  ID,");
            sql.AppendLine("  NAME,");
            sql.AppendLine("  DESCRIPTION,");
            sql.AppendLine("  FIXERAPIACCESSKEY,");
            sql.AppendLine("  DATECODE");
            sql.AppendLine("from dbo.USR_EXCHANGERATEREFRESHPROCESS;");

            return sql.ToString();
        }
    }
}