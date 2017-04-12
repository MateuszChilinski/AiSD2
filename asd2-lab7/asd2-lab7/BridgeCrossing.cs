using System;
using System.Collections.Generic;
using System.Linq;

namespace ASD
{
    public class BridgeCrossing
    {
        /// <summary>
        ///     Metoda rozwiązuje zadanie optymalnego przechodzenia przez most.
        /// </summary>
        /// <param name="_times">Tablica z czasami przejścia poszczególnych osób</param>
        /// <param name="strategy">
        ///     Strategia przekraczania mostu: lista list identyfikatorów kolejnych osób,
        ///     które przekraczają most (na miejscach parzystych przejścia par przez most,
        ///     na miejscach nieparzystych powroty jednej osoby z latarką). Jeśli istnieje więcej niż jedna strategia
        ///     realizująca przejście w optymalnym czasie wystarczy zwrócić dowolną z nich.
        /// </param>
        /// <returns>Minimalny czas, w jakim wszyscy turyści mogą pokonać most</returns>
        public static int minTime;

        public static List<List<int>> minList;
        public static Dictionary<int, int> timesTo;

        public static int CrossBridge(int[] times, out List<List<int>> strategy)
        {
            if (times.Length == 1)
            {
                strategy = new List<List<int>>();
                var z = new List<int>();
                z.Add(0);
                strategy.Add(z);
                return times[0];
            }
            minTime = int.MaxValue;
            minList = new List<List<int>>();
            timesTo = new Dictionary<int, int>();
            var k = 0;
            foreach (var time in times)
                if (!timesTo.ContainsKey(time))
                    timesTo.Add(time, k++);
            var beforeB = new Dictionary<int, int>();
            var afterB = new Dictionary<int, int>();
            for (var i = 0; i < times.Length; i++)
                if (beforeB.ContainsKey(times[i]))
                    beforeB[times[i]] += 1;
                else
                    beforeB.Add(times[i], 1);
            Calculate(beforeB, afterB, 0, false, minList);
            var usedV = new bool[times.Length];
            foreach (var smallList in minList)
            {
                var changeTo = -1;
                if (smallList.Count == 1)
                {
                    for (var i = 0; i < times.Length; i++)
                    {
                        if (times[i] == beforeB.ElementAt(smallList[0]).Key && usedV[i])
                        {
                            smallList[0] = i;
                            usedV[i] = false;
                            break;
                        }
                    }
                    continue;
                }
                if (smallList[0] == smallList[1])
                {
                    var n = 0;
                    var search_for = smallList[0];
                    for (var i = 0; i < times.Length; i++)
                    {
                        if (times[i] == beforeB.ElementAt(search_for).Key && !usedV[i])
                        {
                            smallList[n++] = i;
                            usedV[i] = true;
                        }
                        if (n > 1)
                            break;
                    }
                    continue;
                }
                int z = 0;
                int[] c = new int[2];
                c[0] = smallList[0];
                c[1] = smallList[1];
                foreach (var smallElement in smallList)
                {
                    for (var i = 0; i < times.Length; i++)
                        if (times[i] == beforeB.ElementAt(smallElement).Key && !usedV[i])
                        {
                            usedV[i] = true;
                            c[z++] = i;
                            break;
                        }
                }
                smallList[0] = c[0];
                smallList[1] = c[1];
            }
            strategy = minList;
            return minTime;
        }

        public static int findMinKey(Dictionary<int, int> afterB)
        {
            var min = int.MaxValue;
            foreach (var k1 in afterB)
                if (min > k1.Key)
                    min = k1.Key;
            return min;
        }

        public static void goToOtherSide(Dictionary<int, int> beforeB, Dictionary<int, int> afterB, int k1, bool rtrn)
        {
            if (rtrn)
            {
                if (beforeB.ContainsKey(k1))
                    beforeB[k1] += 1;
                else
                    beforeB.Add(k1, 1);
                if (afterB.ContainsKey(k1) && afterB[k1] == 1)
                    afterB.Remove(k1);
                else
                    afterB[k1]--;
            }
            else
            {
                if (afterB.ContainsKey(k1))
                    afterB[k1] += 1;
                else
                    afterB.Add(k1, 1);
                if (beforeB.ContainsKey(k1) && beforeB[k1] == 1)
                    beforeB.Remove(k1);
                else
                    beforeB[k1]--;
            }
        }

        public static void Calculate(Dictionary<int, int> beforeB, Dictionary<int, int> afterB, int currTime, bool rtrn,
            List<List<int>> currList)
        {
            if (currTime > minTime) return;
            if (beforeB.Count == 0)
            {
                if (currTime < minTime)
                {
                    minList = currList;
                    minTime = currTime;
                }
                return;
            }

            if (!rtrn)
            {
                for (var i = 0; i < beforeB.Count; i++)
                for (var j = i; j < beforeB.Count; j++)
                {
                    var k1 = beforeB.ElementAt(i);
                    var k2 = beforeB.ElementAt(j);
                    if (k1.Equals(k2) && k1.Value < 2)
                        continue;
                    var beforeB_temp = new Dictionary<int, int>(beforeB);
                    var afterB_temp = new Dictionary<int, int>(afterB);
                    var tempList = new List<List<int>>(currList);
                    var tmpL = new List<int>();
                    tmpL.Add(timesTo[k1.Key]);
                    tmpL.Add(timesTo[k2.Key]);
                    tempList.Add(tmpL);
                    goToOtherSide(beforeB_temp, afterB_temp, k1.Key, false);
                    goToOtherSide(beforeB_temp, afterB_temp, k2.Key, false);
                    if (currTime + Math.Max(k1.Key, k2.Key) > minTime) continue;
                    currTime += Math.Max(k1.Key, k2.Key);
                    Calculate(beforeB_temp, afterB_temp, currTime, true, tempList);
                    currTime -= Math.Max(k1.Key, k2.Key);
                }
            }
            else
            {
                var beforeB_temp = new Dictionary<int, int>(beforeB);
                var afterB_temp = new Dictionary<int, int>(afterB);
                var currDude = findMinKey(afterB_temp);
                var tempList = new List<List<int>>(currList);
                var tmpL = new List<int>();
                tmpL.Add(timesTo[currDude]);
                tempList.Add(tmpL);
                goToOtherSide(beforeB_temp, afterB_temp, currDude, true);
                currTime += currDude;
                Calculate(beforeB_temp, afterB_temp, currTime, false, tempList);
                currTime -= currDude;
            }
        }

        // MOŻESZ NAWET DODAĆ CAŁE KLASY (ALE NIE MUSISZ)
        // MOŻESZ DOPISAĆ POTRZEBNE POLA I METODY POMOCNICZE
    }
}