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

using System.Configuration;

using System.Data.SqlClient;

namespace HOLACHICOS {
    /// <summary>
    /// Lógica de interacción para Window1.xaml (Ventana de Inicio de Sesión)
    /// </summary>
    public partial class Window1 : Window {
        public Window1() {
            InitializeComponent();
        }

        // =========================================================
        // FUNCIÓN: Maneja el clic en el botón "ojito" (Mostrar/Ocultar Contraseña)
        // =========================================================
        private void btnShowPassword_Click(object sender, RoutedEventArgs e) {
            // Lógica para mostrar/ocultar contraseña
            if (txtPasswordHidden.Visibility == Visibility.Visible) {
                txtPasswordVisible.Text = txtPasswordHidden.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPasswordHidden.Visibility = Visibility.Collapsed;
                btnShowPassword.Content = "🙈";
            }
            else {
                txtPasswordHidden.Password = txtPasswordVisible.Text;
                txtPasswordHidden.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                btnShowPassword.Content = "👁";
            }
        }

        // =========================================================
        // FUNCIÓN: Lógica para el botón de Iniciar Sesión
        // =========================================================
        private void Login_Click(object sender, RoutedEventArgs e) {
            string email = txtEmail.Text;
            string password = txtPasswordHidden.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
                MessageBox.Show("Por favor, introduce tu email y contraseña.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Aquí iría la lógica de inicio de sesión real (ej. API, Base de Datos, etc.)
            // Si el login es exitoso:
            MessageBox.Show("¡Inicio de sesión exitoso! Bienvenido a CBA.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            // new HomeWindow().Show(); // Navegar a la ventana principal de la aplicación
            // this.Close(); 
        }

        // =========================================================
        // FUNCIÓN: Maneja el clic en "Sign Up" para cambiar a la ventana de Registro (MainWindow)
        // =========================================================
        private void NavigateToRegister_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            // Crea y muestra la ventana de Registro
            MainWindow registerWindow = new MainWindow();
            registerWindow.Show();

            // Cierra la ventana actual (Login)
            this.Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPasswordHidden.Password.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, introduce tu email y contraseña.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                string connStr = ConfigurationManager.ConnectionStrings["CbaConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Revisamos si existe el usuario
                    string query = "SELECT COUNT(*) FROM Usuarios WHERE CORREO=@correo AND CONTRASEÑA=@pass";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", email);
                        cmd.Parameters.AddWithValue("@pass", password);

                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("¡Inicio de sesión exitoso! Bienvenido a CBA.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            // Si no existe, lo guardamos automáticamente
                            SaveUserToDatabase(email, password);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveUserToDatabase(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, completa los campos.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["CbaConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Verificamos si ya existe el correo
                    string checkQuery = "SELECT COUNT(*) FROM Usuarios WHERE CORREO=@correo";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@correo", email);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Este correo ya está registrado.", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    // Insertar nuevo usuario
                    string query = "INSERT INTO Usuarios (CORREO, CONTRASEÑA) VALUES (@correo, @pass)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", email);
                        cmd.Parameters.AddWithValue("@pass", password);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            MessageBox.Show($"Usuario guardado correctamente con el correo: {email}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo guardar el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}