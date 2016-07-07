using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Data.SQLite;
using EmguFaceDetectionWPF.Constants;
using EmguFaceDetectionWPF.Models;
using System.Data.Common;

namespace EmguFaceDetectionWPF.Helpers
{
    public class SqliteHelper
    {
        public void Init()
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string dbName = "emgudb.sqlite";
            string fullDbPath = Path.Combine(baseDirectory, dbName);

            string recognizerName = "recognizer.YAML";
            string fulRecognizerPath = Path.Combine(baseDirectory, recognizerName);
            if (!File.Exists(fullDbPath) || !File.Exists(fulRecognizerPath))
            {
                SQLiteConnection.CreateFile(fullDbPath);
                InitTables();

                File.Create(fulRecognizerPath);
            }
        }

        public string InsertUser(UserModel user)
        {
            int existingUid = GetUniqueId(user.Name);
            if (existingUid == 0) existingUid = GenerateUniqueId();
            else return string.Empty;

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DbConstants.CONNECTION_STRING))
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(DbConstants.INSERT_USER, con))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@uid", existingUid);
                        cmd.Parameters.AddWithValue("@name", user.Name);
                        cmd.Parameters.AddWithValue("@face", user.Face);
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();

                    return string.Format("Added user with Unique ID: {0}", existingUid);
                }
            }
            catch (Exception ex)
            {
                return string.Format("Error adding user with message: {0}", ex.Message);
            }
        }

        public string InsertLog(int user, string message, DateTime now)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(DbConstants.CONNECTION_STRING))
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(DbConstants.INSERT_LOG, con))
                    {
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@user", user);
                        cmd.Parameters.AddWithValue("@log", now);
                        cmd.Parameters.AddWithValue("@msg", message);
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();

                    return string.Format("Logged user with Unique ID: {0}", user);
                }
            }
            catch (Exception ex)
            {
                return string.Format("Error adding user with message: {0}", ex.Message);
            }
        }

        public List<UserModel> GetUsers(string name)
        {
            var users = new List<UserModel>();
            try
            {
                string sql = name.ToLower().Equals(DbConstants.ALL_USERS_KEY.ToLower()) ? DbConstants.GET_ALL_USERS : DbConstants.GET_USER_BY_NAME;
                using (SQLiteConnection con = new SQLiteConnection(DbConstants.CONNECTION_STRING))
                {
                    con.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        cmd.Prepare();
                        cmd.Parameters.Clear();
                        if (!name.ToLower().Equals(DbConstants.ALL_USERS_KEY.ToLower())) cmd.Parameters.AddWithValue("@name", name);
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (!rdr.HasRows) return null;
                            while (rdr.Read())
                            {
                                users.Add(new UserModel
                                {
                                    Uid = rdr.GetInt32(1),
                                    Name = rdr.GetString(2),
                                    Face = rdr.GetFieldValue<byte[]>(3)
                                });
                            }
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Poop, " + ex.Message);
            }
            return users;
        }

        public async Task<List<LogModel>> GetLogs()
        {
            List<LogModel> logs = new List<LogModel>();
            try
            {
                string sql = "SELECT a.UniqueId, a.Name, b.Log FROM logs b INNER JOIN users a ON a.UniqueId = b.User;";
                using (SQLiteConnection con = new SQLiteConnection(DbConstants.CONNECTION_STRING))
                {
                    await con.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        using (DbDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (!rdr.HasRows) return null;
                            while (await rdr.ReadAsync())
                            {
                                logs.Add(new LogModel
                                {
                                    Uid = await rdr.GetFieldValueAsync<long>(0),
                                    Name = await rdr.GetFieldValueAsync<string>(1),
                                    Log = await rdr.GetFieldValueAsync<DateTime>(2)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Poop, " + ex.Message);
            }
            return logs;
        }

        public int GetUniqueId(string name)
        {
            int uid = 0;
            using (SQLiteConnection con = new SQLiteConnection(DbConstants.CONNECTION_STRING))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(DbConstants.GET_UID, con))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@name", name);
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (!rdr.HasRows) return 0;
                        while (rdr.Read())
                            uid = rdr.GetInt32(0);
                    }
                }
            }
            return uid;
        }

        public int GenerateUniqueId()
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            return Convert.ToInt32(date);
        }

        private void InitTables()
        {
            using (SQLiteConnection con = new SQLiteConnection(DbConstants.CONNECTION_STRING))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = DbConstants.TABLE_USERS;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = DbConstants.TABLE_LOGS;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}
