using RosMockLyn.Mocking.Routing;

namespace RosMockLyn.Mocking
{
    public abstract class MockBase : IMock
    {
        protected MockBase()
        {
            SubstitutionContext = new MockSubstitutionContext();
        }

        public ISubstitutionContext SubstitutionContext { get; private set; }
    }
}