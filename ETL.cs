using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace ConsoleApplication_test_20151202
{
    class ETL
    {
        public class Read_File
        {
            public static StreamReader CSV_File(string file_name)
            {
                return new StreamReader(File.OpenRead(file_name));
            }
            public static StreamReader TXT_File(string file_name)
            {
                return new StreamReader(File.OpenRead(file_name));
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
    }
}
