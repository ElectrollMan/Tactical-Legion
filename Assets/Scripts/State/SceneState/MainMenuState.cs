using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TurnBaseUtil;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuState : ISceneState
{
    public MainMenuState(SceneStateController Controller) : base(Controller)
    {
        this.StateName = "MainMenuState";
    }

    public override void StateBegin()
    {
        GameObject gameloop = UnityTool.FindGameObject("GameLoop");
        GameLoop gameLoopScript = gameloop.GetComponent<GameLoop>();
        AudioSource audio = gameloop.GetComponent<AudioSource>();
        if (audio.clip != gameLoopScript.start)
        {
            audio.clip = gameLoopScript.start;
            audio.Play();
        }

        // === Main menu buttons ===
        UITool.GetButton("StartGameButton").onClick.AddListener(() => OnStartGameBtnClick(null));
        UITool.GetButton("StartMultiplayerGameButton").onClick.AddListener(() => OnStartMultiplayerBtnClick(null));
        UITool.GetButton("AboutButton").onClick.AddListener(() => OnAboutBtnClick(null));
        UITool.GetButton("OkButton").onClick.AddListener(() => OnOkBtnClick(null));
        UITool.GetButton("ExitGameButton").onClick.AddListener(() => OnExitGameBtnClick(null));
        UITool.GetButton("AudioButton").onClick.AddListener(() => OnAudioBtnClick(null));
        UITool.GetButton("CloseButton").onClick.AddListener(() => OnCloseAudioManagerBtnClick(null));
        UITool.GetButton("ControlButton").onClick.AddListener(() => OnControlBtnClick(null));

        // === Team Settings Back button ===
        UITool.GetButton("BackButton").onClick.AddListener(() => OnBackBtnClick(null));

        // === Control Panel Back button (if named differently) ===
        Button btnControlBack = UITool.GetButton("CoolButton"); // change this if your ControlPanel button has a different name
        if (btnControlBack != null)
        {
            Debug.Log("Control Back button found!");
            btnControlBack.onClick.AddListener(() => OnBackBtnClick(btnControlBack));
        }
        else
        {
            Debug.LogWarning("Control Back button not found. Check its GameObject name in the scene!");
        }
    }

    private void OnStartGameBtnClick(Button button)
    {
        Global.teamA.teamPlayers.Clear();
        Global.teamB.teamPlayers.Clear();
        Global.teamA.AddTeamPlayer(new TeamPlayer(UITool.GetUIComponent<Text>("PlayerA1_Name").text, Global.teamA_Color));
        Global.teamA.AddTeamPlayer(new TeamPlayer(UITool.GetUIComponent<Text>("PlayerA2_Name").text, Global.teamA_Color));
        Global.teamA.AddTeamPlayer(new TeamPlayer(UITool.GetUIComponent<Text>("PlayerA3_Name").text, Global.teamA_Color));
        Global.teamB.AddTeamPlayer(new TeamPlayer(UITool.GetUIComponent<Text>("PlayerB1_Name").text, Global.teamB_Color));
        Global.teamB.AddTeamPlayer(new TeamPlayer(UITool.GetUIComponent<Text>("PlayerB2_Name").text, Global.teamB_Color));
        Global.teamB.AddTeamPlayer(new TeamPlayer(UITool.GetUIComponent<Text>("PlayerB3_Name").text, Global.teamB_Color));
        m_Controller.SetState(new LevelSelectState(m_Controller), "LevelSelectScene");
    }

    private void OnBackBtnClick(Button button)
    {
        Debug.Log("Back button clicked!");
        UITool.FindUIGameObject("TeamSettingsPanel").transform.DOMoveY(10f, 1f);
        UITool.FindUIGameObject("ControlPanel").transform.DOMoveY(10f, 1f);
        UITool.FindUIGameObject("MainMenuPanel").transform.DOMoveY(0, 1f);
    }

    private void OnStartMultiplayerBtnClick(Button button)
    {
        UITool.FindUIGameObject("MainMenuPanel").transform.DOMoveY(10f, 0f);
        UITool.FindUIGameObject("TeamSettingsPanel").transform.DOMoveY(0, 1f);
    }

    private void OnAboutBtnClick(Button button)
    {
        UITool.FindUIGameObject("AboutPanel").transform.DOMoveY(0, 0f);
    }

    private void OnOkBtnClick(Button button)
    {
        UITool.FindUIGameObject("AboutPanel").transform.DOMoveY(10f, 0f);
    }

    private void OnExitGameBtnClick(Button button)
    {
        Application.Quit();
    }

    private void OnAudioBtnClick(Button button)
    {
        UITool.FindUIGameObject("AudioManagerPanel").transform.DOLocalMoveY(0, 0.5f);
    }

    private void OnCloseAudioManagerBtnClick(Button button)
    {
        UITool.FindUIGameObject("AudioManagerPanel").transform.DOLocalMoveY(500, 0.5f);
    }

    private void OnControlBtnClick(Button button)
    {
        UITool.FindUIGameObject("ControlPanel").transform.DOMoveY(0, 1f);
    }
}
