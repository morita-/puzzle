using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPzlGameOver : MonoBehaviour
{
    [SerializeField] private GameObject m_GameOverMenuObj;
    [SerializeField] private CMonney m_GameOverMonney;
    [SerializeField] private Button m_ContinueBtn;
    [SerializeField] private GameObject[] m_CharImg;

    CPzlGameFlow.enGameFlow m_Next;

    // Start is called before the first frame update
    void Start()
    {
        m_Next = CPzlGameFlow.enGameFlow.Null;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_GameOverMonney.IsSellItem() == true)
        {
            //買える
            m_ContinueBtn.interactable = true;
        }
        else
        {
            //買えない
            m_ContinueBtn.interactable = false;
        }
    }

    public void SetActivateMenu(bool enable)
    {
        m_GameOverMenuObj.SetActive(enable);
    }

    public void InitGameOver(int nowMonney)
    {
        m_Next = CPzlGameFlow.enGameFlow.Null;
        SetNowMonney(nowMonney);
        m_GameOverMonney.SetStoreMonney(GlobalObjectPzl.m_GlobalParam.CONTINUE_MASEKI_PRICE, 0);
        int idx=(int)Random.Range(0, m_CharImg.Length);
        if (idx < 0 || idx >= m_CharImg.Length) {
            idx = 0;
        }
        for (int i = 0; i < m_CharImg.Length; i++)
        {
            if (i == idx)
            {
                m_CharImg[i].SetActive(true);
            }
            else
            {
                m_CharImg[i].SetActive(false);
            }
        }
    }
    public CPzlGameFlow.enGameFlow UpdateGameOver()
    {
        if (m_Next != CPzlGameFlow.enGameFlow.Null)
        {
            return m_Next;
        }
        return CPzlGameFlow.enGameFlow.Null;
    }

    public void SetNowMonney(int monney)
    {
        m_GameOverMonney.m_Monney = monney;
    }
    public int GetGameOverNowMonney()
    {
        return m_GameOverMonney.m_Monney;
    }

    public void OnRestartClick()
    {
        m_Next = CPzlGameFlow.enGameFlow.GameOverRestart;
    }
    public void OnContinueClick()
    {
        m_Next = CPzlGameFlow.enGameFlow.GameOverContinue;

    }
}
