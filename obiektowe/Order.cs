using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace obiektowe
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("AuthorSub")]
        public User Author { get; set; }
        public string AuthorSub { get; set; }
        public DateTime CreationTime { get; set; }
        public int Number { get; set; }

        public List<OrderItem> Products { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime DeliverTime { get; set; }
    }

    public enum OrderStatus { New, Sent, Accomplished, Rejected, Unaccomplished }
}
