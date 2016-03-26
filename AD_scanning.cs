using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Data;

namespace ConsoleApplication_test_20151202
{
    class AD_scanning
    {
        public DirectorySearcher GetLdap(string ip, string dc, string username, string password, string filter)
        {
            StringBuilder LDAP_IPDC = new StringBuilder();
            LDAP_IPDC.Append("LDAP://");
            LDAP_IPDC.Append(ip);
            LDAP_IPDC.Append("/");
            LDAP_IPDC.Append(dc);
            return new DirectorySearcher(new DirectoryEntry(LDAP_IPDC.ToString(), username, password,AuthenticationTypes.Secure), filter);
        }
        public List<string> GetADProperty(DirectorySearcher ds)
        {
            ds.PageSize = 1000;
            ds.SizeLimit = 10000;
            List<string> userlist = new List<string>();
            try
            {
                string property = "sAMAccountName";
                
                SearchResultCollection resultCollection = ds.FindAll();
                foreach (SearchResult Search_Result in resultCollection)
                {
                    if (Search_Result.GetDirectoryEntry().Properties[property].Value == null)
                    {
                        continue;
                    }
                    else
                    {
                        userlist.Add(Search_Result.GetDirectoryEntry().Properties[property].Value.ToString().ToLower());
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
                {Console.WriteLine(ex.Message);}
            catch (NullReferenceException ex)
                {Console.WriteLine(ex.Message);}
            catch (ArgumentException ex)
                {Console.WriteLine(ex.Message);}
            return userlist;
        }
    }
}
