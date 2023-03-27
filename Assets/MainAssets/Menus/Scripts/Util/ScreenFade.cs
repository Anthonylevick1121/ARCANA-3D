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
    
    public void FadeScreen(CanvasLayer fadeLayer, Action onFade, Color initialFadeColor)
    {
        backCover.color = initialFadeColor;
        FadeScreen(fadeLayer, onFade, true);
    }
    public void FadeScreen(CanvasLayer fadeLayer, Action onFade) => FadeScreen(fadeLayer, onFade, false);
    private void FadeScreen(CanvasLayer fadeLayer, Action onFade,
        bool fadeBack, bool showLoadingText = false, bool fadeInAfter = true)
    {
        backCover.canvas.sortingOrder = (int) fadeLayer;
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
    
    private void LoadSceneWithFade(string scene, bool fadeBack, bool showLoadingText, CanvasLayer transitionLayer)
    {
        FadeScreen(transitionLayer, () => StartCoroutine(LoadScene(scene)), fadeBack, showLoadingText, false);
    }
    
    public void LoadSceneWithFade(string scene, bool showLoadingText,
        CanvasLayer transitionLayer = CanvasLayer.MenuTransition)
    {
        LoadSceneWithFade(scene, false, showLoadingText, transitionLayer);
    }

    public void LoadSceneWithFade(string scene, Color initialFadeColor, bool showLoadingText,
        CanvasLayer transitionLayer = CanvasLayer.MenuTransition)
    {
        backCover.color = initialFadeColor;
        LoadSceneWithFade(scene, true, showLoadingText, transitionLayer);
    }
}

public enum CanvasLayer
{
    // available layers: hud, subtitles, pause screen, win/lose screen, main menus, screen fade
    
    // hud always bottom
    // end screen below subtitles and end transition
    // pause screen above subtitles and hud, below main menu transition
    // subtitles above all except main menus and main menu transitions
    // main menus above all except main menu transitions
    
    // other level fades: player caught, between-level load
    // - caught uses MoveTransition
    // - between-level will count as a menu transition
    
    // main menus will never be present with any of the rest of these, aside from the transition
    // exception is credits, with the subtitles, so we add that instead
    
    Hud, MoveTransition, EndScreen, EndTransition, Subtitles, PauseScreen, CreditsScreen, MenuTransition
}
