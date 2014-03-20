using System;
using System.Web.Mvc;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;


namespace Google.Apis.Interview_log
{
    public class AppFlowMetadata : FlowMetadata
    {
        private static readonly string CLIENT_ID = "904247126982.apps.googleusercontent.com";
        private static readonly string CLIENT_SECRET = "6_uJd_KuIZ7o06YS42gc2jId";
        private static readonly string REDIRECT_URI = "https://localhost:49874";
        private static readonly IAuthorizationCodeFlow flow =
            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = CLIENT_ID,
                    ClientSecret = CLIENT_SECRET,
               
                },
                Scopes = new[] { CalendarService.Scope.Calendar} ,
                DataStore = new FileDataStore("Calendar.Api.Auth.Store")
             });

        public override string GetUserId(Controller controller)
        {
             var user = controller.Session["user"];
            if (user == null)
            {
                user = Guid.NewGuid();
                controller.Session["user"] = user;
            }
            return user.ToString();

        }

        public override IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }
    }
}