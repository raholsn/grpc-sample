using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Rest.Contracts;

using RestService;

namespace PerformanceTests
{
    public class RestPerformanceTest
    {
        private readonly HttpClient _httpClient;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private int NrOfRequests { get; set; }
        private IList<RestRequest> RestRequests { get; set; }

        public RestPerformanceTest()
        {
            Task.Run(() => Program.Main(null));
            Thread.Sleep(1000);

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5002/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TimeSpan> Test()
        {
            _stopWatch.Reset();
            _stopWatch.Start();
            foreach (var restRequest in RestRequests)
            {
                await PostAsync("api/values/post", restRequest);
            }

            _stopWatch.Stop();
            return _stopWatch.Elapsed;
        }

        private async Task<RestRequest> PostAsync(string uri, RestRequest data)
        {
            var payload = new StringContent(JsonConvert.SerializeObject(data));
            payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.PostAsync(uri, payload);

            return JsonConvert.DeserializeObject<RestRequest>(await response.Content.ReadAsStringAsync());
        }

        public RestPerformanceTest Build()
        {
            if (NrOfRequests == 0)
            {
                throw new ArgumentException("request not specified");
            }

            RestRequests = new List<RestRequest>();

            for (var i = 0; i < NrOfRequests; i++)
            {
                RestRequests.Add(new RestRequest
                {
                    Value = i
                });
            }

            return this;
        }

        public RestPerformanceTest WithRequests(int nrOfRequests)
        {
            NrOfRequests = nrOfRequests;

            return this;
        }
    }
}
