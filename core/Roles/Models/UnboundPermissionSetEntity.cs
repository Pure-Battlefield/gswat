using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace core.Roles.Models
{
    public class UnboundPermissionSetEntity : TableEntity 
    {
        public string Guid
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
                _permissions = JsonConvert.DeserializeObject<PermissionSetEntity>(value);
                PartitionKey = _permissions.Namespace;
            }
        }

        public PermissionSetEntity Permissions
        {
            get { return _permissions; }
            set
            {
                _permissions = value;
                _serializedPermissionSetEntity = JsonConvert.SerializeObject(_permissions);
                PartitionKey = value.Namespace;
            }
        } 
    }
}
