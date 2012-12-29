using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.ServerInterface
{
    class ServerSetting : TableEntity
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

        public ServerSetting(String address, int port, String password)
        {
            Address = address;
            Port = port;
            Password = password;
        }
    }
}
