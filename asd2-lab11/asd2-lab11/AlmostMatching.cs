
using System.Collections.Generic;
using ASD.Graphs;
using System.Linq;

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
        static int max = 0;
        static int allC = 0;
        static double min;
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
                    if(!allEdges.Contains(e) && !allEdges.Contains(new Edge(e.To, e.From, e.Weight)))
                        allEdges.Add(e);
                }
            }
            max = 0;
            min = double.MaxValue;
            bestList = null;
            List<Edge> edgeList = new List<Edge>();
            List<Edge> myEdgeList = new List<Edge>();
            int[] verts = new int[g.VerticesCount];
            int used=0;
            int z = Search(edgeList, allEdges, verts, used);
            return new AlmostMatchingSolution(max, bestList);
            }
        public static int Search(List<Edge> prevList, List<Edge> allEdges, int[] verts, int used)
        {
            if (allEdges.Count + prevList.Count < max)
                return prevList.Count;
            if(allEdges.Count == 0 && used <= allC)
            {
                if (max <= prevList.Count)
                {
                    if (max == prevList.Count)
                    {
                        double tmp_min = 0.0;
                        foreach(Edge ed in prevList)
                        {
                            tmp_min += ed.Weight;
                            if(tmp_min >= min)
                            return prevList.Count;
                        }
                        if (tmp_min < min)
                        {
                            min = tmp_min;
                            bestList = new List<Edge>(prevList);
                        }
                    }
                    else
                    {
                        min = (from s in prevList
                              select s.Weight).Sum();
                        max = prevList.Count;
                        bestList = new List<Edge>(prevList);
                    }
                }
                return prevList.Count;
            }
            else if(used > allC)
            {
                return -1;
            }
            Edge e = allEdges[0];
            allEdges.Remove(e);
            prevList.Add(e);
            bool from = false;
            bool to = false;
            if (++verts[e.From] > 1) { used++; from = true; }
            if (++verts[e.To] > 1) { used++; to = true; }
            Search(new List<Edge>(prevList), new List<Edge>(allEdges), verts, used);
            prevList.Remove(e);
            --verts[e.From];
            --verts[e.To];
            if (from) { used--; }
            if (to) { used--; }
            Search(new List<Edge>(prevList), new List<Edge>(allEdges), verts, used);
            
            return prevList.Count;
        }
    }

}


