using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows.Forms;

namespace folDigLib
{
    public class Cancelar
    {
        public clsDatosDeRespuestaDelaCancelacionFD Documento(clsDatosParaCancelarFD datos)
        {
            clsDatosDeRespuestaDelaCancelacionFD _respuesta = new clsDatosDeRespuestaDelaCancelacionFD();
            string txtSalida = datos.txtSalida;
            switch (datos.tipoTimbrado)
            {

                case "Pruebas":
                    //Guarda los datos de respuesta del timbrado en un archivo fisico
                    _respuesta.codigoResultado = "SOA9998";
                    _respuesta.esCancelable = "ErrorGopac";
                    _respuesta.mensajeResultado = "";
                    _respuesta.operacionExitosa = "False";
                    _respuesta.mensajeError = "Error al cancelar";
                    _respuesta.mensajeDetallado = "No existe ambiente de prueba para cancelar documentos";
                    _respuesta.xmlResultado = "";
                    clsTimbrar timbrar = new clsTimbrar();
                    timbrar.GuardarArchivoSalidaCancelacion(_respuesta, txtSalida);
                    break;
                case "Produccion":
                    _respuesta = CancelarDocumento(datos);
                    break;
            }
            return _respuesta;

        }

        public clsDatosDeRespuestaDelaCancelacionFD CancelarDocumento(clsDatosParaCancelarFD datos)
        {
            clsDatosDeRespuestaDelaCancelacionFD _respuesta = new clsDatosDeRespuestaDelaCancelacionFD();
            string txtUsuario = datos.usuario;
            string txtPassw = datos.passw;
            string txtRfcEmisor = datos.rfcEmisor;// "";
            string txtClavePrivadaBase64 = datos.clavePrivadaBase64;// "";
            string txtPasswordClavePrivadaBase64 = datos.passwClavePrivadaBase64;// "";

            if (datos.folioSustitucion == "0")
                datos.folioSustitucion = "";
            string txtFolioSustitucion = datos.folioSustitucion;// "";
            string txtMotivo = datos.motivo;// "";
            string txtRfcReceptor = datos.rfcReceptor;// "";
            decimal txtTotal = Convert.ToDecimal(datos.total);// 0;
            string txtUuidDocumento = datos.uuidDocumento;// "";

            //Nombre del archivo xml acuse
            string xmlSalida = datos.xmlSalida;// + "Historico\\" + pArchivo + "Can_Acuse.xml";
            string txtSalida = datos.txtSalida;// pRutaSalida + "Historico\\" + pArchivo + "Can.txt"; ;

            try
            {
                //------------------------------------------------------------------------------------------------
                //Instancia del ws
                //------------------------------------------------------------------------------------------------
                folDigLib.WSCFDI.WSCFDI33Client ws = new folDigLib.WSCFDI.WSCFDI33Client();
                //------------------------------------------------------------------------------------------------
                //Instancia de la respuesta del ws
                //------------------------------------------------------------------------------------------------
                folDigLib.WSCFDI.RespuestaCancelacion Respuesta = new folDigLib.WSCFDI.RespuestaCancelacion();

                List<folDigLib.WSCFDI.DetalleCFDICancelacion> objDetalle = new List<folDigLib.WSCFDI.DetalleCFDICancelacion>();
                objDetalle.Add(new WSCFDI.DetalleCFDICancelacion
                {
                    //EsCancelable = total <= 1000 ? "Cancelable sin aceptación" : "Cancelable con aceptación",
                    FolioSustitucion = txtFolioSustitucion == null ? "" : txtFolioSustitucion,
                    Motivo = txtMotivo,
                    RFCReceptor = txtRfcReceptor,
                    Total = txtTotal,
                    UUID = txtUuidDocumento
                });
                int objetos = objDetalle.Count;
                folDigLib.WSCFDI.DetalleCFDICancelacion[] DetalleCancelacion = new folDigLib.WSCFDI.DetalleCFDICancelacion[objetos];
                DetalleCancelacion = objDetalle.ToArray();

                //------------------------------------------------------------------------------------------------
                //Llamado al metodo de cancelacion
                //------------------------------------------------------------------------------------------------

                Respuesta = ws.CancelarCFDI(txtUsuario, txtPassw, txtRfcEmisor, DetalleCancelacion, txtClavePrivadaBase64, txtPasswordClavePrivadaBase64);
                //------------------------------------------------------------------------------------------------
                //Instancia de la respuesta masica de cancelacion
                //------------------------------------------------------------------------------------------------
                List<folDigLib.WSCFDI.DetalleCancelacion> RespuestaCancelacionDetallada = new List<folDigLib.WSCFDI.DetalleCancelacion>();

                if (Respuesta.DetallesCancelacion != null)
                {
                    RespuestaCancelacionDetallada = Respuesta.DetallesCancelacion.ToList();
                    foreach (var DetRespuesta in RespuestaCancelacionDetallada)
                    {
                        //Guarda los datos de respuesta del timbrado en un archivo fisico
                        _respuesta.codigoResultado = DetRespuesta.CodigoResultado;
                        _respuesta.esCancelable = DetRespuesta.EsCancelable;
                        _respuesta.mensajeResultado = DetRespuesta.MensajeResultado;
                        _respuesta.operacionExitosa = Respuesta.OperacionExitosa.ToString();
                        _respuesta.mensajeError = Respuesta.MensajeError;
                        _respuesta.mensajeDetallado = Respuesta.MensajeErrorDetallado;
                        _respuesta.xmlResultado = Respuesta.XMLAcuse;
                        clsTimbrar timbrar = new clsTimbrar();
                        timbrar.GuardarArchivoSalidaCancelacion(_respuesta, txtSalida);
                    }
                }
                else
                {
                    //Guarda los datos de respuesta del timbrado en un archivo fisico
                    if (Respuesta.MensajeError == "Error de autenticación de usuario.")
                    {
                        _respuesta.codigoResultado = "SOA9996";
                    }
                    else
                    {
                        _respuesta.codigoResultado = "SOA9997";
                    }

                    _respuesta.esCancelable = "";
                    _respuesta.mensajeResultado = "";
                    _respuesta.operacionExitosa = Respuesta.OperacionExitosa.ToString();
                    _respuesta.mensajeError = Respuesta.MensajeError;
                    _respuesta.mensajeDetallado = Respuesta.MensajeErrorDetallado;
                    _respuesta.xmlResultado = Respuesta.XMLAcuse;
                    clsTimbrar timbrar = new clsTimbrar();
                    timbrar.GuardarArchivoSalidaCancelacion(_respuesta, txtSalida);
                }
                //------------------------------------------------------------------------------------------------
                //Genera xml del acuse
                //------------------------------------------------------------------------------------------------
                if (Respuesta.XMLAcuse != null && Respuesta.XMLAcuse != "")
                {
                    XmlDocument AcuseXML = new XmlDocument();
                    AcuseXML.LoadXml(Respuesta.XMLAcuse);
                    AcuseXML.Save(xmlSalida);
                }

            }
            catch (Exception ex)
            {

                _respuesta.codigoResultado = "SOA9999";
                _respuesta.esCancelable = "";
                _respuesta.mensajeResultado = "";
                _respuesta.operacionExitosa = "False";
                _respuesta.mensajeError = ex.Message;
                _respuesta.mensajeDetallado = ex.Message;
                _respuesta.xmlResultado = "";
                clsTimbrar timbrar = new clsTimbrar();
                timbrar.GuardarArchivoSalidaCancelacion(_respuesta, txtSalida);
            }
            return _respuesta;
        }
    }
}
