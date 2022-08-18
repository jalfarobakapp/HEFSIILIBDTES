using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace HEFSIILIBDTES.AUTENTICACION.DTE.CERTIFICACION
{
    /// <summary>
    /// Metodos relacionados con login del SII
    /// </summary>
    internal class HefLogin
    {

        /// <summary>
        /// Recupera el token de producción DTES
        /// </summary>
        internal static HefRespuesta RecuperarToken(string Rut, X509Certificate2 Certificado)
        {
            ////
            //// Iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Cual es el resultado?
            string token = string.Empty;

            ////
            //// Cree la ruta temporal de archivos tkns
            string directorio = Path.Combine(Path.GetTempPath(), "HEFTOKENS", "C");
            Directory.CreateDirectory(directorio);

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Pregunte si existe un token para este contribuyente
                string archivo = Path.Combine(directorio, string.Format("{0}.cer.dte.tkn", Rut));

                ////
                //// Si el archivo existe consulte el contenido
                if (File.Exists(archivo))
                {
                    
                    ////
                    //// Lea el contenido del archivo
                    string content = File.ReadAllText(archivo, Encoding.GetEncoding("ISO-8859-1"));

                    ////
                    //// Recupere los datos del archivo
                    string fecha = content.Split('|')[0];
                    token = content.Split('|')[1];

                    ////
                    //// habran pasado 2 horas desde la fecha 
                    //// de generación del documento token?
                    DateTime dt;
                    if (!DateTime.TryParse(fecha, out dt))
                        throw new Exception("No puedo convertir la fecha string en datetime token");

                    ////
                    //// Desde el momento que se generó la fecha han pasado 2 horas?
                    TimeSpan span = (DateTime.Now - dt);
                    if (span.Hours > 2)
                    { 
                        ////
                        //// Obtener un nuevo token
                        HefRespuesta r = RecuperarTokenSII(Certificado);
                        if (!r.EsCorrecto)
                            throw new Exception("No fue posible consultar token en el SII. " + r.Detalle );

                        ////
                        //// Recupere el token
                        token = r.Resultado.ToString();

                        ////
                        //// Guarde el token en disco
                        File.WriteAllText(
                            archivo,
                                string.Format("{0}|{1}",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token ),
                                    Encoding.GetEncoding("ISO-8859-1")
                                    );

                    }
                    
                }
                else
                {
                    ////
                    //// Obtener un nuevo token
                    HefRespuesta r = RecuperarTokenSII(Certificado);
                    if (!r.EsCorrecto)
                        throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                    ////
                    //// Recupere el token
                    token = r.Resultado.ToString();

                    ////
                    //// Guarde el token en disco
                    File.WriteAllText(
                        archivo,
                            string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                Encoding.GetEncoding("ISO-8859-1")
                                );



                }

                ////
                //// Construya la respuesta
                resp.EsCorrecto = true;
                resp.Mensaje = "Recuperación token OK";
                resp.Resultado = token;


            }
            catch (Exception ex)
            {
                ////
                //// Notificar al usuario
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible recuperar token";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regrese el token;
            return resp;

        }

        /// <summary>
        /// Recupera el token de certificación del SII
        /// </summary>
        private static HefRespuesta RecuperarTokenSII(X509Certificate2 certificado)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Iniciar el proceso
            try
            {

                #region RECUPERAR SEMILLA

                ////
                //// Mi semilla
                string mi_semilla = string.Empty;

                ////
                //// Recupere la semilla de la operación
                SeedCertificacion.CrSeedService semilla = new SeedCertificacion.CrSeedService();
                mi_semilla = semilla.getSeed();

                ////
                //// Recupere la semilla
                Match mSemilla = Regex.Match(mi_semilla, "<SEMILLA>(.*?)</SEMILLA>", RegexOptions.Singleline);
                if (!mSemilla.Success)
                    throw new Exception("No fue posible recuperar la semilla desde el SII.");

                ////
                //// Recuperar la semilla desde el SII
                mi_semilla = mSemilla.Groups[1].Value;



                #endregion

                #region FIRMAR LA SEMILLA

                ////
                //// Construya el sobre para colocar la semilla
                string body = string.Format("<getToken><item><Semilla>{0}</Semilla></item></getToken>", double.Parse(mi_semilla).ToString());

                ////
                //// Firmar la semilla
                mi_semilla = FUNCIONES.HefFirmas.firmarDocumentoSemilla(body, certificado);

                #endregion

                #region RECUPERAR TOKEN UTILIZANDO EL TOKEN

                ////
                //// Obtenga el token a partir de de la semilla firmada.
                TokenCertificacion.GetTokenFromSeedService gt = new TokenCertificacion.GetTokenFromSeedService();
                string valorRespuesta = gt.getToken(mi_semilla);
                if (string.IsNullOrEmpty(valorRespuesta))
                    throw new Exception("No fue posible recuperar el documento xml token desde el SII.");

                ////
                //// Lea la respuesta del sii
                //// respuesta = FuncionesComunes.leerRespuestaToken(valorRespuesta);
                Match mToken = Regex.Match(valorRespuesta, "<TOKEN>(.*?)</TOKEN>", RegexOptions.Singleline);
                if (!mToken.Success)
                    throw new Exception("No se encontró toekn del SII en la respuesta.");

                #endregion

                ////
                //// Si todo fue corrceto generar la respuesta
                resp.EsCorrecto = true;
                resp.Mensaje = "Recupeación del token Correcta.";
                resp.Resultado = mToken.Groups[1].Value;


            }
            catch (Exception Ex)
            {
                ////
                //// Indique al usuario el error
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible recuperar el token.";
                resp.Detalle = Ex.Message;
                resp.Resultado = null;

            }


            ////
            //// Regrese el valor de retorno
            return resp;

        }
        
    }

}

namespace HEFSIILIBDTES.AUTENTICACION.DTE.PRODUCCION
{
    /// <summary>
    /// Metodos relacionados con login del SII
    /// </summary>
    internal class HefLogin
    {

        /// <summary>
        /// Recupera el token de producción DTES
        /// </summary>
        internal static HefRespuesta RecuperarToken(string Rut, X509Certificate2 Certificado)
        {
            ////
            //// Iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Cual es el resultado?
            string token = string.Empty;

            ////
            //// Cree la ruta temporal de archivos tkns
            string directorio = Path.Combine(Path.GetTempPath(), "HEFTOKENS", "C");
            Directory.CreateDirectory(directorio);

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Pregunte si existe un token para este contribuyente
                string archivo = Path.Combine(directorio, string.Format("{0}.pro.dte.tkn", Rut));

                ////
                //// Si el archivo existe consulte el contenido
                if (File.Exists(archivo))
                {

                    ////
                    //// Lea el contenido del archivo
                    string content = File.ReadAllText(archivo, Encoding.GetEncoding("ISO-8859-1"));

                    ////
                    //// Recupere los datos del archivo
                    string fecha = content.Split('|')[0];
                    token = content.Split('|')[1];

                    ////
                    //// habran pasado 2 horas desde la fecha 
                    //// de generación del documento token?
                    DateTime dt;
                    if (!DateTime.TryParse(fecha, out dt))
                        throw new Exception("No puedo convertir la fecha string en datetime token");

                    ////
                    //// Desde el momento que se generó la fecha han pasado 2 horas?
                    TimeSpan span = (DateTime.Now - dt);
                    if (span.Hours > 2)
                    {
                        ////
                        //// Obtener un nuevo token
                        HefRespuesta r = RecuperarTokenSII(Certificado);
                        if (!r.EsCorrecto)
                            throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                        ////
                        //// Recupere el token
                        token = r.Resultado.ToString();

                        ////
                        //// Guarde el token en disco
                        File.WriteAllText(
                            archivo,
                                string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                    Encoding.GetEncoding("ISO-8859-1")
                                    );

                    }

                }
                else
                {
                    ////
                    //// Obtener un nuevo token
                    HefRespuesta r = RecuperarTokenSII(Certificado);
                    if (!r.EsCorrecto)
                        throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                    ////
                    //// Recupere el token
                    token = r.Resultado.ToString();

                    ////
                    //// Guarde el token en disco
                    File.WriteAllText(
                        archivo,
                            string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                Encoding.GetEncoding("ISO-8859-1")
                                );



                }

                ////
                //// Construya la respuesta
                resp.EsCorrecto = true;
                resp.Mensaje = "Recuperación token OK";
                resp.Resultado = token;


            }
            catch (Exception ex)
            {
                ////
                //// Notificar al usuario
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible recuperar token";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regrese el token;
            return resp;

        }

        /// <summary>
        /// Recupera el token de certificación del SII
        /// </summary>
        private static HefRespuesta RecuperarTokenSII(X509Certificate2 certificado)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Iniciar el proceso
            try
            {

                #region RECUPERAR SEMILLA

                ////
                //// Mi semilla
                string mi_semilla = string.Empty;

                ////
                //// Recupere la semilla de la operación
                SeedProduccion.CrSeedService semilla = new SeedProduccion.CrSeedService();
                mi_semilla = semilla.getSeed();

                ////
                //// Recupere la semilla
                Match mSemilla = Regex.Match(mi_semilla, "<SEMILLA>(.*?)</SEMILLA>", RegexOptions.Singleline);
                if (!mSemilla.Success)
                    throw new Exception("No fue posible recuperar la semilla desde el SII.");

                ////
                //// Recuperar la semilla desde el SII
                mi_semilla = mSemilla.Groups[1].Value;



                #endregion

                #region FIRMAR LA SEMILLA

                ////
                //// Construya el sobre para colocar la semilla
                string body = string.Format("<getToken><item><Semilla>{0}</Semilla></item></getToken>", double.Parse(mi_semilla).ToString());

                ////
                //// Firmar la semilla
                mi_semilla = FUNCIONES.HefFirmas.firmarDocumentoSemilla(body, certificado);

                #endregion

                #region RECUPERAR TOKEN UTILIZANDO EL TOKEN

                ////
                //// Obtenga el token a partir de de la semilla firmada.
                TokenProduccion.GetTokenFromSeedService gt = new TokenProduccion.GetTokenFromSeedService();
                string valorRespuesta = gt.getToken(mi_semilla);
                if (string.IsNullOrEmpty(valorRespuesta))
                    throw new Exception("No fue posible recuperar el documento xml token desde el SII.");

                ////
                //// Lea la respuesta del sii
                //// respuesta = FuncionesComunes.leerRespuestaToken(valorRespuesta);
                Match mToken = Regex.Match(valorRespuesta, "<TOKEN>(.*?)</TOKEN>", RegexOptions.Singleline);
                if (!mToken.Success)
                    throw new Exception("No se encontró toekn del SII en la respuesta.");

                #endregion

                ////
                //// Si todo fue corrceto generar la respuesta
                resp.EsCorrecto = true;
                resp.Mensaje = "Recupeación del token Correcta.";
                resp.Resultado = mToken.Groups[1].Value;


            }
            catch (Exception Ex)
            {
                ////
                //// Indique al usuario el error
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible recuperar el token.";
                resp.Detalle = Ex.Message;
                resp.Resultado = null;

            }


            ////
            //// Regrese el valor de retorno
            return resp;

        }

    }

}

namespace HEFSIILIBDTES.AUTENTICACION.BOL.CERTIFICACION
{
    /// <summary>
    /// Metodos relacionados con login del SII
    /// </summary>
    internal class HefLogin
    {

        /// <summary>
        /// Recupera el token de producción DTES
        /// </summary>
        internal static HefRespuesta RecuperarToken(string Rut, X509Certificate2 Certificado)
        {
            ////
            //// Iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Cual es el resultado?
            string token = string.Empty;

            ////
            //// Cree la ruta temporal de archivos tkns
            string directorio = Path.Combine(Path.GetTempPath(), "HEFTOKENS", "C");
            Directory.CreateDirectory(directorio);

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Pregunte si existe un token para este contribuyente
                string archivo = Path.Combine(directorio, string.Format("{0}.cer.bol.tkn", Rut));

                ////
                //// Si el archivo existe consulte el contenido
                if (File.Exists(archivo))
                {

                    ////
                    //// Lea el contenido del archivo
                    string content = File.ReadAllText(archivo, Encoding.GetEncoding("ISO-8859-1"));

                    ////
                    //// Recupere los datos del archivo
                    string fecha = content.Split('|')[0];
                    token = content.Split('|')[1];

                    ////
                    //// habran pasado 2 horas desde la fecha 
                    //// de generación del documento token?
                    DateTime dt;
                    if (!DateTime.TryParse(fecha, out dt))
                        throw new Exception("No puedo convertir la fecha string en datetime token");

                    ////
                    //// Desde el momento que se generó la fecha han pasado 2 horas?
                    TimeSpan span = (DateTime.Now - dt);
                    if (span.Minutes > 5)
                    {
                        ////
                        //// Obtener un nuevo token
                        HefRespuesta r = RecuperarTokenSII(Certificado);
                        if (!r.EsCorrecto)
                            throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                        ////
                        //// Recupere el token
                        token = r.Resultado.ToString();

                        ////
                        //// Guarde el token en disco
                        File.WriteAllText(
                            archivo,
                                string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                    Encoding.GetEncoding("ISO-8859-1")
                                    );
                    }

                }
                else
                {
                    ////
                    //// Obtener un nuevo token
                    HefRespuesta r = RecuperarTokenSII(Certificado);
                    if (!r.EsCorrecto)
                        throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                    ////
                    //// Recupere el token
                    token = r.Resultado.ToString();

                    ////
                    //// Guarde el token en disco
                    File.WriteAllText(
                        archivo,
                            string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                Encoding.GetEncoding("ISO-8859-1")
                                );

                }

                ////
                //// Construya la respuesta
                resp.EsCorrecto = true;
                resp.Mensaje = "Recuperación token OK";
                resp.Resultado = token;


            }
            catch (Exception ex)
            {
                ////
                //// Notificar al usuario
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible recuperar token";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regrese el token;
            return resp;

        }

        /// <summary>
        /// Recupera la semilla 
        /// </summary>
        internal static HefRespuesta RecuperarTokenSII(X509Certificate2 Certificado)
        {
        
            ////
            //// Iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "Proceso conectar";
            
            ////
            //// Inicia el proceso
            try
            {
            
                ////
                //// Recuperar token
                resp = RecuperarSemilla();
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recupere la respuesta
                //// hay semilla?
                if (!Regex.IsMatch(resp.Resultado.ToString(), "<ESTADO>00</ESTADO>", RegexOptions.Singleline))
                {
                    resp.EsCorrecto = false;
                    resp.Detalle = "La respuesta del SII no regreso semilla";
                    return resp;
                }

                ////
                //// Recupere la semilla
                string semilla = Regex.Match(
                    resp.Resultado.ToString(),
                        "<SEMILLA>(.*?)</SEMILLA>",
                            RegexOptions.Singleline
                                ).Groups[1].Value;

                ////
                //// Empaquetar semilla antes de firmarla
                semilla = string.Format(
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><getToken><item><Semilla>{0}</Semilla></item></getToken>",
                        double.Parse(semilla).ToString());

                ////
                //// Crear el xml de semilla firmada.
                string semilla_firmada = FUNCIONES.HefFirmas.firmarDocumentoSemilla(semilla, Certificado);

                ////
                //// Recuperar el token
                HefRespuesta respToken = RecuperarToken(semilla_firmada);
                if (!respToken.EsCorrecto)
                    return respToken;

                ////
                //// El Sii regreso token?
                if (!Regex.IsMatch(respToken.Resultado.ToString(), "<ESTADO>00</ESTADO>", RegexOptions.Singleline))
                {
                    respToken.EsCorrecto = false;
                    respToken.Detalle = "La respuesta del SII no regreso semilla";
                    return respToken;
                }

                ////
                //// Recupere el token de la operación
                string token = Regex.Match(
                    respToken.Resultado.ToString(),
                        "<TOKEN>(.*?)</TOKEN>",
                            RegexOptions.Singleline
                                ).Groups[1].Value;

                ////
                //// Completar la respuesta
                resp.EsCorrecto = true;
                resp.Resultado = token;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
            }
            
            ////
            //// Regrese el valor de retorno
            return resp;

        }
        
        /// <summary>
        /// Recupera la semilla desde el sii boletas 
        /// </summary>
        private static HefRespuesta RecuperarSemilla()
        {
            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "RecuperarSemilla";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            string uriSIITarget = "https://apicert.sii.cl/recursos/v1/boleta.electronica.semilla";

            ////
            //// Inicie el procesamiento
            try
            {

                #region CREAR REQUEST

                //////
                ////// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "GET";
                
                ////
                //// Recupere la respuesta
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                ////
                //// Si no hay respuesta del servidor
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("El SII no ha contestado nuestra solicitud.");

                ////
                //// Recupere la respuesta del SII
                string data = string.Empty;
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    ////
                    //// Recupere la respuesta
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream);

                    ////
                    //// Escriba el resultado en disco
                    data = readStream.ReadToEnd();

                    ////
                    //// Cierre objetos
                    response.Close();
                    readStream.Close();

                }

                #endregion

                ////
                //// cree respuesta 
                resp.EsCorrecto = true;
                resp.Detalle = "Recuperacion OK";
                resp.Resultado = data;

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;

            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }
        
        /// <summary>
        /// Recupera la semilla desde el sii - boletas 
        /// </summary>
        private static HefRespuesta RecuperarToken(string semilla_firmada)
        {
            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "RecuperarToken";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            string uriSIITarget = "https://apicert.sii.cl/recursos/v1/boleta.electronica.token";

            ////
            //// Inicie el procesamiento
            try
            {

                #region CREAR REQUEST

                //////
                ////// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/xml";

                ////
                //// Escriba los parametros en el request antes de consultar en el SII
                webRequest.ContentLength = semilla_firmada.Length;
                byte[] requestData = Encoding.UTF8.GetBytes(semilla_firmada);
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(requestData, 0, requestData.Length);
                }

                ////
                //// Recupere la respuesta
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                ////
                //// Si no hay respuesta del servidor
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("El SII no ha contestado nuestra solicitud.");

                ////
                //// Recupere la respuesta del SII
                string data = string.Empty;
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    ////
                    //// Recupere la respuesta
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream);

                    ////
                    //// Escriba el resultado en disco
                    data = readStream.ReadToEnd();

                    ////
                    //// Cierre objetos
                    response.Close();
                    readStream.Close();

                }

                #endregion

                ////
                //// cree respuesta 
                resp.EsCorrecto = true;
                resp.Detalle = "Recuperacion OK";
                resp.Resultado = data;

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;

            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }
        
    }

}

namespace HEFSIILIBDTES.AUTENTICACION.BOL.PRODUCCION
{
    /// <summary>
    /// Metodos relacionados con login del SII
    /// </summary>
    internal class HefLogin
    {

        /// <summary>
        /// Recupera el token de producción DTES
        /// </summary>
        internal static HefRespuesta RecuperarToken(string Rut, X509Certificate2 Certificado)
        {
            ////
            //// Iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Cual es el resultado?
            string token = string.Empty;

            ////
            //// Cree la ruta temporal de archivos tkns
            string directorio = Path.Combine(Path.GetTempPath(), "HEFTOKENS", "P");
            Directory.CreateDirectory(directorio);

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Pregunte si existe un token para este contribuyente
                string archivo = Path.Combine(directorio, string.Format("{0}.pro.bol.tkn", Rut));

                ////
                //// Si el archivo existe consulte el contenido
                if (File.Exists(archivo))
                {

                    ////
                    //// Lea el contenido del archivo
                    string content = File.ReadAllText(archivo, Encoding.GetEncoding("ISO-8859-1"));

                    ////
                    //// Recupere los datos del archivo
                    string fecha = content.Split('|')[0];
                    token = content.Split('|')[1];

                    ////
                    //// habran pasado 2 horas desde la fecha 
                    //// de generación del documento token?
                    DateTime dt;
                    if (!DateTime.TryParse(fecha, out dt))
                        throw new Exception("No puedo convertir la fecha string en datetime token");

                    ////
                    //// Desde el momento que se generó la fecha han pasado 2 horas?
                    TimeSpan span = (DateTime.Now - dt);
                    if (span.Hours > 2)
                    {
                        ////
                        //// Obtener un nuevo token
                        HefRespuesta r = RecuperarTokenSII(Certificado);
                        if (!r.EsCorrecto)
                            throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                        ////
                        //// Recupere el token
                        token = r.Resultado.ToString();

                        ////
                        //// Guarde el token en disco
                        File.WriteAllText(
                            archivo,
                                string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                    Encoding.GetEncoding("ISO-8859-1")
                                    );
                    }

                }
                else
                {
                    ////
                    //// Obtener un nuevo token
                    HefRespuesta r = RecuperarTokenSII(Certificado);
                    if (!r.EsCorrecto)
                        throw new Exception("No fue posible consultar token en el SII. " + r.Detalle);

                    ////
                    //// Recupere el token
                    token = r.Resultado.ToString();

                    ////
                    //// Guarde el token en disco
                    File.WriteAllText(
                        archivo,
                            string.Format("{0}|{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), token),
                                Encoding.GetEncoding("ISO-8859-1")
                                );

                }

                ////
                //// Construya la respuesta
                resp.EsCorrecto = true;
                resp.Mensaje = "Recuperación token OK";
                resp.Resultado = token;


            }
            catch (Exception ex)
            {
                ////
                //// Notificar al usuario
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible recuperar token";
                resp.Detalle = ex.Message;
            }

            ////
            //// Regrese el token;
            return resp;

        }

