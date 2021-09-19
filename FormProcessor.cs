using System;
using System.Collections.Generic;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace NotifyIRPAppointment
{
    public class FormProcessor
    {
        public void GetAppointment()
        {
            //var mDriver = new ChromeDriver("D:\\src\\ChromeWebDriver\\MacOS");
            var mDriver = new ChromeDriver("/Volumes/Storage/src/ChromeWebDriver/MacOs");
            mDriver.Url = "https://burghquayregistrationoffice.inis.gov.ie/Website/AMSREG/AMSRegWeb.nsf/AppSelect?OpenForm";
            mDriver.Manage().Window.Maximize();

            mDriver.FindElement(By.XPath("//select[@id='Category']/option[contains(.,'All')]")).Click();
            mDriver.FindElement(By.XPath("//select[@id='SubCategory']/option[contains(.,'All')]")).Click();
            mDriver.FindElement(By.XPath("//select[@id='ConfirmGNIB']/option[contains(.,'Yes')]")).Click();

            var gnibNo = mDriver.FindElement(By.Id("GNIBNo"));
            gnibNo.Clear();
            gnibNo.SendKeys("847198");
            
            var gnibExDt = mDriver.FindElement(By.Id("GNIBExDT"));
            ((IJavaScriptExecutor)mDriver).ExecuteScript("arguments[0].value='07/01/2020'", gnibExDt);

            mDriver.FindElement(By.XPath("//input[@id='UsrDeclaration']")).Click();
            
            mDriver.FindElement(By.XPath("//input[@id='GivenName']")).SendKeys("Erdem");
            mDriver.FindElement(By.XPath("//input[@id='SurName']")).SendKeys("Kemer");

            var dob = mDriver.FindElement(By.Id("DOB"));
            ((IJavaScriptExecutor)mDriver).ExecuteScript("arguments[0].value='09/05/1983'", dob);
            
            mDriver.FindElement(By.XPath("//select[@id='Nationality']/option[contains(.,'Turkey, Republic of')]")).Click();
            
            mDriver.FindElement(By.XPath("//input[@id='Email']")).SendKeys("erdemkemer@gmail.com");
            mDriver.FindElement(By.XPath("//input[@id='EmailConfirm']")).SendKeys("erdemkemer@gmail.com");

            mDriver.FindElement(By.XPath("//select[@id='FamAppYN']/option[contains(.,'Yes')]")).Click();
            
            mDriver.FindElement(By.XPath("//select[@id='FamAppNo']/option[contains(.,'2')]")).Click();
            
            mDriver.FindElement(By.XPath("//select[@id='PPNoYN']/option[contains(.,'Yes')]")).Click();

            mDriver.FindElement(By.XPath("//input[@id='PPNo']")).SendKeys("U06114646");
            
            mDriver.FindElement(By.XPath("//button[@id='btLook4App']")).Click();

            mDriver.FindElement(By.XPath("//select[@id='AppSelectChoice']/option[contains(.,'closest to today')]")).Click();
            
            mDriver.FindElement(By.XPath("//button[@id='btSrch4Apps']")).Click();
            
            var table = mDriver.FindElement(By.XPath("//div[@id='dvAppOptions']"));
            ICollection<IWebElement> appointments = table.FindElements(By.XPath("//div[@class='appOption']"));
            foreach (var appointment in appointments)
            {
                var button = appointment.FindElement(By.XPath("//button"));
                var date = appointment.FindElement(By.XPath("//td[2]"));

                var appDate = date.GetAttribute("innerText").ToLower();
                var exactDate = appDate.Substring(0, appDate.IndexOf('-')).Trim();
                CultureInfo provider = CultureInfo.InvariantCulture;
                var exactDateInDate = DateTime.ParseExact(exactDate,"d MMMM yyyy", provider);
                
                Console.WriteLine($"Found the reservation on {exactDateInDate.ToShortDateString()}");
                if ((exactDateInDate - DateTime.Today).TotalDays < 35)
                {
                    Console.WriteLine($"Booking the reservation on {exactDateInDate.ToShortDateString()}");
                    ((IJavaScriptExecutor)mDriver).ExecuteScript("window.focus();");
                    break;
                }
            }

            Console.Read();
            mDriver.Quit();
        }
    }
}