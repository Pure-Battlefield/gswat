using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.Roles
{
    public class UserEntity : TableEntity
    {
        private string _googleUsername;
        public string GoogleUsername { 
            get { return _googleUsername; }
            set 
            { 
                _googleUsername = value;
                RowKey = GoogleUsername;
            }
        }

        public string BattlelogID { get; set; }

        public bool AccountEnabled { get; set; }

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

        private PermissionSetEntity _permissions;
        public PermissionSetEntity Permissions {
            get { return _permissions; }
            set { 
                _permissions = value;
                StringWriter righter = new StringWriter();
                XmlSerializer cereal = new XmlSerializer(typeof(PermissionSetEntity));
                cereal.Serialize(righter, value);
                _serializedPermissionSetEntity = righter.ToString();
                PartitionKey = value.Namespace;
            }
        }

        public UserEntity()
        {
            GoogleUsername = "";
            BattlelogID = "";
            AccountEnabled = true;
            Permissions = new PermissionSetEntity();
            PartitionKey = Permissions.Namespace;
            RowKey = GoogleUsername;
        }

        public UserEntity(string googleUsername, string battlelogID, bool accountEnabled,
                          PermissionSetEntity permissions)
        {
            GoogleUsername = googleUsername;
            BattlelogID = battlelogID;
            AccountEnabled = accountEnabled;
            Permissions = permissions;
            PartitionKey = permissions.Namespace;
            RowKey = GoogleUsername;
        }
    }
}
