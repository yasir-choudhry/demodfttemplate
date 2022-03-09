using AutomationFrameworkCore;
using DevOpsDemofromMVCTemplate.BddTest.PageObjects;
using DevOpsDemofromMVCTemplate.BddTest.Services;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace DevOpsDemofromMVCTemplate.BddTest
{
    public class PageOps
    {
        private readonly PageAction _action;
        private readonly IWebDriver _driver;
        private readonly Check _check;
        private readonly Home _home;
        private readonly DataService _ds;


        public PageOps(PageAction action, IWebDriver driver, Check check, Home home, DataService ds)
        {
            _check = check;
            _action = action;
            _driver = driver;
            _home = home;
            _ds = ds;
        }

    }
}
