﻿using Dapper;
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
    public class PartidaAdapter
    {
        private const string connectionString = "data source=TorneioDB;initial catalog = Partida; integrated security = True; MultipleActiveResultSets=True;";

        public List<Partida> GetPartidas()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Partida";
                return connection.Query<Partida>(sqlCommand).AsList();
            }
        }

        public Partida GetPartidaById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Partida WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                return connection.QueryFirstOrDefault<Partida>(sqlCommand, parameters);
            }
        }

        public int InsertPartida(int time1Id, int time2Id, int? golsTime1, int? golsTime2, int? torneioId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select MAX(Id) from Partida";
                int? maxId = connection.Query<int?>(sqlCommand).FirstOrDefault();
                int newId = 1;

                if (maxId.HasValue)
                {
                    newId = maxId.Value + 1;
                }

                sqlCommand = "Insert into Partida values (@Id, @Nome)";

                var parameters = new DynamicParameters();
                parameters.Add("Id", newId, DbType.Int32);
                parameters.Add("Time1Id", time1Id, DbType.Int32);
                parameters.Add("Time2Id", time2Id, DbType.Int32);
                parameters.Add("GolsTime1", golsTime1, DbType.Int32);
                parameters.Add("GolsTime2", golsTime2, DbType.Int32);
                parameters.Add("TorneioId", torneioId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int UpdatePartida(int id, int time1Id, int time2Id, int? golsTime1, int? golsTime2, int? torneioId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Partida WHERE Id = {0}", id);
                Partida partida = connection.QueryFirstOrDefault<Partida>(sqlCommand);

                if (partida == null)
                    return InsertPartida(time1Id, time2Id, golsTime1, golsTime2, torneioId);

                sqlCommand = "Update Partida SET Nome = @Nome WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", partida.Id, DbType.Int32);
                parameters.Add("Time1Id", time1Id, DbType.Int32);
                parameters.Add("Time2Id", time2Id, DbType.Int32);
                parameters.Add("GolsTime1", golsTime1, DbType.Int32);
                parameters.Add("GolsTime2", golsTime2, DbType.Int32);
                parameters.Add("TorneioId", torneioId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int DeletePartida(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Partida WHERE Id = {0}", id);
                Partida partida = connection.QueryFirstOrDefault<Partida>(sqlCommand);

                if (partida == null)
                    return 0;

                sqlCommand = "DELETE Partida WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", partida.Id, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}