using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{

    public partial struct Point
    {

        /// <summary>
        /// Metoda zwraca odległość w przestrzeni Euklidesowej między dwoma punktami w przestrzeni dwuwymiarowej
        /// </summary>
        /// <param name="p1">Pierwszy punkt w przestrzeni dwuwymiarowej</param>
        /// <param name="p2">Drugi punkt w przestrzeni dwuwymiarowej</param>
        /// <returns>Odległość w przestrzeni Euklidesowej między dwoma punktami w przestrzeni dwuwymiarowej</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(1)
        /// </remarks>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
        }

    }

    public class ByY : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            if(p1.y == p2.y)
            {
                return p1.x.CompareTo(p2.x);
            }
            return p1.y.CompareTo(p2.y);
        }
    }
    public class ByX : IComparer<Point>
    {
        public int Compare(Point p1, Point p2)
        {
            return p1.x.CompareTo(p2.x);
        }
    }
    class SweepClosestPair
    {

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność O(n^2), gdzie n to liczba punktów w chmurze
        /// </remarks>
        public static Tuple<Point, Point> FindClosestPointsBrute(List<Point> points, out double minDistance)
        {
            minDistance = double.MaxValue;
            Point p1m = new Point(), p2m = new Point();
            foreach (var p1 in points)
            {
                foreach (var p2 in points)
                {
                    if (p1 != p2)
                    {
                        double dist = Point.Distance(p1, p2);
                        if (dist < minDistance)
                        {
                            p1m = p1;
                            p2m = p2;
                            minDistance = dist;
                        }
                    }
                }
            }
            return new Tuple<Point, Point>(p1m, p2m);
        }

        /// <summary>
        /// Metoda zwraca dwa najbliższe punkty w dwuwymiarowej przestrzeni Euklidesowej
        /// </summary>
        /// <param name="points">Chmura punktów</param>
        /// <param name="minDistance">Odległość pomiędzy najbliższymi punktami</param>
        /// <returns>Para najbliższych punktów. Kolejność nie ma znaczenia</returns>
        /// <remarks>
        /// 1) Algorytm powinien mieć złożoność n*logn, gdzie n to liczba punktów w chmurze
        /// </remarks>
        public static Tuple<Point, Point> FindClosestPoints(List<Point> points, out double minDistance)
        {
            points.Sort(new ByX());
            SortedSet<Point> D = new SortedSet<Point>(new ByY());
            minDistance = double.MaxValue;
            Point p1m = new Point(), p2m = new Point();
            HashSet<Point> tD = new HashSet<Point>();
            int lastD = 0;
            for(int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                for(int k = lastD; k < i && points[i].x - points[k].x > minDistance; k++)
                {
                    lastD = k;
                    D.Remove(points[k]);
                }
                foreach (var pTd in D.GetViewBetween(new Point(point.x-minDistance,point.y-minDistance), new Point(point.x,point.y+minDistance)))
                {
                        double dist = Point.Distance(pTd, point);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            p1m = pTd;
                            p2m = point;
                        }
                }
                D.Add(point);
            }
            return new Tuple<Point, Point>(p1m, p2m);
        }

    }

}
