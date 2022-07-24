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
    public class TimeEmTorneioAdapter
    {
        private const string connectionString = "server=.\\SQLEXPRESS;Integrated Security=SSPI; database=TorneioDB";

        public List<TimeEmTorneio> GetTimesEmTorneios()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from TimeEmTorneio";
                return connection.Query<TimeEmTorneio>(sqlCommand).AsList();
            }
        }

        public List<TimeEmTorneio> GetTimesEmTorneioByTorneioId(int torneioId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from TimeEmTorneio WHERE TorneioId = @TorneioId";

                var parameters = new DynamicParameters();
                parameters.Add("TorneioId", torneioId, DbType.Int32);
                return connection.Query<TimeEmTorneio>(sqlCommand, parameters).AsList();
            }
        }
        public List<TimeEmTorneio> GetTimesEmTorneioByTimeId(int timeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from TimeEmTorneio WHERE TimeId = @TimeId";

                var parameters = new DynamicParameters();
                parameters.Add("TimeId", timeId, DbType.Int32);
                return connection.Query<TimeEmTorneio>(sqlCommand, parameters).AsList();
            }
        }
        public TimeEmTorneio GetTimesEmTorneioByTimeIdETorneioId(int timeId, int torneioId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from TimeEmTorneio WHERE TimeId = @TimeId AND TorneioId = @TorneioId";

                var parameters = new DynamicParameters();
                parameters.Add("TimeId", timeId, DbType.Int32);
                parameters.Add("TorneioId", torneioId, DbType.Int32);
                return connection.QueryFirstOrDefault<TimeEmTorneio>(sqlCommand, parameters);
            }
        }

        public int InsertTimeEmTorneio(int torneioId, int timeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Insert into TimeEmTorneio values (@TorneioId, @TimeId)";

                var parameters = new DynamicParameters();
                parameters.Add("TorneioId", torneioId, DbType.Int32);
                parameters.Add("TimeId", timeId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
        public int MudarTimeDeTorneio(int timeId, int torneioId, int? novoTorneioId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from TimeEmTorneio WHERE TimeId = {0} AND TorneioId = {1}", torneioId);
                TimeEmTorneio timeEmTorneio = connection.QueryFirstOrDefault<TimeEmTorneio>(sqlCommand);
                if (timeEmTorneio == null)
                    return InsertTimeEmTorneio(torneioId, timeId);

                sqlCommand = "Update TimeEmTorneio SET TorneioId = @NewTorneioId WHERE TimeId = @TimeId AND TorneioId = @TorneioId";

                var parameters = new DynamicParameters();
                parameters.Add("TorneioId", torneioId, DbType.Int32);
                parameters.Add("NewTorneioId", novoTorneioId, DbType.Int32);
                parameters.Add("TimeId", timeId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int RemoverTimeDeTorneio(int torneioId, int timeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from TimeEmTorneio WHERE TorneioId = {0} AND TimeId = {1}", torneioId, timeId);
                TimeEmTorneio timeEmTorneio = connection.QueryFirstOrDefault<TimeEmTorneio>(sqlCommand);

                if (timeEmTorneio == null)
                    return 0;

                sqlCommand = "DELETE TimeEmTorneio WHERE TorneioId = @TorneioId AND TimeId =@TimeId";

                var parameters = new DynamicParameters();
                parameters.Add("TorneioId", torneioId, DbType.Int32);
                parameters.Add("TimeId", timeId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}