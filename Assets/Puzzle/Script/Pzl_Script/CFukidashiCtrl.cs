using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFukidashiCtrl : MonoBehaviour
{
    [SerializeField] private CFukidashi[] m_FukidashiList;

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = Vector3.zero;
        for(int cnt=0;cnt<m_FukidashiList.Length;cnt++)
        {
            if (cnt == 0)
            {
                m_FukidashiList[cnt].SetActive(true);
            }
            else
            {
                m_FukidashiList[cnt].SetActive(false);
            }
        }
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
