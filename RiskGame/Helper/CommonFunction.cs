using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RiskGame.Helper.Const;

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

        private static readonly Random getrandomProbability = new Random();
        public static bool IsProbability(int? prob = null)
        {
            //ถ้า Prob น้อย โอกาสได้น้อย, ถ้า Prob มาก โอกาสได้มาก: 
            var defaultProb = prob.HasValue && prob > 0 ? prob / 10.0 : 0.50;
            lock (getrandomProbability)
            {
                var randomProb = getrandomProbability.NextDouble();
                if(randomProb <= defaultProb)
                {
                    return true;
                }
            }
            return false;
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

        public static string IsGameFinishedFormat(this bool value)
        {
            if(value)
            {
                return "Game Done";
            }
            return "Playing";
        }

        public static double RiskImpactFormat(int riskImpact)
        {
            return riskImpact * 0.10;
        }
    }
}