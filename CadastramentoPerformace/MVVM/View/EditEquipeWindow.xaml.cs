using CadastramentoPerformace.MVVM.Model;
using CadastramentoPerformace.SQL;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CadastramentoPerformace.MVVM.View
{
    /// <summary>
    /// Interaction logic for EditEquipeWindow.xaml
    /// </summary>
    public partial class EditEquipeWindow : Window
    {
        private Equipe _equipe;
        public ObservableCollection<string> ExecutoresList;

        public EditEquipeWindow(Equipe equipe)
        {
            InitializeComponent();
            _equipe = equipe;
            LocalidadeBlock.Text = equipe.NomeLocal;
            NumeroEquipeBlock.Text = equipe.NumeroEquipe.ToString();
            ExecutoresList = new ObservableCollection<string>(equipe.ExecutoresList);
            ExecutoresListView.ItemsSource = ExecutoresList;
        }

        private void RemoveExecutorBtn(object sender, RoutedEventArgs e)
        {
            if (ExecutoresList.Any())
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

        private void ExcluirEquipeBtn(object sender, RoutedEventArgs e)
        {
            DataAcess db = new DataAcess();
            var task = Task.Run(async () => await db.DeleteEquipe(_equipe.NomeLocal, _equipe.NumeroEquipe));
            if(!task.Result)
            {
                MessageBox.Show("Erro na conexão. Operação nâo concluida!");
                return;
            }
            else
            {
                Close();
            }
        }

        private void FinalizarEquipeBtn(object sender, RoutedEventArgs e)
        {
            DataAcess db = new DataAcess();
            db.UpdateEquipe(_equipe.NomeLocal, _equipe.NumeroEquipe, ExecutoresListToString());
            this.Close();
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