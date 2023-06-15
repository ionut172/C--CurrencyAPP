using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Convertor
{
    public partial class MainPage : ContentPage
    {
        List<object> raspuns = new List<object>();
        List<Moneda> monede = new List<Moneda>();

        public MainPage()
        {
            InitializeComponent();
            InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            try
            {
                string url = "https://www.floatrates.com/daily/eur.json";
                object responseData = await GetData<object>(url);

                if (responseData != null)
                {
                    string jsonString = JsonSerializer.Serialize(responseData);
                    

                    // Parse the JSON string into a dictionary
                    Dictionary<string, object> dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

                    foreach (var item in dataDict)
                    {
                        // Get the values for each currency
                        Dictionary<string, object> currencyData = JsonSerializer.Deserialize<Dictionary<string, object>>(item.Value.ToString());

                        // Create a new Moneda object and populate its properties
                        Moneda moneda = new Moneda
                        {
                            Code = currencyData["code"].ToString(),
                            AlphaCode = currencyData["alphaCode"].ToString(),
                            NumericCode = currencyData["numericCode"].ToString(),
                            Name = currencyData["name"].ToString(),
                            Date = DateTime.Parse(currencyData["date"].ToString())
                        };

                        // Parse the Rate and InverseRate values
                        decimal rate;
                        decimal inverseRate;
                        decimal.TryParse(currencyData["rate"].ToString(), out rate);
                        decimal.TryParse(currencyData["inverseRate"].ToString(), out inverseRate);
                        moneda.Rate = rate;
                        moneda.InverseRate = inverseRate;


                        monede.Add(moneda);
                    }


                    FromCurrencyPicker.ItemsSource = null;
                    FromCurrencyPicker.Items.Clear();
                    ToCurrencyPicker.ItemsSource = null;
                    ToCurrencyPicker.Items.Clear();


                    foreach (var moneda in monede)
                    {
                        string eticheta = moneda.Name + " - " + moneda.Rate;
                        FromCurrencyPicker.Items.Add(eticheta + moneda.AlphaCode);
                        ToCurrencyPicker.Items.Add(eticheta);

                    }


                    FromCurrencyPicker.SelectedItem = FromCurrencyPicker.Items[0];
                    ToCurrencyPicker.SelectedItem = ToCurrencyPicker.Items[0];




                }
                else
                {
                    await DisplayAlert("NULL", "Data is null", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Obiecte", ex.ToString(), "Inchide");
            }
        }



        public static async Task<T> GetData<T>(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseBody);
        }

        private async void OnConvertClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
        
            try
            {
                string selectedFromCurrency = FromCurrencyPicker.SelectedItem.ToString();
                string selectedToCurrency = ToCurrencyPicker.SelectedItem.ToString();

                string fromCurrencyName = selectedFromCurrency.Split('-')[0].Trim();
                string toCurrencyName = selectedToCurrency.Split('-')[0].Trim();

                Moneda fromMoneda = monede.Find(moneda => moneda.Name == fromCurrencyName);
                Moneda toMoneda = monede.Find(moneda => moneda.Name == toCurrencyName);
                Moneda usd = monede.Find(moneda => moneda.Name == "U.S. Dollar");

                if (fromMoneda != null && toMoneda != null)
                {
                    await DisplayAlert("Conversion Rates", $"From {fromMoneda.Name}: {fromMoneda.Rate}\nTo {toMoneda.Name}: {toMoneda.Rate}", "OK");


                    decimal result = Calcul(fromMoneda, toMoneda, usd );
                    AmountIn.Text = result.ToString();
                }
                else
                {
                    await DisplayAlert("Error", "Invalid currency selection", "OK");
                }

               
               
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
            }
        }
        private decimal Calcul(Moneda obj1, Moneda obj2, Moneda obj3)
        {
            decimal result;
            int AmountEntryResult;
            Int32.TryParse(AmountEntry.Text, out AmountEntryResult);
            result = AmountEntryResult / obj3.Rate * obj2.Rate;
            decimal newResult = Math.Round(result, 2);
            return newResult;
         

        }


    }
}
