using Npgsql;
using System.Text;

namespace dbSetup
{
    public class DataGenerator
    {
        private static readonly Random random = new Random();
        static int recordsNum = 40;
        private static string[] emailDomains = { "gmail.com", "yahoo.com", "outlook.com", "example.com" };

        public static void InsertRandomData(string connection)
        {
            List<int> IndustryIdsList = new List<int>();
            List<Guid> PersonIdsList = new List<Guid>();
            List<Guid> ChatIdsList = new List<Guid>();
            List<Guid> ProjectIdsList = new List<Guid>();
            List<Guid> TopicIdsList = new List<Guid>();
            List<Guid> DocumentIdsList = new List<Guid>();



            using (NpgsqlConnection con = new NpgsqlConnection(connection))
            {
                con.Open();
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;
                    //Insert random data into industry table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO industry(industry_name) VALUES('{GenerateRandomString(7)}')";
                        cmd.ExecuteNonQuery();
                    }

                    string sql = "SELECT id FROM industry;";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("id"));
                                IndustryIdsList.Add(id);
                            }
                        }
                    }

                    
                    // Insert random data into person table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        string firstName = GenerateRandomString(6);
                        string lastName = GenerateRandomString(10);
                        string email = $"{firstName.ToLower()}.{lastName.ToLower()}@{emailDomains[random.Next(emailDomains.Length)]}";
                        string type = random.Next(10) == 0 ? "Admin" : "User";
                        string password = GenerateRandomString(10); // Set a default password for example purposes

                        cmd.CommandText = $"INSERT INTO person(industry_id, firstname, surname, email, type, password) VALUES({IndustryIdsList[random.Next(0, IndustryIdsList.Count)]}, '{firstName}', '{lastName}', '{email}', '{type}', '{password}')";
                        cmd.ExecuteNonQuery();
                    }

                    // Insert random data into chat table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO chat(name, created_at, updated_at, description) VALUES('{GenerateRandomString(6)}', now(), now(), 'Chat room for {GenerateRandomString(15)}')";
                        cmd.ExecuteNonQuery();
                    }

                    sql = "SELECT id FROM person;";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid id = reader.GetGuid(reader.GetOrdinal("id"));
                                PersonIdsList.Add(id);
                            }
                        }
                    }

                    sql = "SELECT id FROM chat;";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid id = reader.GetGuid(reader.GetOrdinal("id"));
                                ChatIdsList.Add(id);
                            }
                        }
                    }

                    // Insert random data into document table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO document(filename, file_path) VALUES('file{i}.txt', '/path/to/file{i}.txt')";
                        cmd.ExecuteNonQuery();
                    }

                    sql = "SELECT id FROM document;";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid id = reader.GetGuid(reader.GetOrdinal("id"));
                                DocumentIdsList.Add(id);
                            }
                        }
                    }


                    // Insert random data into message table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO message(created_at, updated_at, text, from_id, to_id, chat_id, attachment_id) VALUES(now(), now(), '{GenerateRandomString(20)}', '{PersonIdsList[random.Next(0, PersonIdsList.Count)]}', '{PersonIdsList[random.Next(0, PersonIdsList.Count)]}', '{ChatIdsList[random.Next(0, ChatIdsList.Count)]}', '{DocumentIdsList[random.Next(0, DocumentIdsList.Count)]}')";
                        cmd.ExecuteNonQuery();
                    }

                    // Insert random data into person_chat table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO person_chat(person_id, chat_id) VALUES('{PersonIdsList[random.Next(0, PersonIdsList.Count)]}', '{ChatIdsList[random.Next(0, ChatIdsList.Count)]}')";
                        cmd.ExecuteNonQuery();
                    }

                    // Insert random data into project table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO project(name, description, client_id, dev_id) VALUES('{GenerateRandomString(8)}', 'Description for {GenerateRandomString(15)}', '{PersonIdsList[random.Next(0, PersonIdsList.Count)]}', '{PersonIdsList[random.Next(0, PersonIdsList.Count)]}')";
                        cmd.ExecuteNonQuery();
                    }

                    sql = "SELECT id FROM project;";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid id = reader.GetGuid(reader.GetOrdinal("id"));
                                ProjectIdsList.Add(id);
                            }
                        }
                    }

                    // Insert random data into project_industry table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO project_industry(project_id, industry_id) VALUES('{ProjectIdsList[random.Next(0, ProjectIdsList.Count)]}', '{IndustryIdsList[random.Next(0, IndustryIdsList.Count)]}')";
                        cmd.ExecuteNonQuery();
                    }

                    // Insert random data into topic table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO topic(name, project_id) VALUES('{GenerateRandomString(6)}', '{ProjectIdsList[random.Next(0, ProjectIdsList.Count)]}')";
                        cmd.ExecuteNonQuery();
                    }

                    sql = "SELECT id FROM topic;";
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, con))
                    {
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid id = reader.GetGuid(reader.GetOrdinal("id"));
                                TopicIdsList.Add(id);
                            }
                        }
                    }

                    // Insert random data into timepoint table
                    for (int i = 0; i < recordsNum; i++)
                    {
                        cmd.CommandText = $"INSERT INTO timepoint(date, topic_id) VALUES(now(), '{TopicIdsList[random.Next(0, TopicIdsList.Count)]}')";
                        cmd.ExecuteNonQuery();
                    }

                    Console.WriteLine("----- Random data inserted successfully -----");
                }
            }
        }

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomString = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }

            return randomString.ToString();
        }

    }
}
