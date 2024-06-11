using System.Collections.Generic;
using System.Threading.Tasks;
using Services.D.Core.Models;
using Services.D.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using Services.D.Core.Events;

namespace Services.D.Core.Repositories
{
    public class ServiceRepo : RepoBase, IServiceRepo
    {
        #region *** private

        private readonly IDBRepo _dbManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly IEventBus _bus;

        #endregion
        #region *** ctor

        public ServiceRepo(IDBRepo dbManager,
            IJwtFactory jwtFactory,
            IEventBus bus,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _jwtFactory = jwtFactory;
            _dbManager = dbManager;
            _bus = bus;
        }

        #endregion
        #region *** public

        public Task GetMessage(string message)
        {
            Console.WriteLine($"Message Received - {message}");
            
            return Task.CompletedTask;
        }

        #endregion
    }
}