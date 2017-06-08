using System;
using System.Collections.Generic;
using System.Linq;
using AutomationHelper.Waiters;

namespace AutomationHelper.Extensions
{
    public static class LinqExtensions
    {
        #region LINQ EXTENSIONS
        public static T GetRandomElement<T>(this IEnumerable<T> values)
        {
            var elementsCount = values.Count();
            var random = new Random();
            var randomIndex = random.Next(elementsCount);
            return values.ElementAt(randomIndex);
        }
        public static T GetRandomElement<T>(this IEnumerable<T> values, string err)
        {
            try
            {
                var elementsCount = values.Count();
                var random = new Random();
                var randomIndex = random.Next(elementsCount);
                return values.ElementAt(randomIndex);
            }
            catch (Exception ex)
            {
                throw new Exception(err,ex);
            }
        }

        public static T FirstUntilNumberOfException<T>(this IEnumerable<T> values, string err)
        {
            return FirstUntilNumberOfException(values, e => true, err);
        }

        public static T FirstUntilNumberOfException<T>(this IEnumerable<T> values, Func<T, bool> predicate, string err)
        {
            try
            {
                return
                    Wait.UntilNumberOfExceptions(()=>
                    values.First(predicate));
            }
            catch (Exception e)
            {
                throw new Exception(err+ ". "+e);
            }
        }

        public static T First<T>(this IEnumerable<T> values, string err)
        {
            return First(values, e => true, err);
        }

        public static T First<T>(this IEnumerable<T> values, Func<T, bool> predicate, string err)
        {
            try
            {
                return values.First(predicate);
            }
            catch (Exception e)
            {
                throw new Exception(err + ". " + e);
            }
        }


        public static T Second<T>(this IEnumerable<T> values, string err)
        {
            return Second(values, e => true, err);
        }

        public static T Second<T>(this IEnumerable<T> values, Func<T, bool> predicate, string err)
        {
            try
            {
                return values.Where(predicate).ElementAt(1);
            }
            catch (Exception e)
            {
                throw new Exception(err + ". " + e);
            }

        }
        public static T Last<T>(this IEnumerable<T> values, string err)
        {
            return Last(values, e => true, err);
        }

        public static T Last<T>(this IEnumerable<T> values, Func<T, bool> predicate, string err)
        {
            try
            {
                return values.Last(predicate);
            }
            catch (Exception e)
            {
                throw new Exception(err+ ". "+e);
            }

        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy<T, int>((item) => rnd.Next());
        }

        public static IEnumerable<T> FindMin<T>(this IEnumerable<T> values, Func<T, int> selector)
        {
            var min = values.Min(selector);
            return values.Where(v => selector(v) == min);
        }
        public static IEnumerable<T> FindMin<T>(this IEnumerable<T> values, Func<T, double> selector)
        {
            var min = values.Min(selector);
            return values.Where(v => selector(v) == min);
        }

        public static IEnumerable<T> FindMax<T>(this IEnumerable<T> values, Func<T, int> selector)
        {
            var max = values.Max(selector);
            return values.Where(v => selector(v) == max);
        }
        public static IEnumerable<T> FindMax<T>(this IEnumerable<T> values, Func<T, double> selector)
        {
            var max = values.Max(selector);
            return values.Where(v => selector(v) == max);
        }

        /// <summary>
        /// {a,b,c} and {b,a,c} -is equal (do not look at sort)
        /// </summary>
        public static bool ListEquals<T>(this List<T> target, List<T> source)
        {
            var temp = new List<T>(target);
            if (temp.Count != source.Count)
                return false;
            foreach (var s in source)
            {
                if (temp.Any(t => t.Equals(s)))
                    temp.Remove(s);
                else return false;
            }
            //expected that temp should be empty
            return (!temp.Any());
        }

        public static string JoinByComma(this IEnumerable<string> values)
        {
            return String.Join(", ", values);
        }
        #endregion
    }
}
