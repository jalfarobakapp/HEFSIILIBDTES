using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace HEFSIILIBDTES.CONSULTAS
{
    /// <summary>
    /// Metodos relacionados con las consultas al SII
    /// </summary>
    public class HefConsultas
    {
        /// <summary>
        /// Consulta el estado de u documento DTE o RCOF utilizando el TRACKID de la operación de envío del documento
        /// </summary>
        /// <param name="rutEmisor">Rut de la empresa emisora del documento Ejemplo:99999999-K</param>
        /// <param name="trackid">Trackid de la operación de envio de un documento.</param>
        /// <param name="Cn">ombre comun del certificado instlada en el pc actual</param>
        /// <returns></returns>
        public static HefRespuesta EstadoDteTrackid(string rutEmisor,string trackid, string Cn, AmbienteSII ambiente)
        {
            ////
            //// Beta?
            HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            if (!respVal.EsCorrecto)
                return respVal;


            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoDteTrackid()";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Iniciar validación de los elementos de la consulta
                rutEmisor = rutEmisor.ToUpper();
                if (!Regex.IsMatch(rutEmisor, "[0-9]{3,8}-[0-9K]{1}", RegexOptions.Singleline))
                    throw new Exception("Formato del rut no es correcto. Ejemplo: 99999999-K");

                ////
                //// Validar el Rut
                if (!FUNCIONES.HefValidaciones.ValidaRut(rutEmisor))
                    throw new Exception("Rut ingresado no es valido.");

                ////
                //// Valide el trackid
                if (!Regex.IsMatch(trackid, "[0-9]{1,15}", RegexOptions.Singleline))
                    throw new Exception("Trackid ingresado no es valido.");

                ////
                //// Validar que exista el certificado.
                if (string.IsNullOrEmpty(Cn))
                    throw new Exception("Debe ingresar el cn del certificado a utilizar.");

                ////
                //// Recupera el certificado
                X509Certificate2 Certificado = FUNCIONES.HefCertificados.RecuperarCertificado(Cn);
                if (Certificado == null)
                    throw new Exception("No encuentro o no tengo permisos para ver el Certifiado '" +Cn + "'");

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
                
                ////
                //// Iniciar la consulta al SII
                resp = NEGOCIO.HefConsultas.EstadoDteTrackid(rutEmisor, trackid, Certificado, ambiente);
                               
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
        /// <param name="Cn">ombre comun del certificado instlada en el pc actual</param>
        /// <returns></returns>
        public static HefRespuesta EstadoBolTrackid(string rutEmisor, string trackid, string Cn, AmbienteSII ambiente)
        {

            ////
            //// Beta?
            //HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            //if (!respVal.EsCorrecto)
            //    return respVal;

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "EstadoBolTrackid()";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Iniciar validación de los elementos de la consulta
                rutEmisor = rutEmisor.ToUpper();
                if (!Regex.IsMatch(rutEmisor, "[0-9]{3,8}-[0-9K]{1}", RegexOptions.Singleline))
                    throw new Exception("Formato del rut no es correcto. Ejemplo: 99999999-K");

                ////
                //// Validar el Rut
                if (!FUNCIONES.HefValidaciones.ValidaRut(rutEmisor))
                    throw new Exception("Rut ingresado no es valido.");

                ////
                //// Valide el trackid
                if (!Regex.IsMatch(trackid, "[0-9]{1,15}", RegexOptions.Singleline))
                    throw new Exception("Trackid ingresado no es valido.");

                ////
                //// Validar que exista el certificado.
                if (string.IsNullOrEmpty(Cn))
                    throw new Exception("Debe ingresar el cn del certificado a utilizar.");

                ////
                //// Recupera el certificado
                X509Certificate2 Certificado = FUNCIONES.HefCertificados.RecuperarCertificado(Cn);
                if (Certificado == null)
                    throw new Exception("No encuentro o no tengo permisos para ver el Certifiado '" + Cn + "'");

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

                ////
                //// Iniciar la consulta al SII
                resp = NEGOCIO.HefConsultas.EstadoBolTrackid(rutEmisor, trackid, Certificado, ambiente);

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
        /// <param name="Cn">ombre comun del certificado instlada en el pc actual</param>
        /// <returns></returns>
        public static HefRespuesta EstadoBoleta(
            string rutEmisor,
                string tipo,
                    string folio,
                        string rutReceptor,
                            string monto,
                                string fchEmision,
                                    string cn,
                                        AmbienteSII ambiente
                                    
                                    )
        {

            ////
            //// Beta?
            HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            if (!respVal.EsCorrecto)
                return respVal;


            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "EstadoBoleta";

            ////
            //// Inicie la validacion de los parametros 
            ////

            ////
            //// Validar el formato del rut emisor
            rutEmisor = rutEmisor.ToUpper();
            if (!Regex.IsMatch(rutEmisor, "[0-9]{3,8}-[0-9K]{1}", RegexOptions.Singleline))
                throw new Exception("Formato del rut emisor no es correcto. Ejemplo: 99999999-K");

            ////
            //// Validar el Rut
            if (!FUNCIONES.HefValidaciones.ValidaRut(rutEmisor))
                throw new Exception("Rut emisor ingresado no es valido.");

            ////
            //// Validar el formato del rut receptor
            rutReceptor = rutReceptor.ToUpper();
            if (!Regex.IsMatch(rutReceptor, "[0-9]{3,8}-[0-9K]{1}", RegexOptions.Singleline))
                throw new Exception("Formato del rut receptor no es correcto. Ejemplo: 99999999-K");

            ////
            //// Validar el Rut
            if (!FUNCIONES.HefValidaciones.ValidaRut(rutReceptor))
                throw new Exception("Rut receptor ingresado no es valido.");

            ////
            //// Validacion del tipo
            if (!Regex.IsMatch(tipo, "39|41", RegexOptions.Singleline))
                throw new Exception("Tipo de documento debe ser 39|41.");

            ////
            //// Validacion del folio
            if (!Regex.IsMatch(folio, "[0-9]{1,10}", RegexOptions.Singleline))
                throw new Exception("Folio del documento no es valido.");

            ////
            //// Validacion del monto
            if (!Regex.IsMatch(monto, "[0-9]{1,18}", RegexOptions.Singleline))
                throw new Exception("Monto ingresado no es valido.");

            ////
            //// Validacion fecha emision
            if (!Regex.IsMatch(fchEmision, "[0-9]{4}-[0-9]{2}-[0-9]{2}", RegexOptions.Singleline))
                throw new Exception("Fecha ingresada no tiene formato correcto. Ejemplo: yyyy-mm-dd.");

            ////
            //// Es una fecha valida?
            DateTime dtt;
            if (!DateTime.TryParse(fchEmision, out dtt))
                throw new Exception("Fecha ingresada no es valida. Ejemplo: yyyy-mm-dd.");

            ////
            //// Actualice la fecha
            fchEmision = dtt.ToString("dd-MM-yyyy");

            ////
            //// Validar que exista el certificado.
            if (string.IsNullOrEmpty(cn))
                throw new Exception("Debe ingresar el cn del certificado a utilizar.");

            ////
            //// Validar que exista el certificado.
            X509Certificate2 certificado = FUNCIONES.HefCertificados.RecuperarCertificado(cn);
            if (certificado == null)
                throw new Exception("El certificado no existe o no se tiene acceso a el.");

            ////
            //// Tiene private key?
            if (!certificado.HasPrivateKey)
                throw new Exception("El certificado no tiene asignada una private key.");

            ////
            //// El certificado esta vigente?
            DateTime dt;
            if (!DateTime.TryParse(certificado.GetExpirationDateString(), out dt))
                throw new Exception("No puedo detectar la fecha de expiración del certificado.");
            if (DateTime.Now > dt)
                throw new Exception("El certificado se encuentra expirado.");

            ////
            //// Iniciar
            try
            {
                ////
                //// Consultar al sii 
                resp = NEGOCIO.HefConsultas.EstadoBoleta(
                rutEmisor,
                    tipo,
                        folio,
                            rutReceptor,
                                monto,
                                    fchEmision,
                                        certificado,
                                            ambiente
                );


            }
            catch (Exception ex)
            {
                ////
                //// Notificar el error
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Consultar a la capa de negocio por los datos
            return resp;


        }

    }

}
