using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    static SoundMgr m_inst = null;
    //
    [SerializeField] private AudioListener m_AudioListener = null;
    [SerializeField] private SoundResource m_Resource = null;
    //
    [SerializeField] private UnityEngine.Audio.AudioMixer m_AudioMixer;
    [SerializeField] private GameObject m_AudioSourceBase = null;
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup[] m_MixGourp = new UnityEngine.Audio.AudioMixerGroup[(int)SoundConfig.Channel.MAX_Ch];
    AudioSource[] m_SoundSource = new AudioSource[(int)SoundConfig.Channel.MAX_Ch];
    AudioObject[] m_SoundSourceObj = new AudioObject[(int)SoundConfig.Channel.MAX_Ch];


    static public SoundMgr Instance()
    {
        return m_inst;
    }
    private void Awake()
    {
        m_inst = this;
        for(int i = 0; i < (int)SoundConfig.Channel.MAX_Ch; i++)
        {
            GameObject audiObj = GameObject.Instantiate(m_AudioSourceBase);
            m_SoundSourceObj[i] = audiObj.GetComponent<AudioObject>();
            m_SoundSource[i] = audiObj.GetComponent<AudioSource>();
            m_SoundSource[i].loop = false;
            m_SoundSource[i].playOnAwake = false;
            switch ((SoundConfig.Channel)i)
            {
                default:
                case SoundConfig.Channel.BGM:
                    m_MixGourp[i] = m_AudioMixer.FindMatchingGroups("BGM")[0];
                    m_SoundSource[i].spatialBlend = 0;
                    break;
                case SoundConfig.Channel.CharVoice1:
                case SoundConfig.Channel.CharVoice2:
                    m_MixGourp[i] = m_AudioMixer.FindMatchingGroups("Voice")[0];
                    m_SoundSource[i].spatialBlend = 0;
                    break;
                case SoundConfig.Channel.GameSE_00:
                case SoundConfig.Channel.GameSE_01:
                case SoundConfig.Channel.GameSE_02:
                case SoundConfig.Channel.GameSE_03:
                case SoundConfig.Channel.GameSE_04:
                case SoundConfig.Channel.GameSE_05:
                case SoundConfig.Channel.GameSE_06:
                case SoundConfig.Channel.GameSE_07:
                    m_MixGourp[i] = m_AudioMixer.FindMatchingGroups("GameSE")[0];
                    m_SoundSource[i].spatialBlend = 1;
                    break;
                case SoundConfig.Channel.MenuSE_00:
                case SoundConfig.Channel.MenuSE_01:
                case SoundConfig.Channel.MenuSE_02:
                case SoundConfig.Channel.MenuSE_03:
                case SoundConfig.Channel.MenuSE_04:
                case SoundConfig.Channel.MenuSE_05:
                case SoundConfig.Channel.MenuSE_06:
                case SoundConfig.Channel.MenuSE_07:
                    m_MixGourp[i] = m_AudioMixer.FindMatchingGroups("MenuSE")[0];
                    m_SoundSource[i].spatialBlend = 0;
                    break;

            }
            //ミキサーグループセット
            m_SoundSource[i].outputAudioMixerGroup = m_MixGourp[i];
        }


        }
    // Start is called before the first frame update
    void Start()
    {
        if(m_Resource != null)
        {
            m_Resource.LoadAllSEFiles();
        }
           
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool PlaySound(SoundConfig.Channel channel, SoundConfig.AudioClipType clipType, string key, GameObject obj,float fadeOut_sec=-1)
    {
        switch (channel)
        {
            case SoundConfig.Channel.GameSE:
                channel = GetEmptyChannel(SoundConfig.Channel.SE_START, SoundConfig.Channel.SE_END);
                break;
            case SoundConfig.Channel.MenuSE:
                channel = GetEmptyChannel(SoundConfig.Channel.MENU_SE_START, SoundConfig.Channel.MENU_SE_END);
                break;
            case SoundConfig.Channel.BGM:
                StartCoroutine(LoadPlayBGM(key));
                break;
            default: break;
        }
        if ((int)channel < 0)
        {
            return false;
        }
        if (m_Resource.m_AudioSource.ContainsKey(key) == false)
        {
            return false;
        }
        AudioClip clip = m_Resource.m_AudioSource[key];

        int ch_no = (int)channel;
        if(m_SoundSource[ch_no].isPlaying == false || fadeOut_sec<=0) 
        {
            Play(ch_no, clipType, clip, obj);
        }
        else
        {
            StartCoroutine(FadeOutPlay(fadeOut_sec,ch_no, clipType, clip, obj));
        }
        return true;
    }

    void Play(int ch_no,SoundConfig.AudioClipType clipType,AudioClip clip,GameObject obj)
    {
        switch (clipType)
        {
            default:
            case SoundConfig.AudioClipType.Aud_2D:
                m_SoundSource[ch_no].spatialBlend = 0;
                break;
            case SoundConfig.AudioClipType.Aud_3D:
                m_SoundSource[ch_no].gameObject.transform.position = obj.transform.position;
                m_SoundSource[ch_no].spatialBlend = 1;
                m_SoundSourceObj[ch_no].m_traceObj = obj;
                break;
            case SoundConfig.AudioClipType.Aud_3D_Static:
                m_SoundSource[ch_no].gameObject.transform.position = obj.transform.position;
                m_SoundSource[ch_no].spatialBlend = 1;
                break;
        }

        m_SoundSource[ch_no].clip = clip;
        if (m_SoundSource[ch_no].clip != null)
        {
            m_SoundSource[ch_no].Play();
        }

    }

    IEnumerator FadeOutPlay(float fadeOut_sec, int ch_no, SoundConfig.AudioClipType clipType, AudioClip clip, GameObject obj)
    {
        float start_fadeOut_sec = fadeOut_sec;
        float start_volume = m_SoundSource[ch_no].volume;
        while (m_SoundSource[ch_no].volume > 0)
        {
            fadeOut_sec -= Time.deltaTime;
            m_SoundSource[ch_no].volume = start_volume * (fadeOut_sec/start_fadeOut_sec);
            yield return null;
        }
        m_SoundSource[ch_no].volume = start_fadeOut_sec;
        Play(ch_no, clipType, clip, obj);

    }

    SoundConfig.Channel GetEmptyChannel(SoundConfig.Channel start,SoundConfig.Channel end)
    {
        for(int ch = (int)start; ch <= (int)end; ch++)
        {
            if (m_SoundSource[ch].isPlaying)
            {
                continue;
            }
            return (SoundConfig.Channel)ch;

        }
        return SoundConfig.Channel.Invalid;
    }

    IEnumerator LoadPlayBGM(string key)
    {
        yield return m_Resource.LoadBGMFile(key);

        AudioClip clip = m_Resource.m_AudioSourceBGM[key];
        Play((int)SoundConfig.Channel.BGM, SoundConfig.AudioClipType.Aud_2D, clip, null);
        m_Resource.m_AudioSourceBGM.Clear();

    }
}
