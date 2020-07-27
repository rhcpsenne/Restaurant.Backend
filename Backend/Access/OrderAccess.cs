using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Access
{
    public class OrderAccess
    {
        public static void AddOrder(OrderObj order)
        {
            try
            {
                using (var context = new RestaurantDBContext())
                {
                    context.OrderObjs.Add(order);
                    context.SaveChanges();
                }
            }
            catch
            {
                return;
            }
        }

        public static List<OrderObj> GetOrderObjs()
        {
            var context = new RestaurantDBContext();
            return context.OrderObjs.ToList();
        }

        public static OrderObj GetOrderById(int id)
        {
            return GetOrderObjs().Where(o => o.Id.Equals(id)).FirstOrDefault();
        }
    }
}
