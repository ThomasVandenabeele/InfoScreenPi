using System;
using System.Collections.Generic;
using System.Threading;
using InfoScreenPi.Entities;

namespace InfoScreenPi.Extensions
{
    public static class ExtensionMethods
    {

        private static Random rng = new Random();  

        /*
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }*/

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static DateTime ParseDate(this string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result;
            else
                return DateTime.MinValue;
        }

        public static List<Item> CleanList(this List<Item> lijst){
            if(lijst.Count == 3) return lijst;
            for(int i = 0; i < lijst.Count - 2; i++)
            {
                Item i1 = lijst[i];
                Item i2 = lijst[i+1];

                if(i1.Id == i2.Id){
                    lijst.RemoveAt(i);
                    lijst.CleanList();
                }
                else if(i == lijst.Count -2){
                    return lijst;
                }
            }
            return lijst;
        }

    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }

}