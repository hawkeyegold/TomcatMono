using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace TomcatMono.Persist {
	public static class KeyValueStore {
		private static readonly string _dbPath;

		static KeyValueStore() {
			string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string dir = Path.Combine(root, "TomcatMono");
			Directory.CreateDirectory(dir);

			_dbPath = Path.Combine(dir, "persist.db");

			EnsureSchema();
		}

		private static void EnsureSchema() {
			using var conn = new SqliteConnection($"Data Source={_dbPath}");
			conn.Open();

			string sql = @"
                CREATE TABLE IF NOT EXISTS KeyValue (
                    Key TEXT PRIMARY KEY,
                    Value TEXT NOT NULL
                );
            ";

			using var cmd = new SqliteCommand(sql, conn);
			cmd.ExecuteNonQuery();
		}

		public static void Set(string key, string value) {
			using var conn = new SqliteConnection($"Data Source={_dbPath}");
			conn.Open();

			string sql = @"
                INSERT INTO KeyValue (Key, Value)
                VALUES ($key, $value)
                ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;
            ";

			using var cmd = new SqliteCommand(sql, conn);
			cmd.Parameters.AddWithValue("$key", key);
			cmd.Parameters.AddWithValue("$value", value);
			cmd.ExecuteNonQuery();
		}

		public static string? Get(string key) {
			using var conn = new SqliteConnection($"Data Source={_dbPath}");
			conn.Open();

			string sql = "SELECT Value FROM KeyValue WHERE Key = $key LIMIT 1";

			using var cmd = new SqliteCommand(sql, conn);
			cmd.Parameters.AddWithValue("$key", key);

			object? result = cmd.ExecuteScalar();
			return result?.ToString();
		}
	}
}
