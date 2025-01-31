﻿using System.ComponentModel.DataAnnotations;

namespace GameService.Models.Db
{
    public class MatchHistory
    {
        [Key]
        public int matchnumber { get; set; }
        public decimal bet { get; set; }
        public int firstplayerid { get; set; }
        public int secondplayerid { get; set; }
        public string firstplayerkey { get; set; } = "";
        public string secondplayerkey { get; set; } = "";
        public int? winner { get; set; } = null;
        public DateTime datematch { get; set; }
    }
}
