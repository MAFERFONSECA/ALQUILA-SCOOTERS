using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Data;


namespace RENTA_SCOOTERS.CLASES
{
    public class Alquiler
    {
        public int AlquilerId { get; set; }
        public int PatinId { get; set; }
        public string ClienteTelefono { get; set; }
        public byte[] CredencialImagen { get; set; }
        public int MinutosId { get; set; }
        public float Costo { get; set; }
        public int TiempoRestante { get; set; }
        public DateTime HoraFinalizacion { get; set; }
        public bool EstaEnCurso { get; set; }
        public bool TimeUp { get; set; }
        public Brush BackgroundColor => TiempoRestante <= 0 ? Brushes.Red : Brushes.Transparent;
        public string NombrePatin { get; set; }
        public string ImagenPatin { get; set; } // Aquí se almacenará la ruta de la imagen


        public bool IsFinalizado { get; set; } // Nueva propiedad


        public bool RegistrarAlquiler()
        {
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();
            try
            {
                // Verificación de parámetros nulos o vacíos antes de proceder
                if (CredencialImagen == null || CredencialImagen.Length == 0)
                {
                    Console.WriteLine("La imagen de la credencial está vacía o nula.");
                    return false;
                }

                using (SqlCommand cmd = new SqlCommand("INSERT INTO ALQUILER (ALQ_PAT_ID, ALQ_CLI_TELEFONO, ALQ_CLI_CREDENCIAL, ALQ_MIN_ID, ALQ_COST, ALQ_REMAINING_TIME, ALQ_TIME_UP, ALQ_IS_RUNNING) VALUES (@PatinId, @Telefono, @Credencial, @MinutosId, @Costo, @TiempoRestante, @HoraFinalizacion, @EstaEnCurso)", conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@PatinId", PatinId);
                    cmd.Parameters.AddWithValue("@Telefono", ClienteTelefono);
                    cmd.Parameters.AddWithValue("@Credencial", CredencialImagen);
                    cmd.Parameters.AddWithValue("@MinutosId", MinutosId);
                    cmd.Parameters.AddWithValue("@Costo", Costo);
                    cmd.Parameters.AddWithValue("@TiempoRestante", TiempoRestante);
                    cmd.Parameters.AddWithValue("@HoraFinalizacion", HoraFinalizacion);
                    cmd.Parameters.AddWithValue("@EstaEnCurso", EstaEnCurso);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Alquiler registrado exitosamente.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No se insertaron filas en la base de datos.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al registrar el alquiler: " + ex.Message);
                Console.WriteLine("Detalles de la excepción: " + ex.ToString());  // Para más detalles sobre el error
                return false;
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }



        public List<Alquiler> ObtenerAlquileresActivos()
        {
            List<Alquiler> alquileres = new List<Alquiler>();
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();

            try
            {
                // Consulta SQL ajustada para obtener los campos necesarios, incluyendo los ID de alquiler y patín
                string consulta = @"
        SELECT 
            A.ALQ_ID, 
            A.ALQ_PAT_ID, 
            P.PAT_NOMBRE, 
            P.PAT_IMAGEN, 
            A.ALQ_COST, 
            A.ALQ_REMAINING_TIME
        FROM ALQUILER A
        INNER JOIN PATIN P ON A.ALQ_PAT_ID = P.PAT_ID
        WHERE A.ALQ_IS_RUNNING = 1"; // solo alquileres activos

                using (SqlCommand cmd = new SqlCommand(consulta, conexion.ObtenerConexion()))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Alquiler alquiler = new Alquiler
                        {
                            // Ahora se incluyen los identificadores ALQ_ID y ALQ_PAT_ID
                            AlquilerId = Convert.ToInt32(reader["ALQ_ID"]), // Agregar AlquilerId
                            PatinId = Convert.ToInt32(reader["ALQ_PAT_ID"]), // Agregar PatinId
                            NombrePatin = reader["PAT_NOMBRE"].ToString(),
                            ImagenPatin = reader["PAT_IMAGEN"].ToString(), // Guardamos la ruta de la imagen
                            Costo = reader["ALQ_COST"] == DBNull.Value ? 0f : Convert.ToSingle(reader["ALQ_COST"]),
                            TiempoRestante = reader["ALQ_REMAINING_TIME"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ALQ_REMAINING_TIME"])
                        };

                        alquileres.Add(alquiler);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los alquileres: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return alquileres;
        }







        public BitmapImage ImagenPatinBitmap
        {
            get
            {
                if (!string.IsNullOrEmpty(ImagenPatin))
                {
                    try
                    {
                        return new BitmapImage(new Uri(ImagenPatin, UriKind.Absolute));
                    }
                    catch
                    {
                        // Manejar el caso en que la ruta sea incorrecta
                        return null;
                    }
                }
                return null;
            }
        }







        public bool FinalizarAlquiler(int alquilerId)
        {
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();
            try
            {
                // Verificación si el alquiler existe
                string consultaVerificacion = "SELECT COUNT(*) FROM ALQUILER WHERE ALQ_ID = @AlquilerId";
                using (SqlCommand cmdVerificacion = new SqlCommand(consultaVerificacion, conexion.ObtenerConexion()))
                {
                    cmdVerificacion.Parameters.AddWithValue("@AlquilerId", alquilerId);
                    int count = (int)cmdVerificacion.ExecuteScalar();
                    if (count == 0)
                    {
                        Console.WriteLine("No se encontró el alquiler con el ID proporcionado.");
                        return false;
                    }
                }

                // Actualización de la columna ALQ_IS_RUNNING
                using (SqlCommand cmd = new SqlCommand("UPDATE ALQUILER SET ALQ_IS_RUNNING = @EstaEnCurso WHERE ALQ_ID = @AlquilerId", conexion.ObtenerConexion()))
                {
                    cmd.Parameters.Add("@EstaEnCurso", SqlDbType.Bit).Value = false;  // Cambia a 0 (false)
                    cmd.Parameters.AddWithValue("@AlquilerId", alquilerId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Alquiler finalizado exitosamente.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No se encontró el alquiler especificado o no se actualizó el registro.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al finalizar el alquiler: " + ex.Message);
                return false;
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }




        public void DisminuirTiempoRestante()
        {
            if (TiempoRestante > 0)
            {
                TiempoRestante--; // Disminuir el tiempo localmente
                ActualizarTiempoRestanteBD(); // Reflejar el cambio en la base de datos
            }
            else
            {
                EstaEnCurso = false;
                TimeUp = true; // El tiempo ha terminado
            }
        }

        private void ActualizarTiempoRestanteBD()
        {
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE ALQUILER SET ALQ_REMAINING_TIME = @TiempoRestante WHERE ALQ_ID = @AlquilerId", conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@TiempoRestante", TiempoRestante);
                    cmd.Parameters.AddWithValue("@AlquilerId", AlquilerId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el tiempo restante: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }



    }
}
