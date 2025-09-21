namespace Dependency_Inversion_Principle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ILogger fileLogger = new FileLogger();
            ILogger databaseLogger = new DatabaseLogger();

            var processor1 = new OrderProcessor(fileLogger);
            processor1.ProcessOrder();

            var processor2 = new OrderProcessor(databaseLogger);
            processor2.ProcessOrder();
        }

        // DIP: Abstraction that both high-level (OrderProcessor)
        // and low-level (loggers) depend on.
        public interface ILogger
        {
            void Log(string message);
        }

        // Low-level module depends on abstraction (ILogger), not on OrderProcessor.
        public class FileLogger : ILogger
        {
            public void Log(string message)
            {
                Console.WriteLine($"Writing to file: {message}");
            }
        }

        // Another low-level module depending on abstraction (ILogger).
        public class DatabaseLogger : ILogger
        {
            public void Log(string message)
            {
                Console.WriteLine($"Writing to database: {message}");
            }
        }

        public class OrderProcessor
        {
            private readonly ILogger _logger;

            // DIP: High-level module depends on abstraction (ILogger),
            // not on concrete classes like FileLogger or DatabaseLogger.
            // Dependency is injected from outside.
            public OrderProcessor(ILogger logger)
            {
                _logger = logger;
            }

            public void ProcessOrder()
            {
                _logger.Log("Processing order..."); // Uses abstraction
                Console.WriteLine("Order processed.");
            }
        }
    }

}
