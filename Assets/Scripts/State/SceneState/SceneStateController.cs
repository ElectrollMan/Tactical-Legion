using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateController
{
    private ISceneState m_State;
    private bool m_bRunBegin = false;
    private AsyncOperation m_AsyncOperation = null;

    public SceneStateController() { }

    public void SetState(ISceneState State, string LoadLevelName)
    {
        Debug.Log("SetState:" + State.ToString());

        m_bRunBegin = false;

        LoadLevel(LoadLevelName);

        if (m_State != null)
            m_State.StateEnd();

        m_State = State;
    }

    private void LoadLevel(string LoadLevelName)
    {
        if (string.IsNullOrEmpty(LoadLevelName))
            return;

        m_AsyncOperation = SceneManager.LoadSceneAsync(LoadLevelName);
    }

    public void StateUpdate()
    {
        if (m_AsyncOperation != null && !m_AsyncOperation.isDone)
            return;

        if (m_State != null && !m_bRunBegin)
        {
            m_State.StateBegin();
            m_bRunBegin = true;
        }

        m_State?.StateUpdate();
    }
}
