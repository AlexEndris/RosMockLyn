using RosMockLyn.Core;

namespace RosMockLyn.Cmd
{
    public class Program
    {
        public static void Main()
        {
            var testingRoslyn = new TestingRoslyn();

            testingRoslyn.DoSomething();
        }
    }
}
