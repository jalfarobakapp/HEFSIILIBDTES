using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace HEFSIILIBDTES.FUNCIONES
{
    /// <summary>
    /// Metodos de firmas de documentos
    /// </summary>
    internal class HefFirmas
    {
        /// <summary>
        /// Firma del documento
        /// </summary>
        /// <param name="xmldocument"></param>
        /// <param name="certificado"></param>
        /// <param name="referenciaUri"></param>
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

        /// <summary>
        /// Firma la semilla para ser enviada al SII
        /// </summary>
        /// <param name="documento"></param>system.
        /// <param name="certificado"></param>
        /// <returns></returns>
        internal static string firmarDocumentoSemilla(string documento, X509Certificate2 certificado)
        {

            ////
            //// Cree un nuevo documento xml y defina sus caracteristicas
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(documento);

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(doc);

            // Add the key to the SignedXml document.  'key'
            signedXml.SigningKey = certificado.PrivateKey;

            // Get the signature object from the SignedXml object.
            Signature XMLSignature = signedXml.Signature;

            // Create a reference to be signed.  Pass "" 
            // to specify that all of the current XML
            // document should be signed.
            Reference reference = new Reference("");

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

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

            // Append the element to the XML document.
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));


            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            // Save the signed XML document to a file specified
            // using the passed string.
            //XmlTextWriter xmltw = new XmlTextWriter(@"d:\ResultadoFirma.xml", new UTF8Encoding(false));
            //doc.WriteTo(xmltw);
            //xmltw.Close();
            return doc.InnerXml;

        }


    }
}
