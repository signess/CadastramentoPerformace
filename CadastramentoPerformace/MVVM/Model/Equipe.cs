using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CadastramentoPerformace.MVVM.Model
{
    public class Equipe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property. 
        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string NomeLocal { get; set; }
        public int NumeroEquipe { get; set; }
        public string Executores { get; set; }

        private List<string> _executoresList = new List<string>();

        public List<string> ExecutoresList
        {
            get { return _executoresList; }
            set 
            {
                _executoresList = value;
                NotifyPropertyChanged();
            }
        }

        public Equipe()
        {

        }


        public void AddExecutor(string nome)
        {
            string novoExecutor = char.ToUpper(nome[0]) + nome.Substring(1);
            ExecutoresList.Add(novoExecutor);
        }
    }
}
