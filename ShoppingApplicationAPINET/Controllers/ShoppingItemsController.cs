using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShoppingApplicationAPINET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ShoppingApplicationAPINET.Controllers
{
    public class ShoppingItemsController : Controller
    {
        private readonly ShoppingContext _context;

        public ShoppingItemsController(ShoppingContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("/list/getitems")]
        public async Task<IActionResult> GetShoppingListItems(
            [FromBody] int user_ID
        )
        {
            try
            {
                List<ShoppingItem> shoppingItems = await _context.ShoppingItems.Where((i) => i.User_ID == user_ID).ToListAsync();
                return Ok(shoppingItems);
            } catch (Exception ex)
            {
                return Problem(ex.ToString());
            }

        }

        [HttpPost]
        [Route("/list/create")]
        public async Task<IActionResult> CreateShoppingListItem(ShoppingItem shoppingItem)
        {
            try
            {
                await _context.AddAsync<ShoppingItem>(shoppingItem);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/list/purchased")]
        public async Task<IActionResult> MarkShoppingListItemPurchased(int shoppingItemID)
        {
            try
            {
                ShoppingItem? shoppingItem = await _context.ShoppingItems.FindAsync(shoppingItemID);
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
        public async Task<IActionResult> UpdateShoppingListItem (ShoppingItem shoppingItem)
        {
            try
            {
                _context.ShoppingItems.Update(shoppingItem);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost]
        [Route("/list/delete")]
        public async Task<IActionResult> DeleteShoppingListItem(int shoppingItemID)
        {
            try
            {
                ShoppingItem? shoppingItem = await _context.ShoppingItems.FindAsync(shoppingItemID);
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

