using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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

        public List<string> PermissionSet { get; set; }

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