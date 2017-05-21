using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Linq;
using DashDocs.Services;
using System.Security.Claims;
using DashDocs.Models;
using Newtonsoft.Json;
using DashDocs.Helpers;
using DashDocs.Helpers.Exceptions;

[assembly: OwinStartup(typeof(DashDocs.Startup))]

namespace DashDocs
{
    public class Startup
    {
        private string clientAppId = ConfigurationManager.AppSettings["ClientAppId"].ToString();
        private string authority = ConfigurationManager.AppSettings["Authority"].ToString();
        private string replyUrl = ConfigurationManager.AppSettings["ReplyUrl"].ToString();

        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions() { CookieSecure = CookieSecureOption.Always });
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            var options = new OpenIdConnectAuthenticationOptions
            {
                ClientId = clientAppId,
                Authority = authority,
                TokenValidationParameters = new TokenValidationParameters {  ValidateIssuer = false },
                RedirectUri = replyUrl
            };

            var notifications = new OpenIdConnectAuthenticationNotifications
            {
                AuthorizationCodeReceived = AuthCodeReceived,
                AuthenticationFailed = AuthFailed,
            };

            options.Notifications = notifications;

            app.UseOpenIdConnectAuthentication(options);
        }

        public Task AuthCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            var oid = Guid.Parse(notification.JwtSecurityToken.Claims.Single(c => c.Type == "oid").Value);
            var tid = Guid.Parse(notification.JwtSecurityToken.Claims.Single(c => c.Type == "tid").Value);
            var firstname = notification.JwtSecurityToken.Claims.Single(c => c.Type == "name").Value;
            var lastname = notification.JwtSecurityToken.Claims.Single(c => c.Type == "family_name").Value;

            var context = new DashDocsContext();

            var customer = context.Customers.SingleOrDefault(c => c.Id == tid);
            if (customer != null)
            {
                var user = context.Users.SingleOrDefault(u => u.Id == oid && u.CustomerId == tid);
                if (user == null)
                {
                    // new user first sign-in
                    user = new User
                    {
                        Id = oid,
                        CustomerId = tid,
                        FirstName = firstname,
                        LastName = lastname
                    };

                    context.Users.Add(user);
                    context.SaveChanges();
                }

                // though the application can access the claims from the returned
                // JWTToken, it's better to have custom claim properties as this eases up the usage.
                var applicationClaims = new AppClaims
                {
                    CustomerId = tid,
                    CustomerName = customer.Name,
                    UserId = oid,
                    DisplayName = user.FirstName + user.LastName
                };

                var claim = new Claim("ddcs", JsonConvert.SerializeObject(applicationClaims));
                notification.AuthenticationTicket.Identity.AddClaim(claim);
            }
            else
            {
                throw new UserLoggedInWithoutExistingCustomerException()
                {
                    TenantId = tid,
                    UserId = oid,
                    FirstName = firstname,
                    LastName = lastname
                };
            }
            return Task.FromResult(0);
        }

        private Task AuthFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> arg)
        {
            var ex = arg.Exception as UserLoggedInWithoutExistingCustomerException;
            if (ex != null)
            {
                arg.OwinContext.Response.Redirect(
                    $"/customer/enroll?tid={ex.TenantId}&uid={ex.UserId}&fn={ex.FirstName}&ln={ex.LastName}");
            }

            arg.HandleResponse();
            return Task.FromResult(0);
        }
    }
}
