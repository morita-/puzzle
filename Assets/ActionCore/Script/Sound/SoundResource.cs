using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundResource : MonoBehaviour
{
    [SerializeField] private SoundData m_SoundData = null;

    public Dictionary<string,AudioClip> m_AudioSource = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> m_AudioSourceBGM = new Dictionary<string, AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator LoadBGMFile(string key)
    {
        foreach(SoundDataBgmEntity bgmObj in m_SoundData.BGM)
        {
            if(bgmObj.bgmkey == key)
            {
                ResourceRequest req = Resources.LoadAsync<AudioClip>(bgmObj.path + "/" + bgmObj.file);
                yield return LoadFile(req, bgmObj.bgmkey, m_AudioSourceBGM);
                yield break;
            }
        }
    }
    public void LoadAllSEFiles()
    {
        foreach(SoundDataSeEntity seObj in m_SoundData.SE)
        {
            ResourceRequest req = Resources.LoadAsync<AudioClip>(seObj.path + "/" + seObj.file);
            StartCoroutine(LoadFile(req, seObj.sekey, m_AudioSource));
        }
    }

    IEnumerator LoadFile(ResourceRequest req,string sekey, Dictionary<string,AudioClip> dic)
    {
        while (req.isDone == false)
        {
            yield return null;
        }

        AudioClip clip = req.asset as AudioClip;
        dic.Add(sekey, clip);
    }
}
