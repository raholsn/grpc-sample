using System.Threading;
using System.Threading.Tasks;

using AutoFixture;

using Grpc.Core;

using GrpcService;

namespace ServiceTests
{
    public class GrpcServiceTestsBase
    {
        public static readonly Fixture Fixture = new Fixture();
        public readonly Grpc.Contract.GrpcService.GrpcServiceClient Client;

        public GrpcServiceTestsBase()
        {
            Task.Run(() => Start.Main(null));
            Thread.Sleep(1000);

            var channel = new Channel("localhost:54321", ChannelCredentials.Insecure);
            Client = new Grpc.Contract.GrpcService.GrpcServiceClient(channel);
        }
    }
}
