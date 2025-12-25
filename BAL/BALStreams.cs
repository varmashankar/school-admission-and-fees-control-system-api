using SchoolErpAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SchoolErpAPI.BAL
{
    public class BALStreams
    {
        public SqlConnection con;
        public SqlDataAdapter Adp;
        public DataTable Dt;

        public BALStreams()
        {
            con = DBConnection.GlobalConnection();
        }

        #region saveStream
        public SPResponse saveStream(Streams data)
        {
            SqlCommand cmd = new SqlCommand("saveStream", con);
            cmd.CommandType = CommandType.StoredProcedure;

            Function func = new Function();
            func.addClassAttributes<Streams>(ref cmd, data);
            func.addDefaultSPOutput(ref cmd);

            SPResponse response = func.getDefaultSPOutput(cmd, con);
            return response;
        }
        #endregion

        #region getStreamList
        public List<GetStream> getStreamList(StreamFilter filter)
        {
            Adp = new SqlDataAdapter("getStreamList", con);
            Adp.SelectCommand.CommandType = CommandType.StoredProcedure;

            Function function = new Function();
            function.addClassAttributes<StreamFilter>(ref Adp, filter);

            Dt = new DataTable();
            Adp.Fill(Dt);

            List<GetStream> list = new List<GetStream>();
            if (Dt.Rows.Count > 0)
            {
                List<string> columns = new List<string>();
                foreach (DataColumn dc in Dt.Columns) columns.Add(dc.ColumnName);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    GetStream item = Function.BindData<GetStream>(Dt.Rows[i], columns);
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion

        #region getStreamDetails
        public GetStream getStreamDetails(StreamFilter filter)
        {
            try
            {
                var list = getStreamList(filter);
                if (list != null && list.Count > 0) return list[0];
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in stream details: " + ex.Message);
                return null;
            }
        }
        #endregion

        #region changeStatus
        public SPResponse changeStatus(Streams data)
        {
            SqlCommand cmd = new SqlCommand("changeStatusForStream", con);
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

        #region deleteStream
        public SPResponse deleteStream(Streams data)
        {
            SqlCommand cmd = new SqlCommand("deleteStream", con);
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