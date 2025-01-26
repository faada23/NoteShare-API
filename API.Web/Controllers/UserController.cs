using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using API.Core.Models;
using API.Application.DTOs;
using API.Application.Mapper;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{   
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IUnitOfWork _unitOfWork;

    public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork,IUserService userService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userService = userService;
    }

}