        /// <summary>
        /// Recupera la semilla 
        /// </summary>
        internal static HefRespuesta RecuperarTokenSII(X509Certificate2 Certificado)
        {

            ////
            //// Iniciar la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "Proceso conectar";

            ////
            //// Inicia el proceso
            try
            {

                ////
                //// Recuperar token
                resp = RecuperarSemilla();
                if (!resp.EsCorrecto)
                    return resp;

                ////
                //// Recupere la respuesta
                //// hay semilla?
                if (!Regex.IsMatch(resp.Resultado.ToString(), "<ESTADO>00</ESTADO>", RegexOptions.Singleline))
                {
                    resp.EsCorrecto = false;
                    resp.Detalle = "La respuesta del SII no regreso semilla";
                    return resp;
                }

                ////
                //// Recupere la semilla
                string semilla = Regex.Match(
                    resp.Resultado.ToString(),
                        "<SEMILLA>(.*?)</SEMILLA>",
                            RegexOptions.Singleline
                                ).Groups[1].Value;

                ////
                //// Empaquetar semilla antes de firmarla
                semilla = string.Format(
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><getToken><item><Semilla>{0}</Semilla></item></getToken>",
                        double.Parse(semilla).ToString());

                ////
                //// Crear el xml de semilla firmada.
                string semilla_firmada = FUNCIONES.HefFirmas.firmarDocumentoSemilla(semilla, Certificado);

                ////
                //// Recuperar el token
                HefRespuesta respToken = RecuperarToken(semilla_firmada);
                if (!respToken.EsCorrecto)
                    return respToken;

                ////
                //// El Sii regreso token?
                if (!Regex.IsMatch(respToken.Resultado.ToString(), "<ESTADO>00</ESTADO>", RegexOptions.Singleline))
                {
                    respToken.EsCorrecto = false;
                    respToken.Detalle = "La respuesta del SII no regreso semilla";
                    return respToken;
                }

                ////
                //// Recupere el token de la operación
                string token = Regex.Match(
                    respToken.Resultado.ToString(),
                        "<TOKEN>(.*?)</TOKEN>",
                            RegexOptions.Singleline
                                ).Groups[1].Value;

                ////
                //// Completar la respuesta
                resp.EsCorrecto = true;
                resp.Resultado = token;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
            }

            ////
            //// Regrese el valor de retorno
            return resp;

        }

        /// <summary>
        /// Recupera la semilla desde el sii boletas 
        /// </summary>
        private static HefRespuesta RecuperarSemilla()
        {
            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "RecuperarSemilla";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            string uriSIITarget = "https://api.sii.cl/recursos/v1/boleta.electronica.semilla";

            ////
            //// Inicie el procesamiento
            try
            {

                #region CREAR REQUEST

                
                
                //////
                ////// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "GET";


                ////
                //// Recupere la respuesta
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                ////
                //// Si no hay respuesta del servidor
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("El SII no ha contestado nuestra solicitud.");

                ////
                //// Recupere la respuesta del SII
                string data = string.Empty;
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    ////
                    //// Recupere la respuesta
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream);

                    ////
                    //// Escriba el resultado en disco
                    data = readStream.ReadToEnd();

                    ////
                    //// Cierre objetos
                    response.Close();
                    readStream.Close();

                }

                #endregion

                ////
                //// cree respuesta 
                resp.EsCorrecto = true;
                resp.Detalle = "Recuperacion OK";
                resp.Resultado = data;

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;

            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }

        /// <summary>
        /// Recupera la semilla desde el sii - boletas 
        /// </summary>
        private static HefRespuesta RecuperarToken(string semilla_firmada)
        {
            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "RecuperarToken";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            string uriSIITarget = "https://api.sii.cl/recursos/v1/boleta.electronica.token";

            ////
            //// Inicie el procesamiento
            try
            {

                #region CREAR REQUEST

                //////
                ////// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/xml";

                ////
                //// Escriba los parametros en el request antes de consultar en el SII
                webRequest.ContentLength = semilla_firmada.Length;
                byte[] requestData = Encoding.UTF8.GetBytes(semilla_firmada);
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(requestData, 0, requestData.Length);
                }

                ////
                //// Recupere la respuesta
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                ////
                //// Si no hay respuesta del servidor
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("El SII no ha contestado nuestra solicitud.");

                ////
                //// Recupere la respuesta del SII
                string data = string.Empty;
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    ////
                    //// Recupere la respuesta
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream);

                    ////
                    //// Escriba el resultado en disco
                    data = readStream.ReadToEnd();

                    ////
                    //// Cierre objetos
                    response.Close();
                    readStream.Close();

                }

                #endregion

                ////
                //// cree respuesta 
                resp.EsCorrecto = true;
                resp.Detalle = "Recuperacion OK";
                resp.Resultado = data;

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;

            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }

    }

}