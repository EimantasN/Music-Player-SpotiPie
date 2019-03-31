using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mobile_Api.Models
{
    public class CustomRestClient : RestClient
    {
        private static IRestRequest GET { get; set; } = new RestRequest(Method.GET);

        private static IRestRequest POST { get; set; } = new RestRequest(Method.POST);

        private bool Active { get; set; }

        public long Id { get; set; }

        public CustomRestClient()
        {
            Id = DateTime.Now.Ticks;
        }

        public CustomRestClient(string baseUrl)
        {
            Id = DateTime.Now.Ticks;
            BaseUrl = new Uri(baseUrl);
        }

        public long GetId()
        {
            return Id;
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
            try
            {
                IRestResponse response;
                if (method == Method.GET)
                    response = await base.ExecuteGetTaskAsync(GET);
                else
                    response = await base.ExecuteTaskAsync(POST);

                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<List<T>>(response.Content);
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception e)
            {
                throw e;
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
