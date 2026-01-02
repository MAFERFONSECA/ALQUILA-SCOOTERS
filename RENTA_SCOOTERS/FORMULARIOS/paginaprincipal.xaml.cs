using AForge.Video;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Data.SqlClient;
using ALQUILER_SCOOTERS;
using RENTA_SCOOTERS.FORMULARIOS;
using RENTA_SCOOTERS.CLASES;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using System.IO;
using System.Data.SqlTypes;
using System.Windows.Media;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;



namespace ALQUILER_SCOOTERS.FORMULARIOS
{
    /// <summary>
    /// Lógica de interacción para paginaprincipal.xaml
    /// </summary>
    /// 



    public class TimeRemainingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int remainingTime && remainingTime <= 0)
            {
                // Si el tiempo restante es 0 o menor, cambia a color rojo
                return new SolidColorBrush(Colors.Red);
            }
            // Si no, retorna un color verde (o cualquier otro color que desees)
            return new SolidColorBrush(Colors.Green);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // No es necesario implementar ConvertBack si no planeas usarlo
            throw new NotImplementedException();
        }
    }


    public partial class paginaprincipal : Window
    {

        private DispatcherTimer timer = new DispatcherTimer();
        private List<Alquiler> alquileresActivos = new List<Alquiler>();


        public ObservableCollection<Alquiler> AlquileresActivos { get; set; } = new ObservableCollection<Alquiler>();

        private List<Minutos> minutosList;


        private const double ratePer15Minutes = 50.0;


        private int selectedMinutes = 0;
        private SoundPlayer soundPlayer;
        private Mapa _mapaWindow;
        private FORMULARIOS.reservas reservasWindow;









        // Declarar el temporizador a nivel de clase
        private DispatcherTimer temporizador;

        private void IniciarTemporizador()
        {
            temporizador = new DispatcherTimer();
            temporizador.Interval = TimeSpan.FromSeconds(1); // Actualizar cada segundo
            temporizador.Tick += Temporizador_Tick;
            temporizador.Start();
        }
        private void Temporizador_Tick(object sender, EventArgs e)
        {
            // Crear una instancia de la clase Alquiler
            Alquiler alquilerInstance = new Alquiler();

            // Obtener los alquileres activos
            List<Alquiler> alquileresActivos = alquilerInstance.ObtenerAlquileresActivos();

            foreach (var alquiler in alquileresActivos)
            {
                // Reducir el tiempo restante
                if (alquiler.TiempoRestante > 0)
                {
                    alquiler.TiempoRestante--;

                    // Actualizar en la base de datos
                    ActualizarTiempoRestanteEnBD(alquiler.AlquilerId, alquiler.TiempoRestante);
                }
                else if (!alquiler.TimeUp) // Si el tiempo llega a 0 por primera vez
                {
                    // Marcar el alquiler como finalizado
                    alquiler.TimeUp = true;

                    // Reproducir el sonido antes de cualquier mensaje o advertencia
                    ReproducirSonido();

                    // Mostrar advertencia de que el alquiler ha finalizado
                    MostrarAdvertenciaFinalizado(alquiler);

                    // Cambiar el color de la interfaz (si usas bindings o actualizaciones visuales)
                    RefrescarListaDeAlquileres();
                }
            }

            // Refrescar la lista de alquileres en la interfaz al final
            RefrescarListaDeAlquileres();
        }

        private void ReproducirSonido()
        {
            try
            {
                // Ruta del archivo de sonido
                SoundPlayer soundPlayer = new SoundPlayer("C:\\Users\\mafre\\source\\repos\\RENTA_SCOOTERS\\RENTA_SCOOTERS\\Resources\\advertencia.wav");
                soundPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al reproducir el sonido: " + ex.Message);
            }
        }

        private void MostrarAdvertenciaFinalizado(Alquiler alquiler)
        {
            // Mostrar el mensaje de advertencia (por ejemplo, con un MessageBox)
            MessageBox.Show($"El alquiler de {alquiler.NombrePatin} ha finalizado.");

            // Usar un DispatcherTimer para eliminar el alquiler de la lista después de 3 segundos
            DispatcherTimer advertenciaTimer = new DispatcherTimer();
            advertenciaTimer.Interval = TimeSpan.FromSeconds(1); // 3 segundos
            advertenciaTimer.Tick += (sender, e) =>
            {
                // Lógica para quitar la advertencia y finalmente eliminar el alquiler de la lista si es necesario
                advertenciaTimer.Stop(); // Detener el temporizador

                // Finalizar alquiler (podrías guardar esto en la base de datos si es necesario)
                alquiler.FinalizarAlquiler(alquiler.AlquilerId);

                // Eliminar el alquiler de la lista de alquileres activos
                alquileresActivos.Remove(alquiler);

                // Refrescar la lista de alquileres en la interfaz
                RefrescarListaDeAlquileres();
            };
            advertenciaTimer.Start();
        }





        private void ActualizarTiempoRestanteEnBD(int alquilerId, int tiempoRestante)
        {
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE ALQUILER SET ALQ_REMAINING_TIME = @TiempoRestante WHERE ALQ_ID = @AlquilerId", conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@TiempoRestante", tiempoRestante);
                    cmd.Parameters.AddWithValue("@AlquilerId", alquilerId);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el tiempo restante: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion();
            }
        }







        public paginaprincipal()
        {
            InitializeComponent();
            InitializeScooterList();
            DataContext = this;

            // Inicializa el Timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);  // Configura el timer para que se ejecute cada segundo
            timer.Tick += Timer_Tick;  // Llama al método Timer_Tick cada vez que el timer se ejecute
            timer.Start();  // Inicia el timer

            // Inicializar el reproductor de sonido
            soundPlayer = new SoundPlayer("C:\\Users\\YORDI VALDEZ\\Downloads\\alert.wav");
            //soundPlayer = new SoundPlayer("C:\\Users\\Jairo Alberto Valdez\\Downloads\\alert.wav");
            this.Closing += new CancelEventHandler(Window_Closing);

            CargarMinutosDesdeBaseDeDatos();
            // En el constructor o en el método de inicialización:
            DataContext = this; // Enlazamos el DataContext de la ventana
            CargarAlquileresActivos();
            CargarScootersActivos();


            // Inicializa la colección de alquileres
            AlquileresActivos = new ObservableCollection<Alquiler>();

            // Configura el ListBox
            selectedScootersListBox.ItemsSource = AlquileresActivos;

            // Carga los alquileres activos
            CargarAlquileresActivos();
            ActualizarVentaTotal();




            IniciarTemporizador();




        }






        private void ActualizarVentaTotal()
        {
            DateTime fechaHoy = DateTime.Today; // Obtiene la fecha de hoy sin la parte de la hora
            double ventaTotalHoy = 0;

            // Instancia la clase Conexion y abre la conexión
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();

            string query = "SELECT SUM(ALQ_COST) FROM ALQUILER WHERE CONVERT(DATE, ALQ_TIME_UP) = @Fecha";

            try
            {
                using (SqlCommand command = new SqlCommand(query, conexion.ObtenerConexion()))
                {
                    // Añadimos el parámetro para la fecha
                    command.Parameters.AddWithValue("@Fecha", fechaHoy.Date);

                    // Ejecutamos la consulta y obtenemos el resultado
                    var result = command.ExecuteScalar();

                    if (result != DBNull.Value)
                    {
                        ventaTotalHoy = Convert.ToDouble(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al ejecutar la consulta: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion(); // Cierra la conexión
            }

            // Actualiza el TextBlock con el resultado de la venta total
            totalCostTextBlock.Text = $"${ventaTotalHoy:F2}"; // Formatea a dos decimales
        }









        private void CargarScootersActivos()
        {
            Alquiler alquiler = new Alquiler();
            selectedScootersListBox.ItemsSource = alquiler.ObtenerAlquileresActivos();
        }

        private void CargarAlquileresActivos()
        {
            try
            {
                AlquileresActivos.Clear();

                // Crea una instancia de Alquiler
                Alquiler alquilerInstance = new Alquiler();

                // Llama al método a través de la instancia
                var alquileres = alquilerInstance.ObtenerAlquileresActivos();

                foreach (var alquiler in alquileres)
                {
                    AlquileresActivos.Add(alquiler);
                }

                Console.WriteLine("Alquileres activos cargados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar alquileres activos: {ex.Message}");
            }
        }









        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtiene el botón que fue clickeado
                var button = sender as Button;
                if (button == null)
                {
                    Console.WriteLine("Error: El botón es nulo.");
                    return;
                }

                // Obtiene el objeto de tipo Alquiler desde el DataContext del botón
                var alquiler = button.DataContext as Alquiler;
                if (alquiler == null)
                {
                    Console.WriteLine("Error: El DataContext no contiene un objeto válido de tipo Alquiler.");
                    return;
                }

                // Muestra una ventana de confirmación para eliminar el alquiler
                MessageBoxResult result = MessageBox.Show(
                    $"¿Estás seguro de que deseas eliminar el alquiler del patín {alquiler.NombrePatin}?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Si el usuario confirma la eliminación
                if (result == MessageBoxResult.Yes)
                {
                    Console.WriteLine($"El usuario confirmó eliminar el patín: {alquiler.NombrePatin}");

                    // Busca el ID del alquiler asociado al patín
                    int alquilerId = ObtenerAlquilerIdPorPatinId(alquiler.PatinId);

                    if (alquilerId == -1)
                    {
                        Console.WriteLine($"No se encontró un alquiler asociado al patín con ID {alquiler.PatinId}.");
                        return;
                    }

                    Console.WriteLine($"Intentando finalizar el alquiler con ID: {alquilerId}");
                    Alquiler alquilerObj = new Alquiler();
                    bool finalizado = alquilerObj.FinalizarAlquiler(alquilerId);

                    if (finalizado)
                    {
                        Console.WriteLine($"El alquiler con ID {alquilerId} fue finalizado correctamente.");

                        // Remueve el objeto del ListBox y actualiza la colección
                        AlquileresActivos.Remove(alquiler);  // Remueve de la ObservableCollection

                        // Actualiza la interfaz de usuario
                        selectedScootersListBox.ItemsSource = null; // Desvincula temporalmente el ItemsSource
                        selectedScootersListBox.ItemsSource = AlquileresActivos; // Vuelve a asignar la colección

                        // Actualiza el costo total
                        ActualizarVentaTotal();  // Llamada para actualizar el costo total en el TextBlock


                    }
                    else
                    {
                        Console.WriteLine($"No se pudo finalizar el alquiler con ID {alquilerId}.");
                    }
                }
                else
                {
                    Console.WriteLine("El usuario canceló la eliminación.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }







        private int ObtenerAlquilerIdPorPatinId(int patinId)
        {
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ALQ_ID FROM ALQUILER WHERE ALQ_PAT_ID = @PatinId AND ALQ_IS_RUNNING = 1", conexion.ObtenerConexion()))
                {
                    cmd.Parameters.AddWithValue("@PatinId", patinId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int alquilerId))
                    {
                        Console.WriteLine($"Alquiler encontrado para PatinId = {patinId}: ALQ_ID = {alquilerId}");
                        return alquilerId;
                    }
                    else
                    {
                        Console.WriteLine($"No se encontró un alquiler para PatinId = {patinId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar AlquilerId para el patín con ID {patinId}: {ex.Message}");
            }
            finally
            {
                conexion.CerrarConexion();
            }

            return -1; // Devuelve -1 si no se encontró el alquiler
        }















        private void Telefonotextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Permite solo números
            e.Handled = !char.IsDigit(e.Text, 0);
        }


        private void CargarMinutosDesdeBaseDeDatos()
        {
            minutosList = Minutos.ObtenerMinutosDesdeBaseDeDatos();

            // Limpiar el ComboBox antes de agregar datos
            timeComboBox.Items.Clear();

            // Agregar cada objeto Minutos al ComboBox
            foreach (Minutos minuto in minutosList)
            {
                // Agregar el tiempo como texto
                timeComboBox.Items.Add($"{minuto.MinTiempo} minutos");
            }

            // Seleccionar el primer valor por defecto
            if (timeComboBox.Items.Count > 0)
                timeComboBox.SelectedIndex = 0;
        }



        private Minutos ObtenerMinutosSeleccionados()
        {
            if (timeComboBox.SelectedItem is string selectedText)
            {
                // Extraer el tiempo en minutos
                int tiempoSeleccionado = int.Parse(selectedText.Replace(" minutos", ""));

                // Buscar el objeto relacionado en la lista
                return minutosList.FirstOrDefault(m => m.MinTiempo == tiempoSeleccionado);
            }

            return null;
        }





        private float ObtenerCostoSeleccionado()
        {
            if (timeComboBox.SelectedItem != null)
            {
                // Obtener el texto seleccionado y eliminar la palabra "minutos"
                string textoSeleccionado = timeComboBox.SelectedItem.ToString();
                int tiempoSeleccionado = int.Parse(textoSeleccionado.Replace(" minutos", "").Trim());

                // Buscar el MinCosto basado en el MIN_TIEMPO seleccionado
                Minutos minutoSeleccionado = minutosList.FirstOrDefault(m => m.MinTiempo == tiempoSeleccionado);

                if (minutoSeleccionado != null)
                {
                    return minutoSeleccionado.MinCosto; // Retorna el costo asociado al MIN_TIEMPO seleccionado
                }
            }

            return 0f;
        }



        public void ActualizarListaPatines()
        {
            InitializeScooterList();  // Recargar la lista de patines
        }
        private void InitializeScooterList()
        {
            List<patin> patines = new List<patin>();

            // Usar la clase Conexion para abrir la conexión
            Conexion conexion = new Conexion();
            conexion.AbrirConexion();

            // Consulta SQL para obtener los patines desde la base de datos
            string query = "SELECT PAT_ID, PAT_NOMBRE, PAT_IMAGEN FROM PATIN";

            try
            {
                using (SqlCommand command = new SqlCommand(query, conexion.ObtenerConexion()))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);  // PAT_ID
                            string nombre = reader.GetString(1);  // PAT_NOMBRE

                            // La columna PAT_IMAGEN es un string (ruta de la imagen)
                            string imagenPath = reader["PAT_IMAGEN"] as string;

                            // Verifica si la ruta de la imagen es válida
                            if (!string.IsNullOrEmpty(imagenPath))
                            {
                                // Si la ruta no está vacía, agrega el patín a la lista
                                patines.Add(new patin { PAT_ID = id, PAT_NOMBRE = nombre, PAT_IMAGEN = imagenPath });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los patines desde la base de datos: " + ex.Message);
            }
            finally
            {
                conexion.CerrarConexion();
            }

            // Asignar la lista de patines al ListBox
            scooterListBox.ItemsSource = patines;
        }




        private void TimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (timeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                try
                {
                    if (selectedItem.Tag.ToString() == "Other")
                    {
                        customAmountTextBox.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        selectedMinutes = selectedItem.Tag.ToString() == "Promo" ? 60 : int.Parse(selectedItem.Tag.ToString());
                        customAmountTextBox.Visibility = Visibility.Collapsed;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("El formato del tiempo seleccionado no es correcto.", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }







        private void RefrescarListaDeAlquileres()
        {
            // Crear una instancia de la clase Alquiler
            Alquiler alquiler = new Alquiler();

            // Llamar al método ObtenerAlquileresActivos
            List<Alquiler> alquileresActivos = alquiler.ObtenerAlquileresActivos();

            // Actualizar el ListBox
            selectedScootersListBox.ItemsSource = alquileresActivos;
            selectedScootersListBox.Items.Refresh();
        }











        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Verificar que se haya seleccionado un scooter
            if (scooterListBox.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un scooter para alquilar.");
                return;
            }

            // Verificar que se haya seleccionado un tiempo
            if (timeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tiempo de alquiler.");
                return;
            }

            // Verificar que se haya ingresado un número de teléfono
            string telefono = telefonotextbox.Text;
            if (string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show("Ingrese un número de teléfono.");
                return;
            }

            // Verificar que se haya ingresado una foto de la credencial
            if (capturedImage.Source == null)
            {
                MessageBox.Show("Debe capturar una imagen de la credencial antes de proceder.");
                return;
            }

            // Obtener datos del scooter seleccionado
            var selectedScooter = (patin)scooterListBox.SelectedItem; // Clase `patin`
            int patinId = selectedScooter.PAT_ID;

            // Crear una instancia de Alquiler
            Alquiler alquilerInstance = new Alquiler();

            // Obtener la lista de alquileres activos usando la instancia
            List<Alquiler> alquileresActivos = alquilerInstance.ObtenerAlquileresActivos();

            // Verificar si el scooter seleccionado está en alquiler
            bool scooterDisponible = alquileresActivos.All(a => a.NombrePatin != selectedScooter.PAT_NOMBRE);

            if (!scooterDisponible)
            {
                MessageBox.Show("El scooter seleccionado ya está en alquiler.");
                return;
            }

            // Obtener el texto seleccionado del ComboBox y buscar el objeto Minutos relacionado
            string selectedTimeText = timeComboBox.SelectedItem.ToString(); // Ejemplo: "10 minutos"
            int tiempoSeleccionado = int.Parse(selectedTimeText.Replace(" minutos", ""));
            Minutos selectedMinutes = minutosList.FirstOrDefault(m => m.MinTiempo == tiempoSeleccionado);

            if (selectedMinutes == null)
            {
                MessageBox.Show("Error al obtener los datos del tiempo seleccionado.");
                return;
            }

            int minutosId = selectedMinutes.MinId;
            float costo = selectedMinutes.MinCosto;

            // Convertir la imagen de la credencial a binario
            byte[] credencialImagen = ConvertirImagenABinario(capturedImage.Source);

            // Calcular el tiempo restante y la hora de finalización
            int tiempoRestante = selectedMinutes.MinTiempo * 60; // Convertir minutos a segundos
            DateTime horaFinalizacion = DateTime.Now.AddMinutes(selectedMinutes.MinTiempo);

            // Crear una instancia de Alquiler y asignar valores
            Alquiler alquiler = new Alquiler
            {
                PatinId = patinId,
                ClienteTelefono = telefono,
                CredencialImagen = credencialImagen,
                MinutosId = minutosId,
                Costo = costo,
                TiempoRestante = tiempoRestante,
                HoraFinalizacion = horaFinalizacion,
                EstaEnCurso = true
            };

            // Registrar el alquiler
            if (alquiler.RegistrarAlquiler())
            {
                MessageBox.Show("Alquiler registrado exitosamente.");

                // Refrescar la lista de alquileres activos
                RefrescarListaDeAlquileres();

                // Actualizar el TextBlock con el costo total de ventas del día
                ActualizarVentaTotal();

                // Limpiar todos los campos
                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show("Error al registrar el alquiler.");
            }
        }





        // Método para limpiar el formulario
        private void LimpiarFormulario()
        {
            // Deseleccionar scooter
            scooterListBox.SelectedItem = null;

            // Deseleccionar tiempo
            timeComboBox.SelectedItem = null;

            // Limpiar campo de teléfono
            telefonotextbox.Text = string.Empty;

            // Limpiar imagen capturada
            capturedImage.Source = null;

            // Opcional: Restablecer el enfoque al primer control
            scooterListBox.Focus();
        }



        private byte[] ConvertirImagenABinario(ImageSource imageSource)
        {
            if (imageSource is BitmapSource bitmapSource)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                using (MemoryStream stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            return null;
        }




        private void abrirmapa_Click(object sender, RoutedEventArgs e)
        {
            registropatin ventanapatin = new registropatin();
            // Mostrar la ventana de usuarios
            ventanapatin.Show();
        }



        private void abrirusuarios_Click(object sender, RoutedEventArgs e)
        {
            usuario ventanaUsuarios = new usuario();
            // Mostrar la ventana de usuarios
            ventanaUsuarios.Show();

        }
        private void TakePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var takePhotoForm = new tomarfoto();
            takePhotoForm.ShowDialog();  // Muestra la ventana como diálogo modal

            if (takePhotoForm.CapturedImage != null)
            {
                // Asignar la imagen capturada al control Image en el formulario principal
                capturedImage.Source = takePhotoForm.CapturedImage;
            }
            else
            {
                MessageBox.Show("No se capturó ninguna imagen.");
            }
        }

        private void tomarfoto_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si la cámara está disponible
            if (IsCameraAvailable())
            {
                // Si la cámara está disponible, abre la ventana
                tomarfoto ventanatomarfoto = new tomarfoto();
                ventanatomarfoto.ShowDialog(); // Usamos ShowDialog para bloquear la ventana principal hasta que se cierre la ventana de la cámara

                // Después de cerrar la ventana de la cámara, verificar si se capturó una imagen
                if (ventanatomarfoto.CapturedImage != null)
                {
                    // Asignar la imagen capturada al control Image en la ventana principal
                    capturedImage.Source = ventanatomarfoto.CapturedImage;
                }
                else
                {
                    MessageBox.Show("No se capturó ninguna imagen.");
                }
            }
            else
            {
                // Si no hay cámara disponible, muestra un mensaje
                MessageBox.Show("Por favor, enciende la cámara.", "Cámara no disponible", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Método para verificar si hay una cámara disponible
        private bool IsCameraAvailable()
        {
            // Obtener todos los dispositivos de captura de video
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // Si hay al menos un dispositivo de video (cámara), entonces la cámara está disponible
            return videoDevices.Count > 0;
        }





        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (Alquiler alquiler in selectedScootersListBox.Items)
            {
                if (alquiler.TiempoRestante > 0)
                {
                    alquiler.TiempoRestante--; // Decrementar un segundo
                }

                if (alquiler.TiempoRestante <= 0 && alquiler.EstaEnCurso)
                {
                    alquiler.EstaEnCurso = false; // Actualizar estado de alquiler
                    soundPlayer.Play(); // Reproducir sonido cuando el tiempo se agote
                }
            }

            selectedScootersListBox.Items.Refresh(); // Actualizar la vista para reflejar los cambios
        }



        private void AddSelectedScootersToList()
        {
            var selectedScooters = scooterListBox.SelectedItems.Cast<Scooter>().ToList();
            double cost = 0;
            TimeSpan rentalTime;
            string rentalTimeText = "";

            if (customAmountTextBox.Visibility == Visibility.Visible)
            {
                if (double.TryParse(customAmountTextBox.Text, out double customAmount))
                {
                    cost = customAmount * selectedScooters.Count;
                    rentalTime = TimeSpan.FromMinutes(customAmount / ratePer15Minutes * 15);
                }
                else
                {
                    MessageBox.Show("Por favor, ingresa una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                if (selectedMinutes == 20)
                {
                    cost = 50.0 * selectedScooters.Count;
                }
                else if (selectedMinutes == 60 && timeComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag.ToString() == "Promo")
                {
                    cost = 150.0 * selectedScooters.Count; // Tarifa promocional
                }
                else
                {
                    cost = Math.Ceiling(selectedMinutes / 15.0) * ratePer15Minutes * selectedScooters.Count;
                }
                rentalTime = TimeSpan.FromMinutes(selectedMinutes);

                // Obtener el texto correcto del ComboBoxItem seleccionado
                if (timeComboBox.SelectedItem is ComboBoxItem comboBoxItem)
                {
                    rentalTimeText = comboBoxItem.Content.ToString();
                }
            }

            var newGroup = new SelectedGroup
            {
                SelectedScooters = selectedScooters,
                RentalTime = rentalTimeText,
                Cost = cost,
                RemainingTime = rentalTime,
                TimeUp = false,
                IsRunning = true
            };

            selectedScootersListBox.Items.Insert(0, newGroup);
        }



        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
       

        private void DeselectScootersFromList()
        {
            var selectedScooters = scooterListBox.SelectedItems.Cast<Scooter>().ToList();
            foreach (var scooter in selectedScooters)
            {
                scooterListBox.SelectedItems.Remove(scooter);
            }
        }

        private void UpdateTotalCost()
        {
            double totalCost = 0;
            foreach (SelectedGroup group in selectedScootersListBox.Items)
            {
                totalCost += group.Cost;
            }

            totalCostTextBlock.Text = $"${totalCost:F2}";
        }

        public class Scooter
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }

        public class SelectedGroup : INotifyPropertyChanged
        {
            private TimeSpan _remainingTime;
            private bool _timeUp;

            public List<Scooter> SelectedScooters { get; set; }
            public string RentalTime { get; set; }
            public double Cost { get; set; }
            public TimeSpan RemainingTime
            {
                get => _remainingTime;
                set
                {
                    _remainingTime = value;
                    OnPropertyChanged("RemainingTime");
                }
            }

            public bool TimeUp
            {
                get => _timeUp;
                set
                {
                    _timeUp = value;
                    OnPropertyChanged("TimeUp");
                }
            }
            public bool IsRunning { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

       









        private void MaxMinButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                MaxMinButtonImage.Source = new BitmapImage(new Uri("/Images/maximize.png", UriKind.Relative));
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                MaxMinButtonImage.Source = new BitmapImage(new Uri("/Images/minimize.png", UriKind.Relative));
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Elimina Application.Current.Shutdown() para evitar cerrar la aplicación completa.
        }



        private void GeneratePDFButton_Click(object sender, RoutedEventArgs e)
        {
            BuscarAlquiler ventanapatin = new BuscarAlquiler();
            // Mostrar la ventana de usuarios
            ventanapatin.Show();
        }

        private class Rental
        {
            public string ScooterName { get; set; }
            public string RentalTime { get; set; }
            public double Cost { get; set; }
            public bool TimeUp { get; set; }
        }

        private void cargarImagenButton_Click(object sender, RoutedEventArgs e)
        {
            login ventanapatin = new login(); // Crea una nueva instancia del login
            ventanapatin.Show(); // Muestra la ventana del login
            this.Close(); // Cierra únicamente la ventana actual
        }

        private void minutos_Click(object sender, RoutedEventArgs e)
        {
            editarminutos ventanapatin = new editarminutos(); // Crea una nueva instancia del login
            ventanapatin.Show(); // Muestra la ventana del login
            this.Close(); // Cierra únicamente la ventana actual
        }
    }
}