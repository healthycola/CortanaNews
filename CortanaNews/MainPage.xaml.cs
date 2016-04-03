using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CortanaNews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        string[] ExtractTeasersFromJsonObject(JsonObject obj)
        {
            string[] output = null;
            if (obj.ContainsKey("list"))
            {
                var list = obj.GetNamedObject("list");

                if (list.ContainsKey("story"))
                {
                    var stories = list.GetNamedArray("story");

                    output = new string[stories.Count];
                    int i = 0;
                    foreach (var story in stories)
                    {
                        if (story.GetObject().ContainsKey("teaser"))
                        {
                            var teaser = story.GetObject().GetNamedObject("teaser");
                            if (teaser.ContainsKey("$text"))
                            {
                                output[i] = teaser.GetNamedString("$text");
                            }
                        }

                        i++;
                    }
                }
            }

            return output;
        }
        async void MakeNPRRequest()
        {
            string url = "http://api.npr.org/";
            string parameters = "query?id=1001&fields=title%2Cteaser%2Caudio&output=JSON&apiKey=MDIzNDM4NzA4MDE0NTkwNTA3NDVmM2FhMA000";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = client.GetAsync(parameters).Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = await response.Content.ReadAsStringAsync();

                    JsonValue x = JsonValue.Parse(dataObjects);
                    JsonObject obj = JsonObject.Parse(dataObjects);
                    var teasers = ExtractTeasersFromJsonObject(obj);

                    foreach (var teaser in teasers)
                    {
                        NewsText.Text += teaser + "\n";
                    }
                 }
            }
            catch (Exception ex)
            {
                NewsText.Text = ex.ToString();
            }
        }

        private void FullnewsButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void HeadLineButtonClick(object sender, RoutedEventArgs e)
        {
            MakeNPRRequest();
        }
    }
}
