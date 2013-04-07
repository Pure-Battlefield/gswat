using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;
using core.Logging;

namespace core.Roles
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

        public string Email { get; set; }

        public string BattlelogID { get; set; }

        public bool AccountEnabled { get; set; }

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
    }
}
