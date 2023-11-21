using System.Runtime.Serialization;

namespace SecurityAnalyzer.Exceptions
{
    /// <summary>
    /// Thrown if the rate limit is reached on an API.
    /// </summary>
    [Serializable]
    public class RateLimitException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="RateLimitException"/>.
        /// </summary>
        public RateLimitException() { }

        /// <summary>
        /// Creates a new <see cref="RateLimitException"/>.
        /// </summary>
        /// <param name="message">
        /// Message to put in the exception.
        /// </param>
        public RateLimitException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="RateLimitException"/>.
        /// </summary>
        /// <param name="message">
        /// Message to put in the exception.
        /// </param>
        /// <param name="innerException">
        /// Inner exception details.
        /// </param>
        public RateLimitException(string message, Exception innerException) : base(message, innerException) { }

        protected RateLimitException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
