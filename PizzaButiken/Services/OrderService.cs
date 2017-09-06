using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaButiken.Data;
using PizzaButiken.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaButiken.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void CreateOrder(int cartId, ApplicationUser user)
        {
            var appUser = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;
            var carts = _context.Carts.Include(ci => ci.Items);
            var currentCart = carts.FirstOrDefault(id => id.CartId == cartId);
            var totalSum = currentCart.Items.Sum(x => x.Price * x.Quantity);
            var shippingFee = totalSum > 499 ? 0 : 49;

            var shippingAddress = new ShippingAddress
            {
                CustomerName = user.CustomerName,
                Street = user.Street,
                PhoneNumber = user.PhoneNumber,
                PostalCode = user.PostalCode,
                City = user.City,
                Email = user.Email
            };

            _context.Add(shippingAddress);
            _context.SaveChanges();

            var order = new Order
            {
                ApplicationUserId = appUser?.Id,
                CartId = cartId,
                TotalPrice = totalSum,
                ShippingFee = shippingFee,
                ShippingAddressId = shippingAddress.ShippingAddressId
            };
            _context.Add(order);
            _context.SaveChanges();
        }

        public Order GetOrder(int cartId)
        {
            return _context.Orders.FirstOrDefault(x => x.CartId == cartId);
        }

        public Payment Payment()
        {
            return new Payment();
        }
    }
}
