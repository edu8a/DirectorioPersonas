using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DirectorioWPFClient.Models;
using DirectorioWPFClient.Services;

namespace DirectorioWPFClient
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;
        private readonly ApiService _apiService;

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7245/api/")
            };
            _apiService = new ApiService();
            CargarPersonasAsync();
            CargarFacturasAsync();
        }
        private async Task CargarFacturasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("Facturas");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var facturas = JsonSerializer.Deserialize<List<Factura>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                FacturasDataGrid.ItemsSource = facturas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        // cargar facturas por PersonaId
        private async Task CargarFacturasPorPersonaAsync(int personaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Facturas/{personaId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var facturas = JsonSerializer.Deserialize<List<Factura>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    FacturasDataGrid.ItemsSource = facturas;
                }
                else
                {
                    MessageBox.Show($"No se encontraron facturas para la persona con ID: {personaId}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    FacturasDataGrid.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // buscar facturas por PersonaId
        private async void BuscarFacturasPorPersonaButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuscarPersonaIdTextBox.Text))
            {
                MessageBox.Show("Por favor, ingrese un ID de persona válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (int.TryParse(BuscarPersonaIdTextBox.Text, out int personaId))
            {
                await CargarFacturasPorPersonaAsync(personaId);
            }
            else
            {
                MessageBox.Show("El ID de persona debe ser un número.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async void BuscarPersonaButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BuscarIdentificacionTextBox.Text))
            {
                MessageBox.Show("Por favor, ingrese una identificación válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string identificacion = BuscarIdentificacionTextBox.Text;

            try
            {
                var response = await _httpClient.GetAsync($"Personas/{identificacion}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var persona = JsonSerializer.Deserialize<Persona>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (persona != null)
                    {
                        PersonaInfoTextBlock.Text = $"ID: {persona.Id}\nNombre: {persona.Nombre}\nApellido Paterno: {persona.ApellidoPaterno}\nApellido Materno: {persona.ApellidoMaterno}\nIdentificación: {persona.Identificacion}";
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    PersonaInfoTextBlock.Text = "No se encontró una persona con la identificación proporcionada.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al buscar la persona: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





        // crear una nueva persona
        private async void CrearButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NombreTextBox.Text) ||
                string.IsNullOrWhiteSpace(ApellidoPaternoTextBox.Text) ||
                string.IsNullOrWhiteSpace(IdentificacionTextBox.Text))
            {
                MessageBox.Show("Los campos Nombre, Apellido Paterno e Identificación son obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var nuevaPersona = new Persona
                {
                    Nombre = NombreTextBox.Text,
                    ApellidoPaterno = ApellidoPaternoTextBox.Text,
                    ApellidoMaterno = string.IsNullOrWhiteSpace(ApellidoMaternoTextBox.Text) ? null : ApellidoMaternoTextBox.Text,
                    Identificacion = IdentificacionTextBox.Text
                };

                var resultado = await _apiService.CreatePersonaAsync(nuevaPersona);

                if (resultado)
                {
                    MessageBox.Show("Persona creada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormulario();
                    await CargarPersonasAsync();
                }
                else
                {
                    MessageBox.Show("Error al crear la persona. Verifique si la identificación ya existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // cargar lista de personas
        private async Task CargarPersonasAsync()
        {
            try
            {
                var personas = await _apiService.GetPersonasAsync();
                PersonasDataGrid.ItemsSource = personas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar personas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // eliminar persona seleccionada
        private async void EliminarButton_Click(object sender, RoutedEventArgs e)
        {
            if (PersonasDataGrid.SelectedItem is Persona personaSeleccionada)
            {
                try
                {
                    var resultado = await _apiService.DeletePersonaAsync(personaSeleccionada.Identificacion);

                    if (resultado)
                    {
                        MessageBox.Show("Persona eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                        // actualizar el DataGrid de personas
                        await CargarPersonasAsync();

                        // actualizar el DataGrid de facturas
                        await CargarFacturasAsync();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar la persona. Verifique si la persona existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecciona una persona para eliminar.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // cambiarde pestaña
        private void CambiarAPestañaFacturas(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 1;
        }

        // crear factura
        private async void CrearFacturaButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PersonaIdTextBox.Text) ||
                FechaDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(MontoTextBox.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var nuevaFactura = new
                {
                    PersonaId = int.Parse(PersonaIdTextBox.Text),
                    Fecha = FechaDatePicker.SelectedDate.Value,
                    Monto = decimal.Parse(MontoTextBox.Text)
                };

                var json = JsonSerializer.Serialize(nuevaFactura);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Facturas", content);

                if (response.IsSuccessStatusCode)
                {

                    LimpiarFormularioFactura();
                    await CargarFacturasAsync();
                }

                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($" {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void LimpiarFormulario()
        {
            NombreTextBox.Text = string.Empty;
            ApellidoPaternoTextBox.Text = string.Empty;
            ApellidoMaternoTextBox.Text = string.Empty;
            IdentificacionTextBox.Text = string.Empty;
        }

     
        private void LimpiarFormularioFactura()
        {
            PersonaIdTextBox.Text = string.Empty;
            FechaDatePicker.SelectedDate = null;
            MontoTextBox.Text = string.Empty;
        }
    }
}
