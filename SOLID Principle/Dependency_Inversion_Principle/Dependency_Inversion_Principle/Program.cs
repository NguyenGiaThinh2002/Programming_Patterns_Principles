namespace Dependency_Inversion_Principle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ILogger fileLogger = new FileLogger();
            ILogger databaseLogger = new DatabaseLogger();

            var processor1 = new OrderProcessor(fileLogger);
            processor1.ProcessOrder(); // Output: Writing to file: Processing order... \n Order processed.

            var processor2 = new OrderProcessor(databaseLogger);
            processor2.ProcessOrder(); // Output: Writing to database: Processing order... \n Order processed.
        }

        public interface ILogger
        {
            void Log(string message);
        }

        public class FileLogger : ILogger
        {
            public void Log(string message)
            {
                Console.WriteLine($"Writing to file: {message}");
            }
        }

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

            public OrderProcessor(ILogger logger) // Dependency injected via constructor
            {
                _logger = logger;
            }

            public void ProcessOrder()
            {
                _logger.Log("Processing order...");
                Console.WriteLine("Order processed.");
            }
        }
    }
}
