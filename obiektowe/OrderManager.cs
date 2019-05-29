using System.Collections.Generic;
using System.Linq;

namespace obiektowe
{
    public class OrderManager
    {
        private readonly ContextDB _context;

        public OrderManager(ContextDB context)
        {
            _context = context;
        }

        public Order GetOrder(int number)
        {
            return _context.Orders.FirstOrDefault(order => order.Number == number);
        }

        public List<Order> GetOrders(OrderStatus status)
        {
            return _context.Orders.Where(order => order.Status == status).ToList();
        }

        public List<Order> GetOrders(User author)
        {
            return _context.Orders.Where(order => order.Author == author).ToList();
        }

        public List<Order> GetOrders(List<int> numbers)
        {
            return _context.Orders.Where(order => numbers.Contains(order.Number)).ToList();
        }
    }
}
