using System;

namespace GestiuneFinante
{
    class Program
    {
        static void Main()
        {
            // 1. Inițializăm nivelul de stocare (DAL)
            IStocareDate stocareSistem = new StocareJson();

            // 2. Injectăm stocarea în logica de business
            // Aceasta va încărca automat datele existente la pornire
            GestiuneFinanciara gestiune = new GestiuneFinanciara(stocareSistem);

            bool ruleaza = true;
            Console.WriteLine("=== SISTEM GESTIONARE FINANȚE v2.0 ===");

            while (ruleaza)
            {
                Console.WriteLine("\n1. Adaugă Venit");
                Console.WriteLine("2. Adaugă Cheltuială");
                Console.WriteLine("3. Vezi Balanța Totală");
                Console.WriteLine("4. Raport Detaliat (Categorii)");
                Console.WriteLine("5. Ieșire");
                Console.Write("\nAlege o opțiune: ");

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
                        decimal balanta = gestiune.ObtineBalantaTotala();
                        Console.WriteLine($"\nBalanța curentă: {balanta} RON");
                        break;
                    case "4":
                        AfiseazaRaportCategorii(gestiune);
                        break;
                    case "5":
                        ruleaza = false;
                        Console.WriteLine("Datele au fost asigurate. La revedere!");
                        break;
                    default:
                        Console.WriteLine("Opțiune invalidă!");
                        break;
                }
            }
        }

        static void AdaugaTranzactieUtilizator(GestiuneFinanciara gestiune, TipTranzactie tip)
        {
            Console.WriteLine($"\n--- ADAUGĂ {tip.ToString().ToUpper()} ---");
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
                    gestiune.AdaugaTranzactie(t); // Salvarea se face automat aici
                    Console.WriteLine("Succes! Tranzacția a fost salvată pe disc.");
                }
                else { Console.WriteLine("Eroare: Categorie inexistentă!"); }
            }
            else { Console.WriteLine("Eroare: Suma trebuie să fie un număr!"); }
        }

        static void AfiseazaRaportCategorii(GestiuneFinanciara gestiune)
        {
            var raportComplet = gestiune.ObtineRaportComplet();

            Console.WriteLine("\n========================================");
            Console.WriteLine("       RAPORT FINANCIAR DETALIAT");
            Console.WriteLine("========================================");

            if (raportComplet.Count == 0)
            {
                Console.WriteLine("Nu există tranzacții înregistrate.");
            }

            foreach (var sectiune in raportComplet)
            {
                if (sectiune.Key == TipTranzactie.Venit)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n[ + ] VENITURI:");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[ - ] CHELTUIELI:");
                }
                Console.ResetColor();

                foreach (var categorie in sectiune.Value)
                {
                    Console.WriteLine($"  {categorie.Key,-15} : {categorie.Value,10} RON");
                }
            }

            Console.WriteLine("\n----------------------------------------");
            decimal total = gestiune.ObtineBalantaTotala();
            Console.Write(" STATUS FINAL: ");
            if (total >= 0) Console.ForegroundColor = ConsoleColor.Green; else Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{total} RON");
            Console.ResetColor();
            Console.WriteLine("========================================\n");
        }
    }
}