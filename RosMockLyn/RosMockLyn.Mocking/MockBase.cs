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
            SetReturnValue(calledMember, value);
            IgnorePreviousCall(calledMember);
        }

        private void IgnorePreviousCall(string calledMember)
        {
            var fieldInfo = GetType().GetRuntimeFields()
                                     .First(x => x.Name == string.Format("{0}_Calls", calledMember));

            var value = ((int)fieldInfo.GetValue(this)) - 1;
            fieldInfo.SetValue(this, value);
        }

        private void SetReturnValue(string calledMember, object value)
        {
            var fieldInfo = GetType().GetRuntimeFields()
                                     .First(x => x.Name == string.Format("{0}_ReturnValue", calledMember));

            fieldInfo.SetValue(this, value);
        }

        public void Received(int numExpectedCalls)
        {
            asserting = true;
            expectedCalls = numExpectedCalls;
        }

        public void AssertCalled(int actual)
        {
            Assert.AreEqual(expectedCalls, actual);
            this.asserting = false;
        }

        protected void Record([CallerMemberName] string caller = "")
        {
            Recorder.Record(this, caller);
        }
    
    }
}