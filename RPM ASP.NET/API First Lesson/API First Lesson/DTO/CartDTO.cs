using System;
using System.Collections.Generic;

namespace API_First_Lesson.Models
{
    public class CartDTO
    {
        public int CartId { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public DateTime CreatedDate { get; set; }

        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    }
}
