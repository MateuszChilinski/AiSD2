using System;
using ASD.Graphs;

namespace ASD
{


    public static class SmurfphoneFactory
    {
        /// <summary>
        /// Metoda zwraca największą możliwą do wyprodukowania liczbę smerfonów
        /// </summary>
        /// <param name="providers">Dostawcy</param>
        /// <param name="factories">Fabryki</param>
        /// <param name="distanceCostMultiplier">współczynnik kosztu przewozu</param>
        /// <param name="productionCost">Łączny koszt produkcji wszystkich smerfonów</param>
        /// <param name="transport">Tablica opisująca ilości transportowanych surowców miedzy poszczególnymi dostawcami i fabrykami</param>
        /// <param name="maximumProduction">Maksymalny rozmiar produkcji</param>
        public static double CalculateFlow(Provider[] providers, Factory[] factories, double distanceCostMultiplier, out double productionCost, out int[,] transport, int maximumProduction = int.MaxValue)
        {
            Graph gFlow = new AdjacencyListsGraph<SimpleAdjacencyList>(true, providers.Length+factories.Length*2+3);
            Graph gCost = new AdjacencyListsGraph<SimpleAdjacencyList>(true, providers.Length+factories.Length*2+3);

            int i = 1;
            for(int z = 1; z < providers.Length+1; z++)
            {
                gFlow.AddEdge(0, z, providers[z-1].Capacity);
                gCost.AddEdge(0, z, providers[z-1].Cost);
            }


            foreach (var provider in providers)
            {
                int k = providers.Length+1;
                foreach(var factory in factories)
                {
                    double distance = Math.Ceiling(Math.Sqrt((factory.Position.X - provider.Position.X)*(factory.Position.X - provider.Position.X) + (factory.Position.Y - provider.Position.Y)* (factory.Position.Y - provider.Position.Y)) * distanceCostMultiplier);
                    gFlow.AddEdge(new Edge(i, k, double.MaxValue));
                    gCost.AddEdge(new Edge(i, k, distance));
                    gFlow.AddEdge(new Edge(i, k+factories.Length, double.MaxValue));
                    gCost.AddEdge(new Edge(i, k + factories.Length, distance));
                    k++;
                }
                i++;
            }
            for (int z = providers.Length + 1; z < factories.Length + providers.Length + 1; z++)
            {
                    gFlow.AddEdge(z, providers.Length + factories.Length*2 + 1, factories[z - providers.Length - 1].Limit);
                    gCost.AddEdge(z, providers.Length + factories.Length*2 + 1, factories[z - providers.Length - 1].LowerCost);
                    gFlow.AddEdge(z + factories.Length, providers.Length + factories.Length*2 + 1, int.MaxValue);
                    gCost.AddEdge(z + factories.Length, providers.Length + factories.Length*2 + 1, factories[z - providers.Length - 1].HigherCost);
            }
            Graph fl;
            gCost.Add(new Edge(providers.Length + factories.Length * 2 + 1, providers.Length + factories.Length * 2 + 2, 0));
            gFlow.Add(new Edge(providers.Length + factories.Length * 2 + 1, providers.Length + factories.Length * 2 + 2, maximumProduction));
            double max = gFlow.MinCostFlow(gCost, 0, providers.Length + factories.Length*2 + 2, out productionCost, out fl);

            transport = new int[providers.Length, factories.Length];
            for (int z = 1; z < providers.Length+1; z++)
            {
                foreach (Edge e in fl.OutEdges(z))
                {
                    int to = e.To;
                    if(to > providers.Length+factories.Length)
                        to -= factories.Length;
                    transport[e.From-1, to-providers.Length-1] += (int)e.Weight;
                }
            }
            
            return max;
        }
    }
}
