using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.BLL
{
    public class OrderItemAccess
    {
        public static void AddOrderItem(OrderItem orderItem)
        {
            try
            {
                using (var context = new RestaurantDBContext())
                {
                    context.OrderItems.Add(orderItem);
                    context.SaveChanges();
                }
            }
            catch
            {
                return;
            }
        }

        public static List<OrderItem> GetOrderItems()
        {
            var context = new RestaurantDBContext();
            return context.OrderItems.ToList();
        }

        public static OrderItem GetOrderItem(int id)
        {
            var context = new RestaurantDBContext();
            return context.OrderItems.Find(id);
        }

        public static List<OrderItem> GetOrderItemsByOrder(int orderId)
        {
            var context = new RestaurantDBContext();
            return context.OrderItems.Where(o => o.OrderId == orderId).ToList();
        }
    }
}
