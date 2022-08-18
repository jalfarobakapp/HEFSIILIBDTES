using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;


namespace HEFSIILIBDTES.FUNCIONES
{
    /// <summary>
    /// Metodos relacionados con los certificados
    /// </summary>
    public class HefCertificados
    {
        /// <summary>
        /// Recupera un certificado
        /// </summary>
        /// <param name="CN"></param>
        /// <returns></returns>
        public static X509Certificate2 RecuperarCertificado(string CN)
        {

            ////
            //// Respuesta
            X509Certificate2 certificado = null;

            ////
            //// Certificado que se esta buscando
            if (string.IsNullOrEmpty(CN) || CN.Length == 0)
                return null;

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


                //// X509Certificate2 certificado = new X509Certificate2("path", "password");


            }
            catch (Exception)
            {
                certificado = null;
            }

            return certificado;

        }

        internal static void firmarDocumentoXml(ref XmlDocument xmldocument, X509Certificate2 certificado, string referenciaUri)
        {
            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(xmldocument);

            // Add the key to the SignedXml document.  'key'
            signedXml.SigningKey = certificado.PrivateKey;

            // Get the signature object from the SignedXml object.
            Signature XMLSignature = signedXml.Signature;

            // Create a reference to be signed.  Pass "" 
            // to specify that all of the current XML
            // document should be signed.
            Reference reference = new Reference();
            reference.Uri = referenciaUri;

            // Add the Reference object to the Signature object.
            XMLSignature.SignedInfo.AddReference(reference);

            // Add an RSAKeyValue KeyInfo (optional; helps recipient find key to validate).
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new RSAKeyValue((RSA)certificado.PrivateKey));

            ////
            //// Agregar información del certificado x509
            //// X509Certificate MSCert = X509Certificate.CreateFromCertFile(Certificate);
            keyInfo.AddClause(new KeyInfoX509Data(certificado));

            // Add the KeyInfo object to the Reference object.
            XMLSignature.KeyInfo = keyInfo;


            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();
            
            ///
            /// inserte la firma en el DTE
            xmldocument.DocumentElement.AppendChild(xmldocument.ImportNode(xmlDigitalSignature, true));

        }


    }
}
