Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports InventarioApi.DTOs
Imports InventarioApi.Services

Namespace Controllers

    <ApiController>
    <Route("api/[controller]")>
    Public Class ProductosController
        Inherits ControllerBase

        Private ReadOnly _service As IProductoService

        Public Sub New(service As IProductoService)
            _service = service
        End Sub

        ' GET api/productos  → todos los productos incluyendo su categoría
        <HttpGet>
        <ProducesResponseType(GetType(List(Of ProductoRespuestaDto)), StatusCodes.Status200OK)>
        Public Async Function ObtenerTodos() As Task(Of IActionResult)
            Return Ok(Await _service.ObtenerTodosAsync())
        End Function

        ' GET api/productos/5
        <HttpGet("{id:int}")>
        <ProducesResponseType(GetType(ProductoRespuestaDto), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function ObtenerPorId(id As Integer) As Task(Of IActionResult)
            Return Ok(Await _service.ObtenerPorIdAsync(id))
        End Function

        ' GET api/productos/categoria/2  → productos de una categoría específica
        <HttpGet("categoria/{categoriaId:int}")>
        <ProducesResponseType(GetType(List(Of ProductoRespuestaDto)), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function ObtenerPorCategoria(categoriaId As Integer) As Task(Of IActionResult)
            Return Ok(Await _service.ObtenerPorCategoriaAsync(categoriaId))
        End Function

        ' POST api/productos
        <HttpPost>
        <ProducesResponseType(GetType(ProductoRespuestaDto), StatusCodes.Status201Created)>
        <ProducesResponseType(StatusCodes.Status400BadRequest)>
        <ProducesResponseType(StatusCodes.Status409Conflict)>
        Public Async Function Crear(dto As ProductoCrearDto) As Task(Of IActionResult)
            Dim creado = Await _service.CrearAsync(dto)
            Return CreatedAtAction(NameOf(ObtenerPorId), New With {.id = creado.Id}, creado)
        End Function

        ' PUT api/productos/5
        <HttpPut("{id:int}")>
        <ProducesResponseType(GetType(ProductoRespuestaDto), StatusCodes.Status200OK)>
        <ProducesResponseType(StatusCodes.Status400BadRequest)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        <ProducesResponseType(StatusCodes.Status409Conflict)>
        Public Async Function Actualizar(id As Integer, dto As ProductoActualizarDto) As Task(Of IActionResult)
            Return Ok(Await _service.ActualizarAsync(id, dto))
        End Function

        ' DELETE api/productos/5
        <HttpDelete("{id:int}")>
        <ProducesResponseType(StatusCodes.Status204NoContent)>
        <ProducesResponseType(StatusCodes.Status404NotFound)>
        Public Async Function Eliminar(id As Integer) As Task(Of IActionResult)
            Await _service.EliminarAsync(id)
            Return NoContent()
        End Function

    End Class

End Namespace