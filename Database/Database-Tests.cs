using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Swarmops.Database
{
    public partial class SwarmDb
    {
        public bool TestLogin()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TestCreateTable()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command = GetDbCommand (
                        "CREATE TABLE `CredentialsTests` (" +
                        "  `TestId` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT," +
                        "  `Description` VARCHAR(256) NOT NULL," +
                        "  `DateTimeCreated` DATETIME NOT NULL," +
                        "  PRIMARY KEY (`TestId`)" +
                        ") ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='This table is created to test database credentials'",
                        connection);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }

        public bool TestAlterTable()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command = GetDbCommand (
                        "ALTER TABLE `CredentialsTests` ADD COLUMN `AppendedColumn` INT AFTER `DateTimeCreated`",
                        connection);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }


        public bool TestDropTable()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command = GetDbCommand (
                        "DROP TABLE `CredentialsTests`", connection);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }

        public bool TestCreateProcedure()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command = GetDbCommand (
                        "CREATE PROCEDURE `CreateCredentialsTest`(" +
                        "   description VARCHAR(128)" +
                        ")" +
                        "BEGIN" +
                        "" +
                        "    INSERT INTO CredentialsTests (Description,DateTimeCreated)" +
                        "        VALUES(description, NOW());" +
                        "    " +
                        "END", connection);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }

        public bool TestDropProcedure()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command = GetDbCommand (
                        "DROP PROCEDURE `CreateCredentialsTest`", connection);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }

        public bool TestExecute (string testDescription)
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();
                    DbCommand command = GetDbCommand ("CreateCredentialsTest", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    AddParameterWithName (command, "description", testDescription);

                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }

        public bool TestSelect()
        {
            try
            {
                using (DbConnection connection = GetMySqlDbConnection())
                {
                    connection.Open();

                    DbCommand command =
                        GetDbCommand (
                            "SELECT * FROM CredentialsTests", connection);
                    command.ExecuteReader();

                    return true;
                }
            }
            catch (Exception debug)
            {
                Debug.WriteLine (debug.ToString());
                return false;
            }
        }
    }
}