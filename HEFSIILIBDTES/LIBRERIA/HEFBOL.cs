using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HEFSIILIBDTES.LIBRERIA.BOL
{
    /// <summary>
    /// Boleta Electronica
    /// </summary>
    [XmlRoot(ElementName = "DTE")]
    public class HEFBOLETA
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public HEFBOLETA()
        {
            this.Version = "1.0";
            this.Documento.Encabezado.IdDoc.FchEmis = DateTime.Now.ToString("yyyy-MM-dd");
            this.Documento.TmstFirma = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            this.Documento.ID = "HefestoID";
        }

        HEF_Documento _Documento = new HEF_Documento();
        public HEF_Documento Documento
        {
            get { return _Documento; }
            set { _Documento = value; }
        }

        ////
        //// Define la version de la boleta.
        [XmlAttribute("version")]
        public string Version { get; set; }

        #region METODOS INTERNOS

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
            return NEGOCIO.HefPublicadores.PublicarBol(this);
           
        }

        /// <summary>
        /// Inicia la publicación del documento boleta sin enviarlo al SII
        /// </summary>
        /// <returns></returns>
        public HefRespuesta PublicarXml()
        {
            ////
            //// Beta?
            HefRespuesta resp = NEGOCIO.hefControl.esValido();
            if (!resp.EsCorrecto)
                return resp;

            ////
            //// Iniciar la publicación del documento
            return NEGOCIO.HefPublicadores.PublicarXmlBol(this);


        }



        #endregion

        
    }
    /* Fin clase DTE */

    /// <summary>
    /// Informacion Tributaria de la Boleta
    /// </summary>
    public class HEF_Documento
    {

        /// <summary>
        /// Identificacion y Totales del Documento
        /// </summary>
        HEF_Encabezado _Encabezado = new HEF_Encabezado();
        public HEF_Encabezado Encabezado
        {
            get { return _Encabezado; }
            set { _Encabezado = value; }
        }

        /// <summary>
        /// Detalle de Itemes del Documento
        /// </summary>
        List<HEF_Detalle> _Detalle = new List<HEF_Detalle>();
        [XmlElement(ElementName = "Detalle")]
        public List<HEF_Detalle> Detalle
        {
            get { return _Detalle; }
            set { _Detalle = value; }
        }

        /// <summary>
        /// Subtotales Informativos
        /// </summary>
        List<HEF_SubTotInfo> _SubTotInfo = new List<HEF_SubTotInfo>();
        [XmlElement(ElementName = "SubTotInfo")]
        public List<HEF_SubTotInfo> SubTotInfo
        {
            get { return _SubTotInfo; }
            set { _SubTotInfo = value; }
        }

        /// <summary>
        /// Descuentos y/o Recargos que afectan al total del Documento
        /// </summary>
        List<HEF_DscRcgGlobal> _DscRcgGlobal = new List<HEF_DscRcgGlobal>();
        [XmlElement(ElementName = "DscRcgGlobal")]
        public List<HEF_DscRcgGlobal> DscRcgGlobal
        {
            get { return _DscRcgGlobal; }
            set { _DscRcgGlobal = value; }
        }

        /// <summary>
        /// Identificacion de otros documentos Referenciados por Documento
        /// </summary>
        List<HEF_Referencia> _Referencia = new List<HEF_Referencia>();
        [XmlElement(ElementName = "Referencia")]
        public List<HEF_Referencia> Referencia
        {
            get { return _Referencia; }
            set { _Referencia = value; }
        }

        /// <summary>
        /// Fecha y Hora en que se Firmo Digitalmente el Documento AAAA-MM-DDTHH:MI:SS
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string TmstFirma { get; set; }


        [XmlAttribute("ID")]
        public string ID { get; set; }


    } /* Fin clase Documento */

    /// <summary>
    /// Identificacion y Totales del Documento
    /// </summary>
    public class HEF_Encabezado
    {

        /// <summary>
        /// Identificacion del DTE
        /// </summary>
        HEF_IdDoc _IdDoc = new HEF_IdDoc();
        public HEF_IdDoc IdDoc
        {
            get { return _IdDoc; }
            set { _IdDoc = value; }
        }

        /// <summary>
        /// Datos del Emisor
        /// </summary>
        HEF_Emisor _Emisor = new HEF_Emisor();
        public HEF_Emisor Emisor
        {
            get { return _Emisor; }
            set { _Emisor = value; }
        }

        /// <summary>
        /// Datos del Receptor
        /// </summary>
        HEF_Receptor _Receptor = new HEF_Receptor();
        public HEF_Receptor Receptor
        {
            get { return _Receptor; }
            set { _Receptor = value; }
        }

        /// <summary>
        /// Montos Totales del DTE
        /// </summary>
        HEF_Totales _Totales = new HEF_Totales();
        public HEF_Totales Totales
        {
            get { return _Totales; }
            set { _Totales = value; }
        }


    } /* Fin clase Encabezado */

    /// <summary>
    /// Identificacion del DTE
    /// </summary>
    public class HEF_IdDoc
    {

        /// <summary>
        /// Tipo de Boleta
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : int
        /// </remarks>
        public int TipoDTE { get; set; }

        /// <summary>
        /// Folio del Documento Electronico
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : int
        /// </remarks>
        public int Folio { get; set; }

        /// <summary>
        /// Fecha Emision Contable del DTE (AAAA-MM-DD)
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string FchEmis { get; set; }

        /// <summary>
        /// Indica el Tipo de Transaccion
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double IndServicio { get; set; }
        public bool ShouldSerializeIndServicio() { return (IndServicio == 0) ? false : true; }

        /// <summary>
        /// Indica el Uso de Montos Netos en Detalle
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : int
        /// </remarks>
        public int IndMntNeto { get; set; }
        public bool ShouldSerializeIndMntNeto() { return (IndMntNeto == 0) ? false : true; }

        /// <summary>
        /// Periodo de Facturacion - Desde (AAAA-MM-DD)
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string PeriodoDesde { get; set; }

        /// <summary>
        /// Periodo Facturacion - Hasta (AAAA-MM-DD)
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string PeriodoHasta { get; set; }

        /// <summary>
        /// Fecha de Vencimiento del Pago (AAAA-MM-DD)
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string FchVenc { get; set; }


    } /* Fin clase IdDoc */

    /// <summary>
    /// Datos del Emisor
    /// </summary>
    public class HEF_Emisor
    {

        /// <summary>
        /// RUT del Emisor del DTE
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string RUTEmisor { get; set; }

        /// <summary>
        /// Nombre o Razon Social del Emisor
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string RznSocEmisor { get; set; }

        /// <summary>
        /// Giro del Emisor que Corresponde a la Transaccion
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string GiroEmisor { get; set; }

        /// <summary>
        /// Codigo de Sucursal Entregado por el SII
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double CdgSIISucur { get; set; }
        public bool ShouldSerializeCdgSIISucur() { return (CdgSIISucur == 0) ? false : true; }

        /// <summary>
        /// Direccion de Origen o Emision
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string DirOrigen { get; set; }

        /// <summary>
        /// Comuna de Origen
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CmnaOrigen { get; set; }

        /// <summary>
        /// Ciudad de Origen
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CiudadOrigen { get; set; }


    } /* Fin clase Emisor */

    /// <summary>
    /// Datos del Receptor
    /// </summary>
    public class HEF_Receptor
    {

        /// <summary>
        /// RUT del Receptor del DTE
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string RUTRecep { get; set; }

        /// <summary>
        /// Codigo Interno del Receptor
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CdgIntRecep { get; set; }

        /// <summary>
        /// Nombre o Razon Social del Receptor
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string RznSocRecep { get; set; }

        /// <summary>
        /// Telefono o E-mail de Contacto del Receptor
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string Contacto { get; set; }

        /// <summary>
        /// Direccion en la Cual se Envian los Productos o se Prestan los Servicios
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string DirRecep { get; set; }

        /// <summary>
        /// Comuna de Recepcion
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CmnaRecep { get; set; }

        /// <summary>
        /// Ciudad de Recepcion
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CiudadRecep { get; set; }

        /// <summary>
        /// Direccion Postal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string DirPostal { get; set; }

        /// <summary>
        /// Comuna Postal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CmnaPostal { get; set; }

        /// <summary>
        /// Ciudad Postal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CiudadPostal { get; set; }


    } /* Fin clase Receptor */

    /// <summary>
    /// Montos Totales del DTE
    /// </summary>
    public class HEF_Totales
    {

        /// <summary>
        /// Monto Neto
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double MntNeto { get; set; }
        public bool ShouldSerializeMntNeto() { return (MntNeto == 0) ? false : true; }

        /// <summary>
        /// Monto Exento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double MntExe { get; set; }
        public bool ShouldSerializeMntExe() { return (MntExe == 0) ? false : true; }

        /// <summary>
        /// Monto de IVA
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double IVA { get; set; }
        public bool ShouldSerializeIVA() { return (IVA == 0) ? false : true; }

        /// <summary>
        /// Monto Total
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double MntTotal { get; set; }

        /// <summary>
        /// Monto No Facturable - Corresponde a Bienes o Servicios Facturados Previamente
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double MontoNF { get; set; }
        public bool ShouldSerializeMontoNF() { return (MontoNF == 0) ? false : true; }

        /// <summary>
        /// Total de Ventas o Servicios del Periodo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double TotalPeriodo { get; set; }
        public bool ShouldSerializeTotalPeriodo() { return (TotalPeriodo == 0) ? false : true; }

        /// <summary>
        /// Saldo Anterior - Puede ser Negativo o Positivo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double SaldoAnterior { get; set; }
        public bool ShouldSerializeSaldoAnterior() { return (SaldoAnterior == 0) ? false : true; }

        /// <summary>
        /// Valor a Pagar Total del Documento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double VlrPagar { get; set; }
        public bool ShouldSerializeVlrPagar() { return (VlrPagar == 0) ? false : true; }


    } /* Fin clase Totales */

    /// <summary>
    /// Detalle de Itemes del Documento
    /// </summary>
    public class HEF_Detalle
    {

        /// <summary>
        /// Numero Secuencial de Linea
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double NroLinDet { get; set; }

        /// <summary>
        /// Codificacion del Item
        /// </summary>
        List<HEF_CdgItem> _CdgItem = new List<HEF_CdgItem>();
        [XmlElement(ElementName = "CdgItem")]
        public List<HEF_CdgItem> CdgItem
        {
            get { return _CdgItem; }
            set { _CdgItem = value; }
        }

        /// <summary>
        /// Indicador de Exencion/Facturacion
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double IndExe { get; set; }
        public bool ShouldSerializeIndExe() { return (IndExe == 0) ? false : true; }

        /// <summary>
        /// Indica si el ítem es :01: TICKET02: VALOR SERVICIO
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double ItemEspectaculo { get; set; }
        public bool ShouldSerializeItemEspectaculo() { return (ItemEspectaculo == 0) ? false : true; }

        /// <summary>
        /// Rut de la Empresa Mandante de la Boleta
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string RUTMandante { get; set; }

        /// <summary>
        /// Nombre del Item
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string NmbItem { get; set; }

        /// <summary>
        /// Informacion de la entrada
        /// </summary>
        HEF_InfoTicket _InfoTicket;
        public HEF_InfoTicket InfoTicket
        {
            get { return _InfoTicket; }
            set { _InfoTicket = value; }
        }

        /// <summary>
        /// Descripcion del Item
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string DscItem { get; set; }

        /// <summary>
        /// Cantidad del Item
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public decimal QtyItem { get; set; }
        public bool ShouldSerializeQtyItem() { return (QtyItem == 0) ? false : true; }

        /// <summary>
        /// Unidad de Medida
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string UnmdItem { get; set; }

        /// <summary>
        /// Precio Unitario del Item en Pesos
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public decimal PrcItem { get; set; }
        public bool ShouldSerializePrcItem() { return (PrcItem == 0) ? false : true; }

        /// <summary>
        /// Porcentaje de Descuento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double DescuentoPct { get; set; }
        public bool ShouldSerializeDescuentoPct() { return (DescuentoPct == 0) ? false : true; }

        /// <summary>
        /// Monto de Descuento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double DescuentoMonto { get; set; }
        public bool ShouldSerializeDescuentoMonto() { return (DescuentoMonto == 0) ? false : true; }

        /// <summary>
        /// Porcentaje de Recargo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double RecargoPct { get; set; }
        public bool ShouldSerializeRecargoPct() { return (RecargoPct == 0) ? false : true; }

        /// <summary>
        /// Monto de Recargo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double RecargoMonto { get; set; }
        public bool ShouldSerializeRecargoMonto() { return (RecargoMonto == 0) ? false : true; }

        /// <summary>
        /// Monto por Linea de Detalle. Corresponde al Monto Bruto, a menos que IndMntNeto Indique lo Contrario 
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double MontoItem { get; set; }


    } /* Fin clase Detalle */

    /// <summary>
    /// Codificacion del Item
    /// </summary>
    public class HEF_CdgItem
    {

        /// <summary>
        /// Tipo de Codificacion
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string TpoCodigo { get; set; }

        /// <summary>
        /// Valor del Codigo de Item, para la Codificacion Particular
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string VlrCodigo { get; set; }


    } /* Fin clase CdgItem */

    /// <summary>
    /// Informacion de la entrada
    /// </summary>
    public class HEF_InfoTicket
    {

        /// <summary>
        /// Corresponde a la numeración única para el evento.
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double FolioTicket { get; set; }

        /// <summary>
        /// Corresponde a la fecha y hora de generación del ticket(AAAA-MM-DDThh:mm:ss)
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string FchGenera { get; set; }

        /// <summary>
        /// Nombre del Espectáculo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string NmbEvento { get; set; }

        /// <summary>
        /// Tipo de ticket, Por ejemplo: Adulto, Niño, etc
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string TpoTiket { get; set; }

        /// <summary>
        /// Código asociado al Evento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CdgEvento { get; set; }

        /// <summary>
        /// Fecha y hora de realización del evento AAAA-MM-DDThh:mm:ss)
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string FchEvento { get; set; }

        /// <summary>
        /// Dirección o identificación del recinto donde se realizará el  Espectáculo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string LugarEvento { get; set; }

        /// <summary>
        /// Sector/Sección de la ubicación en el evento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string UbicEvento { get; set; }

        /// <summary>
        /// Fila correspondiente a la Ubicación en el evento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string FilaUbicEvento { get; set; }

        /// <summary>
        /// N° de Asiento correspondiente a la Ubicación en el evento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string AsntoUbicEvento { get; set; }


    } /* Fin clase InfoTicket */

    /// <summary>
    /// Subtotales Informativos
    /// </summary>
    public class HEF_SubTotInfo
    {

        /// <summary>
        /// Número de Subtotal 
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double NroSTI { get; set; }

        /// <summary>
        /// Glosa
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string GlosaSTI { get; set; }

        /// <summary>
        /// Ubicación para Impresión 
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double OrdenSTI { get; set; }

        /// <summary>
        /// Valor Neto del Subtotal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double SubTotNetoSTI { get; set; }
        public bool ShouldSerializeSubTotNetoSTI() { return (SubTotNetoSTI == 0) ? false : true; }

        /// <summary>
        /// Valor del IVA del Subtotal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double SubTotIVASTI { get; set; }
        public bool ShouldSerializeSubTotIVASTI() { return (SubTotIVASTI == 0) ? false : true; }

        /// <summary>
        /// Valor de los Impuestos adicionales o específicos del Subtotal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double SubTotAdicSTI { get; set; }
        public bool ShouldSerializeSubTotAdicSTI() { return (SubTotAdicSTI == 0) ? false : true; }

        /// <summary>
        /// Valor no Afecto o Exento del Subtotal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double SubTotExeSTI { get; set; }
        public bool ShouldSerializeSubTotExeSTI() { return (SubTotExeSTI == 0) ? false : true; }

        /// <summary>
        /// Valor de la línea de subtotal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double ValSubtotSTI { get; set; }
        public bool ShouldSerializeValSubtotSTI() { return (ValSubtotSTI == 0) ? false : true; }

        /// <summary>
        /// TABLA de  Líneas de Detalle que se agrupan en el Subtotal
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double LineasDeta { get; set; }
        public bool ShouldSerializeLineasDeta() { return (LineasDeta == 0) ? false : true; }


    } /* Fin clase SubTotInfo */

    /// <summary>
    /// Descuentos y/o Recargos que afectan al total del Documento
    /// </summary>
    public class HEF_DscRcgGlobal
    {

        /// <summary>
        /// Numero Secuencial de Linea
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double NroLinDR { get; set; }

        /// <summary>
        /// Tipo de Movimiento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string TpoMov { get; set; }

        /// <summary>
        /// Descripcion del Descuento o Recargo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string GlosaDR { get; set; }

        /// <summary>
        /// Unidad en que se Expresa el Valor
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string TpoValor { get; set; }

        /// <summary>
        /// Valor del Descuento o Recargo
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double ValorDR { get; set; }

        /// <summary>
        /// Indica si el Descuento o Recargo Afecta a Itemes Exentos o No Facturables
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double IndExeDR { get; set; }
        public bool ShouldSerializeIndExeDR() { return (IndExeDR == 0) ? false : true; }


    } /* Fin clase DscRcgGlobal */

    /// <summary>
    /// Identificacion de otros documentos Referenciados por Documento
    /// </summary>
    public class HEF_Referencia
    {

        /// <summary>
        /// Numero Secuencial de Linea de Referencia
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : Si
        /// Tipo de dato utilizado : double
        /// </remarks>
        public double NroLinRef { get; set; }

        /// <summary>
        /// Codigo Interno del Tipo de Referencia
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CodRef { get; set; }

        /// <summary>
        /// Razon Explicita por la que se Referencia el Documento
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string RazonRef { get; set; }

        /// <summary>
        /// Código del Vendedor establecido por la Empresa. Puede estar asociado a INTERNET
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CodVndor { get; set; }

        /// <summary>
        /// Código de la caja establecido por la Empresa
        /// </summary>
        /// <remarks>
        /// Campo es obligatorio   : No
        /// Tipo de dato utilizado : string
        /// </remarks>
        public string CodCaja { get; set; }


    } /* Fin clase Referencia */
 
}
