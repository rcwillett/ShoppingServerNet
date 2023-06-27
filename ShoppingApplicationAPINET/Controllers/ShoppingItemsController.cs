using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingApplicationAPINET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShoppingApplicationAPINET.Types;
using System.Security.Claims;

namespace ShoppingApplicationAPINET.Controllers
{
    public class ShoppingItemsController : Controller
    {
        private readonly ShoppingContext _context;

        public ShoppingItemsController(ShoppingContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/list/getitems")]
        public async Task<IActionResult> GetShoppingListItems()
        {
            try
            {
                Claim? userClaim = HttpContext.User.Claims?.SingleOrDefault(p => p.Type == "User_ID");
                if (userClaim == null)
                {
                    throw new Exception("No Authentication Found");
                }
                int user_ID = int.Parse(userClaim.Value);
                List<ShoppingItem> shoppingItems = await _context.ShoppingItems.Where((i) => i.User_ID == user_ID).ToListAsync();
                List<ShoppingItemClient> clientShoppingItems = new List<ShoppingItemClient>();
                foreach (ShoppingItem item in shoppingItems)
                {
                    ShoppingItemClient clientItem = new ShoppingItemClient();
                    clientItem.id = item.Shopping_Item_ID;
                    clientItem.name = item.Item_Name;
                    clientItem.purchased = item.purchased;
                    clientItem.quantity = item.Quantity;
                    clientShoppingItems.Add(clientItem);
                }
                return Ok(clientShoppingItems);
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }

        }

        [HttpPost]
        [Route("/list/create")]
        public async Task<IActionResult> CreateShoppingListItem([FromBody] CreateShoppingItemRequestBody body)
        {
            try
            {
                Claim? userClaim = HttpContext.User.Claims?.SingleOrDefault(p => p.Type == "User_ID");
                if (userClaim == null)
                {
                    throw new Exception("No Authentication Found");
                }
                int user_ID = int.Parse(userClaim.Value);
                ShoppingItem shoppingItem = new ShoppingItem();
                shoppingItem.Item_Name = body.name;
                shoppingItem.Quantity = body.quantity;
                shoppingItem.User_ID = user_ID;
                await _context.AddAsync<ShoppingItem>(shoppingItem);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/list/purchased")]
        public async Task<IActionResult> MarkShoppingListItemPurchased([FromBody] PurchasedItemRequestBody body)
        {
            try
            {
                ShoppingItem? shoppingItem = await _context.ShoppingItems.FindAsync(body.item_id);
                if (shoppingItem != null)
                {
                    shoppingItem.purchased = true;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                throw new Exception("Failed to find Shopping Item");

            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/list/update")]
        public async Task<IActionResult> UpdateShoppingListItem ([FromBody] ItemUpdateRequestBody body)
        {
            try
            {
                ShoppingItem? shoppingItem = await _context.ShoppingItems.FindAsync(body.item_id);
                if (shoppingItem == null)
                {
                    throw new Exception("Couldn't find shopping item");
                }
                shoppingItem.Quantity = body.quantity;
                shoppingItem.Item_Name = body.name;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/list/remove")]
        public async Task<IActionResult> DeleteShoppingListItem([FromBody] DeleteItemRequestBody body)
        {
            try
            {
                ShoppingItem? shoppingItem = await _context.ShoppingItems.FindAsync(body.item_id);
                if (shoppingItem != null)
                {
                    _context.Remove(shoppingItem);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                throw new Exception("Failed to retrieve shopping list item");
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}

