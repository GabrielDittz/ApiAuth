using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DesafioJedi.Controllers
{
    [ApiController]
    [Route("Home/v1")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("anonymous")]
        [AllowAnonymous]
        public string Anonymous() => "Anônimo";

        [HttpGet]
        [Route("authenticated")]
        [Authorize]
        public string Authenticated() => $"Autenticado - {User.Identity.Name}";

        [HttpGet]
        [Route("padawan")]
        [Authorize(Roles = "Padawan, Jedi")]
        public string Padawan() => "Padawan";

        [HttpGet]
        [Route("jedi")]
        [Authorize(Roles = "Jedi")]
        public string Jedi() => "Jedi";
    }
}
