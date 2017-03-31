using ASD.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    public static class Lab06GraphExtender
    {

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki,
        /// algorytm zwróci jedną z nich).
        /// Dopuszczamy, aby na ścieżce powtarzały się wierzchołki/krawędzie.
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność do O(D), gdzie D jest złożonością implementacji alogorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g"></param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako ciąg krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość znalezionej ścieżki</returns>
        public static double? FindSecondShortestPath(this Graph g, int a, int b, out Edge[] path)
        {
            PathsInfo[] d,dC;
            g.DijkstraShortestPaths(a, out d);
            Graph gC;
            if (g.Directed)
                gC = g.Clone().Reverse();
            else
                gC = g;
            gC.DijkstraShortestPaths(b, out dC);
            Edge[] shortestPath = PathsInfo.ConstructPath(a, b, d);
            if(null == shortestPath)
            {
                path = null; 
                return null;
            }
            double minDist = double.MaxValue;
			int c = -1,c2 = -1;
			Edge tE = new Edge();
            bool foundSame = false;
            foreach(Edge e in shortestPath)
            {
                if (foundSame) break;
                foreach(Edge e2 in g.OutEdges(e.From))
                {
                    if (e2.To == e.To)
                        continue;
					double currMin = dC[e2.To].Dist + d[e.From].Dist + e2.Weight;
                    if(minDist > currMin)
                    {
						tE = e2;
						c = e.From;
                        c2 = e2.To;
                        minDist = currMin;
                    }
                    if(currMin == d[b].Dist)
                    {
                        break;
                    }
                }
            }

            if (c2 == -1)
            {
                path = null;
                return null;
            }
            gC.DijkstraShortestPaths(c2, out dC);
            Edge[] shortestPath1 = PathsInfo.ConstructPath(a, c, d);
			Edge[] shortestPath3 = PathsInfo.ConstructPath(c2, b, dC);
            List<Edge> z = new List<Edge>();
            if(g.Directed)
            {
                gC.DijkstraShortestPaths(b, out dC);
                shortestPath3 = PathsInfo.ConstructPath(b, c2, dC);
            }
            foreach(Edge e in shortestPath1)
            {
                z.Add(e);
            }
            z.Add(tE);
            if (shortestPath3 == null)
            {
                path = null;
                return null;
            }
            List<Edge> z2 = new List<Edge>();
            foreach (Edge e in shortestPath3)
            {
                if(!g.Directed)
                    z.Add(e);
                z2.Add(e);
            }
            if (g.Directed)
            {
                z2.Reverse();
                foreach (Edge e in z2)
                {
                    z.Add(new Edge(e.To, e.From, e.Weight));
                }
            }
            path = z.ToArray(); // shortestPath1 + tE + shortestPath3
            return minDist;
        }

        /// <summary>
        /// Algorytm znajdujący drugą pod względem długości najkrótszą ścieżkę między a i b.
        /// Możliwe, że jej długość jest równa najkrótszej (jeśli są dwie najkrótsze ścieżki,
        /// algorytm zwróci jedną z nich).
        /// Wymagamy, aby na ścieżce nie było powtórzeń wierzchołków ani krawędzi.  
        /// Można założyć, że a!=b oraz że w grafie nie występują pętle.
        /// </summary>
        /// <remarks>
        /// Wymagana złożoność to O(nD), gdzie D jest złożonością implementacji algorytmu Dijkstry w bibliotece Graph.
        /// </remarks>
        /// <param name="g"></param>
        /// <param name="path">null jeśli druga ścieżka nie istnieje, wpp ściezka jako ciąg krawędzi</param>
        /// <returns>null jeśli druga ścieżka nie istnieje, wpp długość tej ścieżki</returns>
        public static double? FindSecondSimpleShortestPath(this Graph g, int a, int b, out Edge[] path)
        {
            PathsInfo[] d;
            g.DijkstraShortestPaths(a, out d);
            Edge[] shortestPath = PathsInfo.ConstructPath(a, b, d);
            if (shortestPath == null)
            {
                path = null;
                return null;
            }
            double minDist = double.MaxValue;
            bool changed = false;
            Edge[] temp = null;
            foreach (Edge e in shortestPath)
            {
                PathsInfo[] dT;
                Graph tG = g.Clone();
                tG.DelEdge(e);
                tG.DijkstraShortestPaths(a, out dT);
                Edge[] secondShortest = PathsInfo.ConstructPath(a, b, dT);
                if (dT[b].Dist <= minDist)
                {
                    changed = true;
                    minDist = dT[b].Dist;
                    temp = PathsInfo.ConstructPath(a, b, dT);
                }
            }
            if (!changed)
            {
                path = null;
                return null;
            }
            path = temp;
            return minDist;
        }
    }
}
