using System;
using System.Xml;
using System.Windows.Forms;

namespace folDigLib
{
    public class Timbrar
    {
        public clsDatosDeRespuestaDelTimbradoFD Documento(clsDatosParaElTimbrarFD datos)
        {
            clsDatosDeRespuestaDelTimbradoFD respuesta = new clsDatosDeRespuestaDelTimbradoFD();
            string tipo = (datos.tipoTimbrado ?? "").Trim();
            if (tipo.Equals("Pruebas", StringComparison.OrdinalIgnoreCase) || tipo.Equals("Prueba", StringComparison.OrdinalIgnoreCase))
            {
                respuesta = timbrarPruebas(datos);
            }
            else
            {
                respuesta = timbrarProductivo(datos);
            }
            return respuesta;
        }

        private WSCFDI.WSCFDI33Client CrearClienteProductivo()
        {
            try
            {
                return new WSCFDI.WSCFDI33Client("soapHttpEndpointHttps");
            }
            catch
            {
                try
                {
                    return new WSCFDI.WSCFDI33Client("soapHttpEndpoint1");
                }
                catch
                {
                    try
                    {
                        return new WSCFDI.WSCFDI33Client();
                    }
                    catch
                    {
                        var binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.Transport);
                        binding.MaxReceivedMessageSize = 2147483647;
                        binding.MaxBufferSize = 2147483647;
                        binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                        var address = new System.ServiceModel.EndpointAddress("https://www.foliosdigitalespac.com/WSTimbrado33/WSCFDI33.svc");
                        return new WSCFDI.WSCFDI33Client(binding, address);
                    }
                }
            }
        }

        private WSPruebas.WSCFDI33Client CrearClientePruebas()
        {
            try
            {
                return new WSPruebas.WSCFDI33Client("soapHttpEndpoint");
            }
            catch
            {
                try
                {
                    return new WSPruebas.WSCFDI33Client();
                }
                catch
                {
                    var binding = new System.ServiceModel.BasicHttpBinding();
                    binding.MaxReceivedMessageSize = 2147483647;
                    binding.MaxBufferSize = 2147483647;
                    binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                    var address = new System.ServiceModel.EndpointAddress("http://app.foliosdigitalespac.com/WSTimbrado33Test/WSCFDI33.svc");
                    return new WSPruebas.WSCFDI33Client(binding, address);
                }
            }
        }

        //------------------------------------------------------------------------
        //  Proceso para timbrar pruebas
        //------------------------------------------------------------------------
        public clsDatosDeRespuestaDelTimbradoFD timbrarPruebas(clsDatosParaElTimbrarFD datos)
        {
            clsDatosDeRespuestaDelTimbradoFD _respuesta = new clsDatosDeRespuestaDelTimbradoFD();
            string xmlSalida = datos.xmlSalida;
            string txtSalida = datos.txtSalida;

            string txtUsuario = datos.usuario;
            string txtPassw = datos.passw;
            string txtReferencia = datos.referencia;
            try
            {
                WSPruebas.WSCFDI33Client ws = CrearClientePruebas();
                WSPruebas.RespuestaTFD33 respuesta = new WSPruebas.RespuestaTFD33();

                XmlDocument doc = new XmlDocument();
                XmlDocument RespuetaXML = new XmlDocument();

                doc.Load(xmlSalida);
                string strXML = doc.OuterXml;

                respuesta = ws.TimbrarCFDI(txtUsuario, txtPassw, strXML, txtReferencia);

                if (respuesta.XMLResultado != "" && respuesta.XMLResultado != null)
                {
                    RespuetaXML.LoadXml(respuesta.XMLResultado);
                    RespuetaXML.Save(xmlSalida);
                }

                _respuesta.archivo = datos.rutaXML + xmlSalida;
                _respuesta.codigoConfirmacion = respuesta.CodigoConfirmacion;
                _respuesta.codigoRespuesta = respuesta.CodigoRespuesta;
                _respuesta.mensajeError = respuesta.MensajeError;
                _respuesta.mensajeErrorDetallado = respuesta.MensajeErrorDetallado;
                _respuesta.operacionExitosa = respuesta.OperacionExitosa.ToString();
                _respuesta.xmlResultado = respuesta.XMLResultado;
                _respuesta.pdfResultado = respuesta.PDFResultado;

                clsTimbrar timbrar = new clsTimbrar();
                timbrar.GuardarArchivoSalida(_respuesta, txtSalida);
            }
            catch (Exception ex)
            {
                _respuesta.archivo = datos.rutaXML + xmlSalida;
                _respuesta.codigoConfirmacion = "Error Interno";
                _respuesta.codigoRespuesta = "Error";
                _respuesta.mensajeError = "Error al procesar timbrado";
                _respuesta.mensajeErrorDetallado = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                _respuesta.operacionExitosa = "False";
                _respuesta.xmlResultado = "";
                _respuesta.pdfResultado = "";
                clsTimbrar timbrar = new clsTimbrar();
                timbrar.GuardarArchivoSalida(_respuesta, txtSalida);
            }

            return _respuesta;
        }

