using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaButiken.Controllers;
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
    public class HomeControllerTests
    {
        private readonly IServiceProvider _serviceProvider;
        public HomeControllerTests()
        {
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(b =>
                b.UseInMemoryDatabase("Pizzadatabas")
                .UseInternalServiceProvider(efServiceProvider));
            services.AddTransient<DishService>();
            services.AddTransient<CartService>();

            _serviceProvider = services.BuildServiceProvider();
        }
        [Fact]
        public void Index_ReturnsAViewResult_WithAllDishes()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var dishService = _serviceProvider.GetRequiredService<DishService>();
            var cartService = _serviceProvider.GetRequiredService<CartService>();
            var dishes = dishService.GetAllDishesAndTheirIngredients();

            var controller = new HomeController(cartService, dishService);

            // Act
            var result = controller.Index(0);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Dish>>(
                viewResult.ViewData.Model);
            Assert.Equal(7, model.Count());
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAllDishesInCategory()
        {
            // Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var dishService = _serviceProvider.GetRequiredService<DishService>();
            var cartService = _serviceProvider.GetRequiredService<CartService>();
            var dishes = dishService.GetAllDishesInCategory(1);

            var controller = new HomeController(cartService, dishService);

            // Act
            var result = controller.Index(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Dish>>(
                viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
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
    }
}
