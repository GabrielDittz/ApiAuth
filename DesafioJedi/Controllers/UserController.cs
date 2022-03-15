using DesafioJedi.Data;
using DesafioJedi.Models;
using DesafioJedi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace DesafioJedi.Controllers
{
    [ApiController]
    [Route("User/v1")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAsync([FromServices] AppDbContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetIdByAsync([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var users = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return users == null ? NotFound() : Ok(users);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostAsync([FromServices] AppDbContext context, [FromBody] User model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var users = new User
            {
                Username = model.Username,
                Password = model.Password,
                Role = model.Role
            };

            try
            {
                await context.Users.AddAsync(users);
                await context.SaveChangesAsync();
                return Created($"v1/users/{users.Id}", users);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutAsync([FromServices] AppDbContext context, [FromRoute] int id, User model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var users = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (users == null)
                return NotFound();

            try
            {
                users.Username = model.Username;
                users.Password = model.Password;
                users.Role = model.Role;

                context.Users.Update(users);
                await context.SaveChangesAsync();

                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAsync([FromServices] AppDbContext context, [FromRoute] int id)
        {
            var users = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (users == null)
                return NotFound();

            try
            {
                context.Users.Remove(users);
                await context.SaveChangesAsync();

                return Ok("Deletado com sucesso!");
            }catch(Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> AutheticateAsync([FromServices] AppDbContext context,[FromBody] User model)
        {
            var users = await context.Users.FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);

            if (users == null)
                return NotFound(new { message = "Usuário ou senha inválidos!"});

            var token = TokenService.GenerateToken(users);           

            users.Password = "";

            return new
            {
                user = users,
                token = token
            };
        }
    }
}
