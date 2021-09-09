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
    /// Interaction logic for AddLocalidadeWindow.xaml
    /// </summary>
    public partial class AddLocalidadeWindow : Window
    {
        public AddLocalidadeWindow()
        {
            InitializeComponent();
        }

        private void FinalizarBtn(object sender, RoutedEventArgs e)
        {
            string nomeLocalText = NomeLocalBox.Text;
            if (!string.IsNullOrEmpty(nomeLocalText))
            {
                DataAcess db = new DataAcess();
                db.InsertLocal(nomeLocalText);
                NomeLocalBox.Text = "";
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
