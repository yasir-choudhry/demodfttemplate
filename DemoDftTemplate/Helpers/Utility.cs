using DemoDftTemplate.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace DemoDftTemplate.Helpers
{
    public static class Utility
    {
        public static string GetCurrentUserEmail()
        {
            IHeaderDictionary aDict = WebHelper.HttpContext.Request.Headers;

            string Result = "test.bdd@dft.gov.uk";

            //Check whether the header that we need is included in the dictionary
            if (aDict.Any(x => x.Key == "X-Goog-Authenticated-User-Email"))
            {
                //if it is then take the value, tostring it and assign to the result
                Result = aDict.First(x => x.Key == "X-Goog-Authenticated-User-Email")
                    .Value.ToString()
                    .Replace("accounts.google.com:", string.Empty);
            }

            return Result;
        }

    }
}
