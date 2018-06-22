using Blackbaud.AppFx.Server.AppCatalog;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Blackbaud.CustomFx.Currency.Catalog
{
    public sealed class ExchangeRateRefreshProcessDeleteRecordOperation : AppRecordOperationPerform
    {
        public override AppRecordOperationPerformResult Perform()
        {
            using (SqlConnection conn = this.RequestContext.OpenAppDBConnection())
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = this.RequestContext.ClientAppInfo.TimeOutSeconds;
                    cmd.CommandText = "dbo.USP_USR_EXCHANGERATEREFRESHPROCESS_DELETEBYID_WITHCHANGEAGENTID";

                    cmd.Parameters.AddWithValue("@ID", new Guid(this.ProcessContext.RecordID));
                    cmd.Parameters.AddWithValue("@CHANGEAGENTID", this.ProcessContext.ChangeAgentID);

                    cmd.ExecuteNonQuery();
                }
            }
            
            return new AppRecordOperationPerformResult();
        }
    }
}