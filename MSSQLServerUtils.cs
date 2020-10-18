using System.Data.SqlClient;

namespace Zadanie
{
    class MSSQLServerUtils
    {
        public static SqlConnection
         GetDBConnection()
        {
            //
            // Data Source=TRAN-VMWARE\SQLEXPRESS;Initial Catalog=simplehr;Persist Security Info=True;User ID=sa;Password=12345
            //
            string connString = @"Data Source=.\SQLEXPRESS; Initial Catalog = country; integrated Security=True; Connect Timeout = 30";

            /*string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;*/

            SqlConnection conn = new SqlConnection(connString);
            return conn;
        }
    }
}
