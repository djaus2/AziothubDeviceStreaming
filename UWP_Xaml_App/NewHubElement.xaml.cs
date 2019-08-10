using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
    public sealed partial class NewHubElement : Page, INotifyPropertyChanged
    {


        public string Property { get; set; }


        public Visibility Show1stButton { get; set; } = Visibility.Visible;
        public Visibility Show2ndButton { get; set; } = Visibility.Collapsed;
        public Visibility Show3rdButton { get; set; } = Visibility.Collapsed;
        public Visibility Show4thButton { get; set; } = Visibility.Collapsed;

        public ValueChanged CreateNewEntity { get; set; } = null;
        public ValueChanged DeleteEntity { get; set; } = null;
        public ValueChanged GenerateEntityInfo { get; set; } = null;

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(
        //[System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null)
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //        //if(!string.IsNullOrEmpty(Property))
        //        //    ValueChanged?.Invoke(Property);
        //}

        //Ref: http://blog.jerrynixon.com/2013/07/solved-two-way-binding-inside-user.html
        public event PropertyChangedEventHandler PropertyChanged;

        //Ref: https://stackoverflow.com/questions/8821961/protected-member-in-sealed-class-warning-a-singleton-class
        private void SetValueDp(DependencyProperty property, object value,
       [System.Runtime.CompilerServices.CallerMemberName] string p=null)
        {
            SetValue(property, value);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(p));
        }

        public static ValueChanged ValueChanged { get; set; } = null;
        public enum Mode
        {
            azCodeSnippet, heading, link, info, comment, sep, multi,
            info2,
            infoWithButtonsOnRight,
            sectionHeading
        }
        public Mode DisplayMode { get; set; } = NewHubElement.Mode.azCodeSnippet;

        public static readonly DependencyProperty CodeProperty =
           DependencyProperty.Register(
               "Code", typeof(string),
               typeof(NewHubElement), null
           );

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValueDp(CodeProperty, value); }
        }// OnPropertyChanged(); /*MultiComment.Text = code;*/ /*MultiComment2.Text = code;*/ } }
        public string Url { get; set; } = "";
        public string UrlText { get; set; } = "";


        public string InfoValue
        {
            get { return infoValue; }
            set { infoValue =value; }
        }


        private bool isExpanded1 = false;
        private string infoValue = "";

        public bool IsExpanded
        {
            get => isExpanded1; set
            {
                isExpanded1 = value;
                if (SubRegion != null)
                    SubRegion.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                Heading1Toggle.IsChecked = value;
            }
        }


        public static readonly DependencyProperty TextInfoProperty =
            DependencyProperty.Register(
                "TextInfo", typeof(string),
                typeof(NewHubElement), null
            );

        public string TextInfo
        {
            get { return (string)GetValue(TextInfoProperty); }
            set { SetValueDp(TextInfoProperty, value); }
        }

        public Color Color { get; set; } = Colors.Red;

        public NewHubElement()
        {
            this.InitializeComponent();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                string name = ((Button) sender).Name;
                if (!string.IsNullOrEmpty(name))
                {
                    switch (name)
                    {
                        case "AzCliCodeSnippetButton":
                            var dataPackage1 = new DataPackage();
                            dataPackage1.SetText(Code);
                            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage1);
                            break;
                        case "InfoWithButtonsOnRightButton_Paste":
                            var cb = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
                            string pastedText = await cb.GetTextAsync();
                            if (!string.IsNullOrEmpty(pastedText))
                            {
                                TextInfo = pastedText;
                            }
                            break;
                        case "InfoWithButtonsOnRightButton_New":
                            CreateNewEntity?.Invoke(Property);
                            break;
                        case "InfoWithButtonsOnRightButton_Delete":
                            DeleteEntity?.Invoke(Property);
                            break;
                        case "InfoWithButtonsOnRightButton_Action":
                            GenerateEntityInfo?.Invoke(Property);
                            break;
                        default:
                            break;
                    }
                }
            }
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SectionHeading.Visibility = (DisplayMode == Mode.sectionHeading) ? Visibility.Visible : Visibility.Collapsed;
            InfoWithButtonsOnRight.Visibility = (DisplayMode == Mode.infoWithButtonsOnRight) ? Visibility.Visible : Visibility.Collapsed;
            //Info2.Visibility = (DisplayMode == Mode.info2) ? Visibility.Visible : Visibility.Collapsed;
            Separator.Visibility = (DisplayMode == Mode.sep) ? Visibility.Visible : Visibility.Collapsed;
            Comment.Visibility = (DisplayMode == Mode.comment) ? Visibility.Visible : Visibility.Collapsed;
            AzCliCodeSnippet.Visibility = (DisplayMode == Mode.azCodeSnippet) ? Visibility.Visible : Visibility.Collapsed;
            AzCliCodeSnippetButton.Visibility = AzCliCodeSnippet.Visibility;
            Heading.Visibility = (DisplayMode == Mode.heading) ? Visibility.Visible : Visibility.Collapsed;
            Link.Visibility = (DisplayMode == Mode.link) ? Visibility.Visible : Visibility.Collapsed;
            Info.Visibility = (DisplayMode == Mode.info) ? Visibility.Visible : Visibility.Collapsed;
            //MultiComment.Visibility = (DisplayMode == Mode.multi) ? Visibility.Visible : Visibility.Collapsed;

            if (DisplayMode == Mode.link)
            {
                LinkHyperLink.NavigateUri = new Uri(Url);
                LinkHyperLink.Content = UrlText;
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

        public void Update()
        {
            DataContext = null;
            this.DataContext = this;
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

        private void InfoVal2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Butt3_Click(object sender, RoutedEventArgs e)
        {
            var cb = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            string pastedText = await cb.GetTextAsync();
            if (!string.IsNullOrEmpty(pastedText))
            {
                TextInfo = pastedText;
            }
        }

        public StackPanel SubRegion { get; set; } = null;

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (SubRegion != null)
                SubRegion.Visibility = (Heading1Toggle.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
            if (Heading1Toggle.IsChecked == true)
            {
                Heading1Toggle.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                Heading1Icon.Symbol = Symbol.BackToWindow;
                Heading1Icon.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
            }
            else
            {
                Heading1Toggle.Foreground = new SolidColorBrush(Colors.Blue);
                Heading1Icon.Symbol = Symbol.Library;
                Heading1Icon.Foreground = new SolidColorBrush(Colors.Blue);
            }
        }
    }
}
