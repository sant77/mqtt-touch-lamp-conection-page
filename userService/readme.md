# User service 

This serive is focused in controll the user session, creation of the new users.

# Run the service

```bash
dotnet run
```

# Agregar una nueva miración
dotnet ef migrations add NombreDeLaMigracion

## Importand commands to migrate models

Lista todas las migraciones en el proyecto.
```bash
dotnet ef migrations list	
```
Elimina la última migración si no se ha aplicado.
```bash
dotnet ef migrations remove	
```

Aplica las migraciones pendientes a la base de datos.

```bash
dotnet ef database update
```

Actualiza la base de datos a una migración específica.

```bash
dotnet ef database update <name>
```
Genera un script SQL de las migraciones pendientes.
```bash
dotnet ef migrations script
```
