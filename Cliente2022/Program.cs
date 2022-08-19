using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HEFSIILIBDTES;
using HEFSIILIBDTES.LIBRERIA.BOL;
using HEFSIILIBDTES.LIBRERIA.RCOF;
using HEFSIILIBDTES.CONSULTAS;

namespace cliente
{
    class Program
    {
        static void Main(string[] args)
        {

            //// *************************************************************************
            //// Proceso de generación de boletas electrónicas
            //// Actualización 05-2021
            //// Actualización 01-2022
            //// *************************************************************************

            //// CrearBOL();
            //// CrearRCOF();
            //// CrearRCOFSinMovimeintos();
            //// ConsultarTrackidBol();
            //// ConsultaEstadoBoletaElectronica();
            //// GenerarPdf();
            //// GenerarPdf417();
            //// PublicarPaqueteBoletas();
            ///

            ////
            //// TEST SET BOLETAS ELECTRPONICAS
            //// CrearBOLXml();
            //PublicarPaqueteBoletasCertificadoSetBoletas();
            //ConsultarTrackidBol();

            PublicarPaqueteBoletas();

            Console.WriteLine("FIN");
            Console.ReadKey();

        }

        /// <summary>
        /// Inicia generación de BOLETA y publica documento en el SII
        /// </summary>
        static void CrearBOL()
        {

            ////
            //// Instanciar la clase
            HEFBOLETA Bol = new HEFBOLETA();

            ////
            //// Configure el objeto
            Bol.Configuracion.FullPathSchema = "herramientas\\schemas\\BOL\\EnvioBOLETA_v11.xsd";
            Bol.Configuracion.FullPathCaf = "herramientas\\FoliosSII772189673312020917169B.xml";
            Bol.Configuracion.CnCertificado = "Marcelo Ivan Rojas Rojas";
            Bol.Configuracion.RutEnviador = "12959262-1";
            Bol.Configuracion.NroResolucion = "0";
            Bol.Configuracion.FchResolucion = "2020-01-01";

            ////
            //// Identificacion de la boleta
            Bol.Documento.Encabezado.IdDoc.TipoDTE = 39;
            Bol.Documento.Encabezado.IdDoc.Folio = 1;
            Bol.Documento.Encabezado.IdDoc.FchEmis = "2015-03-22";
            Bol.Documento.Encabezado.IdDoc.IndServicio = 3;

            ////
            //// Datos del emisor
            Bol.Documento.Encabezado.Emisor.RUTEmisor = "77218967-2";
            Bol.Documento.Encabezado.Emisor.RznSocEmisor = "INGENIERIA Y SISTEMAS DAPCOM LIMITADA";
            Bol.Documento.Encabezado.Emisor.GiroEmisor = "EMPRESAS DE SERVICIO INTEGRALES DE INFORMATICA";
            Bol.Documento.Encabezado.Emisor.DirOrigen = "LA PLAYA 1064";
            Bol.Documento.Encabezado.Emisor.CmnaOrigen = "SAN BERNARDO";
            Bol.Documento.Encabezado.Emisor.CiudadOrigen = "SANTIAGO";

            ////
            //// Datos del Receptor
            Bol.Documento.Encabezado.Receptor.RUTRecep = "66666666-6";
            Bol.Documento.Encabezado.Receptor.RznSocRecep = "YXB S.A.";
            Bol.Documento.Encabezado.Receptor.DirRecep = "BURGOS 80 PISO 2";
            Bol.Documento.Encabezado.Receptor.CmnaRecep = "LAS CONDES";
            Bol.Documento.Encabezado.Receptor.CiudadRecep = "SANTIAGO";

            ////
            //// Cree los totales
            Bol.Documento.Encabezado.Totales.MntNeto = 1000;
            Bol.Documento.Encabezado.Totales.MntExe = 2000;
            Bol.Documento.Encabezado.Totales.IVA = 900;
            Bol.Documento.Encabezado.Totales.MntTotal = 9900;

            ////
            //// Cree el detalle 1 del documento
            //// Detalle #1 ( ITEM AFECTO )
            Bol.Documento.Detalle = new List<HEF_Detalle>();

            HEF_Detalle Detalle = new HEF_Detalle();
            Detalle.NroLinDet = 1;
            Detalle.NmbItem = "ACEITE DE PESCADO B&W";
            Detalle.QtyItem = 100;
            Detalle.PrcItem = 1000;
            Detalle.MontoItem = 100000;
            Bol.Documento.Detalle.Add(Detalle);

            ////
            //// Otro detalle
            HEF_Detalle Detalle2 = new HEF_Detalle();
            Detalle2.NroLinDet = 1;
            Detalle2.NmbItem = "Detalle 2";
            Detalle2.QtyItem = 25000000;
            Detalle2.PrcItem = 0.01225m;
            Detalle2.MontoItem = 306250;
            Bol.Documento.Detalle.Add(Detalle2);

            ////
            //// Para crear la referencia de un documento
            HEF_Referencia Referencia = new HEF_Referencia();
            Referencia.NroLinRef = 1;
            Referencia.CodRef = "SET";
            Referencia.RazonRef = "CASO 290030-1";
            Bol.Documento.Referencia.Add(Referencia);

            ////
            //// Proceso de publicación del documento DTE
            //// 

            ////
            //// Iniciar la publicación del documento
            HefRespuesta resp = Bol.Publicar();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("TEST_BOLETA.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));


            ////
            //// RECUPERAR EL TRACKID DE LA OPERACION
            //// RECUPERAR EL DOCUMENTO XML DE LA OPERACIÓN





        }

        /// <summary>
        /// Permite publicar en disco el documento xml que representa la boleta actual
        /// </summary>
        static void CrearBOLXml()
        {

            #region SET DE PRUEBAS BOLETAS ELECTRONICAS 

            #region CASO 1

            ////
            //// Instanciar la clase
            HEFBOLETA Bol = new HEFBOLETA();

            ////
            //// Configure el objeto
            Bol.Configuracion.FullPathSchema = "herramientas\\schemas\\BOL\\EnvioBOLETA_v11.xsd";
            Bol.Configuracion.FullPathCaf = @"D:\MIS DOCUMENTOS\TODAS MIS CERTIFICACIONES\DANIEL BINFA\77576339-6 cURRENT\cetificacinexenta\BOLETAS\FoliosSII7757633941120228111935.xml";
            Bol.Configuracion.CnCertificado = "MARCELO IVAN ROJAS ROJAS";
            Bol.Configuracion.RutEnviador = "12959262-1";
            Bol.Configuracion.NroResolucion = "0";
            Bol.Configuracion.FchResolucion = "2022-07-28";

            ////
            //// Identificacion de la boleta
            Bol.Documento.Encabezado.IdDoc.TipoDTE = 41;
            Bol.Documento.Encabezado.IdDoc.Folio = 13;
            Bol.Documento.Encabezado.IdDoc.FchEmis = "2022-08-14";
            Bol.Documento.Encabezado.IdDoc.IndServicio = 3;

            ////
            //// Datos del emisor
            Bol.Documento.Encabezado.Emisor.RUTEmisor = "77576339-6";
            Bol.Documento.Encabezado.Emisor.RznSocEmisor = "SMART LEARNING CHILE CAPACITACIONES SPA";
            Bol.Documento.Encabezado.Emisor.GiroEmisor = "SERVICIOS PROFESIONALES Y ASESORIA GERENCIAL";
            Bol.Documento.Encabezado.Emisor.DirOrigen = "GENERAL BUSTAMANTE 34 DP";
            Bol.Documento.Encabezado.Emisor.CmnaOrigen = "PROVIDENCIA";
            Bol.Documento.Encabezado.Emisor.CiudadOrigen = "SANTIAGO";

            ////
            //// Datos del Receptor
            Bol.Documento.Encabezado.Receptor.RUTRecep = "66666666-6";
            Bol.Documento.Encabezado.Receptor.RznSocRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.DirRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.CmnaRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.CiudadRecep = "No hay información";

            ////
            //// Cree los totales
            //Bol.Documento.Encabezado.Totales.MntNeto = 1000;
            //Bol.Documento.Encabezado.Totales.IVA = 900;
            Bol.Documento.Encabezado.Totales.MntExe = 120000;
            Bol.Documento.Encabezado.Totales.MntTotal = 120000;

            ////
            //// Cree el detalle 1 del documento
            //// Detalle #1 ( ITEM AFECTO )
            Bol.Documento.Detalle = new List<HEF_Detalle>();

            ////
            //// Detalle 1
            HEF_Detalle Detalle = new HEF_Detalle();
            Detalle.NroLinDet = 1;
            Detalle.IndExe = 1;
            Detalle.NmbItem = "Consultoria 1";
            Detalle.QtyItem = 1;
            Detalle.UnmdItem = "Un";
            Detalle.PrcItem = 50000;
            Detalle.MontoItem = 50000;
            Bol.Documento.Detalle.Add(Detalle);

            ////
            //// Otro detalle
            HEF_Detalle Detalle2 = new HEF_Detalle();
            Detalle2.NroLinDet = 2;
            Detalle2.IndExe = 1;
            Detalle2.NmbItem = "Consultoria 2";
            Detalle2.QtyItem = 2;
            Detalle2.UnmdItem = "Un";
            Detalle2.PrcItem = 35000;
            Detalle2.MontoItem = 70000;
            Bol.Documento.Detalle.Add(Detalle2);

            ////
            //// Para crear la referencia de un documento
            HEF_Referencia Referencia = new HEF_Referencia();
            Referencia.NroLinRef = 1;
            Referencia.CodRef = "SET";
            Referencia.RazonRef = "CASO-1";
            Bol.Documento.Referencia.Add(Referencia);

            ////
            //// Proceso de publicación del documento DTE
            //// 

            ////
            //// Iniciar la publicación del documento
            HefRespuesta resp = Bol.PublicarXml();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("CASO_BOLETA_1.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));


            ////
            //// RECUPERAR EL TRACKID DE LA OPERACION
            //// RECUPERAR EL DOCUMENTO XML DE LA OPERACIÓN

            #endregion

            #region CASO 2

            ////
            //// Instanciar la clase
            Bol = new HEFBOLETA();

            ////
            //// Configure el objeto
            Bol.Configuracion.FullPathSchema = "herramientas\\schemas\\BOL\\EnvioBOLETA_v11.xsd";
            Bol.Configuracion.FullPathCaf = @"D:\MIS DOCUMENTOS\TODAS MIS CERTIFICACIONES\DANIEL BINFA\77576339-6 cURRENT\cetificacinexenta\BOLETAS\FoliosSII7757633941120228111935.xml";
            Bol.Configuracion.CnCertificado = "MARCELO IVAN ROJAS ROJAS";
            Bol.Configuracion.RutEnviador = "12959262-1";
            Bol.Configuracion.NroResolucion = "0";
            Bol.Configuracion.FchResolucion = "2022-07-28";

            ////
            //// Identificacion de la boleta
            Bol.Documento.Encabezado.IdDoc.TipoDTE = 41;
            Bol.Documento.Encabezado.IdDoc.Folio = 14;
            Bol.Documento.Encabezado.IdDoc.FchEmis = "2022-08-14";
            Bol.Documento.Encabezado.IdDoc.IndServicio = 3;

            ////
            //// Datos del emisor
            Bol.Documento.Encabezado.Emisor.RUTEmisor = "77576339-6";
            Bol.Documento.Encabezado.Emisor.RznSocEmisor = "SMART LEARNING CHILE CAPACITACIONES SPA";
            Bol.Documento.Encabezado.Emisor.GiroEmisor = "SERVICIOS PROFESIONALES Y ASESORIA GERENCIAL";
            Bol.Documento.Encabezado.Emisor.DirOrigen = "GENERAL BUSTAMANTE 34 DP";
            Bol.Documento.Encabezado.Emisor.CmnaOrigen = "PROVIDENCIA";
            Bol.Documento.Encabezado.Emisor.CiudadOrigen = "SANTIAGO";

            ////
            //// Datos del Receptor
            Bol.Documento.Encabezado.Receptor.RUTRecep = "66666666-6";
            Bol.Documento.Encabezado.Receptor.RznSocRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.DirRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.CmnaRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.CiudadRecep = "No hay información";

            ////
            //// Cree los totales
            //Bol.Documento.Encabezado.Totales.MntNeto = 1000;
            //Bol.Documento.Encabezado.Totales.MntExe = 2000;
            //Bol.Documento.Encabezado.Totales.IVA = 900;
            Bol.Documento.Encabezado.Totales.MntExe = 95000;
            Bol.Documento.Encabezado.Totales.MntTotal = 95000;

            ////
            //// Cree el detalle 1 del documento
            //// Detalle #1 ( ITEM AFECTO )
            Bol.Documento.Detalle = new List<HEF_Detalle>();

            ////
            //// Detalle 1
            Detalle = new HEF_Detalle();
            Detalle.NroLinDet = 1;
            Detalle.IndExe = 1;
            Detalle.NmbItem = "Capacitacion";
            Detalle.QtyItem = 1;
            Detalle.UnmdItem = "Hrs";
            Detalle.PrcItem = 95000;
            Detalle.MontoItem = 95000;
            Bol.Documento.Detalle.Add(Detalle);

            ////
            //// Para crear la referencia de un documento
            Referencia = new HEF_Referencia();
            Referencia.NroLinRef = 1;
            Referencia.CodRef = "SET";
            Referencia.RazonRef = "CASO-2";
            Bol.Documento.Referencia.Add(Referencia);

            ////
            //// Proceso de publicación del documento DTE
            //// 

            ////
            //// Iniciar la publicación del documento
            resp = Bol.PublicarXml();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("CASO_BOLETA_2.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));

            #endregion

            #region CASO 3

            ////
            //// Instanciar la clase
            Bol = new HEFBOLETA();

            ////
            //// Configure el objeto
            Bol.Configuracion.FullPathSchema = "herramientas\\schemas\\BOL\\EnvioBOLETA_v11.xsd";
            Bol.Configuracion.FullPathCaf = @"D:\MIS DOCUMENTOS\TODAS MIS CERTIFICACIONES\DANIEL BINFA\77576339-6 cURRENT\cetificacinexenta\BOLETAS\FoliosSII7757633941120228111935.xml";
            Bol.Configuracion.CnCertificado = "MARCELO IVAN ROJAS ROJAS";
            Bol.Configuracion.RutEnviador = "12959262-1";
            Bol.Configuracion.NroResolucion = "0";
            Bol.Configuracion.FchResolucion = "2022-07-28";

            ////
            //// Identificacion de la boleta
            Bol.Documento.Encabezado.IdDoc.TipoDTE = 41;
            Bol.Documento.Encabezado.IdDoc.Folio = 15;
            Bol.Documento.Encabezado.IdDoc.FchEmis = "2022-08-14";
            Bol.Documento.Encabezado.IdDoc.IndServicio = 3;

            ////
            //// Datos del emisor
            Bol.Documento.Encabezado.Emisor.RUTEmisor = "77576339-6";
            Bol.Documento.Encabezado.Emisor.RznSocEmisor = "SMART LEARNING CHILE CAPACITACIONES SPA";
            Bol.Documento.Encabezado.Emisor.GiroEmisor = "SERVICIOS PROFESIONALES Y ASESORIA GERENCIAL";
            Bol.Documento.Encabezado.Emisor.DirOrigen = "GENERAL BUSTAMANTE 34 DP";
            Bol.Documento.Encabezado.Emisor.CmnaOrigen = "PROVIDENCIA";
            Bol.Documento.Encabezado.Emisor.CiudadOrigen = "SANTIAGO";

            ////
            //// Datos del Receptor
            Bol.Documento.Encabezado.Receptor.RUTRecep = "66666666-6";
            Bol.Documento.Encabezado.Receptor.RznSocRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.DirRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.CmnaRecep = "No hay información";
            Bol.Documento.Encabezado.Receptor.CiudadRecep = "No hay información";

            ////
            //// Cree los totales
            //Bol.Documento.Encabezado.Totales.MntNeto = 1000;
            //Bol.Documento.Encabezado.Totales.MntExe = 2000;
            //Bol.Documento.Encabezado.Totales.IVA = 900;
            Bol.Documento.Encabezado.Totales.MntExe = 77500;
            Bol.Documento.Encabezado.Totales.MntTotal = 77500;

            ////
            //// Cree el detalle 1 del documento
            //// Detalle #1 ( ITEM AFECTO )
            Bol.Documento.Detalle = new List<HEF_Detalle>();

            ////
            //// Detalle 1
            Detalle = new HEF_Detalle();
            Detalle.NroLinDet = 1;
            Detalle.IndExe = 1;
            Detalle.NmbItem = "Consulta Medica";
            Detalle.QtyItem = 1;
            Detalle.UnmdItem = "Un";
            Detalle.PrcItem = 25000;
            Detalle.MontoItem = 25000;
            Bol.Documento.Detalle.Add(Detalle);

            ////
            //// Otro detalle
            Detalle2 = new HEF_Detalle();
            Detalle2.NroLinDet = 2;
            Detalle2.IndExe = 1;
            Detalle2.NmbItem = "Atencion Hospitalaria";
            Detalle2.QtyItem = 3;
            Detalle2.UnmdItem = "Un";
            Detalle2.PrcItem = 17500;
            Detalle2.MontoItem = 52500;
            Bol.Documento.Detalle.Add(Detalle2);

            ////
            //// Para crear la referencia de un documento
            Referencia = new HEF_Referencia();
            Referencia.NroLinRef = 1;
            Referencia.CodRef = "SET";
            Referencia.RazonRef = "CASO-3";
            Bol.Documento.Referencia.Add(Referencia);

            ////
            //// Proceso de publicación del documento DTE
            //// 

            ////
            //// Iniciar la publicación del documento
            resp = Bol.PublicarXml();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("CASO_BOLETA_3.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));

            #endregion


            #endregion

        }


        /// <summary>
        /// Inicia generación de BOLETA y publica en el sii el documento.
        /// </summary>
        /// <remarks>
        /// Utiliza certificado digital fisico
        /// </remarks>
        static void CrearBOL_Certificado()
        {

            ////
            //// Instanciar la clase
            HEFBOLETA Bol = new HEFBOLETA();

            ////
            //// Configure el objeto
            Bol.Configuracion.FullPathSchema = "herramientas\\schemas\\BOL\\EnvioBOLETA_v11.xsd";
            Bol.Configuracion.FullPathCaf = "herramientas\\FoliosSII772189673312020917169B.xml";
            Bol.Configuracion.PathCertificado = "herramientas\\Certificado.pfx";
            Bol.Configuracion.PassCertificado = "123456";
            Bol.Configuracion.RutEnviador = "12959262-1";
            Bol.Configuracion.NroResolucion = "0";
            Bol.Configuracion.FchResolucion = "2020-01-01";

            ////
            //// Identificacion de la boleta
            Bol.Documento.Encabezado.IdDoc.TipoDTE = 39;
            Bol.Documento.Encabezado.IdDoc.Folio = 1;
            Bol.Documento.Encabezado.IdDoc.FchEmis = "2015-03-22";
            Bol.Documento.Encabezado.IdDoc.IndServicio = 3;

            ////
            //// Datos del emisor
            Bol.Documento.Encabezado.Emisor.RUTEmisor = "77218967-2";
            Bol.Documento.Encabezado.Emisor.RznSocEmisor = "INGENIERIA Y SISTEMAS DAPCOM LIMITADA";
            Bol.Documento.Encabezado.Emisor.GiroEmisor = "EMPRESAS DE SERVICIO INTEGRALES DE INFORMATICA";
            Bol.Documento.Encabezado.Emisor.DirOrigen = "LA PLAYA 1064";
            Bol.Documento.Encabezado.Emisor.CmnaOrigen = "SAN BERNARDO";
            Bol.Documento.Encabezado.Emisor.CiudadOrigen = "SANTIAGO";

            ////
            //// Datos del Receptor
            Bol.Documento.Encabezado.Receptor.RUTRecep = "3-5";
            Bol.Documento.Encabezado.Receptor.RznSocRecep = "YXB S.A.";
            Bol.Documento.Encabezado.Receptor.DirRecep = "BURGOS 80 PISO 2";
            Bol.Documento.Encabezado.Receptor.CmnaRecep = "LAS CONDES";
            Bol.Documento.Encabezado.Receptor.CiudadRecep = "SANTIAGO";

            ////
            //// Cree los totales
            Bol.Documento.Encabezado.Totales.MntNeto = 1000;
            Bol.Documento.Encabezado.Totales.MntExe = 2000;
            Bol.Documento.Encabezado.Totales.IVA = 900;
            Bol.Documento.Encabezado.Totales.MntTotal = 9900;

            ////
            //// Cree la lista de detalles del documento boleta
            Bol.Documento.Detalle = new List<HEF_Detalle>();

            ////
            //// Cree el detalle 1 del documento
            //// Detalle #1 ( ITEM AFECTO )
            HEF_Detalle Detalle_Afecto = new HEF_Detalle();
            Detalle_Afecto.NroLinDet = 1;
            Detalle_Afecto.NmbItem = "ACEITE DE PESCADO B&W";
            Detalle_Afecto.QtyItem = 100;
            Detalle_Afecto.PrcItem = 1000;
            Detalle_Afecto.MontoItem = 100000;
            Bol.Documento.Detalle.Add(Detalle_Afecto);

            ////
            //// Cree el detalle 1 del documento
            //// Detalle #1 ( ITEM EXENTO )
            HEF_Detalle Detalle_Exento = new HEF_Detalle();
            Detalle_Exento.NroLinDet = 1;
            Detalle_Exento.IndExe = 1;
            Detalle_Exento.NmbItem = "ACEITE DE PESCADO B&W";
            Detalle_Exento.QtyItem = 100;
            Detalle_Exento.PrcItem = 1000;
            Detalle_Exento.MontoItem = 100000;
            Bol.Documento.Detalle.Add(Detalle_Exento);

            ////
            //// Para crear la referencia de un documento
            HEF_Referencia Referencia = new HEF_Referencia();
            Referencia.NroLinRef = 1;
            Referencia.CodRef = "SET";
            Referencia.RazonRef = "CASO 290030-1";
            Bol.Documento.Referencia.Add(Referencia);

            ////
            //// Proceso de publicación del documento DTE
            //// 

            ////
            //// Iniciar la publicación del documento
            HefRespuesta resp = Bol.Publicar();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("TEST_BOLETA.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));

        }

        /// <summary>
        /// Inicia la generación de un archivo RCOF
        /// </summary>
        static void CrearRCOF()
        {
            ////
            //// Instanciar la clase
            HEFRCOF RCOF = new HEFRCOF();

            ////
            //// Configurar la clase para su procesamiento
            RCOF.Configuracion.FullPathSchema = "herramientas\\schemas\\RCOF\\RCOF.xsd";
            RCOF.Configuracion.CnCertificado = "Marcelo Ivan Rojas Rojas";
            RCOF.Configuracion.RutEnviador = "12959262-1";

            ////
            //// Cree la caratula del documento
            //// RCOF.DocumentoConsumoFolios.ID = "gygygygyg";
            RCOF.DocumentoConsumoFolios.Caratula.RutEmisor = "77218967-2";
            RCOF.DocumentoConsumoFolios.Caratula.RutEnvia = "6345363-3";
            RCOF.DocumentoConsumoFolios.Caratula.FchResol = "2020-09-17";
            RCOF.DocumentoConsumoFolios.Caratula.NroResol = "0";
            RCOF.DocumentoConsumoFolios.Caratula.FchInicio = "2020-09-17";
            RCOF.DocumentoConsumoFolios.Caratula.FchFinal = "2020-09-17";
            RCOF.DocumentoConsumoFolios.Caratula.Correlativo = "1";
            RCOF.DocumentoConsumoFolios.Caratula.SecEnvio = "1";
            RCOF.DocumentoConsumoFolios.Caratula.TmstFirmaEnv = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            ////
            //// Agregar un resumen 
            RCOF.DocumentoConsumoFolios.Resumenes = new List<HefResumen>();

            ////
            //// Agregar nuevo resumen
            HefResumen resumen = new HefResumen();
            resumen.TipoDocumento = 39;
            resumen.MntNeto = 1000;
            resumen.MntExento = 0;
            resumen.MntIva = 190;
            resumen.TasaIVA = "19";
            resumen.MntTotal = 1190;
            resumen.FoliosEmitidos = 5;
            resumen.FoliosAnulados = 0;
            resumen.FoliosUtilizados = 5;

            ////
            //// Agregar rango utilizados
            resumen.RangoUtilizados = new List<HefRangoUtilizados>();
            HefRangoUtilizados ru = new HefRangoUtilizados();
            ru.Inicial = 1;
            ru.Final = 5;
            resumen.RangoUtilizados.Add(ru);

            ////
            //// Agregar resumen 
            RCOF.DocumentoConsumoFolios.Resumenes.Add(resumen);


            ////
            //// Agregar nuevo resumen
            resumen = new HefResumen();
            resumen.TipoDocumento = 41;
            resumen.MntNeto = 1000;
            resumen.MntExento = 0;
            resumen.MntIva = 190;
            resumen.TasaIVA = "19";
            resumen.MntTotal = 1190;
            resumen.FoliosEmitidos = 5;
            resumen.FoliosAnulados = 0;
            resumen.FoliosUtilizados = 5;

            ////
            //// Agregar rango utilizados
            ru = new HefRangoUtilizados();
            ru.Inicial = 1;
            ru.Final = 5;
            resumen.RangoUtilizados.Add(ru);


            ////
            //// Agregar resumen 
            RCOF.DocumentoConsumoFolios.Resumenes.Add(resumen);



            ////
            //// Iniciar la publicación del documento RCOF
            HefRespuesta resp = RCOF.Publicar();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("TEST_RCOF.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));

        }

        /// <summary>
        /// Inicia la generación de un archivo RCOF
        /// </summary>
        static void CrearRCOFSinMovimeintos()
        {
            ////
            //// Instanciar la clase
            HEFRCOF RCOF = new HEFRCOF();

            ////
            //// Configurar la clase para su procesamiento
            RCOF.Configuracion.FullPathSchema = "herramientas\\schemas\\RCOF\\RCOF.xsd";
            RCOF.Configuracion.CnCertificado = "Marcelo Ivan Rojas Rojas";
            RCOF.Configuracion.RutEnviador = "12959262-1";

            ////
            //// Cree la caratula del documento
            //// RCOF.DocumentoConsumoFolios.ID = "gygygygyg";
            RCOF.DocumentoConsumoFolios.Caratula.RutEmisor = "77218967-2";
            RCOF.DocumentoConsumoFolios.Caratula.RutEnvia = "6345363-3";
            RCOF.DocumentoConsumoFolios.Caratula.FchResol = "2020-09-17";
            RCOF.DocumentoConsumoFolios.Caratula.NroResol = "0";
            RCOF.DocumentoConsumoFolios.Caratula.FchInicio = "2020-09-17";
            RCOF.DocumentoConsumoFolios.Caratula.FchFinal = "2020-09-17";
            RCOF.DocumentoConsumoFolios.Caratula.Correlativo = "1";
            RCOF.DocumentoConsumoFolios.Caratula.SecEnvio = "1";
            RCOF.DocumentoConsumoFolios.Caratula.TmstFirmaEnv = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            ////
            //// Agregar un resumen 
            RCOF.DocumentoConsumoFolios.Resumenes = new List<HefResumen>();

            ////
            //// Agregar nuevo resumen
            HefResumen resumen = new HefResumen();
            resumen.TipoDocumento = 39;
            resumen.MntNeto = 0;
            resumen.MntExento = 0;
            resumen.MntIva = 0;
            resumen.TasaIVA = "19";
            resumen.MntTotal = 0;
            resumen.FoliosEmitidos = 0;
            resumen.FoliosAnulados = 0;
            resumen.FoliosUtilizados = 0;

            ////
            //// Agregar resumen 
            RCOF.DocumentoConsumoFolios.Resumenes.Add(resumen);

            ////
            //// Iniciar la publicación del documento RCOF
            HefRespuesta resp = RCOF.Publicar();

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);

            ////
            //// Si el proceso fue correcto guarde el documento Dte
            if (resp.EsCorrecto)
                File.WriteAllText("TEST_RCOF.xml", resp.XmlDocumento, Encoding.GetEncoding("ISO-8859-1"));

        }

        /// <summary>
        /// Inicia la consulta de trackid en el SII
        /// </summary>
        static void ConsultarTrackidBol()
        {
            ////
            //// Consultar por el trackid de un documento DTE o RCOF
            HefRespuesta resp = HefConsultas.EstadoBolTrackid(
                "79514800-0",
                    "18488133",
                        "JUAN PABLO SIERRALTA OREZZOLI",
                            AmbienteSII.Certificacion);

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);




            //////
            ////// Consultar por el trackid de un documento DTE o RCOF
            //Console.WriteLine("========================================================");
            //Console.WriteLine();
            //resp = HefConsultas.EstadoBolTrackid(
            //    "76102778-6",
            //        "67604",
            //            "Marcelo Ivan Rojas Rojas",
            //                AmbienteSII.Certificacion);

            //////
            ////// Notificar
            //Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            //Console.WriteLine("Proceso   : " + resp.Proceso);
            //Console.WriteLine("Mensaje   : " + resp.Mensaje);
            //Console.WriteLine("Detalle   : " + resp.Detalle);
            //Console.WriteLine("Trackid   : " + resp.Trackid);
            //Console.WriteLine("========================================================");
            //Console.WriteLine(resp.XmlDocumento);

        }

        /// <summary>
        /// Inicia la consulta de trackid en el SII
        /// </summary>
        static void ConsultaEstadoBoletaElectronica()
        {

            ////
            //// Parametros
            string rutEmpresa = "76102778-6";
            string tipo = "39";
            string folio = "5234";
            string rutReceptor = "66666666-6";
            string monto = "100";
            string fchEmision = "2020-08-22";
            string Cn = "Marcelo Ivan Rojas Rojas";

            ////
            //// Consultar por el trackid de un documento DTE o RCOF
            HefRespuesta resp = HefConsultas.EstadoBoleta(
               rutEmpresa,
                    tipo,
                        folio,
                            rutReceptor,
                                monto,
                                    fchEmision,
                                        Cn,
                                            AmbienteSII.Produccion);

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("Trackid   : " + resp.Trackid);
            Console.WriteLine("========================================================");
            Console.WriteLine(resp.XmlDocumento);


        }

        /// <summary>
        /// Inicia la generación de un documento DTE en formato PDF
        /// </summary>
        static void GenerarPdf()
        {

            ////
            //// Cual es el archivo a publicar en formato PDF.
            //// Cual es el archivo xslt para transformar el documento.
            //string xmlDTE = "TEST_BOLETA.xml";
            //string xmlXsl = "herramientas\\XSLT\\Default_dte.xsl";

            string xmlDTE = "Test dagoberto\\TEST_BOLETA.xml";
            string xmlXsl = "Test dagoberto\\Default_boleta.xsl";

            ////
            //// Inicie la generación y recuperación del archivo
            HefRespuesta resp = HEFPDF.GenerarDocumentoPdf(xmlDTE, xmlXsl);

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("========================================================");

            ////
            //// Iniciar la publicación del documento
            byte[] buffer = (byte[])resp.Resultado;
            File.WriteAllBytes("Resultados\\DocumentoDTE.PDF", buffer);

        }

        /// <summary>
        /// Inicia la generación de un documento DTE en formato PDF
        /// </summary>
        static void GenerarPdf417()
        {

            ////
            //// Cual es el archivo a publicar en formato PDF.
            string xmlDTE = "TEST_DTE.xml";


            ////
            //// Inicie la generación y recuperación del archivo
            HefRespuesta resp = HEFPDF.GenerarDocumentoPdf417(xmlDTE);

            ////
            //// Notificar
            Console.WriteLine("EsCorrecto: " + resp.EsCorrecto);
            Console.WriteLine("Proceso   : " + resp.Proceso);
            Console.WriteLine("Mensaje   : " + resp.Mensaje);
            Console.WriteLine("Detalle   : " + resp.Detalle);
            Console.WriteLine("========================================================");

            ////
            //// Iniciar la publicación del documento
            byte[] buffer = (byte[])resp.Resultado;
            File.WriteAllBytes("Resultados\\Timbre.png", buffer);

        }

        /// <summary>
        /// Inicia la publicación de varios docuemntos Boletas
        /// </summary>
        static void PublicarPaqueteBoletas()
        {

            ////
            //// Parametros de la consulta
            string xsd_path = "herramientas\\SCHEMAS\\BOL\\EnvioBOLETA_v11.xsd"; //// Ruta a schema de validacion de boletas
            string cert_path = "herramientas\\Certificado.pfx";                  //// ruta del certificado digital pfx
            string cert_pass = "0984";//"Garzas7744";                                     //// clave del certificado
            string rut_empresa = "79514800-0";                                   //// rut de la empresa
            string rut_enviador = "12628844-1";                                  //// rut de quien envia los documentos boletas
            string fch_resol = "2012-06-19";                                     //// fecha de resolucion de la empresa
            int nro_resol = 0;                                                   //// número de resolucion 0 = certificación, mayor que cero = producción

            ////
            //// Cree la lista de documentos boletas a enviar al SII
            //// Para pasar los documentos xml en formto string timbrados y firmados
            //// utilice una lista string para agregar las boletas en formato string.
            //// Ejemplo: Se lee una boleta electrónica desde un archivo fisico y luego se carga 
            //// el contenido xml en foramto string en la lista 5 veces. Esta boleta 
            //// debe estar firmada y timbrada.
            List<string> _boletas = new List<string>();
            string path_boleta = @"Boleta\\SET_ENVIO_BOLETA2.xml";
            string content = File.ReadAllText(path_boleta, Encoding.GetEncoding("ISO-8859-1"));
            _boletas.Add(content);


            ////
            //// Inicie el procesamiento de las boletas. El resultado de la operación regresará un objeto HefRespuesta
            //// con la información del procesamiento.
            HefRespuesta resp = HefPublicador.PublicadoBoletasPorLotes(
                rut_empresa,
                    rut_enviador,
                        fch_resol,
                            nro_resol,
                                cert_path,
                                    cert_pass,
                                        _boletas,
                                            xsd_path);


            ////
            //// Imprima el resultado de la operación
            Console.WriteLine("EsCorrecto : {0}", resp.EsCorrecto);
            Console.WriteLine("Mensaje : {0}", resp.Mensaje);
            Console.WriteLine("Detalle : {0}", resp.Detalle);

        }

        /// <summary>
        /// Inicia la publicación de varios docuemntos Boletas
        /// </summary>
        static void PublicarPaqueteBoletasCertificado()
        {


            ////
            //// Parametros de la consulta
            string xsd_path = "herramientas\\SCHEMAS\\BOL\\EnvioBOLETA_v11.xsd"; //// Ruta a schema de validacion de boletas
            string cert_cn = "Marcelo Ivan Rojas Rojas       ";                  //// Cn del certificado digital
            string rut_empresa = "12959262-1";                                   //// rut de la empresa
            string rut_enviador = "12959262-1";                                  //// rut de quien envia los documentos boletas
            string fch_resol = "2022-01-01";                                     //// fecha de resolucion de la empresa
            int nro_resol = 0;                                                   //// número de resolucion 0 = certificación, mayor que cero = producción


            ////
            //// Cree la lista de documentos boletas a enviar al SII
            //// Para pasar los documentos xml en formto string timbrados y firmados
            //// utilice una lista string para agregar las boletas en formato string.
            //// Ejemplo: Se lee una boleta electrónica desde un archivo fisico y luego se carga 
            //// el contenido xml en foramto string en la lista 5 veces. Esta boleta 
            //// debe estar firmada y timbrada.
            List<string> _boletas = new List<string>();
            string path_boleta = @"Boleta\\SET_ENVIO_BOLETA.xml";
            string content = File.ReadAllText(path_boleta, Encoding.GetEncoding("ISO-8859-1"));
            _boletas.Add(content);



            ////
            //// Inicie el procesamiento de las boletas. El resultado de la operación regresará un objeto HefRespuesta
            //// con la información del procesamiento.
            HefRespuesta resp = HefPublicador.PublicadoBoletasPorLotes(
                rut_empresa,
                    rut_enviador,
                        fch_resol,
                            nro_resol,
                                cert_cn,
                                    _boletas,
                                        xsd_path);


            ////
            //// Imprima el resultado de la operación
            Console.WriteLine("EsCorrecto : {0}", resp.EsCorrecto);
            Console.WriteLine("Mensaje : {0}", resp.Mensaje);
            Console.WriteLine("Detalle : {0}", resp.Detalle);

        }

        /// <summary>
        /// Inicia la publicación de varios docuemntos Boletas
        /// </summary>
        static void PublicarPaqueteBoletasCertificadoSetBoletas()
        {


            ////
            //// Parametros de la consulta
            string xsd_path = "herramientas\\SCHEMAS\\BOL\\EnvioBOLETA_v11.xsd"; //// Ruta a schema de validacion de boletas
            string cert_cn = "MARCELO IVAN ROJAS ROJAS";                         //// Cn del certificado digital
            string rut_empresa = "77576339-6";                                   //// rut de la empresa
            string rut_enviador = "12959262-1";                                  //// rut de quien envia los documentos boletas
            string fch_resol = "2022-07-28";                                     //// fecha de resolucion de la empresa
            int nro_resol = 0;                                                   //// número de resolucion 0 = certificación, mayor que cero = producción


            ////
            //// Cree la lista de documentos boletas a enviar al SII
            //// Para pasar los documentos xml en formto string timbrados y firmados
            //// utilice una lista string para agregar las boletas en formato string.
            //// Ejemplo: Se lee una boleta electrónica desde un archivo fisico y luego se carga 
            //// el contenido xml en foramto string en la lista 5 veces. Esta boleta 
            //// debe estar firmada y timbrada.
            List<string> _boletas = new List<string>();
            string content = File.ReadAllText("CASO_BOLETA_1.XML", Encoding.GetEncoding("ISO-8859-1"));
            _boletas.Add(content);
            content = File.ReadAllText("CASO_BOLETA_2.XML", Encoding.GetEncoding("ISO-8859-1"));
            _boletas.Add(content);
            content = File.ReadAllText("CASO_BOLETA_3.XML", Encoding.GetEncoding("ISO-8859-1"));
            _boletas.Add(content);

            ////
            //// Inicie el procesamiento de las boletas. El resultado de la operación regresará un objeto HefRespuesta
            //// con la información del procesamiento.
            HefRespuesta resp = HefPublicador.PublicadoBoletasPorLotes(
                rut_empresa,
                    rut_enviador,
                        fch_resol,
                            nro_resol,
                                cert_cn,
                                    _boletas,
                                        xsd_path);


            ////
            //// Imprima el resultado de la operación
            Console.WriteLine("EsCorrecto : {0}", resp.EsCorrecto);
            Console.WriteLine("Mensaje : {0}", resp.Mensaje);
            Console.WriteLine("Detalle : {0}", resp.Detalle);




        }




    }

}
