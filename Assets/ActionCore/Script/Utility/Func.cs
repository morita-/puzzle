using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorIdx
{
    none = -1,
    black,
    blue,
    red,
    magenda,
    green,
    yellow,
    cyan,
    white,
    blue_hf,
    red_hf,
    magenda_hf,
    green_hf,
    yellow_hf,
    cyan_hf,
    white_hf,
}
namespace UtilityFunction
{

    class Func
    {
        public enum ActionLayer
        {
            normal=0,
            char_ai,
            char_ctrl,
            char_act,
            hud,
            hud_later,
            hud_ctrl,
            camera_act,
            camera_col,
            game_man,
            max
        }
        static public float FLT_EPSILON=0.00000000001f;
        static public float DegAngNormalize(float ang)
        {
            while (ang > 180)
            {
                ang -= 360;
            }
            while (ang < -180)
            {
                ang += 360;
            }

//            ang %= 180;
            //            ang += 180;
            //            ang = ang % 360;
            //            ang -= 180;
            return ang;
        }

        static public float RadAngNormalize(float ang)
        {
            while(ang> Mathf.PI)
            {
                ang -= Mathf.PI * 2;
            }
            while (ang < -Mathf.PI)
            {
                ang += Mathf.PI * 2;
            }

//            ang %= Mathf.PI;
            //            ang += Mathf.PI;
            //            ang = ang % (Mathf.PI*2);
            //            ang -= Mathf.PI;
            return ang;
        }

        static public Vector3 RotateAngleX(Vector3 vec, float angle_x)
        {
            Vector3 ret = Quaternion.AngleAxis(Mathf.Rad2Deg * angle_x, Vector3.right) * vec;
            return ret;
        }
        static public Vector3 RotateAngleY(Vector3 vec,float angle_y)
        {
            Vector3 ret = Quaternion.AngleAxis(Mathf.Rad2Deg*angle_y, Vector3.up) * vec;
            return ret;
        }
        static public Vector3 RotateAngleZ(Vector3 vec,float angle_z)
        {
            Vector3 ret = Quaternion.AngleAxis(Mathf.Rad2Deg * angle_z, Vector3.forward) * vec;
            return ret;
        }

        static public void Log(string format,params System.Object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }


        //
        public static AnimationClip[] s_CatchAnim;
        public static void AnimationLoadAll()
        {
            s_CatchAnim = Resources.LoadAll<AnimationClip>("Animation/Catch");

        }
        public static AnimationClip GetCatchAnimationClip(string clipName)
        {
            AnimationClip originalClip = System.Array.Find<AnimationClip>(s_CatchAnim, item => item is AnimationClip && item.name == clipName);
            return originalClip;
        }


        static public Color GetColorFromColorIdx(ColorIdx colorIdx)
        {
            switch (colorIdx)
            {
                case ColorIdx.black:
                    return Color.black;
                    break;
                case ColorIdx.blue:
                    return Color.blue;
                    break;
                case ColorIdx.cyan:
                    return Color.cyan;
                    break;
                case ColorIdx.green:
                    return Color.green;
                    break;
                case ColorIdx.magenda:
                    return Color.magenta;
                    break;
                case ColorIdx.red:
                    return Color.red;
                    break;
                case ColorIdx.white:
                    return Color.white;
                    break;
                case ColorIdx.yellow:
                    return Color.yellow;
                    break;
                case ColorIdx.blue_hf:
                    return Color.blue/2;
                    break;
                case ColorIdx.cyan_hf:
                    return Color.cyan/2;
                    break;
                case ColorIdx.green_hf:
                    return Color.green/2;
                    break;
                case ColorIdx.magenda_hf:
                    return Color.magenta/2;
                    break;
                case ColorIdx.red_hf:
                    return Color.red/2;
                    break;
                case ColorIdx.white_hf:
                    return Color.white/2;
                    break;
                case ColorIdx.yellow_hf:
                    return Color.yellow/2;
                    break;
                default: break;
            }
            return Color.gray;
        }

        static public float GetScreenRate()
        {
            return Screen.width/750.0f;
        }


    }
}
