using System;
namespace ShoppingApplicationAPINET.Models
{
    public class ShoppingItemClient
    {
        public int id { get; set; }

        public int quantity { get; set; }

        public string name { get; set; } = string.Empty;

        public Boolean purchased { get; set; } = false;
    }
}

