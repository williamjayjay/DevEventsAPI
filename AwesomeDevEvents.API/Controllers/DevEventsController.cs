using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {

        private readonly DevEventsDbContext _context;

        public DevEventsController(DevEventsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {

            var devEvent = _context.DevEvents
            .Include(de => de.Speakers).ToList();

            return Ok(devEvent);
        }

        /// <summary>
        /// Obter um evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Dados do evento</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            return Ok(devEvent);

        }

        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// {"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","title":"string","description":"string","startDate":"2023-07-19T18:11:55.092Z","endDate":"2023-07-19T18:11:55.092Z","speakers":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string","devEventId":"3fa85f64-5717-4562-b3fc-2c963f66afa6"}],"isDeleted":true}
        /// </remarks>
        /// <param name="devEvent">Dados do evento</param>
        /// <returns>Objeto recém-criado</returns>
        /// <response code="201" >Sucesso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);

        }

        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// <remarks>
        /// {"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","title":"string","description":"string","startDate":"2023-07-19T18:11:55.092Z","endDate":"2023-07-19T18:11:55.092Z","speakers":[{"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"string","talkTitle":"string","talkDescription":"string","linkedInProfile":"string","devEventId":"3fa85f64-5717-4562-b3fc-2c963f66afa6"}],"isDeleted":true}
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="devEventInput">Dados do evento</param>
        /// <returns>Nada.</returns>
        /// <response code="404">Não encontrado</response>
        /// <response code="204">Sucesso</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Update(Guid id, DevEvent devEventInput)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Update(devEventInput.Title, devEventInput.Description, devEventInput.StartDate, devEventInput.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();

        }

        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">Identificador de evento</param>
        /// <returns>Nada</returns>
        /// <response code="404">Não encontrado</response>
        /// <response code="204">Sucesso</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Cadastrar palestrante
        /// </summary>
        /// <remarks>
        /// { "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "talkTitle": "string", "talkDescription": "string", "linkedInProfile": "string", "devEventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" }
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="speaker">Dados do palestrante</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Evento não encontrado</response>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker speaker)
        {

            speaker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
            {
                return NotFound();
            }

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return NoContent();

        }

    }
}
