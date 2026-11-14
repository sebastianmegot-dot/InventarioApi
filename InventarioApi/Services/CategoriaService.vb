Imports InventarioApi.DTOs
Imports InventarioApi.Models
Imports InventarioApi.Repositories
Imports InventarioApi.Middleware

Namespace Services

    Public Interface ICategoriaService
        Function ObtenerTodasAsync() As Task(Of List(Of CategoriaRespuestaDto))
        Function ObtenerPorIdAsync(id As Integer) As Task(Of CategoriaRespuestaDto)
        Function ObtenerConProductosAsync(id As Integer) As Task(Of CategoriaRespuestaDto)
        Function CrearAsync(dto As CategoriaCrearDto) As Task(Of CategoriaRespuestaDto)
        Function ActualizarAsync(id As Integer, dto As CategoriaActualizarDto) As Task(Of CategoriaRespuestaDto)
        Function EliminarAsync(id As Integer) As Task
    End Interface

    Public Class CategoriaService
        Implements ICategoriaService

        Private ReadOnly _repo As ICategoriaRepository

        Public Sub New(repo As ICategoriaRepository)
            _repo = repo
        End Sub

        Public Async Function ObtenerTodasAsync() As Task(Of List(Of CategoriaRespuestaDto)) Implements ICategoriaService.ObtenerTodasAsync
            Dim categorias = Await _repo.ObtenerTodasAsync()
            Return categorias.Select(Function(c) ACategoriaDto(c)).ToList()
        End Function

        Public Async Function ObtenerPorIdAsync(id As Integer) As Task(Of CategoriaRespuestaDto) Implements ICategoriaService.ObtenerPorIdAsync
            Dim categoria = Await _repo.ObtenerPorIdAsync(id)
            If categoria Is Nothing Then Throw New NotFoundException($"No se encontró la categoría con id {id}.")
            Return ACategoriaDto(categoria)
        End Function

        Public Async Function ObtenerConProductosAsync(id As Integer) As Task(Of CategoriaRespuestaDto) Implements ICategoriaService.ObtenerConProductosAsync
            Dim categoria = Await _repo.ObtenerConProductosAsync(id)
            If categoria Is Nothing Then Throw New NotFoundException($"No se encontró la categoría con id {id}.")
            Return ACategoriaDto(categoria, incluirProductos:=True)
        End Function

        Public Async Function CrearAsync(dto As CategoriaCrearDto) As Task(Of CategoriaRespuestaDto) Implements ICategoriaService.CrearAsync
            ' Regla 2: categorías duplicadas
            If Await _repo.ExisteNombreAsync(dto.Nombre) Then
                Throw New ReglaNegocioException($"Ya existe una categoría con el nombre '{dto.Nombre}'.")
            End If

            Dim categoria = New Categoria With {
                .Nombre = dto.Nombre.Trim(),
                .Descripcion = dto.Descripcion?.Trim()
            }
            Await _repo.AgregarAsync(categoria)
            Return ACategoriaDto(categoria)
        End Function

        Public Async Function ActualizarAsync(id As Integer, dto As CategoriaActualizarDto) As Task(Of CategoriaRespuestaDto) Implements ICategoriaService.ActualizarAsync
            Dim categoria = Await _repo.ObtenerPorIdAsync(id)
            If categoria Is Nothing Then Throw New NotFoundException($"No se encontró la categoría con id {id}.")

            ' Regla 2 (excluyendo a la misma categoría)
            If Await _repo.ExisteNombreAsync(dto.Nombre, id) Then
                Throw New ReglaNegocioException($"Ya existe una categoría con el nombre '{dto.Nombre}'.")
            End If

            categoria.Nombre = dto.Nombre.Trim()
            categoria.Descripcion = dto.Descripcion?.Trim()
            Await _repo.ActualizarAsync(categoria)
            Return ACategoriaDto(categoria)
        End Function

        Public Async Function EliminarAsync(id As Integer) As Task Implements ICategoriaService.EliminarAsync
            Dim categoria = Await _repo.ObtenerPorIdAsync(id)
            If categoria Is Nothing Then Throw New NotFoundException($"No se encontró la categoría con id {id}.")

            ' Regla 4: no eliminar categorías con productos asociados
            If Await _repo.TieneProductosAsync(id) Then
                Throw New ReglaNegocioException("No se puede eliminar la categoría porque tiene productos asociados.")
            End If

            Await _repo.EliminarAsync(categoria)
        End Function

    End Class

End Namespace