using System;
using System.Text.Json.Serialization;

namespace ShoppingApplicationAPINET.Models
{
	public class ShoppingItem
	{
		public int Shopping_Item_ID { get; set; }

		public int Quantity { get; set; }

		public string Item_Name { get; set; } = string.Empty;

		public Boolean purchased { get; set; } = false;

		public int User_ID { get; set; }

		[JsonIgnore]
		public virtual User User { get; set; }
    }
}

