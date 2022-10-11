namespace ProjectDbV3
{
    public class Lote
    {
        public int IdLote { get; set; }
        public int CodProd { get; set; }
        public string? Descricao { get; set; }
        public int QntPecas { get; set; }
    }
    public class Peca
    {
        public int IdLote { get; set; }
        public int Altura { get; set; }
        public int Largura { get; set; }
        public int Comprimento { get; set; }
        public string Media
        {
            get
            {
                return $"{Altura}         {Largura}         {Comprimento}";
            }
        }
    }
}