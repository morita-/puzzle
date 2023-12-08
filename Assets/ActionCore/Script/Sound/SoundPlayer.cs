using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


static class PlaySound
{
    static public void PlayBGM(string bgmkey){
        SoundMgr soundMgr = SoundMgr.Instance();
        if (soundMgr == null) return;
        soundMgr.PlaySound(SoundConfig.Channel.BGM, SoundConfig.AudioClipType.Aud_2D, bgmkey, null);
    }
    static public void PlayVoice(SoundConfig.Channel channel, string sekey)
    {
        SoundMgr soundMgr = SoundMgr.Instance();
        if (soundMgr == null) return;
        soundMgr.PlaySound(channel, SoundConfig.AudioClipType.Aud_2D, sekey, null);
    }
    static void PlaySE(GameObject gameObj, string sekey, SoundConfig.Channel channel = SoundConfig.Channel.MenuSE, SoundConfig.AudioClipType clipType= SoundConfig.AudioClipType.Aud_2D)
    {
        SoundMgr soundMgr = SoundMgr.Instance();
        if (soundMgr == null) return;
        soundMgr.PlaySound(channel, clipType, sekey, gameObj);
    }

    static public void PlayMenuSE(string sekey)
    {
        PlaySound.PlaySE(null, sekey, SoundConfig.Channel.MenuSE, SoundConfig.AudioClipType.Aud_2D);
    }
    static public void PlayGameSE(GameObject gameObj, string sekey)
    {
        PlaySound.PlaySE(gameObj, sekey, SoundConfig.Channel.GameSE, SoundConfig.AudioClipType.Aud_3D);
    }
}
