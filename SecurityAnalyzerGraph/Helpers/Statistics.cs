
namespace SecurityAnalyzerGraph.Helpers
{
    /// <summary>
    /// Calculates statistics operations.
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Gets the min in the values given.
        /// </summary>
        /// <param name="values">
        /// <see cref="IEnumerable{T}"/> of <see cref="double"/>
        /// </param>
        /// <returns>
        /// The min value in the set.
        /// </returns>
        public static double GetMin(IEnumerable<double> values)
        {
            return values.OrderBy(v => v).FirstOrDefault();
        }

        /// <summary>
        /// Gets the max in the values given.
        /// </summary>
        /// <param name="values">
        /// <see cref="IEnumerable{T}"/> of <see cref="double"/>
        /// </param>
        /// <returns>
        /// The max value in the set.
        /// </returns>
        public static double GetMax(IEnumerable<double> values)
        {
            return values.OrderByDescending(v => v).FirstOrDefault();
        }

        /// <summary>
        /// Gets the median for a given enumerable.
        /// </summary>
        /// <param name="values">
        /// <see cref="IEnumerable{T}"/> of <see cref="double"/> to
        /// find the median for.
        /// </param>
        /// <returns>
        /// <see cref="double"/> represnting the median in the given 
        /// enumerable.
        /// </returns> 
        public static double GetMedian(IEnumerable<double> values)
        {
            if (!values.Any())
            {
                return 0;
            }
            if (values.Count() == 1)
            {
                return values.First();
            }

            IEnumerable<double> sortedValues = values.OrderBy(v => v);
            int count = sortedValues.Count();

            double median = 0;
            if (count % 2 == 0)
            {
                double middle1 = sortedValues.ElementAt((count - 1) / 2);
                double middle2 = sortedValues.ElementAt(count / 2);
                median = (middle1 + middle2) / 2.0;
            }
            else
            {
                median = sortedValues.ElementAt(count / 2);
            }

            return median;
        }

        /// <summary>
        /// Calculates sample standard deviation.
        /// </summary>
        /// <param name="values">
        /// <see cref="IEnumerable{T}"/> of <see cref="double"/>
        /// </param>
        /// <returns>
        /// The sample standard deviation.
        /// </returns>
        public static double GetStandardDeviation(IEnumerable<double> values)
        {
            double mean = values.Average();
            double variance = values.Sum(v => Math.Pow(v - mean, 2)) / values.Count();
            
            return Math.Sqrt(variance);
        } 
    }
}
