using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace NAnyConnect_test1
{
    internal class Account
    {

        public int Id { get; private set; } = 0;
        public string DisplayName { get; private set; } = "Unnamed";
        public string Script { get; private set; } = "connect _enterVpnUrlHere_\n_enterYourUsernameHere_\n<password>";
        public int OrderValue { get; private set; } = 0;


        private Account() { 
        }

        public static System.Collections.ObjectModel.ReadOnlyCollection<Account> GetReadOnlyAccounts() {
            return Accounts.AsReadOnly();
        }


        private static List<Account>? _accounts = null;
        private static List<Account> Accounts
        {
            get
            {
                if (_accounts == null)
                {
                    _accounts = LoadAccounts();
                }
                return _accounts;
            }
            set
            {
                _accounts = value;
            }
        }


        public static void DeleteAccount(int id)
        {
            using (var connection = new SqliteConnection("Data Source=n_any_connect_database.db"))
            {
                connection.Open();
                SqliteCommand sqlite_cmd;
                string Createsql = "DELETE FROM vpn_accounts WHERE id = $id";
                sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.Parameters.AddWithValue("id", id);
                sqlite_cmd.ExecuteNonQuery();
            }
            Accounts.RemoveAll(a => a.Id.Equals(id));
        }




        public static int CreateNewAccount(string displayName, string script)
        {
            int newId = -1;
            using (var connection = new SqliteConnection("Data Source=n_any_connect_database.db"))
            {
                connection.Open();

                SqliteTransaction transaction = null;
                transaction = connection.BeginTransaction();

                SqliteCommand sqlite_cmd;
                string Createsql = "INSERT INTO vpn_accounts (display_name, script, order_value) VALUES ($display_name, $script, '0')";
                sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.Parameters.AddWithValue("$display_name", displayName);
                sqlite_cmd.Parameters.AddWithValue("$script", script);
                sqlite_cmd.ExecuteNonQuery();

                SqliteCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT last_insert_rowid()";
                var reader = cmd.ExecuteReader();
                reader.Read();
                newId = reader.GetInt32(0);

                transaction.Commit();
            }

            Account newAccount = new Account();
            newAccount.Id = newId;
            newAccount.DisplayName = displayName;
            newAccount.Script = script;
            newAccount.OrderValue = 0;
            Accounts.Add(newAccount);

            return newId;
        }

        public static void UpdateAccount(int id, string displayName, string script)
        {
            using (var connection = new SqliteConnection("Data Source=n_any_connect_database.db"))
            {
                connection.Open();

                SqliteCommand sqlite_cmd;
                string Createsql = "UPDATE vpn_accounts SET display_name = $display_name, script = $script WHERE id = $id";
                sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.Parameters.AddWithValue("$id", id);
                sqlite_cmd.Parameters.AddWithValue("$display_name", displayName);
                sqlite_cmd.Parameters.AddWithValue("$script", script);
                sqlite_cmd.ExecuteNonQuery();
            }

            Account changedAccount = Accounts.Where(a => a.Id == id).First();
            changedAccount.DisplayName = displayName;
            changedAccount.Script = script;
        }

        public static int GetPositionFromId(int id) {
            int slot = Accounts.FindIndex(a => a.Id == id);
            return (slot < 2 ? slot : -1);
        }







        private static void CreateTableIfNotExists()
        {
            using (var connection = new SqliteConnection("Data Source=n_any_connect_database.db"))
            {
                connection.Open();

                SqliteCommand sqlite_cmd;
                string Createsql = "CREATE TABLE IF NOT EXISTS vpn_accounts (id INTEGER PRIMARY KEY, display_name TEXT, script TEXT, order_value INT)";
                sqlite_cmd = connection.CreateCommand();
                sqlite_cmd.CommandText = Createsql;
                sqlite_cmd.ExecuteNonQuery();
            }
        }


        private static List<Account> LoadAccounts()
        {
            List<Account> loadedAccounts = new List<Account>();

            CreateTableIfNotExists();
            using (var connection = new SqliteConnection("Data Source=n_any_connect_database.db"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM vpn_accounts ORDER BY order_value, id LIMIT 2";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var displayName = reader.GetString(1);
                        var script = reader.GetString(2);
                        var order = reader.GetInt32(3);

                        Account newAccount = new Account
                        {
                            Id = id,
                            DisplayName = displayName,
                            Script = script,
                            OrderValue = order
                        };
                        loadedAccounts.Add(newAccount);
                    }
                }
            }

            return loadedAccounts;
        }






    }
}
