using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;

namespace BidUp_App.Helpers
{
    internal class DatabaseHelper
    {

        private readonly string connectionString;
        public DatabaseHelper()
        {
            // Load connection string from App.config using the specified name
            connectionString = ConfigurationManager.ConnectionStrings["BidUp-App"].ConnectionString;
        }

        // Method to open a new connection
        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        // Method for executing non-query commands (INSERT, UPDATE, DELETE)
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                return command.ExecuteNonQuery(); // Returns the number of rows affected
            }
        }

        // Method for executing scalar commands (for single value results like COUNT, SUM, etc.)
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                return command.ExecuteScalar(); // Returns a single value
            }
        }

        // Method to execute SELECT queries and return a DataTable
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable); // Fills the DataTable with the result set
                    return dataTable;
                }
            }
        }

        // Optional: Method to hash passwords for secure storage
        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var byteValue in bytes)
                {
                    builder.Append(byteValue.ToString("x2"));
                }
                return builder.ToString();
            }
        }


        public DataRow GetUserByEmailAndPassword(string email, string passwordHash)
        {
            using (SqlConnection connection = GetConnection())
            {
                string query = "SELECT * FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable userTable = new DataTable();
                adapter.Fill(userTable);

                if (userTable.Rows.Count > 0)
                {
                    return userTable.Rows[0]; // Return the first row (assuming unique email)
                }
                else
                {
                    return null; // No user found
                }
            }
        }
    }
}

