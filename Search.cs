using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Zadanie
{
    class Search
    {
        public string textSearch;

        public Country GetSearch()
        {
            string countrystring = "https://restcountries.eu/rest/v2/name/" + textSearch;
            WebRequest request = WebRequest.Create(countrystring);
            request.Method = "GET";

            request.ContentType = "application/json";

            WebResponse response = request.GetResponse();

            string str = string.Empty;

            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader read = new StreamReader(s))
                {
                    str = read.ReadToEnd();
                }
            }
            response.Close();

            return JsonConvert.DeserializeObject<Country>(str.Substring(1, str.Length - 2));
        }
    }
}
