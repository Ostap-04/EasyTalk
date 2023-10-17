using DotNetEnv;
using Npgsql;
using System.Text;

namespace dbSetup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Env.Load("..\\..\\..\\.env");

            var dbusername = Environment.GetEnvironmentVariable("DB_USERNAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var connectionString = $"Host=localhost;Port=5432;Username={dbusername};Password={dbPassword};";
            var connectionDBString = $"Host=localhost;Port=5432;Username={dbusername};Password={dbPassword};Database={dbName};";

            try
            {
                CreateDB(dbName, connectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                CreateTables(connectionDBString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                DataGenerator.InsertRandomData(connectionDBString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                DataGenerator.CreateIndices(connectionDBString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void CreateDB(string dbName, string connectionString)
        {
            

            var conectionString = connectionString;

            using (NpgsqlConnection con = new NpgsqlConnection(conectionString))
            {
                con.Open();
                var createDBStr = $"CREATE DATABASE {dbName};";

                using (NpgsqlCommand cmd = new NpgsqlCommand(createDBStr, con))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("----- DB created successsfuly -----");
                }
            }
        }

        

        //static string GenerateRandomEmail(int length)
        //{
        //    return GenerateRandomString(length) + "@gmail.com";
        //}

        static void CreateTables(string conectionString)
        {
            using (NpgsqlConnection con = new NpgsqlConnection(conectionString))
            {
                con.Open();
                List<string> cmdList = new List<string>();

                cmdList.Add("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");

                var createTable = @$"CREATE TABLE industry(
                    id SERIAL PRIMARY KEY,
                    industry_name VARCHAR(100)
                );";
                cmdList.Add(createTable);

                createTable = @$"CREATE TABLE person(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    industry_id SERIAL REFERENCES industry(id),
                    firstname VARCHAR(100),
                    surname VARCHAR(100),
                    nickname VARCHAR(100),
                    email VARCHAR(100),
                    type VARCHAR(30),
                    password VARCHAR(100),
                    photo BYTEA
                );";
                cmdList.Add(createTable);
                
                createTable = @$"CREATE TABLE chat(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    created_at TIMESTAMP WITH TIME ZONE,
                    updated_at TIMESTAMP WITH TIME ZONE,
                    name VARCHAR(100),
                    description TEXT
                );";
                cmdList.Add(createTable);
                
                createTable = @$"CREATE TABLE person_chat(
                    person_id UUID REFERENCES person(id),
                    chat_id UUID REFERENCES chat(id)
                );";
                cmdList.Add(createTable);

                createTable = @$"CREATE TABLE document(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    filename VARCHAR(100),
                    file_path VARCHAR(150)
                );";
                cmdList.Add(createTable);

                createTable = @$"CREATE TABLE message(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    created_at TIMESTAMP WITH TIME ZONE,
                    updated_at TIMESTAMP WITH TIME ZONE,
                    text TEXT,
                    from_id UUID REFERENCES person(id),
                    to_id UUID REFERENCES person(id),
                    chat_id UUID REFERENCES chat(id),
                    attachment_id UUID REFERENCES document(id)
                );";
                cmdList.Add(createTable);

                createTable = @$"CREATE TABLE project(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    name VARCHAR(100),
                    description TEXT,
                    client_id UUID REFERENCES person(id),
                    dev_id UUID REFERENCES person(id)
                );";
                cmdList.Add(createTable);
                
                createTable = @$"CREATE TABLE project_industry(
                    project_id UUID REFERENCES project(id),
                    industry_id SERIAL REFERENCES industry(id)
                );";
                cmdList.Add(createTable);

                createTable = @$"CREATE TABLE topic(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    name VARCHAR(100),
                    project_id UUID REFERENCES project(id)
                );";
                cmdList.Add(createTable);

                createTable = @$"CREATE TABLE timepoint(
                    id UUID PRIMARY KEY DEFAULT uuid_generate_v4 (),
                    date TIMESTAMP WITH TIME ZONE,
                    topic_id UUID REFERENCES topic(id)
                );";
                cmdList.Add(createTable);
                
                using (NpgsqlCommand cmd = new(string.Join(" ", cmdList), con))
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"----- Created tables successsfuly -----");
                }
            }

        }
    }
}