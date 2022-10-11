using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ProjectDbV3
{
    class Program
    {
        static void Main()
        {
            bool repetir = true;

            if (Validator.IsServerConnected() is false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Falha ao estabelecer conexão! Tente Novamente!");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Conexão estabelecida com sucesso!");
                Console.WriteLine("GERENCIAMENTO DE PRODUTOS MARRARI\n");
                SqlConnection connMaster = new(AppConfigHelper.CnnVal("Master"));
                    connMaster.Open();
                    DataBaseHelper.CreateDb(connMaster);
                    connMaster.Close();

                using SqlConnection connMarrariDb = new(AppConfigHelper.CnnVal("MarrariDb"));
                connMarrariDb.Open();

                DataBaseHelper.CreateTb(connMarrariDb);
                if ((DataBaseHelper.VerificarExistenciaRegistro(connMarrariDb) < 1))
                {
                    DataBaseHelper.InserirRegistros(connMarrariDb);
                }

                do
                {
                    MainMenu();
                    ConsoleKeyInfo keyPressionada = Console.ReadKey(true);
                    if (keyPressionada.Key == ConsoleKey.D1)
                    {
                        QntPecas(connMarrariDb);
                        if (RepetirMenu() is false) { repetir = false; }
                    }
                    else if (keyPressionada.Key == ConsoleKey.D2)
                    {
                        MediaPecas(connMarrariDb);
                        if (RepetirMenu() is false) { repetir = false; }
                    }
                    else if (keyPressionada.Key == ConsoleKey.D3)
                    {
                        MediaLotes(connMarrariDb);
                        if (RepetirMenu() is false) { repetir = false; }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nInválido!");
                        Console.ResetColor();
                        if (RepetirMenu() is false) { repetir = false; }
                    }
                } while (repetir is true);

                connMarrariDb.Close();

                Console.Clear();
                Console.WriteLine("Programa finalizado!");
            }
        }

        /// <summary>
        /// Texto do menu inicial com as opções
        /// </summary>
        private static void MainMenu()
        {
            Console.WriteLine("1) Quantidade de peças");
            Console.WriteLine("2) Média das medidas (Peças)");
            Console.WriteLine("3) Média das medidas (Lotes)");
            Console.Write("\nSelecione uma opção: ");
        }
        /// <summary>
        /// Mensagem de erro caso não encontre na busca
        /// </summary>
        private static void ErroMsg()
        {
            Console.WriteLine("Não foi encontrado o produto com o critério de pesquisa!");
        }
        /// <summary>
        /// Verificar se o usuário deseja repetir o processo
        /// </summary>
        /// <returns>Retornará para a tela de menu caso aperte qualquer tecla, Fechará o programa caso aperte ESC</returns>
        private static bool RepetirMenu()
        {
            Console.WriteLine("\nAperte 'ESC' para encerrar ou qualquer outra tecla para continuar");
            Console.WriteLine("=================================\n");
            ConsoleKeyInfo KeyPressionada = Console.ReadKey(true);
            if (KeyPressionada.Key == ConsoleKey.Escape)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Exibir as medias Altura, Largura e Comprimento
        /// </summary>
        private static void MostrarConsultaMedia()
        {
            Console.WriteLine("\nAltura    Largura    Comprimento ");
        }
        /// <summary>
        /// Mostrar o resultado da busca da quantidade de peças pelo código do produto no banco de dados
        /// </summary>
        /// <param name="connMarrariDb">Conexão com o banco de dados</param>
        private static void QntPecas(SqlConnection connMarrariDb)
        {
            Console.WriteLine("\n=================================");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n QUANTIDADE DE PEÇAS");
            Console.ResetColor();
            Console.Write("\nDigite o Código do produto: ");
            string? digitoBusca = Console.ReadLine();
            if (Validator.ValidarEntradaBusca(digitoBusca) is false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inválido!");
                Console.ResetColor();
            }
            else
            {
                var resultadoBusca = DataBaseHelper.GetBuscaQntPecas(connMarrariDb, digitoBusca);
                if (resultadoBusca != null && resultadoBusca.Count > 0)
                {
                    foreach (var resultados in resultadoBusca)
                    {
                        Console.WriteLine("\nIdLote   QntPecas");
                        Console.WriteLine($"{resultados.IdLote}          {resultados.QntPecas}");
                    }
                }
                else { ErroMsg(); }
            }
        }
        /// <summary>
        /// Mostrar o resultado da busca da Média das medidas das peças pelo id do lote no banco de dados
        /// </summary>
        /// <param name="connMarrariDb">Conexão com banco de dados</param>
        private static void MediaPecas(SqlConnection connMarrariDb)
        {
            Console.WriteLine("\n=================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n MÉDIA DAS MEDIDAS (Peças)");
            Console.ResetColor();
            Console.Write("\nDigite o Id do Lote: ");
            string? digitoBusca = Console.ReadLine();

            if (Validator.ValidarEntradaBusca(digitoBusca) is false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inválido!");
                Console.ResetColor();
            }
            else
            {
                var resultadoBusca = DataBaseHelper.GetBuscaMediaMedidasPecas(connMarrariDb, digitoBusca);
                if (resultadoBusca != null && resultadoBusca.Count > 0)
                {
                    foreach (var resultados in resultadoBusca)
                    {
                        MostrarConsultaMedia();
                        Console.WriteLine(resultados.Media);
                    }
                }
                else { ErroMsg(); }
            }
        }
        /// <summary>
        /// Resultado da busca da Média das medidas das peças pelo Código de produto no banco de dados
        /// </summary>
        /// <param name="connMarrariDb">Conexão com banco de dados</param>
        private static void MediaLotes(SqlConnection connMarrariDb)
        {
            Console.WriteLine("\n=================================");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n MÉDIA DAS MEDIDAS (Lotes)");
            Console.ResetColor();
            Console.Write("\nDigite o Código do Produto: ");
            string? digitoBusca = Console.ReadLine();
            if (Validator.ValidarEntradaBusca(digitoBusca) is false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inválido!");
                Console.ResetColor();
            }
            else
            {
                var resultadoBusca = DataBaseHelper.GetBuscaMediaMedidasLotes(connMarrariDb, digitoBusca);
                if (resultadoBusca != null && resultadoBusca.Count > 0)
                {
                    foreach (var resultados in resultadoBusca)
                    {
                        MostrarConsultaMedia();
                        Console.WriteLine(resultados.Media);
                    }
                }
                else { ErroMsg(); }
            }
        }
    }
}