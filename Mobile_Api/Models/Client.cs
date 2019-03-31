using RestSharp;
using System;

namespace Mobile_Api.Models
{
    public class Client
    {
        private CustomRestClient RestClient { get; set; }
        private bool IsFree { get; set; }
        private int UsedTime { get; set; }
        private DateTime LastUsed { get; set; }

        public Client()
        {
            RestClient = new CustomRestClient();
            IsFree = true;
            UsedTime = 0;
            LastUsed = DateTime.Now;
        }

        public long GetId()
        {
            return RestClient.Id;
        }

        public CustomRestClient GetClient(string url)
        {
            RestClient.BaseUrl = new Uri(url);
            return RestClient;
        }

        public void IncreaseUsedTime()
        {
            this.UsedTime++;
        }

        private void UpdateUseTime()
        {
            this.LastUsed = DateTime.Now;
        }

        public bool GetState()
        {
            return this.IsFree;
        }

        public void LockClient()
        {
            this.IsFree = false;
            UpdateUseTime();
        }

        public void ReleaseClient()
        {
            this.IsFree = true;
            IncreaseUsedTime();
            UpdateUseTime();
        }
    }
}
