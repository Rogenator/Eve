using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace Eve.Entity
{
    public static class DataBase
    {
        public static string SqlConnectionString
        {
            get
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
                scsb.DataSource = ConfigurationManager.AppSettings["DB_Server"];
                scsb.UserID = ConfigurationManager.AppSettings["DB_User"];
                scsb.Password = ConfigurationManager.AppSettings["DB_Password"];
                scsb.InitialCatalog = "SellPoint";
                scsb.MultipleActiveResultSets = true;
                scsb.ApplicationName = "EntityFramework";
                return scsb.ConnectionString;
            }
        }
        public static string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(SqlConnectionString);
                EntityConnectionStringBuilder ecb = new EntityConnectionStringBuilder();
                ecb.Metadata = string.Format("res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", ConfigurationManager.AppSettings["DB_Model"]);
                ecb.Provider = "System.Data.SqlClient";
                ecb.ProviderConnectionString = scsb.ConnectionString;
                return ecb.ConnectionString;
            }
        }
    }
}
