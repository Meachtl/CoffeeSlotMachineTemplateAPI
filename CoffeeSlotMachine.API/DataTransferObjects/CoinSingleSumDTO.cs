using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeSlotMachine.API.DataTransferObjects
{
    public class CoinSingleSumDTO
    {
        public int Value { get; set; }
        public int Amount { get; set; }
        public int Sum { get; set; }

    }
}
