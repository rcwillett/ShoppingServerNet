using System;
namespace ShoppingApplicationAPINET.Types
{
	public interface ILoginRequestBody
	{
		string username { get; set; }
		string password { get; set; }
	}
}

