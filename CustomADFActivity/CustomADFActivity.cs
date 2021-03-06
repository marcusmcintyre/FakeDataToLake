﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Diagnostics;

using Microsoft.Azure.Management.DataFactories.Models;
using Microsoft.Azure.Management.DataFactories.Runtime;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.DataLake.Store.Models;

namespace CustomADFActivity
{
    public class CustomADFActivity : IDotNetActivity
    {
        public IDictionary<string, string> Execute(IEnumerable<LinkedService> linkedServices, IEnumerable<Dataset> datasets, Activity activity, IActivityLogger logger)
        {
            APIHelper api = new APIHelper();
            IEnumerable<GitHubUserDto> enumOfUsers = api.GetUsers();
            List<GitHubUserDto> listOfUsers = enumOfUsers.ToList<GitHubUserDto>();
            List<string> listOfUserStrings = new List<string>();
            DataLakeHelper dl = new DataLakeHelper();
            string current_dir = "/";
            string file_name = "newfile.txt";
            List<FileStatusProperties> listOfFiles = dl.ListItems(current_dir);

            foreach (var row in api.GetUsers())
            {

            }
            string userString = listOfUsers.ElementAt(0).ToString();
            Console.WriteLine("got Users successfully!");


            try
            {
                var rows = new List<string>();
                var count = 0;
                var is_file_created = false;

                foreach (var file in listOfFiles)
                {
                    if (file.PathSuffix.Equals(file_name))
                    {
                        is_file_created = true;
                    }
                }

                foreach (var row in api.GetUsers())
                {
                    rows.Add(row.ToString());
                    count++;

                    if (count != 2000) continue;

                    dl.StoreData(current_dir + file_name, rows, is_file_created);

                    if (!is_file_created) is_file_created = true;

                    count = 0;
                    rows = new List<string>();
                }

                dl.StoreData(current_dir + file_name, rows, is_file_created);


            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                Console.Write(e.InnerException.Message);
                throw;
            }
            return new Dictionary<string, string>();
        }
    }
}
