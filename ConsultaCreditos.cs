using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;


namespace folDigLib
{
    public class ConsultaCreditos
    {
        public void GetCreditos(string rutaCSV,string strUser, string strPassword)
        {
            try
            {
                //Se instancia el WS de Timbrado.
                WSCFDI.WSCFDI33Client ServicioTimbrado = new WSCFDI.WSCFDI33Client();

                //Se instancia la Respuesta del WS de Timbrado.
                WSCFDI.RespuestaCreditos RespuestaServicio = new WSCFDI.RespuestaCreditos();
                List<WSCFDI.DetallesPaqueteCreditos> RespuestaDetalleCreditos = new List<WSCFDI.DetallesPaqueteCreditos>();

                //Nombre del archivo
                string nombreCSV = "FoliosDisponibles.csv";
                string nombreArchivo = rutaCSV + nombreCSV;

                RespuestaServicio = ServicioTimbrado.ConsultarCreditos(strUser, strPassword);


                //Obtiene la respuesta del web services
                if (RespuestaServicio.OperacionExitosa == true)
                {
                    //creación del archivo
                    FileInfo t = new FileInfo(nombreArchivo);
                    StreamWriter Tex = t.CreateText();

                    //Se asigna la respuesta al objeto que contendra la operación de todos los UUID a cancelar.
                    RespuestaDetalleCreditos = RespuestaServicio.Paquetes.ToList();

                    //Se recorre el objeto para obtener la operacion independiente de cada CFDi.

                    int n = 0;
                    string cadena = "NombrePaquete," +
                            "En Uso," +
                            "FechaActivacion," +
                            "FechaVencimiento," +
                            "TotalTimbres," +
                            "TimbresUsados," +
                            "TimbresRestantes," +
                            "Vigente";
                    Tex.WriteLine(cadena);


                    foreach (WSCFDI.DetallesPaqueteCreditos Paquete in RespuestaDetalleCreditos)
                    {
                        string nombrePaquete = Paquete.Paquete.Replace(",", "").ToString();
                        string paqueteEnUso = Paquete.EnUso == false ? "No" : "Si";
                        string fechaActivacion = Paquete.FechaActivacion.Value.ToString("dd MMMM yyyy");
                        string fechaVencimiento = Paquete.FechaVencimiento.Value.ToString("dd MMMM yyyy");
                        string totalTimbres = Paquete.Timbres.ToString();
                        string timbresUsados = Paquete.TimbresUsados.ToString();
                        string timbresRestantes = Paquete.TimbresRestantes.ToString();
                        string paqueteVigente = Paquete.Vigente == false ? "No" : "Si";

                        cadena = nombrePaquete + ",";
                        cadena = cadena + paqueteEnUso + ",";
                        cadena = cadena + fechaActivacion + ",";
                        cadena = cadena + fechaVencimiento + ",";
                        cadena = cadena + totalTimbres + ",";
                        cadena = cadena + timbresUsados + ",";
                        cadena = cadena + timbresRestantes + ",";
                        cadena = cadena + paqueteVigente;
                        //Agrega renglon
                        Tex.WriteLine(cadena);
                    }
                    Tex.Write(Tex.NewLine);
                    Tex.Close();
                }
                else
                {
                    MessageBox.Show("Error al consumir el web services Folios Digitales" + RespuestaServicio.MensajeError);
                    string errorWS = "Error al consumir el web services Folios Digitales" + RespuestaServicio.MensajeError;
                    FileInfo fileError = new FileInfo(rutaCSV + "web_log.txt");
                    StreamWriter swError = fileError.CreateText();
                    swError.Write(errorWS);
                    swError.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                throw;
            }
        }
    }
}
