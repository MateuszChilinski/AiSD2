using System;
using System.Collections.Generic;
using System.Linq;
using TestPlatform;
using TestLab12;

namespace ASD
{

    class Program
    {
        static void Main(string[] args)
        {
            TestParameters param = new TestParameters();
            param.ShowPositiveTests = false;                        //Czy wyświetlać pozytywne wyniki
            param.ShowNegativeTests = true;                         //Czy wyświetlać negatywne wyniki
            param.ShowTimeoutedTests = true;                        //Czy wyświetlać komunikaty o upływie czasu
            param.ShowThrownExepctions = true;                      //Czy wyświetlać komunikaty o wyrzuconych wyjątkach
            param.CatchExeption = true;                             //Czy łapać wyjątki
            param.WorkOnCopy = false;                               //Czy testować funkcję na kopii argumentów
            param.TimeoutDelay = TimeSpan.FromMilliseconds(2500);  //Czas na pojedyńcze zapytanie
            param.NumberOfThreads = 8;     //Ilość wątków
            TestLab12.TestLab12 Test = new TestLab12.TestLab12(param);
            Test.FuncitonToTest = (inq) =>
            {
                double dist;
                List<Point> list = new List<Point>(inq.X.Zip(inq.Y, (a, b) => new Point(a, b)));
                var pair = SweepClosestPair.FindClosestPoints(list, out dist);
                return new AnswerLab12(new Tuple<double, double>(pair.Item1.x, pair.Item1.y), new Tuple<double, double>(pair.Item2.x, pair.Item2.y), dist);
            };
            Test.LoadCases("../../BaseCases");
            Test.PerformTest();

            Test.Clear();
            Test.LoadCases("../../CorrectnessCheck");
            Test.PerformTest();

            Test.Clear();
            Test.LoadCases("../../PerformanceTest");
            Test.PerformTest();

            return;
        }
    }
}
