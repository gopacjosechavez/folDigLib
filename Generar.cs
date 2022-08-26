using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Xml.XPath;
using System.Xml.Xsl;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace folDigLib
{
    public class Generar
    {
        //-------------------------------------------------------------------------
        // Proceso para la Genera XML
        //-------------------------------------------------------------------------
        public folDigLib.respuesta XML(spSoaWeb_CFDI40_DatosGenerales_Result spDatosGenerales, List<spSoaWeb_CFDI40_Conceptos_Result> spDatosConceptos, List<spSoaWeb_CFDI40_Impuestos_Result> spDatosImpuestos,List<spSoaWeb_CFDI40_DatosRealacionados_Result> spDatosRelacionados, string pathXSLT = @"C:\Temp\cfdi\xslt\cadenaoriginal_4_0.xslt")
        {
            folDigLib.respuesta resultado = new folDigLib.respuesta();
            pathXSLT = spDatosGenerales.rutaXML + "\\Historico\\cadenaoriginal_4_0.xslt";
            try
            {
                //--------------------------------------------------------
                //   Carga datos
                //--------------------------------------------------------
                cfdi40.Comprobante comprobante = CargaDatos(spDatosGenerales, spDatosConceptos, spDatosImpuestos, spDatosRelacionados);

                //--------------------------------------------------------
                //   Genera xml
                //--------------------------------------------------------
                string pathXML = spDatosGenerales.rutaXML + "X" + spDatosGenerales.referencia + ".xml";
                string pathTXT = spDatosGenerales.rutaXML + "Historico\\" + spDatosGenerales.referencia + ".txt";
                resultado = GeneraXML(comprobante, pathXML);

                //--------------------------------------------------------
                //   Genera cadena original
                //--------------------------------------------------------
                string cadenaOriginal = CadenaOriginal(pathXML, pathXSLT);
                if (cadenaOriginal != "" && cadenaOriginal.Length > 10)
                {
                    //Sella la cadena original
                    string PrivadaPem = spDatosGenerales.privadaPem;
                    comprobante.Sello = SellarCadena(PrivadaPem, cadenaOriginal);

                    //Re-Genera xml con sello
                    resultado = GeneraXML(comprobante, pathXML);

                    //Inicializa los valores de la cadena original y el sello
                    resultado.cadenaOriginal = cadenaOriginal;
                    resultado.sello = comprobante.Sello;
                }


            }
            catch (Exception ex)
            {
                resultado.resultado = false;
                resultado.archivoXML = "";
                resultado.xml = "";
                resultado.cadenaOriginal = "";
                resultado.sello = "";
                resultado.error = "Error al generar XML";
                resultado.errorDetalle = ex.Message;
            }

            return resultado;
        }

        //-------------------------------------------------------------------------
        // Timbra XML
        //-------------------------------------------------------------------------
        public folDigLib.clsDatosDeRespuestaDelTimbradoFD TimbrarXML(spSoaWeb_CFDI40_DatosGenerales_Result spDatosGenerales, string pathXSLT = @"C:\Temp\cfdi\xslt\cadenaoriginal_4_0.xslt")
        {
            folDigLib.clsDatosDeRespuestaDelTimbradoFD resultado = new folDigLib.clsDatosDeRespuestaDelTimbradoFD();
            pathXSLT = spDatosGenerales.rutaXML + "\\Historico\\cadenaoriginal_4_0.xslt";
            string pathXML = spDatosGenerales.rutaXML + "X" + spDatosGenerales.referencia + ".xml";
            string pathTXT = spDatosGenerales.rutaXML + "Historico\\" + spDatosGenerales.referencia + ".txt";
            try
            {
                //--------------------------------------------------------
                //   Preparando datos para timbrar el xml
                //--------------------------------------------------------
                Timbrar timbrar = new Timbrar();
                clsDatosDeRespuestaDelTimbradoFD respuestaPAC = new clsDatosDeRespuestaDelTimbradoFD();

                clsDatosParaElTimbrarFD datosParaElTimbrado = new clsDatosParaElTimbrarFD();
                datosParaElTimbrado.usuario = spDatosGenerales.usuarioPAC;
                datosParaElTimbrado.passw = spDatosGenerales.passwordPAC;
                datosParaElTimbrado.referencia = spDatosGenerales.referencia;
                datosParaElTimbrado.rutaXML = spDatosGenerales.rutaXML;
                //datosParaElTimbrado.archivoXML = datos.referencia + ".xml";
                //datosParaElTimbrado.archivoTXT = datos.referencia + ".txt";
                datosParaElTimbrado.archivoXML = "X" + spDatosGenerales.referencia + ".xml";
                datosParaElTimbrado.archivoTXT = "X" + spDatosGenerales.referencia + ".txt";
                datosParaElTimbrado.tipoTimbrado = spDatosGenerales.tipotimbrado;
                datosParaElTimbrado.xmlSalida = pathXML;
                datosParaElTimbrado.txtSalida = pathTXT;
                datosParaElTimbrado.archivoXSLT = pathXSLT;
                datosParaElTimbrado.uuid = spDatosGenerales.timbreUUID;

                //--------------------------------------------------------
                //   Procesar el timbrado del xml
                //--------------------------------------------------------
                respuestaPAC = timbrar.Documento(datosParaElTimbrado);
                resultado = respuestaPAC;
            }
            catch (Exception ex)
            {
                resultado.archivo = "";
                resultado.codigoConfirmacion = "";
                resultado.codigoRespuesta = "";
                resultado.mensajeError = "Error";
                resultado.mensajeErrorDetallado = ex.Message;
                resultado.operacionExitosa = "False";
                resultado.xmlResultado = "";
            }
            return resultado;
        }

        //-------------------------------------------------------------------------
        // Genera y Timbra XML
        //-------------------------------------------------------------------------
        public folDigLib.respuesta XML2(Datoscomprobante datos, string pathXSLT = @"C:\Temp\cfdi\xslt\cadenaoriginal_4_0.xslt")
        {
            folDigLib.respuesta resultado = new folDigLib.respuesta();
            try
            {
                //Carga datos
                cfdi40.Comprobante comprobante = CargaDatos2(datos);

                //Genera xml
                string pathXML = datos.rutaXML + "X" + datos.referencia + ".xml";
                string pathTXT = datos.rutaXML + "Historico\\" + datos.referencia + ".txt";
                resultado = GeneraXML(comprobante, pathXML);

                //Genera cadena original
                //pathXSLT = datosParaElTimbrado.archivoXSLT;
                string cadenaOriginal = CadenaOriginal(pathXML, pathXSLT);
                if (cadenaOriginal != "" && cadenaOriginal.Length > 10)
                {
                    //Sella la cadena original
                    string PrivadaPem = datos.privadaPem;
                    comprobante.Sello = SellarCadena(PrivadaPem, cadenaOriginal);

                    //Re-Genera xml con sello
                    resultado = GeneraXML(comprobante, pathXML);

                    //Inicializa los valores de la cadena original y el sello
                    resultado.cadenaOriginal = cadenaOriginal;
                    resultado.sello = comprobante.Sello;


                    //Timbra documento
                    Timbrar timbrar = new Timbrar();
                    clsDatosDeRespuestaDelTimbradoFD respuestaPAC = new clsDatosDeRespuestaDelTimbradoFD();

                    clsDatosParaElTimbrarFD datosParaElTimbrado = new clsDatosParaElTimbrarFD();
                    datosParaElTimbrado.usuario = datos.usuarioPAC;
                    datosParaElTimbrado.passw = datos.passwordPAC;
                    datosParaElTimbrado.referencia = datos.referencia;
                    datosParaElTimbrado.rutaXML = datos.rutaXML;
                    //datosParaElTimbrado.archivoXML = datos.referencia + ".xml";
                    //datosParaElTimbrado.archivoTXT = datos.referencia + ".txt";
                    datosParaElTimbrado.archivoXML = "X" + datos.referencia + ".xml";
                    datosParaElTimbrado.archivoTXT = "X" + datos.referencia + ".txt";
                    datosParaElTimbrado.tipoTimbrado = datos.tipotimbrado;
                    datosParaElTimbrado.xmlSalida = pathXML;
                    datosParaElTimbrado.txtSalida = pathTXT;
                    datosParaElTimbrado.archivoXSLT = pathXSLT;
                    datosParaElTimbrado.uuid = datos.timbreUUID;

                    respuestaPAC = timbrar.Documento(datosParaElTimbrado);
                    if (respuestaPAC.xmlResultado != null && respuestaPAC.xmlResultado != "")
                    {
                        resultado.xml = respuestaPAC.xmlResultado;
                    }
                    //    //MessageBox.Show(respuestaPAC.archivo);
                    //    //MessageBox.Show(respuestaPAC.codigoConfirmacion);
                    //    //MessageBox.Show(respuestaPAC.codigoRespuesta);
                    //    //MessageBox.Show(respuestaPAC.mensajeError);
                    //    //MessageBox.Show(respuestaPAC.mensajeErrorDetallado);
                    //    //MessageBox.Show(respuestaPAC.operacionExitosa);
                    //    //MessageBox.Show(respuestaPAC.xmlResultado);
                }
            }
            catch (Exception ex)
            {
                resultado.resultado = false;
                resultado.archivoXML = "";
                resultado.xml = "";
                resultado.cadenaOriginal = "";
                resultado.sello = "";
                resultado.error = "Error al generar XML";
                resultado.errorDetalle = ex.Message;
            }

            return resultado;
        }

        //-------------------------------------------------------------------------
        // Carga datos para la generacion del xml XML
        //-------------------------------------------------------------------------
        public cfdi40.Comprobante CargaDatos(spSoaWeb_CFDI40_DatosGenerales_Result spDatosGenerales, List<spSoaWeb_CFDI40_Conceptos_Result> spDatosConceptos, List<spSoaWeb_CFDI40_Impuestos_Result> spDatosImpuestos, List<spSoaWeb_CFDI40_DatosRealacionados_Result> spDatosRelacionados)
        {
            cfdi40.Comprobante comprobante = new cfdi40.Comprobante();

            try
            {
                comprobante.Version = spDatosGenerales.version;
                comprobante.Serie = spDatosGenerales.serie;
                comprobante.Folio = spDatosGenerales.folio;
                comprobante.Fecha = spDatosGenerales.fecha;
                comprobante.Sello = "";
                comprobante.FormaPago = spDatosGenerales.formaPago;
                comprobante.NoCertificado = spDatosGenerales.noCertificado;
                comprobante.Certificado = spDatosGenerales.certificado;
                if (spDatosGenerales.condicionesDePago != "")
                    comprobante.CondicionesDePago = spDatosGenerales.condicionesDePago;
                comprobante.SubTotal = Convert.ToDecimal(spDatosGenerales.subtotal);
                //comprobante.Descuento = datos.descuento;
                comprobante.Moneda = spDatosGenerales.moneda;
                comprobante.TipoCambio = 1;// datos.tipocambio;
                comprobante.Total = Convert.ToDecimal(spDatosGenerales.total);
                comprobante.TipoDeComprobante = spDatosGenerales.tipoDeComprobante;
                comprobante.Exportacion = spDatosGenerales.exportacion;
                comprobante.MetodoPago = spDatosGenerales.metodoPago;
                comprobante.LugarExpedicion = spDatosGenerales.lugarExpedicion;
                //comprobante.Confirmacion = datos.confirmacion;

                ////idCliente = datos.idAgenda;
                ///
                //UUIDs Relacionados
                if(spDatosRelacionados.Count > 0)
                {
                    cfdi40.ComprobanteCfdiRelacionados relacion = new cfdi40.ComprobanteCfdiRelacionados();
                    List<cfdi40.ComprobanteCfdiRelacionadosCfdiRelacionado> relaciones = new List<cfdi40.ComprobanteCfdiRelacionadosCfdiRelacionado>();
                    var mTipoRelacion = spDatosRelacionados.FirstOrDefault();
                    foreach (var item in spDatosRelacionados)
                    {
                        relaciones.Add(new cfdi40.ComprobanteCfdiRelacionadosCfdiRelacionado
                        {
                            UUID = item.TimbreUUIDRelacion
                        });
                    }
                    relacion.TipoRelacion = mTipoRelacion.tipoRelacion;
                    relacion.CfdiRelacionado = relaciones.ToArray();
                    comprobante.CfdiRelacionados = relacion;
                }


                //Emisor
                cfdi40.ComprobanteEmisor emisor = new cfdi40.ComprobanteEmisor();
                emisor.Rfc = spDatosGenerales.emisorRFC;
                emisor.Nombre = spDatosGenerales.emisorNombre;
                emisor.RegimenFiscal = spDatosGenerales.emisorRegimenFiscal;
                comprobante.Emisor = emisor;

                //Receptor
                cfdi40.ComprobanteReceptor receptor = new cfdi40.ComprobanteReceptor();
                receptor.Rfc = spDatosGenerales.receptorRfc;
                receptor.Nombre = spDatosGenerales.receptorNombre;
                //receptor.ResidenciaFiscal = "";
                receptor.DomicilioFiscalReceptor = spDatosGenerales.receptorDomicilioFiscal;
                receptor.RegimenFiscalReceptor = spDatosGenerales.receptorRegimenFiscal;
                receptor.UsoCFDI = spDatosGenerales.usoCFDI;
                comprobante.Receptor = receptor;

                //Conceptos
                List<cfdi40.ComprobanteConcepto> concepto = new List<cfdi40.ComprobanteConcepto>();
                //cfdi40.ComprobanteConceptoImpuestos conceptoImpuesto = new cfdi40.ComprobanteConceptoImpuestos();

                var mConceptos = spDatosConceptos.Where(x => x.idTipo == 1).ToList();
                foreach (var item in mConceptos)
                {
                    cfdi40.ComprobanteConceptoImpuestos conceptoImpuesto = new cfdi40.ComprobanteConceptoImpuestos();
                    //Traslados
                    List<cfdi40.ComprobanteConceptoImpuestosTraslado> lstImpuetosT = new List<cfdi40.ComprobanteConceptoImpuestosTraslado>();
                    var mConceptoImpuestos = spDatosConceptos.Where(x => x.serie == item.serie && x.folio == item.folio && x.orden == item.orden && x.producto == item.producto && x.idTipo == 2).ToList();
                    foreach (var item2 in mConceptoImpuestos)
                    {
                        lstImpuetosT.Add(new cfdi40.ComprobanteConceptoImpuestosTraslado
                        {
                            Base = Convert.ToDecimal(item2.importeBase),
                            Impuesto = item2.impuesto,
                            TipoFactor = item2.tipoFactor,
                            TasaOCuota = Convert.ToDecimal(item2.tasaOCuota),
                            Importe = item2.importeImpuesto
                        });
                    }
                    conceptoImpuesto.Traslados = lstImpuetosT.ToArray();

                    //Retenciones

                    concepto.Add(new cfdi40.ComprobanteConcepto
                    {
                        ClaveUnidad = item.claveUnidad,
                        ClaveProdServ = item.ClaveFiscal,
                        Cantidad = item.cantidad,
                        Unidad = item.unidad,
                        NoIdentificacion = item.noIdentificacion,
                        Descripcion = item.descripcion,
                        ValorUnitario = item.valorUnitario,
                        Importe = item.ImporteConcepto,
                        ObjetoImp = item.objetoImp,
                        Impuestos = conceptoImpuesto
                    }

                    );
                }


                comprobante.Conceptos = concepto.ToArray();


                //Impuestos generales
                cfdi40.ComprobanteImpuestos impuesto = new cfdi40.ComprobanteImpuestos();

                //Traslados
                decimal totalImpuestosT = 0;
                List<cfdi40.ComprobanteImpuestosTraslado> lstImpuestosTraslados = new List<cfdi40.ComprobanteImpuestosTraslado>();
                foreach (var item in spDatosImpuestos)
                {
                    lstImpuestosTraslados.Add(new cfdi40.ComprobanteImpuestosTraslado
                    {
                        Base = Convert.ToDecimal(item.importeBase),
                        Impuesto = item.impuesto,
                        TipoFactor = item.tipoFactor,
                        TasaOCuota = Convert.ToDecimal(item.tasaOCuota),
                        Importe = item.importeImpuesto
                    });
                    totalImpuestosT = totalImpuestosT + item.importeImpuesto;
                }
                impuesto.Traslados = lstImpuestosTraslados.ToArray();
                //Total traslados
                impuesto.TotalImpuestosTrasladados = totalImpuestosT;
                impuesto.TotalImpuestosTrasladadosSpecified = true;

                //Retenciones

                //------------------------------------------------
                //   Asigna impuestos al comprobante           
                //------------------------------------------------
                comprobante.Impuestos = impuesto;

            }
            catch (Exception ex)
            {
            }



            return comprobante;
        }
        //-------------------------------------------------------------------------
        // Carga datos para la generacion del xml (Anterior)
        //-------------------------------------------------------------------------
        public cfdi40.Comprobante CargaDatos2(Datoscomprobante datos)
        {
            cfdi40.Comprobante comprobante = new cfdi40.Comprobante();

            try
            {
                comprobante.Version = datos.version;
                comprobante.Serie = datos.serie;
                comprobante.Folio = datos.folio;
                comprobante.Fecha = datos.fecha;
                comprobante.Sello = "";
                comprobante.FormaPago = datos.formaPago;
                comprobante.NoCertificado = datos.noCertificado;
                comprobante.Certificado = datos.certificado;
                comprobante.CondicionesDePago = datos.condicionesDePago;
                comprobante.SubTotal = Convert.ToDecimal(datos.subtotal);
                //comprobante.Descuento = datos.descuento;
                comprobante.Moneda = datos.moneda;
                comprobante.TipoCambio = 1;// datos.tipocambio;
                comprobante.Total = Convert.ToDecimal(datos.total);
                comprobante.TipoDeComprobante = datos.tipoDeComprobante;
                comprobante.Exportacion = datos.exportacion;
                comprobante.MetodoPago = datos.metodoPago;
                comprobante.LugarExpedicion = datos.lugarExpedicion;
                //comprobante.Confirmacion = datos.confirmacion;

                ////idCliente = datos.idAgenda;

                //Emisor
                cfdi40.ComprobanteEmisor emisor = new cfdi40.ComprobanteEmisor();
                emisor.Rfc = datos.emisorRFC;
                emisor.Nombre = datos.emisorNombre;
                emisor.RegimenFiscal = datos.emisorRegimenFiscal;
                comprobante.Emisor = emisor;

                //Receptor
                cfdi40.ComprobanteReceptor receptor = new cfdi40.ComprobanteReceptor();
                receptor.Rfc = datos.receptorRfc;
                receptor.Nombre = datos.receptorNombre;
                //receptor.ResidenciaFiscal = "";
                receptor.DomicilioFiscalReceptor = datos.receptorDomicilioFiscal;
                receptor.RegimenFiscalReceptor = datos.receptorRegimenFiscal;
                receptor.UsoCFDI = datos.usoCFDI;
                comprobante.Receptor = receptor;

                //Conceptos
                List<cfdi40.ComprobanteConcepto> concepto = new List<cfdi40.ComprobanteConcepto>();
                //cfdi40.ComprobanteConceptoImpuestos conceptoImpuesto = new cfdi40.ComprobanteConceptoImpuestos();
                foreach (var item in datos.conceptos)
                {
                    cfdi40.ComprobanteConceptoImpuestos conceptoImpuesto = new cfdi40.ComprobanteConceptoImpuestos();
                    //Traslados
                    List<cfdi40.ComprobanteConceptoImpuestosTraslado> lstImpuetosT = new List<cfdi40.ComprobanteConceptoImpuestosTraslado>();
                    foreach (var item2 in item.impuestos)
                    {
                        lstImpuetosT.Add(new cfdi40.ComprobanteConceptoImpuestosTraslado
                        {
                            Base = item2.Base,
                            Impuesto = item2.Impuesto,
                            TipoFactor = item2.TipoFactor,
                            TasaOCuota = item2.TasaOCuota,
                            Importe = item2.Importe
                        });
                    }
                    conceptoImpuesto.Traslados = lstImpuetosT.ToArray();

                    //Retenciones

                    concepto.Add(new cfdi40.ComprobanteConcepto
                    {
                        ClaveUnidad = item.ClaveUnidad,
                        ClaveProdServ = item.ClaveProdServ,
                        Cantidad = item.Cantidad,
                        Unidad = item.Unidad,
                        NoIdentificacion = item.NoIdentificacion,
                        Descripcion = item.Descripcion,
                        ValorUnitario = item.ValorUnitario,
                        Importe = item.Importe,
                        ObjetoImp = item.ObjetoImp,
                        Impuestos = conceptoImpuesto
                    }

                    );
                }


                comprobante.Conceptos = concepto.ToArray();


                //Impuestos generales
                cfdi40.ComprobanteImpuestos impuesto = new cfdi40.ComprobanteImpuestos();

                //Traslados
                decimal totalImpuestosT = 0;
                List<cfdi40.ComprobanteImpuestosTraslado> lstImpuestosTraslados = new List<cfdi40.ComprobanteImpuestosTraslado>();
                foreach (var item in datos.impuestos)
                {
                    lstImpuestosTraslados.Add(new cfdi40.ComprobanteImpuestosTraslado
                    {
                        Base = item.Base,
                        Impuesto = item.Impuesto,
                        TipoFactor = item.TipoFactor,
                        TasaOCuota = item.TasaOCuota,
                        Importe = item.Importe
                    });
                    totalImpuestosT = totalImpuestosT + item.Importe;
                }
                impuesto.Traslados = lstImpuestosTraslados.ToArray();
                //Total traslados
                impuesto.TotalImpuestosTrasladados = totalImpuestosT;
                impuesto.TotalImpuestosTrasladadosSpecified = true;

                //Retenciones

                //------------------------------------------------
                //   Asigna impuestos al comprobante           
                //------------------------------------------------
                comprobante.Impuestos = impuesto;

            }
            catch (Exception ex)
            {
            }



            return comprobante;
        }

        //-------------------------------------------------------------------------
        // Genera XML
        //-------------------------------------------------------------------------
        public folDigLib.respuesta GeneraXML(cfdi40.Comprobante comprobante, string pathXML)
        {
            folDigLib.respuesta resultado = new folDigLib.respuesta();
            try
            {
                string sXml = "";
                XmlSerializerNamespaces xmlNameSpace = new XmlSerializerNamespaces();
                xmlNameSpace.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                xmlNameSpace.Add("cfdi", "http://www.sat.gob.mx/cfd/4");

                XmlSerializer oXmlSerializar = new XmlSerializer(typeof(cfdi40.Comprobante));
                //--------------------------------------
                // Generación del XML
                //--------------------------------------
                using (var sww = new StringWriterWithEncoding(Encoding.UTF8))   // .StringWriterWithEncoding(Encoding.UTF8))
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = Encoding.UTF8;
                    settings.Encoding = Encoding.GetEncoding("utf-8");
                    settings.Indent = true;

                    using (XmlWriter writter = XmlWriter.Create(sww, settings))
                    {
                        oXmlSerializar.Serialize(writter, comprobante, xmlNameSpace);
                        sXml = sww.ToString();

                    }
                }
                //--------------------------------------
                // Serialización del XML
                // y se guarda el string en un archivo 
                //--------------------------------------

                Clipboard.SetDataObject(sXml, true);

                Encoding utf8 = Encoding.UTF8;
                System.IO.File.WriteAllText(pathXML, sXml, utf8);

                resultado.resultado = true;
                resultado.archivoXML = pathXML;
                resultado.xml = sXml;
                resultado.cadenaOriginal = "";
                resultado.sello = "";
                resultado.error = "";
                resultado.errorDetalle = "";

            }
            catch (Exception ex)
            {
                resultado.resultado = false;
                resultado.archivoXML = "";
                resultado.xml = "";
                resultado.cadenaOriginal = "";
                resultado.sello = "";
                resultado.error = "Error al generar XML";
                resultado.errorDetalle = ex.Message;
            }
            return resultado;
        }

        //-------------------------------------------------------------------------
        // Genera Cadena Original
        //-------------------------------------------------------------------------
        public static string CadenaOriginal(string pathXML, string pathXSLT = @"C:\Temp\cfdi\xslt\cadenaoriginal_4_0.xslt")
        {
            try
            {
                //Cargar el XML
                StreamReader reader = new StreamReader(pathXML); //comprobante
                XPathDocument myXPathDoc = new XPathDocument(reader);

                //Cargando el XSLT
                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(pathXSLT); //xslt del SAT

                StringWriter str = new StringWriter();
                XmlTextWriter myWriter = new XmlTextWriter(str);

                //Aplicando transformacion
                myXslTrans.Transform(myXPathDoc, null, myWriter);

                //Resultado
                string cadenaOriginal = str.ToString();
                cadenaOriginal = cadenaOriginal.Replace("\n", "");
                cadenaOriginal = cadenaOriginal.Replace("\t", "");
                cadenaOriginal = cadenaOriginal.Replace("\r", "");

                reader.Close();
                

                return cadenaOriginal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //-------------------------------------------------------------------------
        // Sella cadena original
        //-------------------------------------------------------------------------
        public static string SellarCadena(string PrivadaPem, string cadenaOriginal)
        {
            string resultado = "";
            try
            {
                string privateKey = PrivadaPem;
                byte[] pemprivatekey = opensslkey.DecodeOpenSSLPrivateKey(privateKey);

                //byte[] byteSign = Encoding.ASCII.GetBytes(cadenaOriginal);
                byte[] byteSign = Encoding.UTF8.GetBytes(cadenaOriginal);

                var rsa = opensslkey.DecodeRSAPrivateKey(pemprivatekey);

                //------------------------------------------------------------
                SHA256 hasher = SHA256CryptoServiceProvider.Create();
                var byteRSA = rsa.SignData(byteSign, hasher);
                //------------------------------------------------------------

                //var byteRSA = rsa.SignData(byteSign, CryptoConfig.MapNameToOID("SHA256"));
                //var byteRSA = rsa.SignData(byteSign, CryptoConfig.MapNameToOID("SHA1"));
                string sello = Convert.ToBase64String(byteRSA);
                resultado = sello;
            }
            catch (Exception ex)
            {
                resultado = ex.Message;
            }
            return resultado;
        }

        //-------------------------------------------------------------------------
        // Formateador de cadenas
        //-------------------------------------------------------------------------
        public sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding()
            {
            }

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get
                {
                    return encoding;
                }
            }
        }

        //-------------------------------------------------------------------------
        // Obtiene datos del timbrado apartir del XML
        //-------------------------------------------------------------------------
        public folDigLib.DatosTimbrado GetDatosTimbrado(string pathXML)
        {
            DatosTimbrado resultado = new DatosTimbrado();
            XmlDocument xml = new XmlDocument();
            xml.Load(pathXML);


            XDocument xmlInput = null;
            xmlInput = XDocument.Parse(xml.OuterXml);
            var mXML = xmlInput.Descendants().Where(x => x.Name.LocalName == "TimbreFiscalDigital").ToList();
            if (mXML != null && mXML.Count > 0)
            {
                foreach (var item in mXML)
                {
                    resultado.resultado = true;
                    resultado.Version = item.Attribute("Version").Value;
                    resultado.RfcProvCertif = item.Attribute("RfcProvCertif").Value; ;
                    resultado.UUID = item.Attribute("UUID").Value; ;
                    resultado.FechaTimbrado = item.Attribute("FechaTimbrado").Value; ;
                    resultado.SelloCFD = item.Attribute("SelloCFD").Value; ;
                    resultado.NoCertificadoSAT = item.Attribute("NoCertificadoSAT").Value; ;
                    resultado.SelloSAT = item.Attribute("SelloSAT").Value; ;
                    resultado.xmlSAT = xml.OuterXml;

                }
            }
            else
            {
                resultado.resultado = false;
                resultado.Version = "";
                resultado.RfcProvCertif = "";
                resultado.UUID = "";
                resultado.FechaTimbrado = "";
                resultado.SelloCFD = "";
                resultado.NoCertificadoSAT = "";
                resultado.SelloSAT = "";
                resultado.xmlSAT = "";
            }

            xml.Clone();

            return resultado;
        }

        //-------------------------------------------------------------------------
        // Genera reporte del documento CFDI4.0
        //-------------------------------------------------------------------------
        public class ReporteBaseCFDI40
        {
            Generar conv = new Generar();
            PdfFont regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont boldP = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            Cell cell = new Cell();
            float fontSize = 5f;
            float fontSizeImpuestos = 3f;
            float fontSizeTotales = 5f;
            float fontSizeTimbre = 5f;
            float altura = 10f;

            //Style styleCell = new Style().SetBorder(new SolidBorder(ColorConstants.BLACK, 0.5f));
            Style styleCell = new Style().SetBorder(Border.NO_BORDER);


            decimal totalIva = 0;
            string moneda = "P";
            string simboloMoneda = "$";
            //string directorioDestino = System.IO.Directory.GetCurrentDirectory();
            string pathLogo = System.IO.Directory.GetCurrentDirectory() + "\\Datos\\Imagenes\\Logo.bmp";

            public void documento(spSoaWeb_CFDI40_DatosGenerales_Result spDatos, List<spSoaWeb_CFDI40_Conceptos_Result> spConceptos, List<spSoaWeb_CFDI40_Impuestos_Result> spImpuestos, List<spSoaWeb_CFDI40_DatosRealacionados_Result> spDatosRelacionados, int totalPaginas, string nombrePDF, string tipoDeImpresion)
            {
                MemoryStream ms = new MemoryStream();
                PdfWriter pw = new PdfWriter(ms);
                PdfDocument pdfDocument = new PdfDocument(pw);
                Document doc = new Document(pdfDocument, PageSize.LETTER);
                doc.SetMargins(85, 35, 50, 35);

                //Genera imagen del logotipo
                iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(pathLogo));

                //Genera encabezado y pie de pagina
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler(img, spDatos, totalPaginas, tipoDeImpresion));
                pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler());
                //---------------------------------------------------------------

                //Datos para El PDF
                if (spDatos.moneda != "MXN")
                {
                    moneda = "D";
                    simboloMoneda = "US";
                }

                EmisorReceptor(doc, spDatos, tipoDeImpresion);
                DocumentosRelacionados(doc, spDatos, spDatosRelacionados);
                Conceptos(doc, spConceptos);
                Impuestos(doc, spImpuestos);
                Totales(doc, spDatos);

                TimbreFiscal(doc, spDatos);

                //doc.Add(new Paragraph("Renglon1").SetFontSize(fontSize));

                //Cierra el documento
                doc.Close();
                byte[] ByteStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(ByteStream, 0, ByteStream.Length);
                ms.Position = 0;

                //Genera el archivo fisico
                FileStream fileStream = new FileStream(nombrePDF, FileMode.Create);
                fileStream.Write(ByteStream, 0, ByteStream.Length);
                fileStream.Close();
            }
            private void EmisorReceptor(Document doc, spSoaWeb_CFDI40_DatosGenerales_Result spDatos, string tipoDeImpresion)
            {
                if (tipoDeImpresion == "recibo")
                {
                    doc.Add(new Paragraph(""));
                    doc.Add(new Paragraph(""));
                }


                float[] cellWidth = { 150f, 150f };
                float[] cellWidth2 = { 30f, 150f, 30f, 150f };
                Table tablaEncabezado = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();
                Table tablaDetalles = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();
                //---------------------------------------------------
                //Tabla detalles (Emisor/Receptor)
                //---------------------------------------------------
                //emisor nombre
                cell = new Cell().Add(new Paragraph("Nombre:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.emisorNombre + " " + spDatos.emisorRegimenCapital).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //receptor nombre
                cell = new Cell().Add(new Paragraph("Nombre:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.receptorNombre + " " + spDatos.receptorRegimenCapital).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //emisor rfc
                cell = new Cell().Add(new Paragraph("RFC:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.emisorRFC).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //receptor rfc
                cell = new Cell().Add(new Paragraph("RFC:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.receptorRfc).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //emisor regimen fiscal
                cell = new Cell().Add(new Paragraph("Régimen Fiscal:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.emisorRegimenFiscal + " " + spDatos.emisorRegimenFiscalDes).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //receptor regimen fiscal
                cell = new Cell().Add(new Paragraph("Régimen Fiscal:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.receptorRegimenFiscal + " " + spDatos.receptorRegimenFiscalDes).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //emisor codigo postal
                cell = new Cell().Add(new Paragraph("Código postal:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.emisorCodigoPostal).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //receptor codigo postal
                cell = new Cell().Add(new Paragraph("Código postal:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.receptorDomicilioFiscal).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //emisor lugar expedicion
                cell = new Cell().Add(new Paragraph("Lugar Expedición:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.lugarExpedicion).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //receptor Uso cfdi
                cell = new Cell().Add(new Paragraph("Uso de CFDI:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.usoCFDI).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //emisor CIF
                cell = new Cell().Add(new Paragraph("CIF:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.emisorCIF).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //receptor CIF
                cell = new Cell().Add(new Paragraph("CIF:").SetFontSize(fontSize)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaDetalles.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.receptorCIF).SetFontSize(fontSize)).AddStyle(styleCell);
                tablaDetalles.AddCell(cell);
                //---------------------------------------------------
                //Tabla titulos (Emisor/Receptor)
                //---------------------------------------------------
                //Datos del Emisor
                cell = new Cell().Add(new Paragraph("Emisor:").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncabezado.AddCell(cell);

                //Datos del Receptor
                cell = new Cell().Add(new Paragraph("Receptor").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncabezado.AddCell(cell);

                cell = new Cell(1, 2).Add(tablaDetalles).AddStyle(styleCell);
                tablaEncabezado.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));

                doc.Add(tablaEncabezado);

            }
            private void DocumentosRelacionados(Document doc, spSoaWeb_CFDI40_DatosGenerales_Result spDatos, List<spSoaWeb_CFDI40_DatosRealacionados_Result> spDatosRelacionados)
            {
                //Documentos relacionados
                bool tienedocumentosRelacionados = false;
                if (spDatosRelacionados.Count > 0)
                    tienedocumentosRelacionados = true;

                if (tienedocumentosRelacionados == true)
                {
                    float[] cellWidth3 = { 150f };
                    float[] cellWidth4 = { 50f, 150f };
                    Table tablaEncabezado = new Table(UnitValue.CreatePercentArray(cellWidth3)).UseAllAvailableWidth();
                    Table tablaDetalles = new Table(UnitValue.CreatePercentArray(cellWidth4)).UseAllAvailableWidth();

                    var mTipoRelacion = spDatosRelacionados.FirstOrDefault();
                    //tipo de sustitucion
                    cell = new Cell().Add(new Paragraph(mTipoRelacion.tipoRelacion + " " + mTipoRelacion.tipoRelacionDesc).SetFontSize(fontSize)).AddStyle(styleCell);
                    tablaDetalles.AddCell(cell);
                    //uuid sustituto
                    foreach(var item in spDatosRelacionados)
                    {
                        cell = new Cell().Add(new Paragraph(item.TimbreUUIDRelacion).SetFontSize(fontSize)).AddStyle(styleCell);
                        tablaDetalles.AddCell(cell);
                    }

                    //Encabezado
                    cell = new Cell().Add(new Paragraph("Documentos relacionados").SetFontSize(fontSize)).AddStyle(styleCell).SetBold().SetItalic();
                    tablaEncabezado.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;

                    //Detalle
                    cell = new Cell().Add(tablaDetalles).AddStyle(styleCell);
                    tablaEncabezado.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));

                    doc.Add(tablaEncabezado);

                }

            }
            private void Conceptos(Document doc, List<spSoaWeb_CFDI40_Conceptos_Result> spConceptos)
            {
                doc.Add(new Paragraph("").SetFontSize(fontSize));
                //Conceptos
                float[] cellWidth5 = { 20f, 30f, 60f, 20f, 20f, 30f, 30f, 30f };
                float[] cellWidth6 = { 20f, 20f, 20f, 20f, 20f, 20f };
                Table tableConceptos = new Table(UnitValue.CreatePercentArray(cellWidth5)).UseAllAvailableWidth();
                Table tableImpuestos = new Table(UnitValue.CreatePercentArray(cellWidth6)).UseAllAvailableWidth();
                //titulos conceptos
                cell = new Cell().Add(new Paragraph("Número").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("ClaveProdServ").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Descripción").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Unidad").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("ObjImpuesto").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Cantidad").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Precio Unitario").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Importe").SetFontSize(fontSize)).AddStyle(styleCell).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                tableConceptos.AddCell(cell);
                //detalle conceptos
                var conceptos = spConceptos.Where(x => x.idTipo == 1).ToList();
                foreach (var item in conceptos)
                {
                    cell = new Cell().Add(new Paragraph(item.noIdentificacion).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.ClaveFiscal).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.descripcion).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.claveUnidad + " " + item.unidad).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.objetoImp).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.cantidad.ToString()).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.valorUnitario.ToString(simboloMoneda + "#,###,##0.00")).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(item.ImporteConcepto.ToString(simboloMoneda + "#,###,##0.00")).SetFontSize(fontSize)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);

                    //titulo impuestos
                    tableImpuestos = new Table(UnitValue.CreatePercentArray(cellWidth6)).UseAllAvailableWidth();
                    cell = new Cell().Add(new Paragraph("Tipo").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                    tableImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("Impuesto").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                    tableImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("Tipo factor").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                    tableImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("TasaoCuota").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                    tableImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("Base").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                    tableImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("Importe").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                    tableImpuestos.AddCell(cell);

                    //detalle impuestos
                    var impuestos = spConceptos.Where(x => x.idTipo == 2 && x.serie == item.serie && x.folio == item.folio && x.orden == item.orden && x.producto == item.producto).ToList();
                    foreach (var imp in impuestos)
                    {


                        cell = new Cell().Add(new Paragraph("Traslado").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableImpuestos.AddCell(cell);
                        cell = new Cell().Add(new Paragraph(imp.impuesto).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableImpuestos.AddCell(cell);
                        cell = new Cell().Add(new Paragraph(imp.tipoFactor).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableImpuestos.AddCell(cell);
                        cell = new Cell().Add(new Paragraph(imp.tasaOCuota.ToString()).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableImpuestos.AddCell(cell);
                        cell = new Cell().Add(new Paragraph(imp.importeBase.Value.ToString(simboloMoneda + "#,###,##0.00")).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableImpuestos.AddCell(cell);
                        cell = new Cell().Add(new Paragraph(imp.importeImpuesto.ToString(simboloMoneda + "#,###,##0.00")).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableImpuestos.AddCell(cell);

                        cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                        tableConceptos.AddCell(cell);

                        //cell = new Cell(1, 3).Add(tableImpuestos).AddStyle(styleCell);
                        //tableConceptos.AddCell(cell);
                    }
                    cell = new Cell(1, 3).Add(tableImpuestos).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tableConceptos.AddCell(cell);
                }
                doc.Add(tableConceptos);

            }
            private void Impuestos(Document doc, List<spSoaWeb_CFDI40_Impuestos_Result> spImpuestos)
            {
                //Impuestos
                float[] cellWidth7 = { 150 };
                float[] cellWidth8 = { 20f, 30f, 60f, 20f, 20f, 30f, 30f, 30f };
                float[] cellWidth9 = { 20f, 20f, 20f, 20f, 20f, 20f };
                Table tablaEncabezado = new Table(UnitValue.CreatePercentArray(cellWidth7)).UseAllAvailableWidth();
                Table tablaEncavezadoEnBlanco = new Table(UnitValue.CreatePercentArray(cellWidth8)).UseAllAvailableWidth();
                Table tablaImpuestos = new Table(UnitValue.CreatePercentArray(cellWidth9)).UseAllAvailableWidth();

                //Encabezado
                cell = new Cell().Add(new Paragraph("Sumatoria de Impuestos").SetFontSize(fontSize)).AddStyle(styleCell).SetBold().SetItalic();
                tablaEncabezado.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                doc.Add(tablaEncabezado);

                //Encabezado en blanco
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                tablaEncavezadoEnBlanco.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;

                //Encabezado de Impuestos
                cell = new Cell().Add(new Paragraph("Tipo").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaImpuestos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Impuesto").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaImpuestos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Tipo factor").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaImpuestos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("TasaoCuota").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaImpuestos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Base").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaImpuestos.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Importe").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaImpuestos.AddCell(cell);

                totalIva = 0;
                //Detalle de impuestos
                foreach (var imp in spImpuestos)
                {
                    cell = new Cell().Add(new Paragraph("Traslado").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tablaImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(imp.impuesto).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tablaImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(imp.tipoFactor).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tablaImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(imp.tasaOCuota.ToString()).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tablaImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(imp.importeBase.Value.ToString(simboloMoneda + "#,###,##0.00")).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tablaImpuestos.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(imp.importeImpuesto.ToString(simboloMoneda + "#,###,##0.00")).SetFontSize(fontSizeImpuestos)).AddStyle(styleCell);
                    tablaImpuestos.AddCell(cell);
                    totalIva = totalIva + imp.importeImpuesto;
                }
                //Celda en blanco
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeImpuestos)).AddStyle(styleCell).SetFontColor(ColorConstants.LIGHT_GRAY);
                tablaEncavezadoEnBlanco.AddCell(cell);

                //Celda de impuestos
                cell = new Cell(1, 3).Add(tablaImpuestos).AddStyle(styleCell);
                tablaEncavezadoEnBlanco.AddCell(cell);

                doc.Add(tablaEncavezadoEnBlanco);

            }
            private void Totales(Document doc, spSoaWeb_CFDI40_DatosGenerales_Result spDatos)
            {
                //-------------------------
                //Totales
                float[] cellWidth10 = { 150f, 15f, 15f };
                Table tablaTotales = new Table(UnitValue.CreatePercentArray(cellWidth10)).UseAllAvailableWidth();

                string cantidadEnLetra = conv.NumeroALetras(Convert.ToDecimal(spDatos.total), moneda);

                cell = new Cell().Add(new Paragraph(cantidadEnLetra).SetFontSize(fontSizeTotales)).AddStyle(styleCell);
                tablaTotales.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Subtotal: " + simboloMoneda).SetFontSize(fontSizeTotales)).AddStyle(styleCell);
                tablaTotales.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.subtotal.ToString()).SetFontSize(fontSizeTotales)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaTotales.AddCell(cell);

                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeTotales)).AddStyle(styleCell);
                tablaTotales.AddCell(cell);
                cell = new Cell().Add(new Paragraph("IVA: " + simboloMoneda).SetFontSize(fontSizeTotales)).AddStyle(styleCell);
                tablaTotales.AddCell(cell);
                cell = new Cell().Add(new Paragraph(totalIva.ToString()).SetFontSize(fontSizeTotales)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaTotales.AddCell(cell);

                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeTotales)).AddStyle(styleCell);
                tablaTotales.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Total: " + simboloMoneda).SetFontSize(fontSizeTotales)).AddStyle(styleCell);
                tablaTotales.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.total.ToString()).SetFontSize(fontSizeTotales)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT);
                tablaTotales.AddCell(cell);
                doc.Add(tablaTotales);

            }
            private void TimbreFiscal(Document doc, spSoaWeb_CFDI40_DatosGenerales_Result spDatos)
            {
                float[] cellWidth1 = { 150f };
                float[] cellWidth2 = { 25f, 50f, 35f, 50f };
                float[] cellWidth3 = { 20f, 200f };
                float[] cellWidth4 = { 200f };
                Table tablaEncabezado = new Table(UnitValue.CreatePercentArray(cellWidth1)).UseAllAvailableWidth();
                Table tablaTimbre = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();
                Table tablaSello = new Table(UnitValue.CreatePercentArray(cellWidth3)).UseAllAvailableWidth();
                Table tablaSellos = new Table(UnitValue.CreatePercentArray(cellWidth4)).UseAllAvailableWidth();

                cell = new Cell().Add(new Paragraph("Timbre Fiscal Digital").SetFontSize(fontSize)).AddStyle(styleCell).SetBold().SetItalic();
                tablaEncabezado.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;
                doc.Add(tablaEncabezado);

                //Primer renglon
                cell = new Cell().Add(new Paragraph("Número de serie del certificado:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT).SetBold();
                tablaTimbre.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.noCertificado).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaTimbre.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Folio Fiscal UUID:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT).SetBold();
                tablaTimbre.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.timbreUUID).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaTimbre.AddCell(cell);
                //Segundo renglon
                cell = new Cell().Add(new Paragraph("Fecha y hora del certificado:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT).SetBold();
                tablaTimbre.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.fechaHoraTimbrado).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaTimbre.AddCell(cell);
                cell = new Cell().Add(new Paragraph("Número de serie del certificado del SAT:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.RIGHT).SetBold();
                tablaTimbre.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.numeroCertificadoSat).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaTimbre.AddCell(cell);
                doc.Add(tablaTimbre);

                //Sellos
                cell = new Cell().Add(new Paragraph("Sello Digital:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT).SetBold();
                tablaSellos.AddCell(cell);

                int longitud = 150;// 250;
                int largo = 0;
                decimal renglones = 0;
                string strRenglones = "";
                int intRenglones = 0;
                float decRenglones = 0;
                if (decRenglones > 0)
                    intRenglones = intRenglones + 1;
                int contador = longitud;
                int cadena = longitud;
                int posini = 0;
                string textofinal = "";
                //------------------------------------------------------------------
                //   selloDigital
                //------------------------------------------------------------------
                if (spDatos.selloDigital != "")
                {
                    largo = spDatos.selloDigital.Length;
                    renglones = Convert.ToDecimal(largo) / Convert.ToDecimal(longitud);
                    strRenglones = renglones.ToString("#.00");
                    intRenglones = int.Parse(strRenglones.Split('.')[0]);
                    decRenglones = float.Parse("0," + strRenglones.Split('.')[1]);
                    if (decRenglones > 0)
                        intRenglones = intRenglones + 1;
                    contador = longitud;
                    cadena = longitud;
                    posini = 0;
                    textofinal = "";

                    for (int i = 0; i < intRenglones; i++)
                    {
                        if (contador > largo)
                            cadena = longitud - (contador - largo);

                        //MessageBox.Show("1.2. Renglon: " + i.ToString() + ", posini = " + posini.ToString() + ", cadena = " + cadena.ToString() + ", largo: " + largo + ", contador: " + contador);

                        if (textofinal == "")
                        {
                            textofinal = spDatos.selloDigital.Substring(posini, cadena);
                        }
                        else
                        {
                            textofinal = textofinal + "\n" + spDatos.selloDigital.Substring(posini, cadena);
                        }

                        posini = posini + longitud;
                        contador = contador + longitud;
                    }

                }

                cell = new Cell().Add(new Paragraph(textofinal).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaSellos.AddCell(cell);
                //cell = new Cell().Add(new Paragraph(spDatos.selloDigital).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                //tablaSellos.AddCell(cell);

                cell = new Cell().Add(new Paragraph("Sello del SAT:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT).SetBold();
                tablaSellos.AddCell(cell);
                //------------------------------------------------------------------
                //   selloDigitalSAT
                //------------------------------------------------------------------
                if (spDatos.sellloSAT != "")
                {
                    largo = spDatos.sellloSAT.Length;
                    renglones = Convert.ToDecimal(largo) / Convert.ToDecimal(longitud);
                    strRenglones = renglones.ToString("#.00");
                    intRenglones = int.Parse(strRenglones.Split('.')[0]);
                    decRenglones = float.Parse("0," + strRenglones.Split('.')[1]);
                    if (decRenglones > 0)
                        intRenglones = intRenglones + 1;
                    contador = longitud;
                    cadena = longitud;
                    posini = 0;
                    textofinal = "";
                    for (int i = 0; i < intRenglones; i++)
                    {
                        if (contador > largo)
                            cadena = longitud - (contador - largo);

                        //MessageBox.Show("0. Renglon: " + i.ToString() + ", posini = " + posini.ToString() + ", cadena = " + cadena.ToString() + ", largo: " + largo + ", contador: " + contador);

                        if (textofinal == "")
                        {
                            textofinal = spDatos.sellloSAT.Substring(posini, cadena);
                        }
                        else
                        {
                            textofinal = textofinal + "\n" + spDatos.sellloSAT.Substring(posini, cadena);
                        }

                        posini = posini + longitud;
                        contador = contador + longitud;
                    }

                }
                cell = new Cell().Add(new Paragraph(textofinal).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaSellos.AddCell(cell);

                cell = new Cell().Add(new Paragraph("Cadena Original del complemento de Certificación del SAT:").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT).SetBold();
                tablaSellos.AddCell(cell);
                //------------------------------------------------------------------
                //   Cadena original
                //------------------------------------------------------------------
                if (spDatos.cadenaOriginal != "")
                {
                    largo = spDatos.cadenaOriginal.Length;
                    renglones = Convert.ToDecimal(largo) / Convert.ToDecimal(longitud);
                    strRenglones = renglones.ToString("#.00");
                    intRenglones = int.Parse(strRenglones.Split('.')[0]);
                    decRenglones = float.Parse("0," + strRenglones.Split('.')[1]);
                    if (decRenglones > 0)
                        intRenglones = intRenglones + 1;
                    contador = longitud;
                    cadena = longitud;
                    posini = 0;
                    textofinal = "";
                    for (int i = 0; i < intRenglones; i++)
                    {
                        if (contador > largo)
                            cadena = longitud - (contador - largo);

                        //MessageBox.Show("0. Renglon: " + i.ToString() + ", posini = " + posini.ToString() + ", cadena = " + cadena.ToString() + ", largo: " + largo + ", contador: " + contador);

                        if (textofinal == "")
                        {
                            textofinal = spDatos.cadenaOriginal.Substring(posini, cadena);
                        }
                        else
                        {
                            textofinal = textofinal + "\n" + spDatos.cadenaOriginal.Substring(posini, cadena);
                        }

                        posini = posini + longitud;
                        contador = contador + longitud;
                    }

                }
                cell = new Cell().Add(new Paragraph(textofinal).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                tablaSellos.AddCell(cell);
                //cell = new Cell().Add(new Paragraph(spDatos.cadenaOriginal).SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                //tablaSellos.AddCell(cell);
                //------------------------------------------------------------------

                string nombreQR = spDatos.rutaArchivoCBB;// .rutaCBB;// @"C:\Temp\timbrar\QR_" + spDatos.referencia.Substring(1) + ".jpg";
                //Genera imagen del QR
                if (File.Exists(nombreQR))
                {
                    iText.Layout.Element.Image img = new iText.Layout.Element.Image(ImageDataFactory.Create(nombreQR));
                    img.SetWidth(45);
                    img.SetHeight(45);

                    cell = new Cell().Add(img).AddStyle(styleCell);
                    tablaSello.AddCell(cell);
                }
                else
                {
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSizeTimbre)).AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT);
                    tablaSello.AddCell(cell);
                }

                cell = new Cell().Add(tablaSellos).AddStyle(styleCell);
                tablaSello.AddCell(cell);
                doc.Add(tablaSello);

                doc.Add(new Paragraph("").SetFontSize(fontSizeTimbre));
                doc.Add(new Paragraph("").SetFontSize(fontSizeTimbre));
                doc.Add(new Paragraph("").SetFontSize(fontSizeTimbre));

                tablaEncabezado = new Table(UnitValue.CreatePercentArray(cellWidth1)).UseAllAvailableWidth();
                cell = new Cell().Add(new Paragraph("Este documento es la representación de un CFDI").SetFontSize(5f)).AddStyle(styleCell).SetTextAlignment(TextAlignment.CENTER).SetBold();
                tablaEncabezado.AddCell(cell);
                doc.Add(tablaEncabezado);

            }


        }
        public class HeaderEventHandler : IEventHandler
        {
            public int gTotal = 0;
            iText.Layout.Element.Image Img;
            spSoaWeb_CFDI40_DatosGenerales_Result spDatos;
            int totalPaginas;
            string tipoDeImpresion;
            //Image ImgAlerta;
            //int IdProgramaSp;
            PdfFont regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont boldP = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            //gsiConexion gEntityLocal;
            //string gAreaImpresion;


            //public HeaderEventHandler2(Image img, int _IdPrograma, gsiConexion _Conexion, Image imagAlerta, string areaImpresion)
            public HeaderEventHandler(iText.Layout.Element.Image img, spSoaWeb_CFDI40_DatosGenerales_Result _spDatos, int _totalPaginas, string _tipoDeImpresion)
            {
                Img = img;
                spDatos = _spDatos;
                totalPaginas = _totalPaginas;
                tipoDeImpresion = _tipoDeImpresion;
                //IdProgramaSp = _IdPrograma;
                //gEntityLocal = _Conexion;
                //ImgAlerta = imagAlerta;
                //gAreaImpresion = areaImpresion;
            }

            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                iText.Kernel.Geom.Rectangle rootArea = new iText.Kernel.Geom.Rectangle(35, page.GetPageSize().GetTop() - 140, page.GetPageSize().GetRight() - 70, 120);

                Canvas canvas = new Canvas(docEvent.GetPage(), rootArea);

                canvas.Add(getTable(docEvent))
                    //.ShowTextAligned("Este es el encabezado de pagina", 10, 0, TextAlignment.CENTER)
                    //.ShowTextAligned("Este es el pie de pagina", 10, 10, TextAlignment.CENTER)
                    //.ShowTextAligned("Textyo agregado", 612, 0, TextAlignment.RIGHT)
                    .Close();

                if(spDatos.Estatus == "Cancelado")
                {
                    string cancelado = @"C:\Temp\timbrar\cancelado.png";
                    iText.Layout.Element.Image maCancelado = new iText.Layout.Element.Image(ImageDataFactory.Create(cancelado));
                    maCancelado.SetWidth(600);
                    maCancelado.SetHeight(600);

                    iText.Kernel.Geom.Rectangle rectangle = new iText.Kernel.Geom.Rectangle(120, 200, 400, 400);
                    Canvas canvas2 = new Canvas(page, rectangle);
                    canvas2.Add(maCancelado).Close();
                }


            }

            public Paragraph GenerateParagraphBoldAndNormal(string BoldText, string NormalText)
            {
                Text first = new Text(BoldText).SetFont(boldP);
                Text second = new Text(NormalText).SetFont(regular);
                Paragraph paragraph = new Paragraph().Add(first).Add(second);

                return paragraph;
            }

            public Table getTable(PdfDocumentEvent docEvent)
            {
                float altura = 22f;
                //float[] cellWidth = { 20f, 80f };
                float[] cellWidth = { 25f, 75f };
                float[] cellWidth2 = { 15f, 40f, 30f, 50f, 20f, 20f };
                float fontSize = 5f;
                Table tabla = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();
                Table tabla2 = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();
                Table tabla3 = new Table(UnitValue.CreatePercentArray(cellWidth2)).UseAllAvailableWidth();

                Style styleCell = new Style().SetBorder(Border.NO_BORDER);
                Cell cell = new Cell();

                //---------------------------------------------------
                if (tipoDeImpresion == "recibo")
                {
                    string strRecibo = "R" + tipoDeImpresion.Substring(1);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                    tabla2.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                    tabla2.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                    tabla2.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                    tabla2.AddCell(cell);
                    cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                    tabla2.AddCell(cell);
                    cell = new Cell().Add(new Paragraph(strRecibo).SetFontSize(fontSize)).AddStyle(styleCell).SetBold();
                    tabla2.AddCell(cell);
                }
                //factura
                cell = new Cell().Add(new Paragraph("Factura:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.serie + "   " + spDatos.folio).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //moneda
                cell = new Cell().Add(new Paragraph("Moneda:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.moneda).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //exportacion
                cell = new Cell().Add(new Paragraph("Exportación:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.exportacion + " " + spDatos.exportacionDesc).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //version
                cell = new Cell().Add(new Paragraph("Versión:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.version).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //metodo de pago
                cell = new Cell().Add(new Paragraph("Método de pago:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.metodoPago + " " + spDatos.metodoPagoDesc).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //rfc pac
                cell = new Cell().Add(new Paragraph("RFC PAC:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph("SVT110323827").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //fecha
                cell = new Cell().Add(new Paragraph("Fecha:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.fecha).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //forma de pago
                cell = new Cell().Add(new Paragraph("Forma de pago:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.formaPago + " " + spDatos.formaPagoDesc).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //tipo
                cell = new Cell().Add(new Paragraph("Tipo:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.tipoDeComprobanteDesc).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //vacio1
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //tipo de cambio
                cell = new Cell().Add(new Paragraph("Tipo de cambio:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.tipoCambio.ToString()).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //pagina
                PdfPage page = docEvent.GetPage();
                int pageNum = docEvent.GetDocument().GetPageNumber(page);
                int pageTot = totalPaginas;
                //int pageTot = docEvent.GetDocument().GetNumberOfPages().;

                cell = new Cell().Add(new Paragraph("Página:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(pageNum.ToString() + " de " + pageTot.ToString()).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //oc
                cell = new Cell().Add(new Paragraph("OC:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.ordenCompra).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //condiciones de pago
                cell = new Cell().Add(new Paragraph("Condiciones de pago:").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph(spDatos.condicionesDePago).SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //vacio2
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                cell = new Cell().Add(new Paragraph("").SetFontSize(fontSize)).AddStyle(styleCell);
                tabla2.AddCell(cell);
                //---------------------------------------------------
                Img.SetWidth(80);
                Img.SetHeight(45);
                //logotipo
                cell = new Cell().Add(Img).AddStyle(styleCell);
                tabla.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f));
                //encabezado
                cell = new Cell().Add(tabla2).AddStyle(styleCell);
                tabla.AddCell(cell).SetBorderBottom(new SolidBorder(ColorConstants.BLACK, 0.5f)); ;

                //---------------------------------------------------
                return tabla;
            }



        }
        public class FooterEventHandler : IEventHandler
        {
            //Image Img;
            PdfFont regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            PdfFont boldP = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            public FooterEventHandler()
            {
            }

            public void HandleEvent(Event @event)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
                PdfDocument pdfDoc = docEvent.GetDocument();
                PdfPage page = docEvent.GetPage();

                iText.Kernel.Geom.Rectangle rootArea = new iText.Kernel.Geom.Rectangle(35, 0, page.GetPageSize().GetRight() - 70, 50);

                Canvas canvas = new Canvas(docEvent.GetPage(), rootArea);

                canvas.Add(getTable(docEvent))
                    //.ShowTextAligned("Este es el encabezado de pagina", 10, 0, TextAlignment.CENTER)
                    //.ShowTextAligned("Este es el pie de pagina", 10, 10, TextAlignment.CENTER)
                    //.ShowTextAligned("Textyo agregado", 612, 0, TextAlignment.RIGHT)
                    .Close();

            }

            public Paragraph GenerateParagraphBoldAndNormal(string BoldText, string NormalText)
            {
                Text first = new Text(BoldText).SetFont(boldP);
                Text second = new Text(NormalText).SetFont(regular);
                Paragraph paragraph = new Paragraph().Add(first).Add(second);

                return paragraph;
            }

            public Table getTable(PdfDocumentEvent docEvent)
            {
                float[] cellWidth = { 50f, 50f };
                Table tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();

                Style styleCell = new Style().SetBorder(Border.NO_BORDER);

                Style styleText = new Style().SetTextAlignment(TextAlignment.RIGHT).SetFontSize(9f);
                Style styleText2 = new Style().SetTextAlignment(TextAlignment.LEFT).SetFontSize(9f);

                Cell cell = new Cell();

                PdfPage page = docEvent.GetPage();


                int pageNum = docEvent.GetDocument().GetPageNumber(page);

                return tableEvent;

            }


        }

        //-------------------------------------------------------------------------
        // Obtiene la cantidad en letras
        //-------------------------------------------------------------------------
        public string NumeroALetras(decimal numberAsString, string moneda, bool conParentesis = true)
        {
            string dec;
            string monDescripcion = "PESOS";
            string simbolo = "M.N.";
            if (moneda == "D")
            {
                monDescripcion = "DÓLARES";
                simbolo = "US DLS.";
            }

            var entero = Convert.ToInt64(Math.Truncate(numberAsString));
            var decimales = Convert.ToInt32(Math.Round((numberAsString - entero) * 100, 2));
            if (decimales > 0)
            {
                //dec = " PESOS CON " + decimales.ToString() + "/100";
                //dec = $" PESOS {decimales:0,0} /100 M.N.)***";
                dec = $" {decimales:0,0}/100 ";

                //dec = $" PESOS {decimales:0,0}/100 M.N.)***";


            }
            //Código agregado por mí
            else
            {
                //dec = " PESOS CON " + decimales.ToString() + " /100";
                dec = $" {decimales:0,0}/100 ";

            }
            var res = NumeroALetras(Convert.ToDouble(entero)) + " " + monDescripcion + " " + dec + " " + simbolo;
            if (conParentesis == true)
                res = "***(" + res + ")***";
            return res;
        }
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private string NumeroALetras(double value)
        {
            string num2Text; value = Math.Truncate(value);
            if (value == 0) num2Text = "CERO";
            else if (value == 1) num2Text = "UNO";
            else if (value == 2) num2Text = "DOS";
            else if (value == 3) num2Text = "TRES";
            else if (value == 4) num2Text = "CUATRO";
            else if (value == 5) num2Text = "CINCO";
            else if (value == 6) num2Text = "SEIS";
            else if (value == 7) num2Text = "SIETE";
            else if (value == 8) num2Text = "OCHO";
            else if (value == 9) num2Text = "NUEVE";
            else if (value == 10) num2Text = "DIEZ";
            else if (value == 11) num2Text = "ONCE";
            else if (value == 12) num2Text = "DOCE";
            else if (value == 13) num2Text = "TRECE";
            else if (value == 14) num2Text = "CATORCE";
            else if (value == 15) num2Text = "QUINCE";
            else if (value < 20) num2Text = "DIECI" + NumeroALetras(value - 10);
            else if (value == 20) num2Text = "VEINTE";
            else if (value < 30) num2Text = "VEINTI" + NumeroALetras(value - 20);
            else if (value == 30) num2Text = "TREINTA";
            else if (value == 40) num2Text = "CUARENTA";
            else if (value == 50) num2Text = "CINCUENTA";
            else if (value == 60) num2Text = "SESENTA";
            else if (value == 70) num2Text = "SETENTA";
            else if (value == 80) num2Text = "OCHENTA";
            else if (value == 90) num2Text = "NOVENTA";
            else if (value < 100) num2Text = NumeroALetras(Math.Truncate(value / 10) * 10) + " Y " + NumeroALetras(value % 10);
            else if (value == 100) num2Text = "CIEN";
            else if (value < 200) num2Text = "CIENTO " + NumeroALetras(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) num2Text = NumeroALetras(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) num2Text = "QUINIENTOS";
            else if (value == 700) num2Text = "SETECIENTOS";
            else if (value == 900) num2Text = "NOVECIENTOS";
            else if (value < 1000) num2Text = NumeroALetras(Math.Truncate(value / 100) * 100) + " " + NumeroALetras(value % 100);
            else if (value == 1000) num2Text = "MIL";
            else if (value < 2000) num2Text = "MIL " + NumeroALetras(value % 1000);
            else if (value < 1000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value % 1000);
                }
            }
            else if (value == 1000000)
            {
                num2Text = "UN MILLON";
            }
            else if (value < 2000000)
            {
                num2Text = "UN MILLON " + NumeroALetras(value % 1000000);
            }
            else if (value < 1000000000000)
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000) * 1000000);
                }
            }
            else if (value == 1000000000000) num2Text = "UN BILLON";
            else if (value < 2000000000000) num2Text = "UN BILLON " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            else
            {
                num2Text = NumeroALetras(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                {
                    num2Text = num2Text + " " + NumeroALetras(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
            }
            return num2Text;
        }

        //-------------------------------------------------------------------------
        // Genera QR para los documentos CFDI4.0
        //-------------------------------------------------------------------------
        public static Array GetImagenQR(string txtQRCode)
        {
            //txtQRCode = "https://www.gopac.mx/";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            //System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            //imgBarCode.Height = 150;
            //imgBarCode.Width = 150;
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                    //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }

        }
        public static byte[] GetImagenQRByte(string txtQRCode)
        {
            //txtQRCode = "https://www.gopac.mx/";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(txtQRCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            //System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            //imgBarCode.Height = 150;
            //imgBarCode.Width = 150;

            //using (System.Drawing.Bitmap bitMap = qrCode.GetGraphic(20))
            using (System.Drawing.Bitmap bitMap = qrCode.GetGraphic(2))
            {

                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] ArchivoBytes = ms.ToArray();// System.Drawing.Image.FromStream(ms); ;
                    return ArchivoBytes;
                    //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                }
            }

        }


    }

}
