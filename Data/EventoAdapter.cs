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
    public class EventoAdapter
    {
        private const string connectionString = "data source=TorneioDB;initial catalog = Evento; integrated security = True; MultipleActiveResultSets=True;";

        public List<Evento> GetEventos()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Evento";
                return connection.Query<Evento>(sqlCommand).AsList();
            }
        }

        public Evento GetEventoById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select * from Evento WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                return connection.QueryFirstOrDefault<Evento>(sqlCommand, parameters);
            }
        }

        public int InsertEvento(string tipo, string valor, DateTime dataHora, int? timeId, int? jogadorId, int? partidaId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = "Select MAX(Id) from Evento";
                int? maxId = connection.Query<int?>(sqlCommand).FirstOrDefault();
                int newId = 1;

                if (maxId.HasValue)
                {
                    newId = maxId.Value + 1;
                }

                sqlCommand = "Insert into Evento values (@Id, @Tipo, @Valor, @DataHora, @TimeId, @JogadorId, @PartidaId)";

                var parameters = new DynamicParameters();
                parameters.Add("Id", newId, DbType.Int32);
                parameters.Add("Tipo", tipo, DbType.String);
                parameters.Add("Valor", valor, DbType.String);
                parameters.Add("DataHora", dataHora, DbType.DateTime);
                parameters.Add("TimeId", timeId, DbType.Int32);
                parameters.Add("JogadorId", jogadorId, DbType.Int32);
                parameters.Add("PartidaId", partidaId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int UpdateEvento(int id, string tipo, string valor, DateTime dataHora, int? timeId, int? jogadorId, int? partidaId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Evento WHERE Id = {0}", id);
                Evento evento = connection.QueryFirstOrDefault<Evento>(sqlCommand);

                if (evento == null)
                    return InsertEvento(tipo, valor, dataHora, timeId, jogadorId, partidaId);

                sqlCommand = "Update Evento SET Nome = @Nome WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", id, DbType.Int32);
                parameters.Add("Tipo", tipo, DbType.String);
                parameters.Add("Valor", valor, DbType.String);
                parameters.Add("DataHora", dataHora, DbType.DateTime);
                parameters.Add("TimeId", timeId, DbType.Int32);
                parameters.Add("JogadorId", jogadorId, DbType.Int32);
                parameters.Add("PartidaId", partidaId, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }

        public int DeleteEvento(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = string.Format("Select * from Evento WHERE Id = {0}", id);
                Evento evento = connection.QueryFirstOrDefault<Evento>(sqlCommand);

                if (evento == null)
                    return 0;

                sqlCommand = "DELETE Evento WHERE Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", evento.Id, DbType.Int32);

                return connection.Execute(sqlCommand, parameters);
            }
        }
    }
}