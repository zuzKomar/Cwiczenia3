using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cw3.Handlers
{
    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder, 
            ISystemClock clock
         //   IStudentDbServive service
                ) : base(options, logger, encoder, clock)//przekazanie do konstruktora z nadklasy
        {
            
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() 
        {
            if (!Request.Headers.ContainsKey("Authorization")) //jesli nasz request nie zawiera nagłówka Authorization
                return AuthenticateResult.Fail("Missing authorization header!"); //zwraca info o niepowodzeniu autoryzacji

            //Authorization : Basic akdsnqkdjqow--
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]); //wyciagamy nagłówek do uwierzytelnienia
            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter);//wyciagamy login i haslo z akdsnqkdjqow-- i wrzucamy do tablicy byte[] (mamy surowy ciąg bytów)
            var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(":"); //zamieniamy ciąg bytów na tablice 2 elementowa typu String <login i hasło> kodowanie

            if (credentials.Length != 2)
            {
                return AuthenticateResult.Fail("Incorrect authorization header value!");
            }
            

            var claims = new[] //informacje o użytkowniku 
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Zuzanna"), //login uzytkownika
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student")
            };
            
            var identity = new ClaimsIdentity(claims, Scheme.Name); //"dowód osobisty" -identyfikuje uzytkownika
            var principal = new ClaimsPrincipal(identity); //moze zawierac wiele elementow jak dowód osobisty
            var ticket = new AuthenticationTicket(principal, Scheme.Name); // udzielenie dostępu temu użytkownikowi, tworzac mu jego ticket
            
            return AuthenticateResult.Success(ticket); //zwraca dostęp uztkownikowi z pomocą ticket u
        }
    }
}