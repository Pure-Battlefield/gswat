using System.Collections;
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

        private string _namespace;
        public string Namespace {
            get { return _namespace; }
            set 
            { 
                _namespace = value;
                PartitionKey = Namespace;
            } 
        }

        public bool AccountEnabled { get; set; }
        public ArrayList Permissions { get; set; }

        public UserEntity()
        {
            GoogleUsername = "";
            BattlelogID = "";
            Namespace = "";
            AccountEnabled = true;
            Permissions = new ArrayList();
            PartitionKey = Namespace;
            RowKey = GoogleUsername;
        }

        public UserEntity(string googleUsername, string battlelogID, bool accountEnabled,
                          ArrayList permissions)
        {
            GoogleUsername = googleUsername;
            BattlelogID = battlelogID;
            AccountEnabled = accountEnabled;
            Permissions = permissions;
            PartitionKey = Namespace;
            RowKey = GoogleUsername;
        }

        public void AddPermission(string permissionName)
        {
            
        }

        public void RemovePermission(string permissionName)
        {
            
        }
    }
}
