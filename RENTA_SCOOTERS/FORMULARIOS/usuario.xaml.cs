using ALQUILER_SCOOTERS.FORMULARIOS;
using System;
using System.Windows;
using RENTA_SCOOTERS.CLASES;


namespace RENTA_SCOOTERS.FORMULARIOS
{
    public partial class usuario : Window
    {
        private Usuario usuarioActual;
        public int SelectedUserId { get; set; }

        public usuario()
        {
            InitializeComponent();
            usuarioActual = new Usuario();
        }

        private void guardarusuario_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text) || string.IsNullOrEmpty(passwordTextBox.Text))
            {
                MessageBox.Show("Por favor, llena todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string nombre = nameTextBox.Text;
            string contrasena = passwordTextBox.Text;

            usuarioActual.GuardarUsuario(nombre, contrasena);
            MessageBox.Show("Usuario guardado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            nameTextBox.Clear();
            passwordTextBox.Clear();
        }

        private void eliminarusuario_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUserId <= 0) // Verifica que haya un usuario seleccionado
            {
                MessageBox.Show("Por favor, selecciona un usuario para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            usuarioActual.EliminarUsuario(SelectedUserId);
            MessageBox.Show("Usuario eliminado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            nameTextBox.Clear();
            passwordTextBox.Clear();
        }

        private void abrirBuscarUsuario_Click(object sender, RoutedEventArgs e)
        {
            BuscarUsuario buscarUsuarioWindow = new BuscarUsuario(this);
            buscarUsuarioWindow.ShowDialog(); // Muestra la ventana como un diálogo modal
        }

        private void paginaprincipal_Click(object sender, RoutedEventArgs e)
        {
            paginaprincipal ventanausuario = new paginaprincipal
            {
                Owner = this  // Establece la ventana actual como dueña
            };
            ventanausuario.Show();
            this.Close();
        }
    }
}

