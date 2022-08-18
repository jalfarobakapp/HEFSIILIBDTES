using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace HEFSIILIBDTES.NEGOCIO
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
        /// <param name="certificado">Certificado inscrito en el SII</param>
        /// <returns></returns>
        internal static HefRespuesta EstadoDteTrackid(string rutEmisor, string trackid, X509Certificate2 certificado, AmbienteSII ambiente)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoDteTrackid() NEG";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// En que ambiente debo hacer la consulta
                ////

                ////
                //// CERTIFICACION
                if (ambiente == AmbienteSII.Certificacion)
                {
                    ////
                    //// Recuperar el token
                    resp = AUTENTICACION.DTE.CERTIFICACION.HefLogin.RecuperarToken(rutEmisor, certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recupere el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Consultar al SII 
                    resp = DAL.HefConsultas.EstadoDteTrackidC(rutEmisor, trackid, token);

                }


                ////
                //// PRODUCCION
                if (ambiente == AmbienteSII.Produccion)
                {
                    ////
                    //// Recuperar el token
                    resp = AUTENTICACION.DTE.PRODUCCION.HefLogin.RecuperarToken(rutEmisor, certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recupere el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Consultar al SII 
                    resp = DAL.HefConsultas.EstadoDteTrackidP(rutEmisor, trackid, token);

                }



            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.Mensaje = "No fue posible consultar el estado del trackid.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;

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
        /// <param name="certificado">Certificado inscrito en el SII</param>
        /// <returns></returns>
        internal static HefRespuesta EstadoBolTrackid(string rutEmisor, string trackid, X509Certificate2 certificado, AmbienteSII ambiente)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoBolTrackid() NEG";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// En que ambiente debo hacer la consulta
                ////

                ////
                //// CERTIFICACION
                if (ambiente == AmbienteSII.Certificacion)
                {
                    ////
                    //// Recuperar el token
                    resp = AUTENTICACION.BOL.CERTIFICACION.HefLogin.RecuperarToken(rutEmisor, certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recupere el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Consultar al SII 
                    resp = DAL.HefConsultas.EstadoBolTrackidC(rutEmisor, trackid, token);

                }


                ////
                //// PRODUCCION
                if (ambiente == AmbienteSII.Produccion)
                {
                    ////
                    //// Recuperar el token
                    resp = AUTENTICACION.BOL.PRODUCCION.HefLogin.RecuperarToken(rutEmisor, certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recupere el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Consultar al SII 
                    resp = DAL.HefConsultas.EstadoBolTrackidP(rutEmisor, trackid, token);

                }

                ////
                //// Complete la información
                if (Regex.IsMatch(resp.Detalle, "\\(401\\)", RegexOptions.Singleline))
                    resp.Detalle += "\r\nEs probable que su certificado no tenga permisos para operar en ambiente de producción o certificación.";


                ////
                //// De formato a json
                if (!string.IsNullOrEmpty(resp.XmlDocumento))
                    resp.XmlDocumento = FUNCIONES.HefPrettyXml.FormatJson(resp.XmlDocumento);
            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.Mensaje = "No fue posible consultar el estado del trackid.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regerse el valor de retorno
            return resp;
        }

        /// <summary>
        /// Consulta el estado de la boleta utilizando valores de ella
        /// </summary>
        /// <returns></returns>
        internal static HefRespuesta EstadoBoleta(
             string rutEmisor,
                string tipo,
                    string folio,
                        string rutReceptor,
                            string monto,
                                string fchEmision,
                                    X509Certificate2 certificado,
                                        AmbienteSII ambiente
            )
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoBoleta NEG";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// En que ambiente debo hacer la consulta
                ////

                ////
                //// CERTIFICACION
                if (ambiente == AmbienteSII.Certificacion)
                {
                    ////
                    //// Recuperar el token
                    resp = AUTENTICACION.BOL.CERTIFICACION.HefLogin.RecuperarToken(rutEmisor, certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recupere el token
                    string token = resp.Resultado.ToString();

                    ////
                    //// Consultar al SII 
                    resp = DAL.HefConsultas.EstadoBoletaC(
                        rutEmisor,
                            tipo,
                                folio,
                                    rutReceptor,
                                        monto,
                                            fchEmision,
                                                token

                        );

                }


                ////
                //// PRODUCCION
                if (ambiente == AmbienteSII.Produccion)
                {
                    ////
                    //// Recuperar el token
                    resp = AUTENTICACION.BOL.PRODUCCION.HefLogin.RecuperarToken(rutEmisor, certificado);
                    if (!resp.EsCorrecto)
                        return resp;

                    ////
                    //// Recupere el token
                    string token = resp.Resultado.ToString();

                    
                    ////
                    //// Consultar al SII 
                    resp = DAL.HefConsultas.EstadoBoletaP(
                        rutEmisor,
                            tipo,
                                folio,
                                    rutReceptor,
                                        monto,
                                            fchEmision,
                                                token

                        );

                }

                ////
                //// Complete la información
                if (Regex.IsMatch(resp.Detalle, "\\(401\\)", RegexOptions.Singleline))
                    resp.Detalle += "\r\nEs probable que su certificado no tenga permisos para operar en ambiente de producción o certificación.";


                ////
                //// De formato a json
                if (!string.IsNullOrEmpty(resp.XmlDocumento))
                    resp.XmlDocumento = FUNCIONES.HefPrettyXml.FormatJson(resp.XmlDocumento);

            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error del proceso
                resp.Mensaje = "No fue posible consultar el estado de boleta";
                resp.Detalle = ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regerse el valor de retorno
            return resp;


        }
               
    }

}
