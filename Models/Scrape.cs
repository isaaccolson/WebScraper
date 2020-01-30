using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockScraper.Models
{
    public class Scrape
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Change { get; set; }
        public string Volume { get; set; }
        public string DayRange { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

    }
}
