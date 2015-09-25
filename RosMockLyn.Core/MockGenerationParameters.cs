using System.Collections.Generic;
using RosMockLyn.Core.Generation;

namespace RosMockLyn.Core
{
    public class MockGenerationParameters
    {
        private readonly string _namespaceName;

        private readonly ClassData _classData;

        private readonly IEnumerable<MethodData> _methodDatas;

        private readonly IEnumerable<PropertyData> _propertyDatas;

        private readonly IEnumerable<IndexerData> _indexerDatas;

        public MockGenerationParameters(string namespaceNameName, ClassData classData, IEnumerable<MethodData> methodDatas, IEnumerable<PropertyData> propertyDatas, IEnumerable<IndexerData> indexerDatas)
        {
            _namespaceName = namespaceNameName;
            _classData = classData;
            _methodDatas = methodDatas;
            _propertyDatas = propertyDatas;
            _indexerDatas = indexerDatas;
        }

        public string NamespaceName
        {
            get { return _namespaceName; }
        }

        public ClassData ClassData
        {
            get { return _classData; }
        }

        public IEnumerable<MethodData> MethodDatas
        {
            get { return _methodDatas; }
        }

        public IEnumerable<PropertyData> PropertyDatas
        {
            get { return _propertyDatas; }
        }

        public IEnumerable<IndexerData> IndexerDatas
        {
            get { return _indexerDatas; }
        }
    }
}