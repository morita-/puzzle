using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPzlMapGame : MonoBehaviour
{
    [SerializeField] private GameObject m_MapCanvasObj;
    [SerializeField] private GameObject m_PanelObj;
    [SerializeField] private GameObject m_ParentObj;
    [SerializeField] private GameObject m_AllClearObj;
    [SerializeField] private CMonney m_TipMonney;

    struct MapTipData
    {
        public string name;
        public GameObject tips;
        public CPzlMapTip mapTip;
        public Pzl_StageParamsEntity entity;

        public void Init()
        {
            name = "";
            tips = null;
            mapTip = null;
            entity = null;
        }
        public void SetData(string n_name,Pzl_StageParamsEntity n_entity,GameObject n_tips)
        {
            name = n_name;
            tips = n_tips;
            mapTip = n_tips.GetComponent<CPzlMapTip>();
            entity = n_entity;
        }
    }

    List<MapTipData> m_MapTipBaseList;
    List<MapTipData> m_MapTipList;

    MapTipData m_NowMapTip;
    MapTipData m_PrevMapTip;

    bool m_bEndCheck = false;
    bool m_bMenuEnd=false;
    bool m_bEnding = false;
    float m_clear_alpha = 0.0f;

    enum enMapClick
    {
        Normal,
        Wide,
    }
    enMapClick m_MapClickState = enMapClick.Normal;
    float m_base_panel_scale = 1.0f;

    MapTipData GetMapTipBaseData(string name)
    {
        for(int cnt = 0; cnt < m_MapTipBaseList.Count; cnt++)
        {
            if(m_MapTipBaseList[cnt].name == name)
            {
                return m_MapTipBaseList[cnt];
            }
        }
        MapTipData mapTipData=new MapTipData();
        mapTipData.Init();
        return mapTipData;
    }
    MapTipData GetMapTipData(int stageNo)
    {
        for (int cnt = 0; cnt < m_MapTipList.Count; cnt++)
        {
            if (m_MapTipList[cnt].entity.StageNo == stageNo)
            {
                return m_MapTipList[cnt];
            }
        }
        MapTipData mapTipData = new MapTipData();
        mapTipData.Init();
        return mapTipData;
    }

    private void Awake()
    {
        m_MapTipBaseList = new List<MapTipData>();
        m_MapTipList = new List<MapTipData>();

        m_NowMapTip = new MapTipData();
        m_NowMapTip.Init();
        m_PrevMapTip = new MapTipData();
        m_PrevMapTip.Init();
        //
        m_base_panel_scale = m_PanelObj.transform.localScale.x;

    }

    // Start is called before the first frame update
    void Start()
    {
        m_bEndCheck = false;
        m_bMenuEnd = false;
        m_bEnding = false;
        m_clear_alpha = 0.0f;
        m_MapClickState = enMapClick.Normal;
    }

    // Update is called once per frame
    void Update()
    {

        if (m_bEnding == true)
        {
            Vector3 pos = m_AllClearObj.transform.localPosition;
            if (pos.x > 0)
            {
                pos.x -= GlobalObjectPzl.m_GlobalParam.ENDING_CLEAR_FLOW_SPD;
                if (pos.x < 0)
                {
                    pos.x = 0;
                }
            }
            if (pos.x < 0)
            {
                pos.x += GlobalObjectPzl.m_GlobalParam.ENDING_CLEAR_FLOW_SPD;
                if (pos.x > 0)
                {
                    pos.x = 0;
                }
            }
            if (pos.y > 0)
            {
                pos.y -= GlobalObjectPzl.m_GlobalParam.ENDING_CLEAR_FLOW_SPD;
                if(pos.y < 0)
                {
                    pos.y = 0;
                }
            }
            if (pos.y < 0)
            {
                pos.y += GlobalObjectPzl.m_GlobalParam.ENDING_CLEAR_FLOW_SPD;
                if(pos.y > 0)
                {
                    pos.y = 0;
                }
            }
            m_AllClearObj.transform.localPosition = pos;

            m_clear_alpha -= GlobalObjectPzl.m_GlobalParam.ENDING_CEAR_ALPA_SPD;
            SetAllObjectTipAlpha(m_clear_alpha);


        }
        else
        {
            //通常はパネル位置を調整
            if (m_NowMapTip.tips != null)
            {
                Vector3 nowPos = m_PanelObj.transform.localPosition;
                Vector3 nowScl = m_PanelObj.transform.localScale;
                Vector3 mapTipPos = Vector3.zero;
                float target_scale = 1.0f;
                switch (m_MapClickState)
                {
                    case enMapClick.Normal:
                        mapTipPos = -m_NowMapTip.tips.transform.localPosition;
                        target_scale = m_base_panel_scale;
                        break;
                    default: break;
                }

                bool bScaleCtrl = false;
                float now_scale = m_PanelObj.transform.localScale.x;
                if (now_scale < target_scale)
                {
                    //now_scale += (target_scale - now_scale) / 10.0f;
                    //now_scale += 0.2f;
                    now_scale = target_scale;
                    if (now_scale > target_scale)
                    {
                        now_scale = target_scale;
                    }
                    bScaleCtrl = true;

                }
                if (now_scale > target_scale)
                {
                    now_scale -= (now_scale - target_scale) / 10.0f;
                    //now_scale -= 0.2f;
                    if (now_scale < target_scale)
                    {
                        now_scale = target_scale;
                    }
                    bScaleCtrl = true;
                }
                Vector3 localScl = m_PanelObj.transform.localScale;
                localScl.x = now_scale;
                localScl.y = now_scale;
                m_PanelObj.transform.localScale = localScl;


                //Vector3 mapTipNowPos = m_NowMapTip.tips.transform.position;
                //RectTransform rectTrans = m_PanelObj.GetComponent<RectTransform>();
                //float offs_x = (rectTrans.rect.width* nowScl.x) /2;
                //float offs_y = (rectTrans.rect.height* nowScl.y) / 2;

                mapTipPos.x *= now_scale;
                mapTipPos.y *= now_scale;
                float max_x = 550;
                float min_x = -550;
                float max_y = 600;
                float min_y = -600;
                if ((mapTipPos.x-nowPos.x) > 0)
                {
                    nowPos.x += Mathf.Abs(mapTipPos.x - nowPos.x) / 10;
                }
                if ((mapTipPos.x-nowPos.x) < 0)
                {
                    nowPos.x -= Mathf.Abs(mapTipPos.x - nowPos.x) / 10;
                }
                if ((mapTipPos.y - nowPos.y) > 0)
                {
                    nowPos.y += Mathf.Abs(mapTipPos.y - nowPos.y) / 10;
                }
                if ((mapTipPos.y - nowPos.y) < 0)
                {
                    nowPos.y -= Mathf.Abs(mapTipPos.y - nowPos.y) / 10;
                }
                if (nowPos.x > max_x)
                {
                    nowPos.x = max_x;
                }
                if (nowPos.x < min_x)
                {
                    nowPos.x = min_x;
                }

                if (nowPos.y > max_y)
                {
                    nowPos.y = max_y;
                }
                if (nowPos.y < min_y)
                {
                    nowPos.y = min_y;
                }


                m_PanelObj.transform.localPosition = nowPos;



            }
        }
    }
    
    void SetAllObjectTipAlpha(float alpha)
    {
        for(int cnt = 0; cnt < m_MapTipList.Count; cnt++)
        {
            m_MapTipList[cnt].mapTip.SetImageAlpha(alpha);
        }
    }

    public void CanvasClick()
    {
        if (m_TipMonney.IsAdvertizeWait() == true)
        {
            return;//Advetizeでクリックされた直後なので、このクリックは無視する
        }
        switch (m_MapClickState)
        {
            case enMapClick.Normal:
                m_MapClickState = enMapClick.Wide;
                break;
            case enMapClick.Wide:
                m_MapClickState = enMapClick.Normal;
                break;
            default:break;
        }
    }

    public void CanvasNextPR_Click()
    {
        if (m_TipMonney.IsSellItem() == true)
        {
            m_bMenuEnd = true;
        }
        else
        {
            m_bEndCheck = m_TipMonney.OnAdvertizeStart();
        }
    }
    
    public bool IsCheck(bool bAdPause)
    {
        if (m_bEndCheck == true && bAdPause == false)
        {
            m_bEndCheck = false;

            //Adポーズ明けに再チェック
            if (m_TipMonney.IsSellItem() == true)
            {
                m_bMenuEnd = true;
            }

        }
        bool bNowAnimEnd = true;
        if (m_NowMapTip.mapTip != null)
        {
            bNowAnimEnd = m_NowMapTip.mapTip.IsAnimationEnd();
        }
        bool bPrevAnimEnd = true;
        if(m_PrevMapTip.mapTip != null)
        {
            bPrevAnimEnd = m_PrevMapTip.mapTip.IsAnimationEnd();
        }
        if (bNowAnimEnd == true && bPrevAnimEnd == true && m_TipMonney.IsSellItem()==true && m_bMenuEnd==true)
        {
            m_TipMonney.SellItem();
            return true;
        }
        return false;
    }

    /// <summary>
    /// マップ中に参照するお金を設定
    /// </summary>
    /// <param name="now_monney"></param>
    public void SetMapNowMonney(int now_monney)
    {
        m_TipMonney.m_Monney = now_monney;
    }
    public int GetMapNowMonney()
    {
        return m_TipMonney.m_Monney;
    }

    /// <summary>
    /// アクティブメニューにする設定
    /// </summary>
    /// <param name="enable"></param>
    public void SetActivateMenu(bool enable)
    {
        m_MapCanvasObj.SetActive(enable);
    }

    public void InitMapTipBaseData()
    {
        MapTipData mapData_n = TipBaseLoad("Normal");
        if (mapData_n.tips != null)
        {
            m_MapTipBaseList.Add(mapData_n);
        }

        MapTipData mapData_e = TipBaseLoad("Event");
        if (mapData_e.tips != null)
        {
            m_MapTipBaseList.Add(mapData_e);
        }

        MapTipData mapData_d = TipBaseLoad("End");
        if (mapData_d.tips != null)
        {
            m_MapTipBaseList.Add(mapData_d);
        }

    }

    private MapTipData TipBaseLoad(string s_name)
    {
        // プレハブを取得
        string tip_file_name = string.Format("_Prefab/Pzl_Book/Books_{0}", s_name);
        GameObject prefab = (GameObject)Resources.Load(tip_file_name);
        MapTipData mapData = new MapTipData();

        if (prefab != null)
        {
            mapData.SetData(s_name, null, prefab);
        }
        else
        {
            mapData.Init();
        }

        return mapData;
    }

    public void SetNextStage(int stageNo, int prev_stageNo)
    {
        MapTipData mapTip = GetMapTipData(stageNo);
        if (mapTip.tips != null)
        {
            mapTip.mapTip.StageStart();

            m_TipMonney.SetStoreMonney(mapTip.entity.needCash, 0);
            m_TipMonney.SetStorePos(mapTip.entity.posX, mapTip.entity.posY - 30);
        }
        else
        {
            m_TipMonney.SetStoreMonney(0, 0);
        }
        m_NowMapTip = mapTip;


        MapTipData mapTip_prev = GetMapTipData(prev_stageNo);
        if (mapTip_prev.tips != null)
        {
            mapTip_prev.mapTip.SetOpen();
        }
        m_PrevMapTip = mapTip_prev;

        m_MapClickState = enMapClick.Normal;
        m_bMenuEnd = false;
    }

    public void SetAllStegeMapTips(List<Pzl_StageParamsEntity> entityList)
    {
        for(int cnt = 0; cnt < entityList.Count; cnt++)
        {
            MapTipData mapBaseData =  GetMapTipBaseData(entityList[cnt].booktype);
            GameObject gameObj = Instantiate(mapBaseData.tips, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, m_ParentObj.transform);
            if (gameObj != null)
            {
                CPzlMapTip mapTip = gameObj.GetComponentInChildren<CPzlMapTip>();
                mapTip.SetPositionScale(entityList[cnt].posX, entityList[cnt].posY, entityList[cnt].scale);


                MapTipData mapTipData = new MapTipData();
                mapTipData.SetData(mapBaseData.name, entityList[cnt], gameObj);
                m_MapTipList.Add(mapTipData);

            }

        }
    }

    public void SetStartEnding()
    {
        m_bEnding = true;
        m_clear_alpha = 0.0f;
    }
}
