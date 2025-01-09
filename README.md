Esta es una guia para poder ejecutar la aplicación y poder realizar la migración.

1. Modificar el archivo appsettings.json, y reemplazar los dos string de conexiones por los de su servidor.
MySqlConnectionString": "Server=COMPLETAR IP O HOSTNAME;Database=COMPLETAR BASE DE DATOS;Uid=COMPLETAR USUARIO;Pwd=COMPLETAR CONTRASEÑA;ConnectionTimeout=120;",
SqlServerConnectionString": "Server=COMPLETAR IP O HOSTNAME;Database=COMPLETAR BASE DE DATOS;Trusted_Connection=True;"

2. Ejecutar por comando o el .exe MigrateMySQLToSQLServer.exe

3. Por comando se vera el proceso hasta que finalice, llevará unos minutos.
