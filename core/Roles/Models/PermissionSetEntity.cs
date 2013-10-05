using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace core.Roles.Models
{
    internal delegate void PermissionsChangedDelegate(object sender, ChangedPermissionSetEventArgs e);

    public class PermissionSetEntity: TableEntity
    {
        private string _namespace;
        internal event PermissionsChangedDelegate PermissionSetChanged;

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

        private List<string> _permissionSet; 
        public List<string> PermissionSet
        {
            set
            {
                _permissionSet = value;
                SerializePermissionSet();
            }
        }

        public IEnumerable<string> GetPermissionSet()
        {
            return _permissionSet.AsReadOnly();
        }

        public void RemovePermission(IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
            {
                _permissionSet.RemoveAll(s => s == permission);
            }
        }

        public void AddPermissions(IEnumerable<string> permissions)
        {
            foreach (var permission in permissions)
            {
                _permissionSet.Add(permission);
            }
            SerializePermissionSet();
        }

        private string _serializedPermissionSet;
        public string SerializedPermissionSet
        {
            get { return _serializedPermissionSet; }
            set
            {
                _serializedPermissionSet = value;
                _permissionSet = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }

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

        public override bool Equals(object obj)
        {
            if (!(obj is PermissionSetEntity))
            {
                throw new ArgumentException("Not a PermissionSetEntity", "obj");
            }
            var other = (PermissionSetEntity) obj;
            if (Namespace == other.Namespace)
            {
                return Enumerable.SequenceEqual(_permissionSet, other.GetPermissionSet());
            }
            return false;
        }

        internal void SerializePermissionSet()
        {
            _serializedPermissionSet = JsonConvert.SerializeObject(_permissionSet);

            if (PermissionSetChanged != null)
            {
                var args = new ChangedPermissionSetEventArgs {SerializedPermissionSet = _serializedPermissionSet};
                PermissionSetChanged(this, args);
            }
        }
    }

    internal class ChangedPermissionSetEventArgs : EventArgs
    {
        internal string SerializedPermissionSet { get; set; }
    }
}