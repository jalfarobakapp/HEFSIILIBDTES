using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using HEFSIILIBDTES.LIBRERIA.BOL;
using System.Security.Cryptography;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using HEFSIILIBDTES.LIBRERIA.RCOF;
using System.Xml.Linq;

namespace HEFSIILIBDTES.NEGOCIO
{
    /// <summary>
    /// Metodos relacionados con la publicación de documentos
    /// </summary>
    internal class HefPublicadores
    {
        

        /// <summary>
        /// Inicia la publicación de documento DTE
        /// </summary>
        internal static HefRespuesta PublicarBol(object ob)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "Publicar BOLETA";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Recuperar el objeto
                HEFBOLETA Dte = (HEFBOLETA)ob;

                #region VALIDACION DE SCHEMA DEL DOCUMENTO

                ////
                //// Serializar el documento
                resp = FUNCIONES.HefSerializar.SerializarObjeto(ob);
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recuperar el documento
                string sDoc = resp.Resultado.ToString();


                ////
                //// Valide que los ruts del documento sean validos en forma y fondo

                ////
                //// Existe el schema para este documento
                if (string.IsNullOrEmpty(Dte.Configuracion.FullPathSchema))
                    throw new Exception("No puedo encontrar o no tengo acceso al schema de validación del DTE");

