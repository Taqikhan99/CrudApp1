﻿using System.ComponentModel.DataAnnotations;

namespace CrudApp1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        
        public int UnitPrice { get; set; }
        public int UnitInStock { get; set; }
        public string ImageUrl { get; set; }

    }
}
