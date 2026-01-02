using RENTA_SCOOTERS.CLASES;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ALQUILER_SCOOTERS.FORMULARIOS;

namespace RENTA_SCOOTERS.FORMULARIOS
{
    /// <summary>
    /// Lógica de interacción para editarminutos.xaml
    /// </summary>
    public partial class editarminutos : Window
    {
        private Conexion conexion; // Instancia de la clase de conexión
        private int idSeleccionado; // ID del registro seleccionado

        public editarminutos()
        {
            InitializeComponent();
            conexion = new Conexion();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                conexion.AbrirConexion();
                using (SqlCommand command = new SqlCommand("SELECT MIN_ID, MIN_TIEMPO, MIN_COSTO FROM MINUTOS", conexion.ObtenerConexion()))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable tabla = new DataTable();
                    adapter.Fill(tabla);

                    dataGrid.ItemsSource = tabla.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }

        private void dataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is DataRowView filaSeleccionada)
            {
                idSeleccionado = Convert.ToInt32(filaSeleccionada["MIN_ID"]);
                txtMinutos.Text = filaSeleccionada["MIN_TIEMPO"].ToString();
                txtCosto.Text = filaSeleccionada["MIN_COSTO"].ToString();
            }
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtMinutos.Text, out int minutos) || !float.TryParse(txtCosto.Text, out float costo))
            {
                MessageBox.Show("Por favor, introduce valores válidos en las cajas de texto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                conexion.AbrirConexion();

                if (idSeleccionado == 0)
                {
                    // Insertar un nuevo registro
                    using (SqlCommand command = new SqlCommand("INSERT INTO MINUTOS (MIN_TIEMPO, MIN_COSTO) VALUES (@MIN_TIEMPO, @MIN_COSTO)", conexion.ObtenerConexion()))
                    {
                        command.Parameters.AddWithValue("@MIN_TIEMPO", minutos);
                        command.Parameters.AddWithValue("@MIN_COSTO", costo);

                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Nuevo registro guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            CargarDatos(); // Recargar datos
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo guardar el nuevo registro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    // Actualizar un registro existente
                    using (SqlCommand command = new SqlCommand("UPDATE MINUTOS SET MIN_TIEMPO = @MIN_TIEMPO, MIN_COSTO = @MIN_COSTO WHERE MIN_ID = @MIN_ID", conexion.ObtenerConexion()))
                    {
                        command.Parameters.AddWithValue("@MIN_ID", idSeleccionado);
                        command.Parameters.AddWithValue("@MIN_TIEMPO", minutos);
                        command.Parameters.AddWithValue("@MIN_COSTO", costo);

                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Registro actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            CargarDatos(); // Recargar datos
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el registro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar la operación: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }




        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Selecciona un registro primero para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este registro?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    conexion.AbrirConexion();
                    using (SqlCommand command = new SqlCommand("DELETE FROM MINUTOS WHERE MIN_ID = @MIN_ID", conexion.ObtenerConexion()))
                    {
                        command.Parameters.AddWithValue("@MIN_ID", idSeleccionado);

                        int filasEliminadas = command.ExecuteNonQuery();

                        if (filasEliminadas > 0)
                        {
                            MessageBox.Show("Registro eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            CargarDatos(); // Recargar datos en el DataGrid
                            LimpiarCampos();
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el registro.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    conexion.CerrarConexion();
                }
            }
        }




        private void LimpiarCampos()
        {
            txtMinutos.Text = string.Empty;
            txtCosto.Text = string.Empty;
            idSeleccionado = 0;
        }

        private void volverButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is paginaprincipal)
                {
                    window.Show(); // Muestra la ventana principal existente
                    break;
                }
            }

            this.Close(); // Cierra la ventana actual
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cancelar el cierre predeterminado
            e.Cancel = true;

            // Mostrar la página principal
            paginaprincipal ventanapatin = new paginaprincipal();
            ventanapatin.Show();

            // Ocultar esta ventana
            this.Hide();
        }

    }
}
