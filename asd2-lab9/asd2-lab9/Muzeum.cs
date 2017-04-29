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
            //gz.Export(nG);
            double z = nG.FordFulkersonDinicMaxFlow(0, g.VerticesCount*2+1, out nG, MaxFlowGraphExtender.DFSBlockingFlow);




            List<List<int>> tmp = new List<List<int>>();
            HashSet<int> eks = new HashSet<int>();
            foreach(var y in exits)
            {
                eks.Add(y);
            }
            for(int i = 0; i < z; i++)
            {
                var inTmp = new List<int>();
                
                int curr = 0;
                HashSet<int> used = new HashSet<int>();
                bool done = false;
                while (curr != g.VerticesCount*2+1 && !done)
                {
                    if(eks.Contains(curr-1))
                    {
                        done = true;
                    }
                    if(curr != 0 && curr <= g.VerticesCount && !used.Contains(curr-1))
                    {
                        inTmp.Add(curr-1);
                        used.Add(curr - 1);
                    }
                    Edge tmpE = new Edge();
                    foreach(var e in nG.OutEdges(curr))
                    {
                        tmpE = e;
                        curr = e.To;
                        break;
                    }
                    nG.DelEdge(tmpE);
                    nG.AddEdge(tmpE.From, tmpE.To, tmpE.Weight - 1);
                }
                tmp.Add(inTmp);
            }
            int[][] tmpA = new int[tmp.Count][];
            int k = 0;
            foreach(var ark in tmp)
            {
                tmpA[k++] = ark.ToArray();
            }
            return new MuseumRoutes((int)z, tmpA);
       }
    }
}

