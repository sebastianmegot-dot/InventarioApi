# InventarioApi

API REST para administrar **categorías** y **productos** de un sistema de inventario.

Examen Segundo Parcial - Programación Orientada por Eventos Avanzada (V. Basic).

## Tecnologías

- Visual Basic .NET (.NET 10)
- ASP.NET Core Web API (controllers)
- Entity Framework Core 10 + SQL Server
- Swagger (Swashbuckle.AspNetCore)

## Arquitectura

```
InventarioApi/                 (raíz del repositorio)
├── README.md
├── InventarioDb.sql           → Script para reconstruir la base de datos
├── Evidencias/                → Capturas de las pruebas en Swagger
├── InventarioApi.slnx
└── InventarioApi/             (proyecto)
    ├── Controllers/    → Reciben las peticiones HTTP y llaman a los Services (no usan el DbContext)
    ├── Services/       → Reglas de negocio y conversión entidad ↔ DTO
    ├── Repositories/   → Acceso a datos con EF Core (Include, consultas, guardado)
    ├── Models/         → Entidades Categoria y Producto (relación 1:N)
    ├── DTOs/           → Datos de entrada (crear / actualizar) y salida (respuesta) con validaciones
    ├── Data/           → AppDbContext (configuración de entidades y relación)
    ├── Middleware/     → Manejo global de excepciones y excepciones personalizadas
    ├── appsettings.json
    └── Program.vb      → Registro de servicios (inyección de dependencias) y pipeline
```

Cada Controller depende de una interfaz de Service (`ICategoriaService`, `IProductoService`). Cada Service depende de interfaces de Repository (`ICategoriaRepository`, `IProductoRepository`). Todas se registran con `AddScoped` en `Program.vb`.

## Requisitos

- Visual Studio 2022/2026 con la carga de trabajo **ASP.NET y desarrollo web**
- .NET 10 SDK
- SQL Server o SQL Server LocalDB (incluido con Visual Studio)

## Configuración y ejecución

1. Clonar el repositorio:
   ```
   git clone https://github.com/sebastianmegot-dot/InventarioApi.git
   ```
2. Abrir `InventarioApi.slnx` en Visual Studio.
3. Revisar la cadena de conexión en `appsettings.json` y ajustar `Server=` si se usa otra instancia de SQL Server:
   ```json
   "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=InventarioDb;Trusted_Connection=True;TrustServerCertificate=True"
   ```
4. Crear la base de datos (una de las dos opciones):
   - **Automática:** al ejecutar el proyecto, `EnsureCreated()` crea la base `InventarioDb` si no existe.
   - **Manual:** ejecutar el script `InventarioDb.sql` en SQL Server Management Studio o en el Explorador de objetos de SQL Server.
5. Ejecutar el proyecto (F5). Swagger se abre en la raíz: `https://localhost:<puerto>/`

## Endpoints

### Categorías

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/Categorias` | Consultar todas las categorías | 200 |
| GET | `/api/Categorias/{id}` | Consultar una categoría por id | 200, 404 |
| GET | `/api/Categorias/{id}/productos` | Consultar una categoría incluyendo sus productos | 200, 404 |
| POST | `/api/Categorias` | Registrar una categoría | 201, 400, 409 |
| PUT | `/api/Categorias/{id}` | Modificar una categoría | 200, 400, 404, 409 |
| DELETE | `/api/Categorias/{id}` | Eliminar una categoría sin productos | 204, 404, 409 |

### Productos

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| GET | `/api/Productos` | Consultar todos los productos incluyendo su categoría | 200 |
| GET | `/api/Productos/{id}` | Consultar un producto por id | 200, 404 |
| GET | `/api/Productos/categoria/{categoriaId}` | Consultar productos de una categoría | 200, 404 |
| POST | `/api/Productos` | Registrar un producto | 201, 400, 409 |
| PUT | `/api/Productos/{id}` | Modificar un producto | 200, 400, 404, 409 |
| DELETE | `/api/Productos/{id}` | Eliminar un producto | 204, 404 |

## Validaciones (DTOs)

| Campo | Validación |
|---|---|
| Nombre del producto | Obligatorio, máximo 50 caracteres |
| Descripción | Máximo 200 caracteres |
| Precio | Mayor que cero |
| Stock | No puede ser negativo |
| Categoría | Obligatoria y debe existir |
| Nombre de categoría | Obligatorio, máximo 50 caracteres |

Si los datos no cumplen las validaciones, la API responde **400 Bad Request**.

## Reglas de negocio (implementadas en Services)

| Regla | Descripción | Código |
|---|---|---|
| 1 | No se permiten dos productos con el mismo nombre (ignora mayúsculas/minúsculas) | 409 |
| 2 | No se permiten dos categorías con el mismo nombre | 409 |
| 3 | No se puede registrar ni modificar un producto con una categoría inexistente | 409 |
| 4 | No se puede eliminar una categoría con productos asociados | 409 |
| 5 | No se permite stock negativo | 400 / 409 |

## Manejo de errores

El middleware `ManejoErroresMiddleware` captura todas las excepciones de forma centralizada:

| Excepción | Código HTTP |
|---|---|
| `NotFoundException` | 404 |
| `ReglaNegocioException` | 409 |
| `DbUpdateException` | 409 |
| Cualquier otra | 500 (mensaje genérico, sin detalles internos) |

Formato de respuesta de error:
```json
{ "estado": 409, "mensaje": "Ya existe un producto con el nombre 'Mouse inalámbrico'." }
```

## Pruebas realizadas en Swagger

Las capturas están en la carpeta `Evidencias/`.

| # | Prueba | Código esperado |
|---|---|---|
| 1 | Registrar una categoría | 201 |
| 2 | Registrar un producto asociado a una categoría | 201 |
| 3 | Consultar todos los productos incluyendo su categoría | 200 |
| 4 | Consultar productos por categoría | 200 |
| 5 | Intentar registrar un producto duplicado | 409 |
| 6 | Intentar registrar un producto con una categoría inexistente | 409 |
| 7 | Intentar eliminar una categoría con productos asociados | 409 |
| 8 | Modificar un producto | 200 |
| 9 | Eliminar un producto | 204 |

## Autor

Sebastian Melgarejo Gomez
