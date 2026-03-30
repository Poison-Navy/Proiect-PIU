using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GestiuneFinante
{
    public class StocareJson : IStocareDate
    {
        private readonly string _caleFisier;

        public StocareJson(string numeFisier = "date_financiare.json")
        {
            _caleFisier = numeFisier;
        }

        public void Salveaza(List<Tranzactie> tranzactii)
        {
            string json = JsonSerializer.Serialize(tranzactii, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_caleFisier, json);
        }

        public List<Tranzactie> Incarca()
        {
            if (!File.Exists(_caleFisier)) return new List<Tranzactie>();
            string json = File.ReadAllText(_caleFisier);
            return JsonSerializer.Deserialize<List<Tranzactie>>(json) ?? new List<Tranzactie>();
        }
    }
}