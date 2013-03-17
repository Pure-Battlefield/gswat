using System;
using System.Collections;
using System.Collections.Generic;
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

        private PermissionSetEntity _permissions;
        public PermissionSetEntity Permissions {
            get { return _permissions; }
            set { 
                _permissions = value;
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
