﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace salvo.Models
{
    public class GamePlayer
    {
        public long Id { get; set; }

        public DateTime? JoinDate { get; set; }

        public long PlayerId { get; set; }
        public Player Player { get; set; }

        public long GameId { get; set; }
        public Game Game { get; set; }

        public ICollection<Ship> Ships { get; set; }
        public ICollection<Salvo> Salvos { get; set; }

        public Score GetScore()
        {
            return Player.GetScore(Game);
        }
        public GamePlayer GetOponent()
        {
            return Game.GamePlayers.FirstOrDefault(gp => gp.Id != Id);
        }
        public ICollection<SalvoHitDTO> GetHits()
        {
            return Salvos.Select(salvo => new SalvoHitDTO
            {
                Turn = salvo.Turn,
                Hits = GetOponent()?.Ships.Select(ship => new ShipHitDTO
                {
                    Type = ship.Type,
                    Hits = salvo.Locations.Where(
                               salvoLocation => ship.Locations.Any(
                                   shipLocation => shipLocation.Location == salvoLocation.Location)
                           ).Select(salvoLocation => salvoLocation.Location).ToList()
                }).ToList()
            }).ToList();
        }
        public ICollection<string> GetSunks()
        {
            int lastTurn = Salvos.Count;

            List<string> salvoLocations = GetOponent()?.Salvos.Where(salvo => salvo.Turn <= lastTurn)
                                                              .SelectMany(salvo => salvo.Locations.Select(location => location.Location))
                                                              .ToList();

            return Ships?.Where(ship => ship.Locations.Select(shipLocation => shipLocation.Location)
                                                      .All(salvoLocation => salvoLocations != null ? salvoLocations.Any(shipLocation => shipLocation == salvoLocation) : false))
                                                      .Select(ship => ship.Type).ToList();
        }
        public GameState GetGameState()
        {
            GameState gameState = GameState.WAIT;

            GamePlayer Opponent = GetOponent();

            if (Opponent == null)
                return GameState.WAIT;
            if (Ships.Count == 0)
                return GameState.PLACE_SHIPS;
            if (Opponent.Ships.Count == 0)
                return GameState.WAIT;
            if (Salvos.Count > Opponent.Salvos.Count)
                return GameState.WAIT;
            if (Salvos.Count < Opponent.Salvos.Count)
                return GameState.ENTER_SALVO;
            if(Salvos.Count == Opponent.Salvos.Count)
            {
                var Sunks = GetSunks();
                var OpponentSunks = Opponent?.GetSunks();
                if (Sunks.Count == Ships.Count && OpponentSunks.Count == Opponent.Ships.Count)
                    return GameState.TIE;
                else if (Sunks.Count == Ships.Count)
                    return GameState.LOSS;
                else if (OpponentSunks.Count == Opponent.Ships.Count)
                    return GameState.WIN;
                else if(JoinDate < Opponent.JoinDate)
                    return GameState.ENTER_SALVO;
                else
                    return GameState.WAIT;
            }

            return gameState;
        }
    }
}
