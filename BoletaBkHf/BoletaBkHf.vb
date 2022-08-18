Imports HEFSIILIBDTES
Imports System.IO
Imports System.Text
Imports System.Reflection.Assembly
Imports HEFSIILIBDTES.CONSULTAS

Module BoletaBkHf

    Dim _Sql As Class_SQL
    Dim Consulta_sql As String

    Dim _Global_BaseBk As String
    Dim _Empresa As String
    Dim _RutEmpresaActiva As String
    Dim _AmbienteCertificacion As Boolean
    Dim _Accion As Enum_Accion

    Enum Enum_Accion
        EnviarBoletaSII
        ConsultarTrackid
    End Enum

    Sub Main()

        Dim Cadena_ConexionSQL_Server As String
        Dim _Id_Dte As Integer
        Dim _Trackid As String

        If Environment.GetCommandLineArgs.Length > 1 Then

            Cadena_ConexionSQL_Server = Environment.GetCommandLineArgs(1)
            Cadena_ConexionSQL_Server = Replace(Cadena_ConexionSQL_Server, "@", " ")
            _Empresa = Environment.GetCommandLineArgs(2)
            _AmbienteCertificacion = Environment.GetCommandLineArgs(3)
            _Id_Dte = Environment.GetCommandLineArgs(4)
            _Trackid = Environment.GetCommandLineArgs(4)
            _Accion = Environment.GetCommandLineArgs(5)

        Else

            _Empresa = "01"
            'Cadena_ConexionSQL_Server = "data source = SIERRALTA; initial catalog = SIERRALTA_PRB; user id = SIERRALTA_PRB; password = SIERRALTA_PRB"
            Cadena_ConexionSQL_Server = "data source = 186.67.37.218,1518; initial catalog = SIERRALTA_PRB; user id = SIERRALTA_PRB; password = SIERRALTA_PRB"
            _AmbienteCertificacion = True
            _Id_Dte = 4
            _Trackid = "18494989"
            _Accion = Enum_Accion.ConsultarTrackid

        End If

        _Sql = New Class_SQL(Cadena_ConexionSQL_Server)

        _Global_BaseBk = _Sql.Fx_Trae_Dato("TABCARAC", "NOKOCARAC", "KOTABLA = 'BAKAPP'") & ".dbo."

        If Not String.IsNullOrEmpty(_Sql.Pro_Error) Then
            Console.WriteLine("Error Sql : {0}", _Sql.Pro_Error)
            Console.ReadKey()
            Return
        End If

        _Sql.Global_BaseBk = _Global_BaseBk

        Console.WriteLine("Cadena_ConexionSQL_Server : {0}", Cadena_ConexionSQL_Server)
        Console.WriteLine("Empresa : {0}", _Empresa)
        Console.WriteLine("Global_BaseBk : {0}", _Global_BaseBk)
        Console.ReadKey()

        Consulta_sql = "Select * From " & _Global_BaseBk & "Zw_Empresas Where Empresa = '" & _Empresa & "'"
        Dim _Row_Empresa As DataRow = _Sql.Fx_Get_DataRow(Consulta_sql)

        Dim _RutEmpresaActiva As String = _Row_Empresa.Item("Rut")

        If _Accion = Enum_Accion.EnviarBoletaSII Then

            Dim _HefRespuesta As New HefRespuesta
            _HefRespuesta = Fx_Enviar_Boleta_SII(_Id_Dte, _AmbienteCertificacion, _RutEmpresaActiva)

            Console.WriteLine(vbCrLf)
            Console.WriteLine(vbCrLf)

            If Not IsNothing(_HefRespuesta) Then

                Console.WriteLine("EsCorrecto : {0}", _HefRespuesta.EsCorrecto)
                Console.WriteLine("FchProceso : {0}", _HefRespuesta.FchProceso)
                Console.WriteLine("Mensaje : {0}", _HefRespuesta.Mensaje)
                Console.WriteLine("Proceso : {0}", _HefRespuesta.Proceso)
                Console.WriteLine("Resultado : {0}", _HefRespuesta.Resultado)
                Console.WriteLine("Trackid : {0}", _HefRespuesta.Trackid)
                'Console.WriteLine("XmlDocumento : {0}", _HefRespuesta.XmlDocumento)

            Else
                Console.WriteLine("No fue posible enviar el documento...")
            End If

        End If

        If _Accion = Enum_Accion.ConsultarTrackid Then

            Dim _HefRespuesta As New HefRespuesta
            _HefRespuesta = Fx_Consultar_Trackid(_Trackid, _AmbienteCertificacion)

            Console.WriteLine(vbCrLf)
            Console.WriteLine(vbCrLf)

            If Not IsNothing(_HefRespuesta) Then

                Console.WriteLine("EsCorrecto : {0}", _HefRespuesta.EsCorrecto)
                Console.WriteLine("FchProceso : {0}", _HefRespuesta.FchProceso)
                Console.WriteLine("Mensaje : {0}", _HefRespuesta.Mensaje)
                Console.WriteLine("Proceso : {0}", _HefRespuesta.Proceso)
                Console.WriteLine("Resultado : {0}", _HefRespuesta.Resultado)
                Console.WriteLine("Trackid : {0}", _HefRespuesta.Trackid)
                Console.WriteLine("XmlDocumento : {0}", _HefRespuesta.XmlDocumento)

            Else
                Console.WriteLine("No fue posible enviar el documento...")
            End If

        End If

        Console.ReadKey()

    End Sub

    Function Fx_Enviar_Boleta_SII(_Id_Dte As Integer,
                                  _AmbienteCertificacion As Boolean,
                                  _RutEmpresaActiva As String) As HefRespuesta

        Consulta_sql = "Select Id,Empresa,Campo,Valor,FechaMod,TipoCampo,TipoConfiguracion" & vbCrLf &
                       "From " & _Global_BaseBk & "Zw_DTE_Configuracion" & vbCrLf &
                       "Where Empresa = '" & _Empresa & "' And TipoConfiguracion = 'ConfEmpresa'"
        Dim _Tbl_ConfEmpresa As DataTable = _Sql.Fx_Get_Tablas(Consulta_sql)

        If Not CBool(_Tbl_ConfEmpresa.Rows.Count) Then
            'MessageBoxEx.Show(Me, "Faltan los datos de configuración DTE para la empresa", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return Nothing
        End If

        'Dim _Path As String = AppPath() & "\Data\Dte"
        Dim _AppPath = AppPath()

        Dim _HefRespuesta As New HefRespuesta
        Dim _HefPublicador As New HefPublicador

        Dim _xsd_path As String = _AppPath & "\Data\Dte\Schemas\BOLETAS\EnvioBOLETA_v11.xsd" '"herramientas\SCHEMAS\BOL\EnvioBOLETA_v11.xsd"
        Dim _cert_path As String = _AppPath & "\Data\Dte\Certificado\Certificado.pfx" '"herramientas\Certificado.pfx"
        Dim _cert_pass As String = "0984"
        Dim _RutEmisor As String = "79514800-0"
        Dim _Rutenvia As String = "12628844-1"
        Dim _FchResol As String = "2012-06-19"
        Dim _NroResol As Integer = 0
        Dim _Cn As String

        For Each _Fila As DataRow In _Tbl_ConfEmpresa.Rows

            Dim _Campo As String = _Fila.Item("Campo").ToString.Trim

            If _Campo = "RutEmisor" Then _RutEmisor = _Fila.Item("Valor")
            If _Campo = "RutEnvia" Then _Rutenvia = _Fila.Item("Valor")
            If _Campo = "FchResol" Then _FchResol = _Fila.Item("Valor")
            If _Campo = "NroResol" Then _NroResol = _Fila.Item("Valor")
            If _Campo = "Cn" Then _Cn = _Fila.Item("Valor")

        Next

        If _AmbienteCertificacion Then
            _NroResol = 0
        End If

        Dim _Certificado As Security.Cryptography.X509Certificates.X509Certificate2 = HEFSIILIBDTES.FUNCIONES.HefCertificados.RecuperarCertificado(_Cn)
        ' HEFSIILIBDTES.FUNCIONESHEFSIILIBDTES  HEFESTO.FIRMA.DOCUMENTO.FuncionesComunes.RecuperarCertificado(_Cn)

        Consulta_sql = "Select * From " & _Global_BaseBk & "Zw_DTE_Documentos Where Id_Dte = " & _Id_Dte
        Dim _Zw_DTE_Documentos As DataRow = _Sql.Fx_Get_DataRow(Consulta_sql)

        Dim _Idmaeedo As Integer = _Zw_DTE_Documentos.Item("Idmaeedo")
        Dim _Xml As String = _Zw_DTE_Documentos.Item("Xml")

        Dim _Ruta = _AppPath & "\Data\" & _RutEmpresaActiva & "\DTE\Documentos\Boleta\"

        Fx_CrearArchivo(_Ruta, "SET_ENVIO_BOLETA.xml", _Xml)
        'Fx_CrearArchivo(_AppPath & "\Data\" & _RutEmpresaActiva & "\DTE\Documentos\Boleta\", "SET_ENVIO_BOLETA.xml", _Xml)

        Dim _mis_boletas As List(Of String) = New List(Of String)()
        Dim path_boleta As String = _Ruta & "\SET_ENVIO_BOLETA.xml"

        Dim content As String = File.ReadAllText(path_boleta, Encoding.GetEncoding("ISO-8859-1"))
        _mis_boletas.Add(content)

        _HefRespuesta = _HefPublicador.PublicadoBoletasPorLotes3(_RutEmisor,
                                                                 _Rutenvia,
                                                                 _FchResol,
                                                                 _NroResol,
                                                                 _Certificado,
                                                                 _mis_boletas,
                                                                 _xsd_path)

        Dim _EsCorrecto = _HefRespuesta.EsCorrecto
        Dim _FchProceso = _HefRespuesta.FchProceso
        Dim _Mensaje = _HefRespuesta.Mensaje
        Dim _Proceso = _HefRespuesta.Proceso
        Dim _Resultado = _HefRespuesta.Resultado
        Dim _Trackid = _HefRespuesta.Trackid
        Dim _XmlDocumento = _HefRespuesta.XmlDocumento

        If _HefRespuesta.EsCorrecto Then

            Dim _Id_Trackid As Integer

            Consulta_sql = "Insert Into " & _Global_BaseBk & "Zw_DTE_Trackid (Id_Dte,Idmaeedo,Trackid,FechaEnvSII,Procesar,AmbienteCertificacion) Values " & vbCrLf &
                           "(" & _Id_Dte & "," & _Idmaeedo & ",'" & _Trackid & "',Getdate(),1," & Convert.ToInt32(_AmbienteCertificacion) & ")"
            _Sql.Ej_Insertar_Trae_Identity(Consulta_sql, _Id_Trackid, False)

            Consulta_sql = "Update " & _Global_BaseBk & "Zw_DTE_Documentos Set Procesado = 1 Where Id_Dte =  " & _Id_Dte
            _Sql.Ej_consulta_IDU(Consulta_sql, False)

        End If

        Return _HefRespuesta

    End Function

    Function Fx_Consultar_Trackid(_Trackid As String, _AmbienteCertificacion As Boolean) As HefRespuesta

        Dim HefRespuesta As New HefRespuesta
        Dim HefConsultas As New HefConsultas
        Dim _Ambiente As AmbienteSII

        Consulta_sql = "Select Id,Empresa,Campo,Valor,FechaMod,TipoCampo,TipoConfiguracion" & vbCrLf &
                       "From " & _Global_BaseBk & "Zw_DTE_Configuracion" & vbCrLf &
                       "Where Empresa = '" & _Empresa & "' And TipoConfiguracion = 'ConfEmpresa'"
        Dim _Tbl_ConfEmpresa As DataTable = _Sql.Fx_Get_Tablas(Consulta_sql)

        Dim _RutEmisor As String
        Dim _Cn As String

        For Each _Fila As DataRow In _Tbl_ConfEmpresa.Rows

            Dim _Campo As String = _Fila.Item("Campo").ToString.Trim

            If _Campo = "RutEmisor" Then _RutEmisor = _Fila.Item("Valor")
            If _Campo = "Cn" Then _Cn = _Fila.Item("Valor")

        Next

        If _AmbienteCertificacion Then
            _Ambiente = AmbienteSII.Certificacion
        Else
            _Ambiente = AmbienteSII.Produccion
        End If

        HefRespuesta = HefConsultas.EstadoBolTrackid(_RutEmisor, _Trackid, _Cn, _Ambiente)

        Return HefRespuesta

        '    HefRespuesta resp = HefConsultas.EstadoBolTrackid(
        '"79514800-0",
        '    "18488133",
        '        "JUAN PABLO SIERRALTA OREZZOLI",
        '            AmbienteSII.Certificacion);

        '        ////
        '        //// Notificar
        '        Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
        '        Console.WriteLine("Proceso   : " + resp.Proceso);
        '        Console.WriteLine("Mensaje   : " + resp.Mensaje);
        '        Console.WriteLine("Detalle   : " + resp.Detalle);
        '        Console.WriteLine("Trackid   : " + resp.Trackid);
        '        Console.WriteLine("========================================================");
        '        Console.WriteLine(resp.XmlDocumento);

    End Function

    Public Function AppPath(Optional backSlash As Boolean = False) As String

        Dim s As String = IO.Path.GetDirectoryName(GetExecutingAssembly.GetCallingAssembly.Location)

        If backSlash Then
            s &= "\"
        End If

        ' si hay que añadirle el backslash
        Return s

    End Function

    Function Fx_CrearArchivo(Ruta As String,
                             NombreArchivo As String,
                             Cuerpo As String)
        Try

            Dim _Palo = Right(Ruta, 1)

            If _Palo <> "\" Then
                Ruta += "\"
            End If

            Dim RutaArchivo As String = Ruta & NombreArchivo

            Dim oSW As New System.IO.StreamWriter(RutaArchivo)

            oSW.WriteLine(Cuerpo)
            oSW.Close()

            'aqui creo el archivo oculto,,, si no se pone este instrucion no pasa nada .. solo es para 
            'asignarles caracteristicas a los archivos 
            'quitalo como comentario y calalo
            'SetAttr(RutaArchivo, vbHidden)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Function

End Module
