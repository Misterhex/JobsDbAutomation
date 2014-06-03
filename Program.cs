using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Firefox;
using NLog;

namespace JobsDbAutomation
{
    class Program
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            _logger.Info("job running");

            var keywords = new string[] { "C#", ".net", "asp .net", "asp", "dotnet", "agile", "scrum", "software engineer", "senior software engineer", "web developer" };
            keywords.ToList().ForEach(i => ApplyKeyword(i).Wait());


            _logger.Info("job completed");
        }

        private static async Task ApplyKeyword(string keyword)
        {
            FirefoxDriver driver = new FirefoxDriver();
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(6));

                driver.Url = "http://sg.jobsdb.com/SG/en/Login/JobSeekerLogin";

                var username = driver.FindElementById("c_JbSrLnItDap_El0");
                var password = driver.FindElementById("c_JbSrLnItDap_Pd0");

                username.SendKeys("");
                password.SendKeys("");

                driver.FindElementById("LoginButton").Click();

                driver.FindElementById("MyJobsDBUserNameContainer");

                _logger.Info("logged in");

                driver.Url = "http://sg.jobsdb.com/sg";

                var searchBox = driver.FindElementById("keywordInput");
                searchBox.SendKeys(keyword);

                driver.FindElementById("searchbox-submit").Click();
                driver.FindElementById("searchbox-submit").Click();

                _logger.Info("searching jobs");

                var singleJobApplyPageFormat = "http://sg.jobsdb.com/SG/EN/Job/SelectCoverLetterAndResume?jobAdIdList={0}";
                var joblinks = driver.FindElementsByCssSelector(".posLink").ToArray().Select(e =>
                {
                    var querystring = e.GetAttribute("href").Substring(e.GetAttribute("href").IndexOf('?'));

                    System.Collections.Specialized.NameValueCollection parameters =
                       System.Web.HttpUtility.ParseQueryString(querystring);

                    var jobLink = string.Format(singleJobApplyPageFormat, parameters["jobsIdList"]);
                    return jobLink;
                }).ToArray();


                _logger.Info("found {0} jobs", joblinks.Length);

                foreach (var e in joblinks)
                {
                    try
                    {

                        driver.Url = e;

                        driver.FindElementById("SelectResumesForApplication_Radio_400003013489293").Click();

                        driver.FindElementById("SubmitApplicationButton").Click();

                        driver.FindElementByClassName("content_w745");

                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                    catch { }

                    _logger.Info("apply job {0} successful", e);
                }

            }
            catch (Exception ex) { _logger.Fatal(ex.Message); }
            finally { driver.Dispose(); }
        }
    }
}
