using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASD.Graphs;

namespace ASD
{
    public static class MatchingGraphExtender
    {
        /// <summary>
        /// Podział grafu na cykle. Zakładamy, że dostajemy graf nieskierowany i wszystkie wierzchołki grafu mają parzyste stopnie
        /// (nie trzeba sprawdzać poprawności danych).
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Tablica cykli; krawędzie każdego cyklu powinny być uporządkowane zgodnie z kolejnością na cyklu, zaczynając od dowolnej</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m)
        /// </remarks>
        public static Edge[][] cyclePartition(this Graph G_o)
        {
            int size = 10000;
            Graph G = G_o.Clone();
            Edge[][] cycles = new Edge[size][];
            bool[] usedVerts = new bool[G.VerticesCount];
            Stack<int> verts = new Stack<int>();
            verts.Push(0);
            int[] prevs = new int[size];
            int prev;
            int curr = -1;
            Queue<int> cyc = new Queue<int>();
            HashSet<Edge> used = new HashSet<Edge>();
            int used_all = 0;
            int k = 0;
            while (verts.Count != 0 || used_all < G.VerticesCount)
            {
                if(verts.Count == 0)
                {
                    for(int i = 0; i < G.VerticesCount; i++)
                    {
                        if(usedVerts[i] == false)
                        {
                            verts.Push(i);
                            break;
                        }
                    }
                }
                prev = curr;
                curr = verts.Pop();
                prevs[curr] = prev;
                foreach (Edge e in G.OutEdges(curr))
                {
                    if (e.To == prev)
                        continue;
                    if (usedVerts[e.To] == true)
                    {
                        prevs[e.To] = curr;
                        int temp_curr = curr;
                        int z = 0;
                        cycles[k] = new Edge[size];
                        HashSet<int> cycle = new HashSet<int>();
                        do
                        {
                            cycle.Add(temp_curr);
                            G.DelEdge(new Edge(temp_curr, prevs[temp_curr]));
                            cycles[k][z++] = new Edge(temp_curr, prevs[temp_curr]);
                            if (verts.Contains(temp_curr))
                            {
                                int check = verts.Pop();
                                if (check != temp_curr)
                                    verts.Push(check);
                                else
                                {
                                    usedVerts[check] = false;
                                }
                            }
                            temp_curr = prevs[temp_curr];
                        }
                        while ((!cycle.Contains(temp_curr)) && temp_curr != curr);
                        Array.Resize(ref cycles[k++], z);
                        break;
                    }
                    else
                    {
                        verts.Push(e.To);
                    }
                }
                if(usedVerts[curr] != true)
                    used_all++;
                usedVerts[curr] = true;
                
            }
            Array.Resize(ref cycles, k);
            return cycles;
        }

        /// <summary>
        /// Szukanie skojarzenia doskonałego w grafie nieskierowanym o którym zakładamy, że jest dwudzielny i 2^r-regularny
        /// (nie trzeba sprawdzać poprawności danych)
        /// </summary>
        /// <param name="G">Badany graf</param>
        /// <returns>Skojarzenie doskonałe w G</returns>
        /// <remarks>
        /// Metoda powinna działać w czasie O(m), gdzie m jest liczbą krawędzi grafu G
        /// </remarks>
        public static Graph perfectMatching(this Graph G)
        {
            return null;
        }
    }
}
