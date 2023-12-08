using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class SoundConfig 
{
    public enum Channel
    {
        MenuSE = -20,
        GameSE = -10,
        Invalid=-1,

        BGM = 0 ,

        CharVoice1,
        CharVoice2,

        MenuSE_00,
        MenuSE_01,
        MenuSE_02,
        MenuSE_03,
        MenuSE_04,
        MenuSE_05,
        MenuSE_06,
        MenuSE_07,

        GameSE_00,
        GameSE_01,
        GameSE_02,
        GameSE_03,
        GameSE_04,
        GameSE_05,
        GameSE_06,
        GameSE_07,
        MAX_Ch,

        MENU_SE_START = MenuSE_00,
        MENU_SE_END = MenuSE_07,

        SE_START = GameSE_00,
        SE_END= GameSE_07,

        VOICE_START = CharVoice1,
        VOICE_END = CharVoice2,
    }
    public enum AudioClipType
    {
        Aud_2D,
        Aud_3D,
        Aud_3D_Static,

    }
}
