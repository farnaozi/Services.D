using AutoMapper;
using Services.D.Core.Dtos;
using Services.D.Core.Interfaces;
using Services.D.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.D.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IServiceRepo _repo;
        private readonly IMapper _mapper;

        public HomeController(IServiceRepo repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost("GetMessage")]
        public async Task<IActionResult> GetMessage(string message)
        {
            return Ok(_repo.GetMessage(message));
        }
    }
}