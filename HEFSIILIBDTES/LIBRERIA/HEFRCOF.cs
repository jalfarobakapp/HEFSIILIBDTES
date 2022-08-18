using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace HEFSIILIBDTES.LIBRERIA.RCOF
{

    /// <summary>
    /// Representa el documento Rcof para envio de resumenes de Boletas Electrónicas
    /// </summary>
    [XmlRoot("ConsumoFolios")]
    public class HEFRCOF
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public HEFRCOF()
        {
            this.Version = "1.0";
            this.DocumentoConsumoFolios.ID = "HEFESTO_CONSUMO_FOLIOS";
            this.DocumentoConsumoFolios.Caratula.TmstFirmaEnv = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

        }

        /// <summary>
        /// Representa la version del documento
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; }

        /// <summary>
        /// Agregar Documento COnsumo de folios
        /// </summary>
        HefDocumentoConsumoFolios _HefDocumentoConsumoFolios = new HefDocumentoConsumoFolios();
        public HefDocumentoConsumoFolios DocumentoConsumoFolios {
            get { return _HefDocumentoConsumoFolios; }
            set { _HefDocumentoConsumoFolios = value; }

        }

        

        /// <summary>
        /// Agregar los elementos de configuración
        /// </summary>
        HefConfiguracion _Configuracion = new HefConfiguracion();
        [XmlIgnore]
        public HefConfiguracion Configuracion
        {
            get { return _Configuracion; }
            set { _Configuracion = value; }
        }

        /// <summary>
        /// Inicia la publicación del documento
        /// </summary>
        public HefRespuesta Publicar()
        {

            ////
            //// Beta?
            HefRespuesta resp = NEGOCIO.hefControl.esValido();
            if (!resp.EsCorrecto)
                return resp;

            ////
            //// Iniciar la publicación del documento
            return NEGOCIO.HefPublicadores.PublicarRcof(this);


        }



    }


    public class HefRangoUtilizados
    {
        public int Inicial { get; set; }
        public int Final { get; set; }
    }

    /// <summary>
    /// Rango de folios utilizados
    /// </summary>
    public class HefRangoAnulados
    {
        public int Inicial { get; set; }
        public int Final { get; set; }
    }

    /// <summary>
    /// Representa el resumen del documento
    /// </summary>
    public class HefResumen
    {

        public int TipoDocumento { get; set; }
        public long MntNeto { get; set; }
        public long MntIva { get; set; }

        public string TasaIVA { get; set; }

        public long MntExento { get; set; }
        public long MntTotal { get; set; }
        public int FoliosEmitidos { get; set; }
        public int FoliosAnulados { get; set; }
        public int FoliosUtilizados { get; set; }


        List<HefRangoUtilizados> _RangoUtilizados = new List<HefRangoUtilizados>();
        [XmlElement]
        public List<HefRangoUtilizados> RangoUtilizados
        {

            get { return _RangoUtilizados; }
            set { _RangoUtilizados = value; }
        }
        public bool ShouldSerializeRangoUtilidados() { return (RangoUtilizados.Count == 0) ? false : true; }



        List<HefRangoAnulados> _RangoAnulados = new List<HefRangoAnulados>();
        [XmlElement]
        public List<HefRangoAnulados> RangoAnulados
        {

            get { return _RangoAnulados; }
            set { _RangoAnulados = value; }
        }
        public bool ShouldSerializeRangoAnulados() { return (RangoAnulados.Count == 0) ? false : true; }


    }

    /// <summary>
    /// Representa la caratula del documento
    /// </summary>
    public class HEfCaratula
    {


        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public HEfCaratula()
        {
            this.TmstFirmaEnv = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            this.Version = "1.0";

        }

        [XmlAttribute("version")]
        public string Version { get; set; }

        public string RutEmisor { get; set; }
        public string RutEnvia { get; set; }

        public string FchResol { get; set; }
        public string NroResol { get; set; }

        public string FchInicio { get; set; }

        public string FchFinal { get; set; }

        public string Correlativo { get; set; }

        public string SecEnvio { get; set; }

        public string TmstFirmaEnv { get; set; }


    }

    /// <summary>
    /// Documento 
    /// </summary>
    public class HefDocumentoConsumoFolios
    {


        public HefDocumentoConsumoFolios()
        {

            this.ID = string.Format("IDConsumo_Folio");

        }

        [XmlAttribute("ID")]
        public string ID { get; set; }


        HEfCaratula _caratula = new HEfCaratula();
        public HEfCaratula Caratula
        {
            get { return _caratula; }
            set { _caratula = value; }

        }


        List<HefResumen> _resumenes = new List<HefResumen>();
        [XmlElement("Resumen")]
        public List<HefResumen> Resumenes
        {
            get { return _resumenes; }
            set { _resumenes = value; }
        }


    }


}
