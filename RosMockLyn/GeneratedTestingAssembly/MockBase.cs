using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeneratedTestingAssembly
{
    public abstract class MockBase : IMock
    {
        public static readonly ICallRecorder Recorder = new CallRecorder();

        protected bool asserting;
        private int expectedCalls;

        public void Returns<T>(string calledMember, T value)
        {

            SetReturnValue(calledMember, value);
        }

        private void SetReturnValue(string calledMember, object value)
        {
            FieldInfo fieldInfo = this.GetType()
                .GetField(
                    string.Format("{0}_ReturnValue", calledMember),
                    BindingFlags.Instance | BindingFlags.NonPublic);

            fieldInfo.SetValue(this, value);
        }

        public void Received(int expectedCalls)
        {
            asserting = true;
            this.expectedCalls = expectedCalls;
        }

        public void AssertCalled(int actual)
        {
            Assert.AreEqual(expectedCalls, actual);
            asserting = false;
        }

        protected void Record([CallerMemberName] string caller = "")
        {
            Recorder.Record(this, caller);
        }
    
    }
}