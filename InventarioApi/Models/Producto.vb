Namespace Models

    ' Entidad Producto: cada producto pertenece a una categoría
    Public Class Producto
        Public Property Id As Integer
        Public Property Nombre As String = String.Empty
        Public Property Descripcion As String
        Public Property Precio As Decimal
        Public Property Stock As Integer
        Public Property FechaRegistro As DateTime

        ' Clave foránea hacia Categoria
        Public Property CategoriaId As Integer

        ' Propiedad de navegación hacia la categoría
        Public Property Categoria As Categoria
    End Class

End Namespace