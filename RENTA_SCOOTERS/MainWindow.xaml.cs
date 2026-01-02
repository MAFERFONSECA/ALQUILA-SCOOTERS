using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;

namespace RENTA_SCOOTERS
{
    public partial class MainWindow : Window
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        public MainWindow()
        {
            InitializeComponent();
            CargarDispositivos();
        }

        private void CargarDispositivos()
        {
            // Cargar las cámaras disponibles
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(Video_NewFrame);
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("No se encontraron cámaras.");
            }
        }

        private async void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Clonar el frame recibido en un bitmap
            using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                if (bitmap != null)
                {
                    try
                    {
                        // Convertir Bitmap a BitmapImage para mostrarlo en WPF
                        BitmapImage bitmapImage = await ConvertirBitmapAImagenAsync(bitmap);

                        // Usar el Dispatcher para acceder al control Image en el hilo de la UI
                        await imgPreview.Dispatcher.InvokeAsync(() =>
                        {
                            imgPreview.Source = bitmapImage;
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al mostrar la imagen: {ex.Message}");
                    }
                }
            }
        }

        private Task<BitmapImage> ConvertirBitmapAImagenAsync(Bitmap bitmap)
        {
            return Task.Run(() =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    // Guardar el bitmap en un MemoryStream
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    memoryStream.Position = 0; // Restablecer la posición del stream

                    // Inicializar BitmapImage
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze(); // Congelar para uso seguro en hilos
                }
                return bitmapImage;
            });
        }

        private void btnTomarFoto_Click(object sender, RoutedEventArgs e)
        {
            if (imgPreview.Source != null)
            {
                // Obtener el frame actual como BitmapSource
                BitmapSource bitmapSource = (BitmapSource)imgPreview.Source;

                // Convertir BitmapSource a Bitmap y guardarla
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                // Guardar la imagen capturada en un archivo
                using (var stream = new System.IO.FileStream("foto_capturada.png", System.IO.FileMode.Create))
                {
                    encoder.Save(stream);
                }

                MessageBox.Show("Foto guardada.");
            }
            else
            {
                MessageBox.Show("No hay imagen para capturar.");
            }
        }

        private void btnEliminarFoto_Click(object sender, RoutedEventArgs e)
        {
            imgPreview.Source = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
            base.OnClosed(e);
        }
    }
}
