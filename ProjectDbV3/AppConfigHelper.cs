using System.Configuration;

namespace ProjectDbV3
{
    public class AppConfigHelper
    {
        /// <summary>
        /// Realizar a conexão com o banco de dados com ConnectionString no app.config
        /// </summary>
        /// <param name="name">Nome da ConnectionString</param>
        /// <returns>ConnectionString para Conexão</returns>
        public static string CnnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}