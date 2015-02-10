#region License
//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Invocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Falsy.Tests
{
    static class Timings
    {
        public static void Main()
        {
            const double interval = 150d;
            const double threshold = 0.02d;
            
            RunTimings<FalsyTests>(interval, threshold);
        }

        private static IEnumerable<char> AddSpaces(this string value)
        {
            yield return value[0];

            foreach (var character in value.Skip(1))
            {
                if (char.IsUpper(character)) yield return ' ';
                yield return character;
            }
        }

        private static string Readable(this string value)
        {
            return string.Join("", value.AddSpaces());
        }

        private static void RunTimings<T>(double interval, double threshold)
            where T : new()
        {
            var total = 0d;
            var test = new T();

            var type = typeof (T);

            var methods =
                type.GetMethods()
                    .Where(x => x.GetCustomAttribute<TestMethodAttribute>() != null && x.GetCustomAttribute<ExpectedExceptionAttribute>() == null)
                    .Select(x => new
                                 {
                                     Name = x.Name.Readable(),
                                     Method = x.BuildLazy()
                                 })
                    .ToList();

            var longestName = methods.Max(x => x.Name.Length);
            var alignment = " |    {0,-" + longestName + "}";

            Console.WriteLine();
            Console.WriteLine("  Timings: {0}", type.FullName);
            Console.WriteLine(" \t- Interval  : {0}", interval);
            Console.WriteLine(" \t- Threshold : {0}", threshold);

            var line = " ♦" + new string('-', longestName + 23) + "♦";
            Console.WriteLine(line);

            foreach (var item in methods)
            {
                dynamic @delegate = item.Method.Value;

                //Warmup
                @delegate(test);

                // Clean up
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                //Testing
                var watch = Stopwatch.StartNew();

                for (var i = 0; i < interval; i++)
                    @delegate(test);

                watch.Stop();

                Console.Write(alignment, item.Name);
                Console.Write(" : ");

                var totalMilliseconds = watch.Elapsed.TotalMilliseconds;
                total += totalMilliseconds;

                var average = totalMilliseconds/interval;

                Console.ForegroundColor = average > threshold ? ConsoleColor.Red : ConsoleColor.Green;

                Console.Write("{0,-11:00.000000000}", average);

                Console.ResetColor();

                Console.WriteLine(" ms |");
            }

            Console.WriteLine(line);

            Console.Write(alignment, "Total Execution Time");
            Console.Write(" : ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("{0,-11:00.000000000}", total);
            Console.ResetColor();
            Console.WriteLine(" ms |");

            var totalAverage = total / methods.Count / interval;
            Console.Write(alignment, "Average Per Call");
            Console.Write(" : ");
            Console.ForegroundColor = totalAverage > threshold ? ConsoleColor.Red : ConsoleColor.Green;
            Console.Write("{0,-11:00.000000000}", totalAverage);
            Console.ResetColor();
            Console.WriteLine(" ms |");

            Console.WriteLine(line);

            Console.ReadLine();
        }
    }
}
