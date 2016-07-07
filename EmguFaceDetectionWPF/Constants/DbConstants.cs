using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguFaceDetectionWPF.Constants
{
    public static class DbConstants
    {
        public const string CONNECTION_STRING = "Data Source=emgudb.sqlite;Version=3;";
        public const string TABLE_USERS = "CREATE TABLE users(id INTEGER PRIMARY KEY AUTOINCREMENT, UniqueId INTEGER, Name VARCHAR(255), Face BLOB);";
        public const string TABLE_LOGS = "CREATE TABLE logs(id INTEGER PRIMARY KEY AUTOINCREMENT, User INTEGER, Log DATETIME, Message VARCHAR(255));";

        public const string ALL_USERS_KEY = "ALL_USERS";

        public const string INSERT_USER = "INSERT INTO users(UniqueId, Name, Face) VALUES(@uid, @name, @face);";
        public const string INSERT_LOG = "INSERT INTO logs(User, Log, Message) VALUES (@user, @log, @msg);";

        public const string GET_UID = "SELECT UniqueId FROM users WHERE Name=@name LIMIT 1;";
        public const string GET_ALL_USERS = "SELECT * FROM users;";
        public const string GET_USER_BY_NAME = "SELECT * FROM users WHERE Name=@name;";
    }
}
