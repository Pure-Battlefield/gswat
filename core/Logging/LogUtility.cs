using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using core.TableStoreEntities;

namespace core.Logging
{
    public static class LogUtility
    {
        static CloudTable LogTable { get; set; }

        public static void Init()
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
                var tableClient = storageAccount.CreateCloudTableClient();
                LogTable = tableClient.GetTableReference("serverLogs");
                LogTable.CreateIfNotExists();
            }
            catch (Exception e)
            {
                LogUtility.Log(MethodBase.GetCurrentMethod().DeclaringType.ToString(), MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        /// <summary>
        /// Log 
        /// </summary>
        /// <param name="className">The class name to include in the log</param>
        /// <param name="funcName">Current function on the stack</param>
        /// <param name="message">Message to log</param>
        public static void Log(String className, String funcName, String message)
        {
            try
            {
                var msg = new LogMessageEntity(DateTime.UtcNow, className, funcName, message);
                var insertOp = TableOperation.Insert(msg);
                LogTable.Execute(insertOp);
            }
            catch (Exception)
            {
                // Abandon all hope, ye who enter here
            }
        }
    }
}
