using SQLite;

namespace AppMinhasCompras.Models
{
    public class Produto
    {
        [PrimaryKey, AutoIncrement]
        public string Id { get; set; }
        public string Desricao { get; set; }
        public double Quantidade { get; set; }
        public double Preco {  get; set; }
    }
}
