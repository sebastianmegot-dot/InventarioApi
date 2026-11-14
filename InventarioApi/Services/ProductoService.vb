Imports InventarioApi.DTOs
Imports InventarioApi.Models
Imports InventarioApi.Repositories
Imports InventarioApi.Middleware

Namespace Services

    Public Interface IProductoService
        Function ObtenerTodosAsync() As Task(Of List(Of ProductoRespuestaDto))
        Function ObtenerPorIdAsync(id As Integer) As Task(Of ProductoRespuestaDto)
        Function ObtenerPorCategoriaAsync(categoriaId As Integer) As Task(Of List(Of ProductoRespuestaDto))
        Function CrearAsync(dto As ProductoCrearDto) As Task(Of ProductoRespuestaDto)
        Function ActualizarAsync(id As Integer, dto As ProductoActualizarDto) As Task(Of ProductoRespuestaDto)
        Function EliminarAsync(id As Integer) As Task
    End Interface

    Public Class ProductoService
        Implements IProductoService

        Private ReadOnly _productoRepo As IProductoRepository
        Private ReadOnly _categoriaRepo As ICategoriaRepository

        Public Sub New(productoRepo As IProductoRepository, categoriaRepo As ICategoriaRepository)
            _productoRepo = productoRepo
            _categoriaRepo = categoriaRepo
        End Sub

        Public Async Function ObtenerTodosAsync() As Task(Of List(Of ProductoRespuestaDto)) Implements IProductoService.ObtenerTodosAsync
            Dim productos = Await _productoRepo.ObtenerTodosAsync()
            Return productos.Select(Function(p) AProductoDto(p)).ToList()
        End Function

        Public Async Function ObtenerPorIdAsync(id As Integer) As Task(Of ProductoRespuestaDto) Implements IProductoService.ObtenerPorIdAsync
            Dim producto = Await _productoRepo.ObtenerPorIdAsync(id)
            If producto Is Nothing Then Throw New NotFoundException($"No se encontró el producto con id {id}.")
            Return AProductoDto(producto)
        End Function

        Public Async Function ObtenerPorCategoriaAsync(categoriaId As Integer) As Task(Of List(Of ProductoRespuestaDto)) Implements IProductoService.ObtenerPorCategoriaAsync
            If Not Await _categoriaRepo.ExisteAsync(categoriaId) Then
                Throw New NotFoundException($"No se encontró la categoría con id {categoriaId}.")
            End If
            Dim productos = Await _productoRepo.ObtenerPorCategoriaAsync(categoriaId)
            Return productos.Select(Function(p) AProductoDto(p)).ToList()
        End Function

        Public Async Function CrearAsync(dto As ProductoCrearDto) As Task(Of ProductoRespuestaDto) Implements IProductoService.CrearAsync
            ' Regla 5: stock no negativo
            If dto.Stock < 0 Then Throw New ReglaNegocioException("El stock no puede ser negativo.")

            ' Regla 1: productos duplicados (ignora mayúsculas/minúsculas)
            If Await _productoRepo.ExisteNombreAsync(dto.Nombre) Then
                Throw New ReglaNegocioException($"Ya existe un producto con el nombre '{dto.Nombre}'.")
            End If

            ' Regla 3: la categoría debe existir
            Dim categoria = Await _categoriaRepo.ObtenerPorIdAsync(dto.CategoriaId)
            If categoria Is Nothing Then
                Throw New ReglaNegocioException($"La categoría con id {dto.CategoriaId} no existe.")
            End If

            Dim producto = New Producto With {
                .Nombre = dto.Nombre.Trim(),
                .Descripcion = dto.Descripcion?.Trim(),
                .Precio = dto.Precio,
                .Stock = dto.Stock,
                .FechaRegistro = DateTime.Now,
                .CategoriaId = dto.CategoriaId
            }
            Await _productoRepo.AgregarAsync(producto)
            Return AProductoDto(producto, categoria.Nombre)
        End Function

        Public Async Function ActualizarAsync(id As Integer, dto As ProductoActualizarDto) As Task(Of ProductoRespuestaDto) Implements IProductoService.ActualizarAsync
            Dim producto = Await _productoRepo.ObtenerPorIdAsync(id)
            If producto Is Nothing Then Throw New NotFoundException($"No se encontró el producto con id {id}.")

            ' Regla 5
            If dto.Stock < 0 Then Throw New ReglaNegocioException("El stock no puede ser negativo.")

            ' Regla 1 (excluyendo al mismo producto)
            If Await _productoRepo.ExisteNombreAsync(dto.Nombre, id) Then
                Throw New ReglaNegocioException($"Ya existe un producto con el nombre '{dto.Nombre}'.")
            End If

            ' Regla 3
            Dim categoria = Await _categoriaRepo.ObtenerPorIdAsync(dto.CategoriaId)
            If categoria Is Nothing Then
                Throw New ReglaNegocioException($"La categoría con id {dto.CategoriaId} no existe.")
            End If

            producto.Nombre = dto.Nombre.Trim()
            producto.Descripcion = dto.Descripcion?.Trim()
            producto.Precio = dto.Precio
            producto.Stock = dto.Stock
            producto.CategoriaId = dto.CategoriaId
            producto.Categoria = categoria

            Await _productoRepo.ActualizarAsync(producto)
            Return AProductoDto(producto)
        End Function

        Public Async Function EliminarAsync(id As Integer) As Task Implements IProductoService.EliminarAsync
            Dim producto = Await _productoRepo.ObtenerPorIdAsync(id)
            If producto Is Nothing Then Throw New NotFoundException($"No se encontró el producto con id {id}.")
            Await _productoRepo.EliminarAsync(producto)
        End Function

    End Class

End Namespace