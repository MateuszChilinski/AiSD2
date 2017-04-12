using System;
using System.Collections.Generic;
using System.Linq;
using ASD.Graphs;

namespace ASD
{
    public static class GraphColoring
    {
        /// <summary>
        /// Ogólna metoda - kolorowanie zachłanne na podstawie ustalonego ciągu wierzchołków.
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <param name="order">Porządek wierzchołków, w jakim mają być one przetwarzane. W przypadku null użyj pierwotnego numerowania wierzchołków</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] GreedyColoring(this Graph g, int[] order = null)
        {
            Graph gC = g.Clone();
            int[] colours = new int[gC.VerticesCount];
            if (order == null)
            {
                order = new int[gC.VerticesCount];
                for (int i = 0; i < gC.VerticesCount; i++)
                {
                    order[i] = i;
                }
            }
            foreach (var i in order)
            {
                HashSet<int> usedColours = new HashSet<int>();
                int largestColour = 1;
                foreach (var edge in gC.OutEdges(i))
                {
                    if (largestColour <= colours[edge.To])
                        largestColour = colours[edge.To] + 1;
                    usedColours.Add(colours[edge.To]);
                }
                for (int j = 1; j <= largestColour; j++)
                {
                    if (!usedColours.Contains(j))
                    {
                        colours[i] = j;
                        break;
                    }
                }
            }
            return colours;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą BFS
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        static int[] sColors;
        static Graph gC;
        public static int[] BFSColoring(this Graph g)
        {
            gC = g.Clone();
            int cc;
            sColors = new int[gC.VerticesCount];
            gC.GeneralSearchAll<EdgesQueue>(visitEdgeF, null, null, out cc);
            return sColors;
        }

        private static bool visitEdgeF(int i)
        {
            HashSet<int> usedColours = new HashSet<int>();
            int largestColour = 1;
            foreach (var edge in gC.OutEdges(i))
            {
                if (largestColour <= sColors[edge.To])
                    largestColour = sColors[edge.To] + 1;
                usedColours.Add(sColors[edge.To]);
            }
            for (int j = 1; j <= largestColour; j++)
            {
                if (!usedColours.Contains(j))
                {
                    sColors[i] = j;
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą LargestBackDegree - niedziałająca
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] LargestBackDegree(this Graph g)
        {
            return null;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą ColorDegreeOrdering
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] ColorDegreeOrdering(this Graph g)
        {
            int[] colourDegree = new int[g.VerticesCount];
            int[] colours = new int[g.VerticesCount];
            while (colourDegree.Max() != -1)
            {
                int highest = colourDegree.Max();
                int curr = -1;
                for (int i = 0; i < colourDegree.Length; i++)
                {
                    if (colourDegree[i] == highest)
                    {
                        curr = i;
                        colourDegree[i] = -1;
                        foreach (var edge in g.OutEdges(i))
                        {
                            if (colourDegree[edge.To] != -1)
                                colourDegree[edge.To]++;
                        }

                        HashSet<int> usedColours = new HashSet<int>();
                        int largestColour = 1;
                        foreach (var edge in g.OutEdges(i))
                        {
                            if (largestColour <= colours[edge.To])
                                largestColour = colours[edge.To] + 1;
                            usedColours.Add(colours[edge.To]);
                        }
                        for (int j = 1; j <= largestColour; j++)
                        {
                            if (!usedColours.Contains(j))
                            {
                                colours[i] = j;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return colours;
        }

        /// <summary>
        /// Przybliżone kolorowanie metodą Incremental
        /// </summary>
        /// <param name="g">Graf do pokolorowania</param>
        /// <returns>Tablica kolorów: na i-tej pozycji ma być kolor i-tego wierzchołka. Kolory numerujemy od 1.</returns>
        /// <remarks>
        /// 1) Algorytm powinien działać dla grafów nieskierowanych, niespójnych
        /// 2) Grafu nie wolno zmieniać
        /// </remarks>
        public static int[] Incremental(this Graph g)
        {
            int[] colours = new int[g.VerticesCount];

            int c = 1;
            while (colours.Min() == 0)
            {
                HashSet<int> usedVerts = new HashSet<int>();
                for (int i = 0; i < g.VerticesCount; i++)
                {
                    if (colours[i] != 0)
                        continue;
                    bool add = true;
                    foreach (var edge in g.OutEdges(i))
                    {
                        if (usedVerts.Contains(edge.To))
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        usedVerts.Add(i);
                        colours[i] = c;
                    }
                }
                c++;
            }
            return colours;
        }
    }
}
