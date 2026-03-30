using System;

namespace GestiuneFinante
{
    public class Tranzactie
    {
        public Guid Id { get; set; }
        public decimal Suma { get; set; }
        public DateTime Data { get; set; }
        public CategorieTranzactie Categorie { get; set; }
        public TipTranzactie Tip { get; set; }
        public string Descriere { get; set; }

        // Constructor pentru JSON Deserialization
        public Tranzactie() { }

        public Tranzactie(decimal suma, CategorieTranzactie categorie, TipTranzactie tip, string descriere = "")
        {
            Id = Guid.NewGuid();
            Suma = suma;
            Data = DateTime.Now;
            Categorie = categorie;
            Tip = tip;
            Descriere = descriere;
        }
    }
}