namespace Interface_Segregation_Principle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var human = new HumanWorker();
            var robot = new RobotWorker();
            var manager = new Manager();

            manager.AssignWork(human); // Works: "Human is working"
            manager.AssignWork(robot); // Works: "Robot is working"
            manager.ProvideLunch(human); // Works: "Human is eating"
                                         // manager.ProvideLunch(robot); // Won't compile: Robot doesn't implement IEatable
        }

        

        public interface IWorkable
        {
            void Work();
        }

        public interface IEatable
        {
            void Eat();
        }

        public class HumanWorker : IWorkable, IEatable
        {
            public void Work()
            {
                Console.WriteLine("Human is working");
            }

            public void Eat()
            {
                Console.WriteLine("Human is eating");
            }
        }

        public class RobotWorker : IWorkable
        {
            public void Work()
            {
                Console.WriteLine("Robot is working");
            }
            // No need to implement IEatable
        }

        public class Manager
        {
            public void AssignWork(IWorkable worker)
            {
                worker.Work(); // Only depends on IWorkable
            }

            public void ProvideLunch(IEatable worker)
            {
                worker.Eat(); // Only depends on IEatable
            }
        }
    }
}
