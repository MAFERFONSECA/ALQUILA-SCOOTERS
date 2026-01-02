using ALQUILER_SCOOTERS.FORMULARIOS;
using CrystalDecisions.CrystalReports.Engine;
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

namespace RENTA_SCOOTERS.FORMULARIOS
{
    /// <summary>
    /// Lógica de interacción para reportealquiler.xaml
    /// </summary>
    public partial class reportealquiler : Window
    {
        public reportealquiler()
        {
            InitializeComponent();
        }

        private string path;

        public reportealquiler(string path)
        {
            InitializeComponent();
            this.path = path;
            carga();
        }
        private void carga() {
            ReportDocument rd = new ReportDocument();
            rd.Load(this.path);
            rd.Refresh();
            crystalReportViewer.ViewerCore.ReportSource = rd;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ReportDocument rd = new ReportDocument();
            rd.Load(this.path);
            rd.Refresh();
            crystalReportViewer.ViewerCore.ReportSource = rd;
        }

        private void volverButton_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar la ventana de registro de patín
            BuscarAlquiler ventanapatin = new BuscarAlquiler
            {
                Owner = this  // Establece la ventana actual como dueña
            };
            ventanapatin.Show();
            this.Close(); // Cierra la ventana de búsqueda
        }
    }
}
