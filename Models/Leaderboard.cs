using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    
    public class Leaderboard
    {
        [Key]
        public int BoardID { get; set; } // Primary key
        public required string Season { get; set; } // Required field
    }
}