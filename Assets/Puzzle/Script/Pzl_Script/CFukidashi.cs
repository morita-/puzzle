using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFukidashi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool isActive()
    {
        return transform.gameObject.activeSelf;
    }
    public void SetActive(bool enable)
    {
        transform.gameObject.SetActive(enable);
    }
}
