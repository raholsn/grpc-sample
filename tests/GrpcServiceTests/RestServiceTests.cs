using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions;

using Newtonsoft.Json;

using Rest.Contracts;

using RestService;

using Xunit;

namespace ServiceTests
{
    public class RestServiceTests : RestServiceTestsBase
    {
        public class PostMany : RestServiceTests
        {
            private readonly IEnumerable<RestRequest> _sampleRequests = fixture.CreateMany<RestRequest>();
            public PostMany()
            {
                HttpClient.BaseAddress = new Uri("http://localhost:65290/");
            }

            [Fact]
            public async Task Should_return_correct_values()
            {
                var response = await HttpClient.PostAsync("api/values/postmany", new StringContent(JsonConvert.SerializeObject(_sampleRequests)));

                var result = JsonConvert.DeserializeObject<IEnumerable<RestResponse>>(await response.Content.ReadAsStringAsync());

                foreach (var restResponse in result)
                {
                    _sampleRequests.ToList().Should().Contain(new RestRequest { Value = restResponse.Value });
                }
            }
        }
    }

    public class RestServiceTestsBase
    {

        public HttpClient HttpClient;
        public static readonly Fixture fixture = new Fixture();

        public RestServiceTestsBase()
        {
            Task.Run(() => Program.Main(null));
            Thread.Sleep(1000);

            HttpClient = new HttpClient();
        }
    }
}
