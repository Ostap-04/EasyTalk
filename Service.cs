using Npgsql;

namespace dbSetup
{
    internal class Service
    {
        public static string connectionString;

        public static List<List<string>> GetData(string table_name)
        {
            List<List<string>> UsersList = new List<List<string>>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                string select = $"SELECT * FROM {table_name};";
                using (NpgsqlCommand command = new NpgsqlCommand(select, con))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            List<string> userData = new List<string>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                userData.Add(reader[i].ToString());
                            }

                            UsersList.Add(userData);
                        }
                    }
                }
            }
            return UsersList;
        }

        public static List<List<string>> SelectUserByType(string type)
        {
            List<List<string>> UsersList = new List<List<string>>();
            using (NpgsqlConnection con = new NpgsqlConnection(connectionString))
            {
                con.Open();
                string select = "SELECT * FROM PERSON WHERE person.type = @type";
                using (NpgsqlCommand command = new NpgsqlCommand(select, con))
                {
                    command.Parameters.AddWithValue("@type", type);
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            List<string> rowData = new List<string>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rowData.Add(reader[i].ToString());
                            }

                            UsersList.Add(rowData);
                        }
                    }
                }
            }
            return UsersList;
        }
    }
}
