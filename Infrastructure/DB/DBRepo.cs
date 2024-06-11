using Services.D.Core.Interfaces;
using Services.D.Core.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.D.Infrastructure.DB
{
    public class DBRepo : DBRepoBase, IDBRepo
    {
        #region *** private


        #endregion
        #region *** ctor

        public DBRepo(IOptions<AppSettings> settings, ILoggerRepo logManager) : base(settings, logManager) { }

        #endregion
        #region *** public 

        #endregion
    }
}
