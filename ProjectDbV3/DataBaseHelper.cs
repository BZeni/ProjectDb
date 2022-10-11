using System.Data;
using System.Data.SqlClient;

namespace ProjectDbV3
{
    public class DataBaseHelper
    {
        /// <summary>
        /// Criar Banco de Dados no servidor SQL
        /// </summary>
        public static void CreateDb(SqlConnection conn)
        {
            string command = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MarrariDB') BEGIN CREATE DATABASE MarrariDB END;";
            using SqlCommand cmd = new(command, conn);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Criar as tabelas "Lotes" e "Pecas"
        /// </summary>
        /// <param name="conn">Conexão com o banco de dados</param>
        public static void CreateTb(SqlConnection conn)
        {
            string commandCreateTbL = "if not exists (select * from sysobjects where name='LOTES' and xtype='U') CREATE TABLE [dbo].[LOTES]([IdLote] [int] IDENTITY(1, 1) NOT NULL," +
                 "[CodProd] [varchar] (10) NULL,[Descricao] [varchar] (300) NULL," +
                 "CONSTRAINT[PK_Lote] PRIMARY KEY CLUSTERED(IdLote))";

            string commandCreateTbP = "if not exists (select * from sysobjects where name='PECAS' and xtype='U') CREATE TABLE [dbo].[PECAS]([IdPeca] [int] IDENTITY(1, 1) NOT NULL," +
                "[IdLote] [int] NULL,[ALTURA][int] NOT NULL,[LARGURA] [int] NOT NULL,[COMPRIMENTO] [int] NULL," +
                "CONSTRAINT[PK_Peca] PRIMARY KEY CLUSTERED(IdPeca), CONSTRAINT[FK_Pecas_IdLote] FOREIGN KEY (IdLote) REFERENCES [dbo].[LOTES] ([IdLote]))";

            using SqlCommand CreateTbLotes = new(commandCreateTbL, conn);
            using SqlCommand CreateTbPecas = new(commandCreateTbP, conn);
            CreateTbLotes.ExecuteNonQuery();
            CreateTbPecas.ExecuteNonQuery();
        }

        /// <summary>
        /// Realizar a busca da quantidade de Peças no banco de dados
        /// </summary>
        /// <param name="conn">Conexão com o banco de dados</param>
        /// <param name="digitoBusca">Parametro de pesquisa</param>
        /// <returns>Caso tenha registros irá retorna-los, caso contrário retornará null</returns>
        public static List<Lote>? GetBuscaQntPecas(SqlConnection conn, string digitoBusca)
        {
            var command = $"SELECT P.idlote, COUNT(idpeca)as qntPecas FROM pecas as p, lotes as l WHERE l.Codprod = @CodProd and l.IdLote = p.IdLote group by P.idlote";
            var lotes = new List<Lote>();
            using SqlCommand cmd = new(command, conn);
            cmd.Parameters.AddWithValue("@CodProd", digitoBusca);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader[0] == DBNull.Value) { return null; }
                else
                {
                    lotes.Add(
                    new Lote
                    {
                        IdLote = (int)reader["IdLote"],
                        QntPecas = (int)reader["qntPecas"],
                    }
                    );
                }
            }
            return lotes;
        }

