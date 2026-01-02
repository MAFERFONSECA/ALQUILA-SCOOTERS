using RENTA_SCOOTERS.CLASES;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;

public class Minutos
{
    public int MinId { get; set; }
    public int MinTiempo { get; set; }
    public float MinCosto { get; set; }

    public Minutos(int minId, int minTiempo, float minCosto)
    {
        MinId = minId;
        MinTiempo = minTiempo;
        MinCosto = minCosto;
    }

    // Método para obtener los minutos desde la base de datos
    public static List<Minutos> ObtenerMinutosDesdeBaseDeDatos()
    {
        List<Minutos> minutosList = new List<Minutos>();

        // Crear una instancia de la clase Conexion
        Conexion conexion = new Conexion();

        // Abrir la conexión a la base de datos
        conexion.AbrirConexion();

        try
        {
            // Obtener la conexión abierta
            SqlConnection con = conexion.ObtenerConexion();

            // Definir la consulta SQL para obtener los minutos
            string query = "SELECT MIN_ID, MIN_TIEMPO, MIN_COSTO FROM MINUTOS";

            // Crear el SqlCommand
            SqlCommand command = new SqlCommand(query, con);

            // Crear el DataTable
            DataTable dataTable = new DataTable();

            // Llenar el DataTable usando un SqlDataAdapter
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
            {
                dataAdapter.Fill(dataTable);
            }

            // Recorrer los resultados y agregar los minutos a la lista
            foreach (DataRow row in dataTable.Rows)
            {
                minutosList.Add(new Minutos(
                    Convert.ToInt32(row["MIN_ID"]),
                    Convert.ToInt32(row["MIN_TIEMPO"]),
                    Convert.ToSingle(row["MIN_COSTO"])
                ));
            }
        }
        catch (Exception ex)
        {
            // Manejar el error de conexión
            Console.WriteLine("Error al obtener los minutos: " + ex.Message);
        }
        finally
        {
            // Cerrar la conexión al final
            conexion.CerrarConexion();
        }

        return minutosList;
    }
}

