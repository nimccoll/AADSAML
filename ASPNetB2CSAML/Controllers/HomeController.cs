using Saml;
using System;
using System.Web;
using System.Web.Mvc;

namespace ASPNetB2CSAML.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string userName = this.User.Identity.Name;
            return View();
        }

        public ActionResult Login()
        {
            //TODO: specify the SAML provider url here, aka "Endpoint"
            var samlEndpoint = "https://{your B2C tenant name}.b2clogin.com/{your B2C tenant name}.onmicrosoft.com/B2C_1A_signup_signin_saml/samlp/sso/login";

            var request = new AuthRequest(
                "https://localhost:44308", //TODO: put your app's "unique ID" here
                "https://localhost:44308/Home/SamlConsume" //TODO: put Assertion Consumer URL (where the provider should redirect users after authenticating)
                );

            //generate the provider URL
            string url = request.GetRedirectUrl(samlEndpoint);

            //then redirect your user to the above "url" var
            //for example, like this:
            //Response.Redirect(url);
            return Redirect(url);
        }

        public ActionResult SamlConsume()
        {
            // 1. TODO: specify the certificate that your SAML provider gave you
            string samlCertificate = @"-----BEGIN CERTIFICATE-----
MIIDTjCCAjagAwIBAgIQG+Yl2F/2grpJxd/q4qW9iDANBgkqhkiG9w0BAQsFADA6
MTgwNgYDVQQDDC9hc3BuZXRiMmNzYW1sLm5pbWNjb2xsb3JndGVuYW50Lm9ubWlj
cm9zb2Z0LmNvbTAeFw0yMDA0MzAxMzI0MjJaFw0zNTA0MzAxMzM0MjFaMDoxODA2
BgNVBAMML2FzcG5ldGIyY3NhbWwubmltY2NvbGxvcmd0ZW5hbnQub25taWNyb3Nv
ZnQuY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0Mr0D67ydXvM
LPJdEz5wAE5cGOU5pjiIlMp6mTZGHiH1LiW/FT14qvPoOE8+/46NxrRVmAUxaLQX
H6amiSIj5GqCiP43BvRNCB6WadNFTUN8J1J2kHguwJKKKh6rtON0wAK0+IhfG2t6
KkbogCDdGQSuYuFmbmPZFKcadk+a+/3pnjxY+TEsWus0lNthR6WzieHrcEQKbNVy
/hJNLPkUS/bDvmllAeBknyZU0mvYp5tdqAnbyeY8XZsdxY/1OHfBnFVAVFyL2keC
dmaXaJ7k6604YXlSspvouJ5QBICWw4zKPQBWFLtBlYNsoBMFQK/BxJ+iDI5gdWxH
gDy+q9h3bQIDAQABo1AwTjAOBgNVHQ8BAf8EBAMCB4AwHQYDVR0lBBYwFAYIKwYB
BQUHAwIGCCsGAQUFBwMBMB0GA1UdDgQWBBS8lTMYLCu3oWA9t+w9qQS0YxIDXzAN
BgkqhkiG9w0BAQsFAAOCAQEAjvMYUGVdnai409YqWgsntNSO5s/RDD1sul/u9ENW
IQeSClcD5GljsHrcQtKUUDc0tPqtk4YLf47bi7QZPKygbsBky4ohNI4yazV7HUTh
ihi7GMaZElXWLWVYJV9Zv6S4C9RMGE05hKJwJhEwhfufYM/Z2gyKiwindt/iDGyK
mwT4moNR/6vvv2IV3wq6vNuU5qfWMLFbJ6djwxwAEWEolCmGNyx09MxU+/p4QAez
FnhdNSz9EztmT8raEbFpAAtxjZjT/cueZnKmlvUbZOJHCHocKjnMyYvbxtjmAI/G
8N9Rtfbw6TxrQLM8axshQVNDz5e15rGxQP3P47tVL4ABjw=!
-----END CERTIFICATE-----";

            // 2. Let's read the data - SAML providers usually POST it into the "SAMLResponse" var
            Saml.Response samlResponse = new Saml.Response(samlCertificate, Request.Form["SAMLResponse"]);

            // 3. We're done!
            if (samlResponse.IsValid())
            {
                //WOOHOO!!! user is logged in

                //Some more optional stuff for you
                //let's extract username/firstname etc
                string username, email, firstname, lastname;
                try
                {
                    username = samlResponse.GetNameID();
                    email = samlResponse.GetEmail();
                    firstname = samlResponse.GetFirstName();
                    lastname = samlResponse.GetLastName();
                }
                catch (Exception ex)
                {
                    //insert error handling code
                    //no, really, please do
                    return null;
                }

                // TODO: Cookie containing user information should be encrypted
                HttpCookie authCookie = new HttpCookie("AzureADAuth");
                authCookie.Values["UserName"] = samlResponse.GetNameID();
                this.Response.Cookies.Add(authCookie);
                return Redirect("/");
            }
            else
            {
                return new HttpUnauthorizedResult();
            }
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
    }
}