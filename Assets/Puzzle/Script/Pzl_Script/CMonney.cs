using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class CMonney : MonoBehaviour
{
    [SerializeField] private HUDTextMeshPro m_MonneyText;
    [SerializeField] private HUDTextMeshPro m_StoreText;
    [SerializeField] private Image m_StoreBackImg;
    [SerializeField] private CPzlGameFlow m_PzlGameFlow;
    [SerializeField] private GameObject m_PR_ImgObj;
    [SerializeField] private Image m_NextImg;
    [SerializeField] private bool m_bNEXT;

    public int m_Monney = 0;
    private int m_DispMonney = 0;
    private int m_StoreCost;
    private float m_StoreCostPer;
    private int m_needPoint;
    private float m_TickCnt=0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (m_TickCnt > 0)
        {
            m_TickCnt -= Time.deltaTime;
            if (m_TickCnt < 0)
            {
                m_TickCnt = 0;
            }
        }
        bool bStoreActive = m_StoreText.transform.gameObject.activeSelf;
        int nowStoreCost = GetNowSoreCost();
        m_StoreText.SetText(GetCostName(nowStoreCost));

        if (nowStoreCost > 0)
        {

            if (bStoreActive == false)
            {
                //無効化されている場合
                m_StoreText.SetActivate(true);//有効化する
            }

            //有効状態
            if (IsSellItem() == false)
            {
                //買えない
                m_StoreBackImg.color = Color.black;
                m_StoreText.SetTextColor(Color.gray);
                if (m_bNEXT)
                {
                    m_PR_ImgObj.SetActive(true);
                }
            }
            else
            {
                //買える
                m_StoreBackImg.color = Color.white;

                if (m_StoreCostPer > 0.1f)
                {
                    //コスト高
                    m_StoreText.SetTextColor(Color.blue);

                }
                else if (m_StoreCostPer < -0.1f)
                {
                    //安売り
                    m_StoreText.SetTextColor(Color.red);
                }
                else
                {
                    //通常売り
                    m_StoreText.SetTextColor(Color.black);
                }

                if (m_bNEXT)
                {
                    m_PR_ImgObj.SetActive(false);
                }
            }
        }
        else
        {
            if (bStoreActive == true)
            {
                //有効化されている場合
                m_StoreText.SetActivate(false);//無効化する
            }
        }

        //持ち金の表示
        if (m_DispMonney != m_Monney)
        {
            if (m_DispMonney < m_Monney)
            {
                PlaySound.PlayMenuSE("common_add_money");
            }
            else
            {

            }
            if (Mathf.Abs(m_DispMonney - m_Monney) < 10)
            {
                if (m_Monney > m_DispMonney) m_DispMonney++;
                if (m_Monney < m_DispMonney) m_DispMonney--;
            }
            else
            {
                if (m_Monney > m_DispMonney) m_DispMonney += (m_Monney - m_DispMonney) / 2;
                if (m_Monney < m_DispMonney) m_DispMonney -= (m_DispMonney - m_Monney) / 2;
            }
        }
        m_MonneyText.SetText(GetCostName(m_DispMonney));

    }

    public bool IsSellItem()
    {
        if (m_Monney >= GetNowSoreCost())
        {
            return true;
        }
        return false;
    }
    public bool IsAdvertizeWait()
    {
        if (m_TickCnt > 0)
        {
            return true;
        }
        return false;
    }
    public void SellItem()
    {
        m_Monney -= GetNowSoreCost();
        m_StoreCost = 0;
    }
    public int GetNowSoreCost()
    {
        return (int)(m_StoreCost + (m_StoreCost * m_StoreCostPer));
    }
    public void AddMonney(int monney)
    {
        m_Monney += monney;
    }
    public string GetCostName(int monney)
    {
        if (monney >= 1000000)
        {
            return string.Format("{0},{1:000},{2:000}ﾘｵﾝ", (int)(monney / 100000), (int)(monney / 1000) % 1000, (int)(monney) % 1000);
        }
        if (monney >= 1000)
        {
            return string.Format("{0},{1:000}ﾘｵﾝ", (int)(monney / 1000) % 1000, (int)(monney) % 1000);
        }
        return string.Format("{0}ﾘｵﾝ", monney);
    }
    public void SetStoreCost(string tip_name, int storeCost, int needPoint)
    {
        needPoint = 0;
        m_needPoint = needPoint;
        m_StoreCostPer = GetSellCost(needPoint);
        m_StoreCost = storeCost;
        ClearNextImg();
    }
    public void SetStoreMonney(int storeMoney,int needPoint)
    {
        m_needPoint = needPoint;
        m_StoreCostPer = GetSellCost(needPoint);
        m_StoreCost = storeMoney;
        ClearNextImg();
    }

    void ClearNextImg()
    {
        if (m_NextImg != null)
        {
            m_NextImg.color = Color.white;
        }
    }


    public void SetStorePos(float pos_x, float pos_y)
    {
        if (m_StoreText != null)
        {
            Vector3 pos = m_StoreText.transform.localPosition;
            pos.x = pos_x;
            pos.y = pos_y;
            m_StoreText.transform.localPosition = pos;
        }
    }

    
    //商品の割引、割増量のパーセンテージ評価
    private float GetSellCost(int needPoint)
    {
        if (needPoint > GlobalObjectPzl.m_GlobalParam.NEED_POINT_UPPER_COST)
        {
            return (needPoint / GlobalObjectPzl.m_GlobalParam.NEED_POINT_UPPER_COST);
        }
        else if (needPoint > GlobalObjectPzl.m_GlobalParam.NEED_POINT_DISCOUNT_COST)
        {
            return -0.8f * (needPoint / GlobalObjectPzl.m_GlobalParam.NEED_POINT_DISCOUNT_COST);
        }
        return 0.0f;
    }

    public void OnNextImg_Enter()
    {
        if (m_NextImg!=null)
        {
            Color col = m_NextImg.color;
            col.r = col.g = col.b = 0.8f;
            m_NextImg.color = col;
            
        }
    }
    public void OnNextImg_Exit()
    {
        if (m_NextImg!=null)
        {
            m_NextImg.color = Color.white;

        }
    }
    public void OnPointerClick()
    {
        OnAdvertizeStart();
    }
    public bool OnAdvertizeStart()
    {
        PlaySound.PlayMenuSE("common_pr_push");
        if (Advertisement.IsReady())
        {
            var options = new ShowOptions { gamerSid= "example", resultCallback = HandleShowResult };
            //Advertisement.Show("", options);
/*            
            if (GlobalObjectPzl.GetInstance().m_DebugParam.ADVERTIZE_TEST_MODE == true)
            {
                options.gamerSid = "";
                Advertisement.Show("", options);
            }
            else
            {
            }
            */
            Advertisement.Show("rewardedVideo", options);
            m_PzlGameFlow.SetGameAdvertizePause(true);
            return true;
        }
        return false;
    }
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                //Debug.Log("The ad was successfully shown.");
                AddMonney(GlobalObjectPzl.m_GlobalParam.GET_ADS_MONNEY);
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.Log("The ad failed to be shown.");
                break;
        }
        m_TickCnt = GlobalObjectPzl.m_GlobalParam.ADVERTIZE_TICK_WAIT;
        m_PzlGameFlow.SetGameAdvertizePause(false);

    }
}
