Imports Microsoft.EntityFrameworkCore
Imports InventarioApi.Data
Imports InventarioApi.Models

Namespace Repositories

    ' Contrato del repositorio de productos
    Public Interface IProductoRepository
        Function ObtenerTodosAsync() As Task(Of List(Of Producto))
        Function ObtenerPorIdAsync(id As Integer) As Task(Of Producto)
        Function ObtenerPorCategoriaAsync(categoriaId As Integer) As Task(Of List(Of Producto))
        Function ExisteNombreAsync(nombre As String, Optional excluirId As Integer = 0) As Task(Of Boolean)
        Function AgregarAsync(producto As Producto) As Task
        Function ActualizarAsync(producto As Producto) As Task
        Function EliminarAsync(producto As Producto) As Task
    End Interface

    Public Class ProductoRepository
        Implements IProductoRepository

        Private ReadOnly _context As AppDbContext

        Public Sub New(context As AppDbContext)
            _context = context
        End Sub

        ' Productos incluyendo los datos de su categoría
        Public Async Function ObtenerTodosAsync() As Task(Of List(Of Producto)) Implements IProductoRepository.ObtenerTodosAsync
            Return Await _context.Productos _
                .Include(Function(p) p.Categoria) _
                .AsNoTracking() _
                .ToListAsync()
        End Function

        Public Async Function ObtenerPorIdAsync(id As Integer) As Task(Of Producto) Implements IProductoRepository.ObtenerPorIdAsync
            Return Await _context.Productos _
                .Include(Function(p) p.Categoria) _
                .FirstOrDefaultAsync(Function(p) p.Id = id)
        End Function

        ' Productos de una categoría específica
        Public Async Function ObtenerPorCategoriaAsync(categoriaId As Integer) As Task(Of List(Of Producto)) Implements IProductoRepository.ObtenerPorCategoriaAsync
            Return Await _context.Productos _
                .Include(Function(p) p.Categoria) _
                .Where(Function(p) p.CategoriaId = categoriaId) _
                .AsNoTracking() _
                .ToListAsync()
        End Function

        ' Regla 1: compara ignorando mayúsculas/minúsculas
        Public Async Function ExisteNombreAsync(nombre As String, Optional excluirId As Integer = 0) As Task(Of Boolean) Implements IProductoRepository.ExisteNombreAsync
            Dim nombreBuscado = nombre.Trim().ToLower()
            Return Await _context.Productos.AnyAsync(
                Function(p) p.Nombre.ToLower().Equals(nombreBuscado) AndAlso p.Id <> excluirId)
        End Function

        Public Async Function AgregarAsync(producto As Producto) As Task Implements IProductoRepository.AgregarAsync
            _context.Productos.Add(producto)
            Await _context.SaveChangesAsync()
        End Function

        Public Async Function ActualizarAsync(producto As Producto) As Task Implements IProductoRepository.ActualizarAsync
            _context.Productos.Update(producto)
            Await _context.SaveChangesAsync()
        End Function

        Public Async Function EliminarAsync(producto As Producto) As Task Implements IProductoRepository.EliminarAsync
            _context.Productos.Remove(producto)
            Await _context.SaveChangesAsync()
        End Function

    End Class

End Namespace