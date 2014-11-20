using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace RosMockLyn.Mocking
{
    public abstract class MockBase : IMock
    {
        public static readonly ICallRecorder Recorder = new CallRecorder();

        protected bool asserting;
        private int expectedCalls;

        public void Returns<T>(string calledMember, T value)
        {

            this.SetReturnValue(calledMember, value);
        }

        private void SetReturnValue(string calledMember, object value)
        {
            var fieldInfo = this.GetType().GetRuntimeFields()
                                .First(x => x.Name == string.Format("{0}_ReturnValue", calledMember));

            fieldInfo.SetValue(this, value);
        }

        public void Received(int expectedCalls)
        {
            this.asserting = true;
            this.expectedCalls = expectedCalls;
        }

        public void AssertCalled(int actual)
        {
            Assert.AreEqual(this.expectedCalls, actual);
            this.asserting = false;
        }

        protected void Record([CallerMemberName] string caller = "")
        {
            Recorder.Record(this, caller);
        }
    
    }
}