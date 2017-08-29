using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
                var tempCart = new Cart ();
                _context.Carts.Add(tempCart);
                _context.SaveChanges();
                session.SetInt32("CartId", tempCart.CartId);
            }
            var CartId = session.GetInt32("CartId").Value;
            return CartId;
        }

        public Cart GetCartForCurrentSession(ISession session)
        {
            var cartId = GetTempCartId(session);
            var carts = _context.Carts.Include(i => i.Items).ThenInclude(d => d.Dish);

            var cart = carts.FirstOrDefault(x => x.CartId == cartId);

            return cart;
        }

        public int GetNumberOfItemsInCart(int cartId)
        {
            var cart = _context.Carts.Find(cartId);

            cart.Items = cart.Items ?? new List<CartItem>();

            return cart.Items.Count();
        }

        public void AddItemForCurrentSession(ISession session, int dishId)
        {
            var carts = _context.Carts.Include(x => x.Items).ThenInclude(d => d.Dish);

            var cart = GetCartForCurrentSession(session);
            var dishes = _context.Dishes.Include(d => d.DishIngredients).ThenInclude(i => i.Ingredient);
            var dish = dishes.FirstOrDefault(x => x.DishId == dishId);
            //var dishIngredients = dish.DishIngredients;
            var cartItemIngredients = new List<CartItemIngredient>();
            foreach (var ingredient in dish.DishIngredients)
            {
                cartItemIngredients.Add(new CartItemIngredient
                {
                    IngredientId = ingredient.IngredientId,
                    Enabled = true
                });
            }

            cart.Items = cart.Items ?? new List<CartItem>();
            cart.Items.Add(new CartItem()
            {
                Dish = dish,
                Quantity = 1,
                CartItmeIngredients = cartItemIngredients
            });

            if (carts.Any(x => x.CartId == cart.CartId))
            {
                _context.Update(cart);
            }
            else
            {
                _context.Add(cart);
            }

            _context.SaveChanges();
        }

        public void CustomizeItemForCurrentsession(ISession session, int cartItemId)
        {

        }

        public void DeleteItemForCurrentSession(ISession session, int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            var cart = GetCartForCurrentSession(session);
            _context.CartItems.Remove(cartItem);
            _context.Update(cart);
            _context.SaveChanges();
        }
    }
}
