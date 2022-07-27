using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;


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
}
