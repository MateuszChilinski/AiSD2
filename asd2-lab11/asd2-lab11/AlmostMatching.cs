
using System.Collections.Generic;
using ASD.Graphs;

namespace lab10
{

public struct AlmostMatchingSolution
    {
        public AlmostMatchingSolution(int edgesCount, List<Edge> solution)
            {
            this.edgesCount=edgesCount;
            this.solution=solution;
            }

        public readonly int edgesCount;
        public readonly List<Edge> solution;
    }



public class AlmostMatching
    {

        /// <summary>
        /// Zwraca najliczniejszy możliwy zbiór krawędzi, którego poziom
        /// ryzyka nie przekracza limitu. W ostatnim etapie zwracać
        /// zbiór o najmniejszej sumie wag ze wszystkich najliczniejszych.
        /// </summary>
        /// <returns>Liczba i lista linek (krawędzi)</returns>
        /// <param name="g">Graf linek</param>
        /// <param name="allowedCollisions">Limit ryzyka</param>
        static Graph realG;
        static int maximum = 0;
        static int allC = 0;
        static List<Edge> bestList;
        public static AlmostMatchingSolution LargestS(Graph g, int allowedCollisions)
            {
             realG = g;
            allC = allowedCollisions;
            List<Edge> allEdges = new List<Edge>();
            for (int i = 0; i < realG.VerticesCount; i++)
            {
                foreach (var e in realG.OutEdges(i))
                {
                    if(!allEdges.Contains(e) && !allEdges.Contains(new Edge(e.To, e.From)))
                        allEdges.Add(e);
                }
            }
            List<Edge> edgeList = new List<Edge>();
            List<Edge> myEdgeList = new List<Edge>();
            int z = Search(edgeList, allEdges);
             return new AlmostMatchingSolution(z, bestList);
            }
        public static int Search(List<Edge> prevList, List<Edge> allEdges)
        {
            if(prevList.Count >= realG.VerticesCount/2 + allC || allEdges.Count == 0)
            {
                bestList = new List<Edge>(prevList);
                return prevList.Count;
            }
            int max = 0;
                Edge e = allEdges[0];
                allEdges.Remove(e);
                prevList.Add(e);
                int tmp = Search(prevList, allEdges);
                if (tmp > max)
                {
                    if (tmp > maximum)
                    {
                        maximum = tmp;
                    }
                    max = tmp;
                }
                else
                {
                    allEdges.Add(e);
                }
                prevList.Remove(e);
            return max;
        }
    }

}


