using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HEFSIILIBDTES.FUNCIONES
{
    /// <summary>
    /// Metodos de validciones
    /// </summary>
    internal class HefValidaciones
    {

        /// <summary>
        /// Inicia la validación de todos los ruts del documento actual
        /// </summary>
        /// <param name="sDoc"></param>
        /// <returns></returns>
        internal static HefRespuesta ValidarTodosLosRuts(string sDoc)
        { 
        
            ////
            //// Iniciar la respuesta del porceso
            HefRespuesta resp = new HefRespuesta();
            resp.Proceso = "Validación de todos los ruts del documento";

            ////
            //// Iniciar el proceso
            try
            {

                ////
                //// Recupre la secuencia de ruts a validar
                List<string> targets = new List<string>();
                targets.Add("<RutEmisor>(.*?)</RutEmisor>");
                targets.Add("<RutEnvia>(.*?)</RutEnvia>");
                targets.Add("<RutReceptor>(.*?)</RutReceptor>");
                targets.Add("<RUTEmisor>(.*?)</RUTEmisor>");
                targets.Add("<RUTRecep>(.*?)</RUTRecep>");

                ////
                //// Recupere los rut seleccionados
                foreach (string target in targets)
                {
                    MatchCollection ruts = Regex.Matches(sDoc, target);
                    if (ruts.Count > 0)
                    {

                        foreach (Match rut in ruts)
                        { 
                            
                        
                        }
                    
                    }
                
                }

            }
            catch (Exception ex)
            {
                ////
                //// Notificar al usuario
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible reaalizar la comprobación de rut del documento";
                resp.Detalle = ex.Message;
                
            }
        

            ////
            //// regrese el valor de retorno
            return resp;
        
        }

        /// <summary>
        /// Metodo de validación de rut con digito verificador
        /// dentro de la cadena
        /// </summary>
        /// <param name="rut">string</param>
        /// <returns>booleano</returns>
        internal static bool ValidaRut(string rut)
        {
            rut = rut.Replace(".", "").ToUpper();
            Regex expresion = new Regex("^([0-9]+-[0-9K])$");
            string dv = rut.Substring(rut.Length - 1, 1);
            if (!expresion.IsMatch(rut))
            {
                return false;
            }
            char[] charCorte = { '-' };
            string[] rutTemp = rut.Split(charCorte);
            if (dv != Digito(int.Parse(rutTemp[0])))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// método que calcula el digito verificador a partir
        /// de la mantisa del rut
        /// </summary>
        /// <param name="rut"></param>
        /// <returns></returns>
        internal static string Digito(int rut)
        {
            int suma = 0;
            int multiplicador = 1;
            while (rut != 0)
            {
                multiplicador++;
                if (multiplicador == 8)
                    multiplicador = 2;
                suma += (rut % 10) * multiplicador;
                rut = rut / 10;
            }
            suma = 11 - (suma % 11);
            if (suma == 11)
            {
                return "0";
            }
            else if (suma == 10)
            {
                return "K";
            }
            else
            {
                return suma.ToString();
            }
        }

        /// <summary>
        /// Inicia la validación de la fecha de resolucion
        /// </summary>
        /// <returns></returns>
        internal static bool ValidarFchResolucion(string fchResol)
        {

            ////
            //// Es valido?
            bool esValido = true;

            ////
            //// La forma es correcta?
            if (!Regex.IsMatch(fchResol, "^\\d{4}-\\d{2}-\\d{2}$"))
                esValido = false;

            ////
            //// Valide que la fecha sea valida
            DateTime dt;
            if (!DateTime.TryParse(fchResol, out dt ))
                esValido = false;

            ////
            //// Regresar el valor de retorno
            return esValido;
                    
        }


    }

}
