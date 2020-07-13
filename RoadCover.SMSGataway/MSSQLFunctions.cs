using System.Data;
using System.Data.SqlClient;

namespace RoadCover.SMSGataway
{
    public class MSSQLFunctions
    {
        const string CONNSTRING = "Data Source=192.168.50.31;Initial Catalog=Rpt_Campaign_Campus;User ID=stephen;Password=passw0rd";

        public bool UpdateSMSLeads()
        {
            bool bReturn = false;


            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spUpdateSMSLeads";
                SqlCommand command = new SqlCommand(sqlsp, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
            return bReturn;
        }

        public DataTable GetSMSLeads()
        {

            DataTable dtcl;
            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spGetLeadsSMS";


                SqlDataAdapter da = new SqlDataAdapter(sqlsp, connection);
                connection.Open();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.CommandTimeout = 60000;

                DataSet ds = new DataSet();
                da.Fill(ds, "GetSMSLeads");
                dtcl = ds.Tables["GetSMSLeads"];

            }
            return dtcl;
        }

        public DataTable GetSMSLeadsToSend()
        {

            DataTable dtcl;
            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spGetLeadsSMSTOSend";


                SqlDataAdapter da = new SqlDataAdapter(sqlsp, connection);
                connection.Open();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.CommandTimeout = 60000;

                DataSet ds = new DataSet();
                da.Fill(ds, "GetSMSLeads");
                dtcl = ds.Tables["GetSMSLeads"];

            }
            return dtcl;
        }

        public DataTable GetSMSLeadsOverThreshold()
        {

            DataTable dtcl;
            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spGetSMSLeadsOverThreshold";


                SqlDataAdapter da = new SqlDataAdapter(sqlsp, connection);
                connection.Open();

                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.CommandTimeout = 60000;

                DataSet ds = new DataSet();
                da.Fill(ds, "GetSMSLeads");
                dtcl = ds.Tables["GetSMSLeads"];

            }
            return dtcl;
        }

        public void UpdateSMSLeadStatus(string LeadID, string NewStatus)
        {
            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spUpdateSMSLeadStatus";
                SqlCommand command = new SqlCommand(sqlsp, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@LeadID", SqlDbType.Text).Value = LeadID;
                command.Parameters.Add("@Status", SqlDbType.Text).Value = NewStatus;
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateSMSLeadStatusRefID(string LeadID, string NewStatus, string RefID)
        {
            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spUpdateSMSLeadStatusRefID";
                SqlCommand command = new SqlCommand(sqlsp, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@LeadID", SqlDbType.Text).Value = LeadID;
                command.Parameters.Add("@Status", SqlDbType.Text).Value = NewStatus;
                command.Parameters.Add("@RefID", SqlDbType.Text).Value = RefID;
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetSMSLeadsByEngageRef(string EngageRef)
        {

            DataTable dtcl;
            using (SqlConnection connection = new SqlConnection(CONNSTRING))
            {
                string sqlsp = "spGetSMSLeadsByEngageRef";


                SqlDataAdapter da = new SqlDataAdapter(sqlsp, connection);
                connection.Open();

                da.SelectCommand.Parameters.Add("@EngageRef", SqlDbType.Text).Value = EngageRef;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.CommandTimeout = 60000;

                DataSet ds = new DataSet();
                da.Fill(ds, "GetSMSLeads");
                dtcl = ds.Tables["GetSMSLeads"];

            }
            return dtcl;
        }
    }
}
