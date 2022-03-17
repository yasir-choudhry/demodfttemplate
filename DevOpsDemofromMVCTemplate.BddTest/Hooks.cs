using AutomationFrameworkCore;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using BoDi;
using DevOpsDemofromMVCTemplate.BddTest.PageObjects;
using DevOpsDemofromMVCTemplate.BddTest.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow;

namespace DevOpsDemofromMVCTemplate.BddTest
{
    [Binding]
    public sealed class Hooks
    {
        //Global Variable for Extend report
        private static ExtentTest _featureName;
        private static ExtentTest _scenario;
        private static AventStack.ExtentReports.ExtentReports _extent;

        private readonly IObjectContainer _objectContainer;
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;

        private IWebDriver _driver;
        private Check _check;
        private DataService _dataService;
        private Home _home;
        private Login _login;
        private PageAction _pageaction;
        private PageOps _pageops;
        private IConfiguration _config;
        private DapperUnitOfWork _uow;

        public Hooks(IObjectContainer objectContainer, ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _objectContainer = objectContainer;
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;

            _config = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json", optional: true)
                .Build();
        }

        [BeforeTestRun]
        public static void InitializeReport()
        {
            //Initialize Extent report before test starts
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            var htmlReporter = new ExtentHtmlReporter(dir + "/ExtentReport.html");
            htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;

            _extent = new AventStack.ExtentReports.ExtentReports();
            _extent.AttachReporter(htmlReporter);

        }

        public static string Capture(IWebDriver _driver, String StringShotName)
        {
            ITakesScreenshot ts = (ITakesScreenshot)_driver;
            Screenshot screenshot = ts.GetScreenshot();
            string path = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            string uppath = path.Substring(0, path.LastIndexOf("bin")) + "Screenshots\\" + StringShotName + ".png";
            string localpath = new Uri(uppath).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            return localpath;
        }

        [AfterTestRun]
        public static void TearDownReport()
        {
            //Flush report once test completes
            UploadExtenReportFilesToGcpBucket();
            _extent.Flush();
        }

        public static void UploadExtenReportFilesToGcpBucket()
        {
            //string CredentialKeyFilePath = GetFilePath("RvssDevServiceAccountCredentials.json");
            //string ExtentFileIndexPath = GetFilePath("index.html");
            //string ExtentFileDashboardPath = GetFilePath("dashboard.html");

            //GoogleCredential credential = null;
            //using (var jsonStream = new FileStream(CredentialKeyFilePath, FileMode.Open,
            //    FileAccess.Read, FileShare.Read))
            //{
            //    credential = GoogleCredential.FromStream(jsonStream);
            //}

            //var storageClient = StorageClient.Create(credential);

            //using (var fileStream = File.OpenRead(ExtentFileIndexPath))
            //{
            //    storageClient.UploadObject("dft-cdg-rvss-extent-reports", "index.html", null, fileStream);
            //}

            //using (var fileStream = File.OpenRead(ExtentFileDashboardPath))
            //{
            //    storageClient.UploadObject("dft-cdg-rvss-extent-reports", "dashboard.html", null, fileStream);
            //}
        }

        public static string GetFilePath(string filename)
        {
            return GetFilePath(Environment.CurrentDirectory, filename);
        }

        public static string GetFilePath(string path, string filename)
        {
            return path + @"/" + filename;
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext fC)
        {
            //Create dynamic feature name
            _featureName = _extent.CreateTest<Feature>(fC.FeatureInfo.Title);
        }

        [AfterStep]
        public void InsertReportingSteps()
        {
            var stepType = _scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();

            if (_scenarioContext.TestError == null)
            {
                if (stepType == "Given")
                    _scenario.CreateNode<Given>(_scenarioContext.StepContext.StepInfo.Text);
                else if (stepType == "When")
                    _scenario.CreateNode<When>(_scenarioContext.StepContext.StepInfo.Text);
                else if (stepType == "Then")
                    _scenario.CreateNode<Then>(_scenarioContext.StepContext.StepInfo.Text);
                else if (stepType == "And")
                    _scenario.CreateNode<And>(_scenarioContext.StepContext.StepInfo.Text);
            }

            else if (_scenarioContext.TestError != null)
            {
                string screenshotpath = Capture(_driver, "Screenshot" + DateTime.Now.ToFileTime());

                if (stepType == "Given")
                    _scenario.CreateNode<Given>(_scenarioContext.StepContext.StepInfo.Text).Fail(_scenarioContext.TestError.InnerException, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotpath).Build());
                else if (stepType == "When")
                    _scenario.CreateNode<When>(_scenarioContext.StepContext.StepInfo.Text).Fail(_scenarioContext.TestError.InnerException, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotpath).Build());
                else if (stepType == "Then")
                    _scenario.CreateNode<Then>(_scenarioContext.StepContext.StepInfo.Text).Fail(_scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotpath).Build());
            }

            ////Pending Status
            if (_scenarioContext.ScenarioExecutionStatus.ToString() == "StepDefinitionPending")
            {
                if (stepType == "Given")
                    _scenario.CreateNode<Given>(_scenarioContext.StepContext.StepInfo.Text).Skip("Step Definition Pending");
                else if (stepType == "When")
                    _scenario.CreateNode<When>(_scenarioContext.StepContext.StepInfo.Text).Skip("Step Definition Pending");
                else if (stepType == "Then")
                    _scenario.CreateNode<Then>(_scenarioContext.StepContext.StepInfo.Text).Skip("Step Definition Pending");
            }
        }


        [BeforeScenario]
        public void Initialize()
        {
            string browserType = Environment.GetEnvironmentVariable("BROWSER_TYPE");
            if (!string.IsNullOrEmpty(browserType))
            {
                Enum.TryParse(browserType, out BrowserType browser);
                SelectBrowser(browser);
            }
            else
            {
                SelectBrowser(BrowserType.Chrome);
            }

            _scenario = _featureName.CreateNode<Scenario>(_scenarioContext.ScenarioInfo.Title);

            //TODO: implement logic that has to run before executing each scenario
        }

        [AfterScenario]
        public void CleanUp()
        {
            _driver.Close();
            _driver.Quit();
        }

        internal void SelectBrowser(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.RemoteChrome:
                    ChromeOptions options = new ChromeOptions();

                    options.AddArguments(new List<string>() {
                    "--ignore-certificate-errors","--allow-insecure-localhost"});
                    Console.WriteLine(options.ToCapabilities());

                    options.AddArguments("--no-sandbox");
                    options.AddArguments("--disable-dev-shm-usage");

                    //options.AddArgument("--headless");
                    //options.AddArguments(new List<string>() {
                    //"--silent-launch",
                    //"--no-startup-window",
                    //"no-sandbox",
                    //"headless",});

                    string HUB_HOST = Environment.GetEnvironmentVariable("HUB_HOST") ?? "localhost";
                    string HUB_PORT = Environment.GetEnvironmentVariable("HUB_PORT") ?? "44344";
                    string APP_PROTOCOL = Environment.GetEnvironmentVariable("APP_PROTOCOL") ?? "https";
                    string APP_HOST = Environment.GetEnvironmentVariable("APP_HOST") ?? "dftmvcwithbddtemplate";
                    string APP_PORT = Environment.GetEnvironmentVariable("APP_PORT") ?? "8443";

                    _driver = new RemoteWebDriver(new Uri("http://" + HUB_HOST + ":" + HUB_PORT + "/wd/hub/"), options);
                    //_driver = new RemoteWebDriver(new Uri("https://host.docker.internal:8080/"), options);
                    //_driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444/wd/hub/"), options);

                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    _check = new Check(_driver);
                    _pageaction = new PageAction(_driver, _check);

                    _home = new Home(_driver, APP_PROTOCOL + "://" + APP_HOST + ":" + APP_PORT);
                    //_home = new Home(_driver, "http://dotnetcore5withbdd:443");
                    //_home = new Home(_driver, "https://host.docker.internal:8080/");
                    //_home = new Home(_driver, "https://dft-ddt-sb-stevenmgriffiths.ew.r.appspot.com/");

                    _login = new Login(_driver, _home);
                    //_uow = new DapperUnitOfWork(_config.GetConnectionString("DefaultConnection"));
                    //_dataService = new DataService(_uow);
                    _pageops = new PageOps(_pageaction, _driver, _check, _home, _dataService);

                    //_objectContainer.RegisterInstanceAs<DataService>(_dataService);
                    _objectContainer.RegisterInstanceAs<Check>(_check);
                    _objectContainer.RegisterInstanceAs<Home>(_home);
                    _objectContainer.RegisterInstanceAs<PageAction>(_pageaction);
                    _objectContainer.RegisterInstanceAs<PageOps>(_pageops);
                    _objectContainer.RegisterInstanceAs<Login>(_login);
                    break;

                case BrowserType.Chrome:
                    ChromeOptions option = new ChromeOptions();

                    option.AddArgument("--headless");
                    option.AddArguments(new List<string>() {
                    "--silent-launch",
                    "--no-startup-window",
                    "no-sandbox",
                    "headless",});

                    Driver.Initialise(option);
                    Driver.Instance.Manage().Window.Maximize();

                    _driver = Driver.Instance;
                    _check = new Check(_driver);
                    _pageaction = new PageAction(_driver, _check);
                    _login = new AutomationFrameworkCore.Login(_driver, _home);
                    //_home = new Home(_driver, "https://dft-ddt-dev-dotnetcorerepo.appspot.com/");
                    _home = new Home(_driver, "https://localhost:44344/");
                    //_uow = new DapperUnitOfWork(_config.GetConnectionString("DefaultConnection"));
                    _dataService = new DataService(_uow);
                    _pageops = new PageOps(_pageaction, _driver, _check, _home, _dataService);

                    _objectContainer.RegisterInstanceAs<DataService>(_dataService);
                    _objectContainer.RegisterInstanceAs<Check>(_check);
                    _objectContainer.RegisterInstanceAs<Home>(_home);
                    _objectContainer.RegisterInstanceAs<PageAction>(_pageaction);
                    _objectContainer.RegisterInstanceAs<PageOps>(_pageops);
                    break;

                case BrowserType.Firefox:
                    var driverDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(driverDir, "geckodriver.exe");
                    service.FirefoxBinaryPath = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
                    service.HideCommandPromptWindow = true;
                    service.SuppressInitialDiagnosticInformation = true;
                    _driver = new FirefoxDriver(service);
                    _objectContainer.RegisterInstanceAs<IWebDriver>(_driver);
                    break;

                case BrowserType.IE:
                    break;

                default:
                    break;
            }
        }
    }

    internal enum BrowserType
    {
        Chrome,
        Firefox,
        IE,
        RemoteChrome
    }
}
