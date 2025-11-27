using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnBaseUtil
{
    public class TeamPlayer : MonoBehaviour
    {
        private int index;
        public int Index { get { return index; } set { index = value; } }

        public PlayerController PlayerController { get; set; }
        public Team belongsTo { get; set; }

        private bool isDestroyed;
        private PlayerUI ui;

        private string name;
        public string Name { get { return name; } set { name = value; } }

        private Color teamColor;
        public int hp = 100;

        public TeamPlayer(string name, Color color)
        {
            this.name = name;
            teamColor = color;
        }

        public void InitUI()
        {
            ui.UpdateName(name);
            ui.UpdateColor(teamColor);
            ui.UpdatePlayerHP(hp);
        }

        public void Copy(TeamPlayer teamPlayer)
        {
            name = teamPlayer.Name;
            teamColor = teamPlayer.teamColor;
            belongsTo = teamPlayer.belongsTo;
        }

        public void DoHurt(int damage)
        {
            hp -= damage;
            if (hp <= 0)
            {
                hp = 0;
                PlayerController.GetComponent<Animator>().SetTrigger("Die");
                PlayerController.GetComponent<AudioSource>().PlayOneShot(PlayerController.dieSFX);
                ui.SetHudActive(false);
                ui.UpdatePlayerHP(hp);
                belongsTo.UpdateHP();
                Invoke("RemoveSelf", 2f);
                PlayerController.IsDead = true;
                return;
            }
            ui.UpdatePlayerHP(hp);
            belongsTo.UpdateHP();
        }

        void Start()
        {
            PlayerController = GetComponent<PlayerController>();
            ui = GetComponent<PlayerUI>();
        }

        private void RemoveSelf()
        {
            if (belongsTo != null)
                belongsTo.RemoveTeamPlayer(this);

            Destroy(gameObject);

            bool anyAlive = false;
            foreach (var team in GameManager.Instance.TurnBaseController.GetTeams())
            {
                if (team != null && team.GetCurrentTeamPlayerCount() > 0)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                Debug.Log("Draw! All players are dead.");
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowDrawUI();
                return;
            }

            if (belongsTo != null && belongsTo.GetCurrentTeamPlayerCount() > 0)
            {
                GameManager.Instance.TurnBaseController.EndTurn();
                GameManager.Instance.TurnBaseController.StartTurn();
            }
            else
            {
                Team winTeam = GameManager.Instance.TurnBaseController.GetNextAliveTeam();
                if (winTeam != null)
                {
                    Debug.Log(winTeam.Name + " Win");
                    UIManager.Instance.ShowWinInfoUI(winTeam.Name);

                    if (winTeam.GetCurrentTurnPlayer() != null)
                        GameManager.Instance.vCam.Follow = winTeam.GetCurrentTurnPlayer().transform;
                }
                else
                {
                    Debug.Log("No surviving teams — Draw!");
                    if (UIManager.Instance != null)
                        UIManager.Instance.ShowDrawUI();
                }
            }
        }

        void Update()
        {
            if (transform.position.y < -8f)
            {
                if (!isDestroyed && !PlayerController.IsDead)
                {
                    isDestroyed = true;
                    DieByFalling();
                }
            }
        }

        private void DieByFalling()
        {
            hp = 0;
            ui.UpdatePlayerHP(hp);
            belongsTo.UpdateHP();
            PlayerController.IsDead = true;

            PlayerController.GetComponent<Animator>().SetTrigger("Die");
            PlayerController.GetComponent<AudioSource>().PlayOneShot(PlayerController.dieSFX);
            ui.SetHudActive(false);

            Invoke(nameof(RemoveSelf), 1.5f);
        }
    }
}
