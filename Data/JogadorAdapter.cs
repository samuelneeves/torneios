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
    public class JogadorAdapter
    {
        private const string connectionString = "data source=TorneioDB;initial catalog = Jogador; integrated security = True; MultipleActiveResultSets=True;";

        public List<Jogador> GetJogadors()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Jogador";
                return connection.Query<Jogador>(sqlCommand).AsList();
            }
        }

        public Jogador GetJogadorById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Jogador WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                return connection.QueryFirstOrDefault<Jogador>(sqlCommand, parameters);
            }
        }

        public int InsertJogador(string nome, DateTime dataNascimento, string pais, int? timeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select MAX(Id) from Jogador";
                int? maxId = connection.Query<int?>(sqlCommand).FirstOrDefault();
                int newId = 1;

                if (maxId.HasValue)
                {
                    newId = maxId.Value + 1;
                }

                sqlCommand = "Insert into Jogador values (@Id, @Nome, @DataNascimento, @Pais, @TimeId)";

                var parameters = new DynamicParameters();
                parameters.Add("Id", newId, DbType.Int32);
                parameters.Add("Nome", nome, DbType.String);
                parameters.Add("DataNascimento", nome, DbType.DateTime);
                parameters.Add("Pais", nome, DbType.String);
                parameters.Add("TimeId", timeId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int UpdateJogador(int id, string nome, DateTime dataNascimento, string pais, int? timeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Jogador WHERE Id = {0}", id);
                Jogador jogador = connection.QueryFirstOrDefault<Jogador>(sqlCommand);

                if (jogador == null)
                    return InsertJogador(nome, dataNascimento, pais, timeId);

                sqlCommand = @"Update Jogador SET Nome = @Nome, DataNascimento = @DataNascimento, 
                                Pais = @Pais, TimeId = @TimeId 
                              WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("Nome", nome, DbType.String);
                parameters.Add("DataNascimento", nome, DbType.DateTime);
                parameters.Add("Pais", nome, DbType.String);
                parameters.Add("TimeId", timeId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int DeleteJogador(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Jogador WHERE Id = {0}", id);
                Jogador jogador = connection.QueryFirstOrDefault<Jogador>(sqlCommand);

                if (jogador == null)
                    return 0;

                sqlCommand = "DELETE Jogador WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", jogador.Id, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}