namespace Liskov_Substitution_Principle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var watcher = new BirdWatcher();
            var sparrow = new Sparrow();
            var ostrich = new Ostrich();

            watcher.MakeBirdFly(sparrow); // Works: "Sparrow is flying"
            //watcher.MakeBirdFly(ostrich); // Won't compile: Ostrich doesn't implement IFlyingBird
            ostrich.Eat(); // Works: "Bird is eating"
        }

        public interface IFlyingBird
        {
            void Fly();
        }

        public class Bird
        {
            // Common bird behavior, e.g., eating
            public void Eat()
            {
                Console.WriteLine("Bird is eating");
            }
        }

        public class Sparrow : Bird, IFlyingBird
        {
            public void Fly()
            {
                Console.WriteLine("Sparrow is flying");
            }
        }

        public class Ostrich : Bird
        {
            // Ostrich doesn't implement IFlyingBird because it can't fly
        }

        public class BirdWatcher
        {
            public void MakeBirdFly(IFlyingBird bird)
            {
                bird.Fly(); // Only works with birds that can fly
            }
        }
    }
}
