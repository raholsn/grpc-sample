using System;
using System.Globalization;
using System.Threading.Tasks;

using Grpc.Core;

using Queries.Contract.protos;

namespace LuckQueryService.Services
{
    public class LuckQueryService : GrpcQueryService.GrpcQueryServiceBase
    {
        public override Task<LuckResponse> GetLuck(LuckRequest request, ServerCallContext context)
        {
            var rnd = new Random();
            var d = (rnd.NextDouble() * 100.0);

            var response = new LuckResponse
            {
                Notification = "Not bad!",
                Procent = $"{d}" + "%",
            };

            if (d > 70.0)
            {
                response.Notification = "Right place, Right time!";
            }
            else if(d < 30)
            {
                response.Notification = "There will be burned coffie today!";

            }

            return Task.FromResult(response);
        }
    }
}
