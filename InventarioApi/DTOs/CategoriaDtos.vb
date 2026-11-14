Imports System.ComponentModel.DataAnnotations

Namespace DTOs

    ' DTO para REGISTRAR una categoría
    Public Class CategoriaCrearDto
        <Required(ErrorMessage:="El nombre de la categoría es obligatorio.")>
        <StringLength(50, ErrorMessage:="El nombre no puede superar los 50 caracteres.")>
        Public Property Nombre As String

        <StringLength(200, ErrorMessage:="La descripción no puede superar los 200 caracteres.")>
        Public Property Descripcion As String
    End Class

    ' DTO para MODIFICAR una categoría
    Public Class CategoriaActualizarDto
        <Required(ErrorMessage:="El nombre de la categoría es obligatorio.")>
        <StringLength(50, ErrorMessage:="El nombre no puede superar los 50 caracteres.")>
        Public Property Nombre As String

        <StringLength(200, ErrorMessage:="La descripción no puede superar los 200 caracteres.")>
        Public Property Descripcion As String
    End Class

    ' DTO para RESPONDER consultas de categorías
    Public Class CategoriaRespuestaDto
        Public Property Id As Integer
        Public Property Nombre As String
        Public Property Descripcion As String

        ' Solo se completa en "consultar categoría incluyendo sus productos"
        Public Property Productos As List(Of ProductoRespuestaDto)
    End Class

End Namespace