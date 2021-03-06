﻿using CoffeeSlotMachine.Core.Contracts;
using CoffeeSlotMachine.Core.Entities;
using CoffeeSlotMachine.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CoffeeSlotMachine.Core.Logic
{
    /// <summary>
    /// Verwaltet einen Bestellablauf. 
    /// </summary>
    public class OrderController : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICoinRepository _coinRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderController()
        {
            _dbContext = new ApplicationDbContext();

            _coinRepository = new CoinRepository(_dbContext);
            _orderRepository = new OrderRepository(_dbContext);
            _productRepository = new ProductRepository(_dbContext);
        }
        /// <summary>
        /// Gibt alle Produkte sortiert nach Namen zurück
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProducts()
        {
            return _productRepository.GetAllProducts().OrderBy(o => o.Name);
        }
        /// <summary>
        /// Eine Bestellung wird für das Produkt angelegt.
        /// </summary>
        /// <param name="product"></param>
        public Order OrderCoffee(Product product)
        {
            return new Order
            {
                Product = product,
                Time = DateTime.Now
            };
        }
        /// <summary>
        /// Münze einwerfen. 
        /// Wurde zumindest der Produktpreis eingeworfen, Münzen in Depot übernehmen
        /// und für Order Retourgeld festlegen. Bestellug abschließen.
        /// </summary>
        /// <returns>true, wenn die Bestellung abgeschlossen ist</returns>
        public bool InsertCoin(Order order, int coinValue)
        {
            if (order.InsertCoin(coinValue))
            {
                order.FinishPayment(GetCoinDepot());
                order.Product.Orders.Add(order);

                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gibt den aktuellen Inhalt der Kasse, sortiert nach Münzwert absteigend zurück
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Coin> GetCoinDepot()
        {
            IEnumerable<Coin> coins;

            coins= _coinRepository.GetAllCoins().OrderByDescending(o => o.CoinValue);
            return coins;
        }
        /// <summary>
        /// Gibt den Inhalt des Münzdepots als String zurück
        /// </summary>
        /// <returns></returns>
        public string GetCoinDepotString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Coin coin in GetCoinDepot())
            {
                if (stringBuilder.Length == 0)
                {
                    stringBuilder.Append($"{coin.Amount}*{coin.CoinValue}");
                }
                else
                {
                    stringBuilder.Append($" + {coin.Amount}*{coin.CoinValue}");
                }
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// Liefert alle Orders inkl. der Produkte zurück
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> GetAllOrdersWithProduct()
        {
            return _orderRepository.GetAllWithProduct();
        }
        /// <summary>
        /// IDisposable:
        ///
        /// - Zusammenräumen (zB. des ApplicationDbContext).
        /// </summary>
        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
