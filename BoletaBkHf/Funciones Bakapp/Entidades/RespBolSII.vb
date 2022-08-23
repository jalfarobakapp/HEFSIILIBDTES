Public Class DetalleRepRech
    Public Property tipo As Integer
    Public Property folio As Integer
    Public Property estado As String
    Public Property descripcion As String
    Public Property [error] As List(Of [Error])
End Class

Public Class [Error]
    Public Property seccion As String
    Public Property linea As Integer
    Public Property nivel As Integer
    Public Property codigo As Integer
    Public Property descripcion As String
    Public Property detalle As String
End Class

Public Class Estadistica
    Public Property tipo As Integer
    Public Property informados As Integer
    Public Property aceptados As Integer
    Public Property rechazados As Integer
    Public Property reparos As Integer
End Class

Public Class RespBolSII
    Public Property rut_emisor As String
    Public Property rut_envia As String
    Public Property trackid As String
    Public Property fecha_recepcion As String
    Public Property estado As String
    Public Property estadistica As List(Of Estadistica)
    Public Property detalle_rep_rech As List(Of DetalleRepRech)
End Class