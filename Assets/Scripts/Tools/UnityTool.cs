using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityTool
{
    public static void Attach(GameObject ParentObj, GameObject ChildObj, Vector3 Pos)
    {
        ChildObj.transform.parent = ParentObj.transform;
        ChildObj.transform.localPosition = Pos;
    }

    public static void AttachToRefPos(GameObject ParentObj, GameObject ChildObj, string RefPointName, Vector3 Pos)
    {
        bool found = false;
        Transform refTransform = null;
        Transform[] allChildren = ParentObj.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == RefPointName)
            {
                if (found)
                {
                    Debug.LogWarning("Parent [" + ParentObj.name + "] has multiple reference points [" + RefPointName + "]");
                    continue;
                }
                found = true;
                refTransform = child;
            }
        }

        if (!found)
        {
            Debug.LogWarning("Parent [" + ParentObj.name + "] does not contain reference point [" + RefPointName + "]");
            Attach(ParentObj, ChildObj, Pos);
            return;
        }

        ChildObj.transform.parent = refTransform;
        ChildObj.transform.localPosition = Pos;
        ChildObj.transform.localScale = Vector3.one;
        ChildObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public static GameObject FindGameObject(string GameObjectName)
    {
        GameObject obj = GameObject.Find(GameObjectName);
        if (obj == null)
        {
            Debug.LogWarning("GameObject [" + GameObjectName + "] not found in scene");
            return null;
        }
        return obj;
    }

    public static GameObject FindChildGameObject(GameObject Container, string gameobjectName)
    {
        if (Container == null)
        {
            Debug.LogError("Container is null");
            return null;
        }

        Transform foundTransform = null;

        if (Container.name == gameobjectName)
        {
            foundTransform = Container.transform;
        }
        else
        {
            Transform[] allChildren = Container.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == gameobjectName)
                {
                    if (foundTransform == null)
                        foundTransform = child;
                    else
                        Debug.LogWarning("Container [" + Container.name + "] has duplicate child names [" + gameobjectName + "]");
                }
            }
        }

        if (foundTransform == null)
        {
            Debug.LogError("Child [" + gameobjectName + "] not found under Container [" + Container.name + "]");
            return null;
        }

        return foundTransform.gameObject;
    }
}
