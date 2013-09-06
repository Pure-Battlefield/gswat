using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.Roles
{
    public class UnboundPermissionSetEntity : TableEntity 
    {
        public string Email
        {
            get { return RowKey; }
            set { RowKey = value; }
        }
        public string Namespace
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        private PermissionSetEntity _permissions;
        private string _serializedPermissionSetEntity;

        public string SerializedPermissionSetEntity
        {
            get { return _serializedPermissionSetEntity; } 
            set
            {
                _serializedPermissionSetEntity = value;
                StringReader righter = new StringReader(value);
                XmlSerializer cereal = new XmlSerializer(typeof(PermissionSetEntity));
                _permissions = (PermissionSetEntity)cereal.Deserialize(righter);
                PartitionKey = _permissions.Namespace;
            }
        }

        public PermissionSetEntity Permissions
        {
            get { return _permissions; }
            set
            {
                _permissions = value;
                StringWriter righter = new StringWriter();
                XmlSerializer cereal = new XmlSerializer(typeof (PermissionSetEntity));
                cereal.Serialize(righter, value);
                _serializedPermissionSetEntity = righter.ToString();
                PartitionKey = value.Namespace;
            }
        } 
    }
}
