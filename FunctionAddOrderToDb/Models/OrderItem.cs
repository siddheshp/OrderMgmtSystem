using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionAddOrderToDb.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public int OrderId { get; set; }
    }
}
