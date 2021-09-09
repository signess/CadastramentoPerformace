using CadastramentoPerformace.Core;
using CadastramentoPerformace.SQL;
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
using System.Windows.Shapes;

namespace CadastramentoPerformace.MVVM.View
{
    /// <summary>
    /// Interaction logic for AddOSWindow.xaml
    /// </summary>
    public partial class AddOSWindow : Window
    {
        public AddOSWindow()
        {
            InitializeComponent();
        }

        private void FinalizarBtn(object sender, RoutedEventArgs e)
        {
            string codigoText = CodigoBox.Text;
            bool improdutivo = ImprodutivoTgl.IsChecked;
            if (!Ajuda.ValidateNumbers(codigoText))
            {
                CodigoBox.Text = "";
                MessageBox.Show("Digite apenas números no campo do Codigo!");
                return;
            }
            string descText = DescricaoBox.Text;
            if (!string.IsNullOrEmpty(codigoText) && !string.IsNullOrEmpty(descText))
            {
                DataAcess db = new DataAcess();
                db.InsertOS(int.Parse(codigoText), descText, improdutivo);
                CodigoBox.Text = "";
                DescricaoBox.Text = "";
                this.Close();
            }
            else
            {
                MessageBox.Show("Você deve preencher o campo!");
                return;
            }
        }
    }
}
