using AdaptiveCards;
using AdaptiveCards.Rendering;
using AdaptiveCards.Rendering.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace AdaptiveCardsSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AdaptiveHostConfig _hostconfig;

        public MainWindow()
        {
            InitializeComponent();
            webBrowser.SuppressScriptErrors(true);
            _hostconfig = new AdaptiveHostConfig
            {
                Actions =
                {
                    ShowCard =
                    {
                        ActionMode = ShowCardActionMode.Popup
                    }
                }
            };
        }

        private void OnShowCard(object sender, RoutedEventArgs e)
        {
            var renderer = new AdaptiveCardRenderer(_hostconfig);
            var version = renderer.SupportedSchemaVersion;
            // renderer.UseXceedElementRenderers();

            var json = LoadJson();
            grid1.Children.Clear();
            if (json is "")
                return;

            var result = AdaptiveCard.FromJson(json);
            var renderedCard = renderer.RenderCard(result.Card);
            renderedCard.OnAction += (card, args) =>
            {
                switch (args.Action)
                {
                    case AdaptiveOpenUrlAction openUrlAction:
                        Process.Start(openUrlAction.Url.ToString());
                        break;

                    case AdaptiveShowCardAction showCardAction:
                        // Eine weitere Adaptive Card anzeigen
                        // Inline oder als Popup
                        break;

                    case AdaptiveSubmitAction submitAction:
                        var inputs = card.UserInputs.AsJson();
                        inputs.Merge(submitAction.Data);
                        MessageBox.Show(this, JsonConvert.SerializeObject(inputs, Formatting.Indented), "SubmitAction");
                        break;
                }
            };

            grid1.Children.Add(renderedCard.FrameworkElement);
        }

        private string LoadJson()
        {
            var picker = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON Files (.json)|*.json"
            };

            if (picker.ShowDialog() == true)
            {
                string json = File.ReadAllText(picker.FileName);
                return json;
            }
            return string.Empty;
        }
    }

    public static class WebBrowserExtensions
    {
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null)
                return;
            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null)
                return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
    }
}
