using System;
using System.Collections.Generic;
using System.IO;


namespace folDigLib
{
    public class clsTimbrar
    {
        public void GuardarArchivoSalida(clsDatosDeRespuestaDelTimbradoFD datos, string txtSalida)
        {
            StreamWriter sw = new StreamWriter(txtSalida);
            try
            {
                if (datos.mensajeErrorDetallado == "" && datos.mensajeError != "")
                {
                    datos.mensajeErrorDetallado = datos.mensajeError;
                }
                sw.WriteLine("Archivo: " + datos.archivo);
                sw.WriteLine("Codigo confirmacion: " + datos.codigoConfirmacion);
                sw.WriteLine("CodigoRespuesta: " + datos.codigoRespuesta);
                sw.WriteLine("MensajeError: " + datos.mensajeError);
                sw.WriteLine("MensajeErrorDetallado: " + datos.mensajeErrorDetallado);
                sw.WriteLine("OperacionExitosa: " + datos.operacionExitosa);
                sw.WriteLine("XMLResultado: " + datos.xmlResultado);
                sw.WriteLine("PDFResultado: " + datos.pdfResultado);
                sw.Close();

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void GuardarArchivoSalidaCancelacion(clsDatosDeRespuestaDelaCancelacionFD datos, string txtSalida)
        {
            StreamWriter sw = new StreamWriter(txtSalida);
            try
            {
                if (datos.mensajeDetallado == "" && datos.mensajeError != "")
                {
                    datos.mensajeDetallado = datos.mensajeError;
                }
                sw.WriteLine("Codigo Resultado: " + datos.codigoResultado);
                sw.WriteLine("Es Cancelable: " + datos.esCancelable);
                sw.WriteLine("Mensaje Resultado: " + datos.mensajeResultado);
                sw.WriteLine("OperacionExitosa: " + datos.mensajeError);
                sw.WriteLine("Mensaje de Error: " + datos.mensajeDetallado);
                sw.WriteLine("Mensaje Detallado: " + datos.operacionExitosa);
                sw.WriteLine("XMLResultado: " + datos.xmlResultado);
                sw.Close();

            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
    public class clsDatosParaElTimbrarFD
    {
        public string usuario { get; set; }
        public string passw { get; set; }
        public string referencia { get; set; }
        public string rutaXML { get; set; }
        public string archivoXML { get; set; }
        public string archivoTXT { get; set; }
        public string tipoTimbrado { get; set; }
        public string xmlSalida { get; set; }
        public string txtSalida { get; set; }
        public string archivoXSLT { get; set; }
        public string uuid { get; set; }

    }
    public class clsDatosParaCancelarFD
    {
        public string usuario { get; set; }
        public string passw { get; set; }
        public string rfcEmisor { get; set; }
        public string clavePrivadaBase64 { get; set; }
        public string passwClavePrivadaBase64 { get; set; }
        public string folioSustitucion { get; set; }
        public string motivo { get; set; }
        public string rfcReceptor { get; set; }
        public decimal? total { get; set; }
        public string uuidDocumento { get; set; }
        public string rutaXML { get; set; }
        public string tipoTimbrado { get; set; }
        public string xmlSalida { get; set; }
        public string txtSalida { get; set; }

    }
    public class clsDatosDeRespuestaDelTimbradoFD
    {
        public string archivo { get; set; }
        public string codigoConfirmacion { get; set; }
        public string codigoRespuesta { get; set; }
        public string mensajeError { get; set; }
        public string mensajeErrorDetallado { get; set; }
        public string operacionExitosa { get; set; }
        public string xmlResultado { get; set; }
        public string pdfResultado { get; set; }
    }
    public class clsDatosDeRespuestaDelaCancelacionFD
    {
        public string codigoResultado { get; set; }
        public string esCancelable { get; set; }
        public string mensajeResultado { get; set; }
        public string operacionExitosa { get; set; }
        public string mensajeError { get; set; }
        public string mensajeDetallado { get; set; }
        public string xmlResultado { get; set; }
    }
    public class respuesta
    {
        public bool resultado { get; set; }
        public string archivoXML { get; set; }
        public string xml { get; set; }
        public string cadenaOriginal { get; set; }
        public string sello { get; set; }
        public string error { get; set; }
        public string errorDetalle { get; set; }

    }
    public class Datoscomprobante
    {
        public string version { get; set; }
        public string serie { get; set; }
        public string folio { get; set; }
        public string fecha { get; set; }
        public string formaPago { get; set; }
        public string noCertificado { get; set; }
        public string certificado { get; set; }
        public string condicionesDePago { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public Nullable<decimal> descuento { get; set; }
        public string moneda { get; set; }
        public Nullable<decimal> tipoCambio { get; set; }
        public Nullable<decimal> total { get; set; }
        public string tipoDeComprobante { get; set; }
        public string tipoDeComprobanteDesc { get; set; }
        public string exportacion { get; set; }
        public string metodoPago { get; set; }
        public string lugarExpedicion { get; set; }
        public string confirmacion { get; set; }
        public string Estatus { get; set; }
        public string timbreUUID { get; set; }
        public string ordenCompra { get; set; }
        public string emisorRFC { get; set; }
        public string emisorNombre { get; set; }
        public string emisorRegimenFiscal { get; set; }
        public string emisorRegimenFiscalDes { get; set; }
        public string factAtrAdquirente { get; set; }
        public string emisorCodigoPostal { get; set; }
        public string emisorCIF { get; set; }
        public string receptorRfc { get; set; }
        public string receptorNombre { get; set; }
        public string receptorDomicilioFiscal { get; set; }
        public string receptorRegimenFiscal { get; set; }
        public string receptorRegimenFiscalDes { get; set; }
        public string usoCFDI { get; set; }
        public Nullable<int> idCliente { get; set; }
        public string residenciaFsical { get; set; }
        public string numRegIdTrib { get; set; }
        public string DomicilioReceptor { get; set; }
        public string receptorCIF { get; set; }
        public string fechaHoraTimbrado { get; set; }
        public string numeroCertificadoSat { get; set; }
        public string selloDigital { get; set; }
        public string sellloSAT { get; set; }
        public string cadenaOriginal { get; set; }
        public string rutaXML { get; set; }
        public string rutaCBB { get; set; }
        public string referencia { get; set; }
        public Nullable<int> idReporte { get; set; }
        public string imagenReporte { get; set; }
        public string usuarioPAC { get; set; }
        public string passwordPAC { get; set; }
        public string servidorSMTP { get; set; }
        public string puertoSMTP { get; set; }
        public string servidorReqAut { get; set; }
        public string usuariosSMTP { get; set; }
        public string passwordSMTP { get; set; }
        public string dirCorreEnviaOmision { get; set; }
        public string privadaPem { get; set; }
        public string tipotimbrado { get; set; }
        public List<DatosConcpetos> conceptos { get; set; }
        public List<DatosImpuestos> impuestos { get; set; }
    }
    public class DatosEmisor
    {
        public string rfc { get; set; }
        public string nombre { get; set; }
        public string regimenFiscal { get; set; }
    }
    public class DatosReceptor
    {
        public string rfc { get; set; }
        public string nombre { get; set; }
        public string domiciliofiscalreceptor { get; set; }
        public string regimenfiscalreceptor { get; set; }
        public string usoCFDI { get; set; }
        public int numeroCliente { get; set; }
    }
    public class DatosConcpetos
    {
        public string ClaveUnidad { get; set; }
        public string ClaveProdServ { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; }
        public string NoIdentificacion { get; set; }
        public string Descripcion { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Importe { get; set; }
        public string ObjetoImp { get; set; }
        public int idOrden { get; set; }
        public int idProducto { get; set; }
        public List<DatosImpuestos> impuestos { get; set; }
    }
    public class DatosImpuestos
    {
        public decimal Base { get; set; }
        public string Impuesto { get; set; }
        public string TipoFactor { get; set; }
        public decimal TasaOCuota { get; set; }
        public decimal Importe { get; set; }
        public int idOrden { get; set; }
        public int idProducto { get; set; }

    }
    public class DatosTimbrado
    {
        public bool resultado { get; set; }
        public string Version { get; set; }
        public string RfcProvCertif { get; set; }
        public string UUID { get; set; }
        public string FechaTimbrado { get; set; }
        public string SelloCFD { get; set; }
        public string NoCertificadoSAT { get; set; }
        public string SelloSAT { get; set; }
        public string xmlSAT { get; set; }
    }
    public partial class spSoaWeb_CFDI40_DatosGenerales_Result
    {
        public string documentoCFDI { get; set; }
        public string version { get; set; }
        public string serie { get; set; }
        public string folio { get; set; }
        public string fecha { get; set; }
        public string formaPago { get; set; }
        public string formaPagoDesc { get; set; }
        public string noCertificado { get; set; }
        public string certificado { get; set; }
        public string condicionesDePago { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public Nullable<decimal> descuento { get; set; }
        public string moneda { get; set; }
        public Nullable<decimal> tipoCambio { get; set; }
        public Nullable<decimal> total { get; set; }
        public string tipoDeComprobante { get; set; }
        public string tipoDeComprobanteDesc { get; set; }
        public string exportacion { get; set; }
        public string exportacionDesc { get; set; }
        public string metodoPago { get; set; }
        public string metodoPagoDesc { get; set; }
        public string lugarExpedicion { get; set; }
        public string confirmacion { get; set; }
        public string confirmacionDesc { get; set; }
        public string Estatus { get; set; }
        public string timbreUUID { get; set; }
        public string ordenCompra { get; set; }
        public string emisorRFC { get; set; }
        public string emisorNombre { get; set; }
        public string emisorRegimenFiscal { get; set; }
        public string emisorRegimenFiscalDes { get; set; }
        public string factAtrAdquirente { get; set; }
        public string emisorCodigoPostal { get; set; }
        public string emisorCIF { get; set; }
        public string emisorRegimenCapital { get; set; }
        public string emisorRegimenCapitalDesc { get; set; }
        public string receptorRfc { get; set; }
        public string receptorNombre { get; set; }
        public string receptorDomicilioFiscal { get; set; }
        public string receptorRegimenFiscal { get; set; }
        public string receptorRegimenFiscalDes { get; set; }
        public string usoCFDI { get; set; }
        public string usoCFDIDesc { get; set; }
        public Nullable<int> idCliente { get; set; }
        public string residenciaFsical { get; set; }
        public string numRegIdTrib { get; set; }
        public string DomicilioReceptor { get; set; }
        public string receptorCIF { get; set; }
        public string receptorRegimenCapital { get; set; }
        public string receptorRegimenCapitalDesc { get; set; }
        public string fechaHoraTimbrado { get; set; }
        public string numeroCertificadoSat { get; set; }
        public string selloDigital { get; set; }
        public string sellloSAT { get; set; }
        public string cadenaOriginal { get; set; }
        public string rutaXML { get; set; }
        public string rutaPDF { get; set; }
        public string rutaCBB { get; set; }
        public string rutaArchivoCBB { get; set; }
        public string referencia { get; set; }
        public Nullable<int> idReporte { get; set; }
        public string imagenReporte { get; set; }
        public string usuarioPAC { get; set; }
        public string passwordPAC { get; set; }
        public string servidorSMTP { get; set; }
        public string puertoSMTP { get; set; }
        public string servidorReqAut { get; set; }
        public string usuariosSMTP { get; set; }
        public string passwordSMTP { get; set; }
        public string dirCorreEnviaOmision { get; set; }
        public string privadaPem { get; set; }
        public string tipotimbrado { get; set; }
    }
    public partial class spSoaWeb_CFDI40_Conceptos_Result
    {
        public string serie { get; set; }
        public int folio { get; set; }
        public int orden { get; set; }
        public int producto { get; set; }
        public int idTipo { get; set; }
        public string strTipo { get; set; }
        public string ClaveFiscal { get; set; }
        public string noIdentificacion { get; set; }
        public decimal cantidad { get; set; }
        public string claveUnidad { get; set; }
        public string unidad { get; set; }
        public string descripcion { get; set; }
        public decimal valorUnitario { get; set; }
        public decimal ImporteConcepto { get; set; }
        public string objetoImp { get; set; }
        public Nullable<decimal> importeBase { get; set; }
        public string impuesto { get; set; }
        public string tipoFactor { get; set; }
        public Nullable<decimal> tasaOCuota { get; set; }
        public decimal importeImpuesto { get; set; }
    }
    public partial class spSoaWeb_CFDI40_Impuestos_Result
    {
        public string serie { get; set; }
        public int folio { get; set; }
        public Nullable<decimal> importeBase { get; set; }
        public string impuesto { get; set; }
        public string tipoFactor { get; set; }
        public Nullable<decimal> tasaOCuota { get; set; }
        public decimal importeImpuesto { get; set; }
    }
    public partial class spSoaWeb_CFDI40_DatosRealacionados_Result
    {
        public string serie { get; set; }
        public int folio { get; set; }
        public string serieRelacion { get; set; }
        public int folioRelacion { get; set; }
        public string tipoMovimientoRelacion { get; set; }
        public string tipoRelacion { get; set; }
        public string tipoRelacionDesc { get; set; }
        public string TimbreUUIDRelacion { get; set; }
    }


}
