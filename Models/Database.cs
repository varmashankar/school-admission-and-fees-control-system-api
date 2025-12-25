using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SchoolErpAPI.Models
{
    public class Database
    {
        #region computeMaxID
        public static int computeMaxID(string table)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["local_demo_db"].ConnectionString);
            SqlCommand cmd;
            SqlDataReader dr;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                int max_id = 0;
                string query = "select max(id) from " + table + "";
                cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 180;
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    max_id = Int32.Parse(dr[0].ToString());
                }
                con.Close();

                return max_id;
            }
            catch (Exception e)
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                return 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        #endregion

        #region insertData
        public static string insertData(string sql)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["local_demo_db"].ConnectionString);
            SqlCommand cmd;
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
                con.Open();
                cmd = new SqlCommand(sql, con);
                cmd.CommandTimeout = 180;
                cmd.ExecuteNonQuery();
                con.Close();

                return "200";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                if (con.State == ConnectionState.Open)
                    con.Close();

                return e.Data + "..." + e.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        #endregion

        #region readData

        // This method reads data from the database based on the provided query and returns a list of strings.
        public static List<string> readData(string query)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["local_demo_db"].ConnectionString);
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                List<string> data = new List<string>(); // Initialize a list to hold the data

                con.Open();

                cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 180; // Set command timeout to 180 seconds
                dr = cmd.ExecuteReader();
                while (dr.Read())// Read each row from the SqlDataReader and add the first column's value to the list
                {
                    data.Add(dr[0].ToString()); // Convert the value to string and add it to the list
                }
                con.Close();

                return data;
            }
            catch (Exception e)
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        #endregion

        #region getDataSet

        // This method executes a SQL query and returns the result as a DataSet.
        public static DataSet getDataSet(string query)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["local_demo_db"].ConnectionString);
            SqlCommand cmd;

            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();

                cmd = new SqlCommand(query, con);
                cmd.CommandTimeout = 180;

                // Create a DataSet to hold the results
                DataSet ds = new DataSet();
                Console.WriteLine(query); // Log the query for debugging purposes

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(ds);
                con.Close();

                return ds;
            }
            catch (Exception e)
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        #endregion

        #region getValue

        // This method executes a SQL query and returns the first value of the first column as a string.
        public static string getValue(string query)
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["local_demo_db"].ConnectionString);
            SqlCommand cmd;

            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                con.Open();
                cmd = new SqlCommand(query, con);

                string value = cmd.ExecuteScalar().ToString();

                con.Close();

                return value;
            }
            catch (Exception e)
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        #endregion
    }
}