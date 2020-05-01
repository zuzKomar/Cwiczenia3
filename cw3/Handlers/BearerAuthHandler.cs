using System.Threading.Tasks;
using cw3.DTOs.Requests;
using cw3.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw3.Handlers
{
    public class BearerAuthHandler
    {
        private readonly IStudentDbService _dbService;

        public BearerAuthHandler(IStudentDbService service)
        {
            _dbService = service;
        }

        public async Task<IActionResult> HandleAuthenticateAsync(LoginRequest request)
        {
            if(!_dbService.CheckUserCredentials(request.IndexNumber, request.Password))
                return new BadRequestResult();
            
            return new AcceptedResult();
        }
    }
}