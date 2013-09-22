using System.IO;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace core.Roles.Models
{
    public class UserEntity : TableEntity
    {
        private string _googleIdNumber;

        public string GoogleIDNumber { 
            get { return _googleIdNumber; }
            set 
            { 
                _googleIdNumber = value;
                RowKey = GoogleIDNumber;
            }
        }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string BattlelogID { get; set; }

        public bool AccountEnabled { get; set; }

        public bool EmailConfirmed { get; internal set; }

        private PermissionSetEntity _permissions;
        public PermissionSetEntity Permissions {
            get { return _permissions; }
            set { 
                _permissions = value;
                _permissions.PermissionSetChanged += ChangedPermissionSet;
                _serializedPermissionSetEntity = JsonConvert.SerializeObject(_permissions);
                PartitionKey = value.Namespace;
            }
        }

        private string _serializedPermissionSetEntity;
        public string SerializedPermissionSetEntity
        {
            get { return _serializedPermissionSetEntity; }
            set
            {
                _serializedPermissionSetEntity = value;
                _permissions = JsonConvert.DeserializeObject<PermissionSetEntity>(value);
                _permissions.PermissionSetChanged += ChangedPermissionSet;
                PartitionKey = _permissions.Namespace;
            }
        }

        public UserEntity()
        {
            GoogleIDNumber = "";
            BattlelogID = "";
            AccountEnabled = true;
            Permissions = new PermissionSetEntity();
            PartitionKey = Permissions.Namespace;
            RowKey = GoogleIDNumber;
        }

        public UserEntity(string googleIDNumber, string email, string battlelogID, bool accountEnabled,
                          PermissionSetEntity permissions)
        {
            GoogleIDNumber = googleIDNumber;
            Email = email;
            BattlelogID = battlelogID;
            AccountEnabled = accountEnabled;
            Permissions = permissions;
            PartitionKey = permissions.Namespace;
            RowKey = GoogleIDNumber;
        }

        internal void ChangedPermissionSet(object sender, ChangedPermissionSetEventArgs args)
        {
            _serializedPermissionSetEntity = JsonConvert.SerializeObject(_permissions);
        }
    }
}
