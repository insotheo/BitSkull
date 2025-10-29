using System;

namespace BitSkull
{
    public static class Log
    {
#if DEBUG
        public static bool IsEnabled = true;
#else
        public static bool IsEnabled = false;
#endif
        /// <summary>
        /// <para>%t - short time</para>
        /// <para>%T - full time</para>
        /// <para>%d - short date</para>
        /// <para>%D - full date</para>
        /// <para>%s - sender</para>
        /// <para>%m - message</para>
        /// <para>%l - log level(lower case)</para>
        /// <para>%L - log level(upper case)</para>
        /// <para>%e - exception message(%m if input is not Exception)</para>
        /// <para>%E - same as Exception.ToString()(same as for %e)</para>
        /// </summary>
        public static string Pattern = "[%T] %s: %e";

        private static void Write(string message, string exceptionMessage, string exceptionString, string sender, string pattern, string logLevel)
        {
            if (String.IsNullOrEmpty(pattern)) pattern = Pattern;
            DateTime now = DateTime.Now;
            message = pattern.Replace("%t", now.ToString("t"))
                             .Replace("%T", now.ToString("T"))
                             .Replace("%d", now.ToString("d"))
                             .Replace("%D", now.ToString("D"))
                             .Replace("%s", sender)
                             .Replace("%m", message)
                             .Replace("%l", logLevel)
                             .Replace("%L", logLevel.ToUpper())
                             .Replace("%e", exceptionMessage)
                             .Replace("%E", exceptionString);
            Console.WriteLine(message);
        }

        public static void Trace<T>(T msg, string sender = "BitSkull", string pattern = "")
        {
            if (!IsEnabled) return;
            string msgStr = msg.ToString();
            if (msg is Exception ex)
                Write(msgStr, ex.Message, ex.ToString(), sender, pattern, "trace");
            else
                Write(msgStr, msgStr, msgStr, sender, pattern, "trace");
        }

        public static void Info<T>(T msg, string sender = "BitSkull", string pattern = "")
        {
            if (!IsEnabled) return;
            string msgStr = msg.ToString();
            Console.ForegroundColor = ConsoleColor.Green;
            if (msg is Exception ex)
                Write(msgStr, ex.Message, ex.ToString(), sender, pattern, "info");
            else
                Write(msgStr, msgStr, msgStr, sender, pattern, "info");
            Console.ResetColor();
        }

        public static void Warn<T>(T msg, string sender = "BitSkull", string pattern = "")
        {
            if (!IsEnabled) return;
            string msgStr = msg.ToString();
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (msg is Exception ex)
                Write(msgStr, ex.Message, ex.ToString(), sender, pattern, "warn");
            else
                Write(msgStr, msgStr, msgStr, sender, pattern, "warn");
            Console.ResetColor();
        }

        public static void Error<T>(T msg, string sender = "BitSkull", string pattern = "")
        {
            if (!IsEnabled) return;
            string msgStr = msg.ToString();
            Console.ForegroundColor = ConsoleColor.Red;
            if (msg is Exception ex)
                Write(msgStr, ex.Message, ex.ToString(), sender, pattern, "error");
            else
                Write(msgStr, msgStr, msgStr, sender, pattern, "error");
            Console.ResetColor();
        }

    }
}
