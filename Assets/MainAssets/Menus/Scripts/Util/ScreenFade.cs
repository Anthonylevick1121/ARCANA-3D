using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Animator))]
public class ScreenFade : MonoBehaviour
{
    private static ScreenFade inst;
    public static ScreenFade instance
    {
        #if UNITY_EDITOR
        get
        {
            if (inst) return inst;
            // statically load into inst
            GameObject obj = (GameObject) PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MainAssets/Menus/Screen Fade Overlay.prefab"));
            inst = obj.GetComponent<ScreenFade>();
            inst.Start();
            DontDestroyOnLoad(obj);
            return inst;
        }
        #else
        get => inst;
        #endif
    }
    
    private void Awake()
    {
        if (inst != null)
        {
            if(inst != this) Destroy(gameObject);
            return;
        }
        
        inst = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // we change the back color before fade
    [SerializeField] private RawImage backCover;
    // could be toggled on or off and animated
    [SerializeField] private TextMeshProUGUI loadingText;
    
    // should we be animating the loading text?
    // yes between once the front cover hits alpha 1, and then until front cover has an alpha of 0.
    private bool animateLoading;
    
    // animation toggling
    private Animator animator;
    private static int doFadeParam = Animator.StringToHash("Cover Screen");
    private static int fadeBackParam = Animator.StringToHash("Fade Back First");
    
    private Action onFadeAction;
    private bool fadeInAfter;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (!animateLoading) return;
        
        // funni loading text ... animation
    }
    
    public void FadeScreen(Action onFade, Color initialFadeColor)
    {
        backCover.color = initialFadeColor;
        FadeScreen(onFade, true);
    }
    public void FadeScreen(Action onFade) => FadeScreen(onFade, false);
    private void FadeScreen(Action onFade, bool fadeBack, bool showLoadingText = false, bool fadeInAfter = true)
    {
        this.fadeInAfter = fadeInAfter;
        onFadeAction = onFade;
        loadingText.gameObject.SetActive(showLoadingText);
        animator.SetBool(fadeBackParam, fadeBack);
        animator.SetBool(doFadeParam, true);
    }
    
    // front canvas is opaque
    public void OnFadeFull()
    {
        onFadeAction?.Invoke();
        animateLoading = loadingText.gameObject.activeInHierarchy;
        if(fadeInAfter)
            animator.SetBool(doFadeParam, false);
    }
    
    // screen is visible again
    public void OnFadeEnd()
    {
        animateLoading = false;
    }
    
    private IEnumerator LoadScene(string scene)
    {
        // async load the next level
        if(PhotonNetwork.IsConnected) PhotonNetwork.SendAllOutgoingCommands();
        yield return null;
        PhotonNetwork.LoadLevel(scene);
        yield return new WaitUntil(() => PhotonNetwork.LevelLoadingProgress >= 1f);
        // level loaded
        animator.SetBool(doFadeParam, false);
    }
    
    private void LoadSceneWithFade(string scene, bool fadeBack, bool showLoadingText)
    {
        FadeScreen(() => StartCoroutine(LoadScene(scene)), fadeBack, showLoadingText, false);
    }
    public void LoadSceneWithFade(string scene, bool showLoadingText) =>
        LoadSceneWithFade(scene, false, showLoadingText);
    public void LoadSceneWithFade(string scene, Color initialFadeColor, bool showLoadingText)
    {
        backCover.color = initialFadeColor;
        LoadSceneWithFade(scene, true, showLoadingText);
    }
}
