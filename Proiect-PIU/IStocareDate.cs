using System.Collections.Generic;

namespace GestiuneFinante
{
    public interface IStocareDate
    {
        void Salveaza(List<Tranzactie> tranzactii);
        List<Tranzactie> Incarca();
    }
}