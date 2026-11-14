Imports InventarioApi.Models
Imports InventarioApi.DTOs

Namespace Services

    Public Module Mapeador

        Public Function AProductoDto(p As Producto, Optional nombreCategoria As String = Nothing) As ProductoRespuestaDto
            Return New ProductoRespuestaDto With {
                .Id = p.Id,
                .Nombre = p.Nombre,
                .Descripcion = p.Descripcion,
                .Precio = p.Precio,
                .Stock = p.Stock,
                .FechaRegistro = p.FechaRegistro,
                .CategoriaId = p.CategoriaId,
                .CategoriaNombre = If(nombreCategoria, p.Categoria?.Nombre)
            }
        End Function

        Public Function ACategoriaDto(c As Categoria, Optional incluirProductos As Boolean = False) As CategoriaRespuestaDto
            Dim dto = New CategoriaRespuestaDto With {
                .Id = c.Id,
                .Nombre = c.Nombre,
                .Descripcion = c.Descripcion
            }
            If incluirProductos Then
                dto.Productos = c.Productos.Select(Function(p) AProductoDto(p, c.Nombre)).ToList()
            End If
            Return dto
        End Function

    End Module

End Namespace