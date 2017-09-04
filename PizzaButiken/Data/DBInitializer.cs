using System;
using PizzaButiken.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace PizzaButiken.Data
{
    public class DBInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> roleManager)
        {

            var aUser = new ApplicationUser();
            aUser.UserName = "student@tset.com";
            aUser.Email = "student@tset.com";
            var r = usermanager.CreateAsync(aUser, "Pa$$w0rd").Result;

            var adminRole = new IdentityRole { Name = "Admin" };
            var roleresult = roleManager.CreateAsync(adminRole).Result;

            var adminUser = new ApplicationUser();
            adminUser.UserName = "admin@tset.com";
            adminUser.Email = "admin@tset.com";
            var adminUserResult = usermanager.CreateAsync(adminUser, "Pa$$w0rd").Result;
            if (adminUserResult.Succeeded)
            {
                usermanager.AddToRoleAsync(adminUser, adminRole.Name);
            }


            if (context.Dishes.ToList().Count == 0)
            {
                var cheese = new Ingredient { Name = "Cheese" };
                var tomatoe = new Ingredient { Name = "Tomatoe" };
                var ham = new Ingredient { Name = "Ham" };
                var mushroom = new Ingredient { Name = "Mushrooms" };
                var pineapple = new Ingredient { Name = "Pineapple" };
                var spaghetti = new Ingredient { Name = "Spagetti" };
                var lettuce = new Ingredient { Name = "Lettuce" };

                context.Ingredients.Add(cheese);
                context.Ingredients.Add(tomatoe);
                context.Ingredients.Add(ham);
                context.Ingredients.Add(mushroom);
                context.Ingredients.Add(pineapple);
                context.Ingredients.Add(lettuce);
                context.Ingredients.Add(spaghetti);

                var cappricciosa = new Dish { Name = "Cappricciosa", Price = 79, ImageUrl= "images/pizza.jpg" };
                var margherita = new Dish { Name = "Margherita", Price = 69, ImageUrl = "images/pizza.jpg" };
                var hawaii = new Dish { Name = "Hawaii", Price = 85, ImageUrl = "images/pizza.jpg" };
                var pastaCarbonara = new Dish { Name = "Pasta Carbonara", Price = 75, ImageUrl = "images/pasta.jpg" };
                var pastaPomodore = new Dish { Name = "Pasta Tomatsås", Price = 69, ImageUrl = "images/pasta.jpg" };
                var saladHam = new Dish { Name = "Ost och Skinksallad", Price = 75, ImageUrl = "images/sallad.jpg" };
                var saladPasta = new Dish { Name = "Pastasallad", Price = 79, ImageUrl = "images/sallad.jpg" };

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

                //context.AddRange(cappricciosaCheese, cappricciosaHam, cappricciosaTomatoe, cappriocciosaMushrooms);
                //context.AddRange(cappricciosa, margherita, hawaii);
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

}
