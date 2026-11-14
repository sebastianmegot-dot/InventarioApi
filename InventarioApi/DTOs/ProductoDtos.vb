Imports System.ComponentModel.DataAnnotations

Namespace DTOs

    ' DTO para REGISTRAR un producto
    Public Class ProductoCrearDto
        <Required(ErrorMessage:="El nombre del producto es obligatorio.")>
        <StringLength(50, ErrorMessage:="El nombre no puede superar los 50 caracteres.")>
        Public Property Nombre As String

        <StringLength(200, ErrorMessage:="La descripción no puede superar los 200 caracteres.")>
        Public Property Descripcion As String

        <Range(0.01, Double.MaxValue, ErrorMessage:="El precio debe ser mayor que cero.")>
        Public Property Precio As Decimal

        <Range(0, Integer.MaxValue, ErrorMessage:="El stock no puede ser negativo.")>
        Public Property Stock As Integer

        <Range(1, Integer.MaxValue, ErrorMessage:="La categoría es obligatoria.")>
        Public Property CategoriaId As Integer
    End Class

    ' DTO para MODIFICAR un producto
    Public Class ProductoActualizarDto
        <Required(ErrorMessage:="El nombre del producto es obligatorio.")>
        <StringLength(50, ErrorMessage:="El nombre no puede superar los 50 caracteres.")>
        Public Property Nombre As String

        <StringLength(200, ErrorMessage:="La descripción no puede superar los 200 caracteres.")>
        Public Property Descripcion As String

        <Range(0.01, Double.MaxValue, ErrorMessage:="El precio debe ser mayor que cero.")>
        Public Property Precio As Decimal

        <Range(0, Integer.MaxValue, ErrorMessage:="El stock no puede ser negativo.")>
        Public Property Stock As Integer

        <Range(1, Integer.MaxValue, ErrorMessage:="La categoría es obligatoria.")>
        Public Property CategoriaId As Integer
    End Class

    ' DTO para RESPONDER consultas de productos (incluye datos de la categoría)
    Public Class ProductoRespuestaDto
        Public Property Id As Integer
        Public Property Nombre As String
        Public Property Descripcion As String
        Public Property Precio As Decimal
        Public Property Stock As Integer
        Public Property FechaRegistro As DateTime
        Public Property CategoriaId As Integer
        Public Property CategoriaNombre As String
    End Class

End Namespace