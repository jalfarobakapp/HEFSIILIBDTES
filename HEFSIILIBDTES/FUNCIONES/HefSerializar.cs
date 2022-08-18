using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace HEFSIILIBDTES.FUNCIONES
{
    /// <summary>
    /// Serializa el objeto actual
    /// </summary>
    internal class HefSerializar
    {
        /// <summary>
        /// Serializa el objeto actual
        /// </summary>
        internal static HefRespuesta SerializarObjeto(object ob)
        {

            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "SerializarObjeto";

            ////
            //// Iniciar los elementos necesarios
            StreamWriter stWriter = null;
            XmlSerializer xmlSerializer;
            
            ////
            //// Iniciar el proceso
            try
            {
                
                ////
                //// Configure la serializacion
                xmlSerializer = new XmlSerializer(ob.GetType());
                MemoryStream memStream = new MemoryStream();
                stWriter = new StreamWriter(memStream);

                ////
                //// Iniciar el xml namespace 
                System.Xml.Serialization.XmlSerializerNamespaces xs = new XmlSerializerNamespaces();
                xs.Add("", "");

                ////
                //// Serializar
                xmlSerializer.Serialize(stWriter, ob, xs);

                ////
                //// Normalizar el documento
                string r = Encoding.GetEncoding("UTF-8").GetString(memStream.GetBuffer());
                r = Regex.Replace(
                    r,
                        "<\\?.*?\\?>",
                            "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>",
                                RegexOptions.Singleline
                    );
                
                ////
                //// Agregar el namespace del SII
                r = Regex.Replace(
                    r,
                        "<DTE version=\"1.0\">",
                            "<DTE version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\">",
                                RegexOptions.Singleline
                    );

                ////
                //// eliminar campos 
                r = Regex.Replace(
                    r,
                        "<Retenedor />",
                            "",
                            RegexOptions.Singleline
                    );
                
                ////
                //// Limpie los caracteres null 
                r = Regex.Replace(
                    r,
                        "\\0",
                            "",
                            RegexOptions.Singleline
                    );

                ////
                //// Limpie los caracteres null 
                r = Regex.Replace(
                    r,
                        "\r\n\\s+\r\n",
                            "\r\n",
                            RegexOptions.Singleline
                    );

                ////
                //// Recuperar la respuesta
                resp.EsCorrecto = true;
                resp.Resultado = r;

            }
            catch( Exception ex )
            {

                ////
                //// Notificar el problema
                resp.EsCorrecto = false;
                resp.Proceso = "SerializarObjeto";
                resp.Mensaje = "No fue posible serializar el objeto";
                resp.Detalle = ex.Message;

            }
            finally
            {
                ////
                //// Destruya el objeto stWriter
                if (stWriter != null) stWriter.Close();

            }


            ////
            //// Normalizar el documento


            ////
            //// Regrese el valor de retorno
            return resp;
                
        }

    }

}
