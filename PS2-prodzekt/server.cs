using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;

namespace PS2_prodzekt
{
    public class ChatHub : Hub
    {

        private static List<object[]> getQuery(string message)
        {
            List<object[]> rows = new List<object[]>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "SERVER=ps2db195000.database.windows.net;DATABASE=mk195000;USER ID=michal7018;PASSWORD=Michal7011;";
                
                conn.Open();

                SqlCommand command = new SqlCommand(message, conn);
                
                if (conn.State == ConnectionState.Closed)
                    conn.Open();


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] temp = new object[reader.FieldCount];

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                temp[i] = reader[i];
                            }
                            rows.Add(temp);
                        }
                    }
            }
            return rows;
        }


        public override Task OnConnected()
        {
            var name = Context.ConnectionId;
            Debug.WriteLine(name.ToString() + "  connected");

            return base.OnConnected();
        }

        public void getTablesList(string message, string conID)
        {
            List<object[]> response = getQuery(message);
            Clients.Client(conID).getTables(response);
        }

        public void ReadSingleTable(string message, string conID)
        {
            List<object[]> response = getQuery(message);
            Clients.Client(conID).singleTableResponse(response);
        }

        
        public void sendQuery(string message, string conID)
        {
            List<object[]> response = getQuery(message);
            Clients.Client(conID).getTables(response);
        }

        public static void db_OnChange()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.refresh();
        }

        public void executeUserCommand(string userID, string query)
        {

            Debug.WriteLine(query);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "SERVER=ps2db195000.database.windows.net;DATABASE=mk195000;USER ID=michal7018;PASSWORD=Michal7011;";

                conn.Open();

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                if (query.Contains("select"))
                {
                    List<object[]> response = getQuery(query);

                    Debug.WriteLine(response);
                    Clients.Client(userID).userCommandResponse(response);
                }
                else {
                    SqlCommand command = new SqlCommand(query);
                    int result = -1;

                    result = command.ExecuteNonQuery();

                    Clients.Client(userID).clientMessage("Error!");
                }
            }
        }


        public void insertRow(string userID, string tableName, string param1, string param2 = null ) {

            bool refreshFlag = false;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "SERVER=ps2db195000.database.windows.net;DATABASE=mk195000;USER ID=michal7018;PASSWORD=Michal7011;";

                conn.Open();

                SqlCommand command = null;

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                switch (tableName)
                {
                    case "czass":
                        command = new SqlCommand("INSERT INTO czass (id, czass) VALUES (@0, @1)", conn);
                        command.Parameters.Add(new SqlParameter("0", param1));
                        command.Parameters.Add(new SqlParameter("0", param2));
                        break;

                    case "msg":
                        command = new SqlCommand("INSERT INTO msg (msg) VALUES (@0)", conn);
                        command.Parameters.Add(new SqlParameter("0", param1));

                        break;
                }

                int result = -1;

                try
                {
                    if (param1 == "")
                        throw new Exception();
                    result = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Clients.Client(userID).clientMessage("Error inserting  data into Database!");
                    refreshFlag = true;
                }

                if (result < 0) {
                    Clients.Client(userID).clientMessage("Error inserting data into Database!");
                    refreshFlag = true;
                }
            }

            if (!refreshFlag)
                db_OnChange();
        }

        public void deleteRow(string userID, string tableName, string param1=null)
        {
            bool refreshFlag = false;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "SERVER=ps2db195000.database.windows.net;DATABASE=mk195000;USER ID=michal7018;PASSWORD=Michal7011;";

                conn.Open();

                SqlCommand command = null;

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                switch (tableName)
                {
                    case "czass":
                        command = new SqlCommand("DELETE FROM czass WHERE id=(@0)", conn);
                        command.Parameters.Add(new SqlParameter("0", param1));
                        break;

                    case "msg":
                        command = new SqlCommand("DELETE FROM msg WHERE id=(@0)", conn);
                        command.Parameters.Add(new SqlParameter("0", param1));

                        break;
                }

                int result = -1;

                try
                {
                    if (param1 == "")
                        throw new Exception();
                    result = command.ExecuteNonQuery();

                }
                catch(Exception e)
                {
                    Clients.Client(userID).clientMessage("Error during deleting data into Database!");
                    refreshFlag = true;
                }

                if (result < 0)
                {
                    Clients.Client(userID).clientMessage("Error during deleting data into Database!");
                    refreshFlag = true;
                }
            }

            if (!refreshFlag)
                db_OnChange();
        }

        public void editRow(string userID, string tableName, string param1 = null, string param2 = null, string param3 = null)
        {
            bool refreshFlag = false;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = "SERVER=ps2db195000.database.windows.net;DATABASE=mk195000;USER ID=michal7018;PASSWORD=Michal7011;";

                conn.Open();

                SqlCommand command = null;

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                switch (tableName)
                {
                    case "czass":
                        command = new SqlCommand("UPDATE czass SET id=(@1), czas=(@2) WHERE id_czas=(@0)", conn);
                        command.Parameters.Add(new SqlParameter("0", param1));
                        command.Parameters.Add(new SqlParameter("1", param2));
                        break;

                    case "msg":
                        command = new SqlCommand("UPDATE msg SET msgg=(@1) WHERE id=(@0)", conn);
                        command.Parameters.Add(new SqlParameter("0", param1));
                        command.Parameters.Add(new SqlParameter("1", param2));

                        break;

                }

                int result = -1;

                try
                {
                    if (param1 == "" || param2 == "")
                        throw new Exception();
                    result = command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Clients.Client(userID).clientMessage("Error during updating data into Database!");
                    refreshFlag = true;
                    
                }

                if (result < 0)
                {
                    Clients.Client(userID).clientMessage("Error during updating data into Database!");
                    refreshFlag = true;
                }

            }

            if (!refreshFlag)
                db_OnChange();
        }
    }
}
