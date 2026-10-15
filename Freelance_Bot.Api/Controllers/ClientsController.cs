
using Freelance_bot.Application.Feature.Client.Request;
using Freelance_bot.Application.IServieces;
using Microsoft.AspNetCore.Mvc;

namespace Freelance_Bot.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController(IClientService clientService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await clientService.GetAllAsync(CurrentUserId));

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var client = await clientService.GetByIdAsync(id, CurrentUserId);
            return client == null ? NotFound() : Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
        {
            var client = await clientService.CreateAsync(CurrentUserId, request);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateClientRequest request)
        {
            try
            {
                return Ok(await clientService.UpdateAsync(
                    id,
                    CurrentUserId,
                    request));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await clientService.DeleteAsync(id, CurrentUserId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}