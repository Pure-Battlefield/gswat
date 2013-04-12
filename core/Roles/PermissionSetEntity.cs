using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.Roles
{
    public class PermissionSetEntity: TableEntity
    {
        private string _namespace;

        public string Namespace
        {
            get { return _namespace; }
            set
            {
                _namespace = value;
                RowKey = Namespace;
                PartitionKey = Namespace;
            }
        }

        private List<string> _permissionSet; 
        public List<string> PermissionSet
        {
            get { return _permissionSet; }
            set
            {
                _permissionSet = value;
                StringWriter righter = new StringWriter();
                XmlSerializer cereal = new XmlSerializer(typeof(List<string>));
                cereal.Serialize(righter, value);
                _serializedPermissionSet = righter.ToString();
            }
        }

        private string _serializedPermissionSet;
        public string SerializedPermissionSet
        {
            get { return _serializedPermissionSet; }
            set
            {
                _serializedPermissionSet = value;
                StringReader righter = new StringReader(value);
                XmlSerializer cereal = new XmlSerializer(typeof(List<string>));
                _permissionSet = (List<string>)cereal.Deserialize(righter);
            }
        }

        public PermissionSetEntity()
        {
            Namespace = "";
            PermissionSet = new List<String>();
        }

        public PermissionSetEntity(string nameSpace, List<String> permissionSet)
        {
            Namespace = nameSpace;
            PermissionSet = permissionSet;
        }
    }
}