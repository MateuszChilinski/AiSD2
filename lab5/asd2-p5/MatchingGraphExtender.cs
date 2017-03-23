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
            Graph G = G_o.Clone();
            Stack<int> verticlesStack = new Stack<int>();
            verticlesStack.Push(0);
            List<int> cycleList = new List<int>();
            bool[] visitedVerticles = new bool[G.VerticesCount];
            int k = 0;
            List<int> temporaryCycleList = new List<int>();

            while (verticlesStack.Count != 0)
            {
                int currentVerticle = verticlesStack.Pop();
                foreach (Edge currentEdge in G.OutEdges(currentVerticle))
                {
                    if(visitedVerticles[currentEdge.To] && cycleList.Last() != currentEdge.To)
                    {
                        cycleList.Add(currentEdge.To);
                        cycleList.Reverse();
                        int currentElement = cycleList.First();
                        do
                        {
                            temporaryCycleList.Add(currentElement);
                            cycleList.RemoveAt(0);
                            currentElement = cycleList.First();
                        }
                        while (currentElement != currentVerticle);
                        k++;
                        // found cycle!
                    }
                    verticlesStack.Push(currentEdge.To);
                }
                cycleList.Add(currentVerticle);
                visitedVerticles[currentVerticle] = true;
            }
            Edge[][] cycles = new Edge[k+1][];
            for(int i = 0; i < k; i++)
            {
                cycles[i] = new Edge[temporaryCycleList.Count];
                for(int z = 0; z < temporaryCycleList.Count; z++)
                {
                    if(z == 0)
                    {
                        cycles[i][z] = new Edge(temporaryCycleList.Last(), temporaryCycleList.First());
                        continue;
                    }
                    else
                    {
                        cycles[i][z] = new Edge(temporaryCycleList.ElementAt(z - 1), temporaryCycleList.ElementAt(z));
                    }
                }
            }
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
