using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public GameObject m_traceObj = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (m_traceObj == null)
        {
            return;
        }

        this.gameObject.transform.position = m_traceObj.transform.position;
    }
}
