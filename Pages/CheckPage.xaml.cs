using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace RSADigitalSignature.Pages
{
    /// <summary>
    /// Логика взаимодействия для CheckPage.xaml
    /// </summary>
    public partial class CheckPage : Page
    {
        public CheckPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BigInteger.TryParse(SignatureField.Text, out BigInteger signature))
            {
                if (BigInteger.TryParse(EField.Text, out BigInteger d))
                {
                    if (BigInteger.TryParse(RField.Text, out BigInteger r))
                    {
                        if (MainWindow.GetMainWindow().SignatureGenerator.CheckSignature(TextField.Text, signature, d, r))
                        {
                            MessageBox.Show("Проверка подписи пройдена успешно", 
                                "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Проверка подписи не пройдена",
                                "Результат", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Значение указанное как r не является корректным",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Значение указанное как d не является корректным",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Значение указанное как подпись не является корректным",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