        /// <summary>
        /// Realizar a busca da Media das Medidas das Peças no banco de dados pelo Id do lote
        /// </summary>
        /// <param name="conn">Conexão com o banco de dados</param>
        /// <param name="digitoBusca">Parametro de pesquisa</param>
        /// <returns>Caso tenha registros irá retorna-los, caso contrário retornará null</returns>
        public static List<Peca>? GetBuscaMediaMedidasPecas(SqlConnection conn, string digitoBusca)
        {
            var command = $"SELECT AVG(Altura) as MediaAltura, AVG(Largura) as MediaLargura, AVG(Comprimento) as MediaComprimento FROM PECAS, LOTES WHERE PECAS.IdLote = @IdLote and PECAS.IdLote = LOTES.IdLote";
            var pecas = new List<Peca>();
            using SqlCommand cmd = new(command, conn);
            cmd.Parameters.AddWithValue("@IdLote", digitoBusca);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader[0] == DBNull.Value) { return null; }
                else
                {
                    pecas.Add(
                    new Peca
                    {
                        Altura = (int)reader["MediaAltura"],
                        Largura = (int)reader["MediaLargura"],
                        Comprimento = (int)reader["MediaComprimento"]
                    }
                    );
                }
            }
            return pecas;
        }

        /// <summary>
        /// Realizar a busca da Media das Medidas das Peças no banco de dados pelo Código do Produto
        /// </summary>
        /// <param name="conn">Conexão com o banco de dados</param>
        /// <param name="digitoBusca">Parametro de pesquisa</param>
        /// <returns>Caso tenha registros irá retorna-los, caso contrário retornará null</returns>
        public static List<Peca>? GetBuscaMediaMedidasLotes(SqlConnection conn, string digitoBusca)
        {
            var command = $"SELECT AVG(Altura) as MediaAltura, AVG(Largura) as MediaLargura, AVG(Comprimento) as MediaComprimento FROM PECAS, LOTES WHERE LOTES.CodProd = @CodProd and PECAS.IdLote = LOTES.IdLote";
            var pecas = new List<Peca>();
            using SqlCommand cmd = new(command, conn);
            cmd.Parameters.AddWithValue("@CodProd", digitoBusca);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader[0] == DBNull.Value) { return null; }
                else
                {
                    pecas.Add(
                    new Peca
                    {
                        Altura = (int)reader["MediaAltura"],
                        Largura = (int)reader["MediaLargura"],
                        Comprimento = (int)reader["MediaComprimento"]
                    }
                    );
                }
            }
            return pecas;
        }

        /// <summary>
        /// Verificar se há registros na tabela
        /// </summary>
        /// <param name="conn">Conexão com o banco de dados</param>
        /// <returns>Caso não haja nenhum registro, inseri-los</returns>
        public static int VerificarExistenciaRegistro(SqlConnection connMarrariDb)
        {
            string selectCmd = "SELECT CodProd FROM LOTES";
            using (SqlCommand cmd = new(selectCmd, connMarrariDb))
            {
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        var result = 1;
                        return result;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Inserir registros nas tabelas Pecas e Lotes
        /// </summary>
        /// <param name="conn">Conexão com o banco de dados</param>
        public static void InserirRegistros(SqlConnection conn)
        {
            Random rand = new();
            for (int i = 1; i <= 10; i++)
            {
                var CodProd = (rand.Next(1000, 1010));
                string command = $"insert into lotes(codprod) VALUES(@CodProd)";
                using SqlCommand cmd = new(command, conn);
                cmd.Parameters.AddWithValue("@CodProd", CodProd);
                cmd.ExecuteNonQuery();
            }
            string SelectCmd = "SELECT Top 100 ([IdLote]) FROM LOTES ORDER BY IdLote DESC";
            var peca = new List<Peca>();
            using (SqlCommand command = new(SelectCmd, conn))
            {
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value)
                    {
                        peca.Add(
                           new Peca
                           {
                               IdLote = (int)reader["IdLote"]
                           }
                           );
                    }
                }
            }
            foreach (var pecas in peca)
            {
                var idlote = pecas.IdLote;
                for (int j = 1; j <= 50; j++)
                {
                    var altura = (rand.Next(1, 20));
                    var largura = (rand.Next(1, 20));
                    var comprimento = (rand.Next(1, 20));
                    string command = $"INSERT INTO PECAS (IdLote, Altura, Largura, Comprimento) VALUES (@IdLote, @Altura, @Largura, @Comprimento)";
                    using SqlCommand cmd = new(command, conn);
                    cmd.Parameters.AddWithValue("@IdLote", idlote);
                    cmd.Parameters.AddWithValue("@Altura", altura);
                    cmd.Parameters.AddWithValue("@Largura", largura);
                    cmd.Parameters.AddWithValue("@Comprimento", comprimento);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
