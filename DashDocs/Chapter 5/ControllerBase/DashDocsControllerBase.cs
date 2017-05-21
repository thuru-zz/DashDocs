using DashDocs.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DashDocs.ControllerBase
{
    public abstract class DashDocsControllerBase : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            
            if(User != null && User.Identity.IsAuthenticated)
            {
                var principal = (ClaimsPrincipal)User;
                var claim = principal.Claims.SingleOrDefault(c => c.Type == "ddcs")?.Value;
                if (claim != null)
                {
                    DashDocsClaims = JsonConvert.DeserializeObject<AppClaims>(claim);
                }
                else
                {
                    throw new ApplicationException("Claims Null or Unidentified : Authentication Exception");
                }
            }
        }

        internal AppClaims DashDocsClaims { get; private set; }
    }
}