using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour
{
    public Transform moveDest;
    
    [NonSerialized]
    private bool created = false;
    
    private struct ButtonAction
    {
        public string name;
        public Action<GameObject> action;
        public bool needObj;
    }
    
    private List<ButtonAction> buttons = new ();
    
    private void Add(string name, Action<GameObject> action, bool needObj = true)
    {
        buttons.Add(new ButtonAction
        {
            name = name, action = action, needObj = needObj
        });
    }
    
    private void OnEnable()
    {
        if(gameObject.name != "EditorHelper") DestroyImmediate(this);
        Selection.selectionChanged += Refresh;
    }
    private void OnDisable() => Selection.selectionChanged -= Refresh;
    
    private void Refresh()
    {
        if (!Selection.activeGameObject) EditorApplication.QueuePlayerLoopUpdate();
    }
    
    private void OnGUI()
    {
        if (!created)
        {
            Create();
            created = true;
        }
        
        float xoff = 10;
        float yoff = 30;
        float height = 30;
        float ydelta = 40;
        GameObject obj = Selection.activeGameObject;
        foreach (ButtonAction button in buttons)
        {
            if (button.needObj && !obj) continue;
            string line = (obj ? obj.name : "null") + " - " + button.name;
            if (GUI.Button(new Rect(xoff, yoff, line.Length * 8, height), line))
            {
                if(!obj) button.action.Invoke(null);
                else foreach(GameObject eachobj in Selection.gameObjects) button.action.Invoke(eachobj);
            }
            yoff += ydelta;
        }
    }
    
    private void Create()
    {
        Add("Add Components", AddComponents);
        Add("Move GameObjects", MoveGameObjects);
        Add("Count Children", obj => print("children: "+obj.transform.childCount));
        Add("Zero Transform", ZeroTransform);
        Add("Integer Children Locs", IntegerLocations);
        Add("Clear Overlapping Children", ClearDuplicates);
    }

    private void ClearDuplicates(GameObject obj)
    {
        Transform t = obj.transform;
        HashSet<Vector3> positions = new ();
        int removed = 0;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            if (!positions.Add(child.localPosition))
            {
                DestroyImmediate(child.gameObject);
                i--;
                removed++;
            }
        }
        print($"removed {removed} overlapping children.");
    }
    
    private void IntegerLocations(GameObject obj)
    {
        Transform t = obj.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            Vector3 pos = child.localPosition;
            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);
            pos.z = Mathf.RoundToInt(pos.z);
            child.localPosition = pos;
        }
    }
    
    private void ZeroTransform(GameObject obj)
    {
        if (obj == gameObject || obj.IsPrefabInstance())
        {
            print("Not allowed for this object.");
            return;
        }
        Transform save = moveDest;
        moveDest = transform;
        MoveGameObjects(obj);
        obj.transform.localPosition = Vector3.zero;
        moveDest = obj.transform;
        MoveGameObjects(gameObject);
        moveDest = save;
    }
    
    private void MoveGameObjects(GameObject obj)
    {
        Transform t = obj.transform;
        print($"Checking to move {t.childCount} children");
        int moved = 0;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            // if(i % 100 == 0) print(child.gameObject.name);
            // if(child.gameObject.name == "FloorTile (1906)") print("rotation: "+child.rotation);
            // if(child.gameObject.name.Equals("FloorTIle (1906)")) print("rotation: "+child.localRotation.eulerAngles);
            // if(child.gameObject.name.Equals("FloorTIle (1906)")) print("pos: "+child.localPosition);
            // if (Math.Abs(child.localPosition.y - 8) < 0.01f)
            {
                child.SetParent(moveDest);
                i--;
                moved++;
            }
        }
        print($"moved {moved} children");
    }
    
    private void AddComponents(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            Transform color = obj.transform.GetChild(i);
            for (int j = 3; j <= 4; j++)
            {
                Transform wallParent = color.GetChild(j);
                for (int k = 0; k < wallParent.childCount; k++)
                {
                    GameObject wall = wallParent.GetChild(k).gameObject;
                    NavMeshObstacle obs = wall.AddComponent<NavMeshObstacle>();
                    obs.carving = true;
                }
            }
        }
    }
}

[CustomEditor(typeof(EditorHelper))]
public class HelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("I'd really RECOMMEND you don't touch this or its game-view buttons lol\n\n" +
            "There is a high chance of breaking things and/or getting the editor stuck in an infinite loop.", MessageType.Warning);
        base.OnInspectorGUI();
    }
}