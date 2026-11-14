Imports Microsoft.EntityFrameworkCore
Imports InventarioApi.Data
Imports InventarioApi.Models

Namespace Repositories

    ' Contrato del repositorio de categorías
    Public Interface ICategoriaRepository
        Function ObtenerTodasAsync() As Task(Of List(Of Categoria))
        Function ObtenerPorIdAsync(id As Integer) As Task(Of Categoria)
        Function ObtenerConProductosAsync(id As Integer) As Task(Of Categoria)
        Function ExisteAsync(id As Integer) As Task(Of Boolean)
        Function ExisteNombreAsync(nombre As String, Optional excluirId As Integer = 0) As Task(Of Boolean)
        Function TieneProductosAsync(id As Integer) As Task(Of Boolean)
        Function AgregarAsync(categoria As Categoria) As Task
        Function ActualizarAsync(categoria As Categoria) As Task
        Function EliminarAsync(categoria As Categoria) As Task
    End Interface

    ' Implementación: única capa que usa el DbContext
    Public Class CategoriaRepository
        Implements ICategoriaRepository

        Private ReadOnly _context As AppDbContext

        Public Sub New(context As AppDbContext)
            _context = context
        End Sub

        Public Async Function ObtenerTodasAsync() As Task(Of List(Of Categoria)) Implements ICategoriaRepository.ObtenerTodasAsync
            Return Await _context.Categorias.AsNoTracking().ToListAsync()
        End Function

        Public Async Function ObtenerPorIdAsync(id As Integer) As Task(Of Categoria) Implements ICategoriaRepository.ObtenerPorIdAsync
            Return Await _context.Categorias.FirstOrDefaultAsync(Function(c) c.Id = id)
        End Function

        ' Usa Include() para traer la categoría junto con sus productos
        Public Async Function ObtenerConProductosAsync(id As Integer) As Task(Of Categoria) Implements ICategoriaRepository.ObtenerConProductosAsync
            Return Await _context.Categorias _
                .Include(Function(c) c.Productos) _
                .AsNoTracking() _
                .FirstOrDefaultAsync(Function(c) c.Id = id)
        End Function

        Public Async Function ExisteAsync(id As Integer) As Task(Of Boolean) Implements ICategoriaRepository.ExisteAsync
            Return Await _context.Categorias.AnyAsync(Function(c) c.Id = id)
        End Function

        ' Compara ignorando mayúsculas/minúsculas. excluirId sirve al modificar (para no compararse consigo misma)
        Public Async Function ExisteNombreAsync(nombre As String, Optional excluirId As Integer = 0) As Task(Of Boolean) Implements ICategoriaRepository.ExisteNombreAsync
            Dim nombreBuscado = nombre.Trim().ToLower()
            Return Await _context.Categorias.AnyAsync(
                Function(c) c.Nombre.ToLower().Equals(nombreBuscado) AndAlso c.Id <> excluirId)
        End Function

        Public Async Function TieneProductosAsync(id As Integer) As Task(Of Boolean) Implements ICategoriaRepository.TieneProductosAsync
            Return Await _context.Productos.AnyAsync(Function(p) p.CategoriaId = id)
        End Function

        Public Async Function AgregarAsync(categoria As Categoria) As Task Implements ICategoriaRepository.AgregarAsync
            _context.Categorias.Add(categoria)
            Await _context.SaveChangesAsync()
        End Function

        Public Async Function ActualizarAsync(categoria As Categoria) As Task Implements ICategoriaRepository.ActualizarAsync
            _context.Categorias.Update(categoria)
            Await _context.SaveChangesAsync()
        End Function

        Public Async Function EliminarAsync(categoria As Categoria) As Task Implements ICategoriaRepository.EliminarAsync
            _context.Categorias.Remove(categoria)
            Await _context.SaveChangesAsync()
        End Function

    End Class

End Namespace