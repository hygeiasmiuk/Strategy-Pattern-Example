using System;
using System.Security.AccessControl;
using System.IO;
using System.Security.Principal;
namespace ConsoleApplication_test_20151202
{
    class Permission_Modify
    {
        public static void AddDirectoryPermissions(string DirectoryName, string UserAccount, string Right, PropagationFlags PropgaFlag)
        {
            Console.WriteLine("add:1");
            DirectoryInfo directoryInfo = new DirectoryInfo(DirectoryName);
            Console.WriteLine("add:2");
            DirectorySecurity dirSecurity = directoryInfo.GetAccessControl();
            Console.WriteLine("add:3");
            switch (Right)
            {
                case "FullControl":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.FullControl,
                                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
                case "Modify":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.Modify,
                                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
                case "ListDirectory":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.ListDirectory,
                                            InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
                case "ReadAndExecute":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.ReadAndExecute,
                                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
                case "Read":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.Read,
                                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
                case "Write":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.Write,
                                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
                default:
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(UserAccount, FileSystemRights.FullControl,
                                            InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                            PropgaFlag, AccessControlType.Allow));
                    break;
            }
            Console.WriteLine("add:4");
            directoryInfo.SetAccessControl(dirSecurity);// Set the access settings.
        }

        public static void PurgeDirectoryPermissions(string DirectoryName)
        {
            Console.WriteLine("purge:1");
            DirectoryInfo dInfo = new DirectoryInfo(DirectoryName);
            Console.WriteLine("purge:2");
            DirectorySecurity dirSecurity = dInfo.GetAccessControl();
            Console.WriteLine("purge:3");
            AuthorizationRuleCollection acl = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
            Console.WriteLine("purge:4");
            foreach (FileSystemAccessRule ace in acl) //to remove any other permission
            {
                try
                {
                    if (ace.IdentityReference.Value.Split('\\')[0] == "YFY")
                    {
                        Console.WriteLine(string.Format("{0} is deleted", ace.IdentityReference.Value.ToString()));
                        dirSecurity.PurgeAccessRules(ace.IdentityReference);  //same as use objSecObj.RemoveAccessRuleSpecific(ace);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine(string.Format("IndexOutOfRangeException: {0}", ace.IdentityReference.Value));
                    continue;
                }
                catch (IdentityNotMappedException)
                {
                    Console.WriteLine(string.Format("IdentityNotMappedException: {0}", ace.IdentityReference.Value));
                    continue;
                }
            }
            Console.WriteLine("purge:5");
            dInfo.SetAccessControl(dirSecurity);

        }
        public static void TakeOwnerShip(DirectoryInfo di)
        {
        }
    }
}