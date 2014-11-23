using RosMockLyn.Mocking.Routing;

namespace RosMockLyn.Mocking
{
    public abstract class MockBase : IMock
    {
        protected MockBase()
        {
            CallRouter = new MockCallRouter();
        }

        public ICallRouter CallRouter { get; private set; }
    }
}