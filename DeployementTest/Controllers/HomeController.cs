using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DeployementTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string accessToken = this.Request.Headers["X-MS-TOKEN-FACEBOOK-ACCESS-TOKEN"];
            if (accessToken == "")
            {
                accessToken = "NULL";
            }
            ViewBag.AccessToken = accessToken;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        private string RequestResponse(string pUrl)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            Stream responseStream = null;
            StreamReader responseReader = null;
            string responseData = "";
            try
            {
                WebResponse webResponse = webRequest.GetResponse();
                responseStream = webResponse.GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception exc)
            {
                Response.Write("<br /><br />ERROR : " + exc.Message);
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseReader.Close();
                }
            }
            return responseData;
        }

        private string CallFacebookGraphApi(string id, string fields, string accessToken)
        {
            string fbGraphiApiRequest = "https://graph.facebook.com/" + id + "?" + fields + "&access_token=" + accessToken;
            string response = RequestResponse(fbGraphiApiRequest);
            if (response == "")
            {
                response = "<br /><br />Error while accessing the FB Graph API";
            }
            string resJson = response.Replace(@"\", string.Empty);
            var obj = JsonConvert.DeserializeObject(resJson);
            string formattedJson = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return formattedJson;
        }
        public ActionResult Facebook()
        {
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = this.Request.Headers["X-MS-TOKEN-FACEBOOK-ACCESS-TOKEN"];
                // Here we are retrieving the users details
                string myFields = "fields=first_name,last_name,middle_name,birthday,currency,email";
                string myJsonResponse = CallFacebookGraphApi("me", myFields, accessToken);
                ViewBag.Me = myJsonResponse;
            }
            return View();
        }
    }
}