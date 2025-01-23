using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using API.Core.Models;
using API.Application.DTOs;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{   
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    public IUnitOfWork UnitOfWork;

    public UserController(ILogger<UserController> logger, IUnitOfWork unitOfWork,IUserService userService)
    {
        _logger = logger;
        UnitOfWork = unitOfWork;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserGet>>> GetAll()
    {
        var users = await UnitOfWork.UserRepository.GetAll();
        var response = users.Select(p => new UserGet(p.Id,p.Username,p.IsBanned,p.CreatedAt,p.Notes));
        return Ok(response);
    }

    [HttpGet("id")]
    public async Task<ActionResult<UserGet>> GetByFilter(Guid id)
    {
        var user = await UnitOfWork.UserRepository.GetByFilter(p => p.Id == id);
        var response = new UserGet(user.Id,user.Username,user.IsBanned,user.CreatedAt,user.Notes);
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] UserCreate userCreate)
    {   
        //доделать
        return Ok();
    }

    [HttpPut]
    public IActionResult Update()
    {   
        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete()
    {
        return Ok();
    }

}
