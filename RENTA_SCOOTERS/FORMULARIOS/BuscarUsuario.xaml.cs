using RENTA_SCOOTERS.CLASES;
using System.Collections.Generic;
using System.Windows;

namespace RENTA_SCOOTERS.FORMULARIOS
{
    public partial class BuscarUsuario : Window
    {
        private Usuario usuarioModel;
        private usuario usuarioWindow;

        public BuscarUsuario(usuario usuario)
        {
            InitializeComponent();
            usuarioWindow = usuario;
            usuarioModel = new Usuario();
            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            var usuarios = usuarioModel.ObtenerUsuarios();
            dataGridUsuarios.ItemsSource = usuarios;
        }

        private void seleccionarButton_Click(object sender, RoutedEventArgs e)
        {
            var usuarioSeleccionado = (Usuario)dataGridUsuarios.SelectedItem;

            if (usuarioSeleccionado != null)
            {
                usuarioWindow.SelectedUserId = usuarioSeleccionado.USU_ID; // Asigna el ID del usuario seleccionado
                usuarioWindow.nameTextBox.Text = usuarioSeleccionado.USU_NOMBRE; // Llenar el textbox de nombre
                usuarioWindow.passwordTextBox.Text = usuarioSeleccionado.USU_CONTRASENA; // Llenar el textbox de contraseña
                this.Close(); // Cierra la ventana de búsqueda
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void volverButton_Click(object sender, RoutedEventArgs e)
        {
            usuario ventanausuario = new usuario
            {
                Owner = this  // Establece la ventana actual como dueña
            };
            ventanausuario.Show();
            this.Close();
        }
    }
}

