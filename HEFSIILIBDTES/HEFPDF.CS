using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEFSIILIBDTES
{
    /// <summary>
    /// Metodos relacionados con generación de los PDFS
    /// </summary>
    public class HEFPDF
    {

        /// <summary>
        /// Inicia la generación de los documentos PDFs
        /// </summary>
        /// <param name="xmlDTE">Fullpath del documento DTE</param>
        /// <param name="xslDTE">Fullpath del documento XSLT</param>
        public static HefRespuesta GenerarDocumentoPdf(string xmlDTE, string xslDTE)
        {
            
            ////
            //// Recuperar los bytes del documento
            return PDFS.HEFPDFS.GenerarDocumentoPdf(xmlDTE, xslDTE);

        }

        /// <summary>
        /// Genera la imagen PDF417
        /// </summary>
        /// <param name="xmlDTE"></param>
        /// <returns>Arreglo de bytes que representan el PDF417</returns>
        public static HefRespuesta GenerarDocumentoPdf417(string xmlDTE)
        {

            ////
            //// Beta?
            HefRespuesta respVal = NEGOCIO.hefControl.esValido();
            if (!respVal.EsCorrecto)
                return respVal;

            ////
            //// Recuperar los bytes del documento
            return PDFS.HEFPDF417.GenerarDocumentoPdf417(xmlDTE);

        }




    }
}
