using System;
using System.Collections;
using System.Collections.Generic;

namespace TurnBaseUtil
{
    public class Team
    {
        public List<TeamPlayer> teamPlayers = new List<TeamPlayer>();
        private int currentTurnPlayerIndex = 0;

        private string name;
        public string Name { get { return name; } set { name = value; } }

        private bool isSelfTurn = false;
        public bool IsSelfTurn { get { return isSelfTurn; } set { isSelfTurn = value; } }

        private int index;
        public int Index { get { return index; } set { index = value; } }

        private int totalHP;
        public int TotalHP { get { return totalHP; } set { totalHP = value; } }

        private int maxHP;

        public Team(string name)
        {
            this.name = name;
        }

        public void InitHP()
        {
            totalHP = 100 * teamPlayers.Count;
            maxHP = totalHP;
        }

        public void UpdateHP()
        {
            int tmpHP = 0;
            foreach(var teamPlayer in teamPlayers)
            {
                tmpHP += teamPlayer.hp;
            }
            totalHP = tmpHP;

            bool isTeamA = (Name == "A");

            if(isTeamA)
                UIManager.Instance.UpdateTeamA_HP_UI(totalHP * 1.0f / maxHP);
            else
                UIManager.Instance.UpdateTeamB_HP_UI(totalHP * 1.0f / maxHP);
        }

        public void AddTeamPlayer(TeamPlayer teamPlayer)
        {
            teamPlayers.Add(teamPlayer);
            teamPlayer.Index = teamPlayers.Count - 1;
            teamPlayer.belongsTo = this;
        }

        public void RemoveTeamPlayer(TeamPlayer teamPlayer)
        {
            try
            {
                teamPlayers.Remove(teamPlayer);
            }
            catch(Exception e)
            {
                GameManager.Instance.LogError(e.Message);
            }
        }

        public TeamPlayer GetCurrentTurnPlayer()
        {
            if (currentTurnPlayerIndex < teamPlayers.Count)
                return teamPlayers[GetCurrentTurnPlayerIndex()];
            else
                return null;
        }

        public int GetCurrentTurnPlayerIndex()
        {
            GameManager.Instance.Log(currentTurnPlayerIndex.ToString());
            return currentTurnPlayerIndex;
        }

        public void NextTurn()
        {
            currentTurnPlayerIndex = (currentTurnPlayerIndex + 1) % teamPlayers.Count;
        }

        public int GetCurrentTeamPlayerCount()
        {
            return teamPlayers.Count;
        }
    }
}
