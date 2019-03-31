﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Helper
{
    public static class CommonFunction
    {
        public static bool CheckCurrentGame()
        {
            return Singleton.Game() != null ? true : false;
        }

        [Serializable]
        public struct ResponseModel
        {
            public bool IsSuccess { get; set; }
            public string Description { get; set; }
            public Object Result { get; set; }
        }

        public static ResponseModel GetResponse(bool isSuccess, string description = null, Object result = null)
        {
            return new ResponseModel
            {
                IsSuccess = isSuccess,
                Description = description,
                Result = result
            };
        }

        private static readonly Random getrandom = new Random();
        public static int RandomNumber(int min, int max)
        {
            lock (getrandom)
            {
                var listInt = new List<int>();
                for (int i = min; i <= max; i++)
                {
                    listInt.Add(i);
                }
                var value = listInt.Shuffle();
                var random = value.OrderBy(x => getrandom.Next()).FirstOrDefault();
                return random;
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (rng == null) throw new ArgumentNullException("rng");

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();
            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }


    }
}