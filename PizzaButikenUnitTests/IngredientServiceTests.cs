using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PizzaButiken.Data;
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
            var _ingredients = _serviceProvider.GetService<IngredientService>();
            var ings = _ingredients.GetAllIngredients();
            Assert.Equal(ings.Count, 0);
        }
    }
}
