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
            
            switch (datos.tipoTimbrado)
            {
                case "Pruebas":
                    respuesta = timbrarPruebas(datos);
                    break;
                case "Produccion":
                    respuesta = timbrarProductivo(datos);
                    break;
            }
            return respuesta;
        }

        //------------------------------------------------------------------------
        //  Proceso para timbrar pruebas
        //------------------------------------------------------------------------
        public clsDatosDeRespuestaDelTimbradoFD timbrarPruebas(clsDatosParaElTimbrarFD datos)
        {
            clsDatosDeRespuestaDelTimbradoFD _respuesta = new clsDatosDeRespuestaDelTimbradoFD();
            //Datos para los Archivos de salida
            string xmlSalida = datos.xmlSalida;// datos.rutaXML + datos.archivoXML;
            string txtSalida = datos.txtSalida;// datos.rutaXML + "Historico\\" + datos.archivoTXT;

            //Datos para el timbrado
            string txtUsuario = datos.usuario;
            string txtPassw = datos.passw;
            string txtReferencia = datos.referencia;
            try
            {
                //Instancias de Pruebas
                WSPruebas.WSCFDI33Client ws = new WSPruebas.WSCFDI33Client();
                WSPruebas.RespuestaTFD33 respuesta = new WSPruebas.RespuestaTFD33();

                //Carga el xml a timbrar
                XmlDocument doc = new XmlDocument();
                XmlDocument RespuetaXML = new XmlDocument();

                doc.Load(xmlSalida);
                string strXML = doc.OuterXml;

                //Timbra el documento en Folios digitales
                respuesta = ws.TimbrarCFDI(txtUsuario, txtPassw, strXML, txtReferencia);

                if (respuesta.XMLResultado != "" && respuesta.XMLResultado != null)
                {
                    RespuetaXML.LoadXml(respuesta.XMLResultado);
                    RespuetaXML.Save(xmlSalida);
                }

                //Guarda los datos de respuesta del timbrado en un archivo fisico
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
                //Guarda los datos de respuesta en case de haber un error
                _respuesta.archivo = datos.rutaXML + xmlSalida;
                _respuesta.codigoConfirmacion = "Error Intero";
                _respuesta.codigoRespuesta = "Error";
                _respuesta.mensajeError = "Error al procesar timbrado";
                _respuesta.mensajeErrorDetallado = "ERROR: " + ex.InnerException.Message;
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

            //Datos para los Archivos de salida
            string xmlSalida = datos.rutaXML + datos.archivoXML;
            string txtSalida = datos.rutaXML + "Historico\\" + datos.archivoTXT;

            //Datos para el timbrado
            string txtUsuario = datos.usuario;
            string txtPassw = datos.passw;
            string txtReferencia = datos.referencia;

            try
            {
                //Instancias de productivo
                WSCFDI.WSCFDI33Client ws = new WSCFDI.WSCFDI33Client();
                WSCFDI.RespuestaTFD33 respuesta = new WSCFDI.RespuestaTFD33();

                //Carga el xml a timbrar
                XmlDocument doc = new XmlDocument();
                XmlDocument RespuetaXML = new XmlDocument();

                doc.Load(xmlSalida);
                string strXML = doc.OuterXml;

                //Timbra el documento en Folios digitales
                respuesta = ws.TimbrarCFDI(txtUsuario, txtPassw, strXML, txtReferencia);

                if (respuesta.XMLResultado != "" && respuesta.XMLResultado != null)
                {
                    RespuetaXML.LoadXml(respuesta.XMLResultado);
                    RespuetaXML.Save(xmlSalida);
                }

                //Guarda los datos de respuesta del timbrado en un archivo fisico
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
                //Guarda los datos de respuesta en case de haber un error
                _respuesta.archivo = datos.rutaXML + xmlSalida;
                _respuesta.codigoConfirmacion = "Error Interno";
                _respuesta.codigoRespuesta = "Error";
                _respuesta.mensajeError = "Error al procesar timbrado";
                _respuesta.mensajeErrorDetallado = "ERROR: " + ex.InnerException.Message;
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
