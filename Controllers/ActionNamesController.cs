using SchoolErpAPI.BAL;
using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SchoolErpAPI.Controllers
{
    public class ActionNamesController : ApiController
    {
        #region getActionNamesList

        [HttpPost]
        public HttpResponseMessage getActionNamesList(ActionNamesFilter dataString)
        {
            try
            {
                List<ActionNames> actionNameList = new List<ActionNames>();

                BALActionNames func = new BALActionNames();
                actionNameList = func.getActionNamesList(dataString);

                return Return.returnHttp("200", actionNameList);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region getControllerNamesList

        [HttpPost]
        public HttpResponseMessage getControllerNamesList()
        {
            try
            {
                List<ActionNames> actionNamesList = new List<ActionNames>();

                BALActionNames func = new BALActionNames();
                actionNamesList = func.getControllerNamesList();

                return Return.returnHttp("200", actionNamesList);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region getActionNamesWithPermissionList

        [HttpPost]
        public HttpResponseMessage getActionNamesWithPermissionList()
        {
            try
            {
                List<ActionNames> actionNamesRowsList = new List<ActionNames>();

                BALActionNames func = new BALActionNames();
                actionNamesRowsList = func.getActionNamesWithPermissionList();

                return Return.returnHttp("200", actionNamesRowsList);

            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region updateOpenActionNames

        [HttpPost]
        public HttpResponseMessage updateOpenActionNames(ActionNames dataString)
        {
            try
            {
                if (String.IsNullOrEmpty(dataString.id.ToString()))
                {
                    return Return.returnHttp("201", "Invalid details. Please try again.");
                }

                if (!dataString.openAccess.HasValue)
                {
                    return Return.returnHttp("201", "Please Enter Access details, It's a mandatory.");
                }

                BALActionNames func = new BALActionNames();
                SPResponse response = func.updateOpenActionNames(dataString);

                if (response.executionStatus != "TRUE")
                {
                    return Return.returnHttp("201", response.message);
                }

                return Return.returnHttp("200", response.message);

            }
            catch (Exception e)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + e.Message + e.StackTrace);
            }
        }

        #endregion

        #region updateActionNamesPermissionList

        [HttpPost]
        public HttpResponseMessage updateActionNamesPermissionList(UpdatePermission dataString)
        {
            try
            {
                if (dataString.actionNamesList != null)
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("action_name_id", typeof(int));
                    dataTable.Columns.Add("role_id", typeof(int));
                    dataTable.Columns.Add("permission_id", typeof(int));
                    dataTable.Columns.Add("permission", typeof(bool));

                    foreach (var actionNames in dataString.actionNamesList)
                    {
                        if (actionNames.id.HasValue)
                        {
                            foreach (var permission in actionNames.permissions)
                            {
                                if (permission.roleId.HasValue)
                                {
                                    dataTable.Rows.Add(actionNames.id, permission.roleId, permission.id, permission.permission);
                                }
                            }
                        }
                    }

                    SqlConnection con = DBConnection.GlobalConnection();
                    SqlCommand cmd = new SqlCommand("updateActionNamesPermissionList", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime datetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), INDIAN_ZONE);

                    cmd.Parameters.AddWithValue("@userId", dataString.userId);
                    cmd.Parameters.AddWithValue("@roleTypeId", dataString.roleTypeId);
                    cmd.Parameters.AddWithValue("@creationTimestamp", datetime.ToString("MM/dd/yyyy HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@temp", dataTable);

                    cmd.Parameters.Add("@outputId", SqlDbType.Int);
                    cmd.Parameters["@outputId"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@message", SqlDbType.NVarChar, 500);
                    cmd.Parameters["@message"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@executionStatus", SqlDbType.NVarChar, 500);
                    cmd.Parameters["@executionStatus"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery().ToString();
                    con.Close();

                    return Return.returnHttp("200", "Record Updated Successfully");
                }
                else
                {
                    return Return.returnHttp("200", "Record Not Found");
                }
            }
            catch (Exception ex)
            {
                return Return.returnHttp("201", "Some Internal Issue Occured. Please try again." + ex.Message + ex.StackTrace);
            }
        }

        #endregion
    }
}