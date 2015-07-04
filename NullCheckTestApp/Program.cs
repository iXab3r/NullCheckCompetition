using System;
using System.Diagnostics;
using System.IO;
using BenchmarkDotNet;
using BenchmarkDotNet.Logging;
using BenchmarkDotNet.Tasks;

namespace NullCheckTestApp
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WindowWidth = 120;
                Console.WindowHeight = 60;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not reconfigure console window size - {0}", ex.Message);
            }

            try
            {
                new BenchmarkRunner(new IBenchmarkLogger[]
                {
                    new BenchmarkConsoleLogger(),
                    new BenchmarkStreamLogger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.log")),
                }).RunCompetition(new NullCheckCompetition());
            }
            finally
            {
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
        }

    }
}