using RENTA_SCOOTERS;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using RENTA_SCOOTERS.CLASES;

namespace ALQUILER_SCOOTERS.FORMULARIOS
{
    /// <summary>
    /// Lógica de interacción para login.xaml
    /// </summary>
    /// FUNCIONAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

    public partial class login : Window
    {
        private Usuario usuario;

        public login()
        {
            InitializeComponent();
            usuario = new Usuario();
        }

        // Evento para manejar el botón Ingresar
        private void INGRESAR_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = TXTUSUARIO.Text.Trim();
            string contrasenaUsuario = TXTCONTRASEÑA.Password.Trim();

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasenaUsuario))
            {
                MessageBox.Show("Por favor, complete todos los campos", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (usuario.VerificarUsuario(nombreUsuario, contrasenaUsuario))
            {
                MessageBox.Show("Inicio de sesión exitoso", "Bienvenido", MessageBoxButton.OK, MessageBoxImage.Information);
                // Redirige a la ventana principal
                paginaprincipal main = new paginaprincipal();
                main.Show();
                this.Close();
            }
            else
            {
                // Mostrar mensaje de error
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Limpiar los campos de usuario y contraseña
                TXTUSUARIO.Clear();  // Limpiar el TextBox de usuario
                TXTCONTRASEÑA.Clear();  // Limpiar el PasswordBox de contraseña
            }
        }


        // Métodos para TXTUSUARIO (Campo de Usuario)
        private void TXTUSUARIO_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceholderUsuario.Visibility == Visibility.Visible)
                PlaceholderUsuario.Visibility = Visibility.Hidden;
        }

        private void TXTUSUARIO_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TXTUSUARIO.Text))
                PlaceholderUsuario.Visibility = Visibility.Visible;
        }

        // Métodos para TXTCONTRASEÑA (Campo de Contraseña)
        private void TXTCONTRASEÑA_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PlaceholderContrasena.Visibility == Visibility.Visible)
                PlaceholderContrasena.Visibility = Visibility.Hidden;
        }

        private void TXTCONTRASEÑA_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TXTCONTRASEÑA.Password))
                PlaceholderContrasena.Visibility = Visibility.Visible;
        }
    }
}
