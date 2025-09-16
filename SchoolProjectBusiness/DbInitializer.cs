using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SchoolProject
{
    public static class DatabasePackager
    {
        private static string connectionString = "Server=.;Database=DD2;User Id=sa;Password=sa123456;";
        private static string databaseName = "DD2";
        private static string backupFilePath = Path.Combine(Application.StartupPath, $"{databaseName}.bak");
        private static string scriptFilePath = Path.Combine(Application.StartupPath, $"{databaseName}_Schema.sql");

        public static bool PackageDatabase()
        {
            try
            {
                // Create backup of the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Backup database
                    string backupQuery = $"BACKUP DATABASE [{databaseName}] TO DISK = '{backupFilePath}'";
                    using (SqlCommand command = new SqlCommand(backupQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Generate schema script
                    GenerateSchemaScript(connection);
                }

                MessageBox.Show("Database packaged successfully!", "Success",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error packaging database: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static void GenerateSchemaScript(SqlConnection connection)
        {
            try
            {
                // This is a simplified approach - in a real scenario, you might use SMO (SQL Server Management Objects)
                // or a third-party library to generate a more complete schema script
                string schemaScript = @"
/* School Management System Database Schema Script */
/* Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " */\n\n";

                // Get table creation scripts
                string tablesQuery = @"
SELECT 
    'IF NOT EXISTS (SELECT * FROM sysobjects WHERE name=''' + t.name + ''' AND xtype=''U'')' + CHAR(13) +
    'CREATE TABLE ' + t.name + ' (' + CHAR(13) +
    STUFF((
        SELECT ', ' + c.name + ' ' + 
            CASE 
                WHEN ty.name IN ('varchar', 'nvarchar', 'char', 'nchar') THEN 
                    ty.name + '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length AS VARCHAR(10)) END + ')'
                WHEN ty.name IN ('decimal', 'numeric') THEN 
                    ty.name + '(' + CAST(c.precision AS VARCHAR(10)) + ', ' + CAST(c.scale AS VARCHAR(10)) + ')'
                ELSE ty.name
            END + 
            CASE WHEN c.is_nullable = 0 THEN ' NOT NULL' ELSE ' NULL' END +
            CASE WHEN ic.column_id IS NOT NULL THEN ' PRIMARY KEY' ELSE '' END
        FROM sys.columns c
        INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
        LEFT JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        LEFT JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id AND i.is_primary_key = 1
        WHERE c.object_id = t.object_id
        ORDER BY c.column_id
        FOR XML PATH('')
    ), 1, 2, '') + CHAR(13) + ')' AS TableScript
FROM sys.tables t
WHERE t.is_ms_shipped = 0
ORDER BY t.name";

                using (SqlCommand command = new SqlCommand(tablesQuery, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        schemaScript += reader["TableScript"].ToString() + "\n\n";
                    }
                }

                // Write schema script to file
                File.WriteAllText(scriptFilePath, schemaScript);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating schema script: {ex.Message}", "Warning",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private static void ExecuteEmbeddedSchemaScript(SqlConnection connection)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "YourNamespace.DatabaseSchema.sql";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string script = reader.ReadToEnd();

                    // Split the script on GO commands (SQL Server requires separate execution for batches)
                    string[] commands = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string command in commands)
                    {
                        if (!string.IsNullOrWhiteSpace(command))
                        {
                            using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                            {
                                sqlCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing schema script: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static bool DeployDatabase()
        {
            try
            {
                // Check if backup file exists
                if (!File.Exists(backupFilePath))
                {
                    MessageBox.Show("Database backup file not found. Please package the database first.",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Restore database
                using (SqlConnection connection = new SqlConnection(connectionString.Replace(databaseName, "master")))
                {
                    connection.Open();

                    // Check if database exists and drop it if it does
                    string checkDbQuery = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'";
                    using (SqlCommand command = new SqlCommand(checkDbQuery, connection))
                    {
                        int exists = (int)command.ExecuteScalar();
                        if (exists > 0)
                        {
                            string dropDbQuery = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}]";
                            using (SqlCommand dropCommand = new SqlCommand(dropDbQuery, connection))
                            {
                                dropCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    // Restore database from backup
                    string restoreQuery = $@"
RESTORE DATABASE [{databaseName}] 
FROM DISK = '{backupFilePath}' 
WITH REPLACE, RECOVERY";

                    using (SqlCommand command = new SqlCommand(restoreQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Database deployed successfully!", "Success",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deploying database: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool IsDatabasePackaged()
        {
            return File.Exists(backupFilePath);
        }
    }
}