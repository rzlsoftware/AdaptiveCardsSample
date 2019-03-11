using AdaptiveCards;
using AdaptiveCards.Rendering;
using AdaptiveCards.Rendering.Wpf;
using Microsoft.Win32;
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
            void showSubmitAction(RenderedAdaptiveCard card, AdaptiveSubmitAction action)
            { }

            var renderer = new AdaptiveCardRenderer(_hostconfig);
            var version = renderer.SupportedSchemaVersion;
            // renderer.UseXceedElementRenderers();

            var json = LoadJson();
            var result = AdaptiveCard.FromJson(json);
            var renderedCard = renderer.RenderCard(result.Card);
            renderedCard.OnAction += (card, args) =>
            {

            };

            grid1.Children.Clear();
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
