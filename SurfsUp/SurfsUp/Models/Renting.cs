﻿using SurfsUp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace SurfsUp.Models
{
    public class Renting
    {
        [Key]
        public int Id { get; set; }
        public string SurfsUpUserId { get; set; }
        public SurfsUpUser? SurfsUpUser { get; set; }
        public int BoardId { get; set; }
        public Board? Board { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
