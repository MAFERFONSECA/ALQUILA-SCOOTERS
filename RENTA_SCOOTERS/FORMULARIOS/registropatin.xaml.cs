using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using RENTA_SCOOTERS.FORMULARIOS;
using ALQUILER_SCOOTERS.FORMULARIOS;
using RENTA_SCOOTERS.CLASES; // Asegúrate de tener el namespace correcto para la clase patin
using System.Linq;

namespace RENTA_SCOOTERS.FORMULARIOS
{
    public partial class registropatin : Window

    {
        private string rutaImagen = ""; // Guardar la ruta de la imagen seleccionada
        private patin nuevoPatin; // Instancia de la clase patin
        public int SelectedPatinId { get; set; } // Propiedad para almacenar el ID del patín seleccionado

        public registropatin()
        {
            InitializeComponent();
            nuevoPatin = new patin(); // Inicializar la clase patin
        }

        private void CargarImagen_Click(object sender, RoutedEventArgs e)
        {
            // Crear el diálogo para seleccionar un archivo
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                rutaImagen = openFileDialog.FileName;

                // Cargar la imagen seleccionada y mostrarla en el control Image
                imagenPatin.Source = new BitmapImage(new Uri(rutaImagen));
            }
        }

        // Método para guardar y modificar el scooter
        // En el formulario donde se guarda o modifica el patín
        private void GuardarScooter_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si se ingresó el nombre del patín
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Por favor, llena todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Obtener el nombre del patín
            string nombre = nameTextBox.Text;

            try
            {
                // Verificar si hay un patín seleccionado para modificar
                if (SelectedPatinId > 0)
                {
                    // Comprobar si hay una imagen nueva seleccionada
                    string imagenParaModificar = string.IsNullOrEmpty(rutaImagen) ? imagenPatin.Source.ToString() : rutaImagen;

                    // Validar que la imagen esté presente (si la imagen no ha cambiado y ya existe una imagen cargada, no es necesario que se cargue una nueva)
                    if (string.IsNullOrEmpty(imagenParaModificar) || imagenParaModificar.Contains("pack://application:,,,")) // Verifica si la imagen es válida
                    {
                        MessageBox.Show("Por favor, cargue una imagen del patín para modificar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Modificar el patín existente
                    nuevoPatin.ModificarPatin(SelectedPatinId, nombre, imagenParaModificar);
                    MessageBox.Show("Scooter modificado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Guardar un nuevo patín
                    if (string.IsNullOrEmpty(rutaImagen))
                    {
                        MessageBox.Show("Por favor, cargue una imagen del patín para guardar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    nuevoPatin.GuardarPatin(nombre, rutaImagen);
                    MessageBox.Show("Scooter registrado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Limpiar los campos después de guardar/modificar
                LimpiarCampos();
                SelectedPatinId = 0; // Resetear el ID del patín seleccionado

                // Obtener una referencia al formulario principal
                var formularioPrincipal = Application.Current.Windows.OfType<paginaprincipal>().FirstOrDefault();

                // Verificar si la referencia al formulario principal es válida
                if (formularioPrincipal != null)
                {
                    // Llamar al método para actualizar la lista de patines en el formulario principal
                    formularioPrincipal.ActualizarListaPatines();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar/modificar el scooter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // Método para eliminar el scooter
        private void EliminarScooter_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si se ha seleccionado un patín
            if (SelectedPatinId <= 0)
            {
                MessageBox.Show("Por favor, seleccione un patín antes de eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Llamar al método para eliminar el patín
                nuevoPatin.EliminarPatin(SelectedPatinId);
                MessageBox.Show("Scooter eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpiar el campo después de eliminar
                LimpiarCampos();
                SelectedPatinId = 0; // Resetear el ID del patín seleccionado
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el scooter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Método para buscar el scooter
        private void BuscarScooter_Click(object sender, RoutedEventArgs e)
        {
            BuscarPatin buscarPatinWindow = new BuscarPatin(this); // Pasar la referencia de la ventana actual
            bool? resultado = buscarPatinWindow.ShowDialog(); // Muestra la ventana como un diálogo modal

            // Si se seleccionó un patín, cargar los datos
            if (resultado == true)
            {
                // Verificar si el ID seleccionado es válido
                if (SelectedPatinId > 0) // Asegúrate de tener la lógica para comprobar esto
                {
                    // Cargar el patín seleccionado
                    var patinSeleccionado = nuevoPatin.ObtenerPatines().Find(p => p.PAT_ID == SelectedPatinId);
                    if (patinSeleccionado != null)
                    {
                        nameTextBox.Text = patinSeleccionado.PAT_NOMBRE; // Asigna el nombre al textbox
                        if (!string.IsNullOrEmpty(patinSeleccionado.PAT_IMAGEN))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(patinSeleccionado.PAT_IMAGEN);
                            bitmap.EndInit();
                            imagenPatin.Source = bitmap; // Asigna la imagen al control
                        }
                        // Asignar la ruta de la imagen para uso posterior
                        rutaImagen = patinSeleccionado.PAT_IMAGEN; // Asignar la ruta de la imagen
                    }
                }
            }
        }
        // Método para limpiar los campos de entrada
        private void LimpiarCampos()
        {
            nameTextBox.Text = "";
            imagenPatin.Source = null;
            rutaImagen = "";
        }

        private void paginaprincipal_Click(object sender, RoutedEventArgs e)
        {
            paginaprincipal ventanapatin = new paginaprincipal
            {
                Owner = this  // Establece la ventana actual como dueña
            };
            ventanapatin.Show();
            this.Close();
        }
    }
}
