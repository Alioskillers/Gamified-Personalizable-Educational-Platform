using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Reward
    {
        [Key]
        public int RewardID { get; set; } // Primary key
        public required int Value { get; set; } // Required field
        public required string Description { get; set; } // Required field
        public required string Type { get; set; } // Required field
    }
}