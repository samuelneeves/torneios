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
    public class TorneioAdapter
    {
        private const string connectionString = "data source=TorneioDB;initial catalog = Torneio; integrated security = True; MultipleActiveResultSets=True;";

        public List<Torneio> GetTorneios()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Torneio";
                return connection.Query<Torneio>(sqlCommand).AsList();
            }
        }

        public Torneio GetTorneioById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Torneio WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                return connection.QueryFirstOrDefault<Torneio>(sqlCommand, parameters);
            }
        }

        public int InsertTorneio(string nome)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select MAX(Id) from Torneio";
                int? maxId = connection.Query<int?>(sqlCommand).FirstOrDefault();
                int newId = 1;

                if (maxId.HasValue)
                {
                    newId = maxId.Value + 1;
                }

                sqlCommand = "Insert into Torneio values (@Id, @Nome)";

                var parameters = new DynamicParameters();
                parameters.Add("Id", newId, DbType.Int32);
                parameters.Add("Nome", nome, DbType.String);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int UpdateTorneio(int id, string nome)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Torneio WHERE Id = {0}", id);
                Torneio torneio = connection.QueryFirstOrDefault<Torneio>(sqlCommand);

                if (torneio == null)
                    return InsertTorneio(nome);

                sqlCommand = "Update Torneio SET Nome = @Nome WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", torneio.Id, DbType.Int32);
                parameters.Add("Nome", nome, DbType.String);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int DeleteTorneio(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Torneio WHERE Id = {0}", id);
                Torneio torneio = connection.QueryFirstOrDefault<Torneio>(sqlCommand);

                if (torneio == null)
                    return 0;

                sqlCommand = "DELETE Torneio WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", torneio.Id, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}