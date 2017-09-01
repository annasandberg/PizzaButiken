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
            var carts = _context.Carts.Include(i => i.Items).ThenInclude(ci => ci.CartItmeIngredients).ThenInclude(x => x.CartItem).ThenInclude(d => d.Dish);

            var cart = carts.FirstOrDefault(x => x.CartId == cartId);

            return cart;
        }

        public int GetNumberOfItemsInCart(int cartId)
        {
            var cart = _context.Carts.Find(cartId);

            cart.Items = cart.Items ?? new List<CartItem>();

            return cart.Items.Sum(x=>x.Quantity);
        }

        public void AddItemForCurrentSession(ISession session, int dishId)
        {
            var cart = GetCartForCurrentSession(session);
            var dish = GetDish(dishId);

            var cartItem = GetCartItemIfSame(cart, dish);

            if (!ItemExistsInCart(cartItem))
            {
                AddNewCartItemToCart(cart, dish);  
            }
            else
            {
                cartItem.Quantity++;
                _context.Update(cartItem);
            }

            var carts = _context.Carts.Include(x => x.Items).ThenInclude(d => d.Dish);

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

        public void CustomizeItemForCurrentsession(ISession session, CartItem cartItem, IFormCollection form)
        {
            var cartItemToCustomize = GetCartItem(cartItem);
            var cart = GetCartForCurrentSession(session);
            //var sameSpecialCartItem = GetSameSpecialCartItem(cart, cartItemToCustomize, form);

            try
            {
                if (cartItemToCustomize.Quantity == 1)
                {
                    SetCartItemName(cartItemToCustomize);

                    //if (SameSpecialPizzaExistsInCart(cart, cartItemToCustomize, form))
                    //{
                    //    sameSpecialCartItem.Quantity++;
                    //    _context.Update(sameSpecialCartItem);
                    //    DeleteItemForCurrentSession(session, cartItem.CartItemId);
                    //}
                    //else
                    //{
                    DeleteCartItemIngredients(cartItemToCustomize);
                    AddCheckedCartItemIngredients(form, cartItemToCustomize);
                    _context.Update(cartItemToCustomize);
                    //}

                    _context.SaveChanges();
                }
                else if (cartItemToCustomize.Quantity > 1)
                {
                    cartItemToCustomize.Quantity--;
                    _context.Update(cartItemToCustomize);
                    _context.SaveChanges();

                    var dish = GetDish(cartItemToCustomize.DishId);

                    //if (SameSpecialPizzaExistsInCart(cart, cartItemToCustomize, form))
                    //{
                    //    sameSpecialCartItem.Quantity++;
                    //    _context.Update(sameSpecialCartItem);
                    //}
                    //else
                    //{
                    AddNewCartItemToCart(cart, dish, form);
                    //}

                    _context.Update(cart);
                    _context.SaveChanges();
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(cartItem.CartItemId))
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    throw;
                }
            }

        }

        public void DeleteItemForCurrentSession(ISession session, int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            var cart = GetCartForCurrentSession(session);
            _context.CartItems.Remove(cartItem);
            _context.Update(cart);
            _context.SaveChanges();
        }

        private Dish GetDish(int dishId)
        {
            var dishes = _context.Dishes.Include(d => d.DishIngredients).ThenInclude(i => i.Ingredient);
            return dishes.FirstOrDefault(x => x.DishId == dishId);
        }

        private void AddNewCartItemToCart(Cart cart, Dish dish, IFormCollection form = null)
        {
            var cartItem = new CartItem
            {
                Dish = dish,
                Name = dish.Name,
                Quantity = 1,
                Price = dish.Price,
            };

            var cartItemIngredients = SetCartItemIngredients(cartItem, dish, form);
            cartItem.CartItmeIngredients = cartItemIngredients;

            cart.Items = cart.Items ?? new List<CartItem>();
            cart.Items.Add(cartItem);
        }

        private List<CartItemIngredient> SetCartItemIngredients(CartItem cartItem = null, Dish dish = null, IFormCollection form = null)
        {
            var cartItemIngredients = new List<CartItemIngredient>();

            if (form == null)
            {
                foreach (var ingredient in dish.DishIngredients)
                {
                    cartItemIngredients.Add(new CartItemIngredient
                    {
                        IngredientId = ingredient.IngredientId,
                        Enabled = true
                    });
                }
            }
            else
            {
                AddCheckedCartItemIngredients(form, cartItem);
                SetCartItemName(cartItem);
            }
            
            return cartItemIngredients;
        }

        private bool ItemExistsInCart(CartItem cartItem)
        {
            if (cartItem == null)
            {
                return false;
            }
            return true;
        }

        private CartItem GetCartItemIfSame(Cart cart, Dish dish)
        {
            return cart.Items.FirstOrDefault(ci => ci.DishId == dish.DishId && ci.Name == dish.Name);
        }

        private CartItem GetCartItem(CartItem cartItem)
        {
            var cartItems = _context.CartItems
                    .Include(x => x.CartItmeIngredients)
                    .ThenInclude(i => i.Ingredient)
                    .ThenInclude(di => di.DishIngredients)
                    .ThenInclude(d => d.Dish);
            return cartItems.FirstOrDefault(c => c.CartItemId == cartItem.CartItemId);
        }

        private void DeleteCartItemIngredients(CartItem cartItem)
        {
            var cartItemIngredients = cartItem.CartItmeIngredients;
            _context.CartItemIngredients.RemoveRange(cartItemIngredients);
            _context.SaveChanges();
        }

        private List<int> GetCheckedIngredients(IFormCollection form)
        {
            var key = form.Keys.FirstOrDefault(k => k.Contains("ingredient-"));
            var dashPos = key.IndexOf("-");
            var checkedIngredients = form.Keys.Where(k => k.Contains("ingredient-"));

            var checkedIds = new List<int>();

            foreach (var ingredient in checkedIngredients)
            {
                var checkboxId = int.Parse(ingredient.Substring(dashPos + 1));
                checkedIds.Add(checkboxId);
            }
            return checkedIds;
        }

        private void AddCheckedCartItemIngredients(IFormCollection form, CartItem cartItem)
        {
            var checkedIngredients = GetCheckedIngredients(form);
            cartItem.CartItmeIngredients = new List<CartItemIngredient>();

            var ingredients = _context.Ingredients.ToList();
            int extraIngredientsPriceSum = 0;

            foreach (var ingredientId in checkedIngredients)
            {
                cartItem.CartItmeIngredients.Add(new CartItemIngredient { IngredientId = ingredientId, Enabled = true, CartItemId = cartItem.CartItemId });

                if (!cartItem.Dish.DishIngredients.Any(i => i.IngredientId == ingredientId))
                {
                    extraIngredientsPriceSum += ingredients.FirstOrDefault(i => i.IngredientId == ingredientId).Price;
                }
            }

            cartItem.Price = cartItem.Dish.Price + extraIngredientsPriceSum;
        }

        private void SetCartItemName(CartItem cartItem)
        {
            if (!cartItem.Name.Contains("Specialized"))
            {
                cartItem.Name = "Specialized" + cartItem.Name;
            }
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }

        //private bool SameSpecialPizzaExistsInCart(Cart cart, CartItem cartItem, IFormCollection form)
        //{
        //    var checkedIngredients = GetCheckedIngredients(form);
        //    var sameCartItem = GetSameSpecialCartItem(cart, cartItem, form);
        //    if (cart.Items.Any(x => x.Name == cartItem.Name && 
        //        x.CartItmeIngredients.Count == checkedIngredients.Count() &&
        //        x.CartItmeIngredients.All(i => i.IngredientId == checkedIngredients.FirstOrDefault(id => id == i.IngredientId))))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //private CartItem GetSameSpecialCartItem(Cart cart, CartItem cartItem, IFormCollection form)
        //{
        //    var checkedIngredients = GetCheckedIngredients(form);

        //    var existingCartItem = cart.Items.FirstOrDefault(x => x.Name == cartItem.Name &&
        //        x.CartItmeIngredients.Count == checkedIngredients.Count() &&
        //        x.CartItmeIngredients.All(i => i.IngredientId == checkedIngredients.FirstOrDefault(id => id == i.IngredientId)));

        //    return existingCartItem;
        //}

    }
}
