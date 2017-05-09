using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace Lab9
{
public struct MuseumRoutes
    {
        public MuseumRoutes(int count, int[][] routes)
            {
            this.liczba = count;
            this.trasy = routes;
            }

        public readonly int liczba;
        public readonly int[][] trasy;
    }


static class Muzeum
    {
        /// <summary>
        /// Znajduje najliczniejszy multizbiór tras
        /// </summary>
        /// <returns>Znaleziony multizbiór</returns>
        /// <param name="g">Graf opisujący muzeum</param>
        /// <param name="cLevel">Tablica o długości równej liczbie wierzchołków w grafie -- poziomy ciekawości wystaw</param>
        /// <param name="entrances">Wejścia</param>
        /// <param name="exits">Wyjścia</param>
        public static MuseumRoutes FindRoutes(Graph g, int[] cLevel, int[] entrances, int[] exits)
        {
            Graph gC = g.Clone();
            Graph nG = new AdjacencyListsGraph<SimpleAdjacencyList>(true, g.VerticesCount*2+2);// in - 0, out - g.V*2 + 1
            for (int i = 0; i < g.VerticesCount; i++)
            {
                nG.AddEdge(i+1, i+g.VerticesCount+1, cLevel[i]);
                foreach (Edge e in g.OutEdges(i))
                {
                    nG.AddEdge(e.From+1, e.From + g.VerticesCount+1, int.MaxValue);
                    nG.AddEdge(e.From + g.VerticesCount+1, e.To+1, int.MaxValue);
                }
            }
            foreach(var ent in entrances)
            {
                nG.AddEdge(0, ent+1, cLevel[ent]);
            }
            foreach(var ext in exits)
            {
                nG.AddEdge(ext+1, g.VerticesCount * 2 + 1, cLevel[ext]);
            }
            GraphExport gz = new GraphExport();
            //gz.Export(g);
            double z = nG.FordFulkersonDinicMaxFlow(0, g.VerticesCount*2+1, out nG, MaxFlowGraphExtender.DFSBlockingFlow);


            List<int> tmp = new List<int>();
            for (int i = 0; i < nG.VerticesCount; i++)
            {
                foreach (var e in nG.OutEdges(i))
                {
                    if (e.Weight == 0)
                    {
                        nG.DelEdge(e);
                    }
                }
            }
            List<List<int>> tL = new List<List<int>>();
            for (int k = 0; k < z; k++)
            {
                getNext(ref nG, 0, ref tmp);
                tL.Add(tmp);
                tmp = new List<int>();
                for (int i = 0; i < nG.VerticesCount; i++)
                {
                    foreach (var e in nG.OutEdges(i))
                    {
                        if (e.Weight == 0)
                        {
                            nG.DelEdge(e);
                        }
                    }
                }
            }
            int[][] tA = new int[(int)z][];
            for(int i = 0; i < z; i++)
            {
                tA[i] = tL.ToArray()[i].ToArray();
            }
            return new MuseumRoutes((int)z, tA);
       }
       public static void getNext(ref Graph rG, int curr, ref List<int> tmp)
        {
            if (rG.OutEdges(curr).Count() == 0)
            {
                return;
            }
            else
            {
                int f = rG.OutEdges(curr).First().From;
                int t = rG.OutEdges(curr).First().To;
                rG.ModifyEdgeWeight(f, t, -1);
                if (t <= (rG.VerticesCount) / 2 - 1)
                    tmp.Add(t - 1);
                getNext(ref rG, t, ref tmp);
            }
        }
    }
}

