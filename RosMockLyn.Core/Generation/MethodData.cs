using System.Collections.Generic;

namespace RosMockLyn.Core.Generation
{
    public struct MethodData
    {
        private readonly string _interfaceName;

        private readonly string _methodName;

        private readonly string _returnType;

        private readonly IEnumerable<Parameter> _parameters;

        public MethodData(string interfaceName, string methodName, string returnType, IEnumerable<Parameter> parameters)
        {
            _interfaceName = interfaceName;
            _methodName = methodName;
            _returnType = returnType;
            _parameters = parameters;
        }

        public string InterfaceName
        {
            get
            {
                return _interfaceName;
            }
        }

        public string MethodName
        {
            get
            {
                return _methodName;
            }
        }

        public string ReturnType
        {
            get
            {
                return _returnType;
            }
        }

        public IEnumerable<Parameter> Parameters
        {
            get
            {
                return _parameters;
            }
        }
    }
}