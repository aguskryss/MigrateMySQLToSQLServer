using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

class DataMigration
{
    private static string MySqlConnectionString;
    private static string SqlServerConnectionString;

    static void Main(string[] args)
    {
        // Configuración para leer el archivo JSON
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Obtener las cadenas de conexión del archivo JSON
        MySqlConnectionString = configuration.GetConnectionString("MySqlConnectionString");
        SqlServerConnectionString = configuration.GetConnectionString("SqlServerConnectionString");
        // Mapeo de nombres de tablas entre SQL Server y MySQL
        var tableMappings = new Dictionary<string, string>
    {
        { "admins", "usuarios" },
        { "regions", "regiones" }, // Mapeo para sucursales
        { "branchs", "sucursales" }, // Mapeo para sucursales
        { "daily", "agendaConfig" },
        { "files", "archivos" },
        { "providers", "proveedores" }, // Mapeo para sucursales
        { "turns", "turnos" },
        {"user_location", "usuario_sucursal" },
        {"provider_branch", "proveedor_sucursal" },
        {"admin_branch", "usuario_sucursal" }

        
        // Agrega más tablas según sea necesario
    };

        // Mapeo de columnas por tabla
        var columnMappingsByTable = new Dictionary<string, Dictionary<string, string>>
    {
        { "daily", new Dictionary<string, string>
            {
                { "id", "id" },
                { "sucursal_id", "branch_id" },
                { "dia", "day" },
                { "desde", "desde" },
                { "hasta", "hasta" },
                { "minutosTurno", "minutosTurno" },
                { "maxTurnos", "maxTurnos" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" },
                { "activo", "activo" }
            }
        },
        { "files", new Dictionary<string, string>
            {
                { "id", "id" },
                { "id_turno", "turn_id" },
                { "codigo", "code" }
            }
        },
        { "turns", new Dictionary<string, string>
            {
                { "id", "id" },
                { "proveedor_id", "provider_id" },
                { "sucursal_id", "branch_id" },
                { "horario", "time" },
                { "horario_ingreso", "entry_time" },
                { "ausente", "absent" },
                { "estado_id", "status" },
                { "observaciones", "observations" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" }
            }
        },
        { "admins", new Dictionary<string, string>
            {
                { "id", "id" },
                { "usuario", "user" },
                { "clave", "pass" },
                { "nombre", "name" },
                { "apellido", "surname" },
                { "mail", "mail" },
                { "estado", "status" },
                { "cambiarClave", "change_pass" },
                { "tempToken", "token" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" },
                { "configUsuarios", "user_config" },
                { "configProveedores", "provider_config" },
                { "configSucursales", "branch_config" }
            }
        },
        { "branchs", new Dictionary<string, string> // Mapeo de la tabla sucursales
            {
                { "id", "id" },
                { "nombre", "name" },
                { "codigo", "code" },
                { "mail_comercial", "commercial_mail" },
                { "mail_it", "it_mail" },
                { "region_id", "region_id" },
                { "Direccion", "address" },
                { "longitud", "longitude" },
                { "latitud", "latitude" },
                { "empresa_id", "company_id" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" }
            }
        },
        { "regions", new Dictionary<string, string> // Mapeo de la tabla sucursales
            {
            { "id", "id" },
                { "nombre", "name" }
            }
        },
        { "providers", new Dictionary<string, string> // Mapeo para proveedores
            {
            { "id", "id" },
                { "codigo", "code" },
                { "cuit", "cuit" },
                { "rs", "business_name" },
                { "nombre_fantasia", "name" },
                { "mail_comercial", "commercial_mail" },
                { "mail_ti", "it_mail" },
                { "region_id", "region_id" },
                { "origen", "origin" },
                { "direccion", "address" },
                { "observaciones", "observations" },
                { "fcObligatorio", "fc_required" },
                { "clave", "pass" },
                { "cambiarClave", "change_pass" },
                { "estado", "status" },
                { "tempToken", "token" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" }
            }
        },
        { "user_location", new Dictionary<string, string> // Mapeo para usuario_sucursal
            {
            { "id", "id" },
                { "usuario_id", "usuario_id" },
                { "sucursal_id", "sucursal_id" },
                { "perfil_id", "perfil_id" },
                { "confirmaTurnos", "confirmaTurnos" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" }
            }
        },
        { "provider_branch", new Dictionary<string, string> // Mapeo para usuario_sucursal
            {
            { "id", "id" },
                { "proveedor_id", "provider_id" },
                { "sucursal_id", "branch_id" },
                { "perfil_id", "profile_id" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" }
            }
        },
        { "admin_branch", new Dictionary<string, string> // Mapeo para usuario_sucursal
            {
            { "id", "id" },
            { "usuario_id", "admin_id" },
                { "sucursal_id", "branch_id" },
                { "perfil_id", "profile_id" },
                { "confirmaTurnos", "confirm" },
                { "created_at", "created_at" },
                { "updated_at", "updated_at" },
                { "created_by", "created_by" },
                { "updated_by", "updated_by" }
            }
        }



        // Puedes seguir agregando más tablas aquí...
        };

        // Comprobamos la conexión a las bases de datos
        if (TestMySqlConnection() && TestSqlServerConnection())
        {
            List<string> tables = new List<string>
        {
            "admins",
            "regions",
            "branchs", // Agregado el nombre de la tabla sucursales
            "daily",
            "providers",
            "turns",
            "files",
            "user_location",
            "provider_branch",
            "admin_branch"
            
            
            // Agrega más tablas según sea necesario
        };


            foreach (var table in tables)
            {
                if (columnMappingsByTable.ContainsKey(table))
                {
                    // Obtenemos el nombre de la tabla en MySQL
                    string mySqlTableName = tableMappings.ContainsKey(table) ? tableMappings[table] : table;
                    MigrateData(table, mySqlTableName, columnMappingsByTable[table]);
                }
                else
                {
                    Console.WriteLine($"No se encontró mapeo para la tabla {table}. Omitiendo migración.");
                }
            }
        }
        else
        {
            Console.WriteLine("Error: No se pudo conectar a una o ambas bases de datos.");
        }
    }

    private static bool TestMySqlConnection()
    {
        try
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(MySqlConnectionString))
            {
                mySqlConnection.Open();
                Console.WriteLine("Conexión exitosa a MySQL.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al conectar a MySQL: " + ex.Message);
            return false;
        }
    }

    private static bool TestSqlServerConnection()
    {
        try
        {
            using (SqlConnection sqlServerConnection = new SqlConnection(SqlServerConnectionString))
            {
                sqlServerConnection.Open();
                Console.WriteLine("Conexión exitosa a SQL Server.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al conectar a SQL Server: " + ex.Message);
            return false;
        }
    }

    // Este es el método genérico que puedes usar para chequear si el registro ya existe
    private static bool CheckIfRecordExists(SqlConnection sqlServerConnection, string tableName, string columnName, object value)
    {
        string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {columnName} = @Value";
        SqlCommand checkCommand = new SqlCommand(checkQuery, sqlServerConnection);
        checkCommand.Parameters.AddWithValue("@Value", value);

        int count = (int)checkCommand.ExecuteScalar();
        return count > 0;  // Retorna true si el registro ya existe
    }

    // Luego, dentro de tu función de migración, usas este método para verificar duplicados antes de insertar:
    public static void MigrateData(string sqlServerTableName, string mySqlTableName, Dictionary<string, string> columnMappings)
    {
        int pageSize = 800; // Número de registros por lote
        int currentPage = 0;
        bool hasMoreData = true;

        using (SqlConnection sqlServerConnection = new SqlConnection(SqlServerConnectionString))
        {
            sqlServerConnection.Open();

            // Desactivar restricciones de claves foráneas antes de realizar las insercioness
            if(sqlServerTableName != "user_location")
                DisableForeignKeys(sqlServerConnection, sqlServerTableName);

            while (hasMoreData)
            {
                string selectQuery = $"SELECT * FROM {mySqlTableName} LIMIT {pageSize} OFFSET {currentPage * pageSize}";

                try
                {
                    using (MySqlConnection mySqlConnection = new MySqlConnection(MySqlConnectionString))
                    {
                        mySqlConnection.Open();

                        MySqlCommand selectCommand = new MySqlCommand(selectQuery, mySqlConnection);
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            bool hasRecords = false; // Bandera para saber si hay registros en el lote
                            while (reader.Read())
                            {
                                hasRecords = true;

                                // Aquí verificamos si el registro ya existe antes de insertarlo
                                string idColumnName = "id";  // Usa el nombre del campo único para la tabla
                                object recordId = reader[idColumnName];  // Aquí puedes cambiar la lógica para otros campos únicos

                                if (CheckIfRecordExists(sqlServerConnection, sqlServerTableName, idColumnName, recordId))
                                {
                                    continue;  // Si el registro ya existe, lo saltamos
                                }

                                string insertQuery = GenerateInsertQuery(reader, columnMappings, sqlServerTableName);
                                SqlCommand sqlCommand = new SqlCommand(insertQuery, sqlServerConnection);

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string columnName = reader.GetName(i);
                                    if (columnMappings.ContainsKey(columnName))
                                    {
                                        string mappedColumn = columnMappings[columnName];
                                        object value = reader.GetValue(i);
                                        if (value == DBNull.Value)
                                        {
                                            sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", DBNull.Value);
                                        }
                                        else
                                        {
                                            if (value is byte || value is sbyte)
                                            {
                                                sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", Convert.ToByte(value));
                                            }
                                            else if (value is int)
                                            {
                                                sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", Convert.ToInt32(value));
                                            }
                                            else if (value is string)
                                            {
                                                sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", value.ToString());
                                            }
                                            else if (value is DateTime)
                                            {
                                                DateTime dateValue = (DateTime)value;

                                                // Compara con DateTime.MinValue o con la fecha inválida "00/00/0000 00:00:00"
                                                if (dateValue == DateTime.MinValue || dateValue.ToString("dd/MM/yyyy HH:mm:ss") == "00/00/0000 00:00:00")
                                                {
                                                    sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", null);
                                                }
                                                else
                                                {
                                                    sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", dateValue);
                                                }
                                            }

                                            else
                                            {
                                                sqlCommand.Parameters.AddWithValue($"@{mappedColumn}", value);
                                            }
                                        }
                                    }
                                }

                                try
                                {
                                    sqlCommand.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    // Log the error with the SQL query
                                    LogError($"Error al insertar registro en {sqlServerTableName}. Query: {insertQuery} | Error: {ex.Message}");
                                }
                            }

                            // Si no hay registros en el lote, significa que hemos terminado
                            hasMoreData = hasRecords;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and include the select query for better tracking
                    LogError($"Error durante la migración de datos de la tabla {sqlServerTableName}. Query: {selectQuery} | Error: {ex.Message} | Stack: {ex.StackTrace}");
                    break; // Si hay un error, detenemos la migración
                }

                // Avanzar a la siguiente página
                currentPage++;
            }
            if (sqlServerTableName != "user_location")
                // Volver a habilitar las claves foráneas después de las inserciones
                EnableForeignKeys(sqlServerConnection, sqlServerTableName);
        }
    }



    private static void DisableForeignKeys(SqlConnection sqlServerConnection, string sqlServerTableName)
    {
        string disableFKQuery = $"ALTER TABLE {sqlServerTableName} NOCHECK CONSTRAINT ALL;";
        SqlCommand disableFKCommand = new SqlCommand(disableFKQuery, sqlServerConnection);
        try
        {
            disableFKCommand.ExecuteNonQuery();
            Console.WriteLine($"Las claves foráneas de la tabla {sqlServerTableName} han sido desactivadas.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al desactivar las claves foráneas en {sqlServerTableName}: {ex.Message}");
        }
    }

    private static void EnableForeignKeys(SqlConnection sqlServerConnection, string sqlServerTableName)
    {
        string enableFKQuery = $"ALTER TABLE {sqlServerTableName} WITH CHECK CHECK CONSTRAINT ALL;";
        SqlCommand enableFKCommand = new SqlCommand(enableFKQuery, sqlServerConnection);
        try
        {
            enableFKCommand.ExecuteNonQuery();
            Console.WriteLine($"Las claves foráneas de la tabla {sqlServerTableName} han sido habilitadas.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al habilitar las claves foráneas en {sqlServerTableName}: {ex.Message}");
        }
    }


    private static void LogError(string message)
    {
        string logFilePath = "migration_log.txt";  // Ruta donde se guardará el log
        using (StreamWriter writer = new StreamWriter(logFilePath, true))  // "true" asegura que se agregue al final del archivo
        {
            writer.WriteLine($"[{DateTime.Now}] {message}");
        }
    }
   

    private static bool CheckIfEmailExists(string email, SqlConnection sqlServerConnection, string tableName)
    {
        if (tableName == "admins")
        {
            string checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE mail = @Mail";
            SqlCommand checkCommand = new SqlCommand(checkQuery, sqlServerConnection);
            checkCommand.Parameters.AddWithValue("@Mail", email);

            int count = (int)checkCommand.ExecuteScalar();
            return count > 0;
        }
        return false;
    }

    private static string GenerateInsertQuery(MySqlDataReader reader, Dictionary<string, string> columnMappings, string tableName)
    {
        var columns = new List<string>();
        var values = new List<string>();

        for (int i = 0; i < reader.FieldCount; i++)
        {
            string columnName = reader.GetName(i);
            if (columnMappings.ContainsKey(columnName))
            {
                string mappedColumn = columnMappings[columnName];
                columns.Add($"[{mappedColumn}]");

                object value = reader[columnName];
                string valueString;
                if (value == DBNull.Value)
                {
                    valueString = "NULL";
                }
                else if (value is DateTime dateTimeValue)
                {
                    valueString = $"'{dateTimeValue:yyyy-MM-dd HH:mm:ss}'";
                }
                else
                {
                    valueString = $"'{value.ToString().Replace("'", "''")}'";
                }
                values.Add(valueString);
            }
        }

        string columnsJoined = string.Join(", ", columns);
        string valuesJoined = string.Join(", ", values);

        if (tableName != "user_location")
            return $"SET IDENTITY_INSERT {tableName} ON; INSERT INTO {tableName} ({columnsJoined}) VALUES ({valuesJoined}); SET IDENTITY_INSERT {tableName} OFF;";
        else
            return $"INSERT INTO {tableName} ({columnsJoined}) VALUES ({valuesJoined});";
    }



}

