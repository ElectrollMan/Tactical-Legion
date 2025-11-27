using UnityEngine;
using UnityEngine.UI;

public class LevelSelectState : ISceneState
{
    public LevelSelectState(SceneStateController Controller) : base(Controller)
    {
        this.StateName = "LevelSelectState";
    }

    public override void StateBegin()
    {
        AssignLevelButton("Level1Button", "BattleScene", "Junkyard");
        AssignLevelButton("Level2Button", "BattleScene", "Surface");
        AssignLevelButton("Level3Button", "BattleScene", "Towers");
        AssignLevelButton("Level4Button", "BattleScene", "FloatIsland");
        AssignLevelButton("Level5Button", "BattleScene", "BigBoat");

        Button btnBack = UITool.GetButton("BackButton");
        if (btnBack != null)
            btnBack.onClick.AddListener(OnBackBtnClick);
        else
            Debug.LogWarning("BackButton not found in LevelSelectScene!");
    }

    private void AssignLevelButton(string buttonName, string sceneName, string mapName)
    {
        Button btn = UITool.GetButton(buttonName);
        if (btn != null)
        {
            btn.onClick.AddListener(() => OnLevelSelect(sceneName, mapName));
            Debug.Log($"Assigned {buttonName} → Map: {mapName}");
        }
        else
        {
            Debug.LogWarning($"Button '{buttonName}' not found in LevelSelectScene!");
        }
    }

    private void OnLevelSelect(string sceneName, string mapName)
    {
        Global.SelectedMap = mapName;
        Debug.Log($"Selected Map: {mapName}");

        m_Controller.SetState(new BattleState(m_Controller), sceneName);
    }

    private void OnBackBtnClick()
    {
        m_Controller.SetState(new MainMenuState(m_Controller), "MainMenuScene");
    }
}
