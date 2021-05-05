using RSADigitalSignature.Pages;
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

namespace RSADigitalSignature
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RSADigitalSignatureGenerator signatureGenerator;
        CheckPage checkPage = new CheckPage();
        GetSignaturePage getSignaturePage = new GetSignaturePage();

        public MainWindow()
        {
            InitializeComponent();
            SignatureGenerator = new RSADigitalSignatureGenerator();
            RField.Text = SignatureGenerator.R.ToString();
            EField.Text = SignatureGenerator.E.ToString();
            DField.Text = SignatureGenerator.D.ToString();
        }

        internal RSADigitalSignatureGenerator SignatureGenerator { get => signatureGenerator; set => signatureGenerator = value; }

        public static MainWindow GetMainWindow()
        {
            MainWindow mainWindow = null;

            foreach (Window window in Application.Current.Windows)
            {
                Type type = typeof(MainWindow);
                if (window != null && window.DependencyObjectType.Name == type.Name)
                {
                    mainWindow = (MainWindow)window;
                    if (mainWindow != null)
                    {
                        break;
                    }
                }
            }

            return mainWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (uint.TryParse(KeyLengthField.Text, out uint result))
            {
                if (result <= 1024 && result > 0)
                {
                    SignatureGenerator.SetupKeys((int)result);
                    RField.Text = SignatureGenerator.R.ToString();
                    EField.Text = SignatureGenerator.E.ToString();
                    DField.Text = SignatureGenerator.D.ToString();
                    MessageBox.Show("Новые ключи сгенерированы", "Сообщение",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ограничение длины ключа - от 1 до 1024 бит", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Некорректное значение длины ключа", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            KeyLengthField.Text = SignatureGenerator.MaxKeysLength.ToString();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ActionFrame.Navigate(getSignaturePage);
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            ActionFrame.Navigate(checkPage);
        }
    }
}
