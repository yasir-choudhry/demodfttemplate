using AutomationFrameworkCore;
using DevOpsDemofromMVCTemplate.BddTest.PageObjects;
using TechTalk.SpecFlow;

namespace DevOpsDemofromMVCTemplate.BddTest.Steps
{
    [Binding]
    public sealed class HomeSteps
    {
        private readonly PageOps _pageOps;
        private readonly PageAction _action;
        private readonly Home _home;
        private readonly Check _check;

        public HomeSteps(Check check, PageAction action, PageOps pageOps, Home home)
        {
            _pageOps = pageOps;
            _action = action;
            _home = home;
            _check = check;
        }

        //HOME PAGE
        [Given(@"I am on the homepage")]
        public void GivenIAmOnTheHomepage()
        {
            _home.GoToHome();
        }

        [Then(@"I should see the text '(.*)'")]
        public void ThenIShouldSeeTheText(string p0)
        {
            _check.IfXPathContainsText(HomeObjects.PageTitle.HomeHeader, p0);
        }

    }
}
