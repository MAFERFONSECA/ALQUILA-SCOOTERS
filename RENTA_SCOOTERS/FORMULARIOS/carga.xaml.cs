using Microsoft.Win32;
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
using System.Windows.Threading;

namespace ALQUILER_SCOOTERS.FORMULARIOS
{
    public partial class carga : Window
    {
        private DispatcherTimer timer;
        private DateTime startTime;
        private bool isLoginShown = false; // Variable para controlar si el login se ha mostrado

        public carga()
        {
            InitializeComponent();
            Loaded += Grid_Loaded;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            StartProgressBar();
        }

        private void StartProgressBar()
        {
            ProgressBar.Value = 0;
            startTime = DateTime.Now;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100); // Actualización cada 100 ms
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - startTime).TotalSeconds;
            if (elapsed >= 5)
            {
                timer.Stop();
                ProgressBar.Value = 100;

                if (!isLoginShown) // Mostrar el formulario de login solo una vez
                {
                    isLoginShown = true;

                    var loginForm = new login();
                    loginForm.Show();

                    // Usar Dispatcher para cerrar la ventana actual de manera segura
                    this.Dispatcher.InvokeAsync(() => this.Close());
                }
            }
            else
            {
                ProgressBar.Value = (elapsed / 5) * 100;
            }
        }
    }

}
