using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    /// <summary>
    /// Order entity
    /// </summary>
    public class OrderObj
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Time { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
