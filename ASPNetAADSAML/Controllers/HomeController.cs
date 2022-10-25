using Saml;
using System;
using System.Web;
using System.Web.Mvc;

namespace ASPNetAADSAML.Controllers
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
            var samlEndpoint = "https://login.microsoftonline.com/{your Azure AD Tenant GUID}/saml2";

            var request = new AuthRequest(
                "https://localhost:44305/", //TODO: put your app's "unique ID" here
                "https://localhost:44305/Home/SamlConsume" //TODO: put Assertion Consumer URL (where the provider should redirect users after authenticating)
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
MIIC8DCCAdigAwIBAgIQbcuiErtYiaFNjozzPDt+uTANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD
EylNaWNyb3NvZnQgQXp1cmUgRmVkZXJhdGVkIFNTTyBDZXJ0aWZpY2F0ZTAeFw0yMDAzMTcxMjM1
NThaFw0yMzAzMTcxMjM1NThaMDQxMjAwBgNVBAMTKU1pY3Jvc29mdCBBenVyZSBGZWRlcmF0ZWQg
U1NPIENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsfGipLEh8tt2
tG/SnZnUxlGwdvVJZMW115DXlSS3iLCFtWo9dBeSXXhCUzAZzMpyboKF5J2wtEtjdtGykz+YWmfJ
T5n4wO11FRNWTHgt1Ueo/EsP2Da4NITpoghUKdPYhyLYXc9u+J+E+OJPUYkmgPvlAlzBJ4ObWmB4
El8Qh0F24OzCLINbXuTM/1YVi+xg0IKA1SaLybOlsdORd1D7cKUHHgoVqR6gSfIOixWbF3sOXfdc
6yx8CoqEIOxIRQbTe7HgAgu0JZIVyskbuXdWixBl0Vons+g1RODOOnBL3dUf5rupeCcwSiYmPXsP
Z7KTORScgRN5V/Rxqt3U/qvzVQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQAvYJG0D1OjD3S99bPE
V87aVcU2C2qKZjBPFR6989NxWUDtY5EZItBuCS2hdClT5dzdUA+ynd6oSRAAPv23pRZTDHWZ9qdn
BbBXkxk+pJYzI+3sXBH4I9nM5B65MJuATUVduwD3i0YdBNR++GlJiNo1cOfDjnoRzRT+5YO+PCQw
yGMCmaByxi7xSgDML3hBskczuOM6lALGy1Gu+MC+C4cqeSvBDiPm5lde1TvnI/ORlbJ71G3qoeIC
5JiikVZLiEQun7ICTCLa+8E3bFAH/xAJ6iVcd/AdGflt0EnT8eZh1GXVAKigFO6MV8DttL3qZJne
C8xYnMZxvBU0HDHdiFnJ
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