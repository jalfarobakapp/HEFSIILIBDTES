<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sii="http://www.sii.cl/SiiDte" exclude-result-prefixes="sii">
  <xsl:output indent="yes" method="html" encoding="iso-8859-1" omit-xml-declaration="yes"/>
  <xsl:decimal-format  name="moneda" decimal-separator="," grouping-separator="."/>

  <!--PARAMETROS -->
  <xsl:param name="esCedible"/>
  
  <!-- PARAMEROS INTERNOS -->
  <xsl:param name="SIIDireccionRegional">S.I.I.- SANTIAGO ORIENTE</xsl:param>
  <xsl:param name="PrefijoFecha">Santiago</xsl:param>
    
  <!-- BEGIN -->
  <xsl:template match="/">
    <xsl:call-template name="crearRepresentacion"/>    
  </xsl:template>
  
  <!-- INDICADOR DE TRASLADO SOLO GUIAS DE DESPACHO -->
  <xsl:template name="indicadorTraslado">
    <xsl:variable name="IT" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado"/>
    <xsl:choose>
      <xsl:when test="$IT='1'">Operación constituye venta</xsl:when>
      <xsl:when test="$IT='2'">Ventas por efectuar</xsl:when>
      <xsl:when test="$IT='3'">Consignaciones</xsl:when>
      <xsl:when test="$IT='4'">Entregas gratuitas</xsl:when>
      <xsl:when test="$IT='5'">Traslados internos</xsl:when>
      <xsl:when test="$IT='6'">Otros traslados no venta</xsl:when>
      <xsl:when test="$IT='7'">Guía de devolución</xsl:when>
      <xsl:when test="$IT='8'">Traslados para exportación(no venta)</xsl:when>
      <xsl:when test="$IT='9'">Venta para exportación</xsl:when>
      <xsl:otherwise>Contado</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <!-- FUNCIONES forma de pago-->
  <xsl:template name="formaDePago">
    <xsl:variable name="FP" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:FmaPago"/>
    <xsl:choose>
      <xsl:when test="$FP='1'">Contado</xsl:when>
      <xsl:when test="$FP='2'">Credito</xsl:when>
      <xsl:when test="$FP='3'">Sin Costo(entrega gratuita)</xsl:when>
      <xsl:otherwise>Contado</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
   
  <!-- FUNCIONES crear uri PDF417 -->
  <xsl:template name="uriPdf417">
    <xsl:variable name="E" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:RUTEmisor"/>
    <xsl:variable name="F" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:Folio"/>
    <xsl:variable name="T" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE"/>
    <xsl:value-of select="concat('R',$E,'T',$T,'F',$F,'.png')"/>
  </xsl:template>
    
  <!-- FUNCIONES FormatoRut -->
  <xsl:template name="FormatoRut">
    <xsl:param name="Rut"/>
    <xsl:variable name="body" select="format-number(substring-before($Rut,'-'),'###.###.###','moneda')"/>
    <xsl:variable name="digito" select="translate(substring-after($Rut,'-'),'k','K')"/>
    <xsl:value-of select="concat($body,'-',$digito)"/>
  </xsl:template>
  
  <!-- FUNCIONES NombreDocumento -->
  <xsl:template name="NombreDocumento">
    <xsl:param name="tip"/>
    <xsl:variable name="resultado">
      <xsl:choose>
        <xsl:when test="$tip='33'">FACTURA ELECTRÓNICA</xsl:when>
        <xsl:when test="$tip='34'">FACTURA NO AFECTA O EXENTA ELECTRÓNICA</xsl:when>
        <xsl:when test="$tip='46'">FACTURA DE COMPRA ELECTRÓNICA</xsl:when>
        <xsl:when test="$tip='56'">NOTA DE DÉBITO ELECTRÓNICA</xsl:when>
        <xsl:when test="$tip='61'">NOTA DE CRÉDITO ELECTRÓNICA</xsl:when>
        <xsl:when test="$tip='52'">GUÍA DE DESPACHO ELECTRÓNICA</xsl:when>
		<xsl:when test="$tip='110'">FACTURA DE EXPORTACIÓN ELECTRÓNICA</xsl:when>
		<xsl:when test="$tip='111'">NOTA DE DÉBITO DE EXPORTACIÓN ELECTRÓNICA</xsl:when>
		<xsl:when test="$tip='112'">NOTA DE CRÉDITO DE EXPORTACIÓN ELECTRÓNICA</xsl:when>
		<xsl:when test="$tip='807'">DUS</xsl:when>
		<xsl:when test="$tip='809'">AWB (Air Will Bill)</xsl:when>
		<xsl:when test="$tip='810'">MIC/DTA</xsl:when>
		<xsl:when test="$tip='812'">Resolución del SNA donde califica Servicios de Exportación</xsl:when>
		<xsl:when test="$tip='813'">Pasaporte</xsl:when>

		<xsl:otherwise>&#160;</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of select="$resultado"/>
  </xsl:template>
  
  <!-- FORMATEO DE FECHA DE GENERACION -->
  <xsl:template name="FechaFormateado">
    <!-- RESULTADO DE LA FUNCION -->
	<div style="text-align:right;padding:3px 3px 3px 3px;font-family:verdana;font-size:13px">
	
		<xsl:value-of select="$PrefijoFecha"/>,&#160;
		<xsl:value-of select="substring(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:FchEmis,9,2)"/>
		
		<xsl:variable name="indiceMes" select="substring(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:FchEmis,6,2)"/>
		<xsl:choose>
			<xsl:when test="$indiceMes='01'"> Enero </xsl:when>
			<xsl:when test="$indiceMes='02'"> Febrero </xsl:when>
			<xsl:when test="$indiceMes='03'"> Marzo </xsl:when>
			<xsl:when test="$indiceMes='04'"> Abril </xsl:when>
			<xsl:when test="$indiceMes='05'"> Mayo </xsl:when>
			<xsl:when test="$indiceMes='06'"> Junio </xsl:when>
			<xsl:when test="$indiceMes='07'"> Julio </xsl:when>
			<xsl:when test="$indiceMes='08'"> Agosto </xsl:when>
			<xsl:when test="$indiceMes='09'"> Septiembre </xsl:when>
			<xsl:when test="$indiceMes='10'"> Octubre </xsl:when>
			<xsl:when test="$indiceMes='11'"> Noviembre </xsl:when>
			<xsl:when test="$indiceMes='12'"> Diciembre </xsl:when>
        </xsl:choose>
		de&#160;
		<xsl:value-of select="substring(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:FchEmis,1,4)"/>
		
	</div>
  </xsl:template>
  
  <!-- PUERTOS -->
  <xsl:template name="Puertos">
 	<xsl:param name="CODIGO"/>
	<xsl:choose>
		<xsl:when test="$CODIGO='111'">MONTREAL</xsl:when>
		<xsl:when test="$CODIGO='112'">COSTA DEL PACIFICO, OTROS NO E</xsl:when>
		<xsl:when test="$CODIGO='113'">HALIFAX</xsl:when>
		<xsl:when test="$CODIGO='114'">VANCOUVER</xsl:when>
		<xsl:when test="$CODIGO='115'">SAINT JOHN</xsl:when>
		<xsl:when test="$CODIGO='116'">TORONTO</xsl:when>
		<xsl:when test="$CODIGO='117'">OTROS PUERTOS DE CANADA NO IDE</xsl:when>
		<xsl:when test="$CODIGO='118'">BAYSIDE</xsl:when>
		<xsl:when test="$CODIGO='120'">PORT CARTIES</xsl:when>
		<xsl:when test="$CODIGO='121'">COSTA DEL ATLANTICO, OTROS NO</xsl:when>
		<xsl:when test="$CODIGO='122'">PUERTOS DEL GOLFO DE MEXICO, O</xsl:when>
		<xsl:when test="$CODIGO='123'">COSTA DEL PACIFICO, OTROS NO E</xsl:when>
		<xsl:when test="$CODIGO='124'">QUEBEC</xsl:when>
		<xsl:when test="$CODIGO='125'">PRINCE RUPERT</xsl:when>
		<xsl:when test="$CODIGO='126'">HAMILTON</xsl:when>
		<xsl:when test="$CODIGO='131'">BOSTON</xsl:when>
		<xsl:when test="$CODIGO='132'">NEW HAVEN</xsl:when>
		<xsl:when test="$CODIGO='133'">BRIDGEPORT</xsl:when>
		<xsl:when test="$CODIGO='134'">NEW YORK</xsl:when>
		<xsl:when test="$CODIGO='135'">FILADELFIA</xsl:when>
		<xsl:when test="$CODIGO='136'">BALTIMORE</xsl:when>
		<xsl:when test="$CODIGO='137'">NORFOLK</xsl:when>
		<xsl:when test="$CODIGO='138'">WILMINGTON</xsl:when>
		<xsl:when test="$CODIGO='139'">CHARLESTON</xsl:when>
		<xsl:when test="$CODIGO='140'">SAVANAH</xsl:when>
		<xsl:when test="$CODIGO='141'">MIAMI</xsl:when>
		<xsl:when test="$CODIGO='142'">EVERGLADES</xsl:when>
		<xsl:when test="$CODIGO='143'">JACKSONVILLE</xsl:when>
		<xsl:when test="$CODIGO='145'">PALM BEACH</xsl:when>
		<xsl:when test="$CODIGO='146'">BATON ROUGE</xsl:when>
		<xsl:when test="$CODIGO='147'">COLUMBRES</xsl:when>
		<xsl:when test="$CODIGO='148'">PITTSBURGH</xsl:when>
		<xsl:when test="$CODIGO='149'">DULUTH</xsl:when>
		<xsl:when test="$CODIGO='150'">MILWAUKEE</xsl:when>
		<xsl:when test="$CODIGO='151'">TAMPA</xsl:when>
		<xsl:when test="$CODIGO='152'">PENSACOLA</xsl:when>
		<xsl:when test="$CODIGO='153'">MOBILE</xsl:when>
		<xsl:when test="$CODIGO='154'">NEW ORLEANS</xsl:when>
		<xsl:when test="$CODIGO='155'">PORT ARTHUR</xsl:when>
		<xsl:when test="$CODIGO='156'">GALVESTON</xsl:when>
		<xsl:when test="$CODIGO='157'">CORPUS CRISTI</xsl:when>
		<xsl:when test="$CODIGO='158'">BROWSVILLE</xsl:when>
		<xsl:when test="$CODIGO='159'">HOUSTON</xsl:when>
		<xsl:when test="$CODIGO='160'">OAKLAND</xsl:when>
		<xsl:when test="$CODIGO='161'">STOCKTON</xsl:when>
		<xsl:when test="$CODIGO='171'">SEATLE</xsl:when>
		<xsl:when test="$CODIGO='172'">PORTLAND</xsl:when>
		<xsl:when test="$CODIGO='173'">SAN FRANCISCO</xsl:when>
		<xsl:when test="$CODIGO='174'">LOS ANGELES</xsl:when>
		<xsl:when test="$CODIGO='175'">LONG BEACH</xsl:when>
		<xsl:when test="$CODIGO='176'">SAN DIEGO</xsl:when>
		<xsl:when test="$CODIGO='180'">OTROS PUERTOS DE ESTADOS UNIDO</xsl:when>
		<xsl:when test="$CODIGO='199'">LOS VILOS</xsl:when>
		<xsl:when test="$CODIGO='201'">PUCHOCO</xsl:when>
		<xsl:when test="$CODIGO='202'">OXIQUIM</xsl:when>
		<xsl:when test="$CODIGO='203'">T. GASERO ABASTIBLE</xsl:when>
		<xsl:when test="$CODIGO='204'">PATACHE</xsl:when>
		<xsl:when test="$CODIGO='205'">CALBUCO</xsl:when>
		<xsl:when test="$CODIGO='206'">MICHILLA</xsl:when>
		<xsl:when test="$CODIGO='207'">PUERTO ANGAMOS</xsl:when>
		<xsl:when test="$CODIGO='208'">POSEIDON</xsl:when>
		<xsl:when test="$CODIGO='209'">TRES PUENTES</xsl:when>
		<xsl:when test="$CODIGO='210'">OTROS PUERTOS DE MEXICO NO ESP</xsl:when>
		<xsl:when test="$CODIGO='211'">TAMPICO</xsl:when>
		<xsl:when test="$CODIGO='212'">COSTA DEL PACIFICO, OTROS PUER</xsl:when>
		<xsl:when test="$CODIGO='213'">VERACRUZ</xsl:when>
		<xsl:when test="$CODIGO='214'">COATZACOALCOS</xsl:when>
		<xsl:when test="$CODIGO='215'">GUAYMAS</xsl:when>
		<xsl:when test="$CODIGO='216'">MAZATLAN</xsl:when>
		<xsl:when test="$CODIGO='217'">MANZANILLO</xsl:when>
		<xsl:when test="$CODIGO='218'">ACAPULCO</xsl:when>
		<xsl:when test="$CODIGO='219'">GOLFO DE MEXICO, OTROS NO ESPE</xsl:when>
		<xsl:when test="$CODIGO='220'">ALTAMIRA</xsl:when>
		<xsl:when test="$CODIGO='221'">CRISTOBAL</xsl:when>
		<xsl:when test="$CODIGO='222'">BALBOA</xsl:when>
		<xsl:when test="$CODIGO='223'">COLON</xsl:when>
		<xsl:when test="$CODIGO='224'">OTROS PUERTOS DE PANAMA NO ESP</xsl:when>
		<xsl:when test="$CODIGO='231'">OTROS PUERTOS DE COLOMBIA NO E</xsl:when>
		<xsl:when test="$CODIGO='232'">BUENAVENTURA</xsl:when>
		<xsl:when test="$CODIGO='233'">BARRANQUILLA</xsl:when>
		<xsl:when test="$CODIGO='241'">OTROS PUERTOS DE ECUADOR NO ES</xsl:when>
		<xsl:when test="$CODIGO='242'">GUAYAQUIL</xsl:when>
		<xsl:when test="$CODIGO='251'">OTROS PUERTOS DE PERU NO ESPEC</xsl:when>
		<xsl:when test="$CODIGO='252'">CALLAO</xsl:when>
		<xsl:when test="$CODIGO='253'">ILO</xsl:when>
		<xsl:when test="$CODIGO='254'">IQUITOS</xsl:when>
		<xsl:when test="$CODIGO='261'">OTROS PUERTOS DE ARGENTINA NO</xsl:when>
		<xsl:when test="$CODIGO='262'">BUENOS AIRES</xsl:when>
		<xsl:when test="$CODIGO='263'">NECOCHEA</xsl:when>
		<xsl:when test="$CODIGO='264'">MENDOZA</xsl:when>
		<xsl:when test="$CODIGO='265'">CORDOBA</xsl:when>
		<xsl:when test="$CODIGO='266'">BAHIA BLANCA</xsl:when>
		<xsl:when test="$CODIGO='267'">COMODORO RIVADAVIA</xsl:when>
		<xsl:when test="$CODIGO='268'">PUERTO MADRYN</xsl:when>
		<xsl:when test="$CODIGO='269'">MAR DEL PLATA</xsl:when>
		<xsl:when test="$CODIGO='270'">ROSARIO</xsl:when>
		<xsl:when test="$CODIGO='271'">OTROS PUERTOS DE URUGUAY NO ES</xsl:when>
		<xsl:when test="$CODIGO='272'">MONTEVIDEO</xsl:when>
		<xsl:when test="$CODIGO='281'">OTROS PUERTOS DE VENEZUELA NO</xsl:when>
		<xsl:when test="$CODIGO='282'">LA GUAIRA</xsl:when>
		<xsl:when test="$CODIGO='285'">MARACAIBO</xsl:when>
		<xsl:when test="$CODIGO='291'">OTROS PUERTOS DE BRASIL NO ESP</xsl:when>
		<xsl:when test="$CODIGO='292'">SANTOS</xsl:when>
		<xsl:when test="$CODIGO='293'">RIO JANEIRO</xsl:when>
		<xsl:when test="$CODIGO='294'">RIO GRANDE DEL SUR</xsl:when>
		<xsl:when test="$CODIGO='295'">PARANAGUA</xsl:when>
		<xsl:when test="$CODIGO='296'">SAO PAULO</xsl:when>
		<xsl:when test="$CODIGO='297'">SALVADOR</xsl:when>
		<xsl:when test="$CODIGO='301'">OTROS PUERTOS DE LAS ANTILLAS</xsl:when>
		<xsl:when test="$CODIGO='302'">CURAZAO</xsl:when>
		<xsl:when test="$CODIGO='399'">OTROS PUERTOS DE AMERICA NO ES</xsl:when>
		<xsl:when test="$CODIGO='411'">SHANGAI</xsl:when>
		<xsl:when test="$CODIGO='412'">DAIREN</xsl:when>
		<xsl:when test="$CODIGO='413'">OTROS PUERTOS DE CHINA NO ESPE</xsl:when>
		<xsl:when test="$CODIGO='421'">NANPO</xsl:when>
		<xsl:when test="$CODIGO='422'">BUSAN CY (PUSAN)</xsl:when>
		<xsl:when test="$CODIGO='423'">OTROS PUERTOS DE COREA</xsl:when>
		<xsl:when test="$CODIGO='431'">MANILA</xsl:when>
		<xsl:when test="$CODIGO='432'">OTROS PUERTOS DE FILIPINAS NO</xsl:when>
		<xsl:when test="$CODIGO='441'">OTROS PUERTOS DE JAPON NO ESPE</xsl:when>
		<xsl:when test="$CODIGO='442'">OSAKA</xsl:when>
		<xsl:when test="$CODIGO='443'">KOBE</xsl:when>
		<xsl:when test="$CODIGO='444'">YOKOHAMA</xsl:when>
		<xsl:when test="$CODIGO='445'">NAGOYA</xsl:when>
		<xsl:when test="$CODIGO='446'">SHIMIZUI</xsl:when>
		<xsl:when test="$CODIGO='447'">MOJI</xsl:when>
		<xsl:when test="$CODIGO='448'">YAWATA</xsl:when>
		<xsl:when test="$CODIGO='449'">FUKUYAMA</xsl:when>
		<xsl:when test="$CODIGO='451'">KAOHSIUNG</xsl:when>
		<xsl:when test="$CODIGO='452'">KEELUNG</xsl:when>
		<xsl:when test="$CODIGO='453'">OTROS PUERTOS DE TAIWAN NO ESP</xsl:when>
		<xsl:when test="$CODIGO='461'">KARHG ISLAND</xsl:when>
		<xsl:when test="$CODIGO='462'">OTROS PUERTOS DE IRAN NO ESPEC</xsl:when>
		<xsl:when test="$CODIGO='471'">CALCUTA</xsl:when>
		<xsl:when test="$CODIGO='472'">OTROS PUERTOS DE INDIA NO E</xsl:when>
		<xsl:when test="$CODIGO='481'">CHALNA</xsl:when>
		<xsl:when test="$CODIGO='482'">OTROS PUERTOS DE BANGLADESH NO</xsl:when>
		<xsl:when test="$CODIGO='491'">OTROS PUERTOS DE SINGAPURE NO</xsl:when>
		<xsl:when test="$CODIGO='492'">HONG KONG</xsl:when>
		<xsl:when test="$CODIGO='499'">OTROS PUERTOS ASIATICOS NO ESP</xsl:when>
		<xsl:when test="$CODIGO='511'">CONSTANZA</xsl:when>
		<xsl:when test="$CODIGO='512'">OTROS PUERTOS DE RUMANIA NO ES</xsl:when>
		<xsl:when test="$CODIGO='521'">VARNA</xsl:when>
		<xsl:when test="$CODIGO='522'">OTROS PUERTOS DE BULGARIA NO E</xsl:when>
		<xsl:when test="$CODIGO='531'">RIJEKA</xsl:when>
		<xsl:when test="$CODIGO='532'">OTROS PUERTOS DE YUGOESLAVIA N</xsl:when>
		<xsl:when test="$CODIGO='533'">BELGRADO</xsl:when>
		<xsl:when test="$CODIGO='534'">OTROS PUERTOS DE SER</xsl:when>
		<xsl:when test="$CODIGO='535'">PODGORITSA</xsl:when>
		<xsl:when test="$CODIGO='536'">OTROS PUERTOS DE MON</xsl:when>
		<xsl:when test="$CODIGO='537'">OTROS PUERTOS DE CRO</xsl:when>
		<xsl:when test="$CODIGO='538'">RIJEKA</xsl:when>
		<xsl:when test="$CODIGO='541'">OTROS PUERTOS DE ITALIA NO ESP</xsl:when>
		<xsl:when test="$CODIGO='542'">GENOVA</xsl:when>
		<xsl:when test="$CODIGO='543'">LIORNA, LIVORNO</xsl:when>
		<xsl:when test="$CODIGO='544'">NAPOLES</xsl:when>
		<xsl:when test="$CODIGO='545'">SALERNO</xsl:when>
		<xsl:when test="$CODIGO='546'">AUGUSTA</xsl:when>
		<xsl:when test="$CODIGO='547'">SAVONA</xsl:when>
		<xsl:when test="$CODIGO='551'">OTROS PUERTOS DE FRANCIA NO ES</xsl:when>
		<xsl:when test="$CODIGO='552'">LA PALLICE</xsl:when>
		<xsl:when test="$CODIGO='553'">LE HAVRE</xsl:when>
		<xsl:when test="$CODIGO='554'">MARSELLA</xsl:when>
		<xsl:when test="$CODIGO='555'">BURDEOS</xsl:when>
		<xsl:when test="$CODIGO='556'">CALAIS</xsl:when>
		<xsl:when test="$CODIGO='557'">BREST</xsl:when>
		<xsl:when test="$CODIGO='558'">RUAN</xsl:when>
		<xsl:when test="$CODIGO='561'">OTROS PUERTOS DE ESPANA NO ESP</xsl:when>
		<xsl:when test="$CODIGO='562'">CADIZ</xsl:when>
		<xsl:when test="$CODIGO='563'">BARCELONA</xsl:when>
		<xsl:when test="$CODIGO='564'">BILBAO</xsl:when>
		<xsl:when test="$CODIGO='565'">HUELVA</xsl:when>
		<xsl:when test="$CODIGO='566'">SEVILLA</xsl:when>
		<xsl:when test="$CODIGO='567'">TARRAGONA</xsl:when>
		<xsl:when test="$CODIGO='571'">LIVERPOOL</xsl:when>
		<xsl:when test="$CODIGO='572'">LONDRES</xsl:when>
		<xsl:when test="$CODIGO='573'">ROCHESTER</xsl:when>
		<xsl:when test="$CODIGO='574'">ETEN SALVERRY</xsl:when>
		<xsl:when test="$CODIGO='576'">OTROS PUERTOS DE INGLATERRA NO</xsl:when>
		<xsl:when test="$CODIGO='577'">DOVER</xsl:when>
		<xsl:when test="$CODIGO='578'">PLYMOUTH</xsl:when>
		<xsl:when test="$CODIGO='581'">HELSINSKI</xsl:when>
		<xsl:when test="$CODIGO='582'">OTROS PUERTOS DE FINLANDIA NO</xsl:when>
		<xsl:when test="$CODIGO='583'">HANKO</xsl:when>
		<xsl:when test="$CODIGO='584'">KEMI</xsl:when>
		<xsl:when test="$CODIGO='585'">KOKKOLA</xsl:when>
		<xsl:when test="$CODIGO='586'">KOTKA</xsl:when>
		<xsl:when test="$CODIGO='587'">OULO</xsl:when>
		<xsl:when test="$CODIGO='588'">PIETARSAARI</xsl:when>
		<xsl:when test="$CODIGO='589'">PORI</xsl:when>
		<xsl:when test="$CODIGO='591'">BREMEN</xsl:when>
		<xsl:when test="$CODIGO='592'">HAMBURGO</xsl:when>
		<xsl:when test="$CODIGO='593'">NUREMBERG</xsl:when>
		<xsl:when test="$CODIGO='594'">FRANKFURT</xsl:when>
		<xsl:when test="$CODIGO='595'">DUSSELDORF</xsl:when>
		<xsl:when test="$CODIGO='596'">OTROS PUERTOS DE ALEMANIA NO E</xsl:when>
		<xsl:when test="$CODIGO='597'">CUXHAVEN</xsl:when>
		<xsl:when test="$CODIGO='598'">ROSTOCK</xsl:when>
		<xsl:when test="$CODIGO='599'">OLDENBURG</xsl:when>
		<xsl:when test="$CODIGO='601'">AMBERES</xsl:when>
		<xsl:when test="$CODIGO='602'">OTROS PUERTOS DE BELGICA NO ES</xsl:when>
		<xsl:when test="$CODIGO='603'">ZEEBRUGGE</xsl:when>
		<xsl:when test="$CODIGO='604'">GHENT</xsl:when>
		<xsl:when test="$CODIGO='605'">OOSTENDE</xsl:when>
		<xsl:when test="$CODIGO='611'">LISBOA</xsl:when>
		<xsl:when test="$CODIGO='612'">OTROS PUERTOS DE PORTUGAL NO E</xsl:when>
		<xsl:when test="$CODIGO='613'">SETUBAL</xsl:when>
		<xsl:when test="$CODIGO='621'">AMSTERDAM</xsl:when>
		<xsl:when test="$CODIGO='622'">ROTTERDAM</xsl:when>
		<xsl:when test="$CODIGO='623'">OTROS PUERTOS DE HOLANDA NO ES</xsl:when>
		<xsl:when test="$CODIGO='631'">GOTEMBURGO</xsl:when>
		<xsl:when test="$CODIGO='632'">OTROS PUERTOS DE SUECIA NO ESP</xsl:when>
		<xsl:when test="$CODIGO='633'">MALMO</xsl:when>
		<xsl:when test="$CODIGO='634'">HELSIMBORG</xsl:when>
		<xsl:when test="$CODIGO='635'">KALMAR</xsl:when>
		<xsl:when test="$CODIGO='641'">AARHUS</xsl:when>
		<xsl:when test="$CODIGO='642'">COPENHAGEN</xsl:when>
		<xsl:when test="$CODIGO='643'">OTROS PUERTOS DE DINAMARCA NO</xsl:when>
		<xsl:when test="$CODIGO='644'">AALBORG</xsl:when>
		<xsl:when test="$CODIGO='645'">ODENSE</xsl:when>
		<xsl:when test="$CODIGO='651'">OSLO</xsl:when>
		<xsl:when test="$CODIGO='652'">OTROS PUERTOS DE NORUEGA NO ES</xsl:when>
		<xsl:when test="$CODIGO='653'">STAVANGER</xsl:when>
		<xsl:when test="$CODIGO='699'">OTROS PUERTOS DE EUROPA NO ESP</xsl:when>
		<xsl:when test="$CODIGO='711'">DURBAM</xsl:when>
		<xsl:when test="$CODIGO='712'">CIUDAD DEL CABO</xsl:when>
		<xsl:when test="$CODIGO='713'">OTROS PUERTOS DE SUDAFRICA NO</xsl:when>
		<xsl:when test="$CODIGO='714'">SALDANHA</xsl:when>
		<xsl:when test="$CODIGO='715'">PORT-ELIZABETH</xsl:when>
		<xsl:when test="$CODIGO='716'">MOSSEL-BAY</xsl:when>
		<xsl:when test="$CODIGO='717'">EAST-LONDON</xsl:when>
		<xsl:when test="$CODIGO='799'">OTROS PUERTOS DE AFRICA NO ESP</xsl:when>
		<xsl:when test="$CODIGO='811'">SIDNEY</xsl:when>
		<xsl:when test="$CODIGO='812'">FREMANTLE</xsl:when>
		<xsl:when test="$CODIGO='813'">OTROS PUERTOS DE AUSTRALIA NO</xsl:when>
		<xsl:when test="$CODIGO='814'">ADELAIDA</xsl:when>
		<xsl:when test="$CODIGO='815'">DARWIN</xsl:when>
		<xsl:when test="$CODIGO='816'">GERALDTON</xsl:when>
		<xsl:when test="$CODIGO='899'">OTROS PUERTOS DE OCEANIA NO</xsl:when>
		<xsl:when test="$CODIGO='900'">RANCHO DE NAVES Y AERONAVES DE</xsl:when>
		<xsl:when test="$CODIGO='901'">ARICA</xsl:when>
		<xsl:when test="$CODIGO='902'">IQUIQUE</xsl:when>
		<xsl:when test="$CODIGO='903'">ANTOFAGASTA</xsl:when>
		<xsl:when test="$CODIGO='904'">COQUIMBO</xsl:when>
		<xsl:when test="$CODIGO='905'">VALPARAISO</xsl:when>
		<xsl:when test="$CODIGO='906'">SAN ANTONIO</xsl:when>
		<xsl:when test="$CODIGO='907'">TALCAHUANO</xsl:when>
		<xsl:when test="$CODIGO='908'">SAN VICENTE</xsl:when>
		<xsl:when test="$CODIGO='909'">LIRQUEN</xsl:when>
		<xsl:when test="$CODIGO='910'">PUERTO MONTT</xsl:when>
		<xsl:when test="$CODIGO='911'">CHACABUCO/PTO.AYSEN</xsl:when>
		<xsl:when test="$CODIGO='912'">PUNTA ARENAS</xsl:when>
		<xsl:when test="$CODIGO='913'">PATILLOS</xsl:when>
		<xsl:when test="$CODIGO='914'">TOCOPILLA</xsl:when>
		<xsl:when test="$CODIGO='915'">MEJILLONES</xsl:when>
		<xsl:when test="$CODIGO='916'">TALTAL</xsl:when>
		<xsl:when test="$CODIGO='917'">CHANARAL/BARQUITO</xsl:when>
		<xsl:when test="$CODIGO='918'">CALDERA</xsl:when>
		<xsl:when test="$CODIGO='919'">CALDERILLA</xsl:when>
		<xsl:when test="$CODIGO='920'">HUASCO/GUACOLDA</xsl:when>
		<xsl:when test="$CODIGO='921'">QUINTERO</xsl:when>
		<xsl:when test="$CODIGO='922'">JUAN FERNANDEZ</xsl:when>
		<xsl:when test="$CODIGO='923'">CONSTUTUCION</xsl:when>
		<xsl:when test="$CODIGO='924'">TOME</xsl:when>
		<xsl:when test="$CODIGO='925'">PENCO</xsl:when>
		<xsl:when test="$CODIGO='926'">CORONEL</xsl:when>
		<xsl:when test="$CODIGO='927'">LOTA</xsl:when>
		<xsl:when test="$CODIGO='928'">LEBU</xsl:when>
		<xsl:when test="$CODIGO='929'">ISLA DE PASCUA</xsl:when>
		<xsl:when test="$CODIGO='930'">CORRAL</xsl:when>
		<xsl:when test="$CODIGO='931'">ANCUD</xsl:when>
		<xsl:when test="$CODIGO='932'">CASTRO</xsl:when>
		<xsl:when test="$CODIGO='933'">QUELLON</xsl:when>
		<xsl:when test="$CODIGO='934'">CHAITEN</xsl:when>
		<xsl:when test="$CODIGO='935'">TORTEL</xsl:when>
		<xsl:when test="$CODIGO='936'">NATALES</xsl:when>
		<xsl:when test="$CODIGO='937'">GUARELLO</xsl:when>
		<xsl:when test="$CODIGO='938'">CUTTER COVE</xsl:when>
		<xsl:when test="$CODIGO='939'">PERCY</xsl:when>
		<xsl:when test="$CODIGO='940'">CLARENCIA</xsl:when>
		<xsl:when test="$CODIGO='941'">GREGORIO</xsl:when>
		<xsl:when test="$CODIGO='942'">CABO NEGRO</xsl:when>
		<xsl:when test="$CODIGO='943'">PUERTO WILLIAMS</xsl:when>
		<xsl:when test="$CODIGO='944'">TERRITORIO ANTARTICO CHILENO</xsl:when>
		<xsl:when test="$CODIGO='945'">SALINAS</xsl:when>
		<xsl:when test="$CODIGO='946'">GUAYACAN</xsl:when>
		<xsl:when test="$CODIGO='947'">PUNTA DELGADA</xsl:when>
		<xsl:when test="$CODIGO='948'">VENTANAS</xsl:when>
		<xsl:when test="$CODIGO='949'">PINO HACHADO(LIUCURA</xsl:when>
		<xsl:when test="$CODIGO='950'">CALETA COLOSO</xsl:when>
		<xsl:when test="$CODIGO='951'">AGUAS NEGRAS</xsl:when>
		<xsl:when test="$CODIGO='952'">ZONA FRANCA IQUIQUE</xsl:when>
		<xsl:when test="$CODIGO='953'">ZONA FRANCA PUNTA ARENAS</xsl:when>
		<xsl:when test="$CODIGO='954'">RIO MAYER</xsl:when>
		<xsl:when test="$CODIGO='955'">RIO MOSCO</xsl:when>
		<xsl:when test="$CODIGO='956'">VISVIRI</xsl:when>
		<xsl:when test="$CODIGO='957'">CHACALLUTA</xsl:when>
		<xsl:when test="$CODIGO='958'">CHUNGARA</xsl:when>
		<xsl:when test="$CODIGO='959'">COLCHANE</xsl:when>
		<xsl:when test="$CODIGO='960'">ABRA DE NAPA</xsl:when>
		<xsl:when test="$CODIGO='961'">OLLAGUE</xsl:when>
		<xsl:when test="$CODIGO='962'">SAN PEDRO DE ATACAMA</xsl:when>
		<xsl:when test="$CODIGO='963'">SOCOMPA</xsl:when>
		<xsl:when test="$CODIGO='964'">SAN FRANCISCO</xsl:when>
		<xsl:when test="$CODIGO='965'">LOS LIBERTADORES</xsl:when>
		<xsl:when test="$CODIGO='966'">MAHUIL MALAL</xsl:when>
		<xsl:when test="$CODIGO='967'">CARDENAL SAMORE</xsl:when>
		<xsl:when test="$CODIGO='968'">PEREZ ROSALES</xsl:when>
		<xsl:when test="$CODIGO='969'">FUTALEUFU</xsl:when>
		<xsl:when test="$CODIGO='970'">PALENA-CARRENLEUFU</xsl:when>
		<xsl:when test="$CODIGO='971'">PANGUIPULLI</xsl:when>
		<xsl:when test="$CODIGO='972'">HUAHUM</xsl:when>
		<xsl:when test="$CODIGO='973'">LAGO VERDE</xsl:when>
		<xsl:when test="$CODIGO='974'">APPELEG</xsl:when>
		<xsl:when test="$CODIGO='975'">PAMPA ALTA</xsl:when>
		<xsl:when test="$CODIGO='976'">HUEMULES</xsl:when>
		<xsl:when test="$CODIGO='977'">CHILE CHICO</xsl:when>
		<xsl:when test="$CODIGO='978'">BAKER</xsl:when>
		<xsl:when test="$CODIGO='979'">DOROTEA</xsl:when>
		<xsl:when test="$CODIGO='980'">CASAS VIEJAS</xsl:when>
		<xsl:when test="$CODIGO='981'">MONTE AYMOND</xsl:when>
		<xsl:when test="$CODIGO='982'">SAN SEBASTIAN</xsl:when>
		<xsl:when test="$CODIGO='983'">COYHAIQUE ALTO</xsl:when>
		<xsl:when test="$CODIGO='984'">TRIANA</xsl:when>
		<xsl:when test="$CODIGO='985'">IBANEZ PALAVICINI</xsl:when>
		<xsl:when test="$CODIGO='986'">VILLA OHIGGINS</xsl:when>
		<xsl:when test="$CODIGO='987'">AEROP.CHACALLUTA</xsl:when>
		<xsl:when test="$CODIGO='988'">AEROP.DIEGO ARACENA</xsl:when>
		<xsl:when test="$CODIGO='989'">AEROP.CERRO MORENO</xsl:when>
		<xsl:when test="$CODIGO='990'">AEROP.EL TEPUAL</xsl:when>
		<xsl:when test="$CODIGO='991'">AEROP.C.I.DEL CAMPO</xsl:when>
		<xsl:when test="$CODIGO='992'">AEROP.A.M.BENITEZ</xsl:when>
		<xsl:when test="$CODIGO='993'">CAP HUACHIPATO</xsl:when>
		<xsl:when test="$CODIGO='994'">ARICA-TACNA</xsl:when>
		<xsl:when test="$CODIGO='995'">ARICA-LA PAZ</xsl:when>
		<xsl:when test="$CODIGO='996'">TERM. PETROLERO ENAP</xsl:when>
		<xsl:when test="$CODIGO='997'">OTROS PTOS. CHILENOS</xsl:when>
		<xsl:when test="$CODIGO='998'">PASO JAMA</xsl:when>
	</xsl:choose>
	
  </xsl:template>
  
  <!-- MODALIDAD DE VENTA -->
  <xsl:template name="ClausulaVenta">
  	<xsl:param name="CODIGO"/>
	<xsl:choose>
		<xsl:when test="$CODIGO='1'">CIF</xsl:when>
		<xsl:when test="$CODIGO='10'">FCA</xsl:when>
		<xsl:when test="$CODIGO='11'">CPT</xsl:when>
		<xsl:when test="$CODIGO='12'">CIP</xsl:when>
		<xsl:when test="$CODIGO='17'">DAT</xsl:when>
		<xsl:when test="$CODIGO='18'">DAP</xsl:when>
		<xsl:when test="$CODIGO='2'">CFR</xsl:when>
		<xsl:when test="$CODIGO='3'">EXW</xsl:when>
		<xsl:when test="$CODIGO='4'">FAS</xsl:when>
		<xsl:when test="$CODIGO='5'">FOB</xsl:when>
		<xsl:when test="$CODIGO='6'">S/CL</xsl:when>
		<xsl:when test="$CODIGO='8'">OTROS</xsl:when>
		<xsl:when test="$CODIGO='9'">DDP</xsl:when>
	</xsl:choose>
  </xsl:template>
  
  <!-- MODALIDAD DE VENTA -->
  <xsl:template name="ModalidadVenta">
  	<xsl:param name="CODIGO"/>
	<xsl:choose>
		<xsl:when test="$CODIGO='1'">FIRME</xsl:when>
		<xsl:when test="$CODIGO='2'">BAJO CONDICION</xsl:when>
		<xsl:when test="$CODIGO='3'">EN CONSIGNACION LIBRE</xsl:when>
		<xsl:when test="$CODIGO='4'">EN CONSIGNACION CON UN MINIMO A FIRME</xsl:when>
		<xsl:when test="$CODIGO='9'">SIN PAGO</xsl:when>
	</xsl:choose>
  </xsl:template>
  
  <!-- MODALIDAD DE VENTA -->
  <xsl:template name="FormaPago">
  	<xsl:param name="CODIGO"/>
	<xsl:choose>
		<xsl:when test="$CODIGO='1'">CONTADO</xsl:when>
		<xsl:when test="$CODIGO='2'">CREDITO</xsl:when>
		<xsl:when test="$CODIGO='3'">SIN COSTO</xsl:when>
	</xsl:choose>
  </xsl:template>
  
  
  <!-- CODIGO PAÍSES -->
  <xsl:template name="Pais">
  	<xsl:param name="CODIGO"/>
	<xsl:choose>
		<xsl:when test="$CODIGO='101'">SENEGAL</xsl:when>
		<xsl:when test="$CODIGO='102'">GAMBIA</xsl:when>
		<xsl:when test="$CODIGO='103'">GUINEA-BISSAU</xsl:when>
		<xsl:when test="$CODIGO='104'">GUINEA</xsl:when>
		<xsl:when test="$CODIGO='105'">SIERRA LEONA</xsl:when>
		<xsl:when test="$CODIGO='106'">LIBERIA</xsl:when>
		<xsl:when test="$CODIGO='107'">COSTA DE MARFIL</xsl:when>
		<xsl:when test="$CODIGO='108'">GHANA</xsl:when>
		<xsl:when test="$CODIGO='109'">TOGO</xsl:when>
		<xsl:when test="$CODIGO='111'">NIGERIA</xsl:when>
		<xsl:when test="$CODIGO='112'">SUDAFRICA</xsl:when>
		<xsl:when test="$CODIGO='113'">BOTSWANA</xsl:when>
		<xsl:when test="$CODIGO='114'">LESOTHO</xsl:when>
		<xsl:when test="$CODIGO='115'">MALAWI</xsl:when>
		<xsl:when test="$CODIGO='116'">ZIMBABWE</xsl:when>
		<xsl:when test="$CODIGO='117'">ZAMBIA</xsl:when>
		<xsl:when test="$CODIGO='118'">COMORAS</xsl:when>
		<xsl:when test="$CODIGO='119'">MAURICIO</xsl:when>
		<xsl:when test="$CODIGO='120'">MADAGASCAR</xsl:when>
		<xsl:when test="$CODIGO='121'">MOZAMBIQUE</xsl:when>
		<xsl:when test="$CODIGO='122'">SWAZILANDIA</xsl:when>
		<xsl:when test="$CODIGO='123'">SUDAN</xsl:when>
		<xsl:when test="$CODIGO='124'">EGIPTO</xsl:when>
		<xsl:when test="$CODIGO='125'">LIBIA</xsl:when>
		<xsl:when test="$CODIGO='126'">TUNEZ</xsl:when>
		<xsl:when test="$CODIGO='127'">ARGELIA</xsl:when>
		<xsl:when test="$CODIGO='128'">MARRUECOS</xsl:when>
		<xsl:when test="$CODIGO='129'">CABO VERDE</xsl:when>
		<xsl:when test="$CODIGO='130'">CHAD</xsl:when>
		<xsl:when test="$CODIGO='131'">NIGER</xsl:when>
		<xsl:when test="$CODIGO='132'">ALTO VOLTA</xsl:when>
		<xsl:when test="$CODIGO='133'">MALI</xsl:when>
		<xsl:when test="$CODIGO='134'">MAURITANIA</xsl:when>
		<xsl:when test="$CODIGO='135'">TANZANIA</xsl:when>
		<xsl:when test="$CODIGO='136'">UGANDA</xsl:when>
		<xsl:when test="$CODIGO='137'">KENIA</xsl:when>
		<xsl:when test="$CODIGO='138'">SOMALIA</xsl:when>
		<xsl:when test="$CODIGO='139'">ETIOPIA</xsl:when>
		<xsl:when test="$CODIGO='140'">ANGOLA</xsl:when>
		<xsl:when test="$CODIGO='141'">BURUNDI</xsl:when>
		<xsl:when test="$CODIGO='142'">RWANDA</xsl:when>
		<xsl:when test="$CODIGO='143'">REP.DEM. CONGO</xsl:when>
		<xsl:when test="$CODIGO='144'">CONGO</xsl:when>
		<xsl:when test="$CODIGO='145'">GABON</xsl:when>
		<xsl:when test="$CODIGO='146'">S.TOM.PRINCIPE</xsl:when>
		<xsl:when test="$CODIGO='147'">GUINEA ECUATRL</xsl:when>
		<xsl:when test="$CODIGO='148'">REP.CENT.AFRIC.</xsl:when>
		<xsl:when test="$CODIGO='149'">CAMERUN</xsl:when>
		<xsl:when test="$CODIGO='150'">BENIN</xsl:when>
		<xsl:when test="$CODIGO='151'">TERR.BRIT.EN AF</xsl:when>
		<xsl:when test="$CODIGO='152'">TER.ESPAN.EN AF</xsl:when>
		<xsl:when test="$CODIGO='153'">TERR.FRAN.EN AF</xsl:when>
		<xsl:when test="$CODIGO='154'">BOPHUTHATSWANA</xsl:when>
		<xsl:when test="$CODIGO='155'">DJIBOUTI</xsl:when>
		<xsl:when test="$CODIGO='156'">SEYCHELLES</xsl:when>
		<xsl:when test="$CODIGO='158'">VIENDA</xsl:when>
		<xsl:when test="$CODIGO='159'">NAMIBIA</xsl:when>
		<xsl:when test="$CODIGO='160'">SUDAN DEL SUR</xsl:when>
		<xsl:when test="$CODIGO='161'">BURKINA FASO</xsl:when>
		<xsl:when test="$CODIGO='162'">CISKEY</xsl:when>
		<xsl:when test="$CODIGO='163'">ERITREA</xsl:when>
		<xsl:when test="$CODIGO='164'">ISLAS MARSHALL</xsl:when>
		<xsl:when test="$CODIGO='165'">SAHARAUI</xsl:when>
		<xsl:when test="$CODIGO='166'">TRANSKEI</xsl:when>
		<xsl:when test="$CODIGO='201'">VENEZUELA</xsl:when>
		<xsl:when test="$CODIGO='202'">COLOMBIA</xsl:when>
		<xsl:when test="$CODIGO='203'">TRINID.Y TOBAGO</xsl:when>
		<xsl:when test="$CODIGO='204'">BARBADOS</xsl:when>
		<xsl:when test="$CODIGO='205'">JAMAICA</xsl:when>
		<xsl:when test="$CODIGO='206'">REP.DOMINICANA</xsl:when>
		<xsl:when test="$CODIGO='207'">BAHAMAS</xsl:when>
		<xsl:when test="$CODIGO='208'">HAITI</xsl:when>
		<xsl:when test="$CODIGO='209'">CUBA</xsl:when>
		<xsl:when test="$CODIGO='210'">PANAMA</xsl:when>
		<xsl:when test="$CODIGO='211'">COSTA RICA</xsl:when>
		<xsl:when test="$CODIGO='212'">NICARAGUA</xsl:when>
		<xsl:when test="$CODIGO='213'">EL SALVADOR</xsl:when>
		<xsl:when test="$CODIGO='214'">HONDURAS</xsl:when>
		<xsl:when test="$CODIGO='215'">GUATEMALA</xsl:when>
		<xsl:when test="$CODIGO='216'">MEXICO</xsl:when>
		<xsl:when test="$CODIGO='217'">GUYANA</xsl:when>
		<xsl:when test="$CODIGO='218'">ECUADOR</xsl:when>
		<xsl:when test="$CODIGO='219'">PERU</xsl:when>
		<xsl:when test="$CODIGO='220'">BRASIL</xsl:when>
		<xsl:when test="$CODIGO='221'">BOLIVIA</xsl:when>
		<xsl:when test="$CODIGO='222'">PARAGUAY</xsl:when>
		<xsl:when test="$CODIGO='223'">URUGUAY</xsl:when>
		<xsl:when test="$CODIGO='224'">ARGENTINA</xsl:when>
		<xsl:when test="$CODIGO='225'">U.S.A.</xsl:when>
		<xsl:when test="$CODIGO='226'">CANADA</xsl:when>
		<xsl:when test="$CODIGO='227'">TERR.BRIT.EN AM</xsl:when>
		<xsl:when test="$CODIGO='228'">TERR.FRAN.EN AM</xsl:when>
		<xsl:when test="$CODIGO='229'">TER.HOLAN.EN AM</xsl:when>
		<xsl:when test="$CODIGO='230'">TERR.D/DINAMARC</xsl:when>
		<xsl:when test="$CODIGO='231'">DOMINICA</xsl:when>
		<xsl:when test="$CODIGO='232'">GRANADA</xsl:when>
		<xsl:when test="$CODIGO='233'">SANTA LUCIA(ISL</xsl:when>
		<xsl:when test="$CODIGO='234'">S.VTE.Y GRANAD.</xsl:when>
		<xsl:when test="$CODIGO='235'">SURINAM</xsl:when>
		<xsl:when test="$CODIGO='236'">BELICE</xsl:when>
		<xsl:when test="$CODIGO='240'">ANTIGUA Y BBUDA</xsl:when>
		<xsl:when test="$CODIGO='241'">SNT.KIT NEVIS</xsl:when>
		<xsl:when test="$CODIGO='242'">ANGUILA</xsl:when>
		<xsl:when test="$CODIGO='243'">ARUBA</xsl:when>
		<xsl:when test="$CODIGO='244'">BERMUDAS</xsl:when>
		<xsl:when test="$CODIGO='245'">ISLAS VIRG.BRIT</xsl:when>
		<xsl:when test="$CODIGO='246'">ISLAS CAYMAN</xsl:when>
		<xsl:when test="$CODIGO='247'">ANTILLAS NEERLANDESAS</xsl:when>
		<xsl:when test="$CODIGO='248'">TURCAS Y CAICOS</xsl:when>
		<xsl:when test="$CODIGO='249'">ISLAS VIRGENES (ESTADOS UNIDOS</xsl:when>
		<xsl:when test="$CODIGO='250'">MARTINICA</xsl:when>
		<xsl:when test="$CODIGO='251'">PUERTO RICO</xsl:when>
		<xsl:when test="$CODIGO='252'">MONSERRAT</xsl:when>
		<xsl:when test="$CODIGO='253'">GROENLANDIA</xsl:when>
		<xsl:when test="$CODIGO='301'">JORDANIA</xsl:when>
		<xsl:when test="$CODIGO='302'">ARABIA SAUDITA</xsl:when>
		<xsl:when test="$CODIGO='303'">KUWAIT</xsl:when>
		<xsl:when test="$CODIGO='304'">OMAN</xsl:when>
		<xsl:when test="$CODIGO='305'">CHIPRE</xsl:when>
		<xsl:when test="$CODIGO='306'">ISRAEL</xsl:when>
		<xsl:when test="$CODIGO='307'">IRAK</xsl:when>
		<xsl:when test="$CODIGO='308'">AFGANISTAN</xsl:when>
		<xsl:when test="$CODIGO='309'">IRAN</xsl:when>
		<xsl:when test="$CODIGO='310'">SIRIA</xsl:when>
		<xsl:when test="$CODIGO='311'">LIBANO</xsl:when>
		<xsl:when test="$CODIGO='312'">QATAR</xsl:when>
		<xsl:when test="$CODIGO='313'">BAHREIN</xsl:when>
		<xsl:when test="$CODIGO='314'">SRI LANKA</xsl:when>
		<xsl:when test="$CODIGO='315'">CAMBODIA</xsl:when>
		<xsl:when test="$CODIGO='316'">LAOS</xsl:when>
		<xsl:when test="$CODIGO='317'">INDIA</xsl:when>
		<xsl:when test="$CODIGO='318'">BHUTAN</xsl:when>
		<xsl:when test="$CODIGO='319'">THAILANDIA</xsl:when>
		<xsl:when test="$CODIGO='320'">NEPAL</xsl:when>
		<xsl:when test="$CODIGO='321'">BANGLADESH</xsl:when>
		<xsl:when test="$CODIGO='322'">YEMEN</xsl:when>
		<xsl:when test="$CODIGO='323'">YEMEN DEL SUR</xsl:when>
		<xsl:when test="$CODIGO='324'">PAKISTAN</xsl:when>
		<xsl:when test="$CODIGO='325'">VIETNAM</xsl:when>
		<xsl:when test="$CODIGO='326'">MYANMAR (EX BIR</xsl:when>
		<xsl:when test="$CODIGO='327'">ISLAS MALDIVAS</xsl:when>
		<xsl:when test="$CODIGO='328'">INDONESIA</xsl:when>
		<xsl:when test="$CODIGO='329'">MALASIA</xsl:when>
		<xsl:when test="$CODIGO='330'">TAIWAN (FORMOSA</xsl:when>
		<xsl:when test="$CODIGO='331'">JAPON</xsl:when>
		<xsl:when test="$CODIGO='332'">SINGAPUR</xsl:when>
		<xsl:when test="$CODIGO='333'">COREA DEL SUR</xsl:when>
		<xsl:when test="$CODIGO='334'">COREA DEL NORTE</xsl:when>
		<xsl:when test="$CODIGO='335'">FILIPINAS</xsl:when>
		<xsl:when test="$CODIGO='336'">CHINA</xsl:when>
		<xsl:when test="$CODIGO='337'">MONGOLIA</xsl:when>
		<xsl:when test="$CODIGO='341'">EMIR.ARAB.UNID.</xsl:when>
		<xsl:when test="$CODIGO='342'">HONG KONG</xsl:when>
		<xsl:when test="$CODIGO='343'">TER.PORTUG.E/AS</xsl:when>
		<xsl:when test="$CODIGO='344'">BRUNEI</xsl:when>
		<xsl:when test="$CODIGO='345'">MACAO</xsl:when>
		<xsl:when test="$CODIGO='346'">REPUBLICA DE YEMEN</xsl:when>
		<xsl:when test="$CODIGO='401'">FIJI</xsl:when>
		<xsl:when test="$CODIGO='402'">NAURU</xsl:when>
		<xsl:when test="$CODIGO='403'">ISLAS TONGA</xsl:when>
		<xsl:when test="$CODIGO='404'">SAMOA OCC.</xsl:when>
		<xsl:when test="$CODIGO='405'">NUEVA ZELANDIA</xsl:when>
		<xsl:when test="$CODIGO='406'">AUSTRALIA</xsl:when>
		<xsl:when test="$CODIGO='407'">TERR.BRIT.EN AU</xsl:when>
		<xsl:when test="$CODIGO='408'">TERR.FRAN.EN AU</xsl:when>
		<xsl:when test="$CODIGO='409'">T.NORTEAM.EN AU</xsl:when>
		<xsl:when test="$CODIGO='412'">PPUA.NVA.GUINEA</xsl:when>
		<xsl:when test="$CODIGO='415'">VANUATU</xsl:when>
		<xsl:when test="$CODIGO='416'">KIRIBATI</xsl:when>
		<xsl:when test="$CODIGO='417'">MICRONESIA</xsl:when>
		<xsl:when test="$CODIGO='418'">ISLAS SALOMON</xsl:when>
		<xsl:when test="$CODIGO='419'">TUVALU</xsl:when>
		<xsl:when test="$CODIGO='420'">PALAU</xsl:when>
		<xsl:when test="$CODIGO='421'">NIUE</xsl:when>
		<xsl:when test="$CODIGO='422'">POLINESIA FRANCESA</xsl:when>
		<xsl:when test="$CODIGO='423'">NUEVA CALEDONIA</xsl:when>
		<xsl:when test="$CODIGO='424'">ISLAS MARIANAS DEL NORTE</xsl:when>
		<xsl:when test="$CODIGO='425'">GUAM</xsl:when>
		<xsl:when test="$CODIGO='426'">TIMOR ORIENTAL</xsl:when>
		<xsl:when test="$CODIGO='427'">ISLAS COOK</xsl:when>
		<xsl:when test="$CODIGO='501'">PORTUGAL</xsl:when>
		<xsl:when test="$CODIGO='502'">ALEMANIA R.F.</xsl:when>
		<xsl:when test="$CODIGO='503'">ALEMANIA R.D.(N</xsl:when>
		<xsl:when test="$CODIGO='504'">ITALIA</xsl:when>
		<xsl:when test="$CODIGO='505'">FRANCIA</xsl:when>
		<xsl:when test="$CODIGO='506'">IRLANDA</xsl:when>
		<xsl:when test="$CODIGO='507'">DINAMARCA</xsl:when>
		<xsl:when test="$CODIGO='508'">SUIZA</xsl:when>
		<xsl:when test="$CODIGO='509'">AUSTRIA</xsl:when>
		<xsl:when test="$CODIGO='510'">REINO UNIDO</xsl:when>
		<xsl:when test="$CODIGO='511'">SUECIA</xsl:when>
		<xsl:when test="$CODIGO='512'">FINLANDIA</xsl:when>
		<xsl:when test="$CODIGO='513'">NORUEGA</xsl:when>
		<xsl:when test="$CODIGO='514'">BELGICA</xsl:when>
		<xsl:when test="$CODIGO='515'">HOLANDA</xsl:when>
		<xsl:when test="$CODIGO='516'">ISLANDIA</xsl:when>
		<xsl:when test="$CODIGO='517'">ESPANA</xsl:when>
		<xsl:when test="$CODIGO='518'">ALBANIA</xsl:when>
		<xsl:when test="$CODIGO='519'">RUMANIA</xsl:when>
		<xsl:when test="$CODIGO='520'">GRECIA</xsl:when>
		<xsl:when test="$CODIGO='521'">U.R.S.S.   (NO</xsl:when>
		<xsl:when test="$CODIGO='522'">TURQUIA</xsl:when>
		<xsl:when test="$CODIGO='523'">MALTA</xsl:when>
		<xsl:when test="$CODIGO='524'">SANTA SEDE</xsl:when>
		<xsl:when test="$CODIGO='525'">ANDORRA</xsl:when>
		<xsl:when test="$CODIGO='526'">YUGOESLAVIA (NO</xsl:when>
		<xsl:when test="$CODIGO='527'">BULGARIA</xsl:when>
		<xsl:when test="$CODIGO='528'">POLONIA</xsl:when>
		<xsl:when test="$CODIGO='529'">CHECOESLOVAQUIA</xsl:when>
		<xsl:when test="$CODIGO='530'">HUNGRIA</xsl:when>
		<xsl:when test="$CODIGO='532'">LUXEMBURGO</xsl:when>
		<xsl:when test="$CODIGO='534'">LIECHTENSTEIN</xsl:when>
		<xsl:when test="$CODIGO='535'">MONACO</xsl:when>
		<xsl:when test="$CODIGO='536'">SAN MARINO</xsl:when>
		<xsl:when test="$CODIGO='540'">ARMENIA</xsl:when>
		<xsl:when test="$CODIGO='541'">AZERBAIJAN</xsl:when>
		<xsl:when test="$CODIGO='542'">BELARUS</xsl:when>
		<xsl:when test="$CODIGO='543'">BOSNIA HEZGVINA</xsl:when>
		<xsl:when test="$CODIGO='544'">REPUBLICA CHECA</xsl:when>
		<xsl:when test="$CODIGO='545'">REP.ESLOVACA</xsl:when>
		<xsl:when test="$CODIGO='546'">REPUBLICA DE SERBIA</xsl:when>
		<xsl:when test="$CODIGO='547'">CROACIA</xsl:when>
		<xsl:when test="$CODIGO='548'">ESLOVENIA</xsl:when>
		<xsl:when test="$CODIGO='549'">ESTONIA</xsl:when>
		<xsl:when test="$CODIGO='550'">GEORGIA</xsl:when>
		<xsl:when test="$CODIGO='551'">KASAJSTAN</xsl:when>
		<xsl:when test="$CODIGO='552'">KIRGISTAN</xsl:when>
		<xsl:when test="$CODIGO='553'">LETONIA</xsl:when>
		<xsl:when test="$CODIGO='554'">LITUANIA</xsl:when>
		<xsl:when test="$CODIGO='555'">MACEDONIA</xsl:when>
		<xsl:when test="$CODIGO='556'">MOLDOVA</xsl:when>
		<xsl:when test="$CODIGO='557'">TADJIKISTAN</xsl:when>
		<xsl:when test="$CODIGO='558'">TURKMENISTAN</xsl:when>
		<xsl:when test="$CODIGO='559'">UCRANIA</xsl:when>
		<xsl:when test="$CODIGO='560'">UZBEKISTAN</xsl:when>
		<xsl:when test="$CODIGO='561'">MONTENEGRO</xsl:when>
		<xsl:when test="$CODIGO='562'">RUSIA</xsl:when>
		<xsl:when test="$CODIGO='563'">ALEMANIA</xsl:when>
		<xsl:when test="$CODIGO='564'">RF YUGOSLAVIA</xsl:when>
		<xsl:when test="$CODIGO='565'">GIBRALTAR</xsl:when>
		<xsl:when test="$CODIGO='566'">GUERNSEY</xsl:when>
		<xsl:when test="$CODIGO='567'">ISLA DE MAN</xsl:when>
		<xsl:when test="$CODIGO='568'">JERSEY</xsl:when>
		<xsl:when test="$CODIGO='585'">GILBRALTAR</xsl:when>
		<xsl:when test="$CODIGO='901'">COMB.Y LUBRIC.</xsl:when>
		<xsl:when test="$CODIGO='902'">RANCHO DE NAVES</xsl:when>
		<xsl:when test="$CODIGO='903'">PESCA EXTRA</xsl:when>
		<xsl:when test="$CODIGO='904'">ORIG.O DEST. NO</xsl:when>
		<xsl:when test="$CODIGO='905'">ZF.IQUIQUE</xsl:when>
		<xsl:when test="$CODIGO='906'">DEPOSITO FRANCO</xsl:when>
		<xsl:when test="$CODIGO='907'">ZF.PARENAS</xsl:when>
		<xsl:when test="$CODIGO='910'">ZF.ARICA-ZF IND</xsl:when>
		<xsl:when test="$CODIGO='997'">CHILE</xsl:when>
		<xsl:when test="$CODIGO='998'">NAC.REPUTADA</xsl:when>
		<xsl:when test="$CODIGO='999'"> OTROS</xsl:when>
	</xsl:choose>
	
  </xsl:template>
  
  
  <!-- CREA LA REPRESENTACION DEL DOCUMENTO -->
  <xsl:template name="crearRepresentacion">
    
    <!-- CREE EL MARCO DE TRABAJO -->
    <html>
      <header>
        <title>Representacion documento DTE</title>
        
        <style>
          .marco
          {
          width:25cm;
          height:20cm;
          border:0px solid #cdcdcd;
          margin-top:10px;

          }

          td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          }


          .DetalleDocumento
          {
          width : 100%;
          height: 10cm;
          border: 2px solid black;
          }

          .DetalleDocumento th
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          }

          .DetalleDocumento td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          }

          .ReferenciasDocumento{
          width:99%;
          border-top:2px solid black;
          border-right:2px solid black;
          border-left:2px solid black;
          border-bottom:1px solid BLACK;
          }

          .ReferenciasDocumento th
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          height:23px;
          }

          .ReferenciasDocumento td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          height:23px;

          }


          .TotalesDocumento
          {
          width:100%;
          border:2px solid black;
          }

          .TotalesDocumento td
          {
          padding:2px 4px 2px 4px;
          font-family:verdana;
          font-size:11px;
          height:23px;
          }

          .FooterDocumento
          {
          width:100%;
          border:0px solid black;
          margin:5px 0 0 0;
          }

          .FooterDocumento td
          {
          padding:4px 4px 4px 4px;
          }

          .PalabrasDocumento
          {
          width:100%;
          border:2px solid #cdcdcd;
          margin:5px 0 0 0;
          }

          .PalabrasDocumento td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          padding:3px 3px 3px 3px;

          }

          .DetalleRowBottom
          {
          border-bottom:1px solid White;
          }

          .DetalleRowRight
          {
          border-right:1px solid White;
          }

          .DetalleRowBottomHeader
          {
          border-bottom:1px solid black;
          }

          .DetalleRowRightHeader
          {
          border-right:1px solid Black;
          }


          .HeaderDocumento
          {
          width:100%
          }

          .HeaderDocumento td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          }



          .ReceptorDocumento
          {
          width:100%;
          border:1px solid black;
          }

          .ReceptorDocumento td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;
          height:23px;
          }

          .TransporteDocumento
          {
          width:100%;
          border:2px solid black;
          }

          .TransporteDocumento td
          {
          vertical-align:top;
          font-family:verdana;
          font-size:11px;

          }

          .CuadroRojo
          {
          font-family:verdana;
          font-size:16px;
          font-weight:bold;
          text-align:center;
          color:red;
          padding:3px;
          width:8cm;
          height:2.5cm;
          text-align:center;
          border:3px solid red;
          margin-left:5px;
          }


          .HeaderEncabezado
          {
          font-family:verdana;
          font-size:12px;
          font-weight:bold;
          padding-top:8px;
          padding-left:3px;
          padding-bottom:3px;

          }

          .CeldaCantidad
          {
          text-align:right;
          padding:0 2px 0 2px;
          }

          .CeldaTexto
          {
          text-align:left;
          padding:0 2px 0 2px;
          }

          .TotaleraDocumento
          {
          margin:10px 0 0 0;
          width:100%;

          }

          .TimbreElectronico
          {
          padding:0 0 0 0;
          text-align:right;
          }

          .TimbreElectronico img
          {
          /*
          width:9cm;
          height:3cm;*/
          }

          .TextoRecibo
          {

          font-family:verdana;
          font-size:8px;
          font-weight:bold;


          }

          .TextoNotaRecibo
          {

          font-family:verdana;
          font-size:7px;
          font-weight:bold;


          }

          /* ESTILO TEXTO RAZON SOCIAL EMPRESA EMISORA */
          .TextoGrande {
          font-family :arial;
          font-size   : 12.5pt;
          font-weight :bold;
          }

          /* ESTILO TEXTO GIRO EMPRESA */
          .TextoGiro {
          font-family :Verdana;
          font-size   :7.5pt;

          }

          /* ============================================================= */
          /* NUEVOS ESTILOS                                                */
          /* ============================================================= */

          /* ESTILO DE TABLA */
          .Tabla
          {
          width:100%;
          border:2px solid black;
          padding-bottom:10px;

          }

          /* ESTILO DE TD TABLA */
          .Tabla td
          {
          font-family: verdana;
          font-size:10pt;
          text-align:left;
          padding:3px 5px 3px 5px;
          background:#FFFFFF;

          }


          /* TITULO DE TABLAS */
          .TablaTitulo
          {
          background:#D8D8D8;
          font-family: verdana;
          font-size:10pt;
          text-align:left;
          padding:3px 5px 3px 5px;
          border-bottom:2px solid black;
          }

          /* TITULO DE TABLAS */
          .TablaDetalle
          {
          background:#D8D8D8;
          font-family: verdana;
          font-size:10pt;
          text-align:left;
          padding:3px 5px 3px 5px;

          }

          .RedondearLogo
          {
          border-radius: 15px 15px 15px 15px;
          border:2px solid black;
          padding:2px;
          }

          .RedondearSuperior
          {
          border-radius: 15px 15px 15px 15px;
          border:2px solid black;
          }


          .RedondearSuperiorParametros
          {
          border-radius: 15px 15px 0px 0px;
          }

          .RedondearInferiorParametros
          {
          border-radius: 0px 0px 15px 15px ;
          }

        </style>
        
      </header>
      <body style="padding-top:20px">
        <table class="marco" align="center">
          <tr>
            <td>
			  
              <xsl:call-template name="HeaderDocumento"/>
			  <br/>
              <xsl:call-template name="ReceptorDocumento"/>
			  <xsl:call-template name="DatosExportacion"/>
			  
			  <br/>
              
              <xsl:call-template name="DetalleDocumento"/>
              <xsl:call-template name="TotaleraDocumento"/>
			  
			  <br/>
			  <br/>
              <xsl:call-template name="FooterDocumento"/>
			  <xsl:call-template name="MensajeCedible"/>
            </td>
          </tr>
        </table>
      </body>
    </html>
    
  </xsl:template>

  <!-- CUADRO ROJO -->
  <xsl:template name="CuadroRojo">
    <div class="CuadroRojo">

      <!-- De formato al rut-->
      <xsl:variable name="RUT">
        <xsl:call-template name="FormatoRut">
          <xsl:with-param name="Rut" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:RUTEmisor"/>
        </xsl:call-template>
      </xsl:variable>
      
      <!-- Calcule el nombre del documento -->
      <xsl:variable name="NombreDte">
        <xsl:call-template name="NombreDocumento">
          <xsl:with-param name="tip" select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE"/>
        </xsl:call-template>
      </xsl:variable>

      <!-- Cual es el folio del documento -->
      <xsl:variable name="Folio">
        <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:Folio"/>
      </xsl:variable>
      
      <!-- SALIDA -->
      <div>
        R.U.T.:<xsl:value-of select="$RUT"/>
        <div style="padding-top:3px;">
          <xsl:value-of select="$NombreDte"/>
        </div>
        <div style="padding-top:3px;">N°<xsl:value-of select="$Folio"/></div>
      </div>



    </div>
    <div style="text-align:center;color:red;font-weight:bold;padding:5px 0 0 0 "><xsl:value-of select="$SIIDireccionRegional"/></div>
  </xsl:template>
  
  <!-- HEADER DEL DOCUMENTO -->
  <xsl:template name="HeaderDocumento">
   
    <table class="HeaderDocumento" border="0px">
      <colgroup>
        <col style="width:auto"/>
        <col style="width:200px"/>
      </colgroup>
      <tr>
        <td>

          <table border="0px" style="width:100%" cellpadding="0" cellspacing="0">
            <colgroup>
              <col style="width:1%"/>
              <col style="width:auto"/>
            </colgroup>
            <tr>
              <td>
                <!-- LOGO DEL EMISOR DEL DOCUMENTO -->
                <!--<img src="IMAGES/Default2.jpg" alt="logo cliente" style="width:110px;height:120px;padding-right:5px"/>-->
              </td>
              <td>
                <!-- DATOS DEL CONTRIBUYENTE -->
                <div class="TextoGrande" style="padding-left:15px">
                  <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:RznSoc"/>
                  <br/>
                  <div class="TextoGiro">
                    <B>
                      DISENO Y VENTA DE MOBILIARIO
                    </B><br/>
                    VENTA AL POR MAYOR DE MUEBLES, EXCEPTO MUEBLES DE OFICINA<br/>
                    VENTA AL POR MENOR DE MUEBLES Y COLCHONES EN COMERCIOS ESPECIALIZADOS<br/>
                    ACTIVIDADES DE DISEÑO Y DECORACION DE INTERIORES

                  </div>
                  <br/>
				          <div style="font-weight:normal;font-size:12px">
                          <b>Dirección:</b>&#160;
				                  <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:DirOrigen"/><br/>
				                  <b>Comuna:</b>&#160;
				                  <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:CmnaOrigen"/><BR/>
                          <b>Ciudad:</b>&#160;
                          <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:CiudadOrigen"/>
                  </div>
                  <br/>
                  
				  
                </div>
              </td>
            </tr>
            <tr>
              <td colspan="2">
                <b>Sucursales</b><br/>
               
              </td>
            </tr>
          </table>
          
        </td>
        <td>
          <xsl:call-template name="CuadroRojo"/>
        </td>
      </tr>
      <tr>
        <td colspan="2">
          <div style="font-weight:normal;font-size:12px">
           
          </div>
        </td>
      </tr>
    </table>
    
    
  </xsl:template>

  <!-- DATOS DEL RECEPTOR -->
  <xsl:template name="ReceptorDocumento">
  
      <!-- AGREGAR FECHA FORMATEADA -->
	  <xsl:call-template name="FechaFormateado"/>
  
      <table border="0px" cellpadding="0" cellspacing="0" class="Tabla RedondearSuperior">
      <colgroup>
        <col style="width:200px;" />
        <col style="width:3px;"/>
        <col style="width:auto"/>
        <col style="width:160px"/>
        <col style="width:3px;"/>
        <col style="width:140px"/>
      </colgroup>  
		<tr >
			<th colspan="6" class="TablaTitulo RedondearSuperiorParametros">&#160;Receptor</th>
		</tr>
		<tr>
          <td style="font-weight:bold;color:#848484">Señor(es)</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Receptor/sii:RznSocRecep"/>
          </td>
          <td style="font-weight:bold;color:#848484">RUT</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Receptor/sii:RUTRecep"/>
          </td>
        </tr>
		<tr>
          <td style="font-weight:bold;color:#848484">Giro</td>
          <td style="font-weight:bold;">:</td>
          <td>
		  DESARROLLO DE SOFTWARE
            <!--<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Receptor/sii:GiroRecep"/>-->
          </td>
		  <td style="font-weight:bold;color:#848484">Teléfono</td>
          <td style="font-weight:bold;">:</td>
          <td>
            62242459
          </td>
        </tr>
		<tr>
          <td style="font-weight:bold;color:#848484">Dirección</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Receptor/sii:DirRecep"/>
          </td>
          <td style="font-weight:bold;color:#848484">Comuna</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Receptor/sii:CmnaRecep"/>
          </td>
        </tr>
		    <tr>
          <td style="font-weight:bold;color:#848484">Vendedor</td>
          <td style="font-weight:bold;">:</td>
          <td>
		  MARCELO ROJAS ROJAS
            <!--<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Emisor/sii:CdgVendedor"/>-->
          </td>
          <td style="font-weight:bold;color:#848484">Ciudad</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Receptor/sii:CiudadRecep"/>
          </td>
        </tr>
		<tr>
			<td style="font-weight:bold;color:#848484">Modalidad de venta</td>
          <td style="font-weight:bold;">:</td>
          <td>
			<xsl:call-template name="ModalidadVenta">
				<xsl:with-param name="CODIGO">
					<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:CodModVenta"/>	
				</xsl:with-param>
			</xsl:call-template>
		  </td>
			<td style="font-weight:bold;color:#848484">Tipo de moneda</td>
			<td style="font-weight:bold;">:</td>
			<td>
				<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:TpoMoneda"/>
			</td>
		</tr>
        
        <!-- GUIAS DE DESPACHO -->
        <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE = '52'">
        <tr>
          <td style="font-weight:bold;color:#848484">Ind.Traslado</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:choose>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '1'">Operación constituye venta</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '2'">Ventas por efectuar</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '3'">Consignaciones</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '4'">Entrega gratuita</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '5'">Traslados internos</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '6'">Otros traslados no venta</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '7'">Guía de devolución</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '8'">Traslado para exportación. (no venta)</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndTraslado = '9'">Venta para exportación</xsl:when>
            </xsl:choose>
          </td>
          <td style="font-weight:bold;color:#848484">Tipo.Desp.</td>
          <td style="font-weight:bold;">:</td>
          <td>
            <xsl:choose>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDespacho = '1'">Despacho por cuenta del receptor</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDespacho = '2'">Despacho por cuenta del emisor</xsl:when>
              <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDespacho = '3'">Despacho por cuenta del emisor a otras instalaciones</xsl:when>
            </xsl:choose>
          </td>
        </tr>
        </xsl:if>
        
        
    </table>
  </xsl:template>

  <!-- DATOS DE LA EXPORTACION -->
  <xsl:template name="DatosExportacion">
  <br/>
  <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE = '110'">
  <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndServicio != '4' or not(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:IndServicio)">
  <table border="0px" cellpadding="0" cellspacing="0" class="Tabla RedondearSuperior">
      <colgroup>
        <col style="width:200px;" />
        <col style="width:3px;"/>
        <col style="width:300px"/>
        <col style="width:150px"/>
        <col style="width:3px;"/>
        <col style="width:250px"/>
      </colgroup>
    <tr>
      <td colspan="6" class="RedondearSuperiorParametros" style="height:10px"></td>
      
    </tr>
		
		<tr>
          <td style="font-weight:bold;color:#848484">Puerto Embarque</td>
          <td style="font-weight:bold;">:</td>
          <td>
			<xsl:call-template name="Puertos">
				<xsl:with-param name="CODIGO">
					<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:CodPtoEmbarque"/>	
				</xsl:with-param>
			</xsl:call-template>
		  </td>
         <td style="font-weight:bold;color:#848484">Pais Receptor</td>
          <td style="font-weight:bold;">:</td>
          <td>
		    <xsl:call-template name="Pais">
				<xsl:with-param name="CODIGO">
					<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:CodPaisRecep"/>	
				</xsl:with-param>
			</xsl:call-template>
		  </td>
        </tr>
		<tr>
          <td style="font-weight:bold;color:#848484">Puerto Desembarque</td>
          <td style="font-weight:bold;">:</td>
          <td>
			<xsl:call-template name="Puertos">
				<xsl:with-param name="CODIGO">
					<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:CodPtoDesemb"/>	
				</xsl:with-param>
			</xsl:call-template>
		  </td>
          <td style="font-weight:bold;color:#848484">Clausula de venta</td>
          <td style="font-weight:bold;">:</td>
          <td>
			<xsl:call-template name="ClausulaVenta">
				<xsl:with-param name="CODIGO">
					<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:CodClauVenta"/>	
				</xsl:with-param>
			</xsl:call-template>
		  </td>
        </tr>
		<tr>
		  <xsl:choose>
			<xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:TotBultos">
				<td style="font-weight:bold;color:#848484">Total bultos</td>
				<td style="font-weight:bold;">:</td>
				<td><xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Transporte/sii:Aduana/sii:TotBultos"/></td>
			</xsl:when>
			<xsl:otherwise>
				<td>&#160;</td>
				<td>&#160;</td>
				<td>&#160;</td>
			</xsl:otherwise>
		  </xsl:choose>
		
          
		  
		  
		  
		  
          <td style="font-weight:bold;color:#848484">Forma de pago</td>
          <td style="font-weight:bold;">:</td>
          <td>
			<xsl:call-template name="FormaPago">
				<xsl:with-param name="CODIGO">
					<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:FmaPago"/>	
				</xsl:with-param>
			</xsl:call-template>
		  </td>
        </tr>
		
		
    </table>
  </xsl:if>
  </xsl:if>
  </xsl:template>

  <!-- DATOS DEL DETALLE DEL DOCUMENTO -->
  <xsl:template name="DetalleDocumento">
    	
    <!-- TABLA DE DETALLE DEL DOCUMENTO -->	
    <table border="0px" cellpadding="0" cellspacing="0" class="Tabla RedondearSuperior" height="300px">
        <colgroup>
			
				<col style="width:30px;"/>
				<col style="width:80px;"/>
				<col style="width:auto"/>
				<col style="width:80px"/>
				<col style="width:80px;"/>
				<col style="width:80px;"/>
				<col style="width:80px;"/>
				<col style="width:80px;"/>
			
		</colgroup>
		<tr style="height:22px;">
			<th colspan="8" class="TablaTitulo RedondearSuperiorParametros">&#160;Detalle</th>
		</tr>
		<tr style="height:22px;">
			<th  class="TablaTitulo">Nro</th>
			<th  class="TablaTitulo">Código</th>
			<th  class="TablaTitulo">Detalle</th>
			<th  class="TablaTitulo">Cantidad</th>
			<th  class="TablaTitulo">Unidad</th>
			<th  class="TablaTitulo">Precio</th>
			<th  class="TablaTitulo">Descto</th>
			<th  class="TablaTitulo">Valor</th>
        </tr>

		<!-- ESPACIO INTERMEDIO -->
		<tr style="height:5px;">
			<td colspan="8"></td>
		</tr>
		
		<!-- MUESTRE AQUI EL DETALLE DEL DOCUMENTO -->
		<xsl:for-each select="sii:DTE/sii:Exportaciones/sii:Detalle">
			<tr style="height:22px;">
				<td ><xsl:value-of select="position()"/></td>
				<td class="TablaDetalle">
				<xsl:choose>
				  <xsl:when test="sii:CdgItem/sii:VlrCodigo">
					<xsl:value-of select="sii:CdgItem/sii:VlrCodigo"/>    
				  </xsl:when>
				  <xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
				</td>
				<td class="TablaDetalle">
				<xsl:choose>
				  <xsl:when test="sii:NmbItem">
					<xsl:value-of select="sii:NmbItem"/>
				  </xsl:when>
				  <xsl:otherwise>&#160;</xsl:otherwise>
				</xsl:choose>
				
				</td>
				<td class="TablaDetalle" style="text-align:right">
				<xsl:choose>
				  <xsl:when test="sii:QtyItem">
					<xsl:value-of select="format-number(sii:QtyItem,'###.###.##0,##','moneda')"/>	
				  </xsl:when>
				  <xsl:otherwise>0</xsl:otherwise>
				</xsl:choose>
				</td>
				<td class="TablaDetalle">
				 <xsl:choose>
				  <xsl:when test="sii:UnmdItem">
					<xsl:value-of select="sii:UnmdItem"/>
				  </xsl:when>
				  <xsl:otherwise>
					&#160;
				  </xsl:otherwise>
				</xsl:choose>
				</td>
				<td class="TablaDetalle" style="text-align:right">
				<xsl:choose>
				  <xsl:when test="sii:PrcItem">
					<xsl:value-of select="format-number(sii:PrcItem,'###.###.##0,##','moneda')"/>
				  </xsl:when>
				  <xsl:otherwise>
					<xsl:value-of select="format-number('0','###.###.##0,##','moneda')"/>
				  </xsl:otherwise>
				</xsl:choose>
				</td>
				<td class="TablaDetalle" style="text-align:right">
				<xsl:choose>
				  <xsl:when test="sii:DescuentoMonto">
					<xsl:value-of select="format-number(sii:DescuentoMonto,'###.###.##0,##','moneda')"/>
				  </xsl:when>
				  <xsl:otherwise>
					<xsl:value-of select="format-number('0','###.###.##0,##','moneda')"/>
				  </xsl:otherwise>
				</xsl:choose>				
				</td>
				<td class="TablaDetalle" style="text-align:right">
					<xsl:value-of select="format-number(sii:MontoItem,'###.###.##0,##','moneda')"/>
				</td>
			</tr>
			
			<!-- COMO LLAMAR A FUNCION PARA SEPARAR DESCRIPCION ADICIONAL -->
			<xsl:if test="sii:DscItem">
				<td>&#160;</td>
			    <td>&#160;</td>
				<td>
					<xsl:call-template name="tokenize">
						<xsl:with-param name="text" select="sii:DscItem"/>
					</xsl:call-template>
				</td>
				<td>&#160;</td>
				<td>&#160;</td>
				<td>&#160;</td>
				<td>&#160;</td>
			</xsl:if>
			
			
			
		</xsl:for-each> 
		
		<!-- separacion -->
		<tr>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
		</tr>
		
		<!-- MUESTRE LOS RECARGOS Y DESCUENTOS GLOBALES -->
		<xsl:for-each select="sii:DTE/sii:Exportaciones/sii:DscRcgGlobal">
		<tr style="height:22px;">
			<td colspan="3">&#160;</td>
			<td colspan="4" style="text-align:right" ><b><xsl:value-of select="sii:GlosaDR"/></b></td>
			<td style="text-align:right"><xsl:value-of select="sii:ValorDR"/></td>
		</tr>
		</xsl:for-each>
		
		<!-- COMPLETE LA TABLA -->
		<tr>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
			<td>&#160;</td>
		</tr>
		
		
		
	</table>
	   
  </xsl:template>

  <!-- DATOS DE LA TOTALERA -->
  <xsl:template name="TotaleraDocumento">
    <table class="TotaleraDocumento" border="0px" cellpadding="0" cellspacing="0">
      <colgroup>
        <col style="width:65%;"/>
        <col style="width:35%"/>
      </colgroup>
      <tr>
        <td>
          <xsl:call-template name="ReferenciasDocumento"/>
        </td>
        <td>
          <xsl:call-template name="TotalesDocumento"/>
        </td>
      </tr>
    </table>
    
  </xsl:template>
  
  <!-- DATOS DE REFERENCIA DEL DOCUMENTO -->
  <xsl:template name="ReferenciasDocumento">
  
    <table class="Tabla RedondearSuperior" border="0px" cellpadding="0" cellspacing="0" style="width:99%">
      <colgroup>
        <col style="width:30%;"/>
        <col style="width:15%"/>
        <col style="width:15%"/>
        <col style="width:auto"/>
      </colgroup>
	  
	  <tr>
        <th class="TablaTitulo RedondearSuperiorParametros" colspan="4">&#160;&#160;Referencias a otros documentos.</th>
      </tr>
	  
      <tr>
        <th class="TablaTitulo">Doc.Ref</th>
        <th class="TablaTitulo">Folio</th>
        <th class="TablaTitulo">Fecha</th>
        <th class="TablaTitulo">Razón Ref</th>
      </tr>
      <xsl:for-each select="sii:DTE/sii:Exportaciones/sii:Referencia">
      <tr>
        <td class="TablaDetalle" style="text-align:center">
          <xsl:call-template name="NombreDocumento">
            <xsl:with-param name="tip" select="sii:TpoDocRef"/>
          </xsl:call-template>
        </td>
        <td class="TablaDetalle" style="padding-right:5px;">
          <xsl:value-of select="sii:FolioRef"/>
        </td>
        <td class="TablaDetalle" style="text-align:center">
          <xsl:value-of select="sii:FchRef"/>
        </td>
        <td class="TablaDetalle" style="padding-left:3px;padding-right:3px;">
          <xsl:value-of select="sii:RazonRef"/>
        </td>
      </tr>
      </xsl:for-each>
      <xsl:if test="count(sii:DTE/sii:Exportaciones/sii:Referencia)=0">
      <tr>
        <td class=" DetalleRowRight">&#160;</td>
        <td class=" DetalleRowRight">&#160;</td>
        <td class=" DetalleRowRight">&#160;</td>
        <td>&#160;</td>
      </tr>
      </xsl:if>
    </table>
  
    

  </xsl:template>

  <!-- DATOS DE REFERENCIA DEL DOCUMENTO -->
  <xsl:template name="TotalesDocumento">
  
	<!-- SI EL DOCUMENTO ACTUAL ESS UNA FACTURA DE COMPRA -->
	<xsl:choose>
	  <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE = 46">

  			<table class="TotalesDocumento" border="0px" cellpadding="0" cellspacing="0">
			<colgroup>
			<col style="width:50%;"/>
			<col style="width:2%"/>
			<col style="width:auto"/>
			</colgroup>
			
			
			<!-- MONTO EXENTO  -->
      
			  <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntExe>0">
			
			  <tr>
			  <td style="font-weight:bold">Monto Exento $</td>
			  <td>:</td>
			  <td class="CeldaCantidad">
			  <xsl:value-of select="format-number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntExe,'###.###.##0,##','moneda')"/>
			  </td>
			  </tr>
			
			  </xsl:if>
      
          
          
			
			<!-- MONTO NETO -->
			<xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntNeto>0">
			
			<tr>
			<td style="font-weight:bold">Neto $</td>
			<td>:</td>
			<td class="CeldaCantidad">
			<xsl:value-of select="format-number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntNeto,'###.###.##0,##','moneda')"/>
			</td>
			</tr>
			
			</xsl:if>
			
			<!-- IVA DEL DOCUMENTO -->
			<xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:IVA>0">
			
			<tr>
			<td style="font-weight:bold">I.V.A. (19%)</td>
			<td>:</td>
			<td class="CeldaCantidad">
			<xsl:value-of select="format-number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:IVA,'###.###.##0,##','moneda')"/>
			</td>
			</tr>
			
			</xsl:if>
			
			<!-- MONTO TOTAL -->
					
			<tr>
			<td style="font-weight:bold">Total $</td>
			<td>:</td>
			<td class="CeldaCantidad">
			<xsl:value-of select="format-number(number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntNeto)+number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:IVA) + number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntExe),'###.###.##0,##','moneda')"/>
			</td>
			</tr>
			
			<!-- IVA RETENIDO -->
			<xsl:variable name="IVARet">
				<xsl:value-of select="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:ImptoReten/sii:MontoImp"/>
			</xsl:variable>
			
			<!-- IVA RETENIDO -->
			<xsl:if test="$IVARet>0">
			
			<tr>
			<td style="font-weight:bold">IVA Retenido $</td>
			<td>:</td>
			<td class="CeldaCantidad">
			<xsl:value-of select="format-number($IVARet,'###.###.##0,##','moneda')"/> 
			</td>
			</tr>
			
			</xsl:if>
			
			
			<!-- TOTAL A PAGAR -->
			<xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntExe = 0">
			
			
			<tr>
			<td style="font-weight:bold">Total a Pagar $</td>
			<td>:</td>
			<td class="CeldaCantidad">
			<xsl:value-of select="format-number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntTotal,'###.###.##0,##','moneda')"/>
			</td>
			</tr>
			
			</xsl:if>
			
			</table>
	  
	  </xsl:when>
	  <xsl:otherwise>
		
		<table class="TotalesDocumento RedondearSuperior" border="0px" cellpadding="0" cellspacing="0">
			<colgroup>
			<col style="width:50%;"/>
			<col style="width:2%"/>
			<col style="width:auto"/>
			</colgroup>
      <tr>
        <td colspan="3" class="RedondearSuperiorParametros" style="height:10px"></td>

      </tr>
			<xsl:choose>
			  <xsl:when test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntExe">
				<tr>
				  <td style="font-weight:bold;font-size:12px">&#160;Exento $</td>
				  <td>:</td>
				  <td class="CeldaCantidad"  style="font-size:12px;padding-right:10px;">
					<xsl:value-of select="format-number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntExe,'###.###.##0,##','moneda')"/>
				  </td>
				</tr>
			  </xsl:when>
			  <xsl:otherwise>
				<tr>
				  <td style="font-weight:bold;font-size:12px">&#160;Exento $</td>
				  <td>:</td>
				  <td class="CeldaCantidad" style="font-size:12px;">0</td>
				</tr>
			  </xsl:otherwise>
			</xsl:choose>  
			
			<tr>
			<td style="font-weight:bold;font-size:12px;">&#160;Total $</td>
			<td>:</td>
			<td class="CeldaCantidad" style="font-size:12px;padding-right:10px;">
				<xsl:value-of select="format-number(sii:DTE/sii:Exportaciones/sii:Encabezado/sii:Totales/sii:MntTotal,'###.###.##0,##','moneda')"/>
			</td>
			</tr>
		</table>
		
	  </xsl:otherwise>
	</xsl:choose>
	    
  </xsl:template>

  <!-- REPRESENTA EL TOTAL EN PALABRAS -->
  <xsl:template name="PalabrasDocumento">
    <table class="PalabrasDocumento" border="0px" cellpadding="0" cellspacing="0">
      <tr>
        <td>Total:</td>
      </tr>
    </table>
  </xsl:template>

  <!-- TIMBRE DEL DOCUMENTO ACTUAL -->
  <xsl:template name="TimbreElectronico">

    <xsl:variable name="uriPdf417Image">
      <xsl:call-template name="uriPdf417"/>
    </xsl:variable>

    <div class="TimbreElectronico">
      <img src="{$uriPdf417Image}" alt="{$uriPdf417Image}"/>
      <div style="text-align:center;padding-top:3px;">&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;Timbre Electrónico SII</div>
      <div style="text-align:center;padding-top:0px;">&#160;&#160;&#160;&#160;&#160;&#160;Res. 0 de 2013 Verifique documento: wwww.sii.cl</div>
    </div>
  </xsl:template>

  <!-- ACUSERECIBO -->
  <xsl:template name="AcuseRecibo">
    
  <!-- Solo Aplique cuando el documento sea cedible-->
    <xsl:if test="$esCedible='True'">
      <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=61">
        <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=56">
          <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=52">
			<xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=111">
				<xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=112">
        <div>
          <table width="50%" height="70" border="0" align="right" cellpadding="0" cellspacing="0" style="border:1px solid black">
            <tr>
              <td width="20%" height="12" valign="middle" class="TextoRecibo">Nombre</td>
              <td width="80%" valign="middle" align="left" class="TextoRecibo">_________________________________________________</td>
            </tr>
            <tr>
              <td height="12" valign="middle" class="TextoRecibo">R.U.T.</td>
              <td valign="middle" align="right" class="TextoRecibo">_________________________________________________</td>
            </tr>
            <tr>
              <td height="12" valign="middle" class="TextoRecibo">Fecha</td>
              <td valign="middle" align="right" class="TextoRecibo">_________________________________________________</td>
            </tr>
            <tr>
              <td height="12" valign="middle" class="TextoRecibo">Recinto</td>
              <td valign="middle" align="right" class="TextoRecibo">_________________________________________________</td>
            </tr>
            <tr>
              <td height="12" valign="middle" class="TextoRecibo">Firma</td>
              <td valign="middle" align="right" class="TextoRecibo">_________________________________________________</td>
            </tr>
            <tr>
              <td height="15" width="100%" colspan="2" align="justify" valign="bottom">
                <table width="100%" border="0" align="right" cellpadding="0" cellspacing="0">
                  <tr>
                    <td class="TextoNotaRecibo" style="padding:0 0 0 0">
                      El acuse de recibo que se declara en este acto, de acuerdo a lo dispuesto en la letra b) del Art. 4°, y la letra c) del Art. 5° de la Ley 19.983, acredita que la entrega de mercaderías o servicio (s) prestado (s) ha (n) sido recibido (s).
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>    
          </div>
		  </xsl:if>
		  </xsl:if>
          </xsl:if>
        </xsl:if>
      </xsl:if>
    </xsl:if>
  </xsl:template>

  <!-- MENSAJE CEDIBLE ( SOLO SI CORRESPONDE )-->
  <xsl:template name="MensajeCedible">
    <div style="text-align:right;padding:px 3px 0 0;">
      <xsl:if test="$esCedible='True'">
		      <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=61">
			      <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=56">
				  <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=111">
				  <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE!=112">
				      CEDIBLE
			      </xsl:if>
				  </xsl:if>
				  </xsl:if>
		      </xsl:if>
          <xsl:if test="sii:DTE/sii:Exportaciones/sii:Encabezado/sii:IdDoc/sii:TipoDTE=52">
            CON SU FACTURA
          </xsl:if>
      </xsl:if>  
    </div>
  </xsl:template>
  
  <!-- REPRESENTA EL FOOTER DEL DOCUMENTO -->
  <xsl:template name="FooterDocumento">
    <table class="FooterDocumento" border="0px" cellpadding="0" cellspacing="0">
      <colgroup>
        <col style="width:50%"/>
        <col style="width:50%"/>
      </colgroup>
      <tr>
        <td style="padding-right:120px">
          <xsl:call-template name="TimbreElectronico"/>
        </td>
        <td>
          <xsl:call-template name="AcuseRecibo"/>
        </td>
      </tr>
    </table>
  </xsl:template>
 
 
 <!-- SEPARA DESCRIPCION ADICIONAL 1152 1175-->
 
 <!-- DEFINA EL RETORNO DE CARRO -->
 <xsl:variable name="_crlf"><xsl:text>|</xsl:text></xsl:variable>
 
 
 <xsl:template match="string/text()" name="tokenize">
    <xsl:param name="text" select="."/>
    <xsl:param name="sep" select="$_crlf"/>
    <xsl:choose>
        <xsl:when test="not(contains($text, $sep))">
            <div> 
                <xsl:value-of select="translate( normalize-space($text),'@', '&#160;' )"/>
            </div>
			
        </xsl:when>
        <xsl:otherwise>
			<div>
                <xsl:value-of select="normalize-space(substring-before($text, $sep))"/>
				<!-- <xsl:value-of select="normalize-space(substring-before($text, $sep))"/> -->
            </div>
			
            <xsl:call-template name="tokenize">
                <xsl:with-param name="text" select="substring-after(  translate($text,'@', '&#160;'  ), $sep )"/>
            </xsl:call-template>
			
        </xsl:otherwise>
    </xsl:choose>
</xsl:template>

</xsl:stylesheet>