using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RENTA_SCOOTERS.CLASES
{
    public class patin
    {
        public int PAT_ID { get; set; }
        public string PAT_NOMBRE { get; set; }
        public string PAT_IMAGEN { get; set; }

        public bool PAT_IS_AVAILABLE { get; set; }

        private readonly Conexion miConexion;

        public patin()
        {
            miConexion = new Conexion();
        }

        // Método para guardar un nuevo patín
        public void GuardarPatin(string nombre, string imagen)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(imagen))
            {
                throw new ArgumentException("El nombre y la imagen no pueden estar vacíos.");
            }

            miConexion.AbrirConexion();
            try
            {
                string query = "INSERT INTO PATIN (PAT_NOMBRE, PAT_IMAGEN, PAT_IS_AVAILABLE) VALUES (@nombre, @imagen, @isAvailable)";
                using (SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@imagen", imagen);
                    cmd.Parameters.AddWithValue("@isAvailable", 1); // Establecer la disponibilidad como 'disponible' por defecto
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el patín: {ex.Message}");
                throw;
            }
            finally
            {
                miConexion.CerrarConexion();
            }
        }


        // Método para eliminar un patín
        public void EliminarPatin(int idPatin)
        {
            miConexion.AbrirConexion();
            try
            {
                string query = "DELETE FROM PATIN WHERE PAT_ID = @id";
                using (SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@id", idPatin);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el patín: {ex.Message}");
                throw;
            }
            finally
            {
                miConexion.CerrarConexion();
            }
        }

        // Método para obtener todos los patines
        public List<patin> ObtenerPatines()
        {
            List<patin> patines = new List<patin>();
            miConexion.AbrirConexion();
            try
            {
                string query = "SELECT PAT_ID, PAT_NOMBRE, PAT_IMAGEN FROM PATIN";
                using (SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion()))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patines.Add(new patin
                            {
                                PAT_ID = reader.GetInt32(0),
                                PAT_NOMBRE = reader.GetString(1),
                                PAT_IMAGEN = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener patines: {ex.Message}");
                throw;
            }
            finally
            {
                miConexion.CerrarConexion();
            }
            return patines;
        }

        // Método para modificar un patín
        public void ModificarPatin(int idPatin, string nuevoNombre, string nuevaImagen)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre) || string.IsNullOrWhiteSpace(nuevaImagen))
            {
                throw new ArgumentException("El nuevo nombre y la nueva imagen no pueden estar vacíos.");
            }

            miConexion.AbrirConexion();
            try
            {
                string query = "UPDATE PATIN SET PAT_NOMBRE = @nuevoNombre, PAT_IMAGEN = @nuevaImagen WHERE PAT_ID = @id";
                using (SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@nuevoNombre", nuevoNombre);
                    cmd.Parameters.AddWithValue("@nuevaImagen", nuevaImagen);
                    cmd.Parameters.AddWithValue("@id", idPatin);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar el patín: {ex.Message}");
                throw;
            }
            finally
            {
                miConexion.CerrarConexion();
            }
        }




        // Método para obtener los patines disponibles (sin alquiler activo)
        public List<patin> ObtenerPatinesDisponibles()
        {
            List<patin> patinesDisponibles = new List<patin>();
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();

            try
            {
                // Consulta para obtener los patines que no tienen alquiler activo
                string consulta = @"
            SELECT P.PAT_ID, P.PAT_NOMBRE, P.PAT_IMAGEN
            FROM PATIN P
            WHERE NOT EXISTS (SELECT 1 FROM ALQUILER A WHERE A.ALQ_PAT_ID = P.PAT_ID AND A.ALQ_IS_RUNNING = 1)"; // Verifica que no haya alquileres activos

                using (SqlCommand cmd = new SqlCommand(consulta, conexion.ObtenerConexion()))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        patin patin = new patin
                        {
                            PAT_ID = Convert.ToInt32(reader["PAT_ID"]),
                            PAT_NOMBRE = reader["PAT_NOMBRE"].ToString(),
                            PAT_IMAGEN = reader["PAT_IMAGEN"].ToString()
                        };

                        patinesDisponibles.Add(patin);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener los patines disponibles: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return patinesDisponibles;
        }





        public void CambiarDisponibilidad(bool disponibilidad)
        {
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();

            try
            {
                string consulta = "UPDATE PATIN SET PAT_IS_AVAILABLE = @disponibilidad WHERE PAT_ID = @patinId";

                using (SqlCommand cmd = new SqlCommand(consulta, conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@disponibilidad", disponibilidad ? 1 : 0);
                    cmd.Parameters.AddWithValue("@patinId", this.PAT_ID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cambiar la disponibilidad del patín: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }






    }
}
