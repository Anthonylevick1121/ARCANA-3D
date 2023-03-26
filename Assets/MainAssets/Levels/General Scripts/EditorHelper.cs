#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class EditorHelper : MonoBehaviour
{
    private void Awake()
    {
        gameObject.hideFlags |= HideFlags.DontSaveInBuild;
    }
    
    public Transform secondary;
    
    [NonSerialized]
    private bool created = false;
    
    private struct ButtonAction
    {
        public string name;
        public Action<Transform> action;
        public Func<Transform, bool> check;
    }
    
    private List<ButtonAction> buttons = new ();
    
    private void Add(string name, Action<Transform> action, Func<Transform, bool> check)
    {
        buttons.Add(new ButtonAction
        {
            name = name, action = action, check = check ?? (t => t)
        });
    }
    private void Add(string name, Action<Transform> action, bool needObj = true) =>
        Add(name, action, t => needObj ? t : true);
    private void Add(string name, Action<Transform, Transform> action, bool needObj = true, bool needSecondary = true) =>
        Add(name, t => action.Invoke(t, secondary),
            t => (!needObj || t) && (!needSecondary || secondary));
    
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
        GameObject[] objs = Selection.gameObjects;
        foreach (ButtonAction button in buttons)
        {
            bool hasSome = objs.Length > 0;
            bool hasMany = objs.Length > 1;
            if (!button.check(hasSome ? Selection.activeGameObject.transform : null)) continue;
            string line = (hasMany ? $"{objs.Length} Objects" : hasSome ? objs[0].name : "null") + " - " + button.name;
            if (GUI.Button(new Rect(xoff, yoff, line.Length * 8, height), line))
            {
                if(!hasSome) button.action.Invoke(null);
                else foreach(GameObject eachobj in objs) button.action.Invoke(eachobj.transform);
            }
            yoff += ydelta;
        }
    }
    
    private void Create()
    {
        // Add("Add Components", AddComponents);
        Add("Count Children", obj => print("children: "+obj.transform.childCount));
        Add("Move Children", MoveChildren);
        Add("Move To", (t, target) => t.SetParent(target));
        Add("Zero Transform", ZeroTransform);
        Add("Zero Transform Relative", ZeroTransformRelative);
        Add("Integer Children Locs", IntegerLocations);
        Add("Clear Overlapping Children", ClearDuplicates);
        Add("Duplicate and Flip Children", t =>
        {
            DuplicateAndMod(t.GetChild(2));
            DuplicateAndMod(t.GetChild(3));
        });
        Add("Sort Walls", CategorizeMaze);
    }
    
    private void ClearDuplicates(Transform t)
    {
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
    
    private void DuplicateAndMod(Transform t)
    {
        // duplicate all children and then flip them 180 deg on y
        int origCount = t.childCount;
        for (int i = 0; i < origCount; i++)
        {
            GameObject child = t.GetChild(i).gameObject;
            Selection.activeGameObject = child;
            Unsupported.CopyGameObjectsToPasteboard();
            Unsupported.PasteGameObjectsFromPasteboard();
            Transform dup = Selection.activeTransform;
            dup.Rotate(Vector3.up, 180, Space.World);
            dup.Translate(Vector3.up * 0.2f, Space.Self);
        }
    }
    
    private void IntegerLocations(Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            Vector3 pos = child.localPosition;
            pos.x = Mathf.RoundToInt(pos.x / 4) * 4;
            pos.y = Mathf.RoundToInt(pos.y / 4) * 4;
            pos.z = Mathf.RoundToInt(pos.z / 4) * 4;
            child.localPosition = pos;
        }
    }
    
    private void ZeroTransform(Transform t)
    {
        if (t.gameObject == gameObject || t.gameObject.IsPrefabInstance())
        {
            print("Not allowed for this object.");
            return;
        }
        MoveChildren(t, transform);
        t.localPosition = Vector3.zero;
        MoveChildren(transform, t);
    }
    
    private void ZeroTransformRelative(Transform t, Transform relativeTo)
    {
        if (t.gameObject == gameObject || t.gameObject == relativeTo.gameObject || t.gameObject.IsPrefabInstance())
        {
            print("Not allowed for this object.");
            return;
        }
        MoveChildren(t, transform);
        int idx = t.GetSiblingIndex();
        Transform original = t.parent;
        t.SetParent(relativeTo);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.SetParent(original);
        t.SetSiblingIndex(idx);
        MoveChildren(transform, t);
    }
    
    private void MoveChildren(Transform t, Transform target)
    {
        if (t.gameObject == target.gameObject || t.gameObject.IsPrefabInstance())
        {
            print($"Not allowed to move children from {t.name} to {target.name}.");
            return;
        }
        
        print($"Checking to move {t.childCount} children");
        int moved = 0;
        for (int i = 0; i < t.childCount; i++)
        {
            t.GetChild(i).SetParent(target);
            i--;
            moved++;
        }
        print($"moved {moved} children");
    }
    
    private void CategorizeMaze(Transform t)
    {
        // t is the parent
        // walls active is 0, inactive is 1, 9 after are each section
        for (int i = 0; i < 9; i++)
        {
            // GameObject active = new ("Walls Active");
            // active.transform.SetParent(t.GetChild(i+2));
            // GameObject inactive = new ("Walls Inactive");
            // inactive.transform.SetParent(t.GetChild(i+2));
            // inactive.SetActive(false);
            t.GetChild(i).GetChild(3).gameObject.SetActive(true);
        }
        // parents created, sort
        
        void Sort(Transform src, int dst)
        {
            while (src.childCount > 0)
            {
                Transform wall = src.GetChild(0);
                int section = (int) MazePuzzle.GetMazeSection(wall.position);
                wall.SetParent(t.GetChild(section).GetChild(dst));
            }
        }
        // active first, then inactive
        // Sort(secondary.GetChild(0), 2);
        // Sort(secondary.GetChild(1), 3);
    }
    
    private void AddComponents(Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform color = t.GetChild(i);
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
        EditorGUILayout.HelpBox("I'd REALLY RECOMMEND you don't touch this or its game-view buttons lol\n\n" +
            "There is a high chance of breaking things and/or getting the editor stuck in an infinite loop.", MessageType.Warning);
        base.OnInspectorGUI();
    }
}
#endif