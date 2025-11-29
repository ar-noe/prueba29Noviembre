using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using System.Configuration;
using System.Data.SqlClient;
namespace HOLACHICOS {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml (Ventana de Registro)
    /// </summary>
    public partial class MainWindow : Window {
        DataClassesCbaDataContext dcBD;
        public MainWindow() {
            InitializeComponent();

            string connStr = ConfigurationManager.ConnectionStrings["HOLACHICOS.Properties.Settings.CbaConnectionString"].ConnectionString;

            dcBD = new DataClassesCbaDataContext(connStr);
        }

        // =========================================================
        // FUNCIÓN: Maneja el clic en el botón "ojito" (Mostrar/Ocultar Contraseña)
        // =========================================================
        private void btnShowPassword_Click(object sender, RoutedEventArgs e) {
            if (txtPasswordHidden.Visibility == Visibility.Visible) {
                // Mostrar el TextBox visible
                txtPasswordVisible.Text = txtPasswordHidden.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPasswordHidden.Visibility = Visibility.Collapsed;
                btnShowPassword.Content = "🙈";
            }
            else {
                // Mostrar el PasswordBox oculto
                txtPasswordHidden.Password = txtPasswordVisible.Text;
                txtPasswordHidden.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                btnShowPassword.Content = "👁";
            }
        }

        
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPasswordHidden.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, completa todos los campos para registrarte.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["CbaConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string query = "INSERT INTO Usuarios (CORREO, CONTRASEÑA) VALUES (@correo, @pass)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@correo", email);
                    cmd.Parameters.AddWithValue("@pass", password);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show($"Usuario registrado correctamente con el correo: {email}", "Registro Exitoso", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Abrir ventana de login
                        new Window1().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo registrar el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    // =========================================================
    // FUNCIÓN: Maneja el clic en "Log in" para cambiar a la ventana de Login
    // =========================================================
    private void NavigateToLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            // Crea y muestra la ventana de Login
            Window1 loginWindow = new Window1();
            loginWindow.Show();

            // Cierra la ventana actual (Registro)
            this.Close();
        }

        //private void txtFullName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        //{
        //    string nombre_completo = 
        //    if()
        //}
    }
}