Namespace Middleware

    ' Se lanza cuando un producto o categoría no existe → HTTP 404
    Public Class NotFoundException
        Inherits Exception
        Public Sub New(mensaje As String)
            MyBase.New(mensaje)
        End Sub
    End Class

    ' Se lanza cuando se viola una regla de negocio → HTTP 409
    Public Class ReglaNegocioException
        Inherits Exception
        Public Sub New(mensaje As String)
            MyBase.New(mensaje)
        End Sub
    End Class

End Namespace