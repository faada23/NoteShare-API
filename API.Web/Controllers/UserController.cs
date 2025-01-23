using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ApiController]
[Route("{controller}")]
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

    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        var response =  await _userService.GetAllUsers();
        return Ok(response);
    }

    public async Task<ActionResult<Task<User>>> GetByFilter(Guid id)
    {
        var response = await _userService.GetUserById(id);
        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create()
    {
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
