using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class Database : IDisposable
    {
        private SQLiteConnection _connection;

        public string Path { get; }

        public bool IsConnected { get; private set; } = false;

        public Database(string path)
        {
            this.Path = path;
            this.Connect();
        }

        public void Connect()
        {
            if (_connection != null)
            {
                throw new InvalidOperationException();
            }

            var sqlConfig = new SQLiteConnectionStringBuilder
            {
                Version = 3,
                DataSource = App.GetLocalFilePath(this.Path),
            };

            this._connection = new SQLiteConnection(sqlConfig.ToString());
            this._connection.Open();

            this.IsConnected = true;

            sqlConfig = null;
        }

        public void Disconnect()
        {
            this.IsConnected = false;
            _connection?.Dispose();
        }

        public SQLiteTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        public SQLiteCommand CreateCommand()
        {
            return _connection.CreateCommand();
        }

        public SQLiteCommand CreateCommand(string query)
        {
            var command = this._connection.CreateCommand();

            command.CommandText = query;

            return command;
        }

        public SQLiteDataReader ExecuteReader(string query)
        {
            using (var command = this.CreateCommand(query))
            {
                return command.ExecuteReader();
            }
        }

        public SQLiteDataReader ExecuteReader(string query, IDictionary<string, object> values)
        {
            var columns = values.Keys;

            using (var command = this.CreateCommand(query))
            {
                foreach (var kvp in values)
                {
                    command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }

                command.Prepare();

                return command.ExecuteReader();
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using (var command = this.CreateCommand(query))
            {
                command.ExecuteNonQuery();
            }
        }

        public void ExecuteNonQuery(string query, IDictionary<string, object> values)
        {
            using (var command = this.CreateCommand(query))
            {
                foreach (var kvp in values)
                {
                    command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }

                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<SQLiteDataReader> Select(string tableName)
        {
            using (var command = this.CreateCommand("SELECT * FROM " + tableName))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return reader;
                }
            }
        }

        public void Insert(string tableName, IDictionary<string, object> values)
        {
            var columns = values.Keys;

            var columnsText = string.Join(",", columns);
            var valuesText = string.Join(",", columns.Select(key => "@" + key));

            using (var command = CreateCommand())
            {
                command.CommandText = $"INSERT INTO {tableName} ({columnsText}) VALUES ({valuesText})";

                foreach (var kvp in values)
                {
                    command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }

                command.Prepare();

                command.ExecuteNonQuery();
            }
        }

        public IList<string> EnumerateTableNames()
        {
            var tableNames = new List<string>();

            using (var reader = ExecuteReader(QueryCollection.SelectTables))
            {
                while (reader.Read())
                {
                    tableNames.Add(reader.GetString(0));
                }
            }

            return tableNames;
        }

        public void Dispose()
        {
            this.Disconnect();
        }

        public static class TableNameCollection
        {
            public const string PorfileImageCache = "profile_image";
        }

        public static class QueryCollection
        {
            public static readonly string CreateProfileImageCacheTable = $"CREATE TABLE {TableNameCollection.PorfileImageCache} (service_id INT NOT NULL, host_url VARCHAR(100), user_id INT NOT NULL, filename VARCHAR(100) NOT NULL, image_data BLOB, PRIMARY KEY(service_id, host_url, user_id))";
            public static readonly string SelectProfileImageCacheData = $"SELECT filename,image_data FROM {TableNameCollection.PorfileImageCache} WHERE user_id=@user_id AND service_id=@service_id AND host_url=@host_url";
            public static readonly string InsertOrReplaceProfileImageCache = $"INSERT OR REPLACE INTO {TableNameCollection.PorfileImageCache} (user_id, service_id, host_url, filename, image_data) VALUES (@user_id, @service_id, @host_url, @filename, @image_data)";
            public static readonly string DeleteProfileImageCacheData = $"DELETE image_data FROM {TableNameCollection.PorfileImageCache} WHERE user_id=@user_id AND service_id=@service_id AND host_url=@host_url";

            public const string SelectTables = "SELECT name FROM sqlite_master WHERE type='table'";
        }
    }
}
