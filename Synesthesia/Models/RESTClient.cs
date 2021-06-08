using System;
using System.Net.Http;
using System.Text;

namespace Synesthesia.Models
{
    public class RESTClient
    {
        private static HttpClient client = new HttpClient();

        public static string Post(Uri url, string json)
        {
            var content = new StringContent(json != null ? json : "{}", Encoding.UTF8, "application/json");

            string jsonResult = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            return jsonResult;
        }
    }
}