                ////
                //// El documento actual es valido según el schema?
                string doc_simulado = FUNCIONES.HefSchema.SimulaDocumentoBolCompleto(sDoc);
                List<string> errores = FUNCIONES.HefSchema.ValidarSchemaDTE(doc_simulado, Dte.Configuracion.FullPathSchema);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));



                #endregion

                #region VALIDACION DEL CERTIFICADO

                ////
                //// Iniciar el certififacdo
                X509Certificate2 Certificado = null;

                ////
                //// Vaidacion de forma de autenticación con certificado
                //// Si existe CN aplicar metodo de recuperación de certificado utilizando CN
                //// Si existe path y password de certificado, utilice metodo para generar el certificado 
                int INDICADOR = 0;
                if (!string.IsNullOrEmpty(Dte.Configuracion.CnCertificado))
                    INDICADOR = 1;
                if (!string.IsNullOrEmpty(Dte.Configuracion.PathCertificado) && !string.IsNullOrEmpty(Dte.Configuracion.PassCertificado))
                    INDICADOR = 2;


                ////
                //// En el caso que se necesite recuperar 
                switch (INDICADOR)
                {
                    case 1:

                        ////
                        //// Recupere el certificado utilizando el contenedor de windows
                        Certificado = FUNCIONES.HefCertificados.RecuperarCertificado(Dte.Configuracion.CnCertificado);
                        if (Certificado == null)
                            throw new Exception("No encuentro o no tengo permisos para ver el Certifiado '" + Dte.Configuracion.CnCertificado + "'");
                        break;

                    case 2:

                        ////
                        //// Valique que el certificado exista
                        if (!File.Exists(Dte.Configuracion.PathCertificado))
                            throw new Exception("El certificado no existe o no se tiene acceso a el.");

                        ////
                        //// Valique password del certificado
                        if (string.IsNullOrEmpty(Dte.Configuracion.PassCertificado))
                            throw new Exception("La password del certificadono puede estar vacía.");

                        ////
                        //// Recupere el certificado utilizando constructor 
                        Certificado = new X509Certificate2(
                            Dte.Configuracion.PathCertificado, 
                                Dte.Configuracion.PassCertificado,
                                    X509KeyStorageFlags.PersistKeySet
                                );
                        break;

                    default:

                        ////
                        //// Indique que no existe metodo para recuperar el certificado
                        throw new Exception("Debe seleccionar un metodo para agregar el certificado digital");

                }
                                
                ////
                //// El certificado tiene clave privada.
                if (!Certificado.HasPrivateKey)
                    throw new Exception("El certificado no tiene clave privada asignada, no se puede usar.");

                ////
                //// El certificado esta expirado?
                DateTime fecha_expiracion;
                if (DateTime.TryParse(Certificado.GetExpirationDateString(), out fecha_expiracion))
                    if (fecha_expiracion < DateTime.Now)
                        throw new Exception("El certificado esta expirado, revise la fecha de expiración.");

                #endregion

                #region VALICADIONES CAF

                ////
                //// Valide el objeto antes de iniciar el proceso
                if (!File.Exists(Dte.Configuracion.FullPathCaf))
                    throw new Exception("No puedo encontrar o no tengo acceso al archivo CAF '" + Dte.Configuracion.FullPathCaf + "'");

                ////
                //// Es un archivo xml valido?
                string Caf = File.ReadAllText(Dte.Configuracion.FullPathCaf, Encoding.GetEncoding("ISO-8859-1"));

                ////
                //// Validar el tipo de documento del archivo caf
                //// <TD>33</TD>
                string TD = Regex.Match(
                    Caf,
                        "<TD>(.*?)</TD>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                if (!"39|41".Contains(TD))
                    throw new Exception("El archivo caf no corresponde a una boleta electrónica");

                ////
                //// Es un archivo caf
                if (!Regex.IsMatch(Caf, "<AUTORIZACION.*</AUTORIZACION>", RegexOptions.Singleline))
                    throw new Exception("El archivo CAF seleccionado no parece ser valido. '" + Dte.Configuracion.FullPathCaf + "'");

                ////
                //// El rut emisor del caf es igual al documento DTE?
                string RUTEmisor = Regex.Match(
                    Caf,
                        "<RE>(.*?)</RE>",
                            RegexOptions.Singleline).Groups[1].Value;
                ////
                //// Es el mismo rut
                if (!RUTEmisor.Equals(Dte.Documento.Encabezado.Emisor.RUTEmisor))
                    throw new Exception("El rut emisor del documento caf no es el mismo del documento BOL");

                ////
                //// El rut emisor del caf es igual al documento DTE?
                string TipoDTE = Regex.Match(
                    Caf,
                        "<TD>(.*?)</TD>",
                            RegexOptions.Singleline).Groups[1].Value;
                ////
                //// Es el mismo rut
                if (!TipoDTE.Equals(Dte.Documento.Encabezado.IdDoc.TipoDTE.ToString()))
                    throw new Exception("El tipo de documento del archivo caf no es el mismo del documento BOL");

                ////
                //// El folio deldocumento es valido?
                string mD = Regex.Match(
                    Caf,
                        "<D>(.*?)</D>",
                            RegexOptions.Singleline
                ).Groups[1].Value;

                string mH = Regex.Match(
                    Caf,
                        "<H>(.*?)</H>",
                            RegexOptions.Singleline
                ).Groups[1].Value;

                ////
                //// Consulta por la valides del rango de folios
                if (Dte.Documento.Encabezado.IdDoc.Folio < Convert.ToInt64(mD) && Dte.Documento.Encabezado.IdDoc.Folio > Convert.ToInt64(mH))
                    throw new Exception("El folio indicado en el documento DTE no tiene relación con el rango de folios del archivo caf.");

                #endregion

                #region COMPLETAR LOS DATOS DEL DOCUMENTO

                ////
                //// Complete el ID del documento
                string ID = string.Format(
                    "HEFR{0}T{1}F{2}",
                        Dte.Documento.Encabezado.Emisor.RUTEmisor.Replace("-", ""),
                            Dte.Documento.Encabezado.IdDoc.TipoDTE.ToString(),
                                Dte.Documento.Encabezado.IdDoc.Folio.ToString()
                    );

                sDoc = Regex.Replace(
                    sDoc,
                        "ID=\".*?\"",
                            "ID=\"" + ID + "\"",
                                RegexOptions.Singleline
                    );


                #endregion

                #region CREAR EL NODO TED DEL DOCUMENTO

                ////
                //// Agregar el nodo ted al documento
                sDoc = FUNCIONES.HefSchema.AgregarTED(sDoc);

                ////
                //// Datosque deben ser recuperados
                string strTipoDTE = string.Empty;
                string strFolio = string.Empty;
                string strRutEmisor = string.Empty;
                string strRUTRecep = string.Empty;
                string strFchEmis = string.Empty;
                string strRznSocRecep = string.Empty;
                string strMntTotal = string.Empty;
                string strNmbItem = string.Empty;

                ////
                //// Complete los datos
                Match match = Regex.Match(sDoc, "<TipoDTE>(.*?)</TipoDTE>", RegexOptions.Singleline);
                if (match.Success)
                    strTipoDTE = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<Folio>(.*?)</Folio>", RegexOptions.Singleline);
                if (match.Success)
                    strFolio = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<RUTEmisor>(.*?)</RUTEmisor>", RegexOptions.Singleline);
                if (match.Success)
                    strRutEmisor = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<RUTRecep>(.*?)</RUTRecep>", RegexOptions.Singleline);
                if (match.Success)
                    strRUTRecep = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<FchEmis>(.*?)</FchEmis>", RegexOptions.Singleline);
                if (match.Success)
                    strFchEmis = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<RznSocRecep>(.*?)</RznSocRecep>", RegexOptions.Singleline);
                if (match.Success)
                {
                    strRznSocRecep = match.Groups[1].Value;
                    if (strRznSocRecep.Length > 40)
                        strRznSocRecep = strRznSocRecep.Substring(0, 40);
                }

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<MntTotal>(.*?)</MntTotal>", RegexOptions.Singleline);
                if (match.Success)
                    strMntTotal = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<NmbItem>(.*?)</NmbItem>", RegexOptions.Singleline);
                if (match.Success)
                {
                    strNmbItem = match.Groups[1].Value;
                    if (strNmbItem.Length > 40)
                        strNmbItem = strNmbItem.Substring(0, 40);
                }

                ////
                //// Completar los datos del nodo ted
                sDoc = Regex.Replace(sDoc, "<RE>XX</RE>", "<RE>" + strRutEmisor + "</RE>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<TD>XX</TD>", "<TD>" + strTipoDTE + "</TD>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<F>XX</F>", "<F>" + strFolio + "</F>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<FE>XX</FE>", "<FE>" + strFchEmis + "</FE>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<RR>XX</RR>", "<RR>" + strRUTRecep + "</RR>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<RSR>XX</RSR>", "<RSR>" + strRznSocRecep + "</RSR>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<MNT>XX</MNT>", "<MNT>" + strMntTotal + "</MNT>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<IT1>XX</IT1>", "<IT1>" + strNmbItem + "</IT1>", RegexOptions.Singleline);

                ////
                //// Ordenar el nodo ted
                sDoc = Regex.Replace(
                    sDoc,
                        "\\s+<!-- BEGIN TED",
                            "\r\n<!-- BEGIN TED",
                                RegexOptions.Singleline
                    );

                ////
                //// Actualice la fecha del timbre del ted
                sDoc = Regex.Replace(
                    sDoc,
                        "<TSTED>XX</TSTED>",
                            "<TSTED>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</TSTED>",
                                RegexOptions.Singleline
                    );

                ////
                //// Agregar el caf al documento documento DTE
                sDoc = Regex.Replace(sDoc,
                    "<CAF/>",
                        Regex.Match(Caf, "<CAF.*?</CAF>", RegexOptions.Singleline).Value,
                            RegexOptions.Singleline
                        );

                ////
                //// Ahora que tenemos el ted cargado en el documento,
                //// calcularemos el timbre ( firma ).
                //// y elinar los caracteres especiales entre los tags
                //// \s+ , \t , \r , \n
                string contenido_dd =
                    Regex.Replace(
                        Regex.Match(sDoc, "<DD>.*?</DD>", RegexOptions.Singleline).Value,
                            ">[\\s\r\n\t]+<",
                                "><",
                                    RegexOptions.Singleline);

                ////
                //// Recuperar el PK del archivo CAF
                string contenido__pk = Regex.Match(Caf, "<RSASK>(.*?)</RSASK>", RegexOptions.Singleline).Groups[1].Value;

                ////
                //// Calcule el hash de los datos a firmar DD
                //// transformando la cadena DD a arreglo de bytes, luego con
                //// el objeto 'SHA1CryptoServiceProvider' creamos el Hash del
                //// arreglo de bytes que representa los datos del DD
                Encoding encode = Encoding.GetEncoding("ISO-8859-1");
                byte[] bytesStrDD = encode.GetBytes(contenido_dd);
                byte[] HashValue = new SHA1CryptoServiceProvider().ComputeHash(bytesStrDD);

                ////
                //// Cree el objeto Rsa para poder firmar el hashValue creado
                //// en el punto anterior. La clase FuncionesComunes.crearRsaDesdePEM()
                //// Transforma la llave rivada del CAF en formato PEM a el objeto
                //// Rsa necesario para la firma.
                RSACryptoServiceProvider rsa = FUNCIONES.HefRsa.crearRsaDesdePEM(contenido__pk);

                ////
                //// Firme el HashValue ( arreglo de bytes representativo de DD )
                //// utilizando el formato de firma SHA1, lo cual regresará un nuevo 
                //// arreglo de bytes.
                byte[] bytesSing = rsa.SignHash(HashValue, "SHA1");

                ////
                //// Recupere la representación en base 64 de la firma, es decir de
                //// el arreglo de bytes 
                string strFRMTSHA1withRSA = Convert.ToBase64String(bytesSing);


                ////
                //// Inserte el timbre que fue calculado en la position que le corresponde
                sDoc = Regex.Replace(sDoc,
                    "<FRMT algoritmo=\"SHA1withRSA\">XX</FRMT>",
                        "<FRMT algoritmo=\"SHA1withRSA\">" + strFRMTSHA1withRSA + "</FRMT>",
                            RegexOptions.Singleline);




                #endregion

                #region FIRMA EL DOCUMENTO DTE

                ////
                //// Cargue el documento DTE como un objeto
                XmlDocument xDoc = new XmlDocument();
                xDoc.PreserveWhitespace = true;
                xDoc.LoadXml(sDoc);

                ////
                //// Inicie la firma del documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xDoc, Certificado, "#" + ID);
                sDoc = xDoc.OuterXml;

                #endregion

                #region CREE EL SOBRE PARA EL ENVIO AL SII

                ////
                //// Crear el ID del Envio
                string IDENV = string.Format(
                    "HEFENVR{0}T{1}F{2}",
                        Dte.Documento.Encabezado.Emisor.RUTEmisor.Replace("-", ""),
                            Dte.Documento.Encabezado.IdDoc.TipoDTE.ToString(),
                                Dte.Documento.Encabezado.IdDoc.Folio.ToString()
                    );

                ////
                //// Cree el sobre para realizar el envio al SII.
                string sEnvioDTE = "";
                sEnvioDTE += "<EnvioBOLETA version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sii.cl/SiiDte EnvioBOLETA_v11.xsd\">\r\n";
                sEnvioDTE += "<!-- HEFESTO DTE : http://lenguajedemaquinas.blogspot.com/ -->\r\n";
                sEnvioDTE += "<SetDTE ID=\"" + IDENV + "\">\r\n";
                sEnvioDTE += "<Caratula version=\"1.0\">\r\n";
                sEnvioDTE += "  <RutEmisor>" + strRutEmisor + "</RutEmisor>\r\n";
                sEnvioDTE += "  <RutEnvia>" + Dte.Configuracion.RutEnviador + "</RutEnvia>\r\n";
                sEnvioDTE += "  <RutReceptor>60803000-K</RutReceptor>\r\n";
                sEnvioDTE += "  <FchResol>" + Dte.Configuracion.FchResolucion + "</FchResol>\r\n";
                sEnvioDTE += "  <NroResol>" + Dte.Configuracion.NroResolucion + "</NroResol>\r\n";
                sEnvioDTE += "  <TmstFirmaEnv>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</TmstFirmaEnv>\r\n";
                sEnvioDTE += "<SubTotDTE>\r\n";
                sEnvioDTE += "<TpoDTE>" + strTipoDTE + "</TpoDTE>\r\n";
                sEnvioDTE += "<NroDTE>1</NroDTE>\r\n";
                sEnvioDTE += "</SubTotDTE>\r\n";
                sEnvioDTE += "</Caratula>\r\n";
                sEnvioDTE += "<!-- DTE BEGIN -->\r\n";
                sEnvioDTE += "<DTE/>\r\n";
                sEnvioDTE += "<!-- DTE END -->\r\n";
                sEnvioDTE += "</SetDTE>\r\n";
                sEnvioDTE += "</EnvioBOLETA>\r\n";

                ////
                //// Recupere solo el nodo DTE sin la instrucción de procesamiento
                sDoc = Regex.Match(sDoc, "<DTE.*?</DTE>", RegexOptions.Singleline).Value;

                ////
                //// Cargar el documento DTE timbrado y firmado
                sEnvioDTE = Regex.Replace(sEnvioDTE, "<DTE/>", sDoc, RegexOptions.Singleline);

                ////
                //// Inicie el firmado del documento actual
                XmlDocument xEnvioDTE = new XmlDocument();
                xEnvioDTE.PreserveWhitespace = true;
                xEnvioDTE.LoadXml(sEnvioDTE);

                ////
                //// Firme el documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xEnvioDTE, Certificado, "#" + IDENV);

                ////
                //// recupere el documento completo
                sDoc = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" + xEnvioDTE.OuterXml;

                #endregion

                #region VALIDACION DEL SCHEMA COMPLETO DEL ENBVIO DE LA BOLETA

                ////
                //// El documento actual es valido según el schema?
                errores = FUNCIONES.HefSchema.ValidarSchemaDTE(sDoc, Dte.Configuracion.FullPathSchema);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));

                #endregion

                #region INICIE EL ENVIO DEL DOCUMENTO AL SII

                ////
                //// Iniciar la publicación del documento
                resp = DAL.BOLETA.HefPublicadores.PublicarBOL(sDoc, Certificado);
                
                #endregion

                ////
                //// complete la respuesta
                resp.Proceso = "Envio de documento DTE al SII";
                resp.XmlDocumento = sDoc;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar documento DTE.";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regresar el valor de retorno
            return resp;

        }

        /// <summary>
        /// Inicia la publicación de documento DTE
        /// </summary>
        internal static HefRespuesta PublicarXmlBol(object ob)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "Publicar xml BOLETA";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Recuperar el objeto
                HEFBOLETA Dte = (HEFBOLETA)ob;

                #region VALIDACION DE SCHEMA DEL DOCUMENTO

                ////
                //// Serializar el documento
                resp = FUNCIONES.HefSerializar.SerializarObjeto(ob);
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recuperar el documento
                string sDoc = resp.Resultado.ToString();


                ////
                //// Valide que los ruts del documento sean validos en forma y fondo

                ////
                //// Existe el schema para este documento
                if (string.IsNullOrEmpty(Dte.Configuracion.FullPathSchema))
                    throw new Exception("No puedo encontrar o no tengo acceso al schema de validación del DTE");

                ////
                //// El documento actual es valido según el schema?
                string doc_simulado = FUNCIONES.HefSchema.SimulaDocumentoBolCompleto(sDoc);
                List<string> errores = FUNCIONES.HefSchema.ValidarSchemaDTE(doc_simulado, Dte.Configuracion.FullPathSchema);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));



                #endregion

                #region VALIDACION DEL CERTIFICADO

                ////
                //// Iniciar el certififacdo
                X509Certificate2 Certificado = null;

                ////
                //// Vaidacion de forma de autenticación con certificado
                //// Si existe CN aplicar metodo de recuperación de certificado utilizando CN
                //// Si existe path y password de certificado, utilice metodo para generar el certificado 
                int INDICADOR = 0;
                if (!string.IsNullOrEmpty(Dte.Configuracion.CnCertificado))
                    INDICADOR = 1;
                if (!string.IsNullOrEmpty(Dte.Configuracion.PathCertificado) && !string.IsNullOrEmpty(Dte.Configuracion.PassCertificado))
                    INDICADOR = 2;


                ////
                //// En el caso que se necesite recuperar 
                switch (INDICADOR)
                {
                    case 1:

                        ////
                        //// Recupere el certificado utilizando el contenedor de windows
                        Certificado = FUNCIONES.HefCertificados.RecuperarCertificado(Dte.Configuracion.CnCertificado);
                        if (Certificado == null)
                            throw new Exception("No encuentro o no tengo permisos para ver el Certifiado '" + Dte.Configuracion.CnCertificado + "'");
                        break;

                    case 2:

                        ////
                        //// Valique que el certificado exista
                        if (!File.Exists(Dte.Configuracion.PathCertificado))
                            throw new Exception("El certificado no existe o no se tiene acceso a el.");

                        ////
                        //// Valique password del certificado
                        if (string.IsNullOrEmpty(Dte.Configuracion.PassCertificado))
                            throw new Exception("La password del certificadono puede estar vacía.");

                        ////
                        //// Recupere el certificado utilizando constructor 
                        Certificado = new X509Certificate2(
                            Dte.Configuracion.PathCertificado,
                                Dte.Configuracion.PassCertificado,
                                    X509KeyStorageFlags.PersistKeySet
                                );
                        break;

                    default:

                        ////
                        //// Indique que no existe metodo para recuperar el certificado
                        throw new Exception("Debe seleccionar un metodo para agregar el certificado digital");

                }

                ////
                //// El certificado tiene clave privada.
                if (!Certificado.HasPrivateKey)
                    throw new Exception("El certificado no tiene clave privada asignada, no se puede usar.");

                ////
                //// El certificado esta expirado?
                DateTime fecha_expiracion;
                if (DateTime.TryParse(Certificado.GetExpirationDateString(), out fecha_expiracion))
                    if (fecha_expiracion < DateTime.Now)
                        throw new Exception("El certificado esta expirado, revise la fecha de expiración.");

                #endregion

                #region VALICADIONES CAF

                ////
                //// Valide el objeto antes de iniciar el proceso
                if (!File.Exists(Dte.Configuracion.FullPathCaf))
                    throw new Exception("No puedo encontrar o no tengo acceso al archivo CAF '" + Dte.Configuracion.FullPathCaf + "'");

                ////
                //// Es un archivo xml valido?
                string Caf = File.ReadAllText(Dte.Configuracion.FullPathCaf, Encoding.GetEncoding("ISO-8859-1"));

                ////
                //// Validar el tipo de documento del archivo caf
                //// <TD>33</TD>
                string TD = Regex.Match(
                    Caf,
                        "<TD>(.*?)</TD>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                if (!"39|41".Contains(TD))
                    throw new Exception("El archivo caf no corresponde a una boleta electrónica");

                ////
                //// Es un archivo caf
                if (!Regex.IsMatch(Caf, "<AUTORIZACION.*</AUTORIZACION>", RegexOptions.Singleline))
                    throw new Exception("El archivo CAF seleccionado no parece ser valido. '" + Dte.Configuracion.FullPathCaf + "'");

                ////
                //// El rut emisor del caf es igual al documento DTE?
                string RUTEmisor = Regex.Match(
                    Caf,
                        "<RE>(.*?)</RE>",
                            RegexOptions.Singleline).Groups[1].Value;
                ////
                //// Es el mismo rut
                if (!RUTEmisor.Equals(Dte.Documento.Encabezado.Emisor.RUTEmisor))
                    throw new Exception("El rut emisor del documento caf no es el mismo del documento BOL");

                ////
                //// El rut emisor del caf es igual al documento DTE?
                string TipoDTE = Regex.Match(
                    Caf,
                        "<TD>(.*?)</TD>",
                            RegexOptions.Singleline).Groups[1].Value;
                ////
                //// Es el mismo rut
                if (!TipoDTE.Equals(Dte.Documento.Encabezado.IdDoc.TipoDTE.ToString()))
                    throw new Exception("El tipo de documento del archivo caf no es el mismo del documento BOL");

                ////
                //// El folio deldocumento es valido?
                string mD = Regex.Match(
                    Caf,
                        "<D>(.*?)</D>",
                            RegexOptions.Singleline
                ).Groups[1].Value;

                string mH = Regex.Match(
                    Caf,
                        "<H>(.*?)</H>",
                            RegexOptions.Singleline
                ).Groups[1].Value;

                ////
                //// Consulta por la valides del rango de folios
                if (Dte.Documento.Encabezado.IdDoc.Folio < Convert.ToInt64(mD) && Dte.Documento.Encabezado.IdDoc.Folio > Convert.ToInt64(mH))
                    throw new Exception("El folio indicado en el documento DTE no tiene relación con el rango de folios del archivo caf.");

                #endregion

                #region COMPLETAR LOS DATOS DEL DOCUMENTO

                ////
                //// Complete el ID del documento
                string ID = string.Format(
                    "HEFR{0}T{1}F{2}",
                        Dte.Documento.Encabezado.Emisor.RUTEmisor.Replace("-", ""),
                            Dte.Documento.Encabezado.IdDoc.TipoDTE.ToString(),
                                Dte.Documento.Encabezado.IdDoc.Folio.ToString()
                    );

                sDoc = Regex.Replace(
                    sDoc,
                        "ID=\".*?\"",
                            "ID=\"" + ID + "\"",
                                RegexOptions.Singleline
                    );


                #endregion

                #region CREAR EL NODO TED DEL DOCUMENTO

                ////
                //// Agregar el nodo ted al documento
                sDoc = FUNCIONES.HefSchema.AgregarTED(sDoc);

                ////
                //// Datosque deben ser recuperados
                string strTipoDTE = string.Empty;
                string strFolio = string.Empty;
                string strRutEmisor = string.Empty;
                string strRUTRecep = string.Empty;
                string strFchEmis = string.Empty;
                string strRznSocRecep = string.Empty;
                string strMntTotal = string.Empty;
                string strNmbItem = string.Empty;

                ////
                //// Complete los datos
                Match match = Regex.Match(sDoc, "<TipoDTE>(.*?)</TipoDTE>", RegexOptions.Singleline);
                if (match.Success)
                    strTipoDTE = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<Folio>(.*?)</Folio>", RegexOptions.Singleline);
                if (match.Success)
                    strFolio = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<RUTEmisor>(.*?)</RUTEmisor>", RegexOptions.Singleline);
                if (match.Success)
                    strRutEmisor = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<RUTRecep>(.*?)</RUTRecep>", RegexOptions.Singleline);
                if (match.Success)
                    strRUTRecep = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<FchEmis>(.*?)</FchEmis>", RegexOptions.Singleline);
                if (match.Success)
                    strFchEmis = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<RznSocRecep>(.*?)</RznSocRecep>", RegexOptions.Singleline);
                if (match.Success)
                {
                    strRznSocRecep = match.Groups[1].Value;
                    if (strRznSocRecep.Length > 40)
                        strRznSocRecep = strRznSocRecep.Substring(0, 40);
                }

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<MntTotal>(.*?)</MntTotal>", RegexOptions.Singleline);
                if (match.Success)
                    strMntTotal = match.Groups[1].Value;

                ////
                //// Complete los datos
                match = Regex.Match(sDoc, "<NmbItem>(.*?)</NmbItem>", RegexOptions.Singleline);
                if (match.Success)
                {
                    strNmbItem = match.Groups[1].Value;
                    if (strNmbItem.Length > 40)
                        strNmbItem = strNmbItem.Substring(0, 40);
                }

                ////
                //// Completar los datos del nodo ted
                sDoc = Regex.Replace(sDoc, "<RE>XX</RE>", "<RE>" + strRutEmisor + "</RE>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<TD>XX</TD>", "<TD>" + strTipoDTE + "</TD>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<F>XX</F>", "<F>" + strFolio + "</F>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<FE>XX</FE>", "<FE>" + strFchEmis + "</FE>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<RR>XX</RR>", "<RR>" + strRUTRecep + "</RR>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<RSR>XX</RSR>", "<RSR>" + strRznSocRecep + "</RSR>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<MNT>XX</MNT>", "<MNT>" + strMntTotal + "</MNT>", RegexOptions.Singleline);
                sDoc = Regex.Replace(sDoc, "<IT1>XX</IT1>", "<IT1>" + strNmbItem + "</IT1>", RegexOptions.Singleline);

                ////
                //// Ordenar el nodo ted
                sDoc = Regex.Replace(
                    sDoc,
                        "\\s+<!-- BEGIN TED",
                            "\r\n<!-- BEGIN TED",
                                RegexOptions.Singleline
                    );

                ////
                //// Actualice la fecha del timbre del ted
                sDoc = Regex.Replace(
                    sDoc,
                        "<TSTED>XX</TSTED>",
                            "<TSTED>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</TSTED>",
                                RegexOptions.Singleline
                    );

                ////
                //// Agregar el caf al documento documento DTE
                sDoc = Regex.Replace(sDoc,
                    "<CAF/>",
                        Regex.Match(Caf, "<CAF.*?</CAF>", RegexOptions.Singleline).Value,
                            RegexOptions.Singleline
                        );

                ////
                //// Ahora que tenemos el ted cargado en el documento,
                //// calcularemos el timbre ( firma ).
                //// y elinar los caracteres especiales entre los tags
                //// \s+ , \t , \r , \n
                string contenido_dd =
                    Regex.Replace(
                        Regex.Match(sDoc, "<DD>.*?</DD>", RegexOptions.Singleline).Value,
                            ">[\\s\r\n\t]+<",
                                "><",
                                    RegexOptions.Singleline);

                ////
                //// Recuperar el PK del archivo CAF
                string contenido__pk = Regex.Match(Caf, "<RSASK>(.*?)</RSASK>", RegexOptions.Singleline).Groups[1].Value;

                ////
                //// Calcule el hash de los datos a firmar DD
                //// transformando la cadena DD a arreglo de bytes, luego con
                //// el objeto 'SHA1CryptoServiceProvider' creamos el Hash del
                //// arreglo de bytes que representa los datos del DD
                Encoding encode = Encoding.GetEncoding("ISO-8859-1");
                byte[] bytesStrDD = encode.GetBytes(contenido_dd);
                byte[] HashValue = new SHA1CryptoServiceProvider().ComputeHash(bytesStrDD);

                ////
                //// Cree el objeto Rsa para poder firmar el hashValue creado
                //// en el punto anterior. La clase FuncionesComunes.crearRsaDesdePEM()
                //// Transforma la llave rivada del CAF en formato PEM a el objeto
                //// Rsa necesario para la firma.
                RSACryptoServiceProvider rsa = FUNCIONES.HefRsa.crearRsaDesdePEM(contenido__pk);

                ////
                //// Firme el HashValue ( arreglo de bytes representativo de DD )
                //// utilizando el formato de firma SHA1, lo cual regresará un nuevo 
                //// arreglo de bytes.
                byte[] bytesSing = rsa.SignHash(HashValue, "SHA1");

                ////
                //// Recupere la representación en base 64 de la firma, es decir de
                //// el arreglo de bytes 
                string strFRMTSHA1withRSA = Convert.ToBase64String(bytesSing);


                ////
                //// Inserte el timbre que fue calculado en la position que le corresponde
                sDoc = Regex.Replace(sDoc,
                    "<FRMT algoritmo=\"SHA1withRSA\">XX</FRMT>",
                        "<FRMT algoritmo=\"SHA1withRSA\">" + strFRMTSHA1withRSA + "</FRMT>",
                            RegexOptions.Singleline);




                #endregion

                #region FIRMA EL DOCUMENTO DTE

                ////
                //// Cargue el documento DTE como un objeto
                XmlDocument xDoc = new XmlDocument();
                xDoc.PreserveWhitespace = true;
                xDoc.LoadXml(sDoc);

                ////
                //// Inicie la firma del documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xDoc, Certificado, "#" + ID);
                sDoc = xDoc.OuterXml;

                #endregion

                #region CREE EL SOBRE PARA EL ENVIO AL SII

                ////
                //// Crear el ID del Envio
                string IDENV = string.Format(
                    "HEFENVR{0}T{1}F{2}",
                        Dte.Documento.Encabezado.Emisor.RUTEmisor.Replace("-", ""),
                            Dte.Documento.Encabezado.IdDoc.TipoDTE.ToString(),
                                Dte.Documento.Encabezado.IdDoc.Folio.ToString()
                    );

                ////
                //// Cree el sobre para realizar el envio al SII.
                string sEnvioDTE = "";
                sEnvioDTE += "<EnvioBOLETA version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sii.cl/SiiDte EnvioBOLETA_v11.xsd\">\r\n";
                sEnvioDTE += "<!-- HEFESTO DTE : http://lenguajedemaquinas.blogspot.com/ -->\r\n";
                sEnvioDTE += "<SetDTE ID=\"" + IDENV + "\">\r\n";
                sEnvioDTE += "<Caratula version=\"1.0\">\r\n";
                sEnvioDTE += "  <RutEmisor>" + strRutEmisor + "</RutEmisor>\r\n";
                sEnvioDTE += "  <RutEnvia>" + Dte.Configuracion.RutEnviador + "</RutEnvia>\r\n";
                sEnvioDTE += "  <RutReceptor>60803000-K</RutReceptor>\r\n";
                sEnvioDTE += "  <FchResol>" + Dte.Configuracion.FchResolucion + "</FchResol>\r\n";
                sEnvioDTE += "  <NroResol>" + Dte.Configuracion.NroResolucion + "</NroResol>\r\n";
                sEnvioDTE += "  <TmstFirmaEnv>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</TmstFirmaEnv>\r\n";
                sEnvioDTE += "<SubTotDTE>\r\n";
                sEnvioDTE += "<TpoDTE>" + strTipoDTE + "</TpoDTE>\r\n";
                sEnvioDTE += "<NroDTE>1</NroDTE>\r\n";
                sEnvioDTE += "</SubTotDTE>\r\n";
                sEnvioDTE += "</Caratula>\r\n";
                sEnvioDTE += "<!-- DTE BEGIN -->\r\n";
                sEnvioDTE += "<DTE/>\r\n";
                sEnvioDTE += "<!-- DTE END -->\r\n";
                sEnvioDTE += "</SetDTE>\r\n";
                sEnvioDTE += "</EnvioBOLETA>\r\n";

                ////
                //// Recupere solo el nodo DTE sin la instrucción de procesamiento
                sDoc = Regex.Match(sDoc, "<DTE.*?</DTE>", RegexOptions.Singleline).Value;

                ////
                //// Cargar el documento DTE timbrado y firmado
                sEnvioDTE = Regex.Replace(sEnvioDTE, "<DTE/>", sDoc, RegexOptions.Singleline);

                ////
                //// Inicie el firmado del documento actual
                XmlDocument xEnvioDTE = new XmlDocument();
                xEnvioDTE.PreserveWhitespace = true;
                xEnvioDTE.LoadXml(sEnvioDTE);

                ////
                //// Firme el documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xEnvioDTE, Certificado, "#" + IDENV);

                ////
                //// recupere el documento completo
                sDoc = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" + xEnvioDTE.OuterXml;

                #endregion

                #region VALIDACION DEL SCHEMA COMPLETO DEL ENBVIO DE LA BOLETA

                ////
                //// El documento actual es valido según el schema?
                errores = FUNCIONES.HefSchema.ValidarSchemaDTE(sDoc, Dte.Configuracion.FullPathSchema);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));

                #endregion

                ////
                //// complete la respuesta
                resp.Proceso = "Envio de documento DTE al SII";
                resp.XmlDocumento = sDoc;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar documento DTE.";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regresar el valor de retorno
            return resp;

        }


        /// <summary>
        /// Inicia la publicación de documento DTE
        /// </summary>
        internal static HefRespuesta PublicarRcof(object ob)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "Publicar documento RCOF";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Recuperar el objeto
                HEFRCOF Rcof = (HEFRCOF)ob;

                #region VALIDACION DE SCHEMA DEL DOCUMENTO

                ////
                //// Serializar el documento
                resp = FUNCIONES.HefSerializar.SerializarObjeto(ob);
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recuperar el documento
                string sDoc = resp.Resultado.ToString();

                ////
                //// Existe el schema para este documento
                if (string.IsNullOrEmpty(Rcof.Configuracion.FullPathSchema))
                    throw new Exception("No puedo encontrar o no tengo acceso al schema de validación del DTE");

                ////
                //// El documento xsd corresponde al validador de RCOF
                string sRcof = File.ReadAllText(Rcof.Configuracion.FullPathSchema, Encoding.GetEncoding("ISO-8859-1"));
                if ( !Regex.IsMatch( sRcof, "name=\"ConsumoFolios\"", RegexOptions.Singleline ) )
                    throw new Exception("Documento XSD no corresponde a Consumo de folios..");


                ////
                //// El documento actual es valido según el schema?
                string doc_simulado = FUNCIONES.HefSchema.SimulaDocumentoRcofCompleto(sDoc);
                List<string> errores = FUNCIONES.HefSchema.ValidarSchemaDTE(doc_simulado, Rcof.Configuracion.FullPathSchema);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));



                #endregion

                #region VALIDACION DEL CERTIFICADO

                ////
                //// Recupera el certificado
                X509Certificate2 Certificado = FUNCIONES.HefCertificados.RecuperarCertificado(Rcof.Configuracion.CnCertificado);
                if (Certificado == null)
                    throw new Exception("No encuentro o no tengo permisos para ver el Certifiado '" + Rcof.Configuracion.CnCertificado + "'");

                ////
                //// El certificado tiene clave privada.
                if (!Certificado.HasPrivateKey)
                    throw new Exception("El certificado no tiene clave privada asignada, no se puede usar.");

                ////
                //// El certificado esta expirado?
                DateTime fecha_expiracion;
                if (DateTime.TryParse(Certificado.GetExpirationDateString(), out fecha_expiracion))
                    if (fecha_expiracion < DateTime.Now)
                        throw new Exception("El certificado esta expirado, revise la fecha de expiración.");

                #endregion

                #region AGREGAR LOS NAMESPACE AL DOCUMENTO

                ////
                //// Complete los namespaces del documento
                sDoc = Regex.Replace(
                    sDoc,
                        "<ConsumoFolios.*?>",
                            "<ConsumoFolios version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sii.cl/SiiDte ConsumoFolio_v10.xsd\">",
                                RegexOptions.Singleline
                     );


                ////
                //// Agregar publicidad
                sDoc = Regex.Replace(
                    sDoc,
                        "<DocumentoConsumoFolios",
                            "<!-- HEFESTO DTE : http://lenguajedemaquinas.blogspot.com/ -->\r\n<DocumentoConsumoFolios",
                                RegexOptions.Singleline
                     );


                ////
                //// De formato
                sDoc = FUNCIONES.HefPrettyXml.FormatXml(sDoc);
                               
                #endregion

                #region FIRMA EL DOCUMENTO DTE

                ////
                //// Recupere el ID del documento
                string ID = Regex.Match(sDoc, "ID=\"(.*?)\"", RegexOptions.Singleline).Groups[1].Value;

                ////
                //// Cargue el documento DTE como un objeto
                XmlDocument xDoc = new XmlDocument();
                xDoc.PreserveWhitespace = true;
                xDoc.LoadXml(sDoc);

                ////
                //// Inicie la firma del documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xDoc, Certificado, "#" + ID);
                sDoc = xDoc.OuterXml;

                #endregion

                #region VALIDACION DE SCHEMA

                ////
                //// validacion del documento 
                errores = FUNCIONES.HefSchema.ValidarSchemaDTE(sDoc, Rcof.Configuracion.FullPathSchema);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));

                #endregion

                #region INICIE EL ENVIO DEL DOCUMENTO AL SII

                ////
                //// Agregar instrucción de procesmiento
                sDoc = Regex.Replace(sDoc, 
                    "<ConsumoFolios", 
                        "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n<ConsumoFolios", 
                            RegexOptions.Singleline
                        );
                

                ////
                //// Iniciar la publicación del documento.
                //// Utiliza publicación DTE para enviar el documento al SII
                //// misma logica que un documento DTE.
                resp = DAL.DTE.HefPublicadores.PublicarDTE(sDoc, Certificado);

                #endregion

                ////
                //// complete la respuesta
                resp.XmlDocumento = sDoc;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
            }

            ////
            //// Regresar el valor de retorno
            return resp;

        }

        /// <summary>
        /// Realiza la publicación de un conjunto de boletas en el SII
        /// </summary>
        /// <returns></returns>
        public static HefRespuesta PublicarBoletas(string RutEmisor, string RutEnvia, string FchResol, int NroResol, string cert_path, string cert_pass, List<string> mis_boletas, X509Certificate2 Certificado, string xsd_path)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "Publicar Boletas";

            ////
            //// Iniciar el proceso
            try
            {

                #region CALCULAR LA CANTIDAD DE SUBTOTDTE

                ////
                //// Genere el resumen de los documentos subyacentes en la lista.
                List<string> _codigos = new List<string>();
                foreach (string boleta in mis_boletas)
                    _codigos.Add( Regex.Match(boleta, "<TipoDTE>(.*?)</TipoDTE>", RegexOptions.Singleline).Groups[1].Value );

                ////
                //// Crear resumen de los documentos
                string SubTotDte = "";
                List<string> _resumen = _codigos.Select(p => p).Distinct().ToList();
                foreach (string _resumen_codigo in _resumen)
                {
                    ////
                    //// inicie la cosntrucción de la estructura SubTotDte
                    SubTotDte += "<SubTotDTE>\r\n";
                    SubTotDte += "<TpoDTE>"+ _resumen_codigo +"</TpoDTE>\r\n";
                    SubTotDte += "<NroDTE>" + _codigos.Where( p=> p == _resumen_codigo).Count().ToString() + "</NroDTE>\r\n";
                    SubTotDte += "</SubTotDTE>\r\n";

                }

                ////
                //// Agregar todos los documentos DTES al sobre de envio.
                string Dtes = "";
                foreach (string DTE in mis_boletas)
                    Dtes += DTE + "\r\n";


                #endregion


                #region CREAR ENVIO DTE

                ////
                //// Cree el Id del envio DTE
                string ID = string.Format("ENVIOBOL{0}", DateTime.Now.ToString("yyyymmddTHHmmss"));

                ////
                //// Cree el sobre para realizar el envio al SII.
                string sEnvioDTE = "";
                sEnvioDTE += "<EnvioBOLETA version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sii.cl/SiiDte EnvioBOLETA_v11.xsd\">\r\n";
                sEnvioDTE += "<!-- HEFESTO DTE : http://lenguajedemaquinas.blogspot.com/ -->\r\n";
                sEnvioDTE += "<SetDTE ID=\"" + ID + "\">\r\n";
                sEnvioDTE += "<Caratula version=\"1.0\">\r\n";
                sEnvioDTE += "  <RutEmisor>" + RutEmisor + "</RutEmisor>\r\n";
                sEnvioDTE += "  <RutEnvia>" + RutEnvia + "</RutEnvia>\r\n";
                sEnvioDTE += "  <RutReceptor>60803000-K</RutReceptor>\r\n";
                sEnvioDTE += "  <FchResol>" + FchResol + "</FchResol>\r\n";
                sEnvioDTE += "  <NroResol>" + NroResol.ToString() + "</NroResol>\r\n";
                sEnvioDTE += "  <TmstFirmaEnv>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</TmstFirmaEnv>\r\n";

                ////
                //// Indique los elementos de este envio
                sEnvioDTE += SubTotDte;
                sEnvioDTE += "</Caratula>\r\n";
                sEnvioDTE += "<!-- DTES -->\r\n";
                
                ////
                //// Cierre el documento
                sEnvioDTE += "</SetDTE>\r\n";
                sEnvioDTE += "</EnvioBOLETA>\r\n";

                ////
                //// De formato al envio DTE
                sEnvioDTE = XDocument.Parse(sEnvioDTE).ToString();


                ////
                //// Agregar los documentos Dtes al sobre
                sEnvioDTE = Regex.Replace(sEnvioDTE, "<!-- DTES -->", Dtes, RegexOptions.Singleline);

                #endregion


                #region FIRMA EL DOCUMENTO DTE

                ////
                //// Inicie el firmado del documento actual
                XmlDocument xEnvioDTE = new XmlDocument();
                xEnvioDTE.PreserveWhitespace = true;
                xEnvioDTE.LoadXml(sEnvioDTE);

                ////
                //// Firme el documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xEnvioDTE, Certificado, "#" + ID);

                ////
                //// recupere el documento completo
                string sDoc = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" + xEnvioDTE.OuterXml;

                #endregion

                #region VALIDACION DEL SCHEMA COMPLETO DEL ENBVIO DE LA BOLETA

                ////
                //// El documento actual es valido según el schema?
                List<string>  errores = FUNCIONES.HefSchema.ValidarSchemaDTE(sDoc, xsd_path);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));

                #endregion

                ////
                //// Guarde el documento en formato ISO-8859-1
                File.WriteAllText("Boleta\\SET_ENVIO_BOLETA.XML", sDoc, Encoding.GetEncoding("ISO-8859-1"));


                #region INICIE EL ENVIO DEL DOCUMENTO AL SII

                ////
                //// Iniciar la publicación del documento
                resp = DAL.BOLETA.HefPublicadores.PublicarBOL(sDoc, Certificado);

                #endregion

                //
                // complete la respuesta
                resp.Proceso = "Envio de documento DTE al SII";
                resp.XmlDocumento = sDoc;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar documento DTE.";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regresar el valor de retorno
            return resp;


        }

        /// <summary>
        /// Realiza la publicación de un conjunto de boletas en el SII
        /// </summary>
        /// <returns></returns>
        public static HefRespuesta PublicarBoletas(string RutEmisor, string RutEnvia, string FchResol, int NroResol,  List<string> mis_boletas, X509Certificate2 Certificado, string xsd_path)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "Publicar Boletas";

            ////
            //// Iniciar el proceso
            try
            {

                #region CALCULAR LA CANTIDAD DE SUBTOTDTE

                ////
                //// Genere el resumen de los documentos subyacentes en la lista.
                List<string> _codigos = new List<string>();
                foreach (string boleta in mis_boletas)
                    _codigos.Add(Regex.Match(boleta, "<TipoDTE>(.*?)</TipoDTE>", RegexOptions.Singleline).Groups[1].Value);

                ////
                //// Crear resumen de los documentos
                string SubTotDte = "";
                List<string> _resumen = _codigos.Select(p => p).Distinct().ToList();
                foreach (string _resumen_codigo in _resumen)
                {
                    ////
                    //// inicie la cosntrucción de la estructura SubTotDte
                    SubTotDte += "<SubTotDTE>\r\n";
                    SubTotDte += "<TpoDTE>" + _resumen_codigo + "</TpoDTE>\r\n";
                    SubTotDte += "<NroDTE>" + _codigos.Where(p => p == _resumen_codigo).Count().ToString() + "</NroDTE>\r\n";
                    SubTotDte += "</SubTotDTE>\r\n";

                }

                ////
                //// Agregar todos los documentos DTES al sobre de envio.
                string Dtes = "";
                foreach (string DTE in mis_boletas)
                    Dtes += DTE + "\r\n";


                #endregion


                #region CREAR ENVIO DTE

                ////
                //// Cree el Id del envio DTE
                string ID = string.Format("ENVIOBOL{0}", DateTime.Now.ToString("yyyymmddTHHmmss"));

                ////
                //// Cree el sobre para realizar el envio al SII.
                string sEnvioDTE = "";
                sEnvioDTE += "<EnvioBOLETA version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sii.cl/SiiDte EnvioBOLETA_v11.xsd\">\r\n";
                sEnvioDTE += "<!-- HEFESTO DTE : http://lenguajedemaquinas.blogspot.com/ -->\r\n";
                sEnvioDTE += "<SetDTE ID=\"" + ID + "\">\r\n";
                sEnvioDTE += "<Caratula version=\"1.0\">\r\n";
                sEnvioDTE += "  <RutEmisor>" + RutEmisor + "</RutEmisor>\r\n";
                sEnvioDTE += "  <RutEnvia>" + RutEnvia + "</RutEnvia>\r\n";
                sEnvioDTE += "  <RutReceptor>60803000-K</RutReceptor>\r\n";
                sEnvioDTE += "  <FchResol>" + FchResol + "</FchResol>\r\n";
                sEnvioDTE += "  <NroResol>" + NroResol.ToString() + "</NroResol>\r\n";
                sEnvioDTE += "  <TmstFirmaEnv>" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss") + "</TmstFirmaEnv>\r\n";

                ////
                //// Indique los elementos de este envio
                sEnvioDTE += SubTotDte;
                sEnvioDTE += "</Caratula>\r\n";
                sEnvioDTE += "<!-- DTES -->\r\n";

                ////
                //// Cierre el documento
                sEnvioDTE += "</SetDTE>\r\n";
                sEnvioDTE += "</EnvioBOLETA>\r\n";

                ////
                //// De formato al envio DTE
                sEnvioDTE = XDocument.Parse(sEnvioDTE).ToString();


                ////
                //// Agregar los documentos Dtes al sobre
                sEnvioDTE = Regex.Replace(sEnvioDTE, "<!-- DTES -->", Dtes, RegexOptions.Singleline);

                #endregion


                #region FIRMA EL DOCUMENTO DTE

                ////
                //// Inicie el firmado del documento actual
                XmlDocument xEnvioDTE = new XmlDocument();
                xEnvioDTE.PreserveWhitespace = true;
                xEnvioDTE.LoadXml(sEnvioDTE);

                ////
                //// Firme el documento
                FUNCIONES.HefCertificados.firmarDocumentoXml(ref xEnvioDTE, Certificado, "#" + ID);

                ////
                //// recupere el documento completo
                string sDoc = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n" + xEnvioDTE.OuterXml;

                #endregion

                #region VALIDACION DEL SCHEMA COMPLETO DEL ENBVIO DE LA BOLETA

                ////
                //// El documento actual es valido según el schema?
                List<string> errores = FUNCIONES.HefSchema.ValidarSchemaDTE(sDoc, xsd_path);
                if (errores.Count > 0)
                    throw new Exception("Errores de schema del documento. " + string.Join("\r\n", errores));

                #endregion

                ////
                //// Guarde el documento en formato ISO-8859-1
                //File.WriteAllText("Boleta\\SET_ENVIO_BOLETA.XML", sDoc, Encoding.GetEncoding("ISO-8859-1"));
                //File.WriteAllText(xsd_path + "\\SET_ENVIO_BOLETA.XML", sDoc, Encoding.GetEncoding("ISO-8859-1"));
                

                #region INICIE EL ENVIO DEL DOCUMENTO AL SII

                ////
                //// Iniciar la publicación del documento
                resp = DAL.BOLETA.HefPublicadores.PublicarBOL(sDoc, Certificado);

                #endregion

                //
                // complete la respuesta
                resp.Proceso = "Envio de documento DTE al SII";
                resp.XmlDocumento = sDoc;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar documento DTE.";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regresar el valor de retorno
            return resp;


        }


    }

}
