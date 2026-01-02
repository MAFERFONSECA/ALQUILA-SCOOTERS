using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;  // Necesario para usar la clase Image
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RENTA_SCOOTERS.CLASES
{
    public class AlquilerManager
    {
        public int AlquilerId { get; set; }
        public int PatinId { get; set; }
        public string ClienteTelefono { get; set; }
        public double Costo { get; set; }
        public int TiempoRestante { get; set; }
        public DateTime HoraFinalizacion { get; set; }
        public bool EstaEnCurso { get; set; }
        public bool TimeUp { get; set; }
        public BitmapImage CredencialImagen { get; set; }
        public BitmapImage PatinImagen { get; set; }  // Propiedad para la imagen del patín


        // Método para consultar los alquileres y convertir la imagen binaria a BitmapImage
        public static List<AlquilerManager> ConsultarAlquileres()
        {
            List<AlquilerManager> alquileres = new List<AlquilerManager>();
            Conexion conexion = new Conexion();

            try
            {
                conexion.AbrirConexion();
                SqlConnection connection = conexion.ObtenerConexion();

                // Consulta SQL con INNER JOIN para obtener también la ruta de la imagen del patín
                string query = @"
            SELECT ALQ_ID, ALQ_PAT_ID, ALQ_CLI_TELEFONO, ALQ_COST, ALQ_REMAINING_TIME, 
                   ALQ_TIME_UP, ALQ_IS_RUNNING, ALQ_CLI_CREDENCIAL, PAT.PAT_IMAGEN
            FROM ALQUILER AS ALQ
            INNER JOIN PATIN AS PAT ON ALQ.ALQ_PAT_ID = PAT.PAT_ID";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    try
                    {
                        AlquilerManager alquiler = new AlquilerManager();

                        if (!reader.IsDBNull(0)) alquiler.AlquilerId = reader.GetInt32(0);
                        if (!reader.IsDBNull(1)) alquiler.PatinId = reader.GetInt32(1);
                        if (!reader.IsDBNull(2)) alquiler.ClienteTelefono = reader.GetString(2);
                        if (!reader.IsDBNull(3)) alquiler.Costo = reader.GetDouble(3);
                        if (!reader.IsDBNull(4)) alquiler.TiempoRestante = reader.GetInt32(4);
                        if (!reader.IsDBNull(5)) alquiler.HoraFinalizacion = reader.GetDateTime(5);
                        if (!reader.IsDBNull(6)) alquiler.EstaEnCurso = reader.GetBoolean(6);

                        if (!reader.IsDBNull(7))
                        {
                            byte[] imageData = (byte[])reader[7];
                            alquiler.CredencialImagen = ConvertirBytesAImagen(imageData);
                        }

                        // Obtener la ruta de la imagen del patín (campo PAT_IMAGEN)
                        if (!reader.IsDBNull(8))
                        {
                            string rutaImagenPatin = reader.GetString(8);
                            alquiler.PatinImagen = ObtenerImagenPatin(rutaImagenPatin);  // Método que convierte la ruta a BitmapImage
                        }

                        alquileres.Add(alquiler);
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine($"Error al procesar un alquiler: {ex.Message}");
                        continue;
                    }
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al consultar los alquileres: {ex.Message}");
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return alquileres;
        }

        public static BitmapImage ObtenerImagenPatin(string ruta)
        {
            if (string.IsNullOrEmpty(ruta)) return null;

            try
            {
                // Aquí convertimos la ruta a una imagen
                BitmapImage bitmap = new BitmapImage(new Uri(ruta, UriKind.RelativeOrAbsolute));
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la imagen del patín: {ex.Message}");
                return null;
            }
        }

        // Método para convertir los bytes en una imagen BitmapImage
        public static BitmapImage ConvertirBytesAImagen(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir los bytes a imagen: {ex.Message}");
                return null;
            }
        }


       


    }
}
