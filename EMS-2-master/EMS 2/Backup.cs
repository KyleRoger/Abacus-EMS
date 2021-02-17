/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.RecoveryServices;
using Microsoft.Azure.Management.RecoveryServices.Backup;
using Microsoft.Azure.Management.RecoveryServices.Backup.Models;
using Microsoft.SqlServer.Dac;
using System.Data.SqlClient;
using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hyak.Common;
using Microsoft.Azure.Management.RecoveryServices.Backup.Models;
using System.Configuration;

namespace Data
{
    class Backup
    {
        public static void Test()
        {
            try
            {
                String potato = "";
                SqlConnection connection = new SqlConnection();
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "abacus-ems2.database.windows.net";
                builder.UserID = "abacus";
                builder.Password = "7HQabV9&At#u";
                builder.InitialCatalog = "EMS2";
                DacServices ds = new DacServices(builder.ConnectionString);
                using (connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    // public void ExportBacpac(string packageFileName, string databaseName);
                    ds.ExportBacpac(@"C:\backup\backup.bacpac", "abacus-ems2.database.windows.net");
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error.ToString());
            }
        }

        /*
        internal static SubscriptionCloudCredentials GetCredentials()
        {
            string certKey = ConfigurationManager.AppSettings["cert"];
            string subscriptionId = ConfigurationManager.AppSettings["subscriptionId"];

            return new CertificateCloudCredentials(subscriptionId, 
                new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    Convert.FromBase64String(certKey)));
        }

        public async Task Test()
        {
            String credentials = "potato";
            // SubscriptionCloudCredentials credentials = 
            RecoveryServicesBackupManagementClient client = new RecoveryServicesBackupManagementClient(GetCredentials());
            TriggerBackupRequest triggerBackupRequest = new TriggerBackupRequest();
            BaseRecoveryServicesJobResponse resp =
                await client.Backups.TriggerBackupAsync(resourceGroupName, resourceName, null, fabricName, containerName, protectedItemName, triggerBackupRequest);
        }
        
    }
}
*/