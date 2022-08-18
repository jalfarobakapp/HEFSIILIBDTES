using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HEFSIILIBDTES
{
    /// <summary>
    /// Representa la respuesta del proceso
    /// </summary>
    public class HefRespuesta
    {

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public HefRespuesta()
        {
            this.FchProceso = DateTime.Now;
        }

        /// <summary>
        /// Indica la fecha del proceso
        /// </summary>
        public DateTime FchProceso { get; set; }


        /// <summary>
        /// Indica si el proceso se ejecuto correctamente
        /// </summary>
        public bool EsCorrecto { get; set; }

        /// <summary>
        /// Nombre del proceso
        /// </summary>
        public string Proceso { get; set; }

        /// <summary>
        /// Mensaje del proceso
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Detalle adicional del proceso o error
        /// </summary>
        public string Detalle { get; set; }

        /// <summary>
        /// Resultado de la operación
        /// </summary>
        public object Resultado { get; set; }

        /// <summary>
        /// Representa el trackid de l operación de envio al SII
        /// </summary>
        public string Trackid { get; set; }

        /// <summary>
        /// Representa el documento procesado
        /// </summary>
        public string XmlDocumento { get; set; }



    }
}
