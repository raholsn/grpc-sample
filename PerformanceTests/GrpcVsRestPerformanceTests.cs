using System;

namespace PerformanceTests
{
    public class GrpcVsRestPerformanceTests
    {
        private static TimeSpan _grpcTime;
        private static TimeSpan _restTime;

        private static void Main(string[] args)
        {
            var grpcPerformanceTest = new GrpcPerformanceTest()
                                      .WithType(Utilities.GrpcTypes.BiDirectionalStream)
                                      .WithRequests(int.Parse(args[0]))
                                      .Build();

            var restPerformanceTest = new RestPerformanceTest()
                                      .WithRequests(int.Parse(args[0]))
                                      .Build();

            _grpcTime = grpcPerformanceTest.Test().Result;
            _restTime = restPerformanceTest.Test().Result;
            Console.WriteLine($"GrpcBiDirectionalStream: {_grpcTime.ToElapsedTime()}");
            Console.WriteLine($"RestPost: {_restTime.ToElapsedTime()}");

            Console.ReadLine();
        }
    }
}
