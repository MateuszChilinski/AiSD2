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
            List<Edge> cycleList = new List<Edge>();
            bool[] visitedVerticles = new bool[G.VerticesCount];
            int k = 0;
            int used = 0;
            List<Edge> temporaryCycleList = new List<Edge>();
            List<List<Edge>> temporaryCycles = new List<List<Edge>>();
            while (verticlesStack.Count != 0 || used < G.VerticesCount)
            {
                if(verticlesStack.Count == 0)
                {
                    for(int z = 0; z < G.VerticesCount; z++)
                    {
                        if(visitedVerticles[z] == false)
                        {
                            verticlesStack.Push(z);
                            break;
                        }
                    }
                    if (verticlesStack.Count == 0)
                        break;
                }
                int currentVerticle = verticlesStack.Pop();
                if (visitedVerticles[currentVerticle])
                    continue;
                int i = 0;
                Edge prev=new Edge();
                bool add = true;
                foreach (Edge cE in G.OutEdges(currentVerticle))
                {
                    Edge currentEdge = cE;
                    bool already_added = false;
                    i++;
                    if (cycleList.Contains(currentEdge) || cycleList.Contains(new Edge(currentEdge.To, currentEdge.From)))
                    {
                        if (i != G.OutDegree(currentVerticle))
                        {
                            prev = currentEdge;
                            continue;
                        }
                        else
                        {
                            already_added = true;
                            cycleList.Add(prev);
                            currentEdge = prev;
                        }
                    }
                    else if(i == G.OutDegree(currentVerticle))
                    {
                        already_added = true;
                        cycleList.Add(currentEdge);
                    }
                    prev = currentEdge;
                    if (visitedVerticles[currentEdge.To])
                    {
                        if (!already_added)
                            cycleList.Add(currentEdge);
                        
                        Edge currentElement = cycleList.Last();
                        Stack<int> tempStack = new Stack<int>();
                        HashSet<int> toRemoveFromStack = new HashSet<int>();
                        while (currentElement.From != currentEdge.To)
                        {
                            toRemoveFromStack.Add(currentElement.From);
                            used--;
                            visitedVerticles[currentElement.From] = false;
                            visitedVerticles[currentElement.To] = false;
                            temporaryCycleList.Add(new Edge(currentElement.To, currentElement.From));
                            G.DelEdge(currentElement);
                            cycleList.RemoveAt(cycleList.Count-1);
                            currentElement = cycleList.Last();
                        }
                        temporaryCycleList.Add(new Edge(currentElement.To, currentElement.From));
                        cycleList.Remove(currentElement);
                        G.DelEdge(currentElement);
                        k++;
                        while (verticlesStack.Count != 0)
                        {
                            int checkVert = verticlesStack.Pop();
                            if(!toRemoveFromStack.Contains(checkVert))
                            {
                                tempStack.Push(checkVert);
                            }
                        }
                        verticlesStack = tempStack;
                        temporaryCycles.Add(temporaryCycleList);
                        temporaryCycleList = new List<Edge>();
                        visitedVerticles[currentEdge.To] = false;
                        visitedVerticles[currentEdge.From] = false;
                        add = false;
                        verticlesStack.Push(currentEdge.To);
                        break;
                        // found cycle!
                    }
                    if (visitedVerticles[currentEdge.To] == false)
                    {
                        used++;
                        verticlesStack.Push(currentEdge.To);
                    }
                }
                if(add)
                visitedVerticles[currentVerticle] = true;
            }
            Edge[][] cycles = new Edge[temporaryCycles.Count][];
            for (int i = 0; i < temporaryCycles.Count; i++)
                cycles[i] = temporaryCycles[i].ToArray();
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
            Graph Gc = G.Clone();
            int howMany = (int) Math.Log(Gc.OutDegree(0), 2);
            for (int k = 0; k < howMany; k++)
            {
                Edge[][] cycles = cyclePartition(Gc);
                for (int i = 0; i < cycles.Length; i++)
                {
                    Edge[] cycle = cycles[i];
                    for (int j = 0; j < cycle.Length; j++)
                    {
                        if (j % 2 == 0)
                            Gc.DelEdge(cycle[j]);
                    }
                }
            }
            return Gc;
        }
    }
}
