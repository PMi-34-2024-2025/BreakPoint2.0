//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BLL
//{
//    public class MainPage
//    {
//        private ApplicationDbContext dbContext = new ApplicationDbContext();

//        public SessionManager GetSessionResults(int userId)
//        {
//            var sessions = dbContext.Sessions
//                .Where(s => s.UserId == userId)
//                .GroupBy(s => s.GameName)
//                .Select(g => new SessionResult
//                {
//                    ApplicationName = g.Key,
//                    SessionDuration = g.Sum(s => (s.End - s.Start).Ticks)
//                }).ToList();

//        }
//        // Обчислення загального часу
//        var totalTime = sessions.Sum(s => s.SessionDuration);
//        var productiveTime = totalTime * 0.6; // Умовно 60% часу продуктивне
//        return new SessionManager
//            {
//                MostUsedApplications = sessions.OrderByDescending(s => s.SessionDuration).ToList(),
//                TotalTime = TimeSpan.FromTicks(totalTime),
//                ProductiveTime = TimeSpan.FromTicks(productiveTime),
//                GameDurations = sessions.ToDictionary(s => s.ApplicationName, s => s.SessionDuration)
//            };
//}
//}