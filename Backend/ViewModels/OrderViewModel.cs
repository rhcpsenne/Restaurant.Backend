using Backend.Access;
using Backend.BLL;
using Backend.Models;
using Backend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Backend.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderDescription { get; set; }
        public string OrderDate { get; set; }


        /// <summary>
        /// Validate the string (if it is in a valid format) and sorts it
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<int> ValidateOrder(List<string> items)
        {
            try
            {
                return items.ConvertAll(int.Parse).OrderBy(p => p).ToList(); 
            }
            catch
            {
                return null;
            }
        }

        public static List<OrderViewModel> GetOrders()
        {
            List<OrderViewModel> orders = new List<OrderViewModel>();
            var orderObjs = OrderAccess.GetOrderObjs();
            foreach(var order in orderObjs)
            {
                orders.Add(GetOrder(order));
            }

            return orders;
        }

        public static OrderViewModel GetOrder(OrderObj order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                OrderDescription = GetOrderDescriptionFromOrder(order)
            };
        }

        /// <summary>
        /// All the logical process to show the outputs are here
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string GetOrderDescriptionFromOrder(OrderObj order)
        {
            bool error = false, keep = true;
            List<Tuple<int, int>> values = new List<Tuple<int, int>>();
            StringBuilder description = new StringBuilder();
            List<OrderItem> items = new List<OrderItem>();
            items = OrderItemAccess.GetOrderItemsByOrder(order.Id);
            values = GetTuples(items);

            foreach(var item in values.Distinct())
            {
                if (keep)
                {
                    switch (order.Time)
                    {
                        case (int)TimeEnum.morning:
                            if (item.Item2 > 1)
                            {
                                if (item.Item1 == (int)MorningEnum.coffee)
                                    description.Append((MorningEnum)Enum.ToObject(typeof(MorningEnum), item.Item1) + "(x" + item.Item2 + "), ");
                                else if (error)
                                {
                                    description.Append((MorningEnum)Enum.ToObject(typeof(MorningEnum), (int)MorningEnum.error) + ", ");
                                    keep = false;
                                }
                                else
                                {
                                    description.Append((MorningEnum)Enum.ToObject(typeof(MorningEnum), item.Item1) + ", ");
                                    description.Append((MorningEnum)Enum.ToObject(typeof(MorningEnum), (int)MorningEnum.error) + ", ");
                                    error = true;
                                    keep = false;
                                }
                            }
                            else
                            {
                                description.Append((MorningEnum)Enum.ToObject(typeof(MorningEnum), item.Item1) + ", ");
                            }
                            break;
                        case (int)TimeEnum.night:
                            if (item.Item2 > 1)
                            {
                                if (item.Item1 == (int)NightEnum.potato)
                                    description.Append((NightEnum)Enum.ToObject(typeof(NightEnum), item.Item1) + "(x" + item.Item2 + "), ");
                                else if (error)
                                {
                                    description.Append((NightEnum)Enum.ToObject(typeof(NightEnum), (int)NightEnum.error) + ", ");
                                    keep = false;
                                }
                                else
                                {
                                    description.Append((NightEnum)Enum.ToObject(typeof(NightEnum), item.Item1) + ", ");
                                    description.Append((NightEnum)Enum.ToObject(typeof(NightEnum), (int)NightEnum.error) + ", ");
                                    error = true;
                                    keep = false;
                                }
                            }
                            else
                            {
                                description.Append((NightEnum)Enum.ToObject(typeof(NightEnum), item.Item1) + ", ");
                            }
                            break;
                    }
                }
            }

            return description.ToString().Remove(description.ToString().Length - 2, 2);
        }

        /// <summary>
        /// Returns a list of Tuples which contains in each value the dish and how many times it appears on the same order
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static List<Tuple<int,int>> GetTuples(List<OrderItem> items)
        {
            List<int> list = new List<int>();
            List<Tuple<int, int>> values = new List<Tuple<int, int>>();
            foreach (var item in items)
            {
                int count = items.Where(c => c.DishId.Equals(item.DishId)).Count();
                if (!list.Contains(item.DishId))
                {
                    values.Add(new Tuple<int, int>(item.DishId, count)); //Tuple values: Dish and how many times it appears
                }
            }
            return values;
        }

        /// <summary>
        /// Method called by Controller to insert the order with the database structure, receiving only the client description of order.
        /// </summary>
        /// <param name="orderViewModel"></param>
        /// <returns></returns>
        public static bool InsertOrder(OrderViewModel orderViewModel)
        {
            try
            {
                var context = new RestaurantDBContext();
                List<string> itemList = new List<string>();
                List<int> numbers = new List<int>();
                itemList = orderViewModel.OrderDescription.Split(",").ToList();
                OrderObj order = GetOrderObj(itemList.FirstOrDefault().ToLower().Trim());
                itemList.RemoveRange(0,1);
                numbers = ValidateOrder(itemList);

                if(numbers != null && numbers.Count > 0)
                {
                    OrderAccess.AddOrder(order);
                    return InsertAllOrderItems(numbers);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method called by Controller to return a specific order by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static OrderViewModel GetOrderById(int id)
        {
            OrderObj order = OrderAccess.GetOrderById(id);
            return GetOrder(order);
        }

        public static OrderObj GetOrderObj(string time)
        {
            return new OrderObj { OrderDate = DateTime.Now, Time = Convert.ToInt32(Enum.Parse(typeof(TimeEnum), time)) };
        }

        /// <summary>
        /// Calls the Access label to insert all the order items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool InsertAllOrderItems(List<int> items)
        {
            try
            {
                foreach(var item in items)
                {
                    OrderItem order = new OrderItem
                    {
                        OrderId = OrderAccess.GetOrderObjs().Last().Id,
                        DishId = item
                    };
                    OrderItemAccess.AddOrderItem(order);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static OrderObj GetLastOrder()
        {
            return OrderAccess.GetOrderObjs().Last();
        }
    }
}
