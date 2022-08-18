using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HEFSIILIBDTES.PDFS
{
    internal class HEFPDF417
    {
        
        /// <summary>
        /// Genera la imagen pdf417
        /// </summary>
        /// <returns>Retorno arreglo de bytes</returns>
        internal static HefRespuesta GenerarDocumentoPdf417(string xmlDTE)
        {
            ////
            //// Iniciar la respuesta
            HefRespuesta resp = new HefRespuesta();

            ////
            //// Resultado 
            byte[] resultadoBytes;

            ////
            //// Iniciar el proceso
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
                string sTED = Regex.Match(sDoc, "<TED.*?</TED>", RegexOptions.Singleline).Value;

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

                ////
                //// Recupere los bytes del archivo
                resultadoBytes = File.ReadAllBytes(archivo_png);
                File.Delete(archivo_png);

                #endregion

                ////
                //// Regresa el documento html
                //// documento html queda almacenado en disco
                //// en variable archivoHtml
                resp.EsCorrecto = true;
                resp.Mensaje = "Generación PDF417";
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
