using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 参考
/// http://docs.unity3d.com/ScriptReference/Application.RegisterLogCallback.html
/// </summary>
public class ExceptionManager : MonoBehaviour
{
    public bool m_View = true;
    string _condition = "";
    string _stackTrace = "";
    string _type = "";

    /// <summary>
    /// イベントの発生を有効にします
    /// </summary>
    void OnEnable()
    {
        Application.RegisterLogCallback(HandleLog);
    }

    /// <summary>
    /// イベントの発生を無効にします
    /// </summary>
    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    /// <summary>
    /// Handles the log.
    /// </summary>
    /// <param name="condition">Condition.</param>
    /// <param name="stackTrace">Stack trace.</param>
    /// <param name="type">Type.</param>
    void HandleLog(string condition, string stackTrace, LogType type)
    {
        _condition = condition;
        _stackTrace = stackTrace;
        _type = type.ToString();

        if(type == LogType.Exception)
        {
            string str = "condition : " + _condition + "\nstackTrace : " + _stackTrace + "\ntype : " + _type;
            Debug.AssertFormat(true, str);
        }
    }

    /// <summary>
    /// Raises the GU event.
    /// </summary>
    void OnGUI()
    {
        if (!m_View)
        {
            return;
        }
        GUILayout.Label("condition : " + _condition + "\nstackTrace : " + _stackTrace + "\ntype : " + _type);
    }
}

