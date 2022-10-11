using System.Configuration;
using System.Data.SqlClient;

namespace ProjectDbV3
{
    public class Validator
    {
        /// <summary>
        /// Validar os digitos de busca
        /// </summary>
        /// <param name="digitoBusca">Opção de busca desejada pelo usuário</param>
        /// <returns>>Continuará com a busca caso true, mensagem de erro caso false</returns>
        public static bool ValidarEntradaBusca(string digitoBusca)
        {
            if (string.IsNullOrWhiteSpace(digitoBusca))
            {
                return false;
            }
            foreach (char c in digitoBusca)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Valida se a conexão foi estabelecida
        /// </summary>
        /// <returns>continuidade do programa com o banco caso haja conexão, e mensagem de erro caso falhe a conexão</returns>
        public static bool IsServerConnected()
        {
            SqlConnection connection = new(AppConfigHelper.CnnVal("Master"));
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }
    }
}

