using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controller
{
    [Route("api/gamePlayers")]
    [ApiController]
    public class GamesPlayersController : ControllerBase
    {
        private IGamePlayerRepository _repository;
        public GamesPlayersController(IGamePlayerRepository respository)
        {
            _repository = respository;
        }
        // GET: api/<GamesPlayersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GamesPlayersController>/5
        [HttpGet("{id}",Name = "GetGameView")]
        public IActionResult GetGameView(long id)
        {
            try
            {
                var gp = _repository.GetGamePlayerView(id);
                var gameView = new GameViewDTO
                {
                    Id = gp.Id,
                    CreationDate = gp.Game.CreationDate,
                    Ships = gp.Ships.Select(ship=> new ShipDTO{

                        Id = ship.Id,
                        Type = ship.Type,
                        Locations=ship.Locations.Select(shipLocation => new ShipLocationDTO
                        {
                            Id=shipLocation.Id,
                            Location=shipLocation.Location
                        }).ToList()
                    }).ToList(),
                    GamePlayers=gp.Game.GamePlayers.Select(gps => new GamePlayerDTO{

                        Id=gps.Id,
                        JoinDate=gps.JoinDate,
                        Player =new PlayerDTO
                        {
                            Id=gps.Player.Id,
                            Email=gps.Player.Email
                        }
                }).ToList(),
                    Salvos = gp.Game.GamePlayers.SelectMany(gps => gps.Salvos.Select(salvo => new SalvoDTO
                    {
                        Id = salvo.Id,
                        Turn = salvo.Turn,
                        Player = new PlayerDTO
                        {
                            Id=gps.Player.Id,
                            Email = gps.Player.Email
                        },
                        Locations = salvo.Locations.Select(salvoLocation=> new SalvoLocationDTO
                        {
                            Id = salvoLocation.Id,
                            Location= salvoLocation.Location
                        }).ToList()
                    })).ToList()
                };
                return Ok(gameView);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<GamesPlayersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GamesPlayersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GamesPlayersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
