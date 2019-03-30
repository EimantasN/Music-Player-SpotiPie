using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api.Models
{
    public class CustomRestClient : RestClient
    {
        private static RestRequest GET { get; set; } = new RestRequest(Method.GET);

        private static RestRequest POST { get; set; } = new RestRequest(Method.POST);

        private bool Active { get; set; }

        public long Id { get; set; }

        public CustomRestClient()
        {

        }

        public CustomRestClient(string baseUrl)
        {
            Id = DateTime.Now.Ticks;
            BaseUrl = new Uri(baseUrl);
        }

        private IRestRequest GetRequest(Method method)
        {
            switch (method)
            {
                case Method.GET:
                    return GET;
                case Method.POST:
                    return POST;
            }

            throw new Exception("Must choose request type");
        }

        public async Task<List<T>> CustomExecuteTaskAsync<T>(Method method)
        {
            IRestResponse response = await base.ExecuteTaskAsync(GetRequest(method));
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<T>>(response.Content);
            }
            else
            {
                return new List<T>();
            }
        }

        private void TimeoutCheck(IRestRequest request, IRestResponse response)
        {
            if (response.StatusCode == 0)
            {
                //Uncomment the line below to throw a real exception.
                throw new TimeoutException("The request timed out!");
            }
        }
    }
}
