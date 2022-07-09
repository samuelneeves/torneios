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
    internal class TransferenciaAdapter
    {
        private const string connectionString = "data source=TorneioDB;initial catalog = Transferencia; integrated security = True; MultipleActiveResultSets=True;";

        public List<Transferencia> GetTransferencias()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Transferencia";
                return connection.Query<Transferencia>(sqlCommand).AsList();
            }
        }

        public Transferencia GetTransferenciaById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Transferencia WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                return connection.QueryFirstOrDefault<Transferencia>(sqlCommand, parameters);
            }
        }

        public int InsertTransferencia(int? timeOrigemId, int? timeDestinoId, DateTime data, double valor, int jogadorId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select MAX(Id) from Transferencia";
                int? maxId = connection.Query<int?>(sqlCommand).FirstOrDefault();
                int newId = 1;

                if (maxId.HasValue)
                {
                    newId = maxId.Value + 1;
                }

                sqlCommand = "Insert into Transferencia values (@Id, @TimeOrigemId, @TimeDestinoId, @Data, @Valor, @JogadorId)";

                var parameters = new DynamicParameters();
                parameters.Add("Id", newId, DbType.Int32);
                parameters.Add("TimeOrigemId", timeOrigemId, DbType.Int32);
                parameters.Add("TimeDestinoId", timeDestinoId, DbType.Int32);
                parameters.Add("Data", data, DbType.DateTime);
                parameters.Add("Valor", valor, DbType.Double);
                parameters.Add("JogadorId", jogadorId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int UpdateTransferencia(int id, int? timeOrigemId, int? timeDestinoId, DateTime data, double valor, int jogadorId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Transferencia WHERE Id = {0}", id);
                Transferencia transferencia = connection.QueryFirstOrDefault<Transferencia>(sqlCommand);

                if (transferencia == null)
                    return InsertTransferencia(timeOrigemId, timeDestinoId, data, valor, jogadorId);

                sqlCommand = @"Update Transferencia SET TimeOrigemId = @TimeOrigemId, TimeDestinoId = @TimeDestinoId, 
                             Data = @Data, Valor = @Valor, JogadorId = @JogadorId 
                            WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("TimeOrigemId", timeOrigemId, DbType.Int32);
                parameters.Add("TimeDestinoId", timeDestinoId, DbType.Int32);
                parameters.Add("Data", data, DbType.DateTime);
                parameters.Add("Valor", valor, DbType.Double);
                parameters.Add("JogadorId", jogadorId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int DeleteTransferencia(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Transferencia WHERE Id = {0}", id);
                Transferencia transferencia = connection.QueryFirstOrDefault<Transferencia>(sqlCommand);

                if (transferencia == null)
                    return 0;

                sqlCommand = "DELETE Transferencia WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", transferencia.Id, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}