namespace Single_Responsibility_Principle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var order = new Order { Id = 1, Total = 100, Items = new List<string> { "Item1", "Item2" } };
            var processor = new OrderProcessor();
            var repository = new OrderRepository();
            var emailService = new EmailService();

            processor.Process(order);           // Processes the order
            repository.Save(order);            // Saves to database
            emailService.SendConfirmationEmail(order); // Sends email
        }

        public class Order
        {
            public int Id { get; set; }
            public decimal Total { get; set; }
            public List<string> Items { get; set; }
        }

        public class OrderProcessor
        {
            public void Process(Order order)
            {
                Console.WriteLine($"Processing order {order.Id}...");
                // Business logic for order processing
            }
        }

        public class OrderRepository
        {
            public void Save(Order order)
            {
                Console.WriteLine($"Saving order {order.Id} to database...");
                // Database storage logic
            }
        }

        public class EmailService
        {
            public void SendConfirmationEmail(Order order)
            {
                Console.WriteLine($"Sending confirmation email for order {order.Id}...");
                // Email sending logic
            }
        }
    }
}
