Imports Microsoft.AspNetCore.Builder
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports InventarioApi.Data
Imports InventarioApi.Repositories
Imports InventarioApi.Services
Imports InventarioApi.Middleware

Module Program

    Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' ---------- Servicios ----------
        builder.Services.AddControllers()
        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        ' EF Core con SQL Server
        builder.Services.AddDbContext(Of AppDbContext)(
            Sub(opciones) opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))

        ' Inyección de dependencias: Repositories y Services
        builder.Services.AddScoped(Of ICategoriaRepository, CategoriaRepository)()
        builder.Services.AddScoped(Of IProductoRepository, ProductoRepository)()
        builder.Services.AddScoped(Of ICategoriaService, CategoriaService)()
        builder.Services.AddScoped(Of IProductoService, ProductoService)()

        Dim app = builder.Build()

        ' Crea la base de datos si no existe (provisorio, ver nota abajo)
        Using scope = app.Services.CreateScope()
            Dim db = scope.ServiceProvider.GetRequiredService(Of AppDbContext)()
            db.Database.EnsureCreated()
        End Using

        ' ---------- Pipeline ----------
        ' El middleware de errores va PRIMERO para capturar todo lo que pase después
        app.UseMiddleware(Of ManejoErroresMiddleware)()

        app.UseSwagger()
        app.UseSwaggerUI(Sub(c)
                             c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventario API v1")
                             c.RoutePrefix = String.Empty   ' Swagger se abre en la raíz
                         End Sub)

        app.MapControllers()
        app.Run()
    End Sub

End Module