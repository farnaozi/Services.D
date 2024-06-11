using Services.D.Core.Events;
using Services.D.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.D.Core.Interfaces
{
    public interface IServiceRepo
    {
        Task GetMessage(string message);
    }
}
