using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeSlotMachine.Core.Entities;
using CoffeeSlotMachine.Core.Logic;
using Microsoft.AspNetCore.Mvc;
using CoffeeSlotMachine.API.DataTransferObjects;

namespace CoffeeSlotMachine.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly OrderController _controller;

        public ValuesController()
        {
            _controller = new OrderController();
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("listAll")]
        public IEnumerable<CoinDTO> GetAmountOfCoins()
        {            
            var coins = _controller.GetCoinDepot();
            var result = coins
                .Select(x => new CoinDTO
                {
                    Amount = x.Amount,
                    Value = x.CoinValue
                }).OrderByDescending(o => o.Value).ToList();
            return result;
        }

        [HttpGet]
        [Route("{value}/sum")]
        public IEnumerable<CoinSingleSumDTO> GetSumOfValue(int value)
        {
            var coins = _controller.GetCoinDepot();

            var result = coins.
                Where(w => w.CoinValue == value)
                .Select(x => new CoinSingleSumDTO
                {
                    Amount = x.Amount,
                    Value = x.CoinValue,
                    Sum=x.Amount*x.CoinValue

                }).ToList();
            return result;
        }

        [HttpGet]
        [Route("totalSum")]
        public (IEnumerable<CoinTotalSumDTO> Values, int TotalSum) GetTotalSum()
        {
            var coins = _controller.GetCoinDepot();

            var result = coins
                .Select(x => new CoinTotalSumDTO
                {
                    Amount = x.Amount,
                    Value = x.CoinValue,
                    Sum = x.Amount * x.CoinValue
                }).ToList();

            int totalSum = result.Sum(s => s.Sum);

            return (Values:result, TotalSum:totalSum);
        }
    }
}
