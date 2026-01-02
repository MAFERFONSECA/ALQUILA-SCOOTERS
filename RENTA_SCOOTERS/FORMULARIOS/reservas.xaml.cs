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

namespace ALQUILER_SCOOTERS.FORMULARIOS
{
    /// <summary>
    /// Lógica de interacción para reservas.xaml
    /// </summary>
    public partial class reservas : Window
    {
        public List<Scooter> AvailableScooters { get; set; }
        public List<Reservation> Reservations { get; set; }

        public reservas()
        {
            InitializeComponent();
            InitializeScooterListBox();
            Reservations = new List<Reservation>();
            this.Closing += Reservas_Closing;
        }

        private void Reservas_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // Cancela el cierre de la ventana
            this.Hide();     // Oculta la ventana en lugar de cerrarla
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide(); // Oculta la ventana cuando se hace clic en el botón de cierre personalizado
        }

        private void InitializeScooterListBox()
        {
            // Esto debería llenarse con la lista de scooters disponibles
            AvailableScooters = new List<Scooter>
            {
                new Scooter { Name = "BIRD1", ImagePath = "pack://application:,,,/Images/BIRD1.png" },
                new Scooter { Name = "BIRD2", ImagePath = "pack://application:,,,/Images/BIRD2.png" },
                new Scooter { Name = "BIRD3", ImagePath = "pack://application:,,,/Images/BIRD3.png" },
                new Scooter { Name = "BIRDNEGRO", ImagePath = "pack://application:,,,/Images/BIRDNEGRO.png" },
                new Scooter { Name = "BIRD DELANTERO", ImagePath = "pack://application:,,,/Images/BIRDDELANTERO.png" },
                new Scooter { Name = "Gotrax", ImagePath = "pack://application:,,,/Images/Gotrax.png" },
                new Scooter { Name = "Bici", ImagePath = "pack://application:,,,/Images/Bici.png" },
                new Scooter { Name = "Chico Verde", ImagePath = "pack://application:,,,/Images/ChicoVerde.png" },
                new Scooter { Name = "Chico Rosa", ImagePath = "pack://application:,,,/Images/ChicoRosa.png" },
                new Scooter { Name = "RAZOR AZUL", ImagePath = "pack://application:,,,/Images/RAZORAZUL.png" }
            };

            scooterListBox.ItemsSource = AvailableScooters;
            scooterListBox.DisplayMemberPath = "Name";
        }

        private void ReserveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(nameTextBox.Text) &&
                !string.IsNullOrWhiteSpace(phoneTextBox.Text) &&
                scooterListBox.SelectedItems.Count > 0 &&
                startDatePicker.SelectedDate.HasValue &&
                !string.IsNullOrWhiteSpace(startTimeTextBox.Text) &&
                durationComboBox.SelectedItem is ComboBoxItem selectedDurationItem)
            {
                var selectedScooters = new List<string>();
                foreach (Scooter scooter in scooterListBox.SelectedItems)
                {
                    selectedScooters.Add(scooter.Name);
                }

                var durationText = selectedDurationItem.Content.ToString();
                var duration = GetDurationFromText(durationText);

                DateTime startDateTime;
                if (!DateTime.TryParse($"{startDatePicker.SelectedDate.Value:yyyy-MM-dd} {startTimeTextBox.Text}", out startDateTime))
                {
                    MessageBox.Show("Por favor, ingresa una hora de inicio válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var endDateTime = startDateTime.AddMinutes(duration);

                var reservation = new Reservation
                {
                    ClientName = nameTextBox.Text,
                    Phone = phoneTextBox.Text,
                    ScooterNames = string.Join(", ", selectedScooters),
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                    Duration = durationText
                };

                Reservations.Add(reservation);
                reservationsListBox.Items.Add(reservation);
            }
            else
            {
                MessageBox.Show("Por favor, completa todos los campos correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetDurationFromText(string durationText)
        {
            switch (durationText)
            {
                case "15 minutos":
                    return 15;
                case "30 minutos":
                    return 30;
                case "45 minutos":
                    return 45;
                case "1 hora":
                    return 60;
                case "1 hora y 15 minutos":
                    return 75;
                case "1 hora y 30 minutos":
                    return 90;
                default:
                    return 0;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Reservation reservation)
            {
                Reservations.Remove(reservation);
                reservationsListBox.Items.Remove(reservation);
            }
        }

        public class Scooter
        {
            public string Name { get; set; }
            public string ImagePath { get; set; }
        }

        public class Reservation
        {
            public string ClientName { get; set; }
            public string Phone { get; set; }
            public string ScooterNames { get; set; }
            public DateTime StartDateTime { get; set; }
            public DateTime EndDateTime { get; set; }
            public string Duration { get; set; }
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
    }
}
