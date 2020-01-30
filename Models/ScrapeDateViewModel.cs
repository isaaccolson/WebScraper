using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockScraper.Models
{
    public class ScrapeDateViewModel
    {
        public List<Scrape> Scrapes { get; set; }
        public SelectList Dates { get; set; }
        public string Date { get; set; }
    }
}
