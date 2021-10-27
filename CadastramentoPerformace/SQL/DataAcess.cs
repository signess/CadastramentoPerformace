using CadastramentoPerformace.MVVM.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CadastramentoPerformace.SQL
{
    public class DataAcess
    {
        public List<Localidade> GetLocalidades()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<Localidade>($"SELECT * FROM Localidades").ToList();
                return output;
            }
        }

        public void InsertLocal(string nome)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                List<Localidade> localList = GetLocalidades();
                int nextID = localList.Count + 1;
                Localidade local = new Localidade { ID = nextID, NomeLocal = nome };
                connection.Execute("INSERT INTO Localidades (ID, NomeLocal) VALUES(@ID, @NomeLocal)", local);
            }
        }

        public async Task<bool> DeleteLocal(int id, string nome)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                Localidade local = new Localidade
                {
                    ID = id,
                    NomeLocal = nome
                };
                var results = await connection.ExecuteAsync("DELETE Localidades Where ID=@ID AND NomeLocal = @NomeLocal", local);
                if (results > 0) return true;
            }
            return false;
        }

        public List<Equipe> GetEquipes()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<Equipe>($"SELECT * FROM Equipes").ToList();
                return output;
            }
        }

        public List<Equipe> GetEquipeFromLocal(string nomelocal)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<Equipe>($"SELECT * FROM Equipes WHERE NomeLocal = '{nomelocal}'").ToList();
                return output;
            }
        }

        public void InsertEquipe(string local, int numeroEquipe, string executores)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                List<Equipe> equipes = new List<Equipe>();
                equipes.Add(new Equipe { NomeLocal = local, NumeroEquipe = numeroEquipe, Executores = executores });
                connection.Execute("INSERT INTO Equipes (NomeLocal, NumeroEquipe, Executores) VALUES(@NomeLocal, @NumeroEquipe, @Executores)", equipes);
            }
        }

        public void UpdateEquipe(string local, int numeroEquipe, string executores)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                Equipe equipe = new Equipe { NomeLocal = local, NumeroEquipe = numeroEquipe, Executores = executores };
                connection.Execute("UPDATE Equipes SET Executores = @Executores WHERE NomeLocal = @NomeLocal AND NumeroEquipe = @NumeroEquipe", equipe);
            }
        }

        public async Task<bool> DeleteEquipe(string local, int numeroEquipe)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                Equipe equipe = new Equipe
                {
                    NomeLocal = local,
                    NumeroEquipe = numeroEquipe
                };
                var results = await connection.ExecuteAsync("DELETE Equipes Where NomeLocal = @NomeLocal AND NumeroEquipe=@NumeroEquipe", equipe);
                if (results > 0) return true;
            }
            return false;
        }

        public List<OS> GetOS()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<OS>($"SELECT * FROM OS").ToList();
                return output;
            }
        }

        public List<OS> GetOSProdutivo(bool produtivo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<OS>($"SELECT * FROM OS WHERE Improdutivo = '{produtivo}'").ToList();
                return output;
            }
        }

        public void InsertOS(int codigo, string desc, bool improdutivo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                OS novaOS = new OS { CodigoOS = codigo, DescricaoOS = desc, Improdutivo = improdutivo };
                connection.Execute("INSERT INTO OS (CodigoOS, DescricaoOS, Improdutivo) VALUES(@CodigoOS, @DescricaoOS, @Improdutivo)", novaOS);
            }
        }

        public async Task<bool> DeleteOS(int codigo, string desc)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                OS os = new OS { CodigoOS = codigo, DescricaoOS = desc };
                var results = await connection.ExecuteAsync("DELETE OS Where CodigoOS = @CodigoOS AND DescricaoOS=@DescricaoOS AND Improdutivo=@Improdutivo", os);
                if (results > 0) return true;
            }
            return false;
        }

        public List<Servico> GetServicos()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<Servico>($"SELECT * FROM Servicos2").ToList();
                return output;
            }
        }

        public List<Servico> GetServicos(string nomeLocal, int codigoOS, int boleean, int numeroEquipe, DateTime startData, DateTime endData)
        {
            string query = "SELECT * FROM Servicos WHERE";
            if (!string.IsNullOrEmpty(nomeLocal))
            {
                query += $" NomeLocal = '{nomeLocal}' AND";
            }

            if (codigoOS != 0)
            {
                query += $" CodigoOS = '{codigoOS}' AND";
            }
            else
            {

                bool improdutivo;
                if (boleean == 1)
                {
                    improdutivo = false;
                    query += $" Improdutivo = '{improdutivo}' AND";
                }
                else if (boleean == 2)
                {
                    improdutivo = true;
                    query += $" Improdutivo = '{improdutivo}' AND";
                }
            }

            if (numeroEquipe != 0)
            {
                query += $" NumeroEquipe = '{numeroEquipe}' AND";
            }
            TimeSpan nDays = new TimeSpan(1,0,0,0);
            DateTime EndData = endData.Add(nDays);
            query += " Data BETWEEN '"+startData.ToString("yyyy-MM-dd")+"' AND '"+EndData.ToString("yyyy-MM-dd")+"'";

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                var output = connection.Query<Servico>(query).ToList();
                return output;
            }

            /*
            if (codigoOS == 0 && boleean == 1)
            {
                using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
                {
                    var output = connection.Query<Servico>($"SELECT * FROM Servicos WHERE NomeLocal = '{nomeLocal}' AND Improdutivo = 'False' AND NumeroEquipe = '{numeroEquipe}' AND Data > '{startData} AND Data < '{endData}").ToList();
                    return output;
                }
            }
            else if (codigoOS == 0 && boleean == 2)
            {
                using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
                {
                    var output = connection.Query<Servico>($"SELECT * FROM Servicos WHERE NomeLocal = '{nomeLocal}' AND Improdutivo = 'True' AND NumeroEquipe = '{numeroEquipe}' AND Data > '{startData} AND Data < '{endData}").ToList();
                    return output;
                }
            }
            else if (codigoOS == 0 && boleean == 0)
            {
                using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
                {
                    var output = connection.Query<Servico>($"SELECT * FROM Servicos WHERE NomeLocal = '{nomeLocal}' AND NumeroEquipe = '{numeroEquipe}' AND Data > '{startData} AND Data < '{endData}").ToList();
                    return output;
                }
            }
            else
            {
                using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
                {
                    var output = connection.Query<Servico>($"SELECT * FROM Servicos WHERE NomeLocal = '{nomeLocal}' AND CodigoOS = '{codigoOS}' AND NumeroEquipe = '{numeroEquipe}' AND Data > '{startData} AND Data < '{endData}").ToList();
                    return output;
                }
            }

            */
        }

        public void InsertServico(string nomeLocal, int codigoOS, string descricaoOS, bool improdutivo, int numeroEquipe, string executor, DateTime data, int qtda, float tempoExecucao)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                Servico novoServico = new Servico { NomeLocal = nomeLocal, CodigoOS = codigoOS, DescricaoOS = descricaoOS, Improdutivo = improdutivo, NumeroEquipe = numeroEquipe, Executor = executor, Data = data, Quantidade=qtda, TempoExecucao = tempoExecucao };
                connection.Execute("INSERT INTO Servicos2 (NomeLocal, CodigoOS, DescricaoOS, Improdutivo, NumeroEquipe, Executor, Data, Quantidade, TempoExecucao) VALUES(@NomeLocal, @CodigoOS, @DescricaoOS, @Improdutivo, @NumeroEquipe, @Executor, @Data, @Quantidade, @TempoExecucao)", novoServico);
            }
        }

        public async Task<bool> DeleteServico(string nomeLocal, int codigoOS, string descricaoOS, bool improdutivo, int numeroEquipe, string executor, DateTime data, int qtda, float tempoExecucao)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("DB")))
            {
                Servico servico = new Servico { NomeLocal = nomeLocal, CodigoOS = codigoOS, DescricaoOS = descricaoOS, Improdutivo = improdutivo, NumeroEquipe = numeroEquipe, Executor = executor, Data = data, Quantidade = qtda, TempoExecucao = tempoExecucao };
                var results = await connection.ExecuteAsync("DELETE Servicos2 Where NomeLocal=@NomeLocal AND CodigoOS = @CodigoOS AND DescricaoOS=@DescricaoOS AND Improdutivo=@Improdutivo AND NumeroEquipe=@NumeroEquipe AND Executor=@Executor AND Data=@Data AND Quantidade=@Quantidade AND TempoExecucao=@TempoExecucao", servico);
                if (results > 0) return true;
            }
            return false;
        }
    }
}