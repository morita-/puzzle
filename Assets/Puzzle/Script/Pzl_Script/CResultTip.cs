using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CResultTip : MonoBehaviour
{
    [System.Serializable]
    public struct stDelayObj
    {
        public GameObject obj;
        public Vector3 pos;
    }
    [SerializeField] private HUDTextMeshPro m_Text;
    [SerializeField] private Image m_BGImg;
    [SerializeField] private Image m_FrontImg;
    [SerializeField] private stDelayObj[] m_DelayObj;
    [SerializeField] private CFukidashiCtrl m_Fukidashi;
    [SerializeField] private CPzlTips[] m_CompTips;

    string m_ResultName;
    int m_CountMax;
    int m_Count;
    bool m_bDeliver=false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetResultData(string name,int count)
    {
        m_ResultName = name;
        m_CountMax = count;
        m_Count = count;
        m_Text.SetText(string.Format("x{0}", m_Count));


        Color col = m_FrontImg.color;
        col.a = 0.0f;
        m_FrontImg.color = col;

    }

    public bool DecResultData()
    {
        if (m_Count <= 0)
        {
            return false;
        }
        m_Count--;
        m_Text.SetText(string.Format("x{0}", m_Count));

        if (m_Count <= 0)
        {
            Color col = m_FrontImg.color;
            col.a = 0.5f;
            m_FrontImg.color = col;
        }
        return true;
    }
    public int GetRestCount()
    {
        return m_Count;
    }

    public void SetDeliverData()
    {
        m_bDeliver = true;
        m_Text.SetActivate(false);
        m_BGImg.transform.gameObject.SetActive(false);

        for(int ii=0;ii< m_DelayObj.Length; ii++)
        {
            m_DelayObj[ii].obj.SetActive(true);
        }
    }
    public void SetDeliverLocalPosition(Vector3 pos)
    {
        if (m_bDeliver)
        {
            for(int ii = m_DelayObj.Length - 2; ii >=0 ; ii--)
            {
                m_DelayObj[ii+1].pos = m_DelayObj[ii].pos;
            }
            if (m_DelayObj.Length > 0)
            {
                m_DelayObj[0].pos = transform.position;
            }
            //
            transform.localPosition = pos;
            for(int ii = 0; ii < m_DelayObj.Length; ii++)
            {
                m_DelayObj[ii].obj.transform.position = m_DelayObj[ii].pos;
            }
        }
        else
        {
            transform.localPosition = pos;
        }
    }
    public void OnClick()
    {
        if (m_Fukidashi.isActive() == true)
        {
            m_Fukidashi.SetActive(false);
        }
        else
        {
            m_Fukidashi.SetActive(true);
        }
    }
    public void OnEnterDown()
    {
        if (m_Fukidashi.isActive() == false)
        {
            m_Fukidashi.SetActive(true);
        }
    }
    public void OnExitUp()
    {
        if (m_Fukidashi.isActive() == true)
        {
            m_Fukidashi.SetActive(false);
        }
    }
}
