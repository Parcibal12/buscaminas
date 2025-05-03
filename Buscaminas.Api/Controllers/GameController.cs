using Microsoft.AspNetCore.Mvc;
using Minesweeper.Api.Services;
using Minesweeper.Api.Models; // Asegúrate que este namespace exista y contenga Board
using System; // Necesario para Guid

namespace Minesweeper.Api.Controllers
{
    [ApiController]
    [Route("api/game")] // Ruta base para este controlador
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService; // Renombrado para claridad

        // Inyecta el servicio del juego
        public GameController(IGameService gameService)
        {
             _gameService = gameService;
        }

        // Endpoint para crear un nuevo juego
        // POST /api/game/new
        [HttpPost("new")]
        // Recibe los parámetros del cuerpo de la solicitud
        public ActionResult<Board> NewGame([FromBody] NewGameRequest req)
        {
            // Llama al servicio para crear el tablero y devuelve el resultado
            // Asegúrate que IGameService.CreateBoard devuelva un objeto compatible con 'Board'
            var board = _gameService.CreateBoard(req.Width, req.Height, req.Mines);
            return Ok(board); // Devuelve 200 OK con el tablero creado
        }


        // Endpoint para revelar una celda
        // POST /api/game/{id}/reveal
        [HttpPost("{id}/reveal")] // Acepta el ID del juego desde la ruta
        // Recibe el ID de la ruta y las coordenadas del cuerpo
        public ActionResult<Board> RevealCell(Guid id, [FromBody] CellRequest req)
        {
             // Llama al servicio para revelar la celda y devuelve el estado actualizado del tablero
             var board = _gameService.Reveal(id, req.Row, req.Col);
             if (board == null)
             {
                 return NotFound($"Game with ID {id} not found."); // Devuelve 404 si el juego no existe
             }
             return Ok(board); // Devuelve 200 OK con el tablero actualizado
        }

        // Endpoint para poner/quitar una bandera en una celda
        // POST /api/game/{id}/toggle-flag
        [HttpPost("{id}/toggle-flag")] // Acepta el ID del juego desde la ruta y usa 'toggle-flag'
        // Recibe el ID de la ruta y las coordenadas del cuerpo
        public ActionResult<Board> ToggleFlag(Guid id, [FromBody] CellRequest req)
        {
             // Llama al servicio para alternar la bandera y devuelve el estado actualizado
             var board = _gameService.ToggleFlag(id, req.Row, req.Col);
              if (board == null)
             {
                 return NotFound($"Game with ID {id} not found."); // Devuelve 404 si el juego no existe
             }
             return Ok(board); // Devuelve 200 OK con el tablero actualizado
        }
    }

    // Modelos para las solicitudes (records son una forma concisa en C# moderno)
    // Asegúrate que las propiedades (Width, Height, Mines, Row, Col) coincidan
    // exactamente (incluyendo mayúsculas/minúsculas) con lo que envía el frontend.
    public record NewGameRequest(int Width, int Height, int Mines);
    public record CellRequest(int Row, int Col);
}