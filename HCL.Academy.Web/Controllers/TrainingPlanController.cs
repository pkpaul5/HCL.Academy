using HCL.Academy.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class TrainingPlanController : BaseController
    {
        [Authorize]
        [SessionExpire]
        public ActionResult TrainingPlan()
        {
            return View();
        }

        [SessionExpire]
        public async Task<PartialViewResult> Training()
        {
            InitializeServiceClient();
            HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Training/GetUserTrainingsDetails", req);
            List<UserSkillDetail> traningModules = await trainingResponse.Content.ReadAsAsync<List<UserSkillDetail>>();
            return PartialView("_TrainingPanel", traningModules);
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, string.Empty);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
        
        /// <summary>
        /// Gets the list of trainings for the selected user in a popup.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [SessionExpire]
        public async Task<JsonResult> GetTrainingsFoPopUp()
        {
            InitializeServiceClient();
        
            OnBoardingViewModel boardingViewModel = new OnBoardingViewModel();
        
            HttpResponseMessage response = await client.PostAsJsonAsync("Onboarding/GetBoardingData", req);
            List<OnBoarding> listOnboarding = await response.Content.ReadAsAsync<List<OnBoarding>>();
        
            if (listOnboarding.Count > 0)
            {
                boardingViewModel.topRowList = listOnboarding.ToList().Where((c, i) => i % 2 == 0).ToList();
                boardingViewModel.bottomRowList = listOnboarding.ToList().Where((c, i) => i % 2 != 0).ToList();
                boardingViewModel.bgColorList = GetBgColor(listOnboarding);          
            }
            return new JsonResult { Data = listOnboarding };
        }
        /// <summary>
        /// Gets the backgroud color based on status.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<bgColor> GetBgColor(List<OnBoarding> list)
        {
            List<bgColor> bgColorList = new List<bgColor>();

            foreach (OnBoarding item in list)
            {
                switch (item.boardingStatus)
                {
                    case OnboardingStatus.NotStarted:
                        bgColorList.Add(bgColor.blue);
                        break;

                    case OnboardingStatus.OnGoing:
                        bgColorList.Add(bgColor.orange);
                        break;

                    case OnboardingStatus.Completed:
                        bgColorList.Add(bgColor.green);
                        break;

                    case OnboardingStatus.Rejected:
                        bgColorList.Add(bgColor.red);
                        break;

                    case OnboardingStatus.Failed: //Added Failed - Sudipta
                        bgColorList.Add(bgColor.red);
                        break;

                    case OnboardingStatus.OverDue: //Added Failed - Sudipta
                        bgColorList.Add(bgColor.red);
                        break;

                    default:
                        bgColorList.Add(bgColor.blue);
                        break;
                }
            }
            return bgColorList;
        }

    }
}