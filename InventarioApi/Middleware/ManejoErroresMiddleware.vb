Imports System.Text.Json
Imports Microsoft.AspNetCore.Http
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Logging

Namespace Middleware

    ' Formato de la respuesta de error que recibe el consumidor de la API
    Public Class ErrorRespuesta
        Public Property Estado As Integer
        Public Property Mensaje As String
    End Class

    ' Middleware global: captura TODAS las excepciones y devuelve el código HTTP adecuado
    ' sin exponer detalles internos (stack trace, mensajes de SQL, etc.)
    Public Class ManejoErroresMiddleware

        Private ReadOnly _siguiente As RequestDelegate
        Private ReadOnly _logger As ILogger(Of ManejoErroresMiddleware)

        Public Sub New(siguiente As RequestDelegate, logger As ILogger(Of ManejoErroresMiddleware))
            _siguiente = siguiente
            _logger = logger
        End Sub

        Public Async Function InvokeAsync(context As HttpContext) As Task
            Dim estado As Integer = 0
            Dim mensaje As String = Nothing

            Try
                Await _siguiente.Invoke(context)

            Catch ex As NotFoundException
                estado = StatusCodes.Status404NotFound
                mensaje = ex.Message

            Catch ex As ReglaNegocioException
                estado = StatusCodes.Status409Conflict
                mensaje = ex.Message

            Catch ex As DbUpdateException
                _logger.LogWarning(ex, "Error al guardar en la base de datos")
                estado = StatusCodes.Status409Conflict
                mensaje = "No se pudo guardar porque se viola una restricción de la base de datos."

            Catch ex As Exception
                ' El detalle queda solo en el log del servidor, NO se envía al cliente
                _logger.LogError(ex, "Error inesperado")
                estado = StatusCodes.Status500InternalServerError
                mensaje = "Ocurrió un error inesperado. Intente nuevamente más tarde."
            End Try

            If estado <> 0 AndAlso Not context.Response.HasStarted Then
                context.Response.StatusCode = estado
                context.Response.ContentType = "application/json"
                Dim opciones = New JsonSerializerOptions With {.PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
                Dim json = JsonSerializer.Serialize(New ErrorRespuesta With {.Estado = estado, .Mensaje = mensaje}, opciones)
                Await context.Response.WriteAsync(json)
            End If
        End Function

    End Class

End Namespace