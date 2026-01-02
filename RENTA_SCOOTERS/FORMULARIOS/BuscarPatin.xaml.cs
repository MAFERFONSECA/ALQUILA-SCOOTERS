using ALQUILER_SCOOTERS.FORMULARIOS;
using RENTA_SCOOTERS.CLASES;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RENTA_SCOOTERS.FORMULARIOS
{
    public partial class BuscarPatin : Window
    {
        private patin patinModel;
        private registropatin registropatinWindow; // Referencia a la ventana de registro de patin

        public BuscarPatin(registropatin registropatin)
        {
            InitializeComponent();
            registropatinWindow = registropatin; // Inicializa la referencia
            patinModel = new patin();
            CargarPatines();
        }

        private void CargarPatines()
        {
            var patines = patinModel.ObtenerPatines(); // Supón que este método obtiene los patines desde la base de datos
            dataGridPatines.ItemsSource = patines;
        }

        private void seleccionarButton_Click(object sender, RoutedEventArgs e)
        {
            var patinSeleccionado = (patin)dataGridPatines.SelectedItem; // Obtener el patín seleccionado

            if (patinSeleccionado != null)
            {
                // Asignar el ID y nombre del patín seleccionado a la ventana de registro
                registropatinWindow.SelectedPatinId = patinSeleccionado.PAT_ID; // Asigna el ID del patín seleccionado
                registropatinWindow.nameTextBox.Text = patinSeleccionado.PAT_NOMBRE; // Llenar el textbox de nombre

                // Cargar la imagen en el control Image si la ruta es válida
                if (!string.IsNullOrEmpty(patinSeleccionado.PAT_IMAGEN))
                {
                    // Verificar si la ruta es válida antes de intentar cargar la imagen
                    Uri imageUri = new Uri(patinSeleccionado.PAT_IMAGEN, UriKind.RelativeOrAbsolute);
                    if (imageUri.IsFile || imageUri.IsAbsoluteUri)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = imageUri; // Usar la ruta de la imagen
                        bitmap.EndInit();
                        registropatinWindow.imagenPatin.Source = bitmap; // Asignar la imagen al control en la ventana de registro
                    }
                    else
                    {
                        MessageBox.Show("La imagen no se pudo cargar porque la ruta es inválida.", "Error de Imagen", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Este patín no tiene imagen asignada.", "Sin Imagen", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.Close(); // Cierra la ventana de búsqueda
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un patín.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void volverButton_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar la ventana de registro de patín
            registropatin ventanapatin = new registropatin
            {
                Owner = this  // Establece la ventana actual como dueña
            };
            ventanapatin.Show();
            this.Close(); // Cierra la ventana de búsqueda
        }
    }
}
