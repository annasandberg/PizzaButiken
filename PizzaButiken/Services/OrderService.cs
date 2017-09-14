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
        private readonly IEmailSender _emailSender;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            IHttpContextAccessor httpContextAccessor, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
        }

        public void CreateOrder(int cartId, ShippingAddress user)
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

        public void SetOrderToPaid(int cartId)
        {
            var order = GetOrder(cartId);

            order.Paid = true;
            _context.Update(order);
            _context.SaveChanges();
        }

        public Order GetOrder(int cartId)
        {
            return _context.Orders.FirstOrDefault(x => x.CartId == cartId);
        }

        public void SendConfirmationEmail(int cartId, string userEmail)
        {
            var order = GetOrder(cartId);
            string message = String.Format("Orderdatum: {0} Summa: {1} SEK Fraktavgift: {2} SEK",
            order.OrderDate.ToString(), order.TotalPrice.ToString(), order.ShippingFee.ToString());
            _emailSender.SendEmailAsync(userEmail, "Tack för din beställning", message);
        }

        public List<Order> GetOrdersToBake()
        {
            return _context.Orders
                .Include(x => x.Cart)
                .ThenInclude(i => i.Items)
                .Where(o => o.Paid == true && o.Baked == false)
                .OrderBy(d => d.OrderDate).ToList();
        }

        public void SetOrderToBaked(Order order)
        {
            var orderBaked = GetOrder(order.CartId);
            orderBaked.Baked = true;
            _context.Update(orderBaked);
            _context.SaveChanges();
        }

        public List<Order> GetOrdersToShip()
        {
            return _context.Orders
                .Include(x => x.Cart)
                .ThenInclude(i => i.Items)
                .Where(o => o.Paid == true && o.Baked == true && o.Shipped == false)
                .OrderBy(d => d.OrderDate).ToList();
        }

        public void SetOrderToShipped(Order order)
        {
            var orderToShip = GetOrder(order.CartId);
            orderToShip.Shipped = true;
            _context.Update(orderToShip);
            _context.SaveChanges();
        }
    }
}
