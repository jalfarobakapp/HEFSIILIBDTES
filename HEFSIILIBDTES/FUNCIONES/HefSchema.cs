using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace HEFSIILIBDTES.FUNCIONES
{
    /// <summary>
    /// Metodos de validación de schemas
    /// </summary>
    internal class HefSchema
    {

        /// <summary>
        /// Inicia la validación de schema del documento DTE
        /// </summary>
        internal static List<string> ValidarSchemaDTE(string sDTE, string sSchema)
        {
            ////
            //// Difina la constante namespace SII
            const string NS = "http://www.sii.cl/SiiDte";

            ////
            //// Defina la lista de errores a rtegresar
            List<string> errores = new List<string>();


            ////
            //// Inicie la validacion de los schemas
            try
            {
                ////
                //// Cree el administrador del schema
                XmlSchemaSet schemas = new XmlSchemaSet();

                ////
                //// Asigne el schema al administrador
                schemas.Add(NS, sSchema);

                ////
                //// Recupere el documento xml (DTE) a validar
                XDocument DocumentoXml = XDocument.Parse(sDTE);

                ////
                //// Inicie la validacion del documento xml contra su schema
                DocumentoXml.Validate(schemas, (o, e) => { errores.Add(e.Message); });

            }
            catch (Exception ex)
            {
                errores.Add(ex.Message);
            }

            ////
            //// Regrese el valor de retorno 
            return errores;

        
        
        }

        /// <summary>
        /// Agrega el nodo TED al Documento Actual
        /// </summary>
        /// <returns></returns>
        internal static string AgregarTED( string sDoc)
        {
            ////
            //// Cree el nodo ted
            string sTED = "";
            sTED += "<!-- TED BEGIN -->\r\n";
            sTED += "<TED version=\"1.0\">\r\n";
            sTED += "<DD>\r\n";
            sTED += "<RE>XX</RE>\r\n";
            sTED += "<TD>XX</TD>\r\n";
            sTED += "<F>XX</F>\r\n";
            sTED += "<FE>XX</FE>\r\n";
            sTED += "<RR>XX</RR>\r\n";
            sTED += "<RSR>XX</RSR>\r\n";
            sTED += "<MNT>XX</MNT>\r\n";
            sTED += "<IT1>XX</IT1>\r\n";
            sTED += "<CAF/>\r\n";
            sTED += "<TSTED>XX</TSTED>\r\n";
            sTED += "</DD>\r\n";
            sTED += "<FRMT algoritmo=\"SHA1withRSA\">XX</FRMT>\r\n";
            sTED += "</TED>\r\n";
            sTED += "<!-- TED END -->\r\n";

            ////
            //// Inserte el nodo ted en el documento
            sDoc = Regex.Replace(
                sDoc,
                    "<TmstFirma>",
                        sTED + "<TmstFirma>",
                            RegexOptions.Singleline
                );
        
            ////
            //// Regresar el valor de retorno
            return sDoc;
        }

        /// <summary>
        /// Simula un documento completo para la validación de schema
        /// </summary>
        internal static string SimulaDocumentoDteCompleto(string sDoc)
        {

            ////
            //// Cree el ted
            string sTED = "";
            sTED += "<TED version=\"1.0\">\r\n";
            sTED += "	<DD>\r\n";
            sTED += "		<RE>77218967-2</RE>\r\n";
            sTED += "		<TD>33</TD><F>1</F>\r\n";
            sTED += "		<FE>2020-09-17</FE>\r\n";
            sTED += "		<RR>60803000-K</RR>\r\n";
            sTED += "		<RSR>SERVICIO DE IMPUESTOS INTERNOS DIRECCION</RSR>\r\n";
            sTED += "		<MNT>620711</MNT>\r\n";
            sTED += "		<IT1>Cajón AFECTO</IT1>\r\n";
            sTED += "		<CAF version=\"1.0\">\r\n";
            sTED += "			<DA>\r\n";
            sTED += "				<RE>77218967-2</RE>\r\n";
            sTED += "				<RS>COMERCIAL Y SERVICIOS PRALUNGO LIMITADA</RS>\r\n";
            sTED += "				<TD>33</TD>\r\n";
            sTED += "				<RNG>\r\n";
            sTED += "					<D>1</D><H>50</H>\r\n";
            sTED += "				</RNG>\r\n";
            sTED += "				<FA>2020-09-17</FA>\r\n";
            sTED += "				<RSAPK>\r\n";
            sTED += "					<M>1/27bSOoaqaz2YmZsT8LtabFbavqY9YR/cdt9vK3Sfwk8Q1g4Ye+l7zxt6pW2IOWMxKH5mqloeoiN8D/yhVPVw==</M><E>Aw==</E>\r\n";
            sTED += "				</RSAPK>\r\n";
            sTED += "				<IDK>100</IDK>\r\n";
            sTED += "			</DA>\r\n";
            sTED += "			<FRMA algoritmo=\"SHA1withRSA\">T2yAfkW95vgYO3Q47mp5qL4rsJyZwo4VDjrx39ZOuAbgQnzLEYWwQFWMrVHr5YfQDdgmnZVzAxuu3tp3BggkDA==</FRMA>\r\n";
            sTED += "		</CAF>\r\n";
            sTED += "		<TSTED>2020-09-17T16:15:08</TSTED>\r\n";
            sTED += "	</DD>\r\n";
            sTED += "	<FRMT algoritmo=\"SHA1withRSA\">fJcft/0w/Mssi1LIk3FZXENh1GjVtwLazZrAGWLxp3bkoz6Op7OMVVRQXnRcgwLSOG3CH+YJa15x+8Xyw3Ajew==</FRMT>\r\n";
            sTED += "</TED>\r\n";


            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "<TmstFirma>",
                        sTED + "<TmstFirma>",
                            RegexOptions.Singleline
                );


            ////
            //// Crear la firma
            string sSignature = "";
            sSignature += "<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">\r\n";
            sSignature += "	<SignedInfo>\r\n";
            sSignature += "		<CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" />\r\n";
            sSignature += "		<SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" />\r\n";
            sSignature += "		<Reference URI=\"#R772189672F1T33\">\r\n";
            sSignature += "			<DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" />\r\n";
            sSignature += "			<DigestValue>9k5mV9klJBV8Z0s3+0Y2cuAKGog=</DigestValue>\r\n";
            sSignature += "		</Reference>\r\n";
            sSignature += "	</SignedInfo>\r\n";
            sSignature += "	<SignatureValue>Z2feYVmH+UbuARLhX1QOqoIrLFFd2ISh2yRJoGiKI1T19SP2J71w4o2+0KlFR13AgCLSSIk9U4rCwPqyDKQvuYGhhS5DyrADbME9rYB3EvtRLqzzhvxbWs8w9L9T9Lg/m8Ir653atRNspipPxJJXqRwkNTr9jpVjKjEKLb5wKxI=</SignatureValue>\r\n";
            sSignature += "	<KeyInfo>\r\n";
            sSignature += "		<KeyValue>\r\n";
            sSignature += "			<RSAKeyValue>\r\n";
            sSignature += "				<Modulus>1f6KXqTmCEMj7KCtv2fXop4rvL2Nc8tN2cPINRv646+aAcIKgrAM+Xc643NFy/gMUZPvkQubs3d2xQM5mGvCKapYDA9lHTLMoqDHLX1+g6FjbkJpCRXEF7Gl3zRnqmmRZ+CyQtJTMynR+BMsOTecNyfk+aLc88XEpPLb4tZ08ws=</Modulus>\r\n";
            sSignature += "				<Exponent>AQAB</Exponent>\r\n";
            sSignature += "			</RSAKeyValue>\r\n";
            sSignature += "		</KeyValue>\r\n";
            sSignature += "		<X509Data>\r\n";
            sSignature += "			<X509Certificate>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</X509Certificate>\r\n";
            sSignature += "		</X509Data>\r\n";
            sSignature += "	</KeyInfo>\r\n";
            sSignature += "</Signature>\r\n";

            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "</Documento>",
                       "</Documento>" + sSignature,
                            RegexOptions.Singleline
                );

            ////
            //// Regerse el valor de retorno
            return sDoc;

        
        }

        /// <summary>
        /// Simula la boleta electrónica completa para aplicar schema del documento
        /// </summary>
        /// <param name="sDoc"></param>
        /// <returns></returns>
        internal static string SimulaDocumentoBolCompleto(string sDoc)
        {
    
            ////
            //// Cree el ted
            string sTED = "";
            sTED += "<TED version=\"1.0\">\r\n";
            sTED += "	<DD>\r\n";
            sTED += "		<RE>77218967-2</RE>\r\n";
            sTED += "		<TD>39</TD><F>1</F>\r\n";
            sTED += "		<FE>2020-09-17</FE>\r\n";
            sTED += "		<RR>60803000-K</RR>\r\n";
            sTED += "		<RSR>SERVICIO DE IMPUESTOS INTERNOS DIRECCION</RSR>\r\n";
            sTED += "		<MNT>620711</MNT>\r\n";
            sTED += "		<IT1>Cajón AFECTO</IT1>\r\n";
            sTED += "		<CAF version=\"1.0\">\r\n";
            sTED += "			<DA>\r\n";
            sTED += "				<RE>77218967-2</RE>\r\n";
            sTED += "				<RS>COMERCIAL Y SERVICIOS PRALUNGO LIMITADA</RS>\r\n";
            sTED += "				<TD>39</TD>\r\n";
            sTED += "				<RNG>\r\n";
            sTED += "					<D>1</D><H>50</H>\r\n";
            sTED += "				</RNG>\r\n";
            sTED += "				<FA>2020-09-17</FA>\r\n";
            sTED += "				<RSAPK>\r\n";
            sTED += "					<M>1/27bSOoaqaz2YmZsT8LtabFbavqY9YR/cdt9vK3Sfwk8Q1g4Ye+l7zxt6pW2IOWMxKH5mqloeoiN8D/yhVPVw==</M><E>Aw==</E>\r\n";
            sTED += "				</RSAPK>\r\n";
            sTED += "				<IDK>100</IDK>\r\n";
            sTED += "			</DA>\r\n";
            sTED += "			<FRMA algoritmo=\"SHA1withRSA\">T2yAfkW95vgYO3Q47mp5qL4rsJyZwo4VDjrx39ZOuAbgQnzLEYWwQFWMrVHr5YfQDdgmnZVzAxuu3tp3BggkDA==</FRMA>\r\n";
            sTED += "		</CAF>\r\n";
            sTED += "		<TSTED>2020-09-17T16:15:08</TSTED>\r\n";
            sTED += "	</DD>\r\n";
            sTED += "	<FRMT algoritmo=\"SHA1withRSA\">fJcft/0w/Mssi1LIk3FZXENh1GjVtwLazZrAGWLxp3bkoz6Op7OMVVRQXnRcgwLSOG3CH+YJa15x+8Xyw3Ajew==</FRMT>\r\n";
            sTED += "</TED>\r\n";
            
            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "<TmstFirma>",
                        sTED + "<TmstFirma>",
                            RegexOptions.Singleline
                );
            
            ////
            //// Crear la firma
            string sSignature = "";
            sSignature += "<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">\r\n";
            sSignature += "	<SignedInfo>\r\n";
            sSignature += "		<CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" />\r\n";
            sSignature += "		<SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" />\r\n";
            sSignature += "		<Reference URI=\"#R772189672F1T33\">\r\n";
            sSignature += "			<DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" />\r\n";
            sSignature += "			<DigestValue>9k5mV9klJBV8Z0s3+0Y2cuAKGog=</DigestValue>\r\n";
            sSignature += "		</Reference>\r\n";
            sSignature += "	</SignedInfo>\r\n";
            sSignature += "	<SignatureValue>Z2feYVmH+UbuARLhX1QOqoIrLFFd2ISh2yRJoGiKI1T19SP2J71w4o2+0KlFR13AgCLSSIk9U4rCwPqyDKQvuYGhhS5DyrADbME9rYB3EvtRLqzzhvxbWs8w9L9T9Lg/m8Ir653atRNspipPxJJXqRwkNTr9jpVjKjEKLb5wKxI=</SignatureValue>\r\n";
            sSignature += "	<KeyInfo>\r\n";
            sSignature += "		<KeyValue>\r\n";
            sSignature += "			<RSAKeyValue>\r\n";
            sSignature += "				<Modulus>1f6KXqTmCEMj7KCtv2fXop4rvL2Nc8tN2cPINRv646+aAcIKgrAM+Xc643NFy/gMUZPvkQubs3d2xQM5mGvCKapYDA9lHTLMoqDHLX1+g6FjbkJpCRXEF7Gl3zRnqmmRZ+CyQtJTMynR+BMsOTecNyfk+aLc88XEpPLb4tZ08ws=</Modulus>\r\n";
            sSignature += "				<Exponent>AQAB</Exponent>\r\n";
            sSignature += "			</RSAKeyValue>\r\n";
            sSignature += "		</KeyValue>\r\n";
            sSignature += "		<X509Data>\r\n";
            sSignature += "			<X509Certificate>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</X509Certificate>\r\n";
            sSignature += "		</X509Data>\r\n";
            sSignature += "	</KeyInfo>\r\n";
            sSignature += "</Signature>\r\n";

            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "</Documento>",
                       "</Documento>" + sSignature,
                            RegexOptions.Singleline
                );

            ////
            //// Cree envio boleta del documento
            string sEnvio = string.Empty;
            sEnvio += "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>\r\n";
            sEnvio += "<EnvioBOLETA version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.sii.cl/SiiDte EnvioBOLETA_v11.xsd\">\r\n";
            sEnvio += "	<SetDTE ID=\"SetDoc\">\r\n";
            sEnvio += "		<Caratula version=\"1.0\">\r\n";
            sEnvio += "			<RutEmisor>77218967-2</RutEmisor>\r\n";
            sEnvio += "			<RutEnvia>6345363-3</RutEnvia>\r\n";
            sEnvio += "			<RutReceptor>60803000-K</RutReceptor>\r\n";
            sEnvio += "			<FchResol>2020-09-17</FchResol>\r\n";
            sEnvio += "			<NroResol>0</NroResol>\r\n";
            sEnvio += "			<TmstFirmaEnv>2020-09-17T16:39:09</TmstFirmaEnv>\r\n";
            sEnvio += "			<SubTotDTE>\r\n";
            sEnvio += "				<TpoDTE>39</TpoDTE>\r\n";
            sEnvio += "				<NroDTE>1</NroDTE>\r\n";
            sEnvio += "			</SubTotDTE>\r\n";
            sEnvio += "		</Caratula>\r\n";
            sEnvio += "		<DTE/>\r\n";
            sEnvio += "	</SetDTE>\r\n";
            sEnvio += "	<Signature/>\r\n";
            sEnvio += "</EnvioBOLETA>\r\n";
            
            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sEnvio,
                    "<DTE/>",
                       Regex.Match(sDoc,"<DTE.*?</DTE>", RegexOptions.Singleline).Value,
                            RegexOptions.Singleline
                );

            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "<Signature/>",
                        sSignature,
                            RegexOptions.Singleline
                );


            ////
            //// retorne el documento
            return sDoc;
        
        }

        /// <summary>
        /// Simula el documento rcof para validar con schema
        /// </summary>
        /// <param name="sDoc"></param>
        /// <returns></returns>
        internal static string SimulaDocumentoRcofCompleto(string sDoc)
        {
            ////
            //// Crear la firma
            string sSignature = "";
            sSignature += "<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">\r\n";
            sSignature += "	<SignedInfo>\r\n";
            sSignature += "		<CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" />\r\n";
            sSignature += "		<SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" />\r\n";
            sSignature += "		<Reference URI=\"#R772189672F1T33\">\r\n";
            sSignature += "			<DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" />\r\n";
            sSignature += "			<DigestValue>9k5mV9klJBV8Z0s3+0Y2cuAKGog=</DigestValue>\r\n";
            sSignature += "		</Reference>\r\n";
            sSignature += "	</SignedInfo>\r\n";
            sSignature += "	<SignatureValue>Z2feYVmH+UbuARLhX1QOqoIrLFFd2ISh2yRJoGiKI1T19SP2J71w4o2+0KlFR13AgCLSSIk9U4rCwPqyDKQvuYGhhS5DyrADbME9rYB3EvtRLqzzhvxbWs8w9L9T9Lg/m8Ir653atRNspipPxJJXqRwkNTr9jpVjKjEKLb5wKxI=</SignatureValue>\r\n";
            sSignature += "	<KeyInfo>\r\n";
            sSignature += "		<KeyValue>\r\n";
            sSignature += "			<RSAKeyValue>\r\n";
            sSignature += "				<Modulus>1f6KXqTmCEMj7KCtv2fXop4rvL2Nc8tN2cPINRv646+aAcIKgrAM+Xc643NFy/gMUZPvkQubs3d2xQM5mGvCKapYDA9lHTLMoqDHLX1+g6FjbkJpCRXEF7Gl3zRnqmmRZ+CyQtJTMynR+BMsOTecNyfk+aLc88XEpPLb4tZ08ws=</Modulus>\r\n";
            sSignature += "				<Exponent>AQAB</Exponent>\r\n";
            sSignature += "			</RSAKeyValue>\r\n";
            sSignature += "		</KeyValue>\r\n";
            sSignature += "		<X509Data>\r\n";
            sSignature += "			<X509Certificate>MIIGWTCCBUGgAwIBAgIKEMHKKwAAABIm6DANBgkqhkiG9w0BAQUFADCB0jELMAkGA1UEBhMCQ0wxHTAbBgNVBAgTFFJlZ2lvbiBNZXRyb3BvbGl0YW5hMREwDwYDVQQHEwhTYW50aWFnbzEUMBIGA1UEChMLRS1DRVJUQ0hJTEUxIDAeBgNVBAsTF0F1dG9yaWRhZCBDZXJ0aWZpY2Fkb3JhMTAwLgYDVQQDEydFLUNFUlRDSElMRSBDQSBGSVJNQSBFTEVDVFJPTklDQSBTSU1QTEUxJzAlBgkqhkiG9w0BCQEWGHNjbGllbnRlc0BlLWNlcnRjaGlsZS5jbDAeFw0yMDA4MjcxOTUxMTRaFw0yMzA4MjcxOTUxMTRaMIHaMQswCQYDVQQGEwJDTDEiMCAGA1UECBMZTUVUUk9QT0xJVEFOQSBERSBTQU5USUFHTzERMA8GA1UEBxMIU0FOVElBR08xLjAsBgNVBAoTJUVEVUFSRE8gRVVHRU5JTyBCT1RUSU5FTExJIE1FUkNBTkRJTk8xCjAIBgNVBAsMASoxLjAsBgNVBAMTJUVEVUFSRE8gRVVHRU5JTyBCT1RUSU5FTExJIE1FUkNBTkRJTk8xKDAmBgkqhkiG9w0BCQEWGU1PTklDQUFSQVZFTkFASE9UTUFJTC5DT00wgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBANX+il6k5ghDI+ygrb9n16KeK7y9jXPLTdnDyDUb+uOvmgHCCoKwDPl3OuNzRcv4DFGT75ELm7N3dsUDOZhrwimqWAwPZR0yzKKgxy19foOhY25CaQkVxBexpd80Z6ppkWfgskLSUzMp0fgTLDk3nDcn5Pmi3PPFxKTy2+LWdPMLAgMBAAGjggKpMIICpTCCAU8GA1UdIASCAUYwggFCMIIBPgYIKwYBBAHDUgUwggEwMC0GCCsGAQUFBwIBFiFodHRwOi8vd3d3LmUtY2VydGNoaWxlLmNsL0NQUy5odG0wgf4GCCsGAQUFBwICMIHxHoHuAEUAbAAgAHIAZQBzAHAAbwBuAGQAZQByACAAZQBzAHQAZQAgAGYAbwByAG0AdQBsAGEAcgBpAG8AIABlAHMAIAB1AG4AIAByAGUAcQB1AGkAcwBpAHQAbwAgAGkAbgBkAGkAcwBwAGUAbgBzAGEAYgBsAGUAIABwAGEAcgBhACAAZABhAHIAIABpAG4AaQBjAGkAbwAgAGEAbAAgAHAAcgBvAGMAZQBzAG8AIABkAGUAIABjAGUAcgB0AGkAZgBpAGMAYQBjAGkA8wBuAC4AIABQAG8AcwB0AGUAcgBpAG8AcgBtAGUAbgB0AGUALDAdBgNVHQ4EFgQUgs1wzCGBMeMbhGGIC4owQuygtmswCwYDVR0PBAQDAgTwMCMGA1UdEQQcMBqgGAYIKwYBBAHBAQGgDBYKMDYzNDUzNjMtMzAfBgNVHSMEGDAWgBR44T6f0hKzejyNzTAOU7NDKQezVTA+BgNVHR8ENzA1MDOgMaAvhi1odHRwOi8vY3JsLmUtY2VydGNoaWxlLmNsL2VjZXJ0Y2hpbGVjYUZFUy5jcmwwOgYIKwYBBQUHAQEELjAsMCoGCCsGAQUFBzABhh5odHRwOi8vb2NzcC5lY2VydGNoaWxlLmNsL29jc3AwPQYJKwYBBAGCNxUHBDAwLgYmKwYBBAGCNxUIgtyDL4WTjGaF1Z0XguLcJ4Hv7DxhgcueFIaoglgCAWQCAQQwIwYDVR0SBBwwGqAYBggrBgEEAcEBAqAMFgo5NjkyODE4MC01MA0GCSqGSIb3DQEBBQUAA4IBAQABCek0Urq1hVzb8T5S3rnna1IAG4z1lJBTgEw28gKy1kfEiAQBOjPoYD0Vx00toIQJz8bdH7aigwnz+FaiYRKjT8pdnn6FALcLDCxdIa25kAp2Oc/s5bxqqTrbnNpxRryWo+FbiREIlvNyiw0KvMS6Dmy4/aLDhYxsVMib+bYWcc7FbMrZFlctlQjc8QgkrAxaECS7N9p4TwHf+Fz4/s6Z7Tdp8yhLNlZee2YM1s2HpEcZrYnNFtRaE7LQ/BNaI8aMClll/dVo6j3g524dUPClO/zy/kXrmKeB26/DPkSlnGBDKvYE0dTG6xW7+9O/HNCpdVf+nrzk+D/4t0E5pfb6</X509Certificate>\r\n";
            sSignature += "		</X509Data>\r\n";
            sSignature += "	</KeyInfo>\r\n";
            sSignature += "</Signature>\r\n";


            ////
            //// Agregar la firma del documento
            sDoc = Regex.Replace(
                sDoc,
                    "</ConsumoFolios>",
                        sSignature + "</ConsumoFolios>",
                            RegexOptions.Singleline);

            ////
            //// Agregar namespaces al documento
            sDoc = Regex.Replace(
                sDoc,
                    "<ConsumoFolios.*?>",
                        "<ConsumoFolios version=\"1.0\" xmlns=\"http://www.sii.cl/SiiDte\">",
                            RegexOptions.Singleline
                
                
                );

            ////
            //// regresar la simulacion
            return sDoc;



        }

        /// <summary>
        /// Simula un documento completo para la validación de schema
        /// </summary>
        internal static string SimulaDocumentoExpCompleto(string sDoc)
        {

            ////
            //// Cree el ted
            string sTED = "";
            sTED += "<TED version=\"1.0\">\r\n";
            sTED += "	<DD>\r\n";
            sTED += "		<RE>77218967-2</RE>\r\n";
            sTED += "		<TD>110</TD><F>1</F>\r\n";
            sTED += "		<FE>2020-09-17</FE>\r\n";
            sTED += "		<RR>60803000-K</RR>\r\n";
            sTED += "		<RSR>SERVICIO DE IMPUESTOS INTERNOS DIRECCION</RSR>\r\n";
            sTED += "		<MNT>620711</MNT>\r\n";
            sTED += "		<IT1>Cajón AFECTO</IT1>\r\n";
            sTED += "		<CAF version=\"1.0\">\r\n";
            sTED += "			<DA>\r\n";
            sTED += "				<RE>77218967-2</RE>\r\n";
            sTED += "				<RS>COMERCIAL Y SERVICIOS PRALUNGO LIMITADA</RS>\r\n";
            sTED += "				<TD>110</TD>\r\n";
            sTED += "				<RNG>\r\n";
            sTED += "					<D>1</D><H>50</H>\r\n";
            sTED += "				</RNG>\r\n";
            sTED += "				<FA>2020-09-17</FA>\r\n";
            sTED += "				<RSAPK>\r\n";
            sTED += "					<M>1/27bSOoaqaz2YmZsT8LtabFbavqY9YR/cdt9vK3Sfwk8Q1g4Ye+l7zxt6pW2IOWMxKH5mqloeoiN8D/yhVPVw==</M><E>Aw==</E>\r\n";
            sTED += "				</RSAPK>\r\n";
            sTED += "				<IDK>100</IDK>\r\n";
            sTED += "			</DA>\r\n";
            sTED += "			<FRMA algoritmo=\"SHA1withRSA\">T2yAfkW95vgYO3Q47mp5qL4rsJyZwo4VDjrx39ZOuAbgQnzLEYWwQFWMrVHr5YfQDdgmnZVzAxuu3tp3BggkDA==</FRMA>\r\n";
            sTED += "		</CAF>\r\n";
            sTED += "		<TSTED>2020-09-17T16:15:08</TSTED>\r\n";
            sTED += "	</DD>\r\n";
            sTED += "	<FRMT algoritmo=\"SHA1withRSA\">fJcft/0w/Mssi1LIk3FZXENh1GjVtwLazZrAGWLxp3bkoz6Op7OMVVRQXnRcgwLSOG3CH+YJa15x+8Xyw3Ajew==</FRMT>\r\n";
            sTED += "</TED>\r\n";


            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "<TmstFirma>",
                        sTED + "<TmstFirma>",
                            RegexOptions.Singleline
                );


            ////
            //// Crear la firma
            string sSignature = "";
            sSignature += "<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">\r\n";
            sSignature += "	<SignedInfo>\r\n";
            sSignature += "		<CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" />\r\n";
            sSignature += "		<SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" />\r\n";
            sSignature += "		<Reference URI=\"#R772189672F1T33\">\r\n";
            sSignature += "			<DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" />\r\n";
            sSignature += "			<DigestValue>9k5mV9klJBV8Z0s3+0Y2cuAKGog=</DigestValue>\r\n";
            sSignature += "		</Reference>\r\n";
            sSignature += "	</SignedInfo>\r\n";
            sSignature += "	<SignatureValue>Z2feYVmH+UbuARLhX1QOqoIrLFFd2ISh2yRJoGiKI1T19SP2J71w4o2+0KlFR13AgCLSSIk9U4rCwPqyDKQvuYGhhS5DyrADbME9rYB3EvtRLqzzhvxbWs8w9L9T9Lg/m8Ir653atRNspipPxJJXqRwkNTr9jpVjKjEKLb5wKxI=</SignatureValue>\r\n";
            sSignature += "	<KeyInfo>\r\n";
            sSignature += "		<KeyValue>\r\n";
            sSignature += "			<RSAKeyValue>\r\n";
            sSignature += "				<Modulus>1f6KXqTmCEMj7KCtv2fXop4rvL2Nc8tN2cPINRv646+aAcIKgrAM+Xc643NFy/gMUZPvkQubs3d2xQM5mGvCKapYDA9lHTLMoqDHLX1+g6FjbkJpCRXEF7Gl3zRnqmmRZ+CyQtJTMynR+BMsOTecNyfk+aLc88XEpPLb4tZ08ws=</Modulus>\r\n";
            sSignature += "				<Exponent>AQAB</Exponent>\r\n";
            sSignature += "			</RSAKeyValue>\r\n";
            sSignature += "		</KeyValue>\r\n";
            sSignature += "		<X509Data>\r\n";
            sSignature += "			<X509Certificate>MIIGWTCCBUGgAwIBAgIKEMHKKwAAABIm6DANBgkqhkiG9w0BAQUFADCB0jELMAkGA1UEBhMCQ0wxHTAbBgNVBAgTFFJlZ2lvbiBNZXRyb3BvbGl0YW5hMREwDwYDVQQHEwhTYW50aWFnbzEUMBIGA1UEChMLRS1DRVJUQ0hJTEUxIDAeBgNVBAsTF0F1dG9yaWRhZCBDZXJ0aWZpY2Fkb3JhMTAwLgYDVQQDEydFLUNFUlRDSElMRSBDQSBGSVJNQSBFTEVDVFJPTklDQSBTSU1QTEUxJzAlBgkqhkiG9w0BCQEWGHNjbGllbnRlc0BlLWNlcnRjaGlsZS5jbDAeFw0yMDA4MjcxOTUxMTRaFw0yMzA4MjcxOTUxMTRaMIHaMQswCQYDVQQGEwJDTDEiMCAGA1UECBMZTUVUUk9QT0xJVEFOQSBERSBTQU5USUFHTzERMA8GA1UEBxMIU0FOVElBR08xLjAsBgNVBAoTJUVEVUFSRE8gRVVHRU5JTyBCT1RUSU5FTExJIE1FUkNBTkRJTk8xCjAIBgNVBAsMASoxLjAsBgNVBAMTJUVEVUFSRE8gRVVHRU5JTyBCT1RUSU5FTExJIE1FUkNBTkRJTk8xKDAmBgkqhkiG9w0BCQEWGU1PTklDQUFSQVZFTkFASE9UTUFJTC5DT00wgZ8wDQYJKoZIhvcNAQEBBQADgY0AMIGJAoGBANX+il6k5ghDI+ygrb9n16KeK7y9jXPLTdnDyDUb+uOvmgHCCoKwDPl3OuNzRcv4DFGT75ELm7N3dsUDOZhrwimqWAwPZR0yzKKgxy19foOhY25CaQkVxBexpd80Z6ppkWfgskLSUzMp0fgTLDk3nDcn5Pmi3PPFxKTy2+LWdPMLAgMBAAGjggKpMIICpTCCAU8GA1UdIASCAUYwggFCMIIBPgYIKwYBBAHDUgUwggEwMC0GCCsGAQUFBwIBFiFodHRwOi8vd3d3LmUtY2VydGNoaWxlLmNsL0NQUy5odG0wgf4GCCsGAQUFBwICMIHxHoHuAEUAbAAgAHIAZQBzAHAAbwBuAGQAZQByACAAZQBzAHQAZQAgAGYAbwByAG0AdQBsAGEAcgBpAG8AIABlAHMAIAB1AG4AIAByAGUAcQB1AGkAcwBpAHQAbwAgAGkAbgBkAGkAcwBwAGUAbgBzAGEAYgBsAGUAIABwAGEAcgBhACAAZABhAHIAIABpAG4AaQBjAGkAbwAgAGEAbAAgAHAAcgBvAGMAZQBzAG8AIABkAGUAIABjAGUAcgB0AGkAZgBpAGMAYQBjAGkA8wBuAC4AIABQAG8AcwB0AGUAcgBpAG8AcgBtAGUAbgB0AGUALDAdBgNVHQ4EFgQUgs1wzCGBMeMbhGGIC4owQuygtmswCwYDVR0PBAQDAgTwMCMGA1UdEQQcMBqgGAYIKwYBBAHBAQGgDBYKMDYzNDUzNjMtMzAfBgNVHSMEGDAWgBR44T6f0hKzejyNzTAOU7NDKQezVTA+BgNVHR8ENzA1MDOgMaAvhi1odHRwOi8vY3JsLmUtY2VydGNoaWxlLmNsL2VjZXJ0Y2hpbGVjYUZFUy5jcmwwOgYIKwYBBQUHAQEELjAsMCoGCCsGAQUFBzABhh5odHRwOi8vb2NzcC5lY2VydGNoaWxlLmNsL29jc3AwPQYJKwYBBAGCNxUHBDAwLgYmKwYBBAGCNxUIgtyDL4WTjGaF1Z0XguLcJ4Hv7DxhgcueFIaoglgCAWQCAQQwIwYDVR0SBBwwGqAYBggrBgEEAcEBAqAMFgo5NjkyODE4MC01MA0GCSqGSIb3DQEBBQUAA4IBAQABCek0Urq1hVzb8T5S3rnna1IAG4z1lJBTgEw28gKy1kfEiAQBOjPoYD0Vx00toIQJz8bdH7aigwnz+FaiYRKjT8pdnn6FALcLDCxdIa25kAp2Oc/s5bxqqTrbnNpxRryWo+FbiREIlvNyiw0KvMS6Dmy4/aLDhYxsVMib+bYWcc7FbMrZFlctlQjc8QgkrAxaECS7N9p4TwHf+Fz4/s6Z7Tdp8yhLNlZee2YM1s2HpEcZrYnNFtRaE7LQ/BNaI8aMClll/dVo6j3g524dUPClO/zy/kXrmKeB26/DPkSlnGBDKvYE0dTG6xW7+9O/HNCpdVf+nrzk+D/4t0E5pfb6</X509Certificate>\r\n";
            sSignature += "		</X509Data>\r\n";
            sSignature += "	</KeyInfo>\r\n";
            sSignature += "</Signature>\r\n";

            ////
            //// Agregar el elemento al documento
            sDoc = Regex.Replace(
                sDoc,
                    "</Exportaciones>",
                       "</Exportaciones>" + sSignature,
                            RegexOptions.Singleline
                );

            ////
            //// Regerse el valor de retorno
            return sDoc;


        }

    }

}
