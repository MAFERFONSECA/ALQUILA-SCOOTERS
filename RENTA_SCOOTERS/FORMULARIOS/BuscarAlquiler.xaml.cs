using RENTA_SCOOTERS.CLASES;
using System;
using System.Collections.Generic;
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
using ALQUILER_SCOOTERS.FORMULARIOS;  // Añadir esta línea si no está ya presente
using CrystalDecisions.CrystalReports.Engine; // Para trabajar con los ReportDocument
using SAPBusinessObjects.WPF.Viewer;  // Para trabajar con CrystalReportsViewer en WPF




namespace RENTA_SCOOTERS.FORMULARIOS
{
    /// <summary>
    /// Lógica de interacción para BuscarAlquiler.xaml
    /// </summary>
    public partial class BuscarAlquiler : Window
    {


        public BuscarAlquiler()
        {
            InitializeComponent();
            CargarAlquileres();
        }

        private void CargarAlquileres()
        {
            // Llamada al método estático para obtener los alquileres
            List<AlquilerManager> alquileres = AlquilerManager.ConsultarAlquileres();

            // Asignar la lista de alquileres al DataGrid
            dataGridAlquileres.ItemsSource = alquileres;
        }

        private void volverButton_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar la ventana de registro de patín
            paginaprincipal ventanapatin = new paginaprincipal
            {
                Owner = this  // Establece la ventana actual como dueña
            };
            ventanapatin.Show();
            this.Close(); // Cierra la ventana de búsqueda

        }



        private void buscaralquiler_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filtro = buscaralquiler.Text.Trim(); // Captura el texto del TextBox

            if (string.IsNullOrWhiteSpace(filtro))
            {
                // Si el TextBox está vacío, muestra todos los datos
                dataGridAlquileres.ItemsSource = AlquilerManager.ConsultarAlquileres();
            }
            else
            {
                // Aplica un filtro basado en números
                var listaFiltrada = AlquilerManager.ConsultarAlquileres()
                    .Where(alquiler =>
                        alquiler.HoraFinalizacion.ToString().Contains(filtro) || // Busca en las fechas
                        alquiler.Costo.ToString().Contains(filtro) ||            // Busca en los costos
                        alquiler.TiempoRestante.ToString().Contains(filtro)      // Busca en el tiempo restante
                    )
                    .ToList();

                // Actualiza el DataGrid con la lista filtrada
                dataGridAlquileres.ItemsSource = listaFiltrada;
            }
        }



        private void PdfButton_Click(object sender, RoutedEventArgs e)
        {
            string ruta = @"C:\Users\mafre\source\repos\RENTA_SCOOTERS\RENTA_SCOOTERS\FORMULARIOS\repalquileres.rpt";
            FORMULARIOS.reportealquiler X = new FORMULARIOS.reportealquiler(ruta);
            X.Show();
        }





    }
}


    

