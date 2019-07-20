using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPXamlApp
{
    public delegate void ValueChanged(string recvTxt);
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewHubElement : Page
    {
        private string code = "";

        public ValueChanged ValueChanged { get; set; } = null;
        public enum Mode { code, heading, link, info, comment, sep, multi }
        public Mode DisplayMode { get; set; } = NewHubElement.Mode.code;

        public string Code { get => code; set  { code = value; Txt.Text = code; /*MultiComment.Text = code;*/ /*MultiComment2.Text = code;*/ } }
        public string Url { get; set; } = "";
        public string UrlText { get; set; } = "";

        public string InfoValue { get; set; } = "";

        public Color Color { get; set; } = Colors.Red;

        public NewHubElement()
        {
            this.InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(Code);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Separator.Visibility= (DisplayMode == Mode.sep) ? Visibility.Visible : Visibility.Collapsed;
            Comment.Visibility = (DisplayMode == Mode.comment) ? Visibility.Visible : Visibility.Collapsed;
            Txt.Visibility = (DisplayMode == Mode.code) ? Visibility.Visible : Visibility.Collapsed;
            Butt.Visibility = Txt.Visibility;
            Heading.Visibility = (DisplayMode == Mode.heading) ? Visibility.Visible : Visibility.Collapsed;
            Link.Visibility = (DisplayMode == Mode.link) ? Visibility.Visible : Visibility.Collapsed;
            Info.Visibility = (DisplayMode == Mode.info) ? Visibility.Visible : Visibility.Collapsed;
            //MultiComment.Visibility = (DisplayMode == Mode.multi) ? Visibility.Visible : Visibility.Collapsed;

            if (DisplayMode == Mode.link)
            {
                HyperLink.NavigateUri = new Uri(Url);
                HyperLink.Content = UrlText;
            }
            else if (DisplayMode == Mode.info)
            {
                //InfoVal.Text = InfoValue;
            }
            else if (DisplayMode == Mode.sep)
            {
                Separator.Background = new SolidColorBrush(Color);
            }
            else if (DisplayMode == Mode.link)
            {
               // MultiComment.Text = Code;
                //MultiComment2.Text = Code;
                //MultiComment.Visibility = Visibility.Visible;
                //MultiComment2.Visibility = Visibility.Collapsed;
            }
            this.DataContext = this;
        }

        private void InfoVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValueChanged?.Invoke(InfoVal.Text);
        }

        public void MultiCommentExpand(object sender, RoutedEventArgs e)
        {
            //if (MultiComment.Visibility == Visibility.Visible)
            //{
            //    Visibility temp = MultiComment1.Visibility;
            //    MultiComment1.Visibility = MultiComment2.Visibility;
            //    MultiComment2.Visibility = temp;
            //}
        }
    }
}
