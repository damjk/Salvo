using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using salvo.Models;
using salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace salvo.Controllers
{
    public enum Resultado
    {
        NOT_FINISHED,
        PLAYER_WON,
        PLAYER_LOSS,
        GAME_TIE
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize("PlayerOnly")]
    public class GamePlayersController : ControllerBase
    {
        private IGamePlayerRepository _repository;
        private IScoreRepository _scoreRepository;

        public GamePlayersController(IGamePlayerRepository repository, IScoreRepository scoreRepository)
        {
            _repository = repository;
            _scoreRepository = scoreRepository;
        }

        // GET: api/<GamePlayersController>
        [HttpGet("{id}", Name = "GetGameView")]
        public IActionResult GetGameView(long id)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                var gamePlayers = _repository.GetGamePlayerView(id);
                var Opponent = gamePlayers.GetOponent();

                if (gamePlayers.Player.Email != email)
                    return Forbid();

                var gameView = new GameViewDTO
                {
                    Id = gamePlayers.Id,
                    CreactionDate = gamePlayers.Game.CreationDate,
                    Ships = gamePlayers.Ships.Select(ship => new ShipDTO
                    {
                        Id = ship.Id,
                        Type = ship.Type,
                        Locations = ship.Locations.Select(location => new ShipLocationDTO
                        {
                            Id = location.Id,
                            Location = location.Location
                        }).ToList()
                    }).ToList(),
                    GamePlayers = gamePlayers.Game.GamePlayers.Select(gamePlayer => new GamePlayerDTO
                    {
                        Id = gamePlayer.Id,
                        JoinDate = gamePlayer.JoinDate,
                        Player = new PlayerDTO
                        {
                            Id = gamePlayer.Player.Id,
                            Name = gamePlayer.Player.Name,
                            Email = gamePlayer.Player.Email
                        }
                    }).ToList(),
                    Salvos = gamePlayers.Game.GamePlayers.SelectMany(gps => gps.Salvos.Select(salvo => new SalvoDTO
                    {
                        Id = salvo.Id,
                        Turn = salvo.Turn,
                        Player = new PlayerDTO
                        {
                            Id = salvo.GamePlayer.Player.Id,
                            Email = salvo.GamePlayer.Player.Email,
                            Name = salvo.GamePlayer.Player.Name
                        },
                        Locations = salvo.Locations.Select(location => new SalvoLocationDTO
                        {
                            Id = location.Id,
                            Location = location.Location
                        }).ToList()
                    })).ToList(),
                    Hits = gamePlayers.GetHits(),
                    HitsOpponent = Opponent?.GetHits(),
                    Sunks = gamePlayers.GetSunks(),
                    SunksOpponent = Opponent?.GetSunks(),
                    GameState = gamePlayers.GetGameState().ToString()
                };
               
                //_logger.LogInfo($"Returned all owners from database.");
                return Ok(gameView);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Something went wrong inside GetAllGames action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/ships", Name = "ships")]
        public IActionResult Post(long id, [FromBody] ICollection<ShipDTO> ships)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                var gamePlayer = _repository.FindById(id);

                if (gamePlayer == null)
                    return StatusCode(403, "Game doesnt Exist");
                if(gamePlayer.Player.Email != email)
                    return StatusCode(403, "The User is not part of this game");
                if(gamePlayer.Ships.Count == 5)
                    return StatusCode(403, "The Ships are already positioned");
                if((gamePlayer.Ships.Count + ships.Count) > 5)
                    return StatusCode(403, "The Player cannot have more than 5 ships in the same game");

                gamePlayer.Ships = ships.Select(ship => new Ship
                {
                    GamePlayerId = id,
                    Locations = ship.Locations.Select(location => new ShipLocation
                    {
                        Id = ship.Id,
                        Location = location.Location
                    }).ToList(),
                    Type = ship.Type
                }).ToList();

                //foreach (ShipDTO shipDTO in ships)
                //{
                //    gamePlayer.Ships.Add(new Ship
                //    {
                //        GamePlayerId = id,
                //        Locations = shipDTO.Locations.Select(location => new ShipLocation
                //        {
                //            Location = location.Location
                //        }).ToList(),
                //        Type = shipDTO.Type
                //    });
                //}

                _repository.Save(gamePlayer);

                return StatusCode(201,"Created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error");
            }
        }

        [HttpPost("{id}/salvos", Name = "salvos")]
        public IActionResult Post(long id, [FromBody] SalvoDTO salvos)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                GamePlayer gamePlayer = _repository.FindById(id);
                GamePlayer opponent = gamePlayer.GetOponent();

                Resultado Resultado = Resultado.NOT_FINISHED;
                bool GameEnded = false;

                //opponent = _repository.FindById(opponent.Id);

                if (gamePlayer == null)
                    return StatusCode(403, "Game doesnt Exist");
                if (gamePlayer.Player.Email != email)
                    return StatusCode(403, "The User is not part of this game");
                if (gamePlayer.Ships.Count != 5)
                    return StatusCode(403, "The Ships arent positioned");
                if (opponent == null)
                    return StatusCode(403, "The game doesnt have an opponent");
                if (opponent.Ships.Count != 5)
                    return StatusCode(403, "The Opponent Ships arent positioned");
                if (gamePlayer.Salvos.Count > opponent.Salvos.Count)
                    return StatusCode(403, "This is not your turn");
                if((gamePlayer.Salvos.Count == opponent.Salvos.Count) && gamePlayer.JoinDate > opponent.JoinDate)
                    return StatusCode(403, "This is not your turn");
                if (salvos.Locations.Count != 5)
                    return StatusCode(403, "Only 5 salvos per turn, no more, no less");

                GameState GameState = gamePlayer.GetGameState();

                switch (GameState)
                {
                    case GameState.WIN:
                    case GameState.LOSS:
                    case GameState.TIE:
                        return StatusCode(403, "The Game Already Ended");
                }

                gamePlayer.Salvos.Add(new Salvo {
                    GamePlayerId = id,
                    Turn = gamePlayer.Salvos.Count + 1,
                    Locations = salvos.Locations.Select(location => new SalvoLocation { 
                        Location = location.Location
                    }).ToList()
                });

                _repository.Save(gamePlayer);

                GameState = gamePlayer.GetGameState();

                switch (GameState)
                {
                    case GameState.WIN:
                        Resultado = Resultado.PLAYER_WON;
                        GameEnded = true;
                        break;
                    case GameState.LOSS:
                        Resultado = Resultado.PLAYER_LOSS;
                        GameEnded = true;
                        break;
                    case GameState.TIE:
                        Resultado = Resultado.GAME_TIE;
                        GameEnded = true;
                        break;
                }

                if (GameEnded)
                {
                    DateTime endDate = DateTime.Now;
                    Score score = new Score
                    {
                        FinishDate = endDate,
                        GameId = gamePlayer.GameId,
                        PlayerId = gamePlayer.PlayerId,
                        Point = (Resultado == Resultado.PLAYER_WON) ? 1 : (Resultado == Resultado.GAME_TIE) ? 0.5 : 0
                    };

                    _scoreRepository.Save(score);

                    Score opponentScore = new Score
                    {
                        FinishDate = endDate,
                        GameId = opponent.GameId,
                        PlayerId = opponent.PlayerId,
                        Point = (Resultado == Resultado.PLAYER_WON) ? 0 : (Resultado == Resultado.GAME_TIE) ? 0.5 : 1
                    };

                    _scoreRepository.Save(opponentScore);
                }

                return StatusCode(201, "Created");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error");
            }
        }
    }
}
