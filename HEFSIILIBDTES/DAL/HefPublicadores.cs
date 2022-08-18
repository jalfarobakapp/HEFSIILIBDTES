using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;

namespace HEFSIILIBDTES.DAL.DTE
{
    /// <summary>
    /// Metodos relacionados con los envios al SII
    /// </summary>
    internal class HefPublicadores
    {

        /// <summary>
        /// Inicia la publicación del documento DTE en el SII
        /// </summary>
        internal static HefRespuesta PublicarDTE(string sDoc, X509Certificate2 Certificado)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "PublicarDTE";

            ////
            //// Inicia la publicacion
            try
            {
                ////
                //// En que ambiente debo publicar el documento?
                string nroResol = Regex.Match(
                    sDoc,
                        "<NroResol>(.*?)</NroResol>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                ////
                //// Inicie la publicacion del documento
                if (nroResol == "0")
                    resp = PublicarDTECertificacion(sDoc, Certificado);
                else
                    resp = PublicarDTEProduccion(sDoc, Certificado);

            }
            catch (Exception ex)
            {

                ////
                //// Notificar
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar el documento en el SII";
                resp.Detalle = ex.Message;
                
            }

            ////
            //// Regrese el valor de retorno
            return resp;

        }

        /// <summary>
        /// Inicia la publicación del documento dte actual en el SII ambiente de certificación
        /// </summary>
        internal static HefRespuesta PublicarDTECertificacion(string sDoc, X509Certificate2 Certificado)
        { 
            ////
            //// iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Inicie el envio
            try
            {
                ////
                //// Quien es el emisor del documento
                string RutEmisor = Regex.Match(
                    sDoc,
                        "<RutEmisor>(.*?)</RutEmisor>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;
                                
                ////
                //// Recuperar token de certificación para reaizar el envío
                resp = AUTENTICACION.DTE.CERTIFICACION.HefLogin.RecuperarToken(RutEmisor, Certificado);
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recuperar el token
                string token = resp.Resultado.ToString();
                
                ////
                //// Recupere los valores a procesar
                string pRutEnvia = Regex.Match(sDoc, "<RutEnvia>(.*?)</RutEnvia>", RegexOptions.Singleline).Groups[1].Value;
                string pRutEmisor = Regex.Match(sDoc, "<RutEmisor>(.*?)</RutEmisor>", RegexOptions.Singleline).Groups[1].Value;
                string pNombreArchivo = "HefestoEnvioDte.xml";

                ////
                //// Construya los encabezados del documento 
                string secuencia = string.Empty;
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"rutSender\"\r\n";
                secuencia += "\r\n";
                secuencia += "{0}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"dvSender\"\r\n";
                secuencia += "\r\n";
                secuencia += "{1}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"rutCompany\"\r\n";
                secuencia += "\r\n";
                secuencia += "{2}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"dvCompany\"\r\n";
                secuencia += "\r\n";
                secuencia += "{3}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"archivo\"; filename=\"{4}\" \r\n";
                secuencia += "Content-Type: application/octet-stream\r\n";
                secuencia += "Content-Transfer-Encoding: binary\r\n";
                secuencia += "\r\n";
                secuencia += "{5}"; ;
                secuencia += "\r\n";
                secuencia += "--7d23e2a11301c4--\r\n";

                ////
                //// Cargar los datos al protocolo
                secuencia = string.Format(
                    secuencia,
                        pRutEnvia.Split('-')[0],
                            pRutEnvia.Split('-')[1],
                                pRutEmisor.Split('-')[0],
                                    pRutEmisor.Split('-')[1],
                                        pNombreArchivo,
                                            sDoc
                );

                ////
                //// Cree los parametros del header.
                string pMethod = "POST";
                string pAccept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/ms-excel, application/msword, */*";
                string pReferer = "www.hefestosDte.cl";
                string pToken = "TOKEN={0}";

                ////
                //// Cree un nuevo request para iniciar el proceso
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://maullin.sii.cl/cgi_dte/UPL/DTEUpload");
                request.Method = pMethod;
                request.Accept = pAccept;
                request.Referer = pReferer;

                ////
                //// Agregar el content-type
                request.ContentType = "multipart/form-data: boundary=7d23e2a11301c4";
                request.ContentLength = secuencia.Length;

                ////
                //// Defina manualmente los headers del request
                request.Headers.Add("Accept-Language", "es-cl");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Cookie", string.Format(pToken, token));

                ////
                //// Defina el user agent
                request.UserAgent = "Mozilla/4.0 (compatible; PROG 1.0; Windows NT 5.0; YComp 5.0.2.4)";
                request.KeepAlive = true;

                ////
                //// Escriba la data en el objeto request.
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream(), Encoding.GetEncoding("ISO-8859-1")))
                {
                    sw.Write(secuencia);
                }

                ////
                //// Defina donde depositar el resultado
                string respuestaSii = string.Empty;

                ////
                //// Recupere la respuesta del sii
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        respuestaSii = sr.ReadToEnd().Trim();
                    }
                }

                ////
                //// Hay respuesta?
                if (string.IsNullOrEmpty(respuestaSii))
                    throw new ArgumentNullException("Respuesta del SII es null");

                ////
                //// Recupere los valores de la respuesta
                string errores = string.Empty;
                if (Regex.IsMatch(respuestaSii, "<STATUS>0</STATUS>", RegexOptions.Singleline))
                {

                    ////
                    //// Recupere la respuesta
                    resp.EsCorrecto = true;
                    resp.Mensaje = "Documento publicado correctamente en SII";
                    resp.Detalle = "Consulte estado del envío utilizando propiedad trackid";
                    resp.Trackid = Regex.Match(respuestaSii, "<TRACKID>(.*?)</TRACKID>", RegexOptions.Singleline).Groups[1].Value;
                    resp.Resultado = respuestaSii;

                } 
                else
                {
                    ////
                    //// Recupere todos los errores encontrados.
                    MatchCollection mc = Regex.Matches(respuestaSii, "<ERROR>(.*?)</ERROR>");
                    foreach (Match m in mc)
                        errores += m.Groups[1].Value + ", ";

                    ////
                    //// Cree la respueta
                    resp.EsCorrecto = false;
                    resp.Mensaje = "No fue posible publicar documento en SII";
                    resp.Detalle = "Lista de errores encontrados: " + errores;
                    resp.Trackid = "-1";
                    resp.Resultado = respuestaSii;
                    
                }

            }
            catch (Exception ex)
            {
                ////
                //// Notificar
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar el documento en el SII";
                resp.Detalle = ex.Message;
                
            }
        
            ////
            //// regrese el valor de retorno
            return resp;
        }

        /// <summary>
        /// Inicia la publicación del documento dte actual en el SII ambiente de certificación
        /// </summary>
        internal static HefRespuesta PublicarDTEProduccion(string sDoc, X509Certificate2 Certificado)
        {
            ////
            //// iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Inicie el envio
            try
            {
                ////
                //// Quien es el emisor del documento
                string RutEmisor = Regex.Match(
                    sDoc,
                        "<RutEmisor>(.*?)</RutEmisor>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                ////
                //// Recuperar token de certificación para reaizar el envío
                resp = AUTENTICACION.DTE.PRODUCCION.HefLogin.RecuperarToken(RutEmisor, Certificado);
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recuperar el token
                string token = resp.Resultado.ToString();

                ////
                //// Recupere los valores a procesar
                string pRutEnvia = Regex.Match(sDoc, "<RutEnvia>(.*?)</RutEnvia>", RegexOptions.Singleline).Groups[1].Value;
                string pRutEmisor = Regex.Match(sDoc, "<RutEmisor>(.*?)</RutEmisor>", RegexOptions.Singleline).Groups[1].Value;
                string pNombreArchivo = "HefestoEnvioDte.xml";

                ////
                //// Construya los encabezados del documento 
                string secuencia = string.Empty;
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"rutSender\"\r\n";
                secuencia += "\r\n";
                secuencia += "{0}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"dvSender\"\r\n";
                secuencia += "\r\n";
                secuencia += "{1}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"rutCompany\"\r\n";
                secuencia += "\r\n";
                secuencia += "{2}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"dvCompany\"\r\n";
                secuencia += "\r\n";
                secuencia += "{3}\r\n";
                secuencia += "--7d23e2a11301c4\r\n";
                secuencia += "Content-Disposition: form-data; name=\"archivo\"; filename=\"{4}\" \r\n";
                secuencia += "Content-Type: application/octet-stream\r\n";
                secuencia += "Content-Transfer-Encoding: binary\r\n";
                secuencia += "\r\n";
                secuencia += "{5}"; ;
                secuencia += "\r\n";
                secuencia += "--7d23e2a11301c4--\r\n";

                ////
                //// Cargar los datos al protocolo
                secuencia = string.Format(
                    secuencia,
                        pRutEnvia.Split('-')[0],
                            pRutEnvia.Split('-')[1],
                                pRutEmisor.Split('-')[0],
                                    pRutEmisor.Split('-')[1],
                                        pNombreArchivo,
                                            sDoc
                );

                ////
                //// Cree los parametros del header.
                string pMethod = "POST";
                string pAccept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-powerpoint, application/ms-excel, application/msword, */*";
                string pReferer = "www.hefestosDte.cl";
                string pToken = "TOKEN={0}";

                ////
                //// Cree un nuevo request para iniciar el proceso
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://palena.sii.cl/cgi_dte/UPL/DTEUpload");
                request.Method = pMethod;
                request.Accept = pAccept;
                request.Referer = pReferer;

                ////
                //// Agregar el content-type
                request.ContentType = "multipart/form-data: boundary=7d23e2a11301c4";
                request.ContentLength = secuencia.Length;

                ////
                //// Defina manualmente los headers del request
                request.Headers.Add("Accept-Language", "es-cl");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Cookie", string.Format(pToken, token));

                ////
                //// Defina el user agent
                request.UserAgent = "Mozilla/4.0 (compatible; PROG 1.0; Windows NT 5.0; YComp 5.0.2.4)";
                request.KeepAlive = true;

                ////
                //// Escriba la data en el objeto request.
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream(), Encoding.GetEncoding("ISO-8859-1")))
                {
                    sw.Write(secuencia);
                }

                ////
                //// Defina donde depositar el resultado
                string respuestaSii = string.Empty;

                ////
                //// Recupere la respuesta del sii
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        respuestaSii = sr.ReadToEnd().Trim();
                    }
                }

