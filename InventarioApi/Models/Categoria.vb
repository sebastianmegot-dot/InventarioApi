Namespace Models

    ' Entidad Categoria: una categoría puede tener muchos productos (relación 1:N)
    Public Class Categoria
        Public Property Id As Integer
        Public Property Nombre As String = String.Empty
        Public Property Descripcion As String

        ' Propiedad de navegación: lista de productos de esta categoría
        Public Property Productos As New List(Of Producto)
    End Class

End Namespace