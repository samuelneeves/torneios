using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using Domain;

namespace Data
{
    public class TimeAdapter
    {
        private const string connectionString = "server=.\\SQLEXPRESS;Integrated Security=SSPI; database=TorneioDB";

        public List<Time> GetTimes()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Time";
                return connection.Query<Time>(sqlCommand).AsList();
            }
        }

        public Time GetTimeById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Time WHERE Id = @Id"; 
                
                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                return connection.QueryFirstOrDefault<Time>(sqlCommand, parameters);
            }
        }

        public int InsertTime(string nome, out int newId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select MAX(Id) from Time";
                int? maxId = connection.Query<int?>(sqlCommand).FirstOrDefault();
                newId = 1;
                
                if (maxId.HasValue)
                {
                    newId = maxId.Value + 1;
                }

                sqlCommand = "Insert into Time values (@Id, @Nome)";

                var parameters = new DynamicParameters();
                parameters.Add("Id", newId, DbType.Int32);
                parameters.Add("Nome", nome, DbType.String);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int UpdateTime(int id, string nome)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Time WHERE Id = {0}", id);
                Time time = connection.QueryFirstOrDefault<Time>(sqlCommand);

                int newId;
                if (time == null)
                    return InsertTime(nome, out newId);

                sqlCommand = "Update Time SET Nome = @Nome WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", time.Id, DbType.Int32);
                parameters.Add("Nome", nome, DbType.String);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int DeleteTime(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Time WHERE Id = {0}", id);
                Time time = connection.QueryFirstOrDefault<Time>(sqlCommand);

                if (time == null)
                    return 0;

                sqlCommand = "DELETE Time WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", time.Id, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}
