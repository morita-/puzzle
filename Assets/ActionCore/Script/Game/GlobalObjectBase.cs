using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObjectBase : MonoBehaviour
{

    static public GlobalObjectBase singleton=null;

    public bool m_bDontDestoryOnLoad = true;


    protected void Awake()
    {
        singleton = this;

        if (m_bDontDestoryOnLoad == true)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    // Use this for initialization
    protected void Start()
    {
    }


}
