using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace RENTA_SCOOTERS.CLASES
{
    public class Usuario
    {
        public int USU_ID { get; set; }
        public string USU_NOMBRE { get; set; }
        public string USU_CONTRASENA { get; set; }

        private Conexion miConexion;

        public Usuario()
        {
            miConexion = new Conexion();
        }

        // Método para guardar un nuevo usuario
        public void GuardarUsuario(string nombre, string contrasena)
        {
            miConexion.AbrirConexion();
            try
            {
                string query = $"INSERT INTO USUARIO (USU_NOMBRE, USU_CONTRASENA) VALUES ('{nombre}', '{contrasena}')";
                SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el usuario: {ex.Message}");
            }
            finally
            {
                miConexion.CerrarConexion();
            }
        }

        // Método para eliminar un usuario
        public void EliminarUsuario(int idUsuario)
        {
            miConexion.AbrirConexion();
            try
            {
                string query = $"DELETE FROM USUARIO WHERE USU_ID = {idUsuario}";
                SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario: {ex.Message}");
            }
            finally
            {
                miConexion.CerrarConexion();
            }
        }

        // Método para obtener todos los usuarios
        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> usuarios = new List<Usuario>();
            miConexion.AbrirConexion();
            try
            {
                string query = "SELECT USU_ID, USU_NOMBRE, USU_CONTRASENA FROM USUARIO";
                SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion());
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usuarios.Add(new Usuario
                    {
                        USU_ID = reader.GetInt32(0),
                        USU_NOMBRE = reader.GetString(1),
                        USU_CONTRASENA = reader.GetString(2)
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
            }
            finally
            {
                miConexion.CerrarConexion();
            }
            return usuarios;
        }

        // Método para modificar un usuario
        public void ModificarUsuario(int idUsuario, string nuevoNombre, string nuevaContrasena)
        {
            miConexion.AbrirConexion();
            try
            {
                string query = $"UPDATE USUARIO SET USU_NOMBRE = '{nuevoNombre}', USU_CONTRASENA = '{nuevaContrasena}' WHERE USU_ID = {idUsuario}";
                SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar el usuario: {ex.Message}");
            }
            finally
            {
                miConexion.CerrarConexion();
            }
        }



        public bool VerificarUsuario(string nombre, string contrasena)
        {
            bool esValido = false;
            miConexion.AbrirConexion();
            try
            {
                string query = $"SELECT COUNT(*) FROM USUARIO WHERE USU_NOMBRE = @nombre AND USU_CONTRASENA = @contrasena";
                SqlCommand cmd = new SqlCommand(query, miConexion.ObtenerConexion());
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                esValido = count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar usuario: {ex.Message}");
            }
            finally
            {
                miConexion.CerrarConexion();
            }
            return esValido;
        }






    }
}

