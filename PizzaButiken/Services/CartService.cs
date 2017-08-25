using Microsoft.AspNetCore.Http;
using PizzaButiken.Data;
using PizzaButiken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int GetTempCartId(ISession session)
        {
            if (!session.GetInt32("CartId").HasValue)
            {
                var tempCart = new Cart { Items = new List<CartItem>() };
                _context.Carts.Add(tempCart); _context.SaveChanges();
                session.SetInt32("CartId", tempCart.CartId);
            }
            var CartId = session.GetInt32("CartId").Value;
            return CartId;
        }

        public Cart GetCartForCurrentSession(ISession session)
        {
            var cartId = GetTempCartId(session);
            var cart = _context.Carts.Find(cartId);
            cart.Items = cart.Items ?? new List<CartItem>();

            return cart;
        }

        public int GetNumberOfItemsInCart(int cartId)
        {
            var cart = _context.Carts.Find(cartId);

            return cart.Items.Count();
        }

        public async Task AddItemForCurrentSession(ISession session, int dishId)
        {
            var cartItem = new CartItem();
            cartItem.CartId = GetTempCartId(session);
            cartItem.Dish = _context.Dishes.Find(dishId);
            cartItem.Quantity = 1;
            _context.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemForCurrentSession(ISession session, int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }
}
