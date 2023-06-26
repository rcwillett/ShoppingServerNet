using System;
namespace ShoppingApplicationAPINET.Models
{
    public class User
    {
        public int User_ID { get; set; }

        public string User_Name { get; set; } = string.Empty;

        public string Password_Hash { get; set; } = string.Empty;

        public virtual List<ShoppingItem> ShoppingItems { get; set; } = new List<ShoppingItem>();
    }
}

