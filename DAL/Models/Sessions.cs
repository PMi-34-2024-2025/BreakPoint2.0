using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Sessions
    {
        [Key] 
        public int SessionId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public string GameName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
