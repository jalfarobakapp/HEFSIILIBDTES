using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using Winnovative;

namespace HEFSIILIBDTES.PDFS
{
    /// <summary>
    /// Metodos relacionados con los documentos PDFs
    /// </summary>
    internal class HEFPDFS
    {

        /// <summary>
        /// Inicia la generación del documento pdf representativo del documento DTE
        /// </summary>
        internal static HefRespuesta GenerarDocumentoPdf(string xmlDTE, string xslDTE)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Resultado 
            byte[] resultadoBytes;

            ////
            ////
            try
            {
                #region DIRECTORIOS TEMPORALES

                ////
                //// Donde debo dejar los archivos temporales
                //// Guarda los archivos temporales en la carpeta 
                //// temporales de windows.
                string directorio_temporal = Path.Combine(Path.GetTempPath(), "HEFESTO PDF");
                Directory.CreateDirectory(directorio_temporal);


                #endregion

                #region VALIDACION DE LOS ARCHIVOS

                ////
                //// Existe el documento?
                if (!File.Exists(xmlDTE))
                    throw new Exception("No fue posible encontrar o no se tiene acceso al archivo Xml");

                ////
                //// 
                if (!File.Exists(xslDTE))
                    throw new Exception("No fue posible encontrar o no se tiene acceso al archivo Xsl");

                #endregion

                #region RECUPERAR EL DOCUMENTO DTE SUBYACENTE

                ////
                //// Recupere el documento DTE subyacente 
                string content = File.ReadAllText(xmlDTE, Encoding.GetEncoding("ISO-8859-1"));
                string sDte = Regex.Match(content, "<DTE.*?</DTE>", RegexOptions.Singleline).Value;

                ////
                //// Recupere el doc
                string sDoc = sDte;

                #endregion

                #region GENERAR IMAGEN PDF417


                ////
                //// XPATH del documento DTE
                //// Elementos necesarios para construir el nombre del archivo de salida PNG
                string sRutEmisor = Regex.Match(sDoc, "<RUTEmisor>(.*?)</RUTEmisor>").Groups[1].Value;
                string sTipoDTE = Regex.Match(sDoc, "<TipoDTE>(.*?)</TipoDTE>").Groups[1].Value;
                string sFolio = Regex.Match(sDoc, "<Folio>(.*?)</Folio>").Groups[1].Value;
                string sTED = Regex.Match(sDoc, "<TED.*?</TED>",RegexOptions.Singleline).Value;

                ////
                //// Mr:11-12-2019
                //// Nuevo archivo temporal
                string archivo_png =
                    Path.Combine(
                        directorio_temporal,
                            string.Format("R{0}T{1}F{2}.PNG", sRutEmisor, sTipoDTE, sFolio));
                
                ////
                //// Prepare el documento
                sTED = sTED.Replace("\t", string.Empty);
                sTED = sTED.Replace("\r\n", string.Empty);
                sTED = sTED.Replace("\n", string.Empty);

                ////
                //// Eliminar los espacios innecesarios
                sTED = Regex.Replace(sTED, ">\\s*<", "><", RegexOptions.Singleline);
                
                ////
                //// Eliminar archivo si es que este existe
                File.Delete(archivo_png);

                ////
                //// Inicie la construccion del barcode
                BarcodePDF417 barcode = new BarcodePDF417();
                barcode.CodeRows = 5;
                barcode.CodeColumns = 18;
                barcode.ErrorLevel = 5;
                barcode.LenCodewords = 999;

                ////
                //// Indique el tipo de compresion del timbre
                barcode.Options = BarcodePDF417.PDF417_FORCE_BINARY;

                ////
                //// Indique cual es el contenido a procesar y transformelo a 
                //// un arreglo de bytes.
                barcode.Text = Encoding.GetEncoding("ISO-8859-1").GetBytes(sTED);

                ////
                //// Cree la imagen utilizando barcode.GetImage()
                iTextSharp.text.Image image = barcode.GetImage();
                image.SetAbsolutePosition(0, 0);
                image.ScaleAbsolute(184, 72);

                ////
                //// Cree la imagen PNG del timbre electrónico 
                //// fisicamente.
                Bitmap imagen = new Bitmap(barcode.CreateDrawingImage(Color.Black, Color.White));

                ////
                //// Guarde la imagen en el disco.
                imagen.Save(archivo_png, System.Drawing.Imaging.ImageFormat.Png);

                #endregion

                #region GENERAR DOCUMENTO HTML

                ////
                //// Importante, para que la imagen del logo se vea, 
                //// es necesario indicar donde se encuentra.
                //string archivoLogo = Path.Combine(
                //    directorio_temporal,
                //    Path.GetFileName(this.PathLogo)
                //    );

                ////
                //// Si el archivo no existe copieelo
                //if (!File.Exists(archivoLogo))
                //    File.Copy(this.PathLogo, archivoLogo);


                ////
                //// Cree el archivo de resultado
                string archivoHtml =
                    Path.Combine(
                        directorio_temporal,
                         string.Format("EXP R{0}T{1}F{2}.html", sRutEmisor, sTipoDTE, sFolio)
                        );

                ////
                //// Si el archivo existe, eliminelo
                //// por precausion
                File.Delete(archivoHtml);


                ////
                //// Setee la lectura del documento xslt para aceptar secuencias javascript
                //// solo si en el futuro son necesarias
                XsltSettings settings = new XsltSettings();
                settings.EnableScript = true;
                settings.EnableDocumentFunction = true;

                ////
                //// Setear la salida del documento
                XmlWriterSettings xwsettings = new XmlWriterSettings();
                xwsettings.Indent = true;
                xwsettings.Encoding = Encoding.GetEncoding("ISO-8859-1");

                ////
                //// Cree los parametros necesarios para la transformacion
                XsltArgumentList xslParametros = new XsltArgumentList();
                xslParametros.AddParam("esCedible", "", "True");

                ////
                //// Test
                //// XmlReader reader = XmlReader.Create(new StringReader(xmlDoc.OuterXml));
                XmlReader reader = XmlReader.Create(new StringReader(sDoc));


                xwsettings.Encoding = Encoding.GetEncoding("utf-8");
                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xslDTE, settings, new XmlUrlResolver());
                using (XmlWriter xw = XmlWriter.Create(archivoHtml, xwsettings))
                {
                    ////
                    //// Realice la transformacion de los documentos
                    //// y depositelo en la carpeta out
                    xslt.Transform(reader, xslParametros, xw);

                }

                ////
                //// Regresa el documento html
                //// documento html queda almacenado en disco
                //// en variable archivoHtml

                #endregion

                #region GENERAR DOCUMENTO PDF417


                ///
                //// Crear el converto de gtml a pdf
                HtmlToPdfConverter htmlToPdfConverter = new HtmlToPdfConverter();
                htmlToPdfConverter.LicenseKey = "oy0+LD0sOz45LD09IjwsPz0iPT4iNTU1NQ==";
                htmlToPdfConverter.HtmlViewerWidth = 1024;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Legal;
                htmlToPdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdfConverter.NavigationTimeout = 60;

                ////
                //// Cree la fullpath del documento
                FileInfo f = new FileInfo(archivoHtml);

                ////
                //// Cual es el archivo a procesar



                string archivoPdfBytes = Path.Combine(f.Directory.FullName, Path.GetFileName(archivoHtml));
                if (!File.Exists(archivoPdfBytes))
                    throw new Exception("No fue posible encontra el archivo html, " + archivoPdfBytes);

                resultadoBytes = htmlToPdfConverter.ConvertHtmlFile(archivoPdfBytes);


                #endregion

                ////
                //// Regrese el valor de retorno del documento
                resp.EsCorrecto = true;
                resp.Mensaje = "Documento Pdf Construído Correctamente!!";
                resp.Detalle = "OK";
                resp.Resultado = resultadoBytes;

            }
            catch (Exception ex)
            {
                ////
                //// Notificar al usuario
                resp.EsCorrecto = false;
                resp.Mensaje = "No fue posible construir el pdf del documento.";
                resp.Detalle = ex.Message;
                resp.Resultado = null;
               
            }

            ////
            //// Regrese el valor de retorno
            return resp;


        }
               
    }

}
