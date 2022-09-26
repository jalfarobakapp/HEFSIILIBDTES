Imports System.IO
Imports System.IO.Compression
Imports System.Reflection.Assembly
Imports System.Text
Imports HEFSIILIBDTES
Imports HEFSIILIBDTES.CONSULTAS
Imports Newtonsoft.Json

Module BoletaBkHf

    Dim _Sql As Class_SQL
    Dim Consulta_sql As String

    Dim _Global_BaseBk As String
    Dim _Empresa As String
    Dim _RutEmpresaActiva As String
    Dim _AmbienteCertificacion As Boolean
    Dim _Accion As Enum_Accion

    Dim _AppPath As String
    Dim _xsd_path As String

    Enum Enum_Accion
        EnviarBoletaSII
        ConsultarTrackid
        'CrearDirectorios
    End Enum

    Sub Main()

        Dim _AppPath As String = AppPath()
        _xsd_path = _AppPath & "\Dtes\Dte\Schemas\BOLETAS\EnvioBOLETA_v11.xsd"

        Dim Cadena_ConexionSQL_Server As String
        Dim _Id_Dte As Integer
        Dim _Trackid As String

        Cadena_ConexionSQL_Server = System.Configuration.ConfigurationSettings.AppSettings.Item("Cadenadeconexion")
        '_Empresa = System.Configuration.ConfigurationSettings.AppSettings.Item("Empresa")

        If String.IsNullOrEmpty(Cadena_ConexionSQL_Server) Then
            Console.WriteLine("Error!!")
            Console.WriteLine("Falta la cadena de conexión en el archivo de configuración, [Cadenadeconexion]")
            Console.WriteLine("Archivo BoletaBkHf.exe.config.xml")
            Console.ReadKey()
            Return
        End If

        'If String.IsNullOrEmpty(_Empresa) Then
        '    Console.WriteLine("Error!!")
        '    Console.WriteLine("Falta la empresa por defecto archivo de configuración, [Empresa]")
        '    Console.WriteLine("Archivo BoletaBkHf.exe.config.xml")
        '    Console.ReadKey()
        '    Return
        'End If

        If Not File.Exists(_xsd_path) Then
            Console.WriteLine("Error!!")
            Console.WriteLine("No existe el archivo {0}", _xsd_path)
            Console.ReadKey()
            Return
        End If

        Dim _Version_BakApp = FileVersionInfo.GetVersionInfo(_AppPath & "\BoletaBkHf.exe").FileVersion

        Console.WriteLine("BoletaBkHf.exe Versión: " & _Version_BakApp)
        'Console.WriteLine(_xsd_path)

        If Environment.GetCommandLineArgs.Length > 1 Then

            'Cadena_ConexionSQL_Server = Environment.GetCommandLineArgs(1)
            'Cadena_ConexionSQL_Server = Replace(Cadena_ConexionSQL_Server, "@", " ")
            Try

                _Empresa = Environment.GetCommandLineArgs(1)
                _AmbienteCertificacion = Environment.GetCommandLineArgs(2)
                _Id_Dte = Environment.GetCommandLineArgs(3)
                _Trackid = Environment.GetCommandLineArgs(4)
                _Accion = Environment.GetCommandLineArgs(5)

                If _AmbienteCertificacion Then

                    Console.WriteLine("******************************")
                    Console.WriteLine("Cadena_ConexionSQL_Server : {0}", Cadena_ConexionSQL_Server)
                    Console.WriteLine("Empresa                   : {0}", _Empresa)
                    Console.WriteLine("AmbienteCertificacion     : {0}", _AmbienteCertificacion)
                    Console.WriteLine("Id_Dte                    : {0}", _Id_Dte)
                    Console.WriteLine("Trackid                   : {0}", _Trackid)
                    Console.WriteLine("Accion                    : {0}", _Accion.ToString)
                    Console.WriteLine("******************************")
                    Console.WriteLine(vbCrLf)

                End If

            Catch ex As Exception
                Console.WriteLine("Error 1!")
                Console.WriteLine(ex.Message)
                Console.ReadKey()
                Return
            End Try

        Else

            '_Empresa = "01"
            'Cadena_ConexionSQL_Server = "data source = SIERRALTA; initial catalog = SIERRALTA_PRB; user id = SIERRALTA_PRB; password = SIERRALTA_PRB"
            'Cadena_ConexionSQL_Server = "data source=186.67.37.218,1518;initial catalog=SIERRALTA_PRB;user id=SIERRALTA_PRB;password = SIERRALTA_PRB"
            '"data@source=186.67.37.218,1518;initial@catalog=SIERRALTA_PRB;user@id=SIERRALTA_PRB;password=SIERRALTA_PRB"
            '01 False 441 0 0
            _Empresa = "01"
            _AmbienteCertificacion = False
            _Id_Dte = 455
            _Trackid = "5842541563" ' SOK ->--"5729876727"
            _Accion = Enum_Accion.ConsultarTrackid
            '_Accion = Enum_Accion.EnviarBoletaSII

        End If

        _Sql = New Class_SQL(Cadena_ConexionSQL_Server)

        _Global_BaseBk = _Sql.Fx_Trae_Dato("TABCARAC", "NOKOCARAC", "KOTABLA = 'BAKAPP'")

        If String.IsNullOrEmpty(_Global_BaseBk) Then
            _Sql.Pro_Error = "No se encontro el nombre de la base de datos de Bakapp, TABCARAC-NOKOCARAC-KOTABLA = 'BAKAPP'"
        End If

        If Not String.IsNullOrEmpty(_Sql.Pro_Error) Then
            Console.WriteLine("Error Sql : {0}", _Sql.Pro_Error)
            Console.ReadKey()
            Return
        End If

        _Global_BaseBk = _Global_BaseBk & ".dbo."

        _Sql.Global_BaseBk = _Global_BaseBk

        'Console.WriteLine("Cadena_ConexionSQL_Server : {0}", Cadena_ConexionSQL_Server)
        'Console.WriteLine("Empresa : {0}", _Empresa)
        'Console.WriteLine("Global_BaseBk : {0}", _Global_BaseBk)
        'Console.WriteLine("******************************")
        'Console.WriteLine(vbCrLf)
        'Console.ReadKey()

        Dim _Row_Empresa As DataRow
        Dim _RutEmpresaActiva As String

        Try
            Consulta_sql = "Select * From " & _Global_BaseBk & "Zw_Empresas Where Empresa = '" & _Empresa & "'"
            _Row_Empresa = _Sql.Fx_Get_DataRow(Consulta_sql)
            _RutEmpresaActiva = _Row_Empresa.Item("Rut")
        Catch ex As Exception
            Console.WriteLine("Error 2: " & ex.Message)
            Console.WriteLine("******************************")
            Console.ReadKey()
            Return
        End Try

        'If _Accion = Enum_Accion.CrearDirectorios Then

        '    If Fx_Crear_Directorios(_RutEmpresaActiva) Then
        '        Console.WriteLine("Directorios creados correctamente")
        '        Console.WriteLine("Empresa    : {0}, Rut;{1}", _Empresa, _RutEmpresaActiva)
        '        Console.WriteLine("******************************")
        '    End If

        'End If

        If _Accion = Enum_Accion.EnviarBoletaSII Then

            Dim _HefRespuesta As New HefRespuesta
            _HefRespuesta = Fx_Enviar_Boleta_SII(_Id_Dte, _AmbienteCertificacion, _RutEmpresaActiva)

            Console.WriteLine(vbCrLf)

            If Not IsNothing(_HefRespuesta) Then

                Console.WriteLine("EsCorrecto : {0}", _HefRespuesta.EsCorrecto)
                Console.WriteLine("FchProceso : {0}", _HefRespuesta.FchProceso)
                Console.WriteLine("Mensaje    : {0}", _HefRespuesta.Mensaje)
                Console.WriteLine("Detalle    : {0}", _HefRespuesta.Detalle)
                Console.WriteLine("Proceso    : {0}", _HefRespuesta.Proceso)
                Console.WriteLine("Resultado  : {0}", _HefRespuesta.Resultado)
                Console.WriteLine("Trackid    : {0}", _HefRespuesta.Trackid)
                'Console.WriteLine("XmlDocumento : {0}", _HefRespuesta.XmlDocumento)

            Else
                Console.WriteLine("No fue posible enviar el documento...")
            End If

        End If

        If _Accion = Enum_Accion.ConsultarTrackid Then

            Dim _HefRespuesta As New HefRespuesta

            Dim _Id_Trackid As Integer = _Sql.Fx_Trae_Dato(_Global_BaseBk & "Zw_DTE_Trackid", "Id", "Trackid = '" & _Trackid & "'", True, False)

            If Not CBool(_Id_Trackid) Then
                Console.WriteLine("No se encontro registros con el Trackid: {0}", _Trackid)
                Return
            End If

            _HefRespuesta = Fx_Consultar_Trackid(_Trackid, _AmbienteCertificacion)

            Console.WriteLine(vbCrLf)

            If Not IsNothing(_HefRespuesta) Then

                Consulta_sql = "Select * From " & _Global_BaseBk & "Zw_DTE_Trackid Where Trackid = '" & _Trackid & "'"
                Dim _TblTrackid As DataTable = _Sql.Fx_Get_Tablas(Consulta_sql)

                'Dim _RowTrackid As DataRow = _Sql.Fx_Get_DataRow(Consulta_sql)

                For Each _RowTrackid As DataRow In _TblTrackid.Rows

                    Dim _Intentos As Integer = _RowTrackid.Item("Intentos")
                    _Id_Trackid = _RowTrackid.Item("Id")

                    If _TblTrackid.Rows.Count = 0 Then ' _Id_Trackid = 0 Then
                        _HefRespuesta.EsCorrecto = False
                        _HefRespuesta.Mensaje = "NO se encontro Id Trackid con Trackid: " & _Trackid
                        _Id_Trackid = 0
                    End If

                    Console.WriteLine("EsCorrecto   : {0}", _HefRespuesta.EsCorrecto)
                    Console.WriteLine("FchProceso   : {0}", _HefRespuesta.FchProceso)
                    Console.WriteLine("Mensaje      : {0}", _HefRespuesta.Mensaje)
                    Console.WriteLine("Detalle      : {0}", _HefRespuesta.Detalle)
                    Console.WriteLine("Proceso      : {0}", _HefRespuesta.Proceso)
                    Console.WriteLine("Resultado    : {0}", _HefRespuesta.Resultado)
                    Console.WriteLine("Trackid      : {0}", _HefRespuesta.Trackid)
                    Console.WriteLine("XmlDocumento : {0}", vbCrLf & _HefRespuesta.XmlDocumento)

                    If CBool(_Id_Trackid) Then

                        Dim _Estado = String.Empty
                        Dim _Glosa = String.Empty

                        Dim _Aceptado As Integer
                        Dim _Informado As Integer
                        Dim _Rechazado As Integer
                        Dim _Reparo As Integer
                        Dim _Procesado As Integer = 1
                        Dim _Procesar As Integer = 0
                        Dim _Respuesta As String = _HefRespuesta.XmlDocumento
                        Dim _VolverProcesar As Boolean

                        _Intentos += 1

                        If Not _HefRespuesta.EsCorrecto Then
                            _Respuesta = _HefRespuesta.Detalle
                            _VolverProcesar = True
                        End If

                        Dim _RespuestSII As RespBolSII = Fx_ObtenerDatosRespuestaSII(_Respuesta)

                        If IsNothing(_RespuestSII) Then
                            _VolverProcesar = True
                        End If

                        If Not IsNothing(_RespuestSII) Then

                            _Estado = _RespuestSII.estado

                            Try
                                _Aceptado = _RespuestSII.estadistica(0).aceptados
                                _Informado = _RespuestSII.estadistica(0).informados
                                _Rechazado = _RespuestSII.estadistica(0).rechazados
                                _Reparo = _RespuestSII.estadistica(0).reparos
                            Catch ex As Exception
                                _Aceptado = False
                                _Informado = False
                                _Rechazado = False
                                _Reparo = False
                            End Try

                            If CBool(_Rechazado) Or CBool(_Reparo) Then
                                _Estado = _RespuestSII.detalle_rep_rech(0).estado
                                _Glosa = _RespuestSII.detalle_rep_rech(0).descripcion
                            Else
                                _Glosa = Fx_GlosaEstados(_Estado, _Aceptado, _Rechazado, _VolverProcesar)
                            End If

                            If _Estado = "EPR" And CBool(_Rechazado) Then
                                _Glosa += " (Revise el SII, puede que el documento este aceptado con otro Trackid)"
                            End If

                        End If

                        If _VolverProcesar And _Intentos <= 3 Then
                            _Procesado = 0
                            _Procesar = 1
                        End If

                        Consulta_sql = "Update " & _Global_BaseBk & "Zw_DTE_Trackid Set " & vbCrLf &
                                       "Procesado = " & _Procesado & "," &
                                       "Procesar = " & _Procesar & "," &
                                       "Informado = " & _Informado & "," &
                                       "Aceptado = " & _Aceptado & "," &
                                       "Rechazado = " & _Rechazado & "," &
                                       "Reparo = " & _Reparo & "," &
                                       "EnviarMail = 0," &
                                       "Estado = '" & _Estado & "'," &
                                       "Glosa = '" & _Glosa & "'," &
                                       "Respuesta = '" & _Respuesta & "'," & vbCrLf &
                                       "Intentos = " & _Intentos & vbCrLf &
                                       "Where Id = " & _Id_Trackid
                        _Sql.Ej_consulta_IDU(Consulta_sql, False)

                    End If

                Next

            Else
                Console.WriteLine("No fue posible enviar el documento...")
            End If

        End If

        If Environment.GetCommandLineArgs.Length <= 1 Then
            Console.ReadKey()
        End If

        End

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


        Dim _HefRespuesta As New HefRespuesta
        Dim _HefPublicador As New HefPublicador

        'Dim _xsd_path As String = _AppPath & "\Dtes\Dte\Schemas\BOLETAS\EnvioBOLETA_v11.xsd"
        Dim _RutEmisor As String
        Dim _Rutenvia As String
        Dim _FchResol As String
        Dim _NroResol As Integer
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

        If IsNothing(_Certificado) Then
            _HefRespuesta.EsCorrecto = False
            _HefRespuesta.Mensaje = "No se encontró el certificado electrónico"
            _HefRespuesta.Detalle = "Nombre del certificado: " & _Cn
            _HefRespuesta.Proceso = "Línea 296"
            Return _HefRespuesta
        End If

        Consulta_sql = "Select * From " & _Global_BaseBk & "Zw_DTE_Documentos Where Id_Dte = " & _Id_Dte
        Dim _Zw_DTE_Documentos As DataRow = _Sql.Fx_Get_DataRow(Consulta_sql)

        If Not String.IsNullOrEmpty(_Sql.Pro_Error) Then
            _HefRespuesta.EsCorrecto = False
            _HefRespuesta.Mensaje = _Sql.Pro_Error
            _HefRespuesta.Detalle = Consulta_sql
            _HefRespuesta.Proceso = "Línea 306"
            Return _HefRespuesta
        End If

        If IsNothing(_Zw_DTE_Documentos) Then
            _HefRespuesta.EsCorrecto = False
            _HefRespuesta.Mensaje = "No se encontro el registro con el Id_Dte: " & _Id_Dte
            _HefRespuesta.Detalle = Consulta_sql
            _HefRespuesta.Proceso = "Línea 306"
            Return _HefRespuesta
        End If

        Dim _Idmaeedo As Integer = _Zw_DTE_Documentos.Item("Idmaeedo")
        Dim _Xml As String = _Zw_DTE_Documentos.Item("CaratulaXml")

        Dim _Ruta = _AppPath & "\Data\DTE\Documentos\Boleta\"
        _Ruta = _AppPath & "\Data\Dtes\Documentos\Boleta\"

        If Not Directory.Exists(_Ruta) Then
            System.IO.Directory.CreateDirectory(_Ruta)
        End If

        Dim _CrearArchivo As String = Fx_CrearArchivo(_Ruta, "SET_ENVIO_BOLETA.xml", _Xml)

        If Not String.IsNullOrEmpty(_CrearArchivo) Then
            _HefRespuesta.EsCorrecto = False
            _HefRespuesta.Mensaje = "Problema al crear el archivo, " & _Ruta & "SET_ENVIO_BOLETA.xml"
            _HefRespuesta.Detalle = _CrearArchivo
            _HefRespuesta.Proceso = "Línea 338"
            Return _HefRespuesta
        End If

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
        Dim _Trackid As String
        Dim _XmlDocumento As String

        If _HefRespuesta.EsCorrecto Then

            _Trackid = _HefRespuesta.Trackid.ToString.Trim
            _XmlDocumento = _HefRespuesta.XmlDocumento

            Dim _Id_Trackid As Integer

            Consulta_sql = "Insert Into " & _Global_BaseBk & "Zw_DTE_Trackid (Id_Dte,Idmaeedo,Trackid,FechaEnvSII,Procesar,AmbienteCertificacion) Values " & vbCrLf &
                           "(" & _Id_Dte & "," & _Idmaeedo & ",'" & _Trackid & "',Getdate(),1," & Convert.ToInt32(_AmbienteCertificacion) & ")"
            _Sql.Ej_Insertar_Trae_Identity(Consulta_sql, _Id_Trackid, False)

            Consulta_sql = "Update " & _Global_BaseBk & "Zw_DTE_Documentos Set Procesado = 1,Procesar = 0 Where Id_Dte =  " & _Id_Dte
            _Sql.Ej_consulta_IDU(Consulta_sql, False)

        Else

            Dim _Detalle As String

            Try
                _Detalle = _HefRespuesta.Detalle
            Catch ex As Exception
                _Detalle = String.Empty
            End Try

            _Detalle = Replace(_Detalle, "'", "''")

            If _Detalle.Contains("Error") Then
                Consulta_sql = "Update " & _Global_BaseBk & "Zw_DTE_Documentos Set Procesado = 1,Procesar = 0,Respuesta = '" & _Detalle & "',ErrorEnvioDTE = 1" & vbCrLf &
                               "Where Id_Dte =  " & _Id_Dte
            Else
                Consulta_sql = "Update " & _Global_BaseBk & "Zw_DTE_Documentos Set Procesado = 0,Procesar = 1,Respuesta = '" & _Detalle & "'" & vbCrLf &
               "Where Id_Dte =  " & _Id_Dte
            End If

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

    End Function

    Function Fx_ObtenerDatosRespuestaSII(json As String) As RespBolSII
        Try
            Dim Arr As RespBolSII = JsonConvert.DeserializeObject(Of RespBolSII)(json)
            Return Arr
        Catch Ex As Exception
            Return Nothing
        End Try
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
                             Cuerpo As String) As String
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
            Return ex.Message
        End Try

        Return ""

    End Function

    Function Fx_Crear_Directorios(RutEmpresaActiva As String) As Boolean

        If Not File.Exists(AppPath() & "\Dte.zip") Then
            Console.WriteLine("******** ERROR AL CREAR DIRECTORIOS**********************")
            Console.WriteLine("No se encontro el archivo: " & AppPath() & "\Dte.zip")
            Return False
        End If

        Dim _AppPath = AppPath()

        If Not Directory.Exists(_AppPath & "\Data\Dte") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\Data\Dte")
        End If
        Console.WriteLine(_AppPath & "\Data\Dte")

        If Not Directory.Exists(_AppPath & "\DocumentosSII") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\DocumentosSII")
        End If
        Console.WriteLine(_AppPath & "\DocumentosSII")

        If Not Directory.Exists(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE")
        End If
        Console.WriteLine(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE")

        If Not Directory.Exists(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos")
        End If
        Console.WriteLine(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos")

        If Not Directory.Exists(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos\Boleta") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos\Boleta")
        End If
        Console.WriteLine(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos\Boleta")

        'Dim _Fullpath = _AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos"

        If Not Directory.Exists(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Hefesto") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Hefesto")
        End If
        Console.WriteLine(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Hefesto")

        If Not Directory.Exists(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Hefesto\CAF") Then
            System.IO.Directory.CreateDirectory(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Hefesto\CAF")
        End If
        Console.WriteLine(_AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Hefesto\CAF")

        Try

            ZipFile.CreateFromDirectory(_AppPath & "\Dte.zip", AppPath())

            'Copiamos la carpeta DocumentosSII en la raiz del sistema
            My.Computer.FileSystem.CopyDirectory(AppPath() & "\Dtes\DocumentosSII", _AppPath & "\DocumentosSII", True)
            Console.WriteLine(AppPath() & "\Dtes\DocumentosSII", _AppPath & "\DocumentosSII")
            'Copiamos la carpeta Dte dentro de la carpeta Data
            My.Computer.FileSystem.CopyDirectory(AppPath() & "\Dtes\Dte", _AppPath & "\Data\Dte", True)
            Console.WriteLine(AppPath() & "\Dtes\Dte", _AppPath & "\Data\Dte")
            'Copiamos la carpeta Documentos dentro de la carpete DTE de la empresa
            My.Computer.FileSystem.CopyDirectory(AppPath() & "\Dtes\Documentos", _AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos", True)
            Console.WriteLine(AppPath() & "\Dtes\Documentos", _AppPath & "\Data\" & RutEmpresaActiva & "\DTE\Documentos")
            'Eliminamos la carpeta de paso
            My.Computer.FileSystem.DeleteDirectory(AppPath() & "\Dtes", FileIO.DeleteDirectoryOption.DeleteAllContents)
            Console.WriteLine("Carpeta de paso eliminada.")
        Catch ex As Exception
            Console.WriteLine("******** ERROR AL CREAR DIRECTORIOS**********************")
            Console.WriteLine("Error 3 al descargar los archivos Dte.zip: " & ex.Message)
            Return False
        End Try

        Return True

    End Function

    Function Fx_GlosaEstados(_Estado As String,
                             ByRef _Aceptado As Integer,
                             ByRef _Rechazado As Integer,
                             ByRef _VolverProcesar As Boolean) As String

        _Aceptado = 0
        _Rechazado = 0
        _VolverProcesar = False

        Select Case _Estado
            Case "RSC"
                _Rechazado = 1
                Return "Rechazado por Error en Schema"
            Case "SOK"
                _VolverProcesar = True
                Return "Schema Validado"
            Case "CRT"
                _VolverProcesar = True
                Return "Carátula OK"
            Case "RFR"
                _Rechazado = 1
                Return "Rechazado por Error en Firma"
            Case "FOK"
                _VolverProcesar = True
                Return "Firma de Envió Validada"
            Case "PDR"
                _VolverProcesar = True
                Return "Envió en Proceso"
            Case "RCT"
                _Rechazado = 1
                Return "Rechazado por Error en Carátula"
            Case "EPR"
                _Aceptado = 1
                Return "Envió Procesado"
            Case "-1"
                _VolverProcesar = True
                Return "ERROR: RETORNO CAMPO ESTADO, NO EXISTE"
            Case "-2"
                _VolverProcesar = True
                Return "ERROR RETORNO"
            Case "-3"
                _VolverProcesar = True
                Return "ERROR: RUT USUARIO NO EXISTE"
            Case "-4"
                _VolverProcesar = True
                Return "ERROR OBTENCION DE DATOS"
            Case "-5"
                _VolverProcesar = True
                Return "ERROR RETORNO DATOS"
            Case "-6"
                _VolverProcesar = True
                Return "ERROR: USUARIO NO AUTORIZADO"
            Case "-7"
                _VolverProcesar = True
                Return "ERROR RETORNO DATOS"
            Case "-8"
                _VolverProcesar = True
                Return "ERROR: RETORNO DATOS"
            Case "-9"
                Return "ERROR: RETORNO DATOS"
            Case "-10"
                _VolverProcesar = True
                Return "ERROR: VALIDA RUT USUARIO"
            Case "-11"
                _VolverProcesar = True
                Return "ERR_CODE, SQL_CODE, SRV_CODE"
            Case "-12"
                _VolverProcesar = True
                Return "ERROR: RETORNO CONSULTA"
            Case "-13"
                _VolverProcesar = True
                Return "ERROR RUT USUARIO NULO"
            Case "-14"
                _VolverProcesar = True
                Return "ERROR XML RETORNO DATOS"
            Case "001"
                _Rechazado = 1
                Return "COOKIE INACTIV"
            Case "002"
                _Rechazado = 1
                Return "TOKEN+INACTIVO"
            Case "003"
                _Rechazado = 1
                Return "NO+EXISTE"
            Case Else
                Return ""
        End Select

    End Function

End Module
