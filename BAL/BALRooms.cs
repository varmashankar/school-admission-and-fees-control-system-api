using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SchoolErpAPI.BAL
{
    public class BALRooms
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALRooms()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveRoom
        public SPResponse saveRoom(Rooms data)
        {
            SqlCommand cmd = new SqlCommand("saveRoom", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function func = new Function();
            func.addClassAttributes<Rooms>(ref cmd, data);
            func.addDefaultSPOutput(ref cmd);

            SPResponse response = func.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region getRoomList
        public List<GetRoom> getRoomList(RoomFilter filter)
        {
            Adp = new SqlDataAdapter("getRoomList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<RoomFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetRoom> list = new List<GetRoom>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetRoom item = Function.BindData<GetRoom>(Dt.Rows[i], columns);
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion

        #region getRoomDetails
        public GetRoom getRoomDetails(RoomFilter filter)
        {
            try
            {
                var list = getRoomList(filter);
                if (list != null && list.Count > 0) return list[0];
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in room details: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region changeStatus
        public SPResponse changeStatus(Rooms data)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForRoom", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", data.status);
            cmd.Parameters.AddWithValue("@userId", (object)data.userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)data.roleTypeId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region deleteRoom
        public SPResponse deleteRoom(Rooms data)
        {
            SqlCommand cmd = new SqlCommand("deleteRoom", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id", (object)data.id ?? DBNull.Value);
            DateTime deletedTs;
            if (!DateTime.TryParse(data.deletedTimestamp, out deletedTs))
                deletedTs = DateTime.Now;

            cmd.Parameters.AddWithValue("@deletedTimestamp", deletedTs.ToString("MM/dd/yyyy HH:mm:ss"));
            cmd.Parameters.AddWithValue("@userId", (object)data.deletedById ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@roleTypeId", (object)data.roleTypeId ?? DBNull.Value);

            Function function = new Function();
            function.addDefaultSPOutput(ref cmd);

            SPResponse response = function.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion
    }
}