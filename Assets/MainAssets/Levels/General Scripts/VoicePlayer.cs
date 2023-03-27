using System;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VoicePlayer : MonoBehaviour
{
    private static VoicePlayer inst;
    public static VoicePlayer instance
    {
#if UNITY_EDITOR
        get
        {
            if (inst) return inst;
            // statically load into inst
            GameObject obj = (GameObject) PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MainAssets/Levels/Voice Lines/Voice Player.prefab"));
            inst = obj.GetComponent<VoicePlayer>();
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
            // finding another means we reloaded the main menu scene; don't play voice clips through that
            if (inst.voicePlayer)
            {
                inst.voicePlayer.Stop();
                inst.subtitle.text = "";
            }
            
            return;
        }
        
        inst = this;
        DontDestroyOnLoad(gameObject);
    }
    
    [SerializeField] private VoiceLineScript voiceScript;
    [SerializeField] private AudioSource voicePlayer;
    [SerializeField] private TextMeshProUGUI subtitle;
    private float subtitleTimer;
    private string captions;
    private bool paused;
    
    private int cachedClip = -1;
    
    // Start is called before the first frame update
    private void Start()
    {
        subtitle.text = "";
        subtitle.canvas.sortingOrder = (int) CanvasLayer.Subtitles;
        if (cachedClip >= 0)
            PlayVoiceLine((VoiceLineId) cachedClip);
    }
    
    private void Update()
    {
        if (paused || subtitleTimer <= 0) return;
        if (captions != null)
        {
            subtitle.text = captions;
            captions = null;
        }
        
        subtitleTimer -= Time.deltaTime;
        if (subtitleTimer <= 0) subtitle.text = "";
    }
    
    public void PauseVoice(bool pause)
    {
        paused = pause;
        if(pause) voicePlayer.Pause();
        else voicePlayer.UnPause();
    }
    
    public void PlayVoiceLine(VoiceLineId id)
    {
        print("play voice line "+Enum.GetName(typeof(VoiceLineId), id));
        if (!voicePlayer)
            cachedClip = (int) id;
        else
        {
            voicePlayer.Stop();
            VoiceLineScript.VoiceLine line = voiceScript.GetLine(id);
            voicePlayer.clip = line.clip;
            if (voicePlayer.clip)
            {
                voicePlayer.volume = line.volume;
                voicePlayer.PlayDelayed(0.75f); // delay to account for fade time, etc
            }
            
            captions = line.subtitle;
            subtitleTimer = (voicePlayer.clip ? line.clip.length : captions.Split(" ").Length / 3f) + 2;
        }
    }
}
