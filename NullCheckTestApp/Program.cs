using System;
using System.Diagnostics;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace NullCheckTestApp
{
    public class Program
    {
        public static void Main(string[] _args)
        {
            try
            {
                Console.WindowWidth = 120;
                Console.WindowHeight = 60;
            }
            catch (Exception)
            {
                //
            }

            new BenchmarkRunner().RunCompetition(new NullCheckCompetition(), new BenchmarkSettings(1, 1));
        }

    }
}