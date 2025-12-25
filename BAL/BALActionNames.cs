using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALActionNames
    {
        public SqlConnection con;  // Database connection object
        public SqlDataAdapter Adp; // Data adapter for executing SQL commands
        public DataTable Dt; // Data table to hold the results of SQL queries

        public BALActionNames() // Constructor to initialize the database connection
        {
            con = DBConnection.GlobalConnection(); // Get the global database connection
        }

        #region getActionNamesList

        public List<ActionNames> getActionNamesList(ActionNamesFilter dataString)
        {

            Adp = new SqlDataAdapter("getActionNamesList", con); // Initialize the data adapter with the stored procedure name
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure; // Set the command type to stored procedure

            Function function = new Function(); // Create an instance of the Function class to handle common operations
            function.addClassAttributes<ActionNamesFilter>(ref Adp, dataString); // Add parameters to the data adapter based on the ActionNamesFilter object

            Dt = new DataTable();  // Create a new data table to hold the results
            Adp.Fill(Dt);  // Fill the data table with the results of the stored procedure

            List<ActionNames> actionNamesList = new List<ActionNames>(); // Initialize a list to hold ActionNames objects

            if (Dt.Rows.Count > 0)
            {
                // Get all columns' name
                List<string> columns = new List<string>(); // Create a list to hold the column names
                foreach (DataColumn dc in Dt.Columns)
                {
                    columns.Add(dc.ColumnName); // Add each column name to the list
                }

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    ActionNames element = Function.BindData<ActionNames>(Dt.Rows[i], columns); // Bind each row of the data table to an ActionNames object using the column names
                    actionNamesList.Add(element); // Add the ActionNames object to the list
                }
            }

            return actionNamesList; // Return the list of ActionNames objects

        }

        #endregion

        #region getControllerNamesList

        public List<ActionNames> getControllerNamesList()
        {
            Adp = new SqlDataAdapter("getControllerNamesList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<ActionNames> actionNamesList = new List<ActionNames>();

            if (Dt.Rows.Count > 0)
            {
                // Get all columns' name
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                {
                    columns.Add(dc.ColumnName);
                }

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    ActionNames element = Function.BindData<ActionNames>(Dt.Rows[i], columns);
                    actionNamesList.Add(element);
                }
            }

            return actionNamesList;

        }

        #endregion

        #region getActionNamesWithPermissionList

        public List<ActionNames> getActionNamesWithPermissionList()
        {
            Adp = new SqlDataAdapter("getActionNamesWithPermissionList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<ActionNamesRow> actionNamesRowsList = new List<ActionNamesRow>();
            List<ActionNames> actionNames = new List<ActionNames>();
            if (Dt.Rows.Count > 0)
            {
                // Get all columns' name
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns)
                {
                    columns.Add(dc.ColumnName);
                }

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    ActionNamesRow element = Function.BindData<ActionNamesRow>(Dt.Rows[i], columns);
                    actionNamesRowsList.Add(element);
                }

                actionNames = actionNamesRowsList
                    .GroupBy(x => new { x.id, x.actionName, x.controllerName, x.openAccess })
                    .Select(y => new ActionNames()
                    {
                        id = y.Key.id,
                        actionName = y.Key.actionName,
                        controllerName = y.Key.controllerName,
                        openAccess = y.Key.openAccess,
                        permissions = y.GroupBy(x => new { x.roleId, x.roleType, x.permissionId, x.permission })
                        .Select(z => new Permission()
                        {
                            id = z.Key.permissionId,
                            roleId = z.Key.roleId,
                            role = z.Key.roleType,
                            permission = z.Key.permission
                        }).ToList()
                    }).ToList();
            }

            return actionNames;

        }

        #endregion  

        #region updateOpenActionNames

        public SPResponse updateOpenActionNames(ActionNames dataString)
        {

            SqlCommand cmd = new SqlCommand("updateOpenActionNames", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@userId", dataString.id);
            cmd.Parameters.AddWithValue("@openAccess", dataString.openAccess);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);

            return response;

        }

        #endregion        
    }
}