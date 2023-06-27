using System;
namespace ShoppingApplicationAPINET.Models
{
    public class LoginRequestBody
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class RegisterRequestBody
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class CreateShoppingItemRequestBody
    {
        public string name { get; set; }
        public int quantity { get; set; }
    }

    public class PurchasedItemRequestBody
    {
        public int item_id { get; set; }
    }

    public class ItemUpdateRequestBody
    {
        public int item_id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public bool purchased { get; set; }
    }

    public class DeleteItemRequestBody
    {
        public int item_id { get; set; }
    }
}