                ////
                //// Hay respuesta?
                if (string.IsNullOrEmpty(respuestaSii))
                    throw new ArgumentNullException("Respuesta del SII es null");

                ////
                //// Recupere los valores de la respuesta
                string errores = string.Empty;
                if (Regex.IsMatch(respuestaSii, "<STATUS>0</STATUS>", RegexOptions.Singleline))
                {

                    ////
                    //// Recupere la respuesta
                    resp.EsCorrecto = true;
                    resp.Mensaje = "Documento publicado correctamente en SII";
                    resp.Detalle = "Consulte estado del envío utilizando propiedad trackid";
                    resp.Trackid = Regex.Match(respuestaSii, "<TRACKID>(.*?)</TRACKID>", RegexOptions.Singleline).Groups[1].Value;
                    resp.Resultado = respuestaSii;

                }
                else
                {
                    ////
                    //// Recupere todos los errores encontrados.
                    MatchCollection mc = Regex.Matches(respuestaSii, "<ERROR>(.*?)</ERROR>");
                    foreach (Match m in mc)
                        errores += m.Groups[1].Value + ", ";

                    ////
                    //// Cree la respueta
                    resp.EsCorrecto = false;
                    resp.Mensaje = "No fue posible publicar documento en SII";
                    resp.Detalle = "Lista de errores encontrados: " + errores;
                    resp.Trackid = "-1";
                    resp.Resultado = respuestaSii;

                }

            }
            catch (Exception ex)
            {
                ////
                //// Notificar
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar el documento en el SII";
                resp.Detalle = ex.Message;

            }

            ////
            //// regrese el valor de retorno
            return resp;
        }

    }

}
namespace HEFSIILIBDTES.DAL.BOLETA
{
    /// <summary>
    /// Metodos relacionados con los envios al SII
    /// </summary>
    internal class HefPublicadores
    {

