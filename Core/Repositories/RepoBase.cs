using Services.D.Core.Models;
using Services.D.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace Services.D.Core.Repositories
{
    public class RepoBase
    {
        #region *** private

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion
        #region *** ctor

        public RepoBase(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion
        #region *** protected properties

        protected string UserName => _httpContextAccessor.HttpContext.User.Claims.Where(p => p.Type == "UserName").FirstOrDefault().Value;
        protected int UserCode => int.Parse(_httpContextAccessor.HttpContext.User.Claims.Where(p => p.Type == "UserCode").FirstOrDefault().Value);
        protected int Language => _httpContextAccessor.HttpContext.Request.Headers["Language"] == "ka-GE" ? 0 : 1;
        protected string Token => _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
        protected string SessionId => _httpContextAccessor.HttpContext.Request.Headers["SessionId"];

        #endregion
        #region *** protected methods

        protected Task<OperationResult> ExecuteAction(Action<OperationResult> action)
        {
            return Task.Run(() =>
            {
                var retValue = new OperationResult();

                try
                {
                    action(retValue);

                    retValue.Success = true;
                }
                catch (Exception err)
                {
                    retValue.Error = err.Message;
                }

                return retValue;
            });
        }

        #endregion
    }
}
