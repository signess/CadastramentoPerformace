using CadastramentoPerformace.Core;
using CadastramentoPerformace.MVVM.Model;
using CadastramentoPerformace.SQL;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CadastramentoPerformace.MVVM.ViewModel
{
    internal class ConsultaViewModel : ObservableObject
    {
        public RelayCommand PesquisarCommand { get; set; }
        public RelayCommand ExcluirServicoCommand { get; set; }
        public RelayCommand ExportarCommand { get; set; }
        public RelayCommand ExportarSumCommand { get; set; }
        private int boolean = 0;
        public ObservableCollection<string> RadioBtnList { get; set; }
        private string _selectedRadioBtn;

        public string SelectedRadioBtn
        {
            get { return _selectedRadioBtn; }
            set
            {
                _selectedRadioBtn = value;
                ChangeRadioBtns();
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Equipe> _equipes;

        public ObservableCollection<Equipe> Equipes
        {
            get { return _equipes; }
            set
            {
                _equipes = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<OS> _os;

        public ObservableCollection<OS> OS
        {
            get { return _os; }
            set
            {
                _os = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Localidade> _localidades;

        public ObservableCollection<Localidade> Localidades
        {
            get { return _localidades; }
            set
            {
                _localidades = value;
                OnPropertyChanged();
            }
        }

        private Localidade _currentLocalidade;

        public Localidade CurrentLocalidade
        {
            get { return _currentLocalidade; }
            set
            {
                _currentLocalidade = value;
                if (_currentLocalidade != null)
                {
                    UpdateEquipes(_currentLocalidade.NomeLocal);
                }
                OnPropertyChanged();
            }
        }

        private OS _currentOS;

        public OS CurrentOS
        {
            get { return _currentOS; }
            set
            {
                _currentOS = value;
                OnPropertyChanged(nameof(CurrentOS));
            }
        }

        private Equipe _currentEquipe;

        public Equipe CurrentEquipe
        {
            get { return _currentEquipe; }
            set
            {
                _currentEquipe = value;
                OnPropertyChanged();
            }
        }

        private DateTime _startDate = DateTime.Now;

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endDate = DateTime.Now;

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        private ListCollectionView _servicos;

        public ListCollectionView Servicos
        {
            get { return _servicos; }
            set
            {
                _servicos = value;
                OnPropertyChanged();
            }
        }

        private Servico _currentServico;

        public Servico CurrentServico
        {
            get { return _currentServico; }
            set
            {
                _currentServico = value;
                OnPropertyChanged();
            }
        }

        public ConsultaViewModel()
        {
            UpdateLocalidades();
            UpdateOS();
            string tudo = "Tudo";
            string produtivo = "Produtivo";
            string improdutivo = "Improdutivo";
            RadioBtnList = new ObservableCollection<string>() { tudo, produtivo, improdutivo };
            SelectedRadioBtn = RadioBtnList.FirstOrDefault();

            PesquisarCommand = new RelayCommand(o =>
            {
                UpdateServicos();
            });

            ExcluirServicoCommand = new RelayCommand(o =>
            {
                if (MessageBox.Show("Deseja excluir esse Serviço?", "Excluir Serviço", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                    return;
                }
                else
                {
                    //do yes stuff
                    var servicoSelected = o as Servico;
                    DataAcess db = new DataAcess();
                    var task = Task.Run(async () => await db.DeleteServico(servicoSelected.NomeLocal, servicoSelected.CodigoOS, servicoSelected.DescricaoOS, servicoSelected.Improdutivo, servicoSelected.NumeroEquipe, servicoSelected.Executor, servicoSelected.Data, servicoSelected.Quantidade, servicoSelected.TempoExecucao));
                    if (!task.Result)
                    {
                        MessageBox.Show("Erro na conexão. Operação nâo concluida!");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Excluído com sucesso!");
                        UpdateOS();
                        return;
                    }

                }
            });

            ExportarCommand = new RelayCommand(o =>
            {
                DataGrid dataGrid = o as DataGrid;
                if (dataGrid != null)
                    Export(dataGrid);
                else
                    return;
            });

            ExportarSumCommand = new RelayCommand(o =>
            {
                DataGrid dataGrid = o as DataGrid;
                if (dataGrid != null)
                    ExportWithSum(dataGrid);
                else
                    return;
            });
        }

        private void ChangeRadioBtns()
        {
            if (SelectedRadioBtn == "Tudo")
            {
                UpdateOS();
                boolean = 0;
            }
            else if (SelectedRadioBtn == "Produtivo")
            {
                UpdateOS(true);
                boolean = 1;
            }
            else if (SelectedRadioBtn == "Improdutivo")
            {
                UpdateOS(false);
                boolean = 2;
            }
        }

        private void UpdateOS()
        {
            DataAcess db = new DataAcess();
            OS tudo = new OS();
            tudo.CodigoOS = 0;
            OS = new ObservableCollection<OS>(db.GetOS());
            OS.Insert(0, tudo);
        }

        private void UpdateOS(bool produtivo)
        {
            DataAcess db = new DataAcess();
            OS tudo = new OS();
            tudo.CodigoOS = 0;
            OS = new ObservableCollection<OS>(db.GetOSProdutivo(produtivo));
            OS.Insert(0, tudo);
        }

        private void UpdateLocalidades()
        {
            DataAcess db = new DataAcess();
            Localidades = new ObservableCollection<Localidade>(db.GetLocalidades());
        }

        private void UpdateEquipes(string nomelocal)
        {
            DataAcess db = new DataAcess();
            Equipe tudo = new Equipe();
            tudo.NumeroEquipe = 0;
            Equipes = new ObservableCollection<Equipe>(db.GetEquipeFromLocal(nomelocal));
            Equipes.Insert(0, tudo);
            foreach (Equipe equipe in Equipes)
            {
                string executoresString = equipe.Executores;
                if (string.IsNullOrEmpty(executoresString))
                    return;
                char[] separators = new char[] { ' ', ',' };
                string[] executores = executoresString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (executores.Length > 0)
                {
                    equipe.ExecutoresList = executores.ToList();
                }
            }
        }

        private void UpdateServicos()
        {
            string nomeLocal = "";
            int codigoOS = 0;
            int numeroEquipe = 0;
            if (CurrentLocalidade != null)
            {
                nomeLocal = CurrentLocalidade.NomeLocal;
            }
            if (CurrentOS != null)
            {
                codigoOS = CurrentOS.CodigoOS;
            }
            if (CurrentEquipe != null)
            {
                numeroEquipe = CurrentEquipe.NumeroEquipe;
            }
            DataAcess db = new DataAcess();
            Servicos = new ListCollectionView(db.GetServicos(nomeLocal, codigoOS, boolean, numeroEquipe, StartDate, EndDate));
            Servicos.GroupDescriptions.Add(new PropertyGroupDescription("CodigoOS"));
        }

        private void Export(DataGrid dataGrid)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true; //www.yazilimkodlama.com
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < dataGrid.Columns.Count; j++) //Başlıklar için
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true; //Başlığın Kalın olması için
                sheet1.Columns[j + 1].ColumnWidth = 15; //Sütun genişliği ayarı
                myRange.Value2 = dataGrid.Columns[j].Header;
            }
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                for (int j = 0; j < dataGrid.Items.Count; j++)
                {
                    TextBlock b = dataGrid.Columns[i].GetCellContent(dataGrid.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
            Range myLast = sheet1.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing);
            int myLastRow = myLast.Row;
            int myLastColumn = myLast.Column;

            Range myLine = (Range)sheet1.Cells[myLastRow + 1, 7];
            sheet1.Cells[myLastRow + 1, 7].Font.Bold = true;
            myLine.Value2 = "=SUM(G2:G" + myLastRow + ")";
            myLine = (Range)sheet1.Cells[myLastRow + 1, 8];
            sheet1.Cells[myLastRow + 1, 8].Font.Bold = true;
            myLine.Value2 = "=SUM(H2:H" + myLastRow + ")";
        }

        private void ExportWithSum(DataGrid dataGrid)
        {
            List<int> a = new List<int>();
            float oldV = 0;
            float newV = 0;

            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true; //www.yazilimkodlama.com
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < dataGrid.Columns.Count; j++) //Başlıklar için
            {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true; //Başlığın Kalın olması için
                sheet1.Columns[j + 1].ColumnWidth = 15; //Sütun genişliği ayarı
                myRange.Value2 = dataGrid.Columns[j].Header;
            }
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                int jump = 0;
                int lastIndex = 2;
                for (int j = 0; j < dataGrid.Items.Count; j++)
                {
                    TextBlock b = dataGrid.Columns[i].GetCellContent(dataGrid.Items[j]) as TextBlock;
                    if (i == 0)
                    {
                        if (j == 0)
                        {
                            newV = float.Parse(b.Text);
                            oldV = newV;
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                            myRange.Value2 = b.Text;
                            a.Add(0);
                        }
                        else
                        {
                            newV = float.Parse(b.Text);
                            if (newV != oldV)
                            {
                                jump += 2;
                                oldV = newV;
                                Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                                myRange.Value2 = b.Text;
                                a.Add(1);
                            }
                            else
                            {
                                Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                                myRange.Value2 = b.Text;
                                a.Add(0);
                            }
                        }
                    }
                    else if (i < 6)
                    {
                        if (a[j] == 0)
                        {
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                            myRange.Value2 = b.Text;
                        }
                        else
                        {
                            jump += 2;
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                            myRange.Value2 = b.Text;
                        }
                    }
                    else if (i >= 6)
                    {
                        if (a[j] == 0)
                        {
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                            myRange.Value2 = b.Text;
                            if(j == dataGrid.Items.Count - 1)
                            {
                                jump++;
                                myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                                sheet1.Cells[j + 2 + jump, i + 1].Font.Bold = true;
                                if (i == 6)
                                    myRange.Value2 = $"=SUM(G{lastIndex}:G{j + 1 + jump})";
                                else
                                    myRange.Value2 = $"=SUM(H{lastIndex}:H{j + 1 + jump})";
                            }
                        }
                        else
                        {
                            jump++;
                            Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 1 + jump, i + 1];
                            sheet1.Cells[j + 1 + jump, i + 1].Font.Bold = true;
                            if (i == 6)
                                myRange.Value2 = $"=SUM(G{lastIndex}:G{j + jump})";
                            else
                                myRange.Value2 = $"=SUM(H{lastIndex}:H{j + jump})";

                            jump++;
                            myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2 + jump, i + 1];
                            sheet1.Cells[j + 2 + jump, i + 1].Font.Bold = false;
                            myRange.Value2 = b.Text;
                            lastIndex = j + 2 + jump;
                        }
                    }
                }
            }
        }

        /*
        private void Sort()
        {
            var column = dataGrid.Columns[2];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, ListSortDirection.Ascending));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = ListSortDirection.Ascending;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }*/
    }
}