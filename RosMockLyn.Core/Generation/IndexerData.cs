using System.Collections.Generic;

namespace RosMockLyn.Core.Generation
{
    public struct IndexerData
    {
        private readonly string _interfaceName;

        private readonly string _type;

        private readonly bool _hasSetter;

        private readonly IEnumerable<Parameter> _parameters;

        public IndexerData(string interfaceName, string type, IEnumerable<Parameter> parameters, bool hasSetter)
        {
            _interfaceName = interfaceName;
            _type = type;
            _hasSetter = hasSetter;
            _parameters = parameters;
        }

        public string InterfaceName
        {
            get
            {
                return _interfaceName;
            }
        }

        public string Type
        {
            get
            {
                return _type;
            }
        }

        public bool HasSetter
        {
            get
            {
                return _hasSetter;
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