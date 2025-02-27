using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Milestone3WebApp.Models
{
    public class Survey
    {
        [Key]
        public int ID { get; set; } // Primary key

        // Required field
        public required string Title { get; set; }
    }
}