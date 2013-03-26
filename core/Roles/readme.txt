###############################

PERMISSIONS README

###############################

Permissions are organized into two tables in Azure table storage:

1. Permission Set Table
	Table Name: permissionSets
	Data Object: PermissionSetEntity
	Partition Key: Namespace
	Row Key: Namespace
	Usage: Stores available permissions, indexed by namespace. 
		These represent the list of permissions that *can* be given out for any given namespace.

2. User Table
	Table Name: users
	Data Object: UserEntity
	Partition Key: Namespace
	Row Key: GoogleUsername
	Usage: Stores actual permissions for each user, indexed by namespace and the Google username of the user. 
		Each user has a list of permissions for each namespace.
		Permissions checks for a namespace are simply a check for whether or not a particular permission string exists in a user's permission list for that given namespace.