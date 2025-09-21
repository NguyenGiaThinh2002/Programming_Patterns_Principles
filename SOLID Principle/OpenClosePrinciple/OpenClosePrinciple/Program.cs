namespace OpenClosePrinciple
{
    internal class Program
    {
        // Define an interface for shapes
        // OCP: Defines abstraction so we can extend the system with new shapes
        // without modifying existing code.
        public interface IShape
        {
            double CalculateArea();
        }

        // Concrete implementations of shapes (extensions, no modification needed)
        public class Rectangle : IShape
        {
            public double Width { get; set; }
            public double Height { get; set; }

            public double CalculateArea()
            {
                return Width * Height;
            }
        }

        public class Circle : IShape
        {
            public double Radius { get; set; }

            public double CalculateArea()
            {
                return Math.PI * Radius * Radius;
            }
        }

        public class Triangle : IShape
        {
            public double Base { get; set; }
            public double Height { get; set; }

            public double CalculateArea()
            {
                return 0.5 * Base * Height;
            }
        }

        // Area calculator that works with any IShape
        public class AreaCalculator
        {
            // OCP: Closed for modification (this code never changes),
            // Open for extension (new shapes can be added via IShape).
            public double CalculateArea(IShape shape)
            {
                return shape.CalculateArea();
            }
        }

        static void Main(string[] args)
        {
            var rectangle = new Rectangle { Width = 5, Height = 4 };
            var circle = new Circle { Radius = 3 };
            var triangle = new Triangle { Base = 6, Height = 4 };

            var calculator = new AreaCalculator();

            Console.WriteLine($"Rectangle Area: {calculator.CalculateArea(rectangle)}"); // Output: 20
            Console.WriteLine($"Circle Area: {calculator.CalculateArea(circle)}");       // Output: ~28.27
            Console.WriteLine($"Triangle Area: {calculator.CalculateArea(triangle)}");    // Output: 12
        }
    }

}
