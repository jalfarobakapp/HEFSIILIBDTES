using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HEFSIILIBDTES.NEGOCIO
{
    /// <summary>
    /// Metodos relacionados con los certificados
    /// </summary>
    internal class HefCertificados
    {

        /// <summary>
        /// Recupera un determinado certificado para poder firmar un documento
        /// </summary>
        /// <param name="CN">Nombre del certificado que se busca</param>
        /// <returns>X509Certificate2</returns>
        internal static X509Certificate2 ObtenerCertificado(string CN)
        {

            ////
            //// Respuesta
            X509Certificate2 certificado = null;

            ////
            //// Certificado que se esta buscando
            if (string.IsNullOrEmpty(CN) || CN.Length == 0)
                return certificado;

            ////
            //// Inicie la busqueda del certificado
            try
            {

                ////
                //// Abra el repositorio de certificados para buscar el indicado
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection Certificados1 = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection Certificados2 = Certificados1.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection Certificados3 = Certificados2.Find(X509FindType.FindBySubjectName, CN, false);

                ////
                //// Si hay certificado disponible envíe el primero
                if (Certificados3 != null && Certificados3.Count != 0)
                    certificado = Certificados3[0];

                ////
                //// Cierre el almacen de sertificados
                store.Close();


            }
            catch (Exception)
            {
                certificado = null;
            }


            ////
            //// Regrese el valor de retorno 
            return certificado;

        }

        /// <summary>
        /// Recupera el nombre del sujeto del certificado
        /// </summary>
        /// <param name="certificado">x509Certificate2</param>
        /// <returns>Regresa el nombre del dueño del certificado</returns>
        internal static string RecuperarNombre(X509Certificate2 certificado)
        {

            ////
            //// Valor a regresar
            string Nombre = string.Empty;

            ////
            //// Recupere el nombre del dueño del certificado
            try
            {
                string cadena = certificado.SubjectName.Name;
                if (!string.IsNullOrEmpty(cadena))
                {
                    string[] elementos = cadena.Split(',');
                    if (elementos != null && elementos.Length > 0)
                    {

                        foreach (string cn in elementos)
                        {
                            if (cn.Contains("CN"))
                            {
                                Nombre = cn.Replace("CN=", string.Empty).Trim();

                            }

                        }



                    }

                }

            }
            catch (Exception)
            {
                Nombre = null;
            }


            ////
            //// Regrese el valor de retorno
            return Nombre;

        }

        /// <summary>
        /// Recupera todos los nombres de los certificados
        /// </summary>
        /// <returns></returns>
        internal static List<string> ListaDeCertificados()
        {

            ////
            //// Inicie el resultado
            List<string> Resultado = new List<string>();
            Resultado.Add("< Seleccione un certificado >");

            try
            {
                ////
                //// Abra el repositorio de certificados para buscar el indicado
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection Certificados1 = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection Certificados2 = Certificados1.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

                ////
                //// Recupere los nombres canonicos de los certificados
                foreach (X509Certificate2 cert in Certificados2)
                {
                    string cn = RecuperarNombre(cert);
                    if (cn != null)
                        Resultado.Add(cn);
                }

            }
            catch (Exception)
            {

                Resultado = null;
            }

            ////
            //// Regrese el valor de resultado
            return Resultado.OrderBy(P => P).ToList();

        }

        /// <summary>
        /// Permite recuperar un certificado
        /// </summary>
        /// <param name="FullPath">Fullpath del archivo pfx</param>
        /// <param name="Pass">PAssword del archivo</param>
        /// <returns>Certificado X509Certificate2</returns>
        internal static X509Certificate2 RecuperarCertificado(string FullPath, string Pass)
        {
            ////
            //// Iniciar la respuesta
            X509Certificate2 cert = null;

            ////
            //// Iniciar la recuperación del certificado
            try
            {
                ////
                //// Reconstruír el certificado utilizando el contructor del mismo.
                cert = new X509Certificate2(FullPath, Pass);
            }
            catch
            {
                cert = null;
            }

            ////
            //// regrese el valor de retorno
            return cert;

        }

    }
}
