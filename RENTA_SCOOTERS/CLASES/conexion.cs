using System;
using System.Data.SqlClient;

namespace RENTA_SCOOTERS.CLASES
{
    internal class Conexion
    { 
        private SqlConnection con;

        public void AbrirConexion()
        {
            try
            {
                string connectionString = "Data Source=MSIMAFER;Initial Catalog=SCOOTERS2;Integrated Security=True";
                con = new SqlConnection(connectionString);
                con.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al abrir la conexión: " + ex.Message);
            }
        }

        public void CerrarConexion()
        {
            if (con != null && con.State == System.Data.ConnectionState.Open)
            {
                con.Close();
            }
        }

        public SqlConnection ObtenerConexion()
        {
            return con;
        }
    }
}

