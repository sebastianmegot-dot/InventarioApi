Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports InventarioApi.DTOs
Imports InventarioApi.Services

Namespace Controllers

    <ApiController>
    <Route("api/[controller]")>
    Public Class CategoriasController
        Inherits ControllerBase

        ' El controller NO usa el DbContext: solo llama al Service
        Private ReadOnly _service As ICategoriaService

        Public Sub New(service As ICategoriaService)
            _service = service
        End Sub

        ' GET api/categorias
        <HttpGet>
        <ProducesResponseType(GetType(List(Of CategoriaRespuestaDto)), StatusCodes.Status200OK)>
        Public Async Function ObtenerTodas() As Task(Of IActionResult)
            Return Ok(Await _service.ObtenerTodasAsync())
        End Function

        ' GET api/categorias/5
        <HttpGet("{id:int}")>
        <ProducesResponseType(GetType(CategoriaRespuestaDto), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function ObtenerPorId(id As Integer) As Task(Of IActionResult)
            Return Ok(Await _service.ObtenerPorIdAsync(id))
        End Function

        ' GET api/categorias/5/productos  → categoría incluyendo sus productos
        <HttpGet("{id:int}/productos")>
        <ProducesResponseType(GetType(CategoriaRespuestaDto), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function ObtenerConProductos(id As Integer) As Task(Of IActionResult)
            Return Ok(Await _service.ObtenerConProductosAsync(id))
        End Function

        ' POST api/categorias
        <HttpPost>
        <ProducesResponseType(GetType(CategoriaRespuestaDto), StatusCodes.Status201Created)>
        <ProducesResponseType(StatusCodes.Status400BadRequest)>
        <ProducesResponseType(StatusCodes.Status409Conflict)>
        Public Async Function Crear(dto As CategoriaCrearDto) As Task(Of IActionResult)
            Dim creada = Await _service.CrearAsync(dto)
            Return CreatedAtAction(NameOf(ObtenerPorId), New With {.id = creada.Id}, creada)
        End Function

        ' PUT api/categorias/5
        <HttpPut("{id:int}")>
        <ProducesResponseType(GetType(CategoriaRespuestaDto), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status400BadRequest)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        <ProducesResponseType(StatusCodes.Status409Conflict)>
        Public Async Function Actualizar(id As Integer, dto As CategoriaActualizarDto) As Task(Of IActionResult)
            Return Ok(Await _service.ActualizarAsync(id, dto))
        End Function

        ' DELETE api/categorias/5
        <HttpDelete("{id:int}")>
        <ProducesResponseType(StatusCodes.Status204NoContent)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        <ProducesResponseType(StatusCodes.Status409Conflict)>
        Public Async Function Eliminar(id As Integer) As Task(Of IActionResult)
            Await _service.EliminarAsync(id)
            Return NoContent()
        End Function

    End Class

End Namespace