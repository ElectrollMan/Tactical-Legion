using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleState : ISceneState
{
    private bool isPaused;
    private RectTransform canvasRectTransform;
    private RectTransform menuPanelRectTransform;

    private GameObject gamePausedPanel;
    private CanvasGroup gamePausedCanvasGroup;

    public BattleState(SceneStateController Controller) : base(Controller)
    {
        this.StateName = "BattleState";
    }

    public override void StateBegin()
    {
        GameObject gameloop = UnityTool.FindGameObject("GameLoop");
        GameLoop gameLoopScript = gameloop.GetComponent<GameLoop>();
        AudioSource audio = gameloop.GetComponent<AudioSource>();
        audio.clip = gameLoopScript.battleGame;
        audio.Play();

        GameObject canvas = UITool.FindUIGameObject("Canvas");
        canvasRectTransform = canvas.GetComponent<RectTransform>();

        GameObject menuPanel = UITool.FindUIGameObject("MenuPanel");
        menuPanelRectTransform = menuPanel.GetComponent<RectTransform>();

        gamePausedPanel = UITool.FindUIGameObject("GamePausedPanel");
        if (gamePausedPanel != null)
        {
            if (gamePausedPanel.GetComponent<CanvasGroup>() == null)
                gamePausedCanvasGroup = gamePausedPanel.AddComponent<CanvasGroup>();
            else
                gamePausedCanvasGroup = gamePausedPanel.GetComponent<CanvasGroup>();

            gamePausedPanel.SetActive(false);
            gamePausedCanvasGroup.alpha = 0;
        }

        Button btnResumeGame = UITool.GetButton("ResumeGameButton");
        btnResumeGame.onClick.AddListener(() => ResumeGame(btnResumeGame));

        Button btnBackToMainMenu = UITool.GetButton("BackToMainMenuButton");
        btnBackToMainMenu.onClick.AddListener(() => BackToMainMenu(btnBackToMainMenu));

        Button btnExitGame = UITool.GetButton("ExitGameButton");
        btnExitGame.onClick.AddListener(() => ExitGame(btnExitGame));

        Button btnEndTurn = UITool.GetButton("EndTurnButton");
        btnEndTurn.onClick.AddListener(() => EndTurn(btnEndTurn));

        Button btnBackToMainMenu2 = UITool.GetButton("BackToMainMenuButton2");
        btnBackToMainMenu2.onClick.AddListener(() => BackToMainMenu2(btnBackToMainMenu2));
    }

    public override void StateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                Time.timeScale = 0;
                menuPanelRectTransform.DOLocalMoveX(-canvasRectTransform.rect.width / 2, 0.5f).SetUpdate(true);
                isPaused = true;

                if (!UIManager.Instance.isEnd)
                    UIManager.Instance.SetEndTurnButtonActive(false);

                if (gamePausedPanel != null)
                {
                    gamePausedPanel.SetActive(true);
                    gamePausedCanvasGroup.DOFade(1f, 0.25f).SetUpdate(true);
                }
            }
            else
            {
                Time.timeScale = 1;
                menuPanelRectTransform.DOLocalMoveX(-canvasRectTransform.rect.width / 2 - 800f, 0.5f).SetUpdate(true);
                isPaused = false;

                if (!UIManager.Instance.isEnd)
                    UIManager.Instance.SetEndTurnButtonActive(true);

                if (gamePausedPanel != null)
                {
                    gamePausedCanvasGroup.DOFade(0f, 0.25f).SetUpdate(true)
                        .OnComplete(() => gamePausedPanel.SetActive(false));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (UIManager.Instance != null && !UIManager.Instance.canOpenBag)
            {
                Debug.Log("Bag is locked — cannot open right now.");
                return;
            }

            GameObject bagPanel = UITool.FindUIGameObject("BagPanel");
            if (!UIManager.Instance.isOpenedBag)
            {
                bagPanel.transform.DOLocalMoveX(250, 1f);
                UIManager.Instance.isOpenedBag = true;
            }
            else
            {
                bagPanel.transform.DOLocalMoveX(600, 1f);
                UIManager.Instance.isOpenedBag = false;
            }
        }
    }

    private void EndTurn(Button button)
    {
        GameManager.Instance.TurnBaseController.EndTurn();
        GameManager.Instance.TurnBaseController.StartTurn();
    }

    private void ResumeGame(Button button)
    {
        Time.timeScale = 1;
        menuPanelRectTransform.DOLocalMoveX(-canvasRectTransform.rect.width / 2 - 300f, 0.5f).SetUpdate(true);
        isPaused = false;
    }

    private void BackToMainMenu(Button button)
    {
        Time.timeScale = 1;
        m_Controller.SetState(new MainMenuState(m_Controller), "MainMenuScene");
    }

    private void ExitGame(Button button)
    {
        Application.Quit();
    }

    private void BackToMainMenu2(Button button)
    {
        m_Controller.SetState(new MainMenuState(m_Controller), "MainMenuScene");
    }
}
