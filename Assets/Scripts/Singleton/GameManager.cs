using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TurnBaseUtil;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CinemachineVirtualCamera vCam;

    public TurnBaseController TurnBaseController { get; set; }
    [HideInInspector]
    public List<GameObject> teamA_Players;
    [HideInInspector]
    public List<GameObject> teamB_Players;

    public IEnumerator DelayFuc(Action action, float delaySeconds)
    {
        Log("StartDelay");
        yield return new WaitForSeconds(delaySeconds);
        action();
        Log("EndDelay");
    }

    public void Log(string msg)
    {
        Debug.Log(msg);
    }

    public void LogError(string errormsg)
    {
        Debug.LogError(errormsg);
    } 

    public void InitGame()
    {
        Log("GameInit");
        vCam.Follow = null;

        Global.teamA.teamPlayers.Clear();
        Global.teamB.teamPlayers.Clear();

        foreach (var teamA_Player in teamA_Players)
        {
            TeamPlayer teamPlayer = teamA_Player.GetComponent<TeamPlayer>();
            teamPlayer.InitUI();
            Global.teamA.AddTeamPlayer(teamPlayer);
            teamPlayer.PlayerController.enabled = false;
        }
        Global.teamA.InitHP();

        foreach (var teamB_Player in teamB_Players)
        {
            TeamPlayer teamPlayer = teamB_Player.GetComponent<TeamPlayer>();
            teamPlayer.InitUI();
            Global.teamB.AddTeamPlayer(teamPlayer);
            teamPlayer.PlayerController.enabled = false;
        }
        Global.teamB.InitHP();

        TurnBaseController.AddTeam(Global.teamA);
        TurnBaseController.AddTeam(Global.teamB);

        TurnBaseController.StartTurn();
    }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }

    void Start()
    {
        TurnBaseController = new TurnBaseController();
        StartCoroutine(DelayFuc(() => { SendMessage("GameInited"); InitGame(); }, 0.2f));
    }

    public void CheckPlayer()
    {
        Debug.Log("Current team: " + TurnBaseController.GetCurrentTurnTeam().Name + 
                  ", Player index: " + TurnBaseController.GetCurrentTurnTeam().GetCurrentTurnPlayerIndex());
        Debug.Log("Player name: " + TurnBaseController.GetCurrentTurnTeam().GetCurrentTurnPlayer().Name);

        var playerController = TurnBaseController.GetCurrentTurnTeam().GetCurrentTurnPlayer().PlayerController;
        if (playerController == null || !playerController.enabled)
        {
            TurnBaseController.EndTurn();
            TurnBaseController.StartTurn();
        }
    }

    void Update()
    {
    }
}
