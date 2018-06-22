using Blackbaud.AppFx.Server.AppCatalog;
using Blackbaud.AppFx.XmlTypes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Blackbaud.CustomFx.Currency.Catalog
{
    public sealed class ExchangeRateRefreshProcessAddDataForm : AppAddDataForm
    {
        public string Name;
        public string Description;
        public string FixerAPIAccessKey;
        public byte DateCode;
        public DateTime HistoricalDate;

        public override AppAddDataFormSaveResult Save()
        {
            Guid id = Guid.NewGuid();
            DateTime now = DateTime.Now;

            try
            {
                using (SqlConnection conn = this.RequestContext.OpenAppDBConnection())
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = this.RequestContext.ClientAppInfo.TimeOutSeconds;
                        cmd.CommandText = GetInsertSql();

                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@NAME", this.Name);
                        cmd.Parameters.AddWithValue("@DESCRIPTION", this.Description);
                        cmd.Parameters.AddWithValue("@FIXERAPIACCESSKEY", this.FixerAPIAccessKey);
                        cmd.Parameters.AddWithValue("@DATECODE", this.DateCode);

                        if (this.DateCode == (byte)Common.DateCode.Historical)
                        {
                            cmd.Parameters.AddWithValue("@HISTORICALDATE", this.HistoricalDate);
                        }

                        cmd.Parameters.AddWithValue("@ADDEDBYID", this.ProcessContext.ChangeAgentID);
                        cmd.Parameters.AddWithValue("@CHANGEDBYID", this.ProcessContext.ChangeAgentID);
                        cmd.Parameters.AddWithValue("@DATEADDED", now);
                        cmd.Parameters.AddWithValue("@DATECHANGED", now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                base.HandleSQLException(ex, GetExpectedDatabaseExceptions());
            }

            return new AppAddDataFormSaveResult()
            {
                ID = id.ToString()
            };
        }

        private string GetInsertSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("insert into dbo.USR_EXCHANGERATEREFRESHPROCESS");
            sql.AppendLine("(");
            sql.AppendLine("   ID,");
            sql.AppendLine("   NAME,");
            sql.AppendLine("   DESCRIPTION,");
            sql.AppendLine("   FIXERAPIACCESSKEY,");
            sql.AppendLine("   DATECODE,");

            if (this.DateCode == (byte)Common.DateCode.Historical)
            {
                sql.AppendLine("   HISTORICALDATE,");
            }

            sql.AppendLine("   ADDEDBYID,");
            sql.AppendLine("   CHANGEDBYID,");
            sql.AppendLine("   DATEADDED,");
            sql.AppendLine("   DATECHANGED");
            sql.AppendLine(")");
            sql.AppendLine("values");
            sql.AppendLine("(");
            sql.AppendLine("   @ID,");
            sql.AppendLine("   @NAME,");
            sql.AppendLine("   @DESCRIPTION,");
            sql.AppendLine("   @FIXERAPIACCESSKEY,");
            sql.AppendLine("   @DATECODE,");

            if (this.DateCode == (byte)Common.DateCode.Historical)
            {
                sql.AppendLine("   @HISTORICALDATE,");
            }

            sql.AppendLine("   @ADDEDBYID,");
            sql.AppendLine("   @CHANGEDBYID,");
            sql.AppendLine("   @DATEADDED,");
            sql.AppendLine("   @DATECHANGED");
            sql.AppendLine(");");

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