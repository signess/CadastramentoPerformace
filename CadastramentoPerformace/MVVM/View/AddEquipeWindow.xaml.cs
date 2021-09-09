using CadastramentoPerformace.Core;
using CadastramentoPerformace.MVVM.Model;
using CadastramentoPerformace.SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddEquipeWindow.xaml
    /// </summary>
    public partial class AddEquipeWindow : Window
    {
        public List<Localidade> LocalidadesList = new List<Localidade>();
        public ObservableCollection<string> ExecutoresList;
        public AddEquipeWindow()
        {
            InitializeComponent();
            PopulateLocalidadeComboBox();
            ExecutoresList = new ObservableCollection<string>();
            ExecutoresListView.ItemsSource = ExecutoresList;
        }

        private void RemoveExecutorBtn(object sender, RoutedEventArgs e)
        {
            if(ExecutoresList.Any())
            {
                ExecutoresList.RemoveAt(ExecutoresList.Count - 1);
            }
        }

        private void AddExecutorBtn(object sender, RoutedEventArgs e)
        {
            string executorText = ExecutorBox.Text;
            if (!string.IsNullOrEmpty(executorText))
            {
                ExecutoresList.Add(executorText);
                ExecutorBox.Text = "";
            }
            else
            {
                MessageBox.Show("Campo não pode estar vazio!");
                return;
            }
        }

        private void PopulateLocalidadeComboBox()
        {
            DataAcess db = new DataAcess();
            LocalidadesList = db.GetLocalidades();
            LocalidadeBox.ItemsSource = LocalidadesList;
            LocalidadeBox.DisplayMemberPath = "NomeLocal";
            LocalidadeBox.SelectedValuePath = "NomeLocal";
        }

        private void FinalizarEquipeBtn(object sender, RoutedEventArgs e)
        {
            string numeroEquipeText = NumeroEquipeBox.Text;
            Localidade comboItem = (Localidade)LocalidadeBox.SelectedItem;
            string localidadeSelected = comboItem.NomeLocal;
            if(!Ajuda.ValidateNumbers(numeroEquipeText))
            {
                NumeroEquipeBox.Text = "";
                MessageBox.Show("Digite apenas números no campo da Equipe!");
                return;
            }

            if (!string.IsNullOrEmpty(numeroEquipeText) && localidadeSelected != null)
            {
                DataAcess db = new DataAcess();
                db.InsertEquipe(localidadeSelected.ToString(), int.Parse(numeroEquipeText), ExecutoresListToString());
                ExecutorBox.Text = "";
                NumeroEquipeBox.Text = "";
                LocalidadeBox.SelectedItem = null;
                this.Close();
            }
            else
            {
                MessageBox.Show("Você deve preencher os dois primeiros campos!");
                return;
            }
        }
        

        private string ExecutoresListToString()
        {
            string finalString = "";
            if (ExecutoresList.Count > 0)
            {
                foreach (string executor in ExecutoresList)
                {
                    finalString += executor + ", ";
                }
            }
            return finalString;
        }
    }
}
