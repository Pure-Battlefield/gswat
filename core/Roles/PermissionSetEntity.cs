using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public List<String> PermissionSet { get; set; }

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