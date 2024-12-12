using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class MyControllerBase : ControllerBase, IAsyncDisposable
    {
        protected UnitOfWork _unitOfWork;

        protected readonly IConfiguration _configuration;

        protected ClaimsIdentity? _identity;

        public MyControllerBase(IConfiguration configuration, IMapper mapper, IMemoryCache cache)
        {
            _configuration = configuration;
            _unitOfWork = new UnitOfWork(_configuration.GetConnectionString("DefaultConnection")!, mapper, cache);
        }

        protected bool AuthenticateUser()
        {
            _identity = HttpContext.User.Identity as ClaimsIdentity;

            if (_identity != null && _identity.IsAuthenticated == true)
            {
                int _accountId;
                Guid _accountGuid;
                int.TryParse(_identity.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value, out _accountId);
                Guid.TryParse(_identity.Claims?.FirstOrDefault(x => x.Type == "Guid")?.Value, out _accountGuid);
                _unitOfWork.AccountId = _accountId;
                _unitOfWork.AccountGuid = _accountGuid;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected List<string> GetRequiredColumns<T>()
        {
            var columns = typeof(T).GetMembers()
                .Where(x => x.IsDefined(typeof(RequiredAttribute), false))
                .Select(s => s.Name)
                .ToList();

            return columns;
        }


        public async ValueTask DisposeAsync()
        {
            if (_unitOfWork != null)
                await _unitOfWork.DisposeAsync();
        }
    }
}
