using CadastramentoPerformace.Core;
using CadastramentoPerformace.MVVM.Model;
using CadastramentoPerformace.MVVM.View;
using CadastramentoPerformace.SQL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CadastramentoPerformace.MVVM.ViewModel
{
    internal class CadastramentoViewModel : ObservableObject
    {
        public RelayCommand AddEquipeCommand { get; set; }
        public RelayCommand EditEquipeCommand { get; set; }
        public RelayCommand AddOSCommand { get; set; }
        public RelayCommand ExcluirOSCommand { get; set; }
        public RelayCommand AddLocalCommand { get; set; }
        public RelayCommand ExcluirLocalCommand { get; set; }

        private ObservableCollection<Equipe> _equipesGridLista;

        public ObservableCollection<Equipe> EquipesGridLista
        {
            get { return _equipesGridLista; }
            set
            {
                _equipesGridLista = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<OS> _osGridLista;

        public ObservableCollection<OS> OSGridLista
        {
            get { return _osGridLista; }
            set
            {
                _osGridLista = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Localidade> _localidadeGridLista;

        public ObservableCollection<Localidade> LocalidadeGridLista
        {
            get { return _localidadeGridLista; }
            set
            {
                _localidadeGridLista = value;
                OnPropertyChanged();
            }
        }

        public CadastramentoViewModel()
        {
            UpdateEquipes();
            UpdateLocalidades();
            UpdateOS();

            AddEquipeCommand = new RelayCommand(o =>
            {
                AddEquipeWindow addEquipeWindow = new AddEquipeWindow();
                addEquipeWindow.Closed += (s, eventarg) =>
                {
                    UpdateEquipes();
                };
                addEquipeWindow.Show();
            });

            EditEquipeCommand = new RelayCommand(args =>
            {
                EditEquipeWindow editEquipeWindow = new EditEquipeWindow(args as Equipe);
                editEquipeWindow.Closed += (s, eventarg) =>
                {
                    UpdateEquipes();
                };
                editEquipeWindow.Show();
            });

            AddOSCommand = new RelayCommand(o =>
            {
                AddOSWindow addOSWindow = new AddOSWindow();
                addOSWindow.Closed += (s, eventarg) =>
                {
                    UpdateOS();
                };
                addOSWindow.Show();
            });

            ExcluirOSCommand = new RelayCommand(o =>
            {
                if (MessageBox.Show("Deseja excluir essa OS?", "Excluir OS", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                    return;
                }
                else
                {
                    //do yes stuff
                    var osSelected = o as OS;
                    DataAcess db = new DataAcess();
                    var task = Task.Run(async () => await db.DeleteOS(osSelected.CodigoOS, osSelected.DescricaoOS));
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

            AddLocalCommand = new RelayCommand(o =>
            {
                AddLocalidadeWindow addLocalWindow = new AddLocalidadeWindow();
                addLocalWindow.Closed += (s, eventarg) =>
                {
                    UpdateLocalidades();
                };
                addLocalWindow.Show();
            });

            ExcluirLocalCommand = new RelayCommand(o =>
            {
                if (MessageBox.Show("Deseja excluir essa localidade?", "Excluir Localidade", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                    return;
                }
                else
                {
                    //do yes stuff
                    var localidadeSelected = o as Localidade;
                    DataAcess db = new DataAcess();
                    var task = Task.Run(async () => await db.DeleteLocal(localidadeSelected.ID, localidadeSelected.NomeLocal));
                    if (!task.Result)
                    {
                        MessageBox.Show("Erro na conexão. Operação nâo concluida!");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Excluído com sucesso!");
                        UpdateLocalidades();
                        return;
                    }

                }
            });
        }


        private void UpdateEquipes()
        {
            DataAcess db = new DataAcess();
            EquipesGridLista = new ObservableCollection<Equipe>(db.GetEquipes());
            foreach (Equipe equipe in EquipesGridLista)
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

        private void UpdateOS()
        {
            DataAcess db = new DataAcess();
            OSGridLista = new ObservableCollection<OS>(db.GetOS());
        }

        private void UpdateLocalidades()
        {
            DataAcess db = new DataAcess();
            LocalidadeGridLista = new ObservableCollection<Localidade>(db.GetLocalidades());
        }
    }
}