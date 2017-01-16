using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CustomADFActivity
{
    class APIHelper
    {
        private static HttpClient client;
        private string url = "http://www.google.com";
        private string JSONresponse = "";

        public APIHelper()
        {
            /*
            //var handler = new WebRequestHandler();
            //handler.ClientCertificates.Add(certificate);
            client = new HttpClient(new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Automatic
            });
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            goToUrl().Wait();
            */

        }

        public IEnumerable<GitHubUserDto> GetUsers()
        {
            // Get the JSON response from the URL
            yield return new GitHubUserDto { Name = "Marcus", Age = 23, HairColor = "Brown", Height = 60.0 };
            yield return new GitHubUserDto { Name = "Mike", Age = 22, HairColor = "Red", Height = 70.0 };
            yield return new GitHubUserDto { Name = "Nevan", Age = 23, HairColor = "Blonde", Height = 65.0 };

            //Parse the JSON into Dto objects and add them to a list
            yield break;
        }


        private async Task goToUrl()
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            JSONresponse = await response.Content.ReadAsStringAsync();
        }

    }
}
