using System;
using System.Collections.Generic;

using static ASD.Geometry;

namespace ASD
{
    public static class SutherlandHodgman
    {
        /// <summary>
        /// Oblicza pole wielokata przy pomocy formuly Gaussa
        /// </summary>
        /// <param name="polygon">Kolejne wierzcholki wielokata</param>
        /// <returns>Pole wielokata</returns>
        public static double PolygonArea(this Point[] polygon)
        {
            double curr = 0;
            for(int i = 1; i < polygon.Length-1; i++)
            {
                curr += (polygon[i].x-polygon[0].x) * (polygon[i + 1].y-polygon[0].y) - (polygon[i+1].x - polygon[0].x) * (polygon[i].y - polygon[0].y);
            }
            return 0.5*Math.Abs(curr);
        }

        /// <summary>
        /// Sprawdza, czy dwa punkty leza po tej samej stronie prostej wyznaczonej przez odcinek s
        /// </summary>
        /// <param name="p1">Pierwszy punkt</param>
        /// <param name="p2">Drugi punkt</param>
        /// <param name="s">Odcinek wyznaczajacy prosta</param>
        /// <returns>
        /// True, jesli punkty leza po tej samej stronie prostej wyznaczonej przez odcinek 
        /// (lub co najmniej jeden z punktow lezy na prostej). W przeciwnym przypadku zwraca false.
        /// </returns>
        public static bool IsSameSide(Point p1, Point p2, Segment s)
        {
            double result1 = (s.pe.x - s.ps.x) * (p1.y - s.ps.y) - (p1.x - s.ps.x) * (s.pe.y - s.ps.y);
            double result2 = (s.pe.x - s.ps.x) * (p2.y - s.ps.y) - (p2.x - s.ps.x) * (s.pe.y - s.ps.y);
            if (result1*result2 >= 0)
                return true;
            return false;
        }

        /// <summary>Oblicza czesc wspolna dwoch wielokatow przy pomocy algorytmu Sutherlanda–Hodgmana</summary>
        /// <param name="subjectPolygon">Wielokat obcinany (wklesly lub wypukly)</param>
        /// <param name="clipPolygon">Wielokat obcinajacy (musi byc wypukly i zakladamy, ze taki jest)</param>
        /// <returns>Czesc wspolna wielokatow</returns>
        /// <remarks>
        /// - mozna zalozyc, ze 3 kolejne punkty w kazdym z wejsciowych wielokatow nie sa wspolliniowe
        /// - wynikiem dzialania funkcji moze byc tak naprawde wiele wielokatow (sytuacja taka moze wystapic,
        ///   jesli wielokat obcinany jest wklesly)
        /// - jesli wielokat obcinany i obcinajacy zawieraja wierzcholki o tych samych wspolrzednych,
        ///   w wynikowym wielokacie moge one byc zduplikowane
        /// - wierzcholki wielokata obcinanego, przez ktore przechodza krawedzie wielokata obcinajacego
        ///   zostaja zduplikowane w wielokacie wyjsciowym
        /// </remarks>
        public static Point[] GetIntersectedPolygon(Point[] subjectPolygon, Point[] clipPolygon)
        {
            List<Point> input = new List<Point>();
            List<Point> output = new List<Point>(subjectPolygon);
            Point inP = new Point((clipPolygon[0].x+clipPolygon[1].x+clipPolygon[2].x)/3.0, (clipPolygon[0].y + clipPolygon[1].y + clipPolygon[2].y) / 3.0);
            for (int i = 0; i < clipPolygon.Length; i++)
            {
                input = new List<Point>(output);
                output.Clear();
                Point pp = input[input.Count - 1];
                foreach (Point p in input)
                {
                    Segment e = new Segment(clipPolygon[i], clipPolygon[(i + 1) % clipPolygon.Length]);
                    if (IsSameSide(p, inP, e))
                    {
                        if(!IsSameSide(pp, inP, e))
                            output.Add(GetIntersectionPoint(new Segment(pp, p), e));
                        output.Add(p);
                    }
                    else
                    {
                        if (IsSameSide(pp, inP, e))
                            output.Add(GetIntersectionPoint(new Segment(pp, p), e));
                    }
                    pp = p;
                }
            }
            for(int i = 0; i < output.Count; i++)
            {
                if (output[i] == output[(i + 1) % output.Count])
                    output.Remove(output[i--]);
            }
            return output.ToArray(); 
        }

        /// <summary>
        /// Zwraca punkt przeciecia dwoch prostych wyznaczonych przez odcinki
        /// </summary>
        /// <param name="seg1">Odcinek pierwszy</param>
        /// <param name="seg2">Odcinek drugi</param>
        /// <returns>Punkt przeciecia prostych wyznaczonych przez odcinki</returns>
        public static Point GetIntersectionPoint(Segment seg1, Segment seg2)
        {
            Point direction1 = new Point(seg1.pe.x - seg1.ps.x, seg1.pe.y - seg1.ps.y);
            Point direction2 = new Point(seg2.pe.x - seg2.ps.x, seg2.pe.y - seg2.ps.y);
            double dotPerp = (direction1.x * direction2.y) - (direction1.y * direction2.x);

            Point c = new Point(seg2.ps.x - seg1.ps.x, seg2.ps.y - seg1.ps.y);
            double t = (c.x * direction2.y - c.y * direction2.x) / dotPerp;

            return new Point(seg1.ps.x + (t * direction1.x), seg1.ps.y + (t * direction1.y));
        }
    }
}
