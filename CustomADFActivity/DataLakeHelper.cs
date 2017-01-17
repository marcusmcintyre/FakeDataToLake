using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.DataLake.Store;
using Microsoft.Azure.Management.DataLake.StoreUploader;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.OData;
using System.IO;
using System.Threading;
using Microsoft.Rest.Azure.Authentication;
using Microsoft.Rest;
using Microsoft.Azure.Management.DataLake.Store.Models;

namespace CustomADFActivity
{
    class DataLakeHelper
    {
        private IDataLakeStoreFileSystemManagementClient inner_client;
        private IDataLakeStoreAccountManagementClient _adlsClient;

        private string client_id = "831733da-f1dc-42f3-bf84-598ff3a92313";
        private string client_key = "3oR9VxnImRBuKVJ+MMKe1uOqY3RLBSsYHCsbWx0xk6g= ";
        private string subscription_id = "8a7e1230-6a2c-4af1-9aab-05c49db1006e";
        private string adls_account_name = "marcusdatalakestore";
        public DataLakeHelper()
        {
            create_adls_client();
        }

        private void create_adls_client()
        {

            // Service principal / appplication authentication with client secret / key
            // Use the client ID and certificate of an existing AAD "Web App" application.
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var domain = "dadamsabilisne.onmicrosoft.com";
            var webApp_clientId = client_id;
            var clientSecret = client_key;
            var clientCredential = new ClientCredential(webApp_clientId, clientSecret);
            var creds = ApplicationTokenProvider.LoginSilentAsync(domain, clientCredential).Result;

            // Create client objects and set the subscription ID
            _adlsClient = new DataLakeStoreAccountManagementClient(creds);
            inner_client = new DataLakeStoreFileSystemManagementClient(creds);

            _adlsClient.SubscriptionId = subscription_id;


            Console.Write("Authentication success.");
        }


        public void StoreData(string path, List<string> rows, bool append)
        {
            try
            {

                var buffer = new MemoryStream();
                var sw = new StreamWriter(buffer);

                foreach (var row in rows)
                {
                    //Ensure the request is below 4mb in size to avoid column alignment issue
                    if (buffer.Length + Encoding.UTF8.GetByteCount(row) > 3500000)
                    {
                        buffer.Position = 0;
                        if (append)
                        {
                            execute_append(path, buffer);
                        }
                        else
                        {
                            execute_create(path, buffer);
                            append = true;
                        }

                        buffer = new MemoryStream();
                        sw = new StreamWriter(buffer);
                    }
                    sw.WriteLine(row);
                    sw.Flush();
                }

                if (buffer.Length <= 0) return;

                buffer.Position = 0;
                if (append)
                {
                    execute_append(path, buffer);
                }
                else
                {
                    execute_create(path, buffer);
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        private void execute_create(string path, MemoryStream ms)
        {
            inner_client.FileSystem.Create(adls_account_name, path, ms);
            Console.WriteLine("File Created");
        }

        private void execute_append(string path, MemoryStream ms)
        {
            inner_client.FileSystem.Append(adls_account_name, path, ms);
            Console.WriteLine("Data Appended");
        }

        // List files and directories
        public List<FileStatusProperties> ListItems(string directoryPath)
        {
            return inner_client.FileSystem.ListFileStatus(adls_account_name, directoryPath).FileStatuses.FileStatus.ToList();
        }

    }
}
