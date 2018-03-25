using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Rest.Contracts;

namespace RestService.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpPost]
        [Route("postmany")]
        public IEnumerable<RestResponse> PostMany([FromBody] IEnumerable<RestRequest> restRequests)
        {
            var response = new List<RestResponse>();

            foreach (var restRequest in restRequests)
            {
                response.Add(new RestResponse
                {
                    Value = restRequest.Value
                });
            }

            return response;
        }

        [HttpPost]
        [Route("post")]
        public RestResponse Post([FromBody] RestRequest restRequest)
        {
            Console.WriteLine($"Recieved Post: {restRequest.Value}");

            return new RestResponse
            {
                Value = restRequest.Value
            };
        }
    }
}
