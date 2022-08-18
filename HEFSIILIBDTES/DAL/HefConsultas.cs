using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HEFSIILIBDTES.DAL
{
    /// <summary>
    /// Metodos relacionados con las consultas al SII
    /// </summary>
    internal class HefConsultas
    {

        /// <summary>
        /// Consulta el estado de u documento DTE o RCOF utilizando el TRACKID de la operación de envío del documento
        /// </summary>
        /// <param name="rutEmisor">Rut de la empresa emisora del documento Ejemplo:99999999-K</param>
        /// <param name="trackid">Trackid de la operación de envio de un documento.</param>
        /// <param name="token">token del sii para comunicción con web services</param>
        /// <returns></returns>
        internal static HefRespuesta EstadoDteTrackidC(string rutEmisor, string trackid, string token)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoDteTrackidC() DAL Certificación";
            resp.Detalle = string.Format("Rut Emisor:{0} - Trackid:{1}", rutEmisor, trackid);

            ////
            //// Iniciar el proceso
            try
            {
                ////
                //// Iniciar la consulta
                CONSULTAS_CERTIFICACION.QueryEstUpService q = new CONSULTAS_CERTIFICACION.QueryEstUpService();
                string respuesta = q.getEstUp(
                    rutEmisor.Split('-')[0],
                        rutEmisor.Split('-')[1],
                            trackid,
                                token
                    );

                ////
                //// Regrese la respuesta de la consulta
                resp.EsCorrecto = true;
                resp.Mensaje = "Consulta ejecutada correctamente";
                resp.Trackid = trackid;
                resp.XmlDocumento = respuesta;
                                                          
            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible consultar el estado del trackid.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;
                resp.Trackid = trackid;
                resp.XmlDocumento = null;

            }

            ////
            //// Regerse el valor de retorno
            return resp;

        }

        /// <summary>
        /// Consulta el estado de u documento DTE o RCOF utilizando el TRACKID de la operación de envío del documento
        /// </summary>
        /// <param name="rutEmisor">Rut de la empresa emisora del documento Ejemplo:99999999-K</param>
        /// <param name="trackid">Trackid de la operación de envio de un documento.</param>
        /// <param name="token">token del sii para comunicción con web services</param>
        /// <returns></returns>
        internal static HefRespuesta EstadoDteTrackidP(string rutEmisor, string trackid, string token)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoDteTrackid() DAL Producción";
            resp.Detalle = string.Format("Rut Emisor:{0} - Trackid:{1}", rutEmisor, trackid);

            ////
            //// Iniciar el proceso
            try
            {
                ////
                //// Iniciar la consulta
                CONSULTAS_PRODUCCION.QueryEstUpService q = new CONSULTAS_PRODUCCION.QueryEstUpService();
                string respuesta = q.getEstUp(
                    rutEmisor.Split('-')[0],
                        rutEmisor.Split('-')[1],
                            trackid,
                                token
                    );

                ////
                //// Regrese la respuesta de la consulta
                resp.EsCorrecto = true;
                resp.Mensaje = "Consulta ejecutada correctamente";
                resp.Trackid = trackid;
                resp.XmlDocumento = respuesta;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible consultar el estado del trackid.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;
                resp.Trackid = trackid;
                resp.XmlDocumento = null;

            }

            ////
            //// Regerse el valor de retorno
            return resp;

        }


        /// <summary>
        /// Consulta el estado de u documento BOL o RCOF utilizando el TRACKID de la operación de envío del documento
        /// </summary>
        /// <param name="rutEmisor">Rut de la empresa emisora del documento Ejemplo:99999999-K</param>
        /// <param name="trackid">Trackid de la operación de envio de un documento.</param>
        /// <param name="token">token del sii para comunicción con web services</param>
        /// <returns></returns>
        internal static HefRespuesta EstadoBolTrackidC(string rutEmisor, string trackid, string token)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoBolTrackid() DAL Certificación";
            resp.Detalle = string.Format("Rut Emisor:{0} - Trackid:{1}", rutEmisor, trackid);

            ////
            //// Iniciar el proceso
            try
            {
                ////
                //// Iniciar la consulta al SII
                string uriSIITarget = string.Format(
                    "https://apicert.sii.cl/recursos/v1/boleta.electronica.envio/{0}-{1}-{2}",
                        rutEmisor.Split('-')[0],
                            rutEmisor.Split('-')[1],
                                trackid
                );

                ////
                //// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "GET";
                webRequest.Headers.Add("cookie", string.Format("TOKEN={0}", token));

                ////
                //// Recupere la respuesta
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                ////
                //// Si no hay respuesta del servidor
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("El SII no ha contestado nuestra solicitud.");

                ////
                //// Recupere la respuesta del SII
                string respuesta = string.Empty;
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
                            respuesta = readStream.ReadToEnd();

                        }
                    }

                }

                ////
                //// Destruya el response
                response = null;

                ////
                //// Regrese la respuesta de la consulta
                resp.EsCorrecto = true;
                resp.Mensaje = "Consulta ejecutada correctamente";
                resp.Trackid = trackid;
                resp.XmlDocumento = respuesta;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible consultar el estado del trackid.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;
                resp.Trackid = trackid;
                resp.XmlDocumento = null;

            }

            ////
            //// Regerse el valor de retorno
            return resp;

        }

        /// <summary>
        /// Consulta el estado de u documento BOL o RCOF utilizando el TRACKID de la operación de envío del documento
        /// </summary>
        /// <param name="rutEmisor">Rut de la empresa emisora del documento Ejemplo:99999999-K</param>
        /// <param name="trackid">Trackid de la operación de envio de un documento.</param>
        /// <param name="token">token del sii para comunicción con web services</param>
        /// <returns></returns>
        internal static HefRespuesta EstadoBolTrackidP(string rutEmisor, string trackid, string token)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoBolTrackid() DAL Producción";
            resp.Detalle = string.Format("Rut Emisor:{0} - Trackid:{1}", rutEmisor, trackid);

            ////
            //// Iniciar el proceso
            try
            {
                ////
                //// Iniciar la consulta al SII
                string uriSIITarget = string.Format(
                    "https://api.sii.cl/recursos/v1/boleta.electronica.envio/{0}-{1}-{2}",
                        rutEmisor.Split('-')[0],
                            rutEmisor.Split('-')[1],
                                trackid
                );

                ////
                //// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "GET";
                webRequest.Headers.Add("cookie", string.Format("TOKEN={0}", token));

                ////
                //// Recupere la respuesta
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                ////
                //// Si no hay respuesta del servidor
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception("El SII no ha contestado nuestra solicitud.");

                ////
                //// Recupere la respuesta del SII
                string respuesta = string.Empty;
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
                            respuesta = readStream.ReadToEnd();

                        }
                    }

                }

                ////
                //// Destruya el response
                response = null;
                                             
                ////
                //// Regrese la respuesta de la consulta
                resp.EsCorrecto = true;
                resp.Mensaje = "Consulta ejecutada correctamente";
                resp.Trackid = trackid;
                resp.XmlDocumento = respuesta;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible consultar el estado del trackid.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;
                resp.Trackid = trackid;
                resp.XmlDocumento = null;

            }

            ////
            //// Regerse el valor de retorno
            return resp;

        }


        /// <summary>
        /// Recupera los estados disponibles para la boleta electrónica
        /// </summary>
        internal static HefRespuesta EstadoBoletaC(
            string rutEmisor,
                string tipo,
                    string folio,
                        string rutReceptor,
                            string monto,
                                string fchEmision,
                                    string token
            )
        {

            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "EstadoBoletaC DAL";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            //// https://apicert.sii.cl/recursos/v1/boleta.electronica/{rut}-{dv}-{tipo}-{folio}/estado
            //// ?rut_receptor=12959262&dv_receptor=1&monto=1000&fechaEmision=20-08-2020
            string uriSIITarget = string.Format(
                "https://apicert.sii.cl/recursos/v1/boleta.electronica/{0}-{1}-{2}-{3}/estado?rut_receptor={4}&dv_receptor={5}&monto={6}&fechaEmision={7}",
                    rutEmisor.Split('-')[0],
                        rutEmisor.Split('-')[1],
                            tipo,
                                folio,
                                    rutReceptor.Split('-')[0],
                                        rutReceptor.Split('-')[1],
                                            monto,
                                                fchEmision

                                            );


            ////
            //// Inicie el procesamiento
            try
            {

                ////
                //// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "GET";
                webRequest.Headers.Add("cookie", string.Format("TOKEN={0}", token));

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

                ////
                //// Destruya el response
                response = null;

                ////
                //// cree respuesta 
                resp.EsCorrecto = true;
                resp.Detalle = "Recuperacion OK";
                resp.XmlDocumento = data;

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }

        /// <summary>
        /// Recupera los estados disponibles para la boleta electrónica
        /// </summary>
        internal static HefRespuesta EstadoBoletaP(
            string rutEmisor,
                string tipo,
                    string folio,
                        string rutReceptor,
                            string monto,
                                string fchEmision,
                                    string token
            )
        {

            ////
            //// Inicie la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "EstadoBoletaP DAL";
            resp.EsCorrecto = false;

            ////
            //// Identifique la url donde recuperar la información
            //// https://api.sii.cl/recursos/v1/boleta.electronica/{rut}-{dv}-{tipo}-{folio}/estado
            //// ?rut_receptor=12959262&dv_receptor=1&monto=1000&fechaEmision=20-08-2020
            string uriSIITarget = string.Format(
                "https://api.sii.cl/recursos/v1/boleta.electronica/{0}-{1}-{2}-{3}/estado?rut_receptor={4}&dv_receptor={5}&monto={6}&fechaEmision={7}",
                    rutEmisor.Split('-')[0],
                        rutEmisor.Split('-')[1],
                            tipo,
                                folio,
                                    rutReceptor.Split('-')[0],
                                        rutReceptor.Split('-')[1],
                                            monto,
                                                fchEmision

                                            );


            ////
            //// Inicie el procesamiento
            try
            {

                ////
                //// Crear conección a la página con la informacion
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uriSIITarget);
                webRequest.Method = "GET";
                webRequest.Headers.Add("cookie", string.Format("TOKEN={0}", token));

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

                ////
                //// Destruya el response
                response = null;

                ////
                //// cree respuesta 
                resp.EsCorrecto = true;
                resp.Detalle = "Recuperacion OK";
                resp.XmlDocumento = data;

            }
            catch (Exception ex)
            {

                ////
                //// Notifique al usario del error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }





    }

}
