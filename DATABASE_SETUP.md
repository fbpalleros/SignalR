# Configuración de la Base de Datos

## Para Desarrolladores en Windows (SQL Server Local)

¡No tenés que hacer nada! La aplicación va a usar la connection string que está en `appsettings.json`:
```
Server=MM56IG22\SQLEXPRESS;Database=SignalRDB;Trusted_Connection=True;TrustServerCertificate=True;
```

Si querés usar una configuración personalizada para desarrollo:
1. Copiá `appsettings.Development.json.template` como `appsettings.Development.json`
2. Modificá la connection string como necesites
3. El archivo está en `.gitignore` así que no se va a subir al repo

## Para Desarrolladores en Mac (Usando Podman/Docker)

Como SQL Server no corre nativamente en Mac, hay que usar un contenedor:

### 1. Instalá Podman
```bash
brew install podman
podman machine init
podman machine start
```

### 2. Levantá el Contenedor de SQL Server
```bash
# Usando Podman directamente:
podman run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=TuPassword123" \
  -p 1433:1433 \
  --name sqlserver-signal-r \
  -v sqlserver-signalr-data:/var/opt/mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest

# O usando docker-compose:
podman-compose up -d
```

### 3. Creá tu Configuración Local
Copiá `appsettings.Development.json.template` como `appsettings.Development.json` y usá:
```json
{
  "ConnectionStrings": {
    "BarcosConnection": "Server=localhost,1433;Database=SignalRDB;User Id=sa;Password=TuPassword123;TrustServerCertificate=True;"
  }
}
```

### 4. Creá la Base de Datos
```bash
# Corré las migraciones
dotnet ef database update
```

### Comandos Útiles
```bash
# Iniciar el contenedor
podman start sqlserver-signal-r

# Parar el contenedor
podman stop sqlserver-signal-r

# Ver los logs
podman logs sqlserver-signal-r

# Conectarte a SQL Server
podman exec -it sqlserver-signal-r /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'TuPassword123' -C
```

## Cómo Funciona

- `appsettings.json` - Contiene la configuración base (se sube al repo)
- `appsettings.Development.json` - Configuración local de cada desarrollador (NO se sube)
- En modo Development, las configuraciones de `Development.json` pisan las de `appsettings.json`
- Cada desarrollador puede tener su propia configuración de base de datos sin joder al resto del equipo 👍
