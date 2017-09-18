using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=aspnet-PizzaButiken-A1662EDC-C9DF-4C27-B74D-1E1BF8647B96;Trusted_Connection=True;MultipleActiveResultSets=true");
            var db = new ApplicationDbContext(builder.Options);
            return db;
        }
    }
}
