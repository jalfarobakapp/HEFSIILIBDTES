using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HEFSIILIBDTES
{
    /// <summary>
    /// Configura el objeto para su procesamiento
    /// </summary>
    public class HefConfiguracion
    {
        /// <summary>
        /// Indica el schema para validar el documento actual
        /// </summary>
        public string FullPathSchema { get; set; }

        /// <summary>
        /// Fullpath del archivo Caf ( Folios del documento )
        /// </summary>
        public string FullPathCaf { get; set; }

        /// <summary>
        /// Nombre del certificado a utilizar para firmar el documento
        /// </summary>
        public string CnCertificado { get; set; }


        /// <summary>
        /// Fullpath del certificado 
        /// </summary>
        public string PathCertificado { get; set; }

        /// <summary>
        /// Password del certificado 
        /// </summary>
        public string PassCertificado { get; set; }





        /// <summary>
        /// Rut de quien envía el documento al SII
        /// </summary>
        public string RutEnviador { get; set; }

        /// <summary>
        /// Número de resolucion del SII
        /// </summary>
        public string NroResolucion { get; set; }

        /// <summary>
        /// Fecha de resolucion del SII
        /// </summary>
        public string FchResolucion { get; set; }

    }
}
