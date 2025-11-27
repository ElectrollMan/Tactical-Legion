using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace TurnBaseUtil
{
    public class TurnBaseController
    {
        public int currentTurnIndex = 1;
        private List<Team> teams = new List<Team>();
        private int currentTurnTeamIndex = 0;
        private TurnProperties turnProperties = new TurnProperties();
        public TurnProperties TurnProperties { get { return turnProperties; } set { turnProperties = value; } }

        public UnityEvent OnTurnStart = new UnityEvent();
        public UnityEvent OnTurnEnd = new UnityEvent();

        public TurnBaseController() { }

        public void AddTeam(Team team)
        {
            teams.Add(team);
            team.Index = teams.Count - 1;
        }

        public void RemoveTeam(Team team)
        {
            try
            {
                teams.Remove(team);
            }
            catch (Exception e)
            {
                GameManager.Instance.LogError(e.Message);
            }
        }

        public Team GetTeam(int index)
        {
            try
            {
                return teams[index];
            }
            catch (Exception e)
            {
                GameManager.Instance.LogError(e.Message);
                return null;
            }
        }

        public Team GetCurrentTurnTeam()
        {
            return teams[GetCurrentTurnTeamIndex()];
        }

        public int GetTeamCount()
        {
            return teams.Count;
        }

        public int GetAllTeamPlayerCount()
        {
            int total = 0;
            foreach (var team in teams)
            {
                total += team.teamPlayers.Count;
            }
            return total;
        }

        public void StartTurn()
        {
            foreach (var team in teams)
            {
                foreach (var player in team.teamPlayers)
                {
                    if (player != null && player.PlayerController != null)
                    {
                        player.PlayerController.enabled = false;
                    }
                }
            }

            Team currentTeam = teams[currentTurnTeamIndex];
            currentTeam.IsSelfTurn = true;

            if (currentTeam.GetCurrentTurnPlayer() != null)
            {
                currentTeam.GetCurrentTurnPlayer().PlayerController.enabled = true;
            }

            turnProperties.WindForce = new Vector2(Random.Range(-5f, 5f) * 50, 0);
            OnTurnStart.Invoke();
        }

        public void EndTurn()
        {
            Team currentTeam = teams[currentTurnTeamIndex];
            currentTeam.IsSelfTurn = false;

            if (currentTeam.GetCurrentTurnPlayer() != null)
            {
                currentTeam.GetCurrentTurnPlayer().PlayerController.enabled = false;
            }

            currentTeam.NextTurn();
            NextTurn();

            OnTurnEnd.Invoke();
        }

        public int GetCurrentTurnTeamIndex()
        {
            return currentTurnTeamIndex;
        }

        public int NextTurnTeamIndex()
        {
            if (currentTurnTeamIndex + 1 >= teams.Count)
            {
                return 0;
            }
            return currentTurnTeamIndex + 1;
        }

        public void NextTurn()
        {
            currentTurnTeamIndex++;
            ClampTurnIndex();
        }

        public void ClampTurnIndex()
        {
            if (currentTurnTeamIndex >= teams.Count)
            {
                currentTurnTeamIndex = 0;
            }
        }

        public List<Team> GetTeams()
        {
            return teams;
        }

        public Team GetNextAliveTeam()
        {
            foreach (var team in teams)
            {
                if (team != null && team.GetCurrentTeamPlayerCount() > 0)
                    return team;
            }
            return null;
        }
    }
}
