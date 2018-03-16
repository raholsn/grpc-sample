using System;

using Grpc.Core;

namespace GrpcService
{
    public class Start
    {
        static void Main(string[] args)
        {
            int port = 54321;

            var server = new Server
            {
                Services = { Grpc.Contract.GrpcService.BindService(new GrpcService.Services.LuckQueryService()) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Server listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();

        }
    }
}
