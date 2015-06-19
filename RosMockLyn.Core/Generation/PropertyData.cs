namespace RosMockLyn.Core.Generation
{
    public struct PropertyData
    {
        private readonly string _interfaceName;

        private readonly string _type;

        private readonly string _name;

        private readonly bool _hasSetter;

        public PropertyData(string interfaceName, string type, string name, bool hasSetter)
        {
            _name = name;
            _type = type;
            _hasSetter = hasSetter;
            _interfaceName = interfaceName;
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

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool HasSetter
        {
            get
            {
                return _hasSetter;
            }
        }
    }
}