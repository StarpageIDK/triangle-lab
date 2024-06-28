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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace lr5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> calculationsHistory = new List<string>();
        XDocument xDocument;
        XElement settings;

        public MainWindow()
        {
            InitializeComponent();

            xDocument = XDocument.Load("settings.xml");
            settings = xDocument.Root;

            Application.Current.MainWindow.Width = Convert.ToDouble(settings.Element("window_width").Value);
            Application.Current.MainWindow.Height = Convert.ToDouble(settings.Element("window_height").Value);

            BaseTextBox.Text = settings.Element("BaseTextBox").Value;
            HeightTextBox.Text = settings.Element("HeightTextBox").Value;
            ResultTextBlock.Text = settings.Element("ResultTextBox").Value;


            switch (settings.Element("theme").Value)
            {
                case "white":
                    LightThemeRadioButton.IsChecked = true;
                    break;

                case "dark":
                    DarkThemeRadioButton.IsChecked = true;
                    break;
            }

            XElement history = settings.Element("history");

            foreach (XElement element in history.Elements())
            {
                ResultsListBox.Items.Add(element.Value);
            }
        }

        private void CalculateArea_Click(object sender, EventArgs e)
        {
            if (double.TryParse(BaseTextBox.Text, out double @base) && double.TryParse(HeightTextBox.Text, out double height))
            {
                double area = 0.5 * @base * height;
                ResultsListBox.Items.Add($"Base: {@base}, Height: {height} -> Area: {area}");
                BaseTextBox.Clear();
                HeightTextBox.Clear();

                ResultTextBlock.Text = $"Area of the triangle: {area}";
            }
            else
            {
                MessageBox.Show("Please enter valid numeric values for base and height.");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // запись в XML-файл настроек
            settings.Element("window_width").Value = Application.Current.MainWindow.Width.ToString();
            settings.Element("window_height").Value = Application.Current.MainWindow.Height.ToString();
            settings.Element("BaseTextBox").Value = BaseTextBox.Text;
            settings.Element("HeightTextBox").Value = HeightTextBox.Text;
            settings.Element("ResultTextBox").Value = ResultTextBlock.Text;

            if (LightThemeRadioButton.IsChecked == true) settings.Element("theme").Value = "white";
            else if (DarkThemeRadioButton.IsChecked == true) settings.Element("theme").Value = "dark";

            // очистка истории
            XElement history = settings.Element("history");
            history.RemoveAll();

            // заполнение истории из списка
            foreach (string item in ResultsListBox.Items)
            {
                history.Add(new XElement("history_element", item));
            }

            xDocument.Save("settings.xml"); // сохранение файла
        }

        private void themes_Checked(object sender, RoutedEventArgs e)
        {
            var active = sender as RadioButton;
            string style = "";

            switch (active.Name)
            {
                case "LightThemeRadioButton":
                    style = "white";
                    break;

                case "DarkThemeRadioButton":
                    style = "dark";
                    break;
            }

            var uri = new Uri(style + ".xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

        }

        private void ClearListBox_Click(object sender, RoutedEventArgs e)
        {
            ResultsListBox.Items.Clear();
        }
    }
}
