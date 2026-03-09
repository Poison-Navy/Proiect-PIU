using System.Linq;
using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;

namespace GestiuneFinante
{
    class Program
    {
        static void Main()
        {
            GestiuneFinanciara gestiune = new GestiuneFinanciara();
            bool ruleaza = true;

            Console.WriteLine("=== SISTEM GESTIONARE VENITURI/CHELTUIELI ===");

            while (ruleaza)
            {
                Console.WriteLine("\n1. Adauga Venit");
                Console.WriteLine("2. Adauga Cheltuiala");
                Console.WriteLine("3. Vezi Balanta Totala");
                Console.WriteLine("4. Raport Cheltuieli pe Categorii");
                Console.WriteLine("5. Iesire");
                Console.Write("\nAlege o optiune: ");

                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        AdaugaTranzactieUtilizator(gestiune, TipTranzactie.Venit);
                        break;
                    case "2":
                        AdaugaTranzactieUtilizator(gestiune, TipTranzactie.Cheltuiala);
                        break;
                    case "3":
                        Console.WriteLine($"\nBalanta curenta: {gestiune.ObtineBalantaTotala()} RON");
                        break;
                    case "4":
                        AfiseazaRaportCategorii(gestiune);
                        break;
                    case "5":
                        ruleaza = false;
                        Console.WriteLine("Datele au fost salvate. La revedere!");
                        break;
                    default:
                        Console.WriteLine("Optiune invalida!");
                        break;
                }
            }
        }

        static void AdaugaTranzactieUtilizator(GestiuneFinanciara gestiune, TipTranzactie tip)
        {
            Console.Write("Suma: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal suma))
            {
                Console.WriteLine("Categorii disponibile: " + string.Join(", ", Enum.GetNames(typeof(CategorieTranzactie))));
                Console.Write("Alege Categoria: ");

                if (Enum.TryParse(Console.ReadLine(), true, out CategorieTranzactie cat))
                {
                    Console.Write("Descriere (opțional): ");
                    string desc = Console.ReadLine();

                    var t = new Tranzactie(suma, cat, tip, desc);
                    gestiune.AdaugaTranzactie(t);
                    Console.WriteLine("Succes!");
                }
                else { Console.WriteLine("Categorie inexistenta!"); }
            }
            else { Console.WriteLine("Suma invalida!"); }
        }

        static void AfiseazaRaportCategorii(GestiuneFinanciara gestiune)
        {
            var raport = gestiune.RaportPeCategorii();
            Console.WriteLine("\n--- CHELTUIELI PE CATEGORII ---");
            foreach (var item in raport)
            {
                Console.WriteLine($"{item.Key}: {item.Value} RON");
            }
        }
    }
}

public static class DataPersistence
{
    private const string FileName = "date_financiare.json";

    public static void Salveaza(List<Tranzactie> date)
    {
        string json = JsonSerializer.Serialize(date);
        File.WriteAllText(FileName, json);
    }

    public static List<Tranzactie> Incarca()
    {
        if (!File.Exists(FileName)) return new List<Tranzactie>();
        string json = File.ReadAllText(FileName);
        return JsonSerializer.Deserialize<List<Tranzactie>>(json);
    }
}

public class GestiuneFinanciara
{
    private List<Tranzactie> _tranzactii = new List<Tranzactie>();

    public void AdaugaTranzactie(Tranzactie t) => _tranzactii.Add(t);

    public decimal ObtineBalantaTotala()
    {
        var venituri = _tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma);
        var cheltuieli = _tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);
        return venituri - cheltuieli;
    }

    public List<Tranzactie> FiltreazaDupaLuna(int luna, int an)
    {
        return _tranzactii.Where(t => t.Data.Month == luna && t.Data.Year == an).ToList();
    }

    public Dictionary<CategorieTranzactie, decimal> RaportPeCategorii()
    {
        return _tranzactii
            .Where(t => t.Tip == TipTranzactie.Cheltuiala)
            .GroupBy(t => t.Categorie)
            .ToDictionary(g => g.Key, g => g.Sum(t => t.Suma));
    }
}

public class Tranzactie
{
    public Guid Id { get; private set; }
    public decimal Suma { get; set; }
    public DateTime Data { get; set; }
    public CategorieTranzactie Categorie { get; set; }
    public TipTranzactie Tip { get; set; }
    public string Descriere { get; set; }

    public Tranzactie(decimal suma, CategorieTranzactie categorie, TipTranzactie tip, string descriere = "")
    {
        Id = Guid.NewGuid(); // Generăm un ID unic automat
        Suma = suma;
        Data = DateTime.Now;
        Categorie = categorie;
        Tip = tip;
        Descriere = descriere;
    }
}

public enum CategorieTranzactie
{
    Salariu,
    Freelancing,
    Alimente,
    Utilitati,
    Transport,
    Divertisment,
    Sanatate
}

public enum TipTranzactie
{
    Venit,
    Cheltuiala
}
