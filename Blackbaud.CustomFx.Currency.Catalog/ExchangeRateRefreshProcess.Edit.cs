using Blackbaud.AppFx.Server.AppCatalog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Blackbaud.AppFx.XmlTypes;

namespace Blackbaud.CustomFx.Currency.Catalog
{
    public sealed class ExchangeRateRefreshProcessEditDataForm : AppEditDataForm
    {
        public string Name;
        public string Description;
        public string FixerAPIAccessKey;
        public byte DateCode;
        public DateTime HistoricalDate;

        public override AppEditDataFormLoadResult Load()
        {
            AppEditDataFormLoadResult result = new AppEditDataFormLoadResult();

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
                            this.FixerAPIAccessKey = reader.GetString(2);
                            this.DateCode = reader.GetByte(3);

                            if (!reader.IsDBNull(4))
                            {
                                this.HistoricalDate = reader.GetDateTime(4);
                            }

                            result.TSLong = reader.GetInt64(5);
                            result.DataLoaded = true;
                        }
                    }
                }
            }

            return result;
        }

        public override AppEditDataFormSaveResult Save()
        {
            try
            {
                using (SqlConnection conn = this.RequestContext.OpenAppDBConnection())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = this.RequestContext.ClientAppInfo.TimeOutSeconds;
                        cmd.CommandText = GetUpdateSql();

                        cmd.Parameters.AddWithValue("@ID", new Guid(this.ProcessContext.RecordID));
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", this.Description);
                        cmd.Parameters.AddWithValue("@FIXERAPIACCESSKEY", this.FixerAPIAccessKey);
                        cmd.Parameters.AddWithValue("@DATECODE", this.DateCode);

                        if (this.DateCode == (byte)Common.DateCode.Historical)
                        {
                            cmd.Parameters.AddWithValue("@HISTORICALDATE", this.HistoricalDate);
                        }

                        cmd.Parameters.AddWithValue("@CHANGEDBYID", this.ProcessContext.ChangeAgentID);
                        cmd.Parameters.AddWithValue("@DATECHANGED", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                base.HandleSQLException(ex, GetExpectedDatabaseExceptions());
            }

            return new AppEditDataFormSaveResult();
        }

        private static string GetSelectSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select");
            sql.AppendLine("  NAME,");
            sql.AppendLine("  DESCRIPTION,");
            sql.AppendLine("  FIXERAPIACCESSKEY,");
            sql.AppendLine("  DATECODE,");
            sql.AppendLine("  HISTORICALDATE,");
            sql.AppendLine("  TSLONG");
            sql.AppendLine("from dbo.USR_EXCHANGERATEREFRESHPROCESS");
            sql.AppendLine("where ID = @ID;");

            return sql.ToString();
        }

        private string GetUpdateSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("update dbo.USR_EXCHANGERATEREFRESHPROCESS set");
            sql.AppendLine("  NAME = @NAME,");
            sql.AppendLine("  DESCRIPTION = @DESCRIPTION,");
            sql.AppendLine("  FIXERAPIACCESSKEY = @FIXERAPIACCESSKEY,");
            sql.AppendLine("  DATECODE = @DATECODE,");

            if (this.DateCode == (byte)Common.DateCode.Historical)
            {
                sql.AppendLine("  HISTORICALDATE = @HISTORICALDATE,");
            }

            sql.AppendLine("  CHANGEDBYID = @CHANGEDBYID,");
            sql.AppendLine("  DATECHANGED = @DATECHANGED");
            sql.AppendLine("where ID = @ID;");

            return sql.ToString();
        }

        private static ExpectedDBExceptions GetExpectedDatabaseExceptions()
        {
            ExpectedDBExceptions exceptions = new ExpectedDBExceptions();
            exceptions.Constraints = new ConstraintDescriptor[2] {
                new ConstraintDescriptor() { Name = "USR_CK_HISTORICALDATEVALID", Field = "HISTORICALDATE", Type = ConstraintType.Format, CustomErrorMsg = "The date can only be specified when a historical rate is selected." },
                new ConstraintDescriptor() { Name = "USR_CK_HISTORICALDATEVALIDRANGE", Field = "HISTORICALDATE", Type = ConstraintType.Format, CustomErrorMsg = "The exchange rate date must be between 1999 and now." }
            };

            return exceptions;
        }
    }
}