using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.TableStoreEntities
{
    public class ServerSettingsEntity : TableEntity
    {
        private String _address;
        private int _port;

        public String Address
        {
            get { return _address; }
            set {
                PartitionKey = value;
                _address = value;
            }
        }

        public int Port
        {
            get { return _port; }
            set { 
                RowKey = value + "";
                _port = value;
            }
        }

        public string Password { get; set; }

        public ServerSettingsEntity(String address, int port, String password)
        {
            Address = address;
            Port = port;
            Password = password;
        }

        public ServerSettingsEntity()
        {
            Address = "";
            Port = 0;
            Password = "";
        }
    }
}
