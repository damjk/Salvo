using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using salvo.Models;
using salvo.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace salvo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("PlayerOnly")]
    public class GamesController : ControllerBase
    {
        private IGameRepository _repository;
        private IPlayerRepository _playerRepository;
        private IGamePlayerRepository _gamePlayerRepository;

        public GamesController(IGameRepository repository,
                               IPlayerRepository playerRepository,
                               IGamePlayerRepository gamePlayerRepository)
        {
            _repository = repository;
            _playerRepository = playerRepository;
            _gamePlayerRepository = gamePlayerRepository;
        }

        // GET: api/<GamesController>
        [HttpGet(Name = "GetAllGames")]
        [AllowAnonymous]
        public IActionResult GetAllGames()
        {
            try
            {
                //Antes
                //var games = _repository.GetAllGames();
                //Ahora
                var gamesDto = _repository.GetAllGamesWithPlayers().Select(
                    game => new GameDTO
                    {
                        Id = game.Id,
                        CreationDate = game.CreationDate,
                        GamePlayers = game.GamePlayers.Select(
                            gamePlayer => new GamePlayerDTO
                            {
                                Id = gamePlayer.Id,
                                JoinDate = gamePlayer.JoinDate,
                                Player = new PlayerDTO
                                {
                                    Id = gamePlayer.Player.Id,
                                    Name = gamePlayer.Player.Name,
                                    Email = gamePlayer.Player.Email
                                },
                                Point = gamePlayer.GetScore() != null ? gamePlayer.GetScore().Point : null
                            }
                        ).ToList()
                    }
                ).ToList();

                GameListDTO gameList = new GameListDTO
                {
                    Email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest",
                    Games = gamesDto
                };

                //_logger.LogInfo($"Returned all owners from database.");
                return Ok(gameList);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong inside GetAllGames action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                DateTime fecha = DateTime.Now;

                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                Player Player = _playerRepository.FindByEmail(email);

                GamePlayer gamePlayer = new GamePlayer
                {
                    PlayerId = Player.Id,
                    Game = new Game
                    {
                        CreationDate = fecha
                    },
                    JoinDate = fecha
                };

                _gamePlayerRepository.Save(gamePlayer);

                return StatusCode(201, gamePlayer.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/players", Name = "Join")]
        public IActionResult Join(long id)
        {
            try
            {
                DateTime fecha = DateTime.Now;
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                Player Player = _playerRepository.FindByEmail(email);
                Game Game = _repository.FindById(id);

                #region Validaciones
                if (Game == null)
                    return StatusCode(403, "Game doesnt exist");
                if (Game.GamePlayers.Where(x => x.Player.Id == Player.Id).FirstOrDefault() != null)
                    return StatusCode(403, "The Player is already on the game");
                if (Game.GamePlayers.Count == 2)
                    return StatusCode(403, "The Game is already full");
                #endregion

                GamePlayer gamePlayer = new GamePlayer
                {
                    GameId = id,
                    PlayerId = Player.Id,
                    JoinDate = fecha
                };

                _gamePlayerRepository.Save(gamePlayer);

                return StatusCode(201, gamePlayer.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "");
            }
        }

        [HttpGet("topTypes")]
        [AllowAnonymous]
        public IActionResult GetTopTypeDestroyed()
        {
            try
            {
                List<String> Sunks = new List<String>();
                var games = _repository.GetAllGamesWithPlayersAndSalvos();
                foreach (var game in games)
                {
                    foreach (var gp in game.GamePlayers)
                    {
                        Sunks.AddRange(gp.GetSunks());
                    }
                }
                var types = Sunks
                     .GroupBy(i => i)
                     .OrderByDescending(g => g.Count());

                IEnumerable sunksTop5 = types.Take(5).Select(type => new
                {
                    type = type.First(),
                    quantity = type.Count()
                });


                return Ok(sunksTop5);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }
        }


        [HttpGet("topLocations")]
        [AllowAnonymous]
        public IActionResult GetTopLocationsDestroyed()
        {
            try
            {
                IEnumerable<String> salvoLocations = _repository.GetAllSalvoLocations()
                .SelectMany(game => game.GamePlayers)
                    .SelectMany(gp => gp.Salvos)
                        .SelectMany(salvo => salvo.Locations)
                            .Select(location => location.Location);

                var locations = salvoLocations
                     .GroupBy(i => i)
                     .OrderByDescending(g => g.Count());

                IEnumerable mayores = locations.Take(5).Select(location => new
                {
                    position = location.First(),
                    quantity = location.Count()
                });
                return Ok(mayores);
            }
            catch (Exception ex)
            {
                return StatusCode(403, ex.Message);
            }

        }
    }
}
