using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HEFSIILIBDTES
{
    /// <summary>
    /// Metodos relacionados con la publicación de boletas electrónicas
    /// </summary>
    public class HefPublicador
    {

        /// <summary>
        /// Permite realizar la publicación de varias boletas electrónicas por lotes de 100 documentos.
        /// </summary>
        public static HefRespuesta PublicadoBoletasPorLotes(string RutEmisor, string RutEnvia, string FchResol, int NroResol, string cert_path, string cert_pass, List<string> mis_boletas, string xsd_path)
        {

            ////
            //// Beta?
            HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            if (!respVal.EsCorrecto)
                return respVal;


            ////
            //// Inicie la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "Publicador de boletas por lotes";

            ////
            //// Inicie el proceso
            try
            {

                ////
                //// Validacion de los parametros del procedimiento
                #region VALIDACION DE PARAMETROS

                ////
                //// Normalice los ruts
                RutEmisor = RutEmisor.Replace(".", "").ToUpper();
                RutEnvia = RutEnvia.Replace(".", "").ToUpper();

                ////
                //// Valide el rut del emisor del documento
                if (!FUNCIONES.HefValidaciones.ValidaRut(RutEmisor))
                    throw new Exception("El rut del emisor no es valido. Ejemplo 99999999-K");

                ////
                //// Valide el rut del emisor del documento
                if (!FUNCIONES.HefValidaciones.ValidaRut(RutEnvia))
                    throw new Exception("El rut enviador no es valido. Ejemplo 99999999-K");

                ////
                //// Valide la fecha de resolucion
                if (!FUNCIONES.HefValidaciones.ValidarFchResolucion(FchResol) )
                    throw new Exception("El rut enviador no es valido. Ejemplo 99999999-K");

                ////
                //// Valide el numero de resolucion
                if ( !Regex.IsMatch(NroResol.ToString(), "\\d+" ) )
                    throw new Exception("Número de resolución no es valido.");

                ////
                //// Existe el archivo xsd?
                if (!File.Exists(xsd_path))
                    throw new Exception("No se encuentra o no se tiene acceso al schema de envio de boletas(XSD).");


                #endregion

                ////
                //// Inicie la validación de los parametros de la funcion
                #region VALIDACION DEL CERTIFICADO

                ////
                //// Iniciar el certififacdo
                X509Certificate2 Certificado = null;


                ////
                //// Valique que el certificado exista
                if (!File.Exists(cert_path))
                    throw new Exception("El certificado no existe o no se tiene acceso a el.");

                ////
                //// Valique password del certificado
                if (string.IsNullOrEmpty(cert_pass))
                    throw new Exception("La password del certificadono puede estar vacía.");

                ////
                //// Recupere el certificado utilizando constructor 
                //// NOTA:
                //// Es posible que esta linea genere un error que indique 'Acceso denegado'
                //// Indicar al antiirus que deje pasar este proceso. Con eso soluciona el
                //// problema.
                Certificado = new X509Certificate2(cert_path, cert_pass, X509KeyStorageFlags.UserKeySet);

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

                ////
                //// Validación de los elementos boletas
                #region VALIDACION DE LOS DOCUMENTOS BOLETAS PASADOS COMO ARGUMENTOS

                ////
                //// Todos los items de la lista tienen documentos DTE del tipo boleta?
                List<string> _boletas_temporales = new List<string>();
                foreach (string boleta in mis_boletas)
                {
                    MatchCollection mBols = Regex.Matches(boleta, "<DTE.*?</DTE>", RegexOptions.Singleline);
                    foreach (Match mBol in mBols)
                        _boletas_temporales.Add(mBol.Value);
                }

                ////
                //// Cuantos elementos tenemos?
                if (_boletas_temporales.Count == 0)
                    throw new Exception("La lista de boletas no contiene elementos boletas validos para iniciar el proceso.");

                ////
                //// Remueva todos los elementos que no sean boletas electrónicas
                //// Recuerde que pueden haber notas de crédito y débito.
                _boletas_temporales.RemoveAll(delegate (string boleta)
                {
                    if (Regex.IsMatch(boleta, "<TipoDTE>39</TipoDTE>|<TipoDTE>41</TipoDTE>|<TipoDTE>61</TipoDTE>|<TipoDTE>56</TipoDTE>", RegexOptions.Singleline))
                        return false;
                    else
                        return true;

                });

                ////
                //// Cuantos elementos tenemos?
                if (_boletas_temporales.Count == 0)
                    throw new Exception("La ista de boletas no tiene elementos boletas validos.");


                #endregion

                ////
                //// Inicia la publicación del documento
                resp = NEGOCIO.HefPublicadores.PublicarBoletas(
                    RutEmisor,
                        RutEnvia,
                            FchResol,
                                NroResol,
                                    cert_path,
                                        cert_pass,
                                            _boletas_temporales,
                                             Certificado,
                                                xsd_path);
                    
                    

            }
            catch ( Exception ex )
            {
                ////
                //// Notifique al usuario
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
                resp.Resultado = null;
                
            }

            ////
            //// Regrese el valor de retorno
            return resp;

        
        }

        /// <summary>
        /// Permite realizar la publicación de varias boletas electrónicas por lotes de 100 documentos.
        /// </summary>
        public static HefRespuesta PublicadoBoletasPorLotes(string RutEmisor, string RutEnvia, string FchResol, int NroResol, string cert_cn, List<string> mis_boletas, string xsd_path)
        {

            ////
            //// Beta?
            HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            if (!respVal.EsCorrecto)
                return respVal;

            ////
            //// Inicie la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "Publicador de boletas por lotes";

            ////
            //// Inicie el proceso
            try
            {

                ////
                //// Validacion de los parametros del procedimiento
                #region VALIDACION DE PARAMETROS

                ////
                //// Normalice los ruts
                RutEmisor = RutEmisor.Replace(".", "").ToUpper();
                RutEnvia = RutEnvia.Replace(".", "").ToUpper();

                ////
                //// Valide el rut del emisor del documento
                if (!FUNCIONES.HefValidaciones.ValidaRut(RutEmisor))
                    throw new Exception("El rut del emisor no es valido. Ejemplo 99999999-K");

                ////
                //// Valide el rut del emisor del documento
                if (!FUNCIONES.HefValidaciones.ValidaRut(RutEnvia))
                    throw new Exception("El rut enviador no es valido. Ejemplo 99999999-K");

                ////
                //// Valide la fecha de resolucion
                if (!FUNCIONES.HefValidaciones.ValidarFchResolucion(FchResol))
                    throw new Exception("El rut enviador no es valido. Ejemplo 99999999-K");

                ////
                //// Valide el numero de resolucion
                if (!Regex.IsMatch(NroResol.ToString(), "\\d+"))
                    throw new Exception("Número de resolución no es valido.");

                ////
                //// Existe el archivo xsd?
                if (!File.Exists(xsd_path))
                    throw new Exception("No se encuentra o no se tiene acceso al schema de envio de boletas(XSD).");


                #endregion

                ////
                //// Inicie la validación de los parametros de la funcion
                #region VALIDACION DEL CERTIFICADO

                ////
                //// Iniciar el certififacdo
                X509Certificate2 Certificado = NEGOCIO.HefCertificados.ObtenerCertificado(cert_cn);
                if ( Certificado == null )
                    throw new Exception("El certificado no existe o no se tiene acceso a el.");

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

                ////
                //// Validación de los elementos boletas
                #region VALIDACION DE LOS DOCUMENTOS BOLETAS PASADOS COMO ARGUMENTOS

                ////
                //// Todos los items de la lista tienen documentos DTE del tipo boleta?
                List<string> _boletas_temporales = new List<string>();
                foreach (string boleta in mis_boletas)
                {
                    MatchCollection mBols = Regex.Matches(boleta, "<DTE.*?</DTE>", RegexOptions.Singleline);
                    foreach (Match mBol in mBols)
                        _boletas_temporales.Add(mBol.Value);
                }

                ////
                //// Cuantos elementos tenemos?
                if (_boletas_temporales.Count == 0)
                    throw new Exception("La lista de boletas no contiene elementos boletas validos para iniciar el proceso.");

                ////
                //// Remueva todos los elementos que no sean boletas electrónicas
                //// Recuerde que pueden haber notas de crédito y débito.
                _boletas_temporales.RemoveAll(delegate (string boleta)
                {
                    if (Regex.IsMatch(boleta, "<TipoDTE>39</TipoDTE>|<TipoDTE>41</TipoDTE>|<TipoDTE>61</TipoDTE>|<TipoDTE>56</TipoDTE>", RegexOptions.Singleline))
                        return false;
                    else
                        return true;

                });

                ////
                //// Cuantos elementos tenemos?
                if (_boletas_temporales.Count == 0)
                    throw new Exception("La ista de boletas no tiene elementos boletas validos.");


                #endregion

                ////
                //// Inicia la publicación del documento
                resp = NEGOCIO.HefPublicadores.PublicarBoletas(
                    RutEmisor,
                        RutEnvia,
                            FchResol,
                                NroResol,
                                    null,
                                        null,
                                            _boletas_temporales,
                                             Certificado,
                                                xsd_path);


            }
            catch (Exception ex)
            {
                ////
                //// Notifique al usuario
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
                resp.Resultado = null;

            }

            ////
            //// Regrese el valor de retorno
            return resp;




        }


        /// <summary>
        /// Permite realizar la publicación de varias boletas electrónicas por lotes de 100 documentos.
        /// </summary>
        public static HefRespuesta PublicadoBoletasPorLotes3(string RutEmisor, string RutEnvia, string FchResol, int NroResol, X509Certificate2 Certificado, List<string> mis_boletas, string xsd_path)
        {

            ////
            //// Beta?
            HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            if (!respVal.EsCorrecto)
                return respVal;


            ////
            //// Inicie la respuesta del proceso
            HefRespuesta resp = new HefRespuesta();
            resp.Mensaje = "Publicador de boletas por lotes";

            ////
            //// Inicie el proceso
            try
            {

                ////
                //// Validacion de los parametros del procedimiento
                #region VALIDACION DE PARAMETROS

                ////
                //// Normalice los ruts
                RutEmisor = RutEmisor.Replace(".", "").ToUpper();
                RutEnvia = RutEnvia.Replace(".", "").ToUpper();

                ////
                //// Valide el rut del emisor del documento
                if (!FUNCIONES.HefValidaciones.ValidaRut(RutEmisor))
                    throw new Exception("El rut del emisor no es valido. Ejemplo 99999999-K");

                ////
                //// Valide el rut del emisor del documento
                if (!FUNCIONES.HefValidaciones.ValidaRut(RutEnvia))
                    throw new Exception("El rut enviador no es valido. Ejemplo 99999999-K");

                ////
                //// Valide la fecha de resolucion
                if (!FUNCIONES.HefValidaciones.ValidarFchResolucion(FchResol))
                    throw new Exception("El rut enviador no es valido. Ejemplo 99999999-K");

                ////
                //// Valide el numero de resolucion
                if (!Regex.IsMatch(NroResol.ToString(), "\\d+"))
                    throw new Exception("Número de resolución no es valido.");

                ////
                //// Existe el archivo xsd?
                if (!File.Exists(xsd_path))
                    throw new Exception("No se encuentra o no se tiene acceso al schema de envio de boletas(XSD).");


                #endregion

                               ////
                //// El certificado esta expirado?
                DateTime fecha_expiracion;
                if (DateTime.TryParse(Certificado.GetExpirationDateString(), out fecha_expiracion))
                    if (fecha_expiracion < DateTime.Now)
                        throw new Exception("El certificado esta expirado, revise la fecha de expiración.");

                ////
                //// Validación de los elementos boletas
                #region VALIDACION DE LOS DOCUMENTOS BOLETAS PASADOS COMO ARGUMENTOS

                ////
                //// Todos los items de la lista tienen documentos DTE del tipo boleta?
                List<string> _boletas_temporales = new List<string>();
                foreach (string boleta in mis_boletas)
                {
                    MatchCollection mBols = Regex.Matches(boleta, "<DTE.*?</DTE>", RegexOptions.Singleline);
                    foreach (Match mBol in mBols)
                        _boletas_temporales.Add(mBol.Value);
                }

                ////
                //// Cuantos elementos tenemos?
                if (_boletas_temporales.Count == 0)
                    throw new Exception("La lista de boletas no contiene elementos boletas validos para iniciar el proceso.");

                ////
                //// Remueva todos los elementos que no sean boletas electrónicas
                //// Recuerde que pueden haber notas de crédito y débito.
                _boletas_temporales.RemoveAll(delegate (string boleta)
                {
                    if (Regex.IsMatch(boleta, "<TipoDTE>39</TipoDTE>|<TipoDTE>41</TipoDTE>|<TipoDTE>61</TipoDTE>|<TipoDTE>56</TipoDTE>", RegexOptions.Singleline))
                        return false;
                    else
                        return true;

                });

                ////
                //// Cuantos elementos tenemos?
                if (_boletas_temporales.Count == 0)
                    throw new Exception("La ista de boletas no tiene elementos boletas validos.");


                #endregion

                ////
                //// Inicia la publicación del documento
                resp = NEGOCIO.HefPublicadores.PublicarBoletas(
                    RutEmisor,
                        RutEnvia,
                            FchResol,
                                NroResol,
                                     _boletas_temporales,
                                             Certificado,
                                                xsd_path);



            }
            catch (Exception ex)
            {
                ////
                //// Notifique al usuario
                resp.EsCorrecto = false;
                resp.Detalle = ex.Message;
                resp.Resultado = null;
                resp.Trackid = "0";
                resp.Mensaje = ex.Message;  
            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }


    }


}
