using CoffeeSlotMachine.Core.Logic;
using CoffeeSlotMachine.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using ConsoleTables;
using System.Linq;

namespace CoffeeSlotMachine.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            InitDatabase();

            Console.WriteLine(
                "Coffee Slot Machine\n" +
                "===================");

            using (OrderController controller =new OrderController())
            {
                int[] coins = { 5, 10, 20, 50, 100, 200 };
                int sumOfCoins = 0;

                var products = controller.GetProducts();
                for (int i = 1; i <= products.Count(); i++)
                {
                    Console.WriteLine($"{i}: {products.ElementAt(i - 1)}");
                }
                Console.WriteLine();
                Console.Write("Bitte wählen Sie Ihr Produkt anhand der Nummer: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                var choosenProduct = products.ElementAt(choice - 1);

                var order = controller.OrderCoffee(choosenProduct);

                Console.WriteLine();
                Console.WriteLine($"Ihre Auswahl: {order.Product}");
                Console.WriteLine();

                Console.WriteLine("Folgende Münzen werden akzeptiert:");
                for (int i = 1; i <= coins.Length; i++)
                {
                    Console.WriteLine($"{i}: {coins[i - 1]}");
                }

                
                Console.Write("Bitte wählen Sie eine Münze anhand der Nummer: ");
                choice = Convert.ToInt32(Console.ReadLine());

                int choosenCoin = coins[choice - 1];

                //sumOfCoins += choosenCoin;

                bool isFinished = false;

                while (!isFinished)
                {
                    isFinished = controller.InsertCoin(order, choosenCoin);

                    if (!isFinished)
                    {
                        Console.WriteLine("Preis noch nicht erreicht!");
                        Console.Write("Bitte wählen Sie eine weitere Münze: ");
                        choice = Convert.ToInt32(Console.ReadLine());
                        choosenCoin = coins[choice - 1];

                        //sumOfCoins += choosenCoin;
                    }
                }

                int returnValue = order.ReturnCents;

                Console.WriteLine($"Sie bekommen {returnValue} Cents zurück");

                string coinString = controller.GetCoinDepotString();

                Console.WriteLine(coinString);



                //Console.WriteLine("übersicht der Produkte: ");
                //foreach (var item in products)
                //{
                //    Console.WriteLine(item);
                //}

                //Console.WriteLine(ConsoleTable.From(products)
                //        .Configure(c => c.NumberAlignment = Alignment.Right));

                //Console.WriteLine("line");


                //Console.WriteLine(
                //    ConsoleTable
                //        .From(products)                        
                //        .Configure(c => c.NumberAlignment = Alignment.Right)
                //        .ToStringAlternative());


                //Console.WriteLine("Bitte wählen Sie ihr Produkt");
                //foreach (var item in products)
                //{
                //    Console.WriteLine(item);
                //}
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static void InitDatabase()
        {
            using (ApplicationDbContext applicationDbContext = new ApplicationDbContext())
            {
                applicationDbContext.Database.EnsureDeleted();
                applicationDbContext.Database.Migrate();
            }
        }
    }
}
