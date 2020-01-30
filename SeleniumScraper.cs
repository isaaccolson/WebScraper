using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Data.SqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using StockScraper.Models;

namespace StockScraper
{
    public class SeleniumScraper
    {

        private const string DESTINATION_URL = "https://www.msn.com/en-us/money";
        private const int SLEEP_TIME = 500;

        private IWebDriver driver;

        private string emailAddress;
        private string loginPassword;

        private IWebElement scrapedTable;
        public SqlConnection connection;


        public SeleniumScraper(string _emailAddress, string _loginPassword)
        {
            
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--proxy-server='direct://'");
            options.AddArgument("--proxy-bypass-list=*");
            options.AddArgument("--start-maximized");
            options.AddArgument("--headless");


            driver = new ChromeDriver();

            emailAddress = _emailAddress;
            loginPassword = _loginPassword;
              
        }
        public void Connect()
        {

            try
            {
                connection = new SqlConnection("Data Source=LAPTOP-Q1SDF04M\\SQLEXPRESS01;Initial Catalog=msn_scraper;Integrated Security=True");
                connection.Open();

            }
            catch (SqlException ex)
            {

            }

        }

        public void Run()
        {

            NavigateToUrl(DESTINATION_URL);

            LoginToUrl();
           
            NavigateToWatchlist();

            ScrapeTable();

            InsertDataToDatabase();

        }

        private void NavigateToUrl(string url)
        {
            driver.Navigate().GoToUrl(url);
            Thread.Sleep(SLEEP_TIME);
        }

        private void LoginToUrl()
        {

            driver.FindElement(By.XPath("//*[@title='Sign in']")).Click();
            Thread.Sleep(SLEEP_TIME);

            InputEmailAddress(emailAddress);
            Thread.Sleep(SLEEP_TIME);

            InputPassword(loginPassword);

        }

        private void NavigateToWatchlist()
        {

            driver.FindElement(By.XPath("//*[@data-id='28']")).Click();
            Thread.Sleep(SLEEP_TIME * 15);
        }

        private void InputEmailAddress(string _emailAddress)
        {
            driver.FindElement(By.XPath("//*[@type='email']")).SendKeys(_emailAddress);
            Thread.Sleep(SLEEP_TIME);

            HitNextButton();

        }

        private void InputPassword(string _loginPassword)
        {

            driver.FindElement(By.XPath("//*[@name='passwd']")).SendKeys(_loginPassword);
            Thread.Sleep(SLEEP_TIME);

            HitNextButton();
        }

        private void HitNextButton()
        {
            driver.FindElement(By.XPath("//*[@type='submit']")).Click();
            Thread.Sleep(SLEEP_TIME);
        }

        private void ScrapeTable()
        {
            scrapedTable = driver.FindElement(By.XPath("//table[@class='fin-table']/tbody"));
        }


        private void InsertDataToDatabase()
        {
            Connect();

            var rowCount = NumberOfRows();
            var columnCount = NumberOfColumns();

            var cellData = scrapedTable.FindElements(By.TagName("td"));


            for (var i = 0; i < rowCount; i++)
            {

                var rowData = new List<string>();

                for (var j = 0; j < columnCount; j++)
                {

                    rowData.Add(cellData[(i * columnCount) + j].Text);

                }

                var Name = rowData[0];
                Decimal Price;

                if (decimal.TryParse(rowData[1], out decimal _price))
                {
                    Price = _price;
                }
                else
                {
                   
                    Price = -1;
                }



                var Change = rowData[2];
                var Volume = rowData[3];
                var DayRange = rowData[4];
                var Date = DateTime.Today;

                    var insertQuery = string.Format("INSERT INTO Scrape VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')", Name, Price, Change, Volume, DayRange, Date);
                   
                    using var cmd = new SqlCommand(insertQuery, connection);

                    using SqlDataReader rdr = cmd.ExecuteReader();

                
                Thread.Sleep(SLEEP_TIME);
            }
        }

        private int NumberOfColumns()
        {

            return driver.FindElement(By.XPath("//table[@class='fin-table']/tbody/tr"))
                .FindElements(By.TagName("td"))
                .Count;

        }

        private int NumberOfRows()
        {
            return scrapedTable.FindElements(By.TagName("tr")).Count;
        }
    }
}