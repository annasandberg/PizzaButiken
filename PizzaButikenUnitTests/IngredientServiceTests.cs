using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaButiken.Data;
using PizzaButiken.Models;
using PizzaButiken.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PizzaButikenUnitTests
{
    public class IngredientServiceTests
    {
        private readonly IServiceProvider _serviceProvider;
        public IngredientServiceTests()
        {
            var efServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(b =>
                b.UseInMemoryDatabase("Pizzadatabas")
                .UseInternalServiceProvider(efServiceProvider));
            services.AddTransient<IngredientService>();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void All_Are_Sorted()
        {
            //Arrange
            var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            Initialize(context);
            var _ingredients = _serviceProvider.GetService<IngredientService>();

            //Act
            var ings = _ingredients.GetAllIngredients();

            //Assert
            Assert.Equal(ings[0].Name, "Cheese");
            Assert.Equal(ings[1].Name, "Ham");
            Assert.Equal(ings[2].Name, "Mushrooms");
            Assert.Equal(ings[3].Name, "Pineapple");
            Assert.Equal(ings[4].Name, "Tomatoe");
        }

        private void Initialize(ApplicationDbContext context)
        {
            var cheese = new Ingredient { Name = "Cheese" };
            var tomatoe = new Ingredient { Name = "Tomatoe" };
            var ham = new Ingredient { Name = "Ham" };
            var mushroom = new Ingredient { Name = "Mushrooms" };
            var pineapple = new Ingredient { Name = "Pineapple" };

            context.Ingredients.Add(cheese);
            context.Ingredients.Add(tomatoe);
            context.Ingredients.Add(ham);
            context.Ingredients.Add(mushroom);
            context.Ingredients.Add(pineapple);
            
            context.SaveChanges();
        }
    }
}
