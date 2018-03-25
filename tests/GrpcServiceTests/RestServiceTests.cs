using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.Xunit2;

using FluentAssertions;

using Newtonsoft.Json;

using Rest.Contracts;

using RestService;

using Xunit;

namespace ServiceTests
{
    public class RestServiceTests : RestServiceTestsBase
    {
        public RestServiceTests()
        {
            HttpClient.BaseAddress = new Uri("http://localhost:5002/");
        }

        public class PostMany : RestServiceTests
        {
            [Theory]
            [AutoData]
            public async Task Should_return_correct_values(IEnumerable<RestRequest> sampleRequest)
            {
                var payload = new StringContent(JsonConvert.SerializeObject(sampleRequest));
                payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await HttpClient.PostAsync("api/values/postmany", payload);

                var result = JsonConvert.DeserializeObject<IEnumerable<RestResponse>>(await response.Content.ReadAsStringAsync());

                foreach (var restResponse in result)
                {
                    sampleRequest.ToList().Should().ContainSingle(x => x.Value == restResponse.Value);
                }
            }
        }

        public class Post : RestServiceTests
        {
            [Theory]
            [AutoData]
            public async Task Should_return_correct_values(RestRequest sampleRequest)
            {
                var payload = new StringContent(JsonConvert.SerializeObject(sampleRequest));
                payload.Headers.ContentType = new MediaTypeHeaderValue("Application/json");

                var response = await HttpClient.PostAsync("api/values/post", payload);

                var result = JsonConvert.DeserializeObject<RestResponse>(await response.Content.ReadAsStringAsync());

                sampleRequest.Value.Should().Be(result.Value);
            }
        }
    }

    public class RestServiceTestsBase
    {
        public static readonly Fixture fixture = new Fixture();
        public HttpClient HttpClient;

        public RestServiceTestsBase()
        {
            Task.Run(() => Program.Main(null));
            Thread.Sleep(1000);

            HttpClient = new HttpClient();

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
