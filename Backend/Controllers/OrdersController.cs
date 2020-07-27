using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.ViewModels;
using Microsoft.AspNetCore.Cors;

namespace Backend.Controllers
{
    [EnableCors("defaultcorspolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly RestaurantDBContext _context;

        public OrdersController(RestaurantDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all Orders with details
        /// </summary>
        /// <returns>All orders with details</returns>
        // GET: api/Orders
        [HttpGet]
        public ActionResult<IEnumerable<OrderViewModel>> GetOrders()
        {
            return OrderViewModel.GetOrders();
        }
        
        /// <summary>
        /// Gets an specific Order with details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Order with details or NotFound</returns>
        // GET: api/Orders/5
        [HttpGet("{id}")]
        public ActionResult<OrderViewModel> GetOrder(int id)
        {
            var order = OrderViewModel.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        /// <summary>
        /// Create a new Order by text description
        /// </summary>
        /// <param name="orderViewModel"></param>
        /// <returns>Order with details or BadRequest</returns>
        // POST: api/Orders
        [HttpPost]
        public ActionResult<OrderViewModel> PostOrder([FromForm] OrderViewModel orderViewModel)
        {
            if (OrderViewModel.InsertOrder(orderViewModel))
            {
                OrderViewModel order = OrderViewModel.GetOrders().Last();
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            return BadRequest();
         }

        public IActionResult GetById(int id)
        {
            return Ok(GetOrder(id));
        }
    }
}
