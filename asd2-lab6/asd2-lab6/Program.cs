using ASD;
using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    class Program
    {
        abstract class BasicSecondPathTestCase : TestCase
        {
            protected Graph g;
            protected Edge[] path;
            protected int a;
            protected int b;
            protected double? result;
            protected double? expectedLength;

            protected BasicSecondPathTestCase(double timeLimit, Graph g, int a, int b, double? expectedLength) : base(timeLimit, null)
            {
                this.g = g;
                this.a = a;
                this.b = b;
                this.expectedLength = expectedLength;
            }

            protected bool? CheckResult(out string message)
            {
                if (expectedLength.HasValue && !result.HasValue)
                {
                    message = $"no path found, but it exists";
                    return false;
                }
                if (!expectedLength.HasValue && result.HasValue)
                {
                    message = $"path found, but it does not exist";
                    return false;
                }
                if (!expectedLength.HasValue && !result.HasValue)
                {
                    message = path==null ? $"OK, no second path exists" : "inconsistent result (returned value: null, path: not null)" ;
                    return path==null;
                }
                if (expectedLength.Value !=result.Value)
                {
                    message = $"wrong length, expected {expectedLength}, returned {result}";
                    return false;
                }
                message = "";
                return null;
            }

            protected Result CheckIfProperPath(out string message)
            {
                if (path.Length == 0)
                {
                    message = "empty path returned";
                    return Result.BadResult;
                }
                if (path[0].From != a)
                {
                    message = "wrong starting point";
                    return Result.BadResult;
                }
                if (path[path.Length - 1].To != b)
                {
                    message = "wrong final point";
                    return Result.BadResult;
                }

                double sum = 0;
                Edge last = new Edge();
                bool first = true;
                foreach (var e in path)
                {
                    if (e.From == b)
                    {
                        message = $"destination vertex appears inside the path";
                        return Result.BadResult;
                    }
                    if (!g.OutEdges(e.From).Contains(e))
                    {
                        message = $"edge {e} does not exist";
                        return Result.BadResult;
                    }
                    if (first)
                        first = false;
                    else
                    {
                        if (e.From != last.To)
                        {
                            message = $"edge {last} cannot be followed by {e}";
                            return Result.BadResult;
                        }
                    }
                    sum += e.Weight;
                    last = e;
                }
                if (sum != result.Value)
                {
                    message = $"path does not match the length, expected {result}, is {sum}";
                    return Result.BadResult;
                }
                message = "OK";
                return Result.Success;
            }
        }

        class RepSecondPathTestCase : BasicSecondPathTestCase
        {
            public RepSecondPathTestCase(double timeLimit, Graph g, int a, int b, double? expectedLength) : base(timeLimit, g, a, b, expectedLength)
            {

            }
            public override void PerformTestCase()
            {
                result = g.Clone().FindSecondShortestPath(a, b, out path);
            }
            public override void VerifyTestCase(out Result resultCode, out string message)
            {
                bool? r = CheckResult(out message);
                if (r.HasValue)
                {
                    if (r.Value)
                        resultCode = Result.Success;
                    else
                        resultCode = Result.BadResult;
                    return;
                }
                resultCode = CheckIfProperPath(out message);
            }
        }

        class SimpleSecondPathTestCase : BasicSecondPathTestCase
        {
            public SimpleSecondPathTestCase(double timeLimit, Graph g, int a, int b, double? expectedLength) : base(timeLimit, g, a, b, expectedLength)
            {

            }
            public override void PerformTestCase()
            {
                result = g.Clone().FindSecondSimpleShortestPath(a, b, out path);
            }
            public override void VerifyTestCase(out Result resultCode, out string message)
            {
                bool? r = CheckResult(out message);
                if (r.HasValue)
                {
                    if (r.Value)
                        resultCode = Result.Success;
                    else
                        resultCode = Result.BadResult;
                    return;
                }
                if (!CheckSimple(out message))
                {
                    resultCode = Result.BadResult;
                    return;
                }
                resultCode = CheckIfProperPath(out message);
            }

            private bool CheckSimple(out string message)
            {
                HashSet<int> vertices = new HashSet<int>();
                bool first = true;
                foreach (var e in path)
                {
                    if (first)
                    {
                        vertices.Add(e.From);
                        vertices.Add(e.To);
                        first = false;
                    }
                   else if (vertices.Contains(e.To))
                    {
                        message = "vertex repeated on a path";
                        return false;
                    }
                    vertices.Add(e.To);
                }

                message = "";
                return true;
            }
        }


        static void Main(string[] args)
        {
            Random rnd = new Random(100);
            TestSet undirectedRep = new TestSet();
            TestSet directedRep = new TestSet();
            TestSet undirectedSimple = new TestSet();
            TestSet directedSimple = new TestSet();
            Graph g;

            #region undirected, with repetitions
            
            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 1);
            g.AddEdge(3, 4, 1);
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, 6));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            g.AddEdge(0, 4, 7);
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, 6));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 2);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 2);
            g.AddEdge(3, 4, 2);
            g.AddEdge(2, 4, 5);
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, 9));


            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 6);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(0, 4, 1);
            g.AddEdge(4, 5, 1);
            g.AddEdge(5, 3, 1);
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 3, 3));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                }
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 99, 21));


            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 3)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                }
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 99, 23));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 1000);
            for (int i = 0; i < 1000; ++i)
                for (int j = i + 1; j < 1000; ++j)
                {
                    if (rnd.Next(10) <= 3)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                }
            undirectedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 99, 6));

            #endregion

            #region directed, with repetitions
            
            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 1);
            g.AddEdge(3, 4, 1);
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            g.AddEdge(0, 4, 7);
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, 7));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 2);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 2);
            g.AddEdge(3, 4, 2);
            g.AddEdge(2, 4, 5);
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, 9));


            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(0, 4, 1);
            g.AddEdge(4, 5, 1);
            g.AddEdge(5, 3, 1);
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 3,  3));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(j, i, 1+rnd.Next(100));
                }
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 4, 24));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(j, i, 1+rnd.Next(100));
                }
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 99, 22));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 1000);
            for (int i = 0; i < 1000; ++i)
                for (int j = i + 1; j < 1000; ++j)
                {
                    if (rnd.Next(10) <= 3)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                    if (rnd.Next(10) <= 3)
                        g.AddEdge(j, i, 1+rnd.Next(100));
                }
            directedRep.TestCases.Add(new RepSecondPathTestCase(10, g, 0, 99, 5));
            #endregion

            #region undirected, without repetitions

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 1);
            g.AddEdge(3, 4, 1);
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            g.AddEdge(0, 4, 7);
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, 7));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 5);
            g.AddEdge(0, 1, 2);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 2);
            g.AddEdge(3, 4, 2);
            g.AddEdge(2, 4, 5);
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, 9));


            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 6);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(0, 4, 1);
            g.AddEdge(4, 5, 1);
            g.AddEdge(5, 3, 1);
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 3, 3));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                }
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 99, 14));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                }
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 99, 29));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(false, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1+rnd.Next(100));
                }
            undirectedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 99, 24));
            #endregion

            #region directed, without repetitions
          
            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 1);
            g.AddEdge(3, 4, 1);
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, null));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(3, 4, 1);
            g.AddEdge(0, 4, 7);
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, 7));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 5);
            g.AddEdge(0, 1, 2);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 2);
            g.AddEdge(3, 4, 2);
            g.AddEdge(2, 4, 5);
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, 9));


            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 6);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 3, 1);
            g.AddEdge(0, 4, 1);
            g.AddEdge(4, 5, 1);
            g.AddEdge(5, 3, 1);
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 3, 3));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1 + rnd.Next(100));
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(j, i, 1 + rnd.Next(100));
                }
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 4, 29));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 100);
            for (int i = 0; i < 100; ++i)
                for (int j = i + 1; j < 100; ++j)
                {
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(i, j, 1 + rnd.Next(100));
                    if (rnd.Next(10) <= 2)
                        g.AddEdge(j, i, 1 + rnd.Next(100));
                }
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 99, 23));

            g = new AdjacencyListsGraph<HashTableAdjacencyList>(true, 1000);
            for (int i = 0; i < 1000; ++i)
                for (int j = i + 1; j < 1000; ++j)
                {
                    if (rnd.Next(10) <= 3)
                        g.AddEdge(i, j, 1 + rnd.Next(100));
                    if (rnd.Next(10) <= 3)
                        g.AddEdge(j, i, 1 + rnd.Next(100));
                }
            directedSimple.TestCases.Add(new SimpleSecondPathTestCase(10, g, 0, 99, 5));
            #endregion

            Console.WriteLine("Path with repetitions, undirected graphs");
            undirectedRep.PreformTests(true, false);
            Console.WriteLine("Path with repetitions, directed graphs");
            directedRep.PreformTests(true, false);
            Console.WriteLine("Path without repetitions, undirected graphs");
            undirectedSimple.PreformTests(true, false);
            Console.WriteLine("Path without repetitions, directed graphs");
            directedSimple.PreformTests(true, false);
        }
    }
}