        //------------------------------------------------------------------------
        //  Proceso para timbrar productivo
        //------------------------------------------------------------------------
        public clsDatosDeRespuestaDelTimbradoFD timbrarProductivo(clsDatosParaElTimbrarFD datos)
        {
            clsDatosDeRespuestaDelTimbradoFD _respuesta = new clsDatosDeRespuestaDelTimbradoFD();

            string xmlSalida = datos.rutaXML + datos.archivoXML;
            string txtSalida = datos.rutaXML + "Historico\\" + datos.archivoTXT;

            string txtUsuario = datos.usuario;
            string txtPassw = datos.passw;
            string txtReferencia = datos.referencia;

            try
            {
                WSCFDI.WSCFDI33Client ws = CrearClienteProductivo();
                WSCFDI.RespuestaTFD33 respuesta = new WSCFDI.RespuestaTFD33();

                XmlDocument doc = new XmlDocument();
                XmlDocument RespuetaXML = new XmlDocument();

                doc.Load(xmlSalida);
                string strXML = doc.OuterXml;

                respuesta = ws.TimbrarCFDI(txtUsuario, txtPassw, strXML, txtReferencia);

                if (respuesta.XMLResultado != "" && respuesta.XMLResultado != null)
                {
                    RespuetaXML.LoadXml(respuesta.XMLResultado);
                    RespuetaXML.Save(xmlSalida);
                }

                _respuesta.archivo = datos.rutaXML + xmlSalida;
                _respuesta.codigoConfirmacion = respuesta.CodigoConfirmacion;
                _respuesta.codigoRespuesta = respuesta.CodigoRespuesta;
                _respuesta.mensajeError = respuesta.MensajeError;
                _respuesta.mensajeErrorDetallado = respuesta.MensajeErrorDetallado;
                _respuesta.operacionExitosa = respuesta.OperacionExitosa.ToString();
                _respuesta.xmlResultado = respuesta.XMLResultado;
                _respuesta.pdfResultado = respuesta.PDFResultado;
                clsTimbrar timbrar = new clsTimbrar();
                timbrar.GuardarArchivoSalida(_respuesta, txtSalida);
            }
            catch (Exception ex)
            {
                _respuesta.archivo = datos.rutaXML + xmlSalida;
                _respuesta.codigoConfirmacion = "Error Interno";
                _respuesta.codigoRespuesta = "Error";
                _respuesta.mensajeError = "Error al procesar timbrado";
                _respuesta.mensajeErrorDetallado = "ERROR: " + (ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                _respuesta.operacionExitosa = "False";
                _respuesta.xmlResultado = "";
                _respuesta.pdfResultado = "";
                clsTimbrar timbrar = new clsTimbrar();
                timbrar.GuardarArchivoSalida(_respuesta, txtSalida);
            }

            return _respuesta;
        }
    }
}