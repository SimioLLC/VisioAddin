using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// Quick and dirty in-memory logging system.
/// Use cases: Logs for developers, Logs for Users for complex systems
/// where detailed guidance on failures would be useful.
/// </summary>
namespace LoggertonHelpers
{

    [Flags]
    public enum EnumLogFlags
    {
        None = 0,
        Information = 1,
        Event = 2,
        Warning = 4,
        Error = 8,
        All = 0xffff
    }

    /// <summary>
    /// A single log entry.
    /// </summary>
    public class LogEntry
    {
        public EnumLogFlags Flags { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Has this message been excluded by regex excludes?
        /// </summary>
        public bool IsExcluded { get; set; }

        public bool IsDeleted { get; set; }

        public LogEntry(EnumLogFlags flags, String msg)
        {
            Flags = flags;
            TimeStamp = DateTime.Now;
            Message = msg;
        }

        public override string ToString()
        {
            string ss = $"{Flags}:{TimeStamp.ToString("HH:mm:ss.ff")} {Message}";
            return ss;
        }
    }

    /// <summary>
    /// The loggerton engine.
    /// </summary>
    public sealed class Loggerton
    {
        private static readonly Loggerton instance = new Loggerton(1000);
        public static Loggerton Instance { get { return instance; } }

        static Loggerton()
        {
        }
        private Loggerton(int logSize)
        {
            for (int ii = 0; ii < logSize; ii++)
                TrashBag.Add(new LogEntry(EnumLogFlags.None, ""));
        }

        /// <summary>
        /// The maximum number of minutes before logs are thrown away.
        /// </summary>
        public int MaximumMinutes = 60;

        /// <summary>
        /// How many logs before we start re-using.
        /// </summary
        public int MaxLogLength = 10000;

        public bool IsEnabled = true;

        private ConcurrentQueue<LogEntry> Logs = new ConcurrentQueue<LogEntry>();
        private ConcurrentBag<LogEntry> TrashBag = new ConcurrentBag<LogEntry>();
        
        /// <summary>
        /// A list of regex filters for excluding logs.
        /// </summary>
        private List<string> excludesList = new List<string>();

        /// <summary>
        /// Set the list of regex exclude strings that each message
        /// will be checked against.
        /// </summary>
        /// <param name="commalist"></param>
        public void SetExcludes( string commalist )
        {
            excludesList.Clear();
            excludesList = commalist.Split(',').ToList();

            // Reevaluate all the logs
            foreach (LogEntry le in Logs)
            {
                le.IsExcluded = false;
                foreach (string expr in excludesList)
                {
                    if (Regex.IsMatch(le.Message, expr, RegexOptions.IgnoreCase))
                    {
                        le.IsExcluded = true;
                        goto GetNextLogEntry;
                    }
                }
                GetNextLogEntry:;
            }
        }

        /// <summary>
        /// Get all logs that should be displayed.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="excludes"></param>
        /// <returns></returns>
        public string GetLogs(EnumLogFlags flags)
        {
            if (!Logs.Any())
                return string.Empty;

            List<LogEntry> filteredLogs = Logs
                .Where(rr => (rr.Flags & flags) != 0)
                .Where(rr => !rr.IsExcluded )
                .OrderByDescending(rr => rr.TimeStamp)
                .ToList();

            
            StringBuilder sb = new StringBuilder();
            foreach (LogEntry le in filteredLogs)
            {
                sb.AppendLine($"{le}");
            }

            return sb.ToString();
        }

        public void ClearLogs()
        {
            CleanLogs(Logs.Count);
        }

        public string ShowLogs()
        {
            return Logs.ToString();
        }

        public void WriteLogs(string path)
        {
            File.WriteAllText(path, Logs.ToString());
        }


        public LogEntry RecycleLogEntry(EnumLogFlags flags, string msg )
        {
            LogEntry recycled = null;
            if (!TrashBag.TryTake(out recycled))
                recycled = new LogEntry(flags, msg);

            recycled.Message = msg;
            recycled.Flags = flags;
            recycled.TimeStamp = DateTime.Now;
            
            return recycled;
        }

        /// <summary>
        /// Take logs from the fifo, and place them in the trashbag
        /// </summary>
        /// <param name="amountToClean"></param>
        public void CleanLogs(int amountToClean)
        {
            if ( Logs.Count >= amountToClean )
            {
                int amount = Logs.Count - MaxLogLength;
                for (int ii = amount; ii > 0; ii--)
                {
                    LogEntry trash = null;
                    if (Logs.TryDequeue(out trash))
                        TrashBag.Add(trash);
                }
            }
        }

        /// <summary>
        /// Create a new log entry
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="message"></param>
        public void LogIt(EnumLogFlags logType, string message)
        {
            if (!IsEnabled)
                return;

            bool isExcluded = false;
            foreach (string expr in excludesList)
            {
                if (Regex.IsMatch(message, expr, RegexOptions.IgnoreCase))
                {
                    isExcluded = true;
                    break;
                }
            }

            LogEntry entry = RecycleLogEntry(logType, message);
            entry.IsExcluded = isExcluded;
            Logs.Enqueue(entry);
        }

    }
}

