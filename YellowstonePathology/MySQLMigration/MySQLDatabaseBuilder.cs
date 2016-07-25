﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace YellowstonePathology.MySQLMigration
{
    public class MySQLDatabaseBuilder
    {
        private const string ConnectionString = "Server = 10.1.2.26; Uid = sqldude; Pwd = 123Whatsup; Database = lis;";
        List<string> m_ForbiddenWords;
        List<string> m_ReservedWords;
        List<string> m_KeyWords;

        public MySQLDatabaseBuilder()
        {

        }

        public void Build()
        {
            List<Type> persistentTypes = this.GetPersistenceClasses();
            foreach (Type persistentType in persistentTypes)
            {
                this.BuildCreateTableCommand(persistentType);
            }
            //this.UpdateTableSchema(typeof(YellowstonePathology.Business.Client.Model.Client));
            //this.MoveData(typeof(YellowstonePathology.Business.Client.Model.Client));
        }

        private void UpdateTableSchema(Type type)
        {             
            PropertyInfo[] properties = type.GetProperties().
                Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentProperty)) || Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentPrimaryKeyProperty))).ToArray();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo property = properties[i];

                string tableName = "tbl" + type.Name;
                string sqlCommand = "ALTER TABLE " + tableName + " ADD column " + property.Name + " " + this.GetMySQLDataType(property.PropertyType) + "; ";                    
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = sqlCommand;
                cmd.Connection = new MySqlConnection(ConnectionString);
                cmd.Connection.Open();                
                
                try
                {
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Column Added: " + property.Name);
                }
                catch(Exception)
                {
                    Console.WriteLine("Column Exists");
                }

            }                       
        }        

        private void BuildCreateTableCommand(Type type)
        {
            string tableName = "tbl" + type.Name;
            string sqlCommand ="Create Table If Not Exists " + tableName + "(";

            string primaryKeyName = string.Empty;
            string primaryKeyType = string.Empty;
            PropertyInfo[] primaryKeyProperties = type.GetProperties().
                Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentPrimaryKeyProperty))).ToArray();
            if (primaryKeyProperties.Length > 0)
            {
                primaryKeyName = primaryKeyProperties[0].Name;
                primaryKeyType = this.GetMySQLDataType(primaryKeyProperties[0].PropertyType);
            }

            PropertyInfo[] properties = type.GetProperties().
                Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentProperty)) || Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentPrimaryKeyProperty))).ToArray();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo property = properties[i];
                sqlCommand = sqlCommand + property.Name + " ";
                sqlCommand = sqlCommand + this.GetMySQLDataType(property.PropertyType) + ", ";                
            }

            if (sqlCommand.Length != 0)
            {
                sqlCommand = sqlCommand.Remove(sqlCommand.Length - 2, 2);
            }

            sqlCommand += ");";

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = sqlCommand;
            cmd.Connection = new MySqlConnection(ConnectionString);
            cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            if (string.IsNullOrEmpty(primaryKeyName) == false)
            {
                this.CreatePrimaryKey(tableName, primaryKeyName, primaryKeyType);
            }
        }

        private string GetMySQLDataType(Type type)
        {
            string result = null;    
                    
            if (type == typeof(string))
            {
                result = "TEXT";
            }
            else if (type == typeof(int))
            {
                result = "INT";
            }
            else if (type == typeof(double))
            {
                result = "DOUBLE";
            }
            else if (type == typeof(Nullable<int>))
            {
                result = "INT";
            }
            else if (type == typeof(DateTime))
            {
                result = "DATETIME";
            }
            else if (type == typeof(bool))
            {
                result = "BIT";
            }
            else if (type == typeof(Nullable<bool>))
            {
                result =  "BIT";
            }
            else if (type == typeof(Nullable<DateTime>))
            {
                result = "DATETIME";
            }
            else if (type == typeof(double?))
            {
                result = "DOUBLE";
            }
            else
            {
                throw new Exception("This Data Type is Not Implemented: " + type.Name);
            }

            return result;
        }

        private void MoveData(Type type)
        {            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = this.GetSelectStatement(type);
            cmd.CommandType = CommandType.Text;

            using (SqlConnection cn = new SqlConnection(YellowstonePathology.Properties.Settings.Default.CurrentConnectionString))
            {
                cn.Open();
                cmd.Connection = cn;
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    MySqlCommand mysqlCmd = new MySqlCommand();
                    
                    mysqlCmd.Connection = new MySqlConnection(ConnectionString);
                    mysqlCmd.Connection.Open();

                    while (dr.Read())
                    {
                        mysqlCmd.CommandText = this.GetInsertStatement(type, dr);
                        mysqlCmd.ExecuteNonQuery();
                    }
                }
            }            
        }

        private string GetSelectStatement(Type type)
        {
            string result = "Select ";

            PropertyInfo[] properties = type.GetProperties().
               Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentProperty)) || Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentPrimaryKeyProperty))).ToArray();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo property = properties[i];
                result = result + property.Name + ", ";
            }

            if (result.Length != 0)
            {
                result = result.Remove(result.Length - 2, 2);
            }

            result = result + " from tbl" + type.Name;
            return result;
        }

        private string GetInsertStatement(Type type, SqlDataReader dr)
        {
            string result = "Insert tbl" + type.Name + "(";

            PropertyInfo[] properties = type.GetProperties().
               Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentProperty)) || Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentPrimaryKeyProperty))).ToArray();

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo property = properties[i];
                result = result + property.Name + ", ";
            }

            if (result.Length != 0)
            {
                result = result.Remove(result.Length - 2, 2);
            }

            result = result + ") values (";

            for (int i = 0; i < properties.Length - 1; i++)
            {
                PropertyInfo property = properties[i];
                string dataType = this.GetMySQLDataType(property.PropertyType);
                if(dataType == "TEXT")
                {
                    string text = dr[i].ToString().Replace("'", "''");
                    if(string.IsNullOrEmpty(text))
                    {
                        result = result + "NULL, ";
                    }
                    else
                    {
                        result = result + "'" + text + "', ";
                    }                    
                }
                else
                {
                    result = result + dr[i] + ", ";
                }                
            }

            if (result.Length != 0)
            {
                result = result.Remove(result.Length - 2, 2);
            }

            result = result + ")";
            return result;
        }

        public List<Type> GetPersistenceClasses()
        {
            List<Type> result = new List<Type>();
            string assemblyName = @"C:\GIT\William\YPILIS\YellowstonePathology\bin\Debug\UserInterface.exe";
            Assembly assembly = Assembly.LoadFile(assemblyName);
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                object[] customAttributes = type.GetCustomAttributes(typeof(YellowstonePathology.Business.Persistence.PersistentClass), false);
                if (customAttributes.Length > 0)
                {
                    foreach (object o in customAttributes)
                    {
                        if (o is YellowstonePathology.Business.Persistence.PersistentClass)
                        {
                            YellowstonePathology.Business.Persistence.PersistentClass persistentClass = (YellowstonePathology.Business.Persistence.PersistentClass)o;
                            if (string.IsNullOrEmpty(persistentClass.StorageName) == false)
                            {
                                //if (persistentClass.BaseStorageName == "tblPanelSetOrder" && string.IsNullOrEmpty(persistentClass.StorageName) == false)
                                //{
                                    result.Add(type);
                                //}
                            }
                        }
                    }
                }
            }
            return result;
        }

        public string CreateIndex(string tableName, string columnName)
        {
            string result = "Index already exists";
            string indexName = "idx_" + tableName + "_" + columnName;
            string command = "Create INDEX " + indexName + " on " + tableName + " (" + columnName + ");";
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = command;
            cmd.Connection = new MySqlConnection(ConnectionString);
            cmd.Connection.Open();

            try
            {
                cmd.ExecuteNonQuery();
                result = "Index created";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = "Index already exists";
            }

            return result;
        }

        public string CreatePrimaryKey(string tableName, string columnName, string keyType)
        {
            string result = "Primary Key created.";
            string command = "alter table " + tableName + " add constraint pk_" + tableName + " primary key(" + columnName;
            if(keyType == "TEXT")
            {
                command += "(50)";
            }
            command += ")";

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = command;
            cmd.Connection = new MySqlConnection(ConnectionString);
            cmd.Connection.Open();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = "Primary Key already exists";
            }

            return result;
        }

        public string DropColumn(string tableName, string columnToBeDropped)
        {
            string result = "Column " + columnToBeDropped + " has been dropped from " + tableName;
            string command = "alter table " + tableName + " drop column " + columnToBeDropped;
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = command;
            cmd.Connection = new MySqlConnection(ConnectionString);
            cmd.Connection.Open();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = "Column " + columnToBeDropped + " does not exist in " + tableName;
            }

            return result;
        }

        private void BuildForbiddenWordLists()
        {
            m_ReservedWords = new List<string>();
            m_ForbiddenWords = new List<string>();
            m_KeyWords = new List<string>();
            m_ForbiddenWords.Add("ACCESSIBLE(R)");
            m_ForbiddenWords.Add("ACCOUNT[a]");
            m_ForbiddenWords.Add("ACTION");
            m_ForbiddenWords.Add("ADD(R)");
            m_ForbiddenWords.Add("AFTER");
            m_ForbiddenWords.Add("AGAINST");
            m_ForbiddenWords.Add("AGGREGATE");
            m_ForbiddenWords.Add("ALGORITHM");
            m_ForbiddenWords.Add("ALL(R)");
            m_ForbiddenWords.Add("ALTER(R)");
            m_ForbiddenWords.Add("ALWAYS[b]");
            m_ForbiddenWords.Add("ANALYSE");
            m_ForbiddenWords.Add("ANALYZE(R)");
            m_ForbiddenWords.Add("AND(R)");
            m_ForbiddenWords.Add("ANY");
            m_ForbiddenWords.Add("AS(R)");
            m_ForbiddenWords.Add("ASC(R)");
            m_ForbiddenWords.Add("ASCII");
            m_ForbiddenWords.Add("ASENSITIVE(R)");
            m_ForbiddenWords.Add("AT");
            m_ForbiddenWords.Add("AUTOEXTEND_SIZE");
            m_ForbiddenWords.Add("AUTO_INCREMENT");
            m_ForbiddenWords.Add("AVG");
            m_ForbiddenWords.Add("AVG_ROW_LENGTH");
            m_ForbiddenWords.Add("BACKUP");
            m_ForbiddenWords.Add("BEFORE(R)");
            m_ForbiddenWords.Add("BEGIN");
            m_ForbiddenWords.Add("BETWEEN(R)");
            m_ForbiddenWords.Add("BIGINT(R)");
            m_ForbiddenWords.Add("BINARY(R)");
            m_ForbiddenWords.Add("BINLOG");
            m_ForbiddenWords.Add("BIT");
            m_ForbiddenWords.Add("BLOB(R)");
            m_ForbiddenWords.Add("BLOCK");
            m_ForbiddenWords.Add("BOOL");
            m_ForbiddenWords.Add("BOOLEAN");
            m_ForbiddenWords.Add("BOTH(R)");
            m_ForbiddenWords.Add("BTREE");
            m_ForbiddenWords.Add("BY(R)");
            m_ForbiddenWords.Add("BYTE");
            m_ForbiddenWords.Add("CACHE");
            m_ForbiddenWords.Add("CALL(R)");
            m_ForbiddenWords.Add("CASCADE(R)");
            m_ForbiddenWords.Add("CASCADED");
            m_ForbiddenWords.Add("CASE(R)");
            m_ForbiddenWords.Add("CATALOG_NAME");
            m_ForbiddenWords.Add("CHAIN");
            m_ForbiddenWords.Add("CHANGE(R)");
            m_ForbiddenWords.Add("CHANGED");
            m_ForbiddenWords.Add("CHANNEL");
            m_ForbiddenWords.Add("CHAR(R)");
            m_ForbiddenWords.Add("CHARACTER(R)");
            m_ForbiddenWords.Add("CHARSET");
            m_ForbiddenWords.Add("CHECK(R)");
            m_ForbiddenWords.Add("CHECKSUM");
            m_ForbiddenWords.Add("CIPHER");
            m_ForbiddenWords.Add("CLASS_ORIGIN");
            m_ForbiddenWords.Add("CLIENT");
            m_ForbiddenWords.Add("CLOSE");
            m_ForbiddenWords.Add("COALESCE");
            m_ForbiddenWords.Add("CODE");
            m_ForbiddenWords.Add("COLLATE(R)");
            m_ForbiddenWords.Add("COLLATION");
            m_ForbiddenWords.Add("COLUMN(R)");
            m_ForbiddenWords.Add("COLUMNS");
            m_ForbiddenWords.Add("COLUMN_FORMAT");
            m_ForbiddenWords.Add("COLUMN_NAME");
            m_ForbiddenWords.Add("COMMENT");
            m_ForbiddenWords.Add("COMMIT");
            m_ForbiddenWords.Add("COMMITTED");
            m_ForbiddenWords.Add("COMPACT");
            m_ForbiddenWords.Add("COMPLETION");
            m_ForbiddenWords.Add("COMPRESSED");
            m_ForbiddenWords.Add("COMPRESSION");
            m_ForbiddenWords.Add("CONCURRENT");
            m_ForbiddenWords.Add("CONDITION(R)");
            m_ForbiddenWords.Add("CONNECTION");
            m_ForbiddenWords.Add("CONSISTENT");
            m_ForbiddenWords.Add("CONSTRAINT(R)");
            m_ForbiddenWords.Add("CONSTRAINT_CATALOG");
            m_ForbiddenWords.Add("CONSTRAINT_NAME");
            m_ForbiddenWords.Add("CONSTRAINT_SCHEMA");
            m_ForbiddenWords.Add("CONTAINS");
            m_ForbiddenWords.Add("CONTEXT");
            m_ForbiddenWords.Add("CONTINUE(R)");
            m_ForbiddenWords.Add("CONVERT(R)");
            m_ForbiddenWords.Add("CPU");
            m_ForbiddenWords.Add("CREATE(R)");
            m_ForbiddenWords.Add("CROSS(R)");
            m_ForbiddenWords.Add("CUBE");
            m_ForbiddenWords.Add("CURRENT");
            m_ForbiddenWords.Add("CURRENT_DATE(R)");
            m_ForbiddenWords.Add("CURRENT_TIME(R)");
            m_ForbiddenWords.Add("CURRENT_TIMESTAMP(R)");
            m_ForbiddenWords.Add("CURRENT_USER(R)");
            m_ForbiddenWords.Add("CURSOR(R)");
            m_ForbiddenWords.Add("CURSOR_NAME");
            m_ForbiddenWords.Add("DATA");
            m_ForbiddenWords.Add("DATABASE(R)");
            m_ForbiddenWords.Add("DATABASES(R)");
            m_ForbiddenWords.Add("DATAFILE");
            m_ForbiddenWords.Add("DATE");
            m_ForbiddenWords.Add("DATETIME");
            m_ForbiddenWords.Add("DAY");
            m_ForbiddenWords.Add("DAY_HOUR(R)");
            m_ForbiddenWords.Add("DAY_MICROSECOND(R)");
            m_ForbiddenWords.Add("DAY_MINUTE(R)");
            m_ForbiddenWords.Add("DAY_SECOND(R)");
            m_ForbiddenWords.Add("DEALLOCATE");
            m_ForbiddenWords.Add("DEC(R)");
            m_ForbiddenWords.Add("DECIMAL(R)");
            m_ForbiddenWords.Add("DECLARE(R)");
            m_ForbiddenWords.Add("DEFAULT(R)");
            m_ForbiddenWords.Add("DEFAULT_AUTH");
            m_ForbiddenWords.Add("DEFINER");
            m_ForbiddenWords.Add("DELAYED(R)");
            m_ForbiddenWords.Add("DELAY_KEY_WRITE");
            m_ForbiddenWords.Add("DELETE(R)");
            m_ForbiddenWords.Add("DESC(R)");
            m_ForbiddenWords.Add("DESCRIBE(R)");
            m_ForbiddenWords.Add("DES_KEY_FILE");
            m_ForbiddenWords.Add("DETERMINISTIC(R)");
            m_ForbiddenWords.Add("DIAGNOSTICS");
            m_ForbiddenWords.Add("DIRECTORY");
            m_ForbiddenWords.Add("DISABLE");
            m_ForbiddenWords.Add("DISCARD");
            m_ForbiddenWords.Add("DISK");
            m_ForbiddenWords.Add("DISTINCT(R)");
            m_ForbiddenWords.Add("DISTINCTROW(R)");
            m_ForbiddenWords.Add("DIV(R)");
            m_ForbiddenWords.Add("DO");
            m_ForbiddenWords.Add("DOUBLE(R)");
            m_ForbiddenWords.Add("DROP(R)");
            m_ForbiddenWords.Add("DUAL(R)");
            m_ForbiddenWords.Add("DUMPFILE");
            m_ForbiddenWords.Add("DUPLICATE");
            m_ForbiddenWords.Add("DYNAMIC");
            m_ForbiddenWords.Add("EACH(R)");
            m_ForbiddenWords.Add("ELSE(R)");
            m_ForbiddenWords.Add("ELSEIF(R)");
            m_ForbiddenWords.Add("ENABLE");
            m_ForbiddenWords.Add("ENCLOSED(R)");
            m_ForbiddenWords.Add("ENCRYPTION");
            m_ForbiddenWords.Add("END");
            m_ForbiddenWords.Add("ENDS");
            m_ForbiddenWords.Add("ENGINE");
            m_ForbiddenWords.Add("ENGINES");
            m_ForbiddenWords.Add("ENUM");
            m_ForbiddenWords.Add("ERROR");
            m_ForbiddenWords.Add("ERRORS");
            m_ForbiddenWords.Add("ESCAPE");
            m_ForbiddenWords.Add("ESCAPED(R)");
            m_ForbiddenWords.Add("EVENT");
            m_ForbiddenWords.Add("EVENTS");
            m_ForbiddenWords.Add("EVERY");
            m_ForbiddenWords.Add("EXCHANGE");
            m_ForbiddenWords.Add("EXECUTE");
            m_ForbiddenWords.Add("EXISTS(R)");
            m_ForbiddenWords.Add("EXIT(R)");
            m_ForbiddenWords.Add("EXPANSION");
            m_ForbiddenWords.Add("EXPIRE");
            m_ForbiddenWords.Add("EXPLAIN(R)");
            m_ForbiddenWords.Add("EXPORT");
            m_ForbiddenWords.Add("EXTENDED");
            m_ForbiddenWords.Add("EXTENT_SIZE");
            m_ForbiddenWords.Add("FALSE(R)");
            m_ForbiddenWords.Add("FAST");
            m_ForbiddenWords.Add("FAULTS");
            m_ForbiddenWords.Add("FETCH(R)");
            m_ForbiddenWords.Add("FIELDS");
            m_ForbiddenWords.Add("FILE");
            m_ForbiddenWords.Add("FILE_BLOCK_SIZE[f]");
            m_ForbiddenWords.Add("FILTER");
            m_ForbiddenWords.Add("FIRST");
            m_ForbiddenWords.Add("FIXED");
            m_ForbiddenWords.Add("FLOAT(R)");
            m_ForbiddenWords.Add("FLOAT4(R)");
            m_ForbiddenWords.Add("FLOAT8(R)");
            m_ForbiddenWords.Add("FLUSH");
            m_ForbiddenWords.Add("FOLLOWS");
            m_ForbiddenWords.Add("FOR(R)");
            m_ForbiddenWords.Add("FORCE(R)");
            m_ForbiddenWords.Add("FOREIGN(R)");
            m_ForbiddenWords.Add("FORMAT");
            m_ForbiddenWords.Add("FOUND");
            m_ForbiddenWords.Add("FROM(R)");
            m_ForbiddenWords.Add("FULL");
            m_ForbiddenWords.Add("FULLTEXT(R)");
            m_ForbiddenWords.Add("FUNCTION");
            m_ForbiddenWords.Add("GENERAL");
            m_ForbiddenWords.Add("GENERATED(R)");
            m_ForbiddenWords.Add("GEOMETRY");
            m_ForbiddenWords.Add("GEOMETRYCOLLECTION");
            m_ForbiddenWords.Add("GET(R)");
            m_ForbiddenWords.Add("GET_FORMAT");
            m_ForbiddenWords.Add("GLOBAL");
            m_ForbiddenWords.Add("GRANT(R)");
            m_ForbiddenWords.Add("GRANTS");
            m_ForbiddenWords.Add("GROUP(R)");
            m_ForbiddenWords.Add("GROUP_REPLICATION");
            m_ForbiddenWords.Add("HANDLER");
            m_ForbiddenWords.Add("HASH");
            m_ForbiddenWords.Add("HAVING(R)");
            m_ForbiddenWords.Add("HELP");
            m_ForbiddenWords.Add("HIGH_PRIORITY(R)");
            m_ForbiddenWords.Add("HOST");
            m_ForbiddenWords.Add("HOSTS");
            m_ForbiddenWords.Add("HOUR");
            m_ForbiddenWords.Add("HOUR_MICROSECOND(R)");
            m_ForbiddenWords.Add("HOUR_MINUTE(R)");
            m_ForbiddenWords.Add("HOUR_SECOND(R)");
            m_ForbiddenWords.Add("IDENTIFIED");
            m_ForbiddenWords.Add("IF(R)");
            m_ForbiddenWords.Add("IGNORE(R)");
            m_ForbiddenWords.Add("IGNORE_SERVER_IDS");
            m_ForbiddenWords.Add("IMPORT");
            m_ForbiddenWords.Add("IN(R)");
            m_ForbiddenWords.Add("INDEX(R)");
            m_ForbiddenWords.Add("INDEXES");
            m_ForbiddenWords.Add("INFILE(R)");
            m_ForbiddenWords.Add("INITIAL_SIZE");
            m_ForbiddenWords.Add("INNER(R)");
            m_ForbiddenWords.Add("INOUT(R)");
            m_ForbiddenWords.Add("INSENSITIVE(R)");
            m_ForbiddenWords.Add("INSERT(R)");
            m_ForbiddenWords.Add("INSERT_METHOD");
            m_ForbiddenWords.Add("INSTALL");
            m_ForbiddenWords.Add("INSTANCE");
            m_ForbiddenWords.Add("INT(R)");
            m_ForbiddenWords.Add("INT1(R)");
            m_ForbiddenWords.Add("INT2(R)");
            m_ForbiddenWords.Add("INT3(R)");
            m_ForbiddenWords.Add("INT4(R)");
            m_ForbiddenWords.Add("INT8(R)");
            m_ForbiddenWords.Add("INTEGER(R)");
            m_ForbiddenWords.Add("INTERVAL(R)");
            m_ForbiddenWords.Add("INTO(R)");
            m_ForbiddenWords.Add("INVOKER");
            m_ForbiddenWords.Add("IO");
            m_ForbiddenWords.Add("IO_AFTER_GTIDS(R)");
            m_ForbiddenWords.Add("IO_BEFORE_GTIDS(R)");
            m_ForbiddenWords.Add("IO_THREAD");
            m_ForbiddenWords.Add("IPC");
            m_ForbiddenWords.Add("IS(R)");
            m_ForbiddenWords.Add("ISOLATION");
            m_ForbiddenWords.Add("ISSUER");
            m_ForbiddenWords.Add("ITERATE(R)");
            m_ForbiddenWords.Add("JOIN(R)");
            m_ForbiddenWords.Add("JSON");
            m_ForbiddenWords.Add("KEY(R)");
            m_ForbiddenWords.Add("KEYS(R)");
            m_ForbiddenWords.Add("KEY_BLOCK_SIZE");
            m_ForbiddenWords.Add("KILL(R)");
            m_ForbiddenWords.Add("LANGUAGE");
            m_ForbiddenWords.Add("LAST");
            m_ForbiddenWords.Add("LEADING(R)");
            m_ForbiddenWords.Add("LEAVE(R)");
            m_ForbiddenWords.Add("LEAVES");
            m_ForbiddenWords.Add("LEFT(R)");
            m_ForbiddenWords.Add("LESS");
            m_ForbiddenWords.Add("LEVEL");
            m_ForbiddenWords.Add("LIKE(R)");
            m_ForbiddenWords.Add("LIMIT(R)");
            m_ForbiddenWords.Add("LINEAR(R)");
            m_ForbiddenWords.Add("LINES(R)");
            m_ForbiddenWords.Add("LINESTRING");
            m_ForbiddenWords.Add("LIST");
            m_ForbiddenWords.Add("LOAD(R)");
            m_ForbiddenWords.Add("LOCAL");
            m_ForbiddenWords.Add("LOCALTIME(R)");
            m_ForbiddenWords.Add("LOCALTIMESTAMP(R)");
            m_ForbiddenWords.Add("LOCK(R)");
            m_ForbiddenWords.Add("LOCKS");
            m_ForbiddenWords.Add("LOGFILE");
            m_ForbiddenWords.Add("LOGS");
            m_ForbiddenWords.Add("LONG(R)");
            m_ForbiddenWords.Add("LONGBLOB(R)");
            m_ForbiddenWords.Add("LONGTEXT(R)");
            m_ForbiddenWords.Add("LOOP(R)");
            m_ForbiddenWords.Add("LOW_PRIORITY(R)");
            m_ForbiddenWords.Add("MASTER");
            m_ForbiddenWords.Add("MASTER_AUTO_POSITION");
            m_ForbiddenWords.Add("MASTER_BIND(R)");
            m_ForbiddenWords.Add("MASTER_CONNECT_RETRY");
            m_ForbiddenWords.Add("MASTER_DELAY");
            m_ForbiddenWords.Add("MASTER_HEARTBEAT_PERIOD");
            m_ForbiddenWords.Add("MASTER_HOST");
            m_ForbiddenWords.Add("MASTER_LOG_FILE");
            m_ForbiddenWords.Add("MASTER_LOG_POS");
            m_ForbiddenWords.Add("MASTER_PASSWORD");
            m_ForbiddenWords.Add("MASTER_PORT");
            m_ForbiddenWords.Add("MASTER_RETRY_COUNT");
            m_ForbiddenWords.Add("MASTER_SERVER_ID");
            m_ForbiddenWords.Add("MASTER_SSL");
            m_ForbiddenWords.Add("MASTER_SSL_CA");
            m_ForbiddenWords.Add("MASTER_SSL_CAPATH");
            m_ForbiddenWords.Add("MASTER_SSL_CERT");
            m_ForbiddenWords.Add("MASTER_SSL_CIPHER");
            m_ForbiddenWords.Add("MASTER_SSL_CRL");
            m_ForbiddenWords.Add("MASTER_SSL_CRLPATH");
            m_ForbiddenWords.Add("MASTER_SSL_KEY");
            m_ForbiddenWords.Add("MASTER_SSL_VERIFY_SERVER_CERT(R)");
            m_ForbiddenWords.Add("MASTER_TLS_VERSION");
            m_ForbiddenWords.Add("MASTER_USER");
            m_ForbiddenWords.Add("MATCH(R)");
            m_ForbiddenWords.Add("MAXVALUE(R)");
            m_ForbiddenWords.Add("MAX_CONNECTIONS_PER_HOUR");
            m_ForbiddenWords.Add("MAX_QUERIES_PER_HOUR");
            m_ForbiddenWords.Add("MAX_ROWS");
            m_ForbiddenWords.Add("MAX_SIZE");
            m_ForbiddenWords.Add("MAX_STATEMENT_TIME");
            m_ForbiddenWords.Add("MAX_UPDATES_PER_HOUR");
            m_ForbiddenWords.Add("MAX_USER_CONNECTIONS");
            m_ForbiddenWords.Add("MEDIUM");
            m_ForbiddenWords.Add("MEDIUMBLOB(R)");
            m_ForbiddenWords.Add("MEDIUMINT(R)");
            m_ForbiddenWords.Add("MEDIUMTEXT(R)");
            m_ForbiddenWords.Add("MEMORY");
            m_ForbiddenWords.Add("MERGE");
            m_ForbiddenWords.Add("MESSAGE_TEXT");
            m_ForbiddenWords.Add("MICROSECOND");
            m_ForbiddenWords.Add("MIDDLEINT(R)");
            m_ForbiddenWords.Add("MIGRATE");
            m_ForbiddenWords.Add("MINUTE");
            m_ForbiddenWords.Add("MINUTE_MICROSECOND(R)");
            m_ForbiddenWords.Add("MINUTE_SECOND(R)");
            m_ForbiddenWords.Add("MIN_ROWS");
            m_ForbiddenWords.Add("MOD(R)");
            m_ForbiddenWords.Add("MODE");
            m_ForbiddenWords.Add("MODIFIES(R)");
            m_ForbiddenWords.Add("MODIFY");
            m_ForbiddenWords.Add("MONTH");
            m_ForbiddenWords.Add("MULTILINESTRING");
            m_ForbiddenWords.Add("MULTIPOINT");
            m_ForbiddenWords.Add("MULTIPOLYGON");
            m_ForbiddenWords.Add("MUTEX");
            m_ForbiddenWords.Add("MYSQL_ERRNO");
            m_ForbiddenWords.Add("NAME");
            m_ForbiddenWords.Add("NAMES");
            m_ForbiddenWords.Add("NATIONAL");
            m_ForbiddenWords.Add("NATURAL(R)");
            m_ForbiddenWords.Add("NCHAR");
            m_ForbiddenWords.Add("NDB");
            m_ForbiddenWords.Add("NDBCLUSTER");
            m_ForbiddenWords.Add("NEVER");
            m_ForbiddenWords.Add("NEW");
            m_ForbiddenWords.Add("NEXT");
            m_ForbiddenWords.Add("NO");
            m_ForbiddenWords.Add("NODEGROUP");
            m_ForbiddenWords.Add("NONBLOCKING");
            m_ForbiddenWords.Add("NONE");
            m_ForbiddenWords.Add("NOT(R)");
            m_ForbiddenWords.Add("NO_WAIT");
            m_ForbiddenWords.Add("NO_WRITE_TO_BINLOG(R)");
            m_ForbiddenWords.Add("NULL(R)");
            m_ForbiddenWords.Add("NUMBER");
            m_ForbiddenWords.Add("NUMERIC(R)");
            m_ForbiddenWords.Add("NVARCHAR");
            m_ForbiddenWords.Add("OFFSET");
            m_ForbiddenWords.Add("OLD_PASSWORD");
            m_ForbiddenWords.Add("ON(R)");
            m_ForbiddenWords.Add("ONE");
            m_ForbiddenWords.Add("ONLY");
            m_ForbiddenWords.Add("OPEN");
            m_ForbiddenWords.Add("OPTIMIZE(R)");
            m_ForbiddenWords.Add("OPTIMIZER_COSTS(R)");
            m_ForbiddenWords.Add("OPTION(R)");
            m_ForbiddenWords.Add("OPTIONALLY(R)");
            m_ForbiddenWords.Add("OPTIONS");
            m_ForbiddenWords.Add("OR(R)");
            m_ForbiddenWords.Add("ORDER(R)");
            m_ForbiddenWords.Add("OUT(R)");
            m_ForbiddenWords.Add("OUTER(R)");
            m_ForbiddenWords.Add("OUTFILE(R)");
            m_ForbiddenWords.Add("OWNER");
            m_ForbiddenWords.Add("PACK_KEYS");
            m_ForbiddenWords.Add("PAGE");
            m_ForbiddenWords.Add("PARSER");
            m_ForbiddenWords.Add("PARSE_GCOL_EXPR");
            m_ForbiddenWords.Add("PARTIAL");
            m_ForbiddenWords.Add("PARTITION(R)");
            m_ForbiddenWords.Add("PARTITIONING");
            m_ForbiddenWords.Add("PARTITIONS");
            m_ForbiddenWords.Add("PASSWORD");
            m_ForbiddenWords.Add("PHASE");
            m_ForbiddenWords.Add("PLUGIN");
            m_ForbiddenWords.Add("PLUGINS");
            m_ForbiddenWords.Add("PLUGIN_DIR");
            m_ForbiddenWords.Add("POINT");
            m_ForbiddenWords.Add("POLYGON");
            m_ForbiddenWords.Add("PORT");
            m_ForbiddenWords.Add("PRECEDES");
            m_ForbiddenWords.Add("PRECISION(R)");
            m_ForbiddenWords.Add("PREPARE");
            m_ForbiddenWords.Add("PRESERVE");
            m_ForbiddenWords.Add("PREV");
            m_ForbiddenWords.Add("PRIMARY(R)");
            m_ForbiddenWords.Add("PRIVILEGES");
            m_ForbiddenWords.Add("PROCEDURE(R)");
            m_ForbiddenWords.Add("PROCESSLIST");
            m_ForbiddenWords.Add("PROFILE");
            m_ForbiddenWords.Add("PROFILES");
            m_ForbiddenWords.Add("PROXY");
            m_ForbiddenWords.Add("PURGE(R)");
            m_ForbiddenWords.Add("QUARTER");
            m_ForbiddenWords.Add("QUERY");
            m_ForbiddenWords.Add("QUICK");
            m_ForbiddenWords.Add("RANGE(R)");
            m_ForbiddenWords.Add("READ(R)");
            m_ForbiddenWords.Add("READS(R)");
            m_ForbiddenWords.Add("READ_ONLY");
            m_ForbiddenWords.Add("READ_WRITE(R)");
            m_ForbiddenWords.Add("REAL(R)");
            m_ForbiddenWords.Add("REBUILD");
            m_ForbiddenWords.Add("RECOVER");
            m_ForbiddenWords.Add("REDOFILE");
            m_ForbiddenWords.Add("REDO_BUFFER_SIZE");
            m_ForbiddenWords.Add("REDUNDANT");
            m_ForbiddenWords.Add("REFERENCES(R)");
            m_ForbiddenWords.Add("REGEXP(R)");
            m_ForbiddenWords.Add("RELAY");
            m_ForbiddenWords.Add("RELAYLOG");
            m_ForbiddenWords.Add("RELAY_LOG_FILE");
            m_ForbiddenWords.Add("RELAY_LOG_POS");
            m_ForbiddenWords.Add("RELAY_THREAD");
            m_ForbiddenWords.Add("RELEASE(R)");
            m_ForbiddenWords.Add("RELOAD");
            m_ForbiddenWords.Add("REMOVE");
            m_ForbiddenWords.Add("RENAME(R)");
            m_ForbiddenWords.Add("REORGANIZE");
            m_ForbiddenWords.Add("REPAIR");
            m_ForbiddenWords.Add("REPEAT(R)");
            m_ForbiddenWords.Add("REPEATABLE");
            m_ForbiddenWords.Add("REPLACE(R)");
            m_ForbiddenWords.Add("REPLICATE_DO_DB");
            m_ForbiddenWords.Add("REPLICATE_DO_TABLE");
            m_ForbiddenWords.Add("REPLICATE_IGNORE_DB");
            m_ForbiddenWords.Add("REPLICATE_IGNORE_TABLE");
            m_ForbiddenWords.Add("REPLICATE_REWRITE_DB");
            m_ForbiddenWords.Add("REPLICATE_WILD_DO_TABLE");
            m_ForbiddenWords.Add("REPLICATE_WILD_IGNORE_TABLE");
            m_ForbiddenWords.Add("REPLICATION");
            m_ForbiddenWords.Add("REQUIRE(R)");
            m_ForbiddenWords.Add("RESET");
            m_ForbiddenWords.Add("RESIGNAL(R)");
            m_ForbiddenWords.Add("RESTORE");
            m_ForbiddenWords.Add("RESTRICT(R)");
            m_ForbiddenWords.Add("RESUME");
            m_ForbiddenWords.Add("RETURN(R)");
            m_ForbiddenWords.Add("RETURNED_SQLSTATE");
            m_ForbiddenWords.Add("RETURNS");
            m_ForbiddenWords.Add("REVERSE");
            m_ForbiddenWords.Add("REVOKE(R)");
            m_ForbiddenWords.Add("RIGHT(R)");
            m_ForbiddenWords.Add("RLIKE(R)");
            m_ForbiddenWords.Add("ROLLBACK");
            m_ForbiddenWords.Add("ROLLUP");
            m_ForbiddenWords.Add("ROTATE");
            m_ForbiddenWords.Add("ROUTINE");
            m_ForbiddenWords.Add("ROW");
            m_ForbiddenWords.Add("ROWS");
            m_ForbiddenWords.Add("ROW_COUNT");
            m_ForbiddenWords.Add("ROW_FORMAT");
            m_ForbiddenWords.Add("RTREE");
            m_ForbiddenWords.Add("SAVEPOINT");
            m_ForbiddenWords.Add("SCHEDULE");
            m_ForbiddenWords.Add("SCHEMA(R)");
            m_ForbiddenWords.Add("SCHEMAS(R)");
            m_ForbiddenWords.Add("SCHEMA_NAME");
            m_ForbiddenWords.Add("SECOND");
            m_ForbiddenWords.Add("SECOND_MICROSECOND(R)");
            m_ForbiddenWords.Add("SECURITY");
            m_ForbiddenWords.Add("SELECT(R)");
            m_ForbiddenWords.Add("SENSITIVE(R)");
            m_ForbiddenWords.Add("SEPARATOR(R)");
            m_ForbiddenWords.Add("SERIAL");
            m_ForbiddenWords.Add("SERIALIZABLE");
            m_ForbiddenWords.Add("SERVER");
            m_ForbiddenWords.Add("SESSION");
            m_ForbiddenWords.Add("SET(R)");
            m_ForbiddenWords.Add("SHARE");
            m_ForbiddenWords.Add("SHOW(R)");
            m_ForbiddenWords.Add("SHUTDOWN");
            m_ForbiddenWords.Add("SIGNAL(R)");
            m_ForbiddenWords.Add("SIGNED");
            m_ForbiddenWords.Add("SIMPLE");
            m_ForbiddenWords.Add("SLAVE");
            m_ForbiddenWords.Add("SLOW");
            m_ForbiddenWords.Add("SMALLINT(R)");
            m_ForbiddenWords.Add("SNAPSHOT");
            m_ForbiddenWords.Add("SOCKET");
            m_ForbiddenWords.Add("SOME");
            m_ForbiddenWords.Add("SONAME");
            m_ForbiddenWords.Add("SOUNDS");
            m_ForbiddenWords.Add("SOURCE");
            m_ForbiddenWords.Add("SPATIAL(R)");
            m_ForbiddenWords.Add("SPECIFIC(R)");
            m_ForbiddenWords.Add("SQL(R)");
            m_ForbiddenWords.Add("SQLEXCEPTION(R)");
            m_ForbiddenWords.Add("SQLSTATE(R)");
            m_ForbiddenWords.Add("SQLWARNING(R)");
            m_ForbiddenWords.Add("SQL_AFTER_GTIDS");
            m_ForbiddenWords.Add("SQL_AFTER_MTS_GAPS");
            m_ForbiddenWords.Add("SQL_BEFORE_GTIDS");
            m_ForbiddenWords.Add("SQL_BIG_RESULT(R)");
            m_ForbiddenWords.Add("SQL_BUFFER_RESULT");
            m_ForbiddenWords.Add("SQL_CACHE");
            m_ForbiddenWords.Add("SQL_CALC_FOUND_ROWS(R)");
            m_ForbiddenWords.Add("SQL_NO_CACHE");
            m_ForbiddenWords.Add("SQL_SMALL_RESULT(R)");
            m_ForbiddenWords.Add("SQL_THREAD");
            m_ForbiddenWords.Add("SQL_TSI_DAY");
            m_ForbiddenWords.Add("SQL_TSI_HOUR");
            m_ForbiddenWords.Add("SQL_TSI_MINUTE");
            m_ForbiddenWords.Add("SQL_TSI_MONTH");
            m_ForbiddenWords.Add("SQL_TSI_QUARTER");
            m_ForbiddenWords.Add("SQL_TSI_SECOND");
            m_ForbiddenWords.Add("SQL_TSI_WEEK");
            m_ForbiddenWords.Add("SQL_TSI_YEAR");
            m_ForbiddenWords.Add("SSL(R)");
            m_ForbiddenWords.Add("STACKED");
            m_ForbiddenWords.Add("START");
            m_ForbiddenWords.Add("STARTING(R)");
            m_ForbiddenWords.Add("STARTS");
            m_ForbiddenWords.Add("STATS_AUTO_RECALC");
            m_ForbiddenWords.Add("STATS_PERSISTENT");
            m_ForbiddenWords.Add("STATS_SAMPLE_PAGES");
            m_ForbiddenWords.Add("STATUS");
            m_ForbiddenWords.Add("STOP");
            m_ForbiddenWords.Add("STORAGE");
            m_ForbiddenWords.Add("STORED(R)");
            m_ForbiddenWords.Add("STRAIGHT_JOIN(R)");
            m_ForbiddenWords.Add("STRING");
            m_ForbiddenWords.Add("SUBCLASS_ORIGIN");
            m_ForbiddenWords.Add("SUBJECT");
            m_ForbiddenWords.Add("SUBPARTITION");
            m_ForbiddenWords.Add("SUBPARTITIONS");
            m_ForbiddenWords.Add("SUPER");
            m_ForbiddenWords.Add("SUSPEND");
            m_ForbiddenWords.Add("SWAPS");
            m_ForbiddenWords.Add("SWITCHES");
            m_ForbiddenWords.Add("TABLE(R)");
            m_ForbiddenWords.Add("TABLES");
            m_ForbiddenWords.Add("TABLESPACE");
            m_ForbiddenWords.Add("TABLE_CHECKSUM");
            m_ForbiddenWords.Add("TABLE_NAME");
            m_ForbiddenWords.Add("TEMPORARY");
            m_ForbiddenWords.Add("TEMPTABLE");
            m_ForbiddenWords.Add("TERMINATED(R)");
            m_ForbiddenWords.Add("TEXT");
            m_ForbiddenWords.Add("THAN");
            m_ForbiddenWords.Add("THEN(R)");
            m_ForbiddenWords.Add("TIME");
            m_ForbiddenWords.Add("TIMESTAMP");
            m_ForbiddenWords.Add("TIMESTAMPADD");
            m_ForbiddenWords.Add("TIMESTAMPDIFF");
            m_ForbiddenWords.Add("TINYBLOB(R)");
            m_ForbiddenWords.Add("TINYINT(R)");
            m_ForbiddenWords.Add("TINYTEXT(R)");
            m_ForbiddenWords.Add("TO(R)");
            m_ForbiddenWords.Add("TRAILING(R)");
            m_ForbiddenWords.Add("TRANSACTION");
            m_ForbiddenWords.Add("TRIGGER(R)");
            m_ForbiddenWords.Add("TRIGGERS");
            m_ForbiddenWords.Add("TRUE(R)");
            m_ForbiddenWords.Add("TRUNCATE");
            m_ForbiddenWords.Add("TYPE");
            m_ForbiddenWords.Add("TYPES");
            m_ForbiddenWords.Add("UNCOMMITTED");
            m_ForbiddenWords.Add("UNDEFINED");
            m_ForbiddenWords.Add("UNDO(R)");
            m_ForbiddenWords.Add("UNDOFILE");
            m_ForbiddenWords.Add("UNDO_BUFFER_SIZE");
            m_ForbiddenWords.Add("UNICODE");
            m_ForbiddenWords.Add("UNINSTALL");
            m_ForbiddenWords.Add("UNION(R)");
            m_ForbiddenWords.Add("UNIQUE(R)");
            m_ForbiddenWords.Add("UNKNOWN");
            m_ForbiddenWords.Add("UNLOCK(R)");
            m_ForbiddenWords.Add("UNSIGNED(R)");
            m_ForbiddenWords.Add("UNTIL");
            m_ForbiddenWords.Add("UPDATE(R)");
            m_ForbiddenWords.Add("UPGRADE");
            m_ForbiddenWords.Add("USAGE(R)");
            m_ForbiddenWords.Add("USE(R)");
            m_ForbiddenWords.Add("USER");
            m_ForbiddenWords.Add("USER_RESOURCES");
            m_ForbiddenWords.Add("USE_FRM");
            m_ForbiddenWords.Add("USING(R)");
            m_ForbiddenWords.Add("UTC_DATE(R)");
            m_ForbiddenWords.Add("UTC_TIME(R)");
            m_ForbiddenWords.Add("UTC_TIMESTAMP(R)");
            m_ForbiddenWords.Add("VALIDATION");
            m_ForbiddenWords.Add("VALUE");
            m_ForbiddenWords.Add("VALUES(R)");
            m_ForbiddenWords.Add("VARBINARY(R)");
            m_ForbiddenWords.Add("VARCHAR(R)");
            m_ForbiddenWords.Add("VARCHARACTER(R)");
            m_ForbiddenWords.Add("VARIABLES");
            m_ForbiddenWords.Add("VARYING(R)");
            m_ForbiddenWords.Add("VIEW");
            m_ForbiddenWords.Add("VIRTUAL(R)");
            m_ForbiddenWords.Add("WAIT");
            m_ForbiddenWords.Add("WARNINGS");
            m_ForbiddenWords.Add("WEEK");
            m_ForbiddenWords.Add("WEIGHT_STRING");
            m_ForbiddenWords.Add("WHEN(R)");
            m_ForbiddenWords.Add("WHERE(R)");
            m_ForbiddenWords.Add("WHILE(R)");
            m_ForbiddenWords.Add("WITH(R)");
            m_ForbiddenWords.Add("WITHOUT");
            m_ForbiddenWords.Add("WORK");
            m_ForbiddenWords.Add("WRAPPER");
            m_ForbiddenWords.Add("WRITE(R)");
            m_ForbiddenWords.Add("X509");
            m_ForbiddenWords.Add("XA");
            m_ForbiddenWords.Add("XID");
            m_ForbiddenWords.Add("XML");
            m_ForbiddenWords.Add("XOR(R)");
            m_ForbiddenWords.Add("YEAR");
            m_ForbiddenWords.Add("YEAR_MONTH(R)");
            m_ForbiddenWords.Add("ZEROFILL(R)");

            foreach(string word in m_ForbiddenWords)
            {
                if(word.Contains("(R)") == true)
                {
                    m_ReservedWords.Add(word.Substring(0, word.Length - 3));
                }
                else
                {
                    m_KeyWords.Add(word);
                }
            }
        }

        private bool IsForbiddenWord(string name, StringBuilder result)
        {
            if(this.m_ReservedWords.Contains(name.ToUpper()))
            {
                result.AppendLine("   *** " + name + " is a reserved word. Modifying to Report" + name);
                return true;
            }

            if (this.m_KeyWords.Contains(name.ToUpper()))
            {
                result.AppendLine("   " + name + " is a key word.");
            }
            return false;
        }

        private void GetClassPersistentProperties(Type type, Type baseType, List<PropertyInfo> tableProperties)
        {
            PropertyInfo[] baseProperties = baseType.GetProperties().
                Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentProperty))).ToArray();
            PropertyInfo[] classProperties = type.GetProperties().
                Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentProperty))).ToArray();
            foreach(PropertyInfo property in classProperties)
            {
                bool matched = false;
                foreach (PropertyInfo baseProperty in baseProperties)
                {
                    if (property.Name == baseProperty.Name)
                    {
                        matched = true;
                        break;
                    }
                }
                if(matched == false)
                {
                    tableProperties.Add(property);
                }
            }
        }

        private void CreateTableCommand(Type type, List<PropertyInfo> tableProperties, StringBuilder result)
        {
            string originalName = type.Name;
            if (originalName.Contains("_Base")) originalName = originalName.Substring(0, originalName.Length - 5);
            string tableName = "tbl" + originalName;
            string sqlCommand = "Create Table If Not Exists " + tableName + "(";

            for (int i = 0; i < tableProperties.Count; i++)
            {
                PropertyInfo columnProperty = tableProperties[i];
                string propertyName = columnProperty.Name;
                if (this.IsForbiddenWord(propertyName, result) == true) propertyName = "Report" + propertyName;
                sqlCommand += propertyName + " ";
                sqlCommand += this.GetMySQLDataType(columnProperty.PropertyType) + ", ";
            }

            if (sqlCommand.Length != 0)
            {
                sqlCommand = sqlCommand.Remove(sqlCommand.Length - 2, 2);
            }

            sqlCommand += ");";
            result.AppendLine(sqlCommand);
        }

        private void CreatePrimaryKeyCommand(string tableName, string columnName, string keyType, StringBuilder result)
        {
            string command = "alter table " + tableName + " add constraint pk_" + tableName + " primary key(" + columnName;
            if (keyType == "TEXT")
            {
                command += "(50)";
            }
            command += ")";
            result.AppendLine(command);
        }

        private void FindPersistentClassIssues(Type type, Type baseType, StringBuilder result)
        {
            string originalName = type.Name;
            if (originalName.Contains("_Base")) originalName = originalName.Substring(0, originalName.Length - 5);
            string tableName = "tbl" + originalName;
            List<PropertyInfo> properties = new List<PropertyInfo>();
            string primaryKeyName = string.Empty;
            string primaryKeyType = string.Empty;
            PropertyInfo[] primaryKeyProperties = type.GetProperties().
                Where(prop => Attribute.IsDefined(prop, typeof(Business.Persistence.PersistentPrimaryKeyProperty))).ToArray();

            if (primaryKeyProperties.Length > 0)
            {
                primaryKeyName = primaryKeyProperties[0].Name;
                primaryKeyType = this.GetMySQLDataType(primaryKeyProperties[0].PropertyType);
                this.IsForbiddenWord(primaryKeyName, result);
                properties.Add(primaryKeyProperties[0]);
            }
            //else
            //{
            //    result.AppendLine("   No Primary Key.");
            //}

            this.GetClassPersistentProperties(type, baseType, properties);

            if (properties.Count == 1 && primaryKeyProperties.Length > 0)
            {
                return;
            //    result.AppendLine("   No Persistent Data.");
            }
            else
            {
                this.CreateTableCommand(type, properties, result);
                this.CreatePrimaryKeyCommand(tableName, primaryKeyName, primaryKeyType, result);
            }
        }

        public void CreatePersistentClassDetails()
        {
            this.BuildForbiddenWordLists();
            StringBuilder result = new StringBuilder();
            List<Type> persistentTypes = this.GetPersistenceClasses();
            foreach (Type persistentType in persistentTypes)
            {
                result.AppendLine(persistentType.Name);
                Type baseType = persistentType.BaseType;
                this.FindPersistentClassIssues(persistentType, baseType, result);
                result.AppendLine();
            }

            if (result.Length > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(@"c:\WCTMP\PersistentErrors.txt", false))
                {
                    outputFile.Write(result);
                }
            }
        }
    }
}
