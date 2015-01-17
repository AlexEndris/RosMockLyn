using RosMockLyn.Mocking.Routing;
using RosMockLyn.Mocking.Routing.Invocations;

namespace RosMockLyn.Mocking
{
    public abstract class MockBase : IMock
    {
        protected MockBase()
        {
            SubstitutionContext = new MockSubstitutionContext(
                new MethodInvocationHandler(),
                new PropertyInvocationHandler(),
                new IndexInvocationHandler());
        }

        public ISubstitutionContext SubstitutionContext { get; private set; }
    }
}