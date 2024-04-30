using System;
using Verse;

namespace ColonyCopilot.Rimworld
{
    /// <summary>
    /// A simple layer ontop of Verse's Log class, to allow for added prefixes and control over logging.
    /// </summary>
    public static class CLog
    {
        private const string Prefix = "[ColonyCopilot] ";
        private static readonly bool IsLoggingEnabled = true;

        public static void Debug(string message)
        {
            if (CcpGameManager.DebugMode)
            {
                Message($"[DEBUG] {message}");
            }
        }
        /// <summary>
        /// Logs a message with an optional token count.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="tokenCount">The token count to log (optional).</param>
        public static void Message(string message, int? tokenCount = null)
        {
            if (!IsLoggingEnabled)
                return;

            try
            {
                Log.Message($"{Prefix}{message}");
            }
            catch (Exception ex)
            {
                // Handle logging exceptions
                Console.WriteLine($"Error logging message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warning(string message)
        {
            try
            {
                Log.Warning($"{Prefix}{message}");
            }
            catch (Exception ex)
            {
                // Handle logging exceptions
                Console.WriteLine($"Error logging warning: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            try
            {
                Log.Error($"{Prefix}{message}");
            }
            catch (Exception ex)
            {
                // Handle logging exceptions
                Console.WriteLine($"Error logging error: {ex.Message}");
            }
        }
    }
}