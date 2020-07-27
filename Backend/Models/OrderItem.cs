using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    /// <summary>
    /// Table to store the specific Dish and the Order ID of it.
    /// The table exists because in a real-life scenario we would create a "Dishes" table, and this entity would make easier to link a Dish to an Order.
    /// </summary>
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Order")]
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int DishId { get; set; }
    }
}
