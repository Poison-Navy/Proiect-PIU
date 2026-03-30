using System.Collections.Generic;
using System.Linq;

namespace GestiuneFinante
{
    public class GestiuneFinanciara
    {
        private List<Tranzactie> _tranzactii;
        private readonly IStocareDate _stocare;

        public GestiuneFinanciara(IStocareDate stocare)
        {
            _stocare = stocare;
            // Încărcăm datele din fișier prin nivelul de stocare
            _tranzactii = _stocare.Incarca() ?? new List<Tranzactie>();
        }

        public void AdaugaTranzactie(Tranzactie t)
        {
            _tranzactii.Add(t);
            // Salvăm automat după fiecare adăugare
            _stocare.Salveaza(_tranzactii);
        }

        public List<Tranzactie> ObtineToate() => _tranzactii;

        // METODA 1: Lipsea sau avea alt nume (eroarea la linia 39 și 114)
        public decimal ObtineBalantaTotala()
        {
            var venituri = _tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma);
            var cheltuieli = _tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);
            return venituri - cheltuieli;
        }

        // METODA 2: Lipsea (eroarea la linia 82)
        public Dictionary<TipTranzactie, Dictionary<CategorieTranzactie, decimal>> ObtineRaportComplet()
        {
            return _tranzactii
                .GroupBy(t => t.Tip)
                .ToDictionary(
                    gTip => gTip.Key,
                    gTip => gTip.GroupBy(t => t.Categorie)
                                .ToDictionary(gCat => gCat.Key, gCat => gCat.Sum(t => t.Suma))
                );
        }
    }
}