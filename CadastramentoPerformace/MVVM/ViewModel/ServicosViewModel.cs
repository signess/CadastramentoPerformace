using CadastramentoPerformace.Core;
using CadastramentoPerformace.MVVM.Model;
using CadastramentoPerformace.SQL;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace CadastramentoPerformace.MVVM.ViewModel
{
    internal class ServicosViewModel : ObservableObject
    {
        public RelayCommand FinalizarCommand { get; set; }

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

        public ObservableCollection<string> Executores { get; set; }
        public ObservableCollection<OS> OS { get; set; }

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
                    Executores.Clear();
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
                if (_currentEquipe == null)
                    ClearExecutores();
                else
                    ExecutoresToComboBox(_currentEquipe);
                OnPropertyChanged();
            }
        }

        private string _currentExecutor;

        public string CurrentExecutor
        {
            get { return _currentExecutor; }
            set
            {
                _currentExecutor = value;
                OnPropertyChanged();
            }
        }

        private DateTime _data = DateTime.Now;

        public DateTime Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        private string _tempoExecucao = "";

        public string TempoExecucao
        {
            get { return _tempoExecucao; }
            set
            {
                _tempoExecucao = value;
                OnPropertyChanged();
            }
        }

        private string _quantidade = "";

        public string Quantidade
        {
            get { return _quantidade; }
            set
            {
                _quantidade = value;
                OnPropertyChanged();
            }
        }

        public ServicosViewModel()
        {
            Executores = new ObservableCollection<string>();
            UpdateLocalidades();
            UpdateOS();

            FinalizarCommand = new RelayCommand(o =>
            {
                Finalizar();
            });
        }

        private void Finalizar()
        {
            string nomeLocal = "";
            if (CurrentLocalidade != null)
                nomeLocal = CurrentLocalidade.NomeLocal;
            int codigoOS = 0;
            string descricaoOS = "";
            bool improdutivo = false;
            if (CurrentOS != null)
            {
                codigoOS = CurrentOS.CodigoOS;
                descricaoOS = CurrentOS.DescricaoOS;
                improdutivo = CurrentOS.Improdutivo;

            }
            int numeroEquipe = 0;
            if (CurrentEquipe != null)
                numeroEquipe = CurrentEquipe.NumeroEquipe;
            string executor = "";
            if (CurrentExecutor != null)
                executor = CurrentExecutor;
            DateTime data = Data;
            if (!Ajuda.ValidateNumbers(TempoExecucao))
            {
                MessageBox.Show("Tempo de execução deve ser em numeros!");
                return;
            }
            if (string.IsNullOrEmpty(TempoExecucao))
                TempoExecucao = "0";
            if (string.IsNullOrEmpty(Quantidade))
                TempoExecucao = "0";
            int quantidade = int.Parse(Quantidade);
            float tempoExecucao = float.Parse(TempoExecucao);
            if (!string.IsNullOrEmpty(nomeLocal) && !string.IsNullOrEmpty(codigoOS.ToString()) && !string.IsNullOrEmpty(numeroEquipe.ToString()) && !string.IsNullOrEmpty(executor) && !string.IsNullOrEmpty(TempoExecucao))
            {
                DataAcess db = new DataAcess();
                db.InsertServico(nomeLocal, codigoOS, descricaoOS, improdutivo,  numeroEquipe, executor, data, quantidade, tempoExecucao);
                MessageBox.Show("Serviço cadastrado com sucesso!");
                Reset();
                return;
            }
            else
            {
                MessageBox.Show("Todos os campos devem estar preenchidos!");
                return;
            }
        }

        private void Reset()
        {
            Data = DateTime.Now;
            TempoExecucao = "0";
            Quantidade = "0";
            UpdateLocalidades();
            UpdateOS();
            Executores.Clear();
            Equipes.Clear();
        }

        private void UpdateEquipes(string nomelocal)
        {
            DataAcess db = new DataAcess();
            Equipes = new ObservableCollection<Equipe>(db.GetEquipeFromLocal(nomelocal));
            foreach (Equipe equipe in Equipes)
            {
                string executoresString = equipe.Executores;
                char[] separators = new char[] { ' ', ',' };
                string[] executores = executoresString.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (executores.Length > 0)
                {
                    equipe.ExecutoresList = executores.ToList();
                }
            }
        }

        private void ExecutoresToComboBox(Equipe equipe)
        {
            ClearExecutores();
            foreach (string s in equipe.ExecutoresList)
            {
                Executores.Add(s);
            }
        }

        private void ClearExecutores()
        {
            if (Executores != null && Executores.Count > 0)
                Executores.Clear();
        }

        private void UpdateOS()
        {
            DataAcess db = new DataAcess();
            OS = new ObservableCollection<OS>(db.GetOS());
        }

        private void UpdateLocalidades()
        {
            DataAcess db = new DataAcess();
            Localidades = new ObservableCollection<Localidade>(db.GetLocalidades());
        }
    }
}