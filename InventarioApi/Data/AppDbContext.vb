Imports Microsoft.EntityFrameworkCore
Imports InventarioApi.Models

Namespace Data

    ' Contexto de base de datos: representa la conexión con SQL Server
    Public Class AppDbContext
        Inherits DbContext

        Public Sub New(options As DbContextOptions(Of AppDbContext))
            MyBase.New(options)
        End Sub

        ' Tablas
        Public Property Categorias As DbSet(Of Categoria)
        Public Property Productos As DbSet(Of Producto)

        ' Configuración de las entidades y la relación 1:N
        Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
            MyBase.OnModelCreating(modelBuilder)

            ' ---------- Categoria ----------
            Dim categoria = modelBuilder.Entity(Of Categoria)()
            categoria.ToTable("Categorias")
            categoria.HasKey(Function(c) c.Id)
            categoria.Property(Function(c) c.Nombre).IsRequired().HasMaxLength(50)
            categoria.Property(Function(c) c.Descripcion).HasMaxLength(200)
            categoria.HasIndex(Function(c) c.Nombre).IsUnique()

            ' ---------- Producto ----------
            Dim producto = modelBuilder.Entity(Of Producto)()
            producto.ToTable("Productos")
            producto.HasKey(Function(p) p.Id)
            producto.Property(Function(p) p.Nombre).IsRequired().HasMaxLength(50)
            producto.Property(Function(p) p.Descripcion).HasMaxLength(200)
            producto.Property(Function(p) p.Precio).HasColumnType("decimal(18,2)")
            producto.Property(Function(p) p.FechaRegistro).HasDefaultValueSql("GETDATE()")
            producto.HasIndex(Function(p) p.Nombre).IsUnique()

            ' ---------- Relación 1:N ----------
            ' Un producto tiene UNA categoría; una categoría tiene MUCHOS productos.
            ' Restrict: la base de datos impide borrar una categoría con productos.
            producto.HasOne(Function(p) p.Categoria) _
                    .WithMany(Function(c) c.Productos) _
                    .HasForeignKey(Function(p) p.CategoriaId) _
                    .OnDelete(DeleteBehavior.Restrict)
        End Sub

    End Class

End Namespace