using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using log4net;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Management;
using System.Collections;
using System.Diagnostics;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace ConsoleApplication_test_20151202
{
    public class Program
    {
        //Private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog log = LogManager.GetLogger("Mylog");
        public static List<string> StringSeparator(string DiskString)
        {
            List <string> DiskStringList = new List<string>();
            DiskStringList = DiskString.Split(';').ToList();
            return DiskStringList;
        }

        public static List<string> GetAccessControlInfo(List<string> dirPath)
        {
            List<string> dirs_storage = new List<string>();
            foreach (string dir in dirPath)
            {
                try
                {
                    List<string> layer_dirs = new List<string>(Directory.EnumerateDirectories(dir, "*", SearchOption.TopDirectoryOnly));
                    if (0 < layer_dirs.Count)
                    {
                        dirs_storage.AddRange(layer_dirs);
                        List<string> sub_dirs = GetAccessControlInfo(layer_dirs);
                        dirs_storage.AddRange(sub_dirs);
                    }
                }
                catch (NotSupportedException NSEx)
                { Console.WriteLine(NSEx.Message); }//path typo occurs
                catch (UnauthorizedAccessException UAEx)
                { Console.WriteLine(UAEx.Message); }
                catch (PathTooLongException PathEx)
                { Console.WriteLine(PathEx.Message); }
                catch (DirectoryNotFoundException PathEx)
                { Console.WriteLine(PathEx.Message+ "in GetAccessControlInfo"); }
            }
            return dirs_storage;
        }

        public static Hashtable getMapping(String Filename)
        {
            Hashtable HT = new Hashtable();
            if (!File.Exists(Filename))
            {
                return HT;
            }
            StreamReader csvdata = ETL.Read_File.CSV_File(Filename);
            while (!csvdata.EndOfStream)
            {
                var line = csvdata.ReadLine();
                var values = line.Split(',');
                try
                {
                    HT.Add(values[0], values[1]);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return HT;
        }

        public class DirAccessRule
        {
            public string User { get; set; }
            public string Right { get; set; }
            public PropagationFlags PropagationRule { get; set; }
        }

        // Get target documentaries' access control info (users' info are included)
        public static List<DirAccessRule> GetUserListofDoc(DirectoryInfo di)
        {
            List < DirAccessRule > DARList= new List<DirAccessRule>();
            try
            {
                Console.WriteLine("GetUserListofDoc:0");
                DirectorySecurity ds = di.GetAccessControl(AccessControlSections.Access);
                Console.WriteLine("GetUserListofDoc:1");
                foreach (FileSystemAccessRule fsar in ds.GetAccessRules(true, true, typeof(NTAccount)))
                {
                    //string dir_owner = ds.GetOwner(typeof(NTAccount)).Value.ToString();

                    string userName = fsar.IdentityReference.Value.ToLower();
                    string userRights = fsar.FileSystemRights.ToString().Split(',').ToList()[0];

                    //string userAccessType = fsar.AccessControlType.ToString();
                    //string ruleSource = fsar.IsInherited ? "Inherited" : "Explicit";
                    //string rulePropagation = fsar.PropagationFlags.ToString();
                    //string ruleInheritance = fsar.InheritanceFlags.ToString();
                    if (!fsar.IsInherited)
                    {
                        DirAccessRule permission_item = new DirAccessRule();
                        permission_item.User = userName;
                        permission_item.Right = userRights;

                        if (fsar.PropagationFlags.ToString() == "None")
                        {
                            Console.WriteLine();
                            permission_item.PropagationRule = PropagationFlags.None;
                        }
                        else if (fsar.PropagationFlags.ToString() == "NoPropagateInherit")
                        {
                            permission_item.PropagationRule = PropagationFlags.NoPropagateInherit;
                        }
                        else
                        {
                            permission_item.PropagationRule = PropagationFlags.InheritOnly;
                        }
                        DARList.Add(permission_item);
                    }
                }
            }
            catch (ArgumentException)
            {
                //same user appear twice or above
                //Console.WriteLine("the user name has already existed");
            }
            catch (UnauthorizedAccessException ex)
                { Console.WriteLine(ex.Message); }
            catch (DirectoryNotFoundException PathEx)
                { Console.WriteLine(PathEx.Message + "in GetUserListofDoc"); }
            return DARList;
        }
        // Check user lists in file server and auth
        //public class Authentificate
        //{
        //    public static Dictionary<string, string> UserListAuth(List<string> old_LDAP_userlist, List<string> new_LDAP_userlist, List<string> FileSrvUserList, DataTable MappingTable)
        //    {
        //        Dictionary<string, string> PassedUserDict = new Dictionary<string, string>();
        //        foreach (string old_user in old_LDAP_userlist)
        //        {
        //            DataRow[] CSV_Matching_result = MappingTable.Select("old_name ='" + old_user + "'");//FIND MATCHED OLD USER

        //            if (CSV_Matching_result.Length == 1)
        //            {
        //                foreach (DataRow Matching_old_new_user in CSV_Matching_result)
        //                {
        //                    try
        //                    {
        //                        if (new_LDAP_userlist.Contains(Matching_old_new_user[1]))//check if new user is in New LDAP
        //                        {
        //                            if (FileSrvUserList.Contains(Matching_old_new_user[1]))//check if file server contains new user account
        //                            {
        //                                //Console.WriteLine("Authentication succeeds");
        //                                PassedUserDict.Add(Matching_old_new_user[0].ToString(), Matching_old_new_user[1].ToString());
        //                            }
        //                            else
        //                            { log.Error("New user name: " + Matching_old_new_user[1] + ", Error Reason: New user name does not exist(Authentification failure)"); }
        //                        }
        //                        else
        //                        { log.Error("Old user name: " + Matching_old_new_user[0] + ", Error Reason: New user name does not exist(Account does not exist)"); }
        //                    }
        //                    catch (ArgumentNullException msg)
        //                    { Console.WriteLine(msg.Message); }
        //                }
        //            }
        //            else
        //            { log.Error("Old user name: " + old_user + ", Error Reason: repeated old user name or old user does not exist)"); }
        //        }
        //        return PassedUserDict;
        //    }
        //}
    
        //public static List<string> FileServerUserList()
        //{
        //    SelectQuery query = new SelectQuery("Win32_UserAccount");
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
        //    List<string> FileSvrUserList = new List<string>();
        //    foreach (ManagementObject user in searcher.Get())
        //    {
        //        FileSvrUserList.Add(user["Name"].ToString());
        //    }
        //    return FileSvrUserList;
        //}
        public static void YFY_Logic(List<string> user_list, List<string> group_list, List<string> total_dirs, Hashtable mappingHT)
        {
            long dir_number = total_dirs.Count;
            List<DirAccessRule> DirAccessRuleList = new List<DirAccessRule>();  //record user rights of directory
            foreach (var dir in total_dirs)
            {
                Console.WriteLine(dir_number + " dirs left...");
                dir_number = dir_number - 1;
                Console.WriteLine("Dir is: " + dir);
                DirectoryInfo di = new DirectoryInfo(dir);
                string NewDomainName = ConfigurationManager.AppSettings["NewDomainName"];
                if (Directory.Exists(dir))
                {
                    if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden) 
                        { continue; }
                    Console.WriteLine(dir+" start Purge");
                    Permission_Modify.PurgeDirectoryPermissions(dir);
                    Console.WriteLine(dir + " end Purge");
                    Console.WriteLine(dir + " start Get userlist");
                    DirAccessRuleList = GetUserListofDoc(di);
                    Console.WriteLine(dir + " end Get userlist");
                    foreach (DirAccessRule PermissionObj in DirAccessRuleList)
                    {
                        Console.WriteLine(string.Format("User: {0}, Right: {1}, Propagation: {2}",PermissionObj.User,PermissionObj.Right,PermissionObj.PropagationRule.ToString()));
                        #region Add the access control entry to the directory for users.
                        try
                        {
                            string username = PermissionObj.User.Split('\\')[1]; // Domain name and user name ex: YFY\user1, P-FYF\user1
                            string CompleteNewDomainUser = username + NewDomainName;
                            if (user_list.Contains(username) || group_list.Contains(username))
                            {
                                Permission_Modify.AddDirectoryPermissions(dir, CompleteNewDomainUser, PermissionObj.Right, PermissionObj.PropagationRule);
                            }
              
                            else if (mappingHT.Contains(username))
                            {
                                string mappingUser = mappingHT[username].ToString();
                                string MappingCompleteNewDomainUser = mappingUser + NewDomainName;
                                Permission_Modify.AddDirectoryPermissions(dir, MappingCompleteNewDomainUser, PermissionObj.Right, PermissionObj.PropagationRule);
                            }
                            else
                            {
                                log.Error("Dir: "+dir.ToString()+ ", Name: " + username + " not FOUND or already exist");
                            }
                        }
                        catch (KeyNotFoundException ex)
                        {
                            Console.WriteLine(dir.ToString()+" "+ex.Message);
                        }
                        catch (IdentityNotMappedException ex)
                        {
                            Console.WriteLine(dir.ToString() + " " + ex.Message);
                        }
                        catch (System.Runtime.InteropServices.COMException)
                        {
                            System.Runtime.InteropServices.COMException exception = new System.Runtime.InteropServices.COMException();
                            Console.WriteLine(dir.ToString() + " " + exception.Message);
                        }
                        catch (InvalidOperationException)
                        {
                            InvalidOperationException InvOpEx = new InvalidOperationException();
                            Console.WriteLine(dir.ToString() + " " + InvOpEx.Message);
                        }
                        catch (NotSupportedException)
                        {
                            NotSupportedException NotSuppEx = new NotSupportedException();
                            Console.WriteLine(dir.ToString() + " " + NotSuppEx.Message);
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            Console.WriteLine(dir.ToString() + " " + ex.Message);
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            Console.WriteLine(ex.Message+" in YFY_Logic");
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine(ex.Message + " in YFY_Logic");
                        }
                        #endregion
                    }
                }
                else
                { 
                    //Console.WriteLine("Directory does not exist"); 
                }
            }
        }

        public static DataTable GetUserList(StreamReader reader)
        {
            DataTable table = new DataTable();
            table.Columns.Add("old_name", typeof(string));
            table.Columns.Add("new_name", typeof(string));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                table.Rows.Add(values[0], values[1]);
            }
            return table;
        }

        static void Main(string[] args)
        {
            #region Log Setting
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.log4net.Log4NetLog>();
            string CurrentPath = Directory.GetCurrentDirectory();
            Console.WriteLine(CurrentPath.ToString());
            string logfilename = @"\AuthTransferErrorLog";
            string logfilename_backup = @"\Backup_TransferErrorLog";
            log4net.GlobalContext.Properties["LogFileName"] = CurrentPath + logfilename_backup; //log file path
            log4net.GlobalContext.Properties["LogFileNamePrimary"] = CurrentPath + logfilename; //log file path
            log4net.Config.XmlConfigurator.Configure();
            #endregion

            #region Server Files Setting 
            string dirPath = ConfigurationManager.AppSettings["target_root_dir"];
            #endregion

            #region User Mapping Setting
            string csv_file = ConfigurationManager.AppSettings["user_mapping"];
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string combined_csv_file = Path.Combine(path, csv_file);
            #endregion

            //Hashtable MappingHT = getMapping(combined_csv_file);

            #region Retrieve All Server Directory
            //List<string> RootList = StringSeparator(dirPath); // RootList: C://; D://....
            //List<string> total_dirs = new List<string>();
            //foreach (string root in RootList)
            //{
            //    total_dirs.Add(root);// RootList
            //}
            //total_dirs.AddRange(GetAccessControlInfo(RootList)); // RootList Subdirectories
            #endregion

            #region Print Directory List 
            //Console.WriteLine("Print Directories: ");
            //foreach (string i in total_dirs)
            //{
            //    DirectoryInfo di = new DirectoryInfo(i);
            //    Console.WriteLine(di);
            //}
            #endregion

            #region Active Directory Setting
            string new_group_ip = ConfigurationManager.AppSettings["new_group_ip"];
            string new_group_dc = ConfigurationManager.AppSettings["new_group_dc"];
            string new_group_username = ConfigurationManager.AppSettings["new_group_username"];
            string new_group_password = ConfigurationManager.AppSettings["new_group_password"];
            string new_group_filter = ConfigurationManager.AppSettings["new_group_filter"];

            string new_user_ip = ConfigurationManager.AppSettings["new_user_ip"];
            string new_user_dc = ConfigurationManager.AppSettings["new_user_dc"];
            string new_user_username = ConfigurationManager.AppSettings["new_user_username"];
            string new_user_password = ConfigurationManager.AppSettings["new_user_password"];
            string new_user_filter = ConfigurationManager.AppSettings["new_user_filter"];
            #endregion

            #region Get LDAP old and new User Lists
            Console.WriteLine("Start Get AD users: ");
            AD_scanning user_LDAPcollection = new AD_scanning();
            List<string> user_list = user_LDAPcollection.GetADProperty(user_LDAPcollection.GetLdap(new_user_ip, new_user_dc, new_user_username, new_user_password, new_user_filter));
            //AD_scanning group_LDAPcollection = new AD_scanning();
            //List<string> group_list = group_LDAPcollection.GetADProperty(group_LDAPcollection.GetLdap(new_group_ip, new_group_dc, new_group_username, new_group_password, new_group_filter));
            Console.WriteLine("End Getting AD users: ");
            #endregion

            //YFY_Logic(user_list, group_list, total_dirs, MappingHT);
            Console.WriteLine("END");
            Console.ReadLine();
        }
    }
}