using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosMockLyn.Cmd
{
    using RosMockLyn.Core;

    class Program
    {
        static void Main(string[] args)
        {
            var testingRoslyn = new TestingRoslyn();

            testingRoslyn.DoSomething();
        }
    }
}