        /// <summary>
        /// Inicia la publicación del documento DTE en el SII
        /// </summary>
        internal static HefRespuesta PublicarBOL(string sDoc, X509Certificate2 Certificado)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "PublicarBOL";

            ////
            //// Inicia la publicacion
            try
            {
                ////
                //// Quien es el emisor del documento
                string RutEmisor = Regex.Match(
                    sDoc,
                        "<RutEmisor>(.*?)</RutEmisor>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                ////
                //// Quien es el emisor del documento
                string RutEnvia = Regex.Match(
                    sDoc,
                        "<RutEnvia>(.*?)</RutEnvia>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                ////
                //// En que ambiente debo publicar el documento?
                string nroResol = Regex.Match(
                    sDoc,
                        "<NroResol>(.*?)</NroResol>",
                            RegexOptions.Singleline
                    ).Groups[1].Value;

                ////
                //// Inicie la publicacion del documento
                if (nroResol == "0")
                {
                    ////
                    //// Recuperar token de certificación para reaizar el envío
                    resp = AUTENTICACION.BOL.CERTIFICACION.HefLogin.RecuperarToken(RutEmisor, Certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recuperar el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Recuperar la respuesta
                    resp = EnvioBoletaElectronicaCertificacion(RutEmisor, RutEnvia, sDoc, token);

                }
                else
                {

                    ////
                    //// Recuperar token de certificación para reaizar el envío
                    resp = AUTENTICACION.BOL.PRODUCCION.HefLogin.RecuperarToken(RutEmisor, Certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recuperar el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Recuperar la respuesta
                    resp = EnvioBoletaElectronicaProduccion(RutEmisor, RutEnvia, sDoc, token);

                }


            }
            catch (Exception ex)
            {

                ////
                //// Notificar
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible publicar el documento en el SII";
                resp.Detalle = ex.Message;

            }

            ////
            //// Regrese el valor de retorno
            return resp;

        }

        /// <summary>
        /// Consulta el trackid de un documento
        /// </summary>
        public static HefRespuesta EnvioBoletaElectronicaCertificacion(
            string rutEmisor,
                string rutEnviador,
                    string archivo,
                        string token
            )
        {

            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "EnvioBoletaElectronica";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            //// /boleta.electronica.envio
            string uriSIITarget = "https://pangal.sii.cl/recursos/v1/boleta.electronica.envio";

            ////
            //// Response
            HttpWebResponse response;

            ////
            //// Inicie el procesamiento
            try
            {

                ////
                //// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "POST";

                ////
                //// Completar el request
                webRequest.Accept = "application/json";
                webRequest.UserAgent = "Mozilla/4.0 ( compatible; PROG 1.0; Windows NT)";
                webRequest.ContentType = "multipart/form-data: boundary=------WebKitFormBoundaryDD9q0WZLO8kYJ00U";
                webRequest.Headers.Add("Cookie", string.Format("TOKEN={0}", token));

                ////
                //// Construya el post
                string POST = "";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"rutSender\"\r\n";
                POST += "\r\n";
                POST += rutEnviador.Split('-')[0] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"dvSender\"\r\n";
                POST += "\r\n";
                POST += rutEnviador.Split('-')[1] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"rutCompany\"\r\n";
                POST += "\r\n";
                POST += rutEmisor.Split('-')[0] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"dvCompany\"\r\n";
                POST += "\r\n";
                POST += rutEmisor.Split('-')[1] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"archivo\"; filename=\"SET_ENVIO_BOLETA.xml\"\r\n";
                POST += "Content-Type: text/xml\r\n";
                POST += "Content-Transfer-Encoding: binary\r\n";
                POST += "\r\n";
                POST += archivo;
                POST += "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U--\r\n";

                ////
                //// Calcular el post
                webRequest.ContentLength = POST.Length;
                byte[] requestData = Encoding.GetEncoding("ISO-8859-1").GetBytes(POST);
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(requestData, 0, requestData.Length);
                }

                ////
                //// Recuperar la respuesta
                string data = string.Empty;
                using (response = (HttpWebResponse)webRequest.GetResponse())
                {

                    ////
                    //// Recupere la respuesta del SII
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ////
                        //// Lea la respuesta del SII
                        using (Stream receiveStream = response.GetResponseStream())
                        {
                            using (StreamReader readStream = new StreamReader(receiveStream))
                            {

                                ////
                                //// Escriba el resultado en disco
                                data = readStream.ReadToEnd();

                            }
                        }

                    }
                    else
                    {
                        ////
                        //// notificar que no hay respuesta
                        throw new Exception("El SII no ha contestado nuestra solicitud.");
                    }

                }

                ////
                //// Recupere los valores de la respuesta
                string errores = string.Empty;
                if (Regex.IsMatch(data, "\"estado\": \"REC\",", RegexOptions.Singleline))
                {

                    ////
                    //// Recupere la respuesta
                    resp.EsCorrecto = true;
                    resp.Mensaje = "Documento publicado correctamente en SII";
                    resp.Detalle = "Consulte estado del envío utilizando propiedad trackid: " + resp.Trackid ;
                    resp.Trackid = Regex.Match(data, "\"trackid\":(.*?),", RegexOptions.Singleline).Groups[1].Value;
                    resp.Resultado = data;

                }
                else
                {
                    ////
                    //// Cree la respueta
                    resp.EsCorrecto = false;
                    resp.Mensaje = "No fue posible publicar documento en SII";
                    resp.Detalle = Regex.Match(data,"\"estado\": \"(.*?)\"",RegexOptions.Singleline).Groups[1].Value;
                    resp.Trackid = "-1";
                    resp.Resultado = data;

                }
                
            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = "No fue posible enviar el documento: " + ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regrese el valor de retorno
            return resp;

        }


        /// <summary>
        /// Consulta el trackid de un documento
        /// </summary>
        public static HefRespuesta EnvioBoletaElectronicaProduccion(
            string rutEmisor,
                string rutEnviador,
                    string archivo,
                        string token
            )
        {

            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "EnvioBoletaElectronica";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            //// /boleta.electronica.envio
            string uriSIITarget = "https://rahue.sii.cl/recursos/v1/boleta.electronica.envio";

            ////
            //// Response
            HttpWebResponse response;

            ////
            //// Inicie el procesamiento
            try
            {

                ////
                //// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "POST";

                ////
                //// Completar el request
                webRequest.Accept = "application/json";
                webRequest.UserAgent = "Mozilla/4.0 ( compatible; PROG 1.0; Windows NT)";
                webRequest.ContentType = "multipart/form-data: boundary=------WebKitFormBoundaryDD9q0WZLO8kYJ00U";
                webRequest.Headers.Add("Cookie", string.Format("TOKEN={0}", token));

                ////
                //// Construya el post
                string POST = "";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"rutSender\"\r\n";
                POST += "\r\n";
                POST += rutEnviador.Split('-')[0] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"dvSender\"\r\n";
                POST += "\r\n";
                POST += rutEnviador.Split('-')[1] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"rutCompany\"\r\n";
                POST += "\r\n";
                POST += rutEmisor.Split('-')[0] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"dvCompany\"\r\n";
                POST += "\r\n";
                POST += rutEmisor.Split('-')[1] + "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U\r\n";
                POST += "Content-Disposition: form-data; name=\"archivo\"; filename=\"SET_ENVIO_BOLETA.xml\"\r\n";
                POST += "Content-Type: text/xml\r\n";
                POST += "Content-Transfer-Encoding: binary\r\n";
                POST += "\r\n";
                POST += archivo;
                POST += "\r\n";
                POST += "--------WebKitFormBoundaryDD9q0WZLO8kYJ00U--\r\n";

                ////
                //// Calcular el post
                webRequest.ContentLength = POST.Length;
                byte[] requestData = Encoding.GetEncoding("ISO-8859-1").GetBytes(POST);
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(requestData, 0, requestData.Length);
                }

                ////
                //// Recuperar la respuesta
                string data = string.Empty;
                using (response = (HttpWebResponse)webRequest.GetResponse())
                {

                    ////
                    //// Recupere la respuesta del SII
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ////
                        //// Lea la respuesta del SII
                        using (Stream receiveStream = response.GetResponseStream())
                        {
                            using (StreamReader readStream = new StreamReader(receiveStream))
                            {

                                ////
                                //// Escriba el resultado en disco
                                data = readStream.ReadToEnd();

                            }
                        }

                    }
                    else
                    {
                        ////
                        //// notificar que no hay respuesta
                        throw new Exception("El SII no ha contestado nuestra solicitud.");
                    }

                }

                ////
                //// Recupere los valores de la respuesta
                string errores = string.Empty;
                if (Regex.IsMatch(data, "\"estado\": \"REC\",", RegexOptions.Singleline))
                {

                    ////
                    //// Recupere la respuesta
                    resp.EsCorrecto = true;
                    resp.Mensaje = "Documento publicado correctamente en SII";
                    resp.Detalle = "Consulte estado del envío utilizando propiedad trackid";
                    resp.Trackid = Regex.Match(data, "\"trackid\":(.*?),", RegexOptions.Singleline).Groups[1].Value;
                    resp.Resultado = data;

                }
                else
                {
                    ////
                    //// Cree la respueta
                    resp.EsCorrecto = false;
                    resp.Mensaje = "No fue posible publicar documento en SII";
                    resp.Detalle = Regex.Match(data, "\"estado\": \"(.*?)\"", RegexOptions.Singleline).Groups[1].Value;
                    resp.Trackid = "-1";
                    resp.Resultado = data;

                }

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = "No fue posible enviar el documento: " + ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regrese el valor de retorno
            return resp;

        }


    }

}