Imports System.Data.SqlClient
Imports System.IO

Public Class Class_SQL

    Dim _SQL_String_conexion As String
    Dim _Cn As New SqlConnection
    Dim _Error As String
    Dim _Global_BaseBk As String

    Public Property Pro_Error As String
        Get
            Return _Error
        End Get
        Set(value As String)
            _Error = value
        End Set
    End Property

    Public Property Global_BaseBk As String
        Get
            Return _Global_BaseBk
        End Get
        Set(value As String)
            _Global_BaseBk = value
        End Set
    End Property

    Public Sub New(SQL_String_conexion As String)
        _SQL_String_conexion = SQL_String_conexion
    End Sub

    Function Ej_consulta_IDU(ConsultaSql As String,
                            Optional MostrarError As Boolean = True) As Boolean
        Try
            'Abrimos la conexión con la base de datos

            Sb_Abrir_Conexion(_Cn)
            'System.Windows.Forms.Application.DoEvents()
            Dim cmd As System.Data.SqlClient.SqlCommand
            cmd = New System.Data.SqlClient.SqlCommand()
            cmd.CommandType = CommandType.Text
            cmd.CommandText = ConsultaSql
            cmd.CommandTimeout = 0
            cmd.Connection = _Cn

            cmd.ExecuteNonQuery()
            'Cerramos la conexión con la base de datos

            Sb_Cerrar_Conexion(_Cn)

            'Application.DoEvents()
            Return True
        Catch ex As Exception
            _Error = ex.Message
            If MostrarError = True Then
                'MsgBox("No se realizo la operación: Insert, Update o Delete..." _
                ', 'MsgBoxStyle.Critical, "Modificar tabla")
                'MsgBox(ex.Message)
            End If
            Return False
        End Try

    End Function

    Function Ej_Insertar_Trae_Identity(ConsultaSql As String,
                                       ByRef _Identity As Integer,
                                       Optional MostrarError As Boolean = True) As Boolean
        Try
            'Abrimos la conexión con la base de datos

            Sb_Abrir_Conexion(_Cn)
            'System.Windows.Forms.Application.DoEvents()
            Dim cmd As System.Data.SqlClient.SqlCommand
            cmd = New System.Data.SqlClient.SqlCommand()
            cmd.CommandType = CommandType.Text
            cmd.CommandText = ConsultaSql
            cmd.CommandTimeout = 0
            cmd.Connection = _Cn

            cmd.ExecuteNonQuery()
            'Cerramos la conexión con la base de datos

            cmd = New SqlCommand("SELECT @@IDENTITY AS 'Identity'", _Cn)

            Dim dfd1 As SqlDataReader = cmd.ExecuteReader()
            While dfd1.Read()
                _Identity = dfd1("Identity")
            End While
            dfd1.Close()

            Sb_Cerrar_Conexion(_Cn)

            'System.Windows.Forms.Application.DoEvents()
            Return True
        Catch ex As Exception
            _Error = ex.Message
            If MostrarError = True Then
                'MsgBox("No se realizo la operación: Insert, Update o Delete..." _
                ', 'MsgBoxStyle.Critical, "Modificar tabla")
                'MsgBox(ex.Message)
            End If
            Return False
        End Try

    End Function

    Function Fx_Get_Tablas(_Consulta_sql As String,
                           Optional _Mostrar_Error As Boolean = True) As DataTable

        Dim _Tbl As New DataTable

        Try

            Sb_Abrir_Conexion(_Cn)

            If Not String.IsNullOrEmpty(_Error) Then
                _Mostrar_Error = False
                Throw New System.Exception(_Error)
            End If

            Dim _SqlDa As New SqlDataAdapter

            _SqlDa = New SqlDataAdapter(_Consulta_sql, _Cn)
            _SqlDa.SelectCommand.CommandTimeout = 8000

            'If _Traer_Schema Then _SqlDa.MissingSchemaAction = MissingSchemaAction.AddWithKey

            _SqlDa.Fill(_Tbl)

            For Each col As DataColumn In _Tbl.Columns
                col.[ReadOnly] = False
            Next

            Sb_Cerrar_Conexion(_Cn)

            ' retornar el dataTable
            Return _Tbl

            ' errores
        Catch ex As Exception
            'If _Mostrar_Error Then 'MsgBox(ex.Message.ToString)
            _Error = ex.Message
        End Try

        Return Nothing

    End Function

    Function Fx_Get_DataRow(_Consulta_sql As String,
                            Optional _Mostrar_Error As Boolean = True) As DataRow

        Try

            Dim _Tbl As DataTable = Fx_Get_Tablas(_Consulta_sql, _Mostrar_Error)

            If CBool(_Tbl.Rows.Count) Then
                Return _Tbl.Rows(0)
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Function Fx_Get_DataSet(_Consulta_sql As String,
                            _Ds As DataSet,
                            _Nombre_Tabla As String) As DataSet

        Dim _Tbl As New DataTable

        Try
            Sb_Abrir_Conexion(_Cn)

            Dim daAuthors As New SqlDataAdapter(_Consulta_sql, _Cn)
            daAuthors.SelectCommand.CommandTimeout = 8000

            daAuthors.Fill(_Ds, _Nombre_Tabla)
            Sb_Cerrar_Conexion(_Cn)

            For Each Tbl As DataTable In _Ds.Tables
                For Each col As DataColumn In Tbl.Columns
                    col.[ReadOnly] = False
                Next
            Next

            ' retornar el dataTable
            Return _Ds

            ' errores
        Catch ex As Exception
            _Error = String.Empty
            'MsgBox(ex.Message.ToString)
        End Try
        Return Nothing

    End Function

    Function Fx_Get_DataSet(Consulta_sql As String,
                            Optional _Traer_Schema As Boolean = True) As DataSet

        Try

            Sb_Abrir_Conexion(_Cn)

            Dim _SqlDa As New SqlDataAdapter
            Dim _DataSt As New DataSet

            _SqlDa = New SqlDataAdapter(Consulta_sql, _Cn)
            _SqlDa.SelectCommand.CommandTimeout = 8000

            If _Traer_Schema Then _SqlDa.MissingSchemaAction = MissingSchemaAction.AddWithKey

            _SqlDa.Fill(_DataSt)

            For Each Tbl As DataTable In _DataSt.Tables
                For Each col As DataColumn In Tbl.Columns
                    col.[ReadOnly] = False
                Next
            Next

            Sb_Cerrar_Conexion(_Cn)

            Return _DataSt
            ' errores
        Catch ex As Exception
            _Error = String.Empty
            'MsgBox(ex.Message.ToString)
        End Try
        Return Nothing

    End Function

    Function Fx_Extrae_Archivo_desde_BD(_Tabla As String,
                                        _Campo As String,
                                        _Condicion As String,
                                        _Nom_Archivo As String,
                                        _Dir_Temp As String) As Boolean

        Dim data As Byte() = Nothing

        Try
            ' Construimos los correspondientes objetos para
            ' conectarnos a la base de datos de SQL Server,
            ' utilizando la seguridad integrada de Windows NT.
            '
            Using cn As New SqlConnection
                Dim sCnn = _SQL_String_conexion
                cn.ConnectionString = sCnn
                Dim cmd As SqlCommand = cn.CreateCommand 'cnn.CreateCommand()
                ' Seleccionamos únicamente el campo que contiene
                ' los documentos, filtrándolo mediante su
                ' correspondiente campo identificador.
                '
                cmd.CommandText = "SELECT " & _Campo & " From " & _Tabla & " WHERE " & _Condicion
                ' Abrimos la conexión.
                cn.Open()
                ' Creamos un DataReader.
                Dim dr As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
                cmd.CommandTimeout = 8000
                ' Leemos el registro.
                dr.Read()
                ' El tamaño del búfer debe ser el adecuado para poder
                ' escribir en el archivo todos los datos leídos.
                '
                ' Si el parámetro 'buffer' lo pasamos como Nothing, obtendremos
                ' la longitud del campo en bytes.
                '
                Dim bufferSize As Integer = Convert.ToInt32(dr.GetBytes(0, 0, Nothing, 0, 0))

                ' Creamos el array de bytes. Como el índice está
                ' basado en cero, la longitud del array será la
                ' longitud del campo menos una unidad.
                '
                data = New Byte(bufferSize - 1) {}

                ' Leemos el campo.
                '
                dr.GetBytes(0, 0, data, 0, bufferSize)

                ' Cerramos el objeto DataReader, e implícitamente la conexión.
                '
                dr.Close()

            End Using

            ' Procedemos a crear el archivo, en el ejemplo
            ' un documento de Microsoft Word.
            '
            If File.Exists(_Dir_Temp & "\" & _Nom_Archivo) Then File.Delete(_Dir_Temp & "\" & _Nom_Archivo)

            Using fs As New IO.FileStream(_Dir_Temp & "\" & _Nom_Archivo, IO.FileMode.CreateNew, IO.FileAccess.Write)

                ' Crea el escritor para la secuencia.
                Dim bw As New IO.BinaryWriter(fs)

                ' Escribir los datos en la secuencia.
                bw.Write(data)

            End Using


            'Sb_WriteBinaryFile(Me, _Dir_Temp & "\" & _Nom_Archivo, data)
            Return True
        Catch ex As Exception
            _Error = String.Empty
            'MessageBox.Show(ex.Message)
        End Try

    End Function

    'System.Windows.Forms.Application.DoEvents()
    Sub Sb_Abrir_Conexion(_Cn As SqlConnection)

        _Error = String.Empty

        Try
            If _Cn.State = ConnectionState.Open Then
                ' Cerrar conexion
                _Cn.Close()
            End If

            _Cn.ConnectionString = _SQL_String_conexion
            _Cn.Open()
            ''MsgBox(sCnn)

        Catch ex As SqlClient.SqlException 'Exception
            _Error = ex.Message & vbCrLf & "Cadena de conexión: " & _Cn.ConnectionString
            ''MsgBox(ex.Message)
            ''MessageBox.Show("ERROR al conectar o recuperar los datos:" & vbCrLf & _
            '                ex.Message, "Conectar con la base", _
            '                'MessageBoxButtons.OK, 'MessageBoxIcon.Exclamation)
        End Try

    End Sub

    Sub Sb_Cerrar_Conexion(_Cn As SqlConnection)
        Try
            If _Cn.State = ConnectionState.Open Then
                '' Cerrar conexion
                _Cn.Close()
            End If
        Catch ex As Exception
            _Error = String.Empty
            'MsgBox(ex.Message)
        End Try
    End Sub

    Sub Sb_Cerrar_Conexion2()
        Try
            If _Cn.State = ConnectionState.Open Then
                '' Cerrar conexion
                _Cn.Close()
            End If
        Catch ex As Exception
            _Error = String.Empty
            'MsgBox(ex.Message)
        End Try
    End Sub

    Function Fx_Eje_Condulta_Insert_Update_Delte_TRANSACCION(Consulta_sql As String) As Boolean

        Dim myTrans As SqlClient.SqlTransaction
        Dim Comando As SqlClient.SqlCommand

        Dim _Fijar As Boolean
        Dim _Cn As New SqlConnection

        Try


            Sb_Abrir_Conexion(_Cn)
            myTrans = _Cn.BeginTransaction()


            Comando = New SqlClient.SqlCommand(Consulta_sql, _Cn)
            Comando.Transaction = myTrans
            Comando.ExecuteNonQuery()

            '**********************************'**********************************

            myTrans.Commit()
            Sb_Cerrar_Conexion(_Cn)

            _Fijar = True
            Return _Fijar

        Catch ex As Exception
            _Error = ex.Message
            ''MsgBox(ex.Message)
        Finally

            If Not _Fijar Then
                myTrans.Rollback()
            End If

        End Try

    End Function

    ''' <summary>
    ''' Devuelve un valor de un campo en especifico de cualquier base de datos, tanto de Random como Bakapp
    ''' </summary>
    ''' <param name="_Tabla">Nombre de tabla</param>
    ''' <param name="_Campo">Nombre del campo a traer</param>
    ''' <param name="_Condicion"></param>
    ''' <param name="_DevNumero">Si devuelve un numero True/False</param>
    ''' <param name="_MostrarError">Si muestra o no el mensaje de error True/False</param>
    ''' <param name="_Dato_Default">Dato que se enviara por defecto si no se encuentra el valor o es nulo</param>
    ''' <param name="_Es_Boolean">Si el campo que devuelve es verdadero o falso... True/False</param>
    ''' <returns></returns>
    Function Fx_Trae_Dato(_Tabla As String,
                         _Campo As String,
                         Optional _Condicion As String = "",
                         Optional _DevNumero As Boolean = False,
                         Optional _MostrarError As Boolean = True,
                         Optional _Dato_Default As String = "",
                         Optional _Es_Boolean As Boolean = False) As String
        Try

            Dim _Valor

            If Not String.IsNullOrEmpty(_Condicion) Then
                _Condicion = vbCrLf & "And " & _Condicion
            End If

            If _DevNumero And String.IsNullOrEmpty(_Dato_Default) Then
                _Dato_Default = 0
            End If


            Dim _Sql As String = "SELECT TOP (1) " & _Campo & " AS CAMPO FROM " & _Tabla & vbCrLf &
                                 "Where 1 > 0" & _Condicion


            Dim _Tbl As DataTable = Fx_Get_Tablas(_Sql, _MostrarError)

            If Not String.IsNullOrEmpty(_Error) Then
                Return _Error
            End If

            Dim cuenta As Long = _Tbl.Rows.Count

            If CBool(_Tbl.Rows.Count) Then

                _Valor = _Tbl.Rows(0).Item("CAMPO")

                If _Es_Boolean Then
                    _Valor = CBool(_Valor)
                Else
                    If _DevNumero Then
                        _Valor = NuloPorNro(_Valor, 0)
                    Else
                        _Valor = NuloPorNro(_Valor, "")
                    End If
                End If

            Else
                If _Es_Boolean Then
                    _Valor = False
                Else
                    If _DevNumero Then
                        _Valor = 0
                    Else
                        _Valor = String.Empty
                    End If
                End If
            End If

            If String.IsNullOrEmpty(_Valor) Then
                _Valor = _Dato_Default
            End If

            Return _Valor

        Catch ex As Exception

            _Error = String.Empty

            If _MostrarError Then
                'MsgBox(ex.Message, 'MsgBoxStyle.Critical, "Error!!")
            Else

                If _MostrarError Then
                    Return ex.Message
                Else
                    If _DevNumero Then
                        Return 0
                    Else
                        Return String.Empty
                    End If
                End If

            End If

        End Try

    End Function

    Function Fx_Cuenta_Registros(_Tabla As String,
                                 Optional _Condicion As String = "") As Double

        If Not String.IsNullOrEmpty(_Condicion) Then
            _Condicion = vbCrLf & "And " & _Condicion
        End If

        Dim _Sql As String = "Select Count(*) As Cuenta From " & _Tabla & " Where 1 > 0 " & _Condicion

        Dim _RowTabpre As DataRow = Fx_Get_DataRow(_Sql)

        Dim _Cuenta As Double

        If (_RowTabpre Is Nothing) Then
            _Cuenta = 0
        Else
            _Cuenta = _RowTabpre.Item("Cuenta")
        End If

        Return _Cuenta

    End Function

    Function Fx_SqlDataReader(Consulta_sql As String) As SqlDataReader

        Sb_Abrir_Conexion(_Cn)
        Dim _Comando As SqlClient.SqlCommand

        _Comando = New SqlCommand(Consulta_sql, _Cn)
        Dim _Sql_DReader As SqlDataReader = _Comando.ExecuteReader()

        'Sb_Cerrar_Conexion(_Cn)

        Return _Sql_DReader

    End Function

    Sub Sb_Eliminar_Tabla_De_Paso(_Tabla_Paso As String)

        Dim _ConsultaSql As String = "BEGIN TRY" & vbCrLf &
                                     "DROP TABLE " & _Tabla_Paso & vbCrLf &
                                     "End Try" & vbCrLf &
                                     "BEGIN CATCH" & vbCrLf &
                                     "END CATCH"

        Ej_consulta_IDU(_ConsultaSql, False)

    End Sub

    Enum Enum_Type
        _String
        _Double
        _Boolean
        _Date
        _ComboBox
    End Enum


    Function Fx_Existe_Tabla(_Tabla As String) As Boolean

        Dim _ConsultaSql As String

        If _Tabla.Contains(_Global_BaseBk) Then

            _Tabla = Replace(_Tabla, _Global_BaseBk, "")
            _ConsultaSql = "USE " & Replace(_Global_BaseBk, ".dbo.", "") & "
                            SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & _Tabla & "'"

        Else

            _ConsultaSql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" & _Tabla & "'"

        End If

        Dim _Tbl As DataTable = Fx_Get_Tablas(_ConsultaSql)

        Return _Tbl.Rows.Count

    End Function

    Function Fx_Exite_Campo(_Tabla As String, _Campo As String) As Boolean

        Dim _ConsultaSql As String

        If _Tabla.Contains(_Global_BaseBk) Then

            _Tabla = Replace(_Tabla, _Global_BaseBk, "")
            _ConsultaSql = "USE " & Replace(_Global_BaseBk, ".dbo.", "") & "
                            SELECT * FROM INFORMATION_SCHEMA.COLUMNS" & vbCrLf &
                            "WHERE COLUMN_NAME = '" & _Campo & "' AND TABLE_NAME = '" & _Tabla & "'"

        Else

            _ConsultaSql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS" & vbCrLf &
                                   "WHERE COLUMN_NAME = '" & _Campo & "' AND TABLE_NAME = '" & _Tabla & "'"

        End If

        Dim _Tbl As DataTable = Fx_Get_Tablas(_ConsultaSql)

        Return CBool(_Tbl.Rows.Count)

    End Function

    Function Fx_Exite_Indice(_Indice As String) As Boolean

        Dim _ConsultaSql As String

        _ConsultaSql = "SELECT Top 1 FROM sys.indexes 
                        WHERE name = '" & _Indice & "' 
                        --AND object_id = OBJECT_ID('TABLENAME') "
        Dim _Row_Indice As DataRow = Fx_Get_DataRow(_ConsultaSql, False)

        If Not IsNothing(_Row_Indice) Then
            Return True
        End If

    End Function

    Function Fx_Agregar_Campo_En_Tabla(_Tabla As String,
                                       _Campo As String,
                                       _Tipo As String,
                                       _Largo As Integer,
                                       _Null As Boolean)

        Dim _Nulo As String

        If _Null Then
            _Nulo = "NULL"
        Else
            _Nulo = "Not Null"
        End If

        Dim _ConsultaSql = "ALTER TABLE dbo.doc_exa ADD column_b VARCHAR(20) NULL"

    End Function

    Function ActualizarGrillaUpInsDel(Consulta_Sql As String,
                                      Grilla As Object,
                                      Optional EsGrilla As Boolean = True)
        Dim Cn As SqlConnection

        Try

            'Abrimos la conexión con SQL
            Sb_Abrir_Conexion(Cn)

            ' Referenciamos el objeto DataTable enlazado
            ' con el control DataGridView.

            Dim dt As DataTable

            If EsGrilla Then
                dt = DirectCast(Grilla.DataSource, DataTable)
            Else
                dt = Grilla
            End If

            ' Procedemos a actualizar la base de datos.
            Dim n As Integer = UpInsDelDatabase(dt, Consulta_Sql, Cn)
            Sb_Cerrar_Conexion(Cn)
            Return n
            ''MessageBox.Show("Nº de registros afectados: " & CStr(n))
        Catch ex As Exception
            ' Se ha producido un error
            Sb_Cerrar_Conexion(Cn)
            'MessageBox.Show(ex.Message)

        End Try
    End Function

    Private Function UpInsDelDatabase(dt As DataTable,
                              sql As String,
                              cn As SqlConnection) As Integer

        ' Si el valor del objeto DataTable es Nothing, provocamos
        ' una excepción.
        '
        If (dt Is Nothing) Then _
            Throw New ArgumentNullException()

        Try

            Dim da As New SqlDataAdapter(sql, cn)

            ' Creamos un objeto CommandBuilder para configurar los comandos
            ' apropiados del adaptador de datos. Se requiere que la tabla
            ' de la base de datos tenga establecida su correspondiente
            ' clave principal.
            '
            Dim cb As New SqlCommandBuilder(da)

            With da
                .InsertCommand = cb.GetInsertCommand()
                .DeleteCommand = cb.GetDeleteCommand()
                .UpdateCommand = cb.GetUpdateCommand()
            End With

            ' Procedemos a actualizar la base de datos, devolviendo
            ' el número de registros afectados.
            '
            Return da.Update(dt)

            'End Using

        Catch ex As Exception
            Throw
            'MsgBox(ex.Message)
        End Try

    End Function

    Function Fx_Ejecutar_Consulta(Consulta_sql As String) As String
        Try
            Dim _Cn As New SqlConnection
            Dim Comando As SqlCommand
            Sb_Abrir_Conexion(_Cn)
            Comando = New SqlCommand(Consulta_sql, _Cn)
            Comando.ExecuteNonQuery()
            Sb_Cerrar_Conexion(_Cn)
            Return "OK"
        Catch ex As Exception
            Sb_Cerrar_Conexion(_Cn)
            Return ex.Message
        End Try
    End Function
    Function Fx_Obtener_Valores(Consulta As String, Cantidad As Integer)
        Try
            Dim Comando As New SqlCommand
            Dim Lector As SqlDataReader
            Dim CantidadQuery As Integer = Cantidad - 1
            Dim Respuesta(CantidadQuery) As String
            Sb_Abrir_Conexion(_Cn)
            Comando = New SqlCommand(Consulta, _Cn)
            Lector = Comando.ExecuteReader
            Do While Lector.Read
                For i = 0 To CantidadQuery
                    Respuesta(i) = Lector(i)
                Next
            Loop
            Lector.Close()
            Sb_Cerrar_Conexion(_Cn)
            Return Respuesta
        Catch ex As Exception
            Sb_Cerrar_Conexion(_Cn)

        End Try
    End Function

    Public Function NuloPorNro(Of T)(value As T, defaultValue As T) As T

        Dim obj1 As Object = value
        Dim obj2 As Object = defaultValue

        Try
            If ((obj1 Is DBNull.Value) OrElse (obj1 Is Nothing)) Then
                ' Es NULL; devolvemos el valor por defecto siempre
                ' y cuando éste tampoco sea NULL.
                '
                If (Not obj2 Is DBNull.Value) Then
                    Return defaultValue
                Else
                    Return Nothing
                End If
            Else
                ' No es NULL ni Nothing; devolvemos el valor pasado.
                '
                Return value
            End If
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

End Class
