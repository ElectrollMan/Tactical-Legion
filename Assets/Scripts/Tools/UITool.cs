using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UITool
{
    private static GameObject m_CanvasObj = null;

    public static void ReleaseCanvas()
    {
        m_CanvasObj = null;
    }

    public static GameObject FindUIGameObject(string UIName)
    {
        if (m_CanvasObj == null)
            m_CanvasObj = UnityTool.FindGameObject("Canvas");
        if (m_CanvasObj == null)
            return null;
        return UnityTool.FindChildGameObject(m_CanvasObj, UIName);
    }

    public static T GetUIComponent<T>(GameObject Container, string UIName) where T : UnityEngine.Component
    {
        GameObject ChildGameObject = UnityTool.FindChildGameObject(Container, UIName);
        if (ChildGameObject == null)
            return null;

        T tempObj = ChildGameObject.GetComponent<T>();
        if (tempObj == null)
        {
            Debug.LogWarning("Component [" + UIName + "] is not of type [" + typeof(T) + "]");
            return null;
        }
        return tempObj;
    }

    public static Button GetButton(string BtnName)
    {
        GameObject UIRoot = GameObject.Find("Canvas");
        if (UIRoot == null)
        {
            Debug.LogWarning("No UI Canvas found in scene");
            return null;
        }

        Transform[] allChildren = UIRoot.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == BtnName)
            {
                Button tmpBtn = child.gameObject.GetComponent<Button>();
                if (tmpBtn == null)
                    Debug.LogWarning("UI Component [" + BtnName + "] is not a Button");
                return tmpBtn;
            }
        }

        Debug.LogWarning("Button [" + BtnName + "] not found in UI Canvas");
        return null;
    }

    public static T GetUIComponent<T>(string UIName) where T : UnityEngine.Component
    {
        GameObject UIRoot = GameObject.Find("Canvas");
        if (UIRoot == null)
        {
            Debug.LogWarning("No UI Canvas found in scene");
            return null;
        }
        return GetUIComponent<T>(UIRoot, UIName);
    }
}
