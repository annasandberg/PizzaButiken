using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using PizzaButiken.Data;
using PizzaButiken.Models;
using PizzaButiken.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace PizzaButikenUnitTests
{
    public class CartServiceTests
    {
        private readonly IServiceProvider _serviceProvider;
        public CartServiceTests()
        {
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(b =>
                b.UseInMemoryDatabase("Pizzadatabas")
                .UseInternalServiceProvider(efServiceProvider));
            services.AddTransient<CartService>();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromSeconds(600);
                options.Cookie.HttpOnly = true;
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void GetsCorrectCartItemIngredients()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish = dishes.Result.Find(x => x.DishId == 1);
            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            //Act
            service.AddItemForCurrentSession(viewContext.HttpContext.Session, dish.DishId);
            var dishIngredients = dish.DishIngredients;
            var cartItemIngredients = cart.Items.Find(x => x.DishId == 1).CartItmeIngredients;

            //Assert
            //att dish ingredienser är samma som de som lagts till i cart
            Assert.Equal(dishIngredients.Count, cartItemIngredients.Count);
            foreach (var ingredient in dishIngredients)
            {
                Assert.True(cartItemIngredients.Exists(x => x.IngredientId == ingredient.IngredientId));
            }
        }

        [Fact]
        public void GetsCorrectCartItemIngredientsIfCustomized()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish = dishes.Result.Find(x => x.DishId == 1);
            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            var cartItemIngredients = new List<CartItemIngredient>();
            foreach (var ingredient in dish.DishIngredients)
            {
                cartItemIngredients.Add(new CartItemIngredient
                {
                    IngredientId = ingredient.IngredientId,
                    Enabled = true
                });
            }

            var cartItem = new CartItem
            {
                DishId = dish.DishId,
                Dish = dish,
                Name = dish.Name,
                Quantity = 1,
                CartItmeIngredients = cartItemIngredients
            };

            cart.Items.Add(cartItem);
            context.SaveChanges();

            var formIngredients = new string[] { "ingredient-1", "ingredient-2", "ingredient-3", "ingredient-4", "ingredient-5" };
            var form = CreateForm(formIngredients);

            var checkedIds = new List<int> { 1, 2, 3, 4, 5 };

            //Act
            service.CustomizeItemForCurrentsession(viewContext.HttpContext.Session, cartItem, form);
            var customizedCartItemIngredients = cart.Items.Find(ci=> ci.CartItemId == cartItem.CartItemId).CartItmeIngredients;

            //Assert
            Assert.Equal(formIngredients.Length, customizedCartItemIngredients.Count);
            foreach (var ingredient in customizedCartItemIngredients)
            {
                Assert.True(checkedIds.Exists(x => x == ingredient.IngredientId));
            }
        }

        [Fact]
        public void CalculatesCartItemPriceCorrectly_OriginalPizzas()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish = dishes.Result.Find(x => x.DishId == 1);
            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            //Act
            service.AddItemForCurrentSession(viewContext.HttpContext.Session, dish.DishId);
            var cartItem = cart.Items.Find(x => x.DishId == 1);

            //Assert
            Assert.Equal(dish.Price, cartItem.Price);
        }

        [Fact]
        public void CalculatesCartItemPriceCorrectly_ExtraIngredients()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish = dishes.Result.Find(x => x.DishId == 1);
            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            var cartItemIngredients = new List<CartItemIngredient>();
            foreach (var ingredient in dish.DishIngredients)
            {
                cartItemIngredients.Add(new CartItemIngredient
                {
                    IngredientId = ingredient.IngredientId,
                    Enabled = true
                });
            }

            var cartItem = new CartItem
            {
                DishId = dish.DishId,
                Dish = dish,
                Name = dish.Name,
                Quantity = 1,
                CartItmeIngredients = cartItemIngredients
            };

            cart.Items.Add(cartItem);
            context.SaveChanges();

            var formIngredients = new string[] { "ingredient-1", "ingredient-2", "ingredient-3", "ingredient-4", "ingredient-5" };
            var form = CreateForm(formIngredients);

            var checkedIds = new List<int> { 1, 2, 3, 4, 5 };

            //Act
            service.CustomizeItemForCurrentsession(viewContext.HttpContext.Session, cartItem, form);
            var customizedCartItem = cart.Items.Find(ci => ci.CartItemId == cartItem.CartItemId);

            //Assert
            Assert.Equal(dish.Price +5, customizedCartItem.Price);
        }

        [Fact]
        public void CalculatesCartItemPriceCorrectly_MinusSomeIngredients()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish = dishes.Result.Find(x => x.DishId == 1);
            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            var cartItemIngredients = new List<CartItemIngredient>();
            foreach (var ingredient in dish.DishIngredients)
            {
                cartItemIngredients.Add(new CartItemIngredient
                {
                    IngredientId = ingredient.IngredientId,
                    Enabled = true
                });
            }

            var cartItem = new CartItem
            {
                DishId = dish.DishId,
                Dish = dish,
                Name = dish.Name,
                Quantity = 1,
                CartItmeIngredients = cartItemIngredients
            };

            cart.Items.Add(cartItem);
            context.SaveChanges();

            var formIngredients = new string[] { "ingredient-1", "ingredient-2"};
            var form = CreateForm(formIngredients);

            var checkedIds = new List<int> { 1, 2 };

            //Act
            service.CustomizeItemForCurrentsession(viewContext.HttpContext.Session, cartItem, form);
            var customizedCartItem = cart.Items.Find(ci => ci.CartItemId == cartItem.CartItemId);

            //Assert
            Assert.Equal(dish.Price, customizedCartItem.Price);

        }

        [Fact]
        public void CalculatesCartItemPriceCorrectly_MinusSomeIngredients_PlusSomeIngredients()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish = dishes.Result.Find(x => x.DishId == 1);
            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            var cartItemIngredients = new List<CartItemIngredient>();
            foreach (var ingredient in dish.DishIngredients)
            {
                cartItemIngredients.Add(new CartItemIngredient
                {
                    IngredientId = ingredient.IngredientId,
                    Enabled = true
                });
            }

            var cartItem = new CartItem
            {
                DishId = dish.DishId,
                Dish = dish,
                Name = dish.Name,
                Quantity = 1,
                CartItmeIngredients = cartItemIngredients
            };

            cart.Items.Add(cartItem);
            context.SaveChanges();

            var formIngredients = new string[] { "ingredient-1", "ingredient-2", "ingredient-5" };
            var form = CreateForm(formIngredients);

            var checkedIds = new List<int> { 1, 2, 5 };

            //Act
            service.CustomizeItemForCurrentsession(viewContext.HttpContext.Session, cartItem, form);
            var customizedCartItem = cart.Items.Find(ci => ci.CartItemId == cartItem.CartItemId);

            //Assert
            Assert.Equal(dish.Price + 5, customizedCartItem.Price);
        }

        [Fact]
        public void CalculatesShoppingCartTotalCorrectly_OriginalPizzas()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var service = _serviceProvider.GetService<CartService>();

            var viewContext = new ViewContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            viewContext.HttpContext.Session = new TestSession();
            viewContext.HttpContext.Session.SetString("CartId", "1");

            var dishes = context.Dishes.Include(di => di.DishIngredients).ThenInclude(i => i.Ingredient).ToListAsync();
            var dish1 = dishes.Result.Find(x => x.DishId == 1);
            var dish2 = dishes.Result.Find(x => x.DishId == 2);
            var dish3 = dishes.Result.Find(x => x.DishId == 3);

            //Act
            service.AddItemForCurrentSession(viewContext.HttpContext.Session, dish1.DishId);
            service.AddItemForCurrentSession(viewContext.HttpContext.Session, dish2.DishId);
            service.AddItemForCurrentSession(viewContext.HttpContext.Session, dish3.DishId);

            var cart = service.GetCartForCurrentSession(viewContext.HttpContext.Session);

            //Assert
            Assert.Equal(cart.Items.Sum(x => x.Price * x.Quantity), (dish1.Price + dish2.Price + dish3.Price));
        }

        private void Initialize(ApplicationDbContext context)
        {
            var cheese = new Ingredient { Name = "Cheese", Price = 5 };
            var tomatoe = new Ingredient { Name = "Tomatoe", Price = 5 };
            var ham = new Ingredient { Name = "Ham", Price = 5 };
            var mushroom = new Ingredient { Name = "Mushrooms", Price = 5 };
            var pineapple = new Ingredient { Name = "Pineapple", Price = 5 };
            var spaghetti = new Ingredient { Name = "Spagetti", Price = 5 };
            var lettuce = new Ingredient { Name = "Lettuce", Price = 5 };

            context.Ingredients.Add(cheese);
            context.Ingredients.Add(tomatoe);
            context.Ingredients.Add(ham);
            context.Ingredients.Add(mushroom);
            context.Ingredients.Add(pineapple);
            context.Ingredients.Add(lettuce);
            context.Ingredients.Add(spaghetti);

            var cappricciosa = new Dish { Name = "Cappricciosa", Price = 79 };
            var margherita = new Dish { Name = "Margherita", Price = 69 };
            var hawaii = new Dish { Name = "Hawaii", Price = 85 };
            var pastaCarbonara = new Dish { Name = "Pasta Carbonara", Price = 75 };
            var pastaPomodore = new Dish { Name = "Pasta Tomatsås", Price = 69 };
            var saladHam = new Dish { Name = "Ost och Skinksallad", Price = 75 };
            var saladPasta = new Dish { Name = "Pastasallad", Price = 79 };

            var cappricciosaCheese = new DishIngredient { Dish = cappricciosa, Ingredient = cheese };
            var cappricciosaHam = new DishIngredient { Dish = cappricciosa, Ingredient = ham };
            var cappricciosaTomatoe = new DishIngredient { Dish = cappricciosa, Ingredient = tomatoe };
            var cappriocciosaMushrooms = new DishIngredient { Dish = cappricciosa, Ingredient = mushroom };

            cappricciosa.DishIngredients = new List<DishIngredient>();
            cappricciosa.DishIngredients.Add(cappricciosaCheese);
            cappricciosa.DishIngredients.Add(cappricciosaTomatoe);
            cappricciosa.DishIngredients.Add(cappricciosaHam);
            cappricciosa.DishIngredients.Add(cappriocciosaMushrooms);

            var pizza = new DishCategory { Name = "Pizza" };
            var salad = new DishCategory { Name = "Sallad" };
            var pasta = new DishCategory { Name = "Pasta" };

            cappricciosa.DishCategory = pizza;

            var margheritaCheese = new DishIngredient { Dish = margherita, Ingredient = cheese };
            var margheritaHam = new DishIngredient { Dish = margherita, Ingredient = ham };
            var margheritaTomatoe = new DishIngredient { Dish = margherita, Ingredient = tomatoe };

            margherita.DishIngredients = new List<DishIngredient>();
            margherita.DishIngredients.Add(margheritaCheese);
            margherita.DishIngredients.Add(margheritaHam);
            margherita.DishIngredients.Add(margheritaTomatoe);

            margherita.DishCategory = pizza;

            hawaii.DishCategory = pizza;
            saladHam.DishCategory = salad;
            saladPasta.DishCategory = salad;
            pastaCarbonara.DishCategory = pasta;
            pastaPomodore.DishCategory = pasta;

            context.Dishes.Add(cappricciosa);
            context.Dishes.Add(margherita);
            context.Dishes.Add(hawaii);
            context.Dishes.Add(saladHam);
            context.Dishes.Add(saladPasta);
            context.Dishes.Add(pastaCarbonara);
            context.Dishes.Add(pastaPomodore);
            context.SaveChanges();
        }

        private IFormCollection CreateForm(string[] values)
        {
            var fields = new Dictionary<string, StringValues>();
            foreach (var item in values)
            {
                fields.Add(item, item);
            }

            var form = new FormCollection(fields); 
            
            return form;
        }

    }


        //Det kan innebära viss refaktorering av kod, men det är troligen inte dåligt.
        //a) Pizza original 
        //b) Pizza original med extra ingredienser 
        //c) Pizza original minus vissa ingredienser 
        //d) Pizza original med extra ingredienser minus vissa ingredienser.
    
}
