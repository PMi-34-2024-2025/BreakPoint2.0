using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BLL
{
    public class SessionManager
    {
        public List<SessionResult> MostUsedApplications { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan ProductiveTime { get; set; }
        public Dictionary<string, long> GameDurations { get; set; } // Ігри та їхня тривалість
    }
}
