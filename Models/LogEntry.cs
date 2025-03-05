using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Models
{
    public class LogEntry
    {
        public string Date { get; init; }
        public string PID { get; init; }
        public string TID { get; init; }
        public string Level { get; init; }
        public string Tag { get; init; }
        public string Message { get; init; }

        public static LogEntry Parse(string log)
        {
            var match = System.Text.RegularExpressions.Regex.Match(log, @"^(?<Date>\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3})\s+(?<PID>\d+)\s+(?<TID>\d+)\s+(?<Level>[VDIWEF])\s+(?<Tag>[^:]+):\s(?<Message>.+)$");
            return match.Success
                ? new LogEntry
                {
                    Date = match.Groups["Date"].Value,
                    PID = match.Groups["PID"].Value,
                    TID = match.Groups["TID"].Value,
                    Level = match.Groups["Level"].Value,
                    Tag = match.Groups["Tag"].Value,
                    Message = match.Groups["Message"].Value
                }
                : null;
        }
    }
}
