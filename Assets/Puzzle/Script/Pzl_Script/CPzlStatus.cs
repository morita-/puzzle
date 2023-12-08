using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPzlStatus : MonoBehaviour
{
    //ステージのデータ設定を保持
    [SerializeField] private GameObject m_ParentObj;
    [SerializeField] private GameObject m_ParentResultObj;
    [SerializeField] private GameObject m_ParentEffectObj;
    [SerializeField] private GameObject m_NextObj;
    [SerializeField] private GameObject m_NextPosObj;
    [SerializeField] private GameObject m_StoreObj;
    [SerializeField] private CTipCareer m_TipCareer;
    [SerializeField] private CTipGauge  m_TipGauge;
    [SerializeField] private CMonney    m_TipMonney;
    [SerializeField] private GameObject m_ResultPosObj;
    [SerializeField] private GameObject m_ResultGeneralPosObj;
    [SerializeField] private Canvas m_GameCanvas;
    [SerializeField] private HUDTextMeshPro m_StageText;
    [SerializeField] private Image m_StageClear;

    //チップ情報
    private List<Pzl_TipsParamsTipsEntity> m_TipsList;
    private List<Pzl_TipsParamsCompTipsEntity> m_TipsCompList;

    //ステージパラメータ情報
    private Pzl_StageParamsEntity m_StageParam=null;

    //チップの実データ
    public struct TipsData{
        public string name;
        public Pzl_TipsParamsTipsEntity tipsEntity;
        public GameObject tips;
        public CPzlTips pzlTips;

        public void Init()
        {
            name = null;
            tipsEntity = null;
            tips = null;
        }
        public void SetData(string tip_name,Pzl_TipsParamsTipsEntity entity,GameObject gameObj)
        {
            name = tip_name;
            tipsEntity = entity;
            tips = gameObj;
            pzlTips = gameObj.GetComponentInChildren<CPzlTips>();
        }
    }
    private List<TipsData> m_TipsBaseData;　//プレハブのベースデータ
    private List<TipsData> m_AreaTips;  //エリアにあるチップ
    private List<TipsData> m_NextTips;  //次のエリアにあるチップ
    private List<TipsData> m_StoreTips; //ストアリアにあるチップ
    private TipsData m_CareerTips; //キャリアされるチップ

    //

    //結果チェック用のデータ

    private struct ResultTip
    {
        public string name;
        public Pzl_TipsParamsCompTipsEntity compTipsEntitiy;
        public GameObject tips;
        public CResultTip resTips;
        public int maxCount;

        public void Init()
        {
            name = null;
            compTipsEntitiy = null;
            tips = null;
            maxCount = 0;
        }
        public void SetData(string tip_name,Pzl_TipsParamsCompTipsEntity entity,GameObject gameObj,int nCnt)
        {
            name = tip_name;
            compTipsEntitiy = entity;
            tips = gameObj;
            resTips = gameObj.GetComponentInChildren<CResultTip>();
            maxCount = nCnt;
        }
        public string GetBaseTipsName(int baseNo)
        {
            switch (baseNo)
            {
                case 0: return compTipsEntitiy.BaseTip0; break;
                case 1: return compTipsEntitiy.BaseTip1; break;
                case 2: return compTipsEntitiy.BaseTip2; break;
                case 3: return compTipsEntitiy.BaseTip3; break;
                case 4: return compTipsEntitiy.BaseTip4; break;
                case 5: return compTipsEntitiy.BaseTip5; break;
                case 6: return compTipsEntitiy.BaseTip6; break;
                case 7: return compTipsEntitiy.BaseTip7; break;
                case 8: return compTipsEntitiy.BaseTip8; break;
                case 9: return compTipsEntitiy.BaseTip9; break;
                case 10: return compTipsEntitiy.BaseTip10; break;
                default:break;
            }
            return null;
        }
    }
    private List<ResultTip> m_ResultBaseData;　//プレハブのベースデータ
    private List<ResultTip> m_ResultList;

    [System.Serializable]
    public struct ResultAnim
    {
        public GameObject obj;
        public Animator anim;
        public GameObject off_obj;

        public void Init()
        {
            obj = null;
            anim = null;
            off_obj = null;
        }
    }
    [SerializeField] List<ResultAnim> m_ResultAnim;

    //スタート完了
    public bool m_bStart = false;
    private int m_StageMonney;
    private GameObject m_CheckTriggerObj;
    private bool m_bCheckTriggerObj;
    private bool m_bCoProcess = false;
    private int  m_nProcessCnt=0;    //子プロセスでアイテム納品中(カウント）
    private bool m_bCheckProcess = false;
    private bool m_bGamePause;  //ゲームポーズ状態
    private bool m_bStoreChange;    //ストア商品変更フラグ
    private float m_StageClearWait=0; //ステージクリア時のウェイト数
    private int m_ComboCount = 0;

    //ボーナスエフェクト
    GameObject m_BonsCoinObjBase;
    GameObject m_BonsBookObjBase;

    //
    public int GetNowMonney()
    {
        return m_TipMonney.m_Monney;
    }
    public void SetNowMonney(int monney)
    {
        m_TipMonney.m_Monney = monney;
    }
    public void SetRestartMonney()
    {
        m_TipMonney.m_Monney = m_StageMonney;
    }

    public GameObject GetNextBoxObj()
    {
        return m_NextObj;
    }

    private void Awake()
    {
        //メンバーの関数のイニシャライズ
        m_TipsBaseData = new List<TipsData>();
        m_AreaTips = new List<TipsData>();
        m_NextTips = new List<TipsData>();
        m_StoreTips = new List<TipsData>();
        m_ResultBaseData = new List<ResultTip>();
        m_ResultList = new List<ResultTip>();
        m_StageMonney = 0;
        m_CheckTriggerObj = null;
        m_bCheckTriggerObj = false;
        m_nProcessCnt = 0;
        m_bCoProcess = false;
        m_bCheckProcess = false;
        m_bGamePause = false;
        m_bStoreChange = false;
    }

    //プレハブベースデータの取得
    private TipsData GetTipsData(string tipName)
    {
        foreach(TipsData tips in m_TipsBaseData)
        {
            if(tipName == tips.name)
            {
                return tips;
            }
        }

        TipsData null_tips=new TipsData();
        null_tips.Init();
        return null_tips;
    }
    //
    string GetResultTipName(string tipName,string tipDetailName)
    {
        string tip_name = tipName;
        if (tipDetailName != "")
        {
            tip_name = tip_name + "_" + tipDetailName;
        }
        return tip_name;
    }

    private ResultTip GetResultTipsData(string tipName)
    {
        foreach (ResultTip tips in m_ResultBaseData)
        {
            if (tipName == tips.name)
            {
                return tips;
            }
        }

        ResultTip null_tips = new ResultTip();
        null_tips.Init();
        return null_tips;
    }
    private ResultTip GetNowResultTipsData(string tipName)
    {
        foreach (ResultTip tips in m_ResultList)
        {
            switch (tips.compTipsEntitiy.ResultData)
            {
                case "Variation":
                    {
                        ResultTip resTip = tips;

                        string[] tipData = new string[11];
                        tipData[0] = resTip.compTipsEntitiy.BaseTip0;
                        tipData[1] = resTip.compTipsEntitiy.BaseTip1;
                        tipData[2] = resTip.compTipsEntitiy.BaseTip2;
                        tipData[3] = resTip.compTipsEntitiy.BaseTip3;
                        tipData[4] = resTip.compTipsEntitiy.BaseTip4;
                        tipData[5] = resTip.compTipsEntitiy.BaseTip5;
                        tipData[6] = resTip.compTipsEntitiy.BaseTip6;
                        tipData[7] = resTip.compTipsEntitiy.BaseTip7;
                        tipData[8] = resTip.compTipsEntitiy.BaseTip8;
                        tipData[9] = resTip.compTipsEntitiy.BaseTip9;
                        tipData[10] = resTip.compTipsEntitiy.BaseTip10;

                        for (int ii=0;ii< tips.compTipsEntitiy.BaseTipNum; ii++)
                        {
                            if (tipName == GetResultTipName(tips.name, tipData[ii]))
                            {
                                return tips;
                            }
                        }
                    }
                    break;
                case "Tips":
                case "Result":
                default:
                    { 
                        if (tipName == tips.name)
                        {
                            return tips;
                        }
                    }
                    break;
            }
        }

        ResultTip null_tips = new ResultTip();
        null_tips.Init();
        return null_tips;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_TipsList = GlobalObjectPzl.m_TipsParam.Tips;
        m_TipsCompList = GlobalObjectPzl.m_TipsParam.CompTips;

        //プレハブ元データ読み込みと登録
        //TipsData
        foreach (Pzl_TipsParamsTipsEntity tip in m_TipsList)
        {
            // プレハブを取得
            string tip_file_name = string.Format("_Prefab/Pzl_Tip/Tip_{0}_{1}", tip.Category, tip.name);
            GameObject prefab = (GameObject)Resources.Load(tip_file_name);

            if (prefab != null)
            {

                Rigidbody2D regd2d = prefab.GetComponent<Rigidbody2D>();
                regd2d.isKinematic = true;

            }
            else
            {
                print("error load prefab[" + tip_file_name + "]\n");
            }
            //登録
            TipsData tipData = new TipsData();
            tipData.SetData(tip.name, tip, prefab);
            m_TipsBaseData.Add(tipData);
        }

        //ResultData
        foreach(Pzl_TipsParamsCompTipsEntity result in m_TipsCompList)
        {
            switch (result.ResultData)
            {
                case "Tips":
                    {
                        //tipsを生成するケースはgameObjとしてTipsのprefabを取得しておく
                        ResultTip tipTipData = new ResultTip();
                        TipsData tipBaseData = GetTipsData(result.CompResult);

                        tipTipData.SetData(result.CompResult, result, tipBaseData.tips, -1);
                        m_ResultBaseData.Add(tipTipData);
                        continue;
                    }
                    break;
                case "Variation":
                case "Result":
                default:
                    //それ以外は通常
                    break;
            }

            //プレハブを取得
            string res_name = GetResultTipName(result.CompResult,result.ResultDetail);

            string tip_file_name = string.Format("_Prefab/Pzl_Tip/ResultTip_{0}_{1}", result.Category, res_name);
            GameObject prefab = (GameObject)Resources.Load(tip_file_name);

            if (prefab != null)
            {


            }
            else
            {
                print("error load prefab[" + tip_file_name + "]\n");
            }
            //登録
            ResultTip tipData = new ResultTip();
            tipData.SetData(res_name, result, prefab, -1);
            m_ResultBaseData.Add(tipData);

        }


        // ボーナス用プレハブを取得
        {
            string bnscn_file_name = string.Format("_Prefab/Pzl_Effects/BonusCoinEff");
            GameObject bnscnPrefab = (GameObject)Resources.Load(bnscn_file_name);

            if (bnscnPrefab != null)
            {
                m_BonsCoinObjBase = bnscnPrefab;
            }

            string bnsbk_file_name = string.Format("_Prefab/Pzl_Effects/BonusBookEff");
            GameObject bnsbkPrefab = (GameObject)Resources.Load(bnsbk_file_name);

            if (bnsbkPrefab != null)
            {
                m_BonsBookObjBase = bnsbkPrefab;
            }
        }

        m_bStart = true;
    }

    //ステージデータの登録
    public void SetStageEntity(Pzl_StageParamsEntity stg_param)
    {
        m_StageParam = stg_param;

        //CareerData
        m_TipCareer.SetCareerSpeed(m_StageParam.CareerSpeed, m_NextPosObj);

        //Gauge
        m_TipGauge.SetGaugeUpSpeed(m_StageParam.UpSpeed,m_StageParam.UpRate);

        m_StageText.SetText(string.Format("Stage.{0}", m_StageParam.StageNo));
    }
    public void SetDecGauge(float dec_Gauge)
    {
        m_TipGauge.DecGage(dec_Gauge);
    }
    // Update is called once per frame
    void Update()
    {
    }

    //ステージ開始時の関数　※Flowからの呼び出し
    public void InitStage()
    {
        string[] tipData = new string[31];
        tipData[0] = m_StageParam.Start0;
        tipData[1] = m_StageParam.Start1;
        tipData[2] = m_StageParam.Start2;
        tipData[3] = m_StageParam.Start3;
        tipData[4] = m_StageParam.Start4;
        tipData[5] = m_StageParam.Start5;
        tipData[6] = m_StageParam.Start6;
        tipData[7] = m_StageParam.Start7;
        tipData[8] = m_StageParam.Start8;
        tipData[9] = m_StageParam.Start9;
        tipData[10] = m_StageParam.Start10;
        tipData[11] = m_StageParam.Start11;
        tipData[12] = m_StageParam.Start12;
        tipData[13] = m_StageParam.Start13;
        tipData[14] = m_StageParam.Start14;
        tipData[15] = m_StageParam.Start15;
        tipData[16] = m_StageParam.Start16;
        tipData[17] = m_StageParam.Start17;
        tipData[18] = m_StageParam.Start18;
        tipData[19] = m_StageParam.Start19;
        tipData[20] = m_StageParam.Start20;
        tipData[21] = m_StageParam.Start21;
        tipData[22] = m_StageParam.Start22;
        tipData[23] = m_StageParam.Start23;
        tipData[24] = m_StageParam.Start24;
        tipData[25] = m_StageParam.Start25;
        tipData[26] = m_StageParam.Start26;
        tipData[27] = m_StageParam.Start27;
        tipData[28] = m_StageParam.Start28;
        tipData[29] = m_StageParam.Start29;
        tipData[30] = m_StageParam.Start30;

        for (int i = 0; i < m_StageParam.TipStartNum; i++)
        {

            AddAreaList(tipData[i],false,Vector3.zero);

        }
        tipData[0] = m_StageParam.Tip0;
        tipData[1] = m_StageParam.Tip1;
        tipData[2] = m_StageParam.Tip2;
        tipData[3] = m_StageParam.Tip3;
        tipData[4] = m_StageParam.Tip4;
        tipData[5] = m_StageParam.Tip5;
        tipData[6] = m_StageParam.Tip6;
        tipData[7] = m_StageParam.Tip7;
        tipData[8] = m_StageParam.Tip8;
        tipData[9] = m_StageParam.Tip9;
        tipData[10] = m_StageParam.Tip10;
        tipData[11] = m_StageParam.Tip11;
        tipData[12] = m_StageParam.Tip12;
        tipData[13] = m_StageParam.Tip13;
        tipData[14] = m_StageParam.Tip14;
        tipData[15] = m_StageParam.Tip15;
        tipData[16] = m_StageParam.Tip16;
        tipData[17] = m_StageParam.Tip17;
        tipData[18] = m_StageParam.Tip18;
        tipData[19] = m_StageParam.Tip19;
        tipData[20] = m_StageParam.Tip20;
        int cnt = 0;
        for(cnt = 0; cnt < m_StageParam.MustStTip; cnt++)
        {
            AddNextTips(tipData[cnt]);
        }
        for (; cnt < m_StageParam.CurrentNum; cnt++)
        {
            AddNextTips(tipData[Random.Range(0,m_StageParam.RundumNum)]);
        }
        SetCurrentTips(m_NextTips[0].tips);//最初にチェックしないための設定
        m_bCheckTriggerObj = false;//最初だけにする

        string tip_name = GetRandomNextTipsName();
        AddCareerTips(tip_name);
        m_TipCareer.SetCareerTipObj(m_CareerTips.tips);

        if (m_StageParam.ResultTip0!="")
        {
            AddResultTipsData(m_StageParam.ResultTip0,m_StageParam.ResultTip0_Detail,m_StageParam.ResultTip0_Num);
        }
        if (m_StageParam.ResultTip1 != "")
        {
            AddResultTipsData(m_StageParam.ResultTip1, m_StageParam.ResultTip1_Detail, m_StageParam.ResultTip1_Num);
        }
        if (m_StageParam.ResultTip2 != "")
        {
            AddResultTipsData(m_StageParam.ResultTip2, m_StageParam.ResultTip2_Detail, m_StageParam.ResultTip2_Num);
        }

    }
    //エリアの初期チップを配置
    private void AddAreaList(string tip_name,bool bPos,Vector3 area_pos)
    {
        {
            TipsData tipObj = GetTipsData(tip_name);
            // プレハブからインスタンスを生成
            GameObject gameObj = Instantiate<GameObject>(tipObj.tips, new Vector3(0, 0, 0), Quaternion.Euler(0,0,tipObj.tipsEntity.StRot), m_ParentObj.transform);

            if (gameObj != null)
            {
                Vector3 pos = Vector3.zero;
                if (bPos)
                {
                    pos = area_pos;
                }
                else
                {
                    pos.x = ((m_AreaTips.Count % 5) - 2) * 100;
                    pos.y = ((int)(m_AreaTips.Count / 5) * 100) - 250;
                }

                gameObj.transform.localPosition = pos;

                Rigidbody2D regd2d = gameObj.GetComponent<Rigidbody2D>();
                regd2d.isKinematic = false;

                CPzlTips pzlTip = gameObj.GetComponentInChildren<CPzlTips>();
                pzlTip.m_State = CPzlTips.TipState.Area;
                pzlTip.SetTipsStatus( this,tipObj.tipsEntity);

                TipsData areaTip = new TipsData();
                areaTip.SetData(tipObj.name, tipObj.tipsEntity, gameObj);
                m_AreaTips.Add(areaTip);
            }
            else
            {
                print("error tips[" + tipObj.name + "]\n");
            }

        }

    }

    public RectTransform GetCanvasRect()
    {
        return m_GameCanvas.GetComponent<RectTransform>();
    }

    //ランダムに次のチップの名前を得ます
    private string GetRandomNextTipsName()
    {
        string[] tipData = new string[21];
        tipData[0] = m_StageParam.Tip0;
        tipData[1] = m_StageParam.Tip1;
        tipData[2] = m_StageParam.Tip2;
        tipData[3] = m_StageParam.Tip3;
        tipData[4] = m_StageParam.Tip4;
        tipData[5] = m_StageParam.Tip5;
        tipData[6] = m_StageParam.Tip6;
        tipData[7] = m_StageParam.Tip7;
        tipData[8] = m_StageParam.Tip8;
        tipData[9] = m_StageParam.Tip9;
        tipData[10] = m_StageParam.Tip10;
        tipData[11] = m_StageParam.Tip11;
        tipData[12] = m_StageParam.Tip12;
        tipData[13] = m_StageParam.Tip13;
        tipData[14] = m_StageParam.Tip14;
        tipData[15] = m_StageParam.Tip15;
        tipData[16] = m_StageParam.Tip16;
        tipData[17] = m_StageParam.Tip17;
        tipData[18] = m_StageParam.Tip18;
        tipData[19] = m_StageParam.Tip19;
        tipData[20] = m_StageParam.Tip20;
        int randNextNo = (int)(Random.Range(0, m_StageParam.RundumNum));

        string result = "";
        result = tipData[randNextNo];
        return result;
    }
    //次のチップを用意
    private void AddNextTips(string tip_name)
    {

        {
            TipsData tipObj = GetTipsData(tip_name);
            // プレハブからインスタンスを生成
            GameObject gameObj = Instantiate<GameObject>(tipObj.tips, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, tipObj.tipsEntity.StRot), m_ParentObj.transform);

            if (gameObj != null)
            {
                Rigidbody2D regd2d = gameObj.GetComponent<Rigidbody2D>();
                CPzlTips pzlTip = gameObj.GetComponentInChildren<CPzlTips>();

                Vector3 pos = Vector3.zero;
                Vector3 scale = Vector3.one;
                if (m_NextTips.Count == 0)
                {
                    pos.x = m_NextObj.transform.localPosition.x;
                    pos.y = m_NextObj.transform.localPosition.y;
                    pos.z = m_NextObj.transform.localPosition.z;

                    regd2d.isKinematic = true;
                    pzlTip.m_State = CPzlTips.TipState.NextPickUp;
                }
                else
                {
                    pos.x = m_NextPosObj.transform.localPosition.x;
                    pos.y = m_NextPosObj.transform.localPosition.y;
                    pos.z = m_NextPosObj.transform.localPosition.z;

                    scale.x = 0.5f;
                    scale.y = 0.5f;

                    regd2d.isKinematic = false;
                    pzlTip.m_State = CPzlTips.TipState.Next;
                }
                gameObj.transform.localPosition = pos;
                gameObj.transform.localScale = scale;
                pzlTip.SetLayer(LayerMask.NameToLayer("PickUpLayer"));
                pzlTip.SetOrder(5, 2);
                pzlTip.SetTipsStatus( this ,tipObj.tipsEntity);

                TipsData nextTip = new TipsData();
                nextTip.SetData(tipObj.name, tipObj.tipsEntity, gameObj);
                m_NextTips.Add(nextTip);
            }
            else
            {
                print("error rundam tips[" + tipObj.name + "]\n");
            }
        }

    }
    //運ばれるチップを用意
    private void AddCareerTips(string tip_name)
    {
        {
            TipsData tipObj = GetTipsData(tip_name);
            // プレハブからインスタンスを生成
            GameObject gameObj = Instantiate<GameObject>(tipObj.tips, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, tipObj.tipsEntity.StRot), m_ParentObj.transform);

            if (gameObj != null)
            {
                Rigidbody2D regd2d = gameObj.GetComponent<Rigidbody2D>();
                CPzlTips pzlTip = gameObj.GetComponentInChildren<CPzlTips>();

                Vector3 pos = Vector3.zero;
                Vector3 scale = Vector3.one;
                pos.x = m_NextPosObj.transform.localPosition.x;
                pos.y = m_NextPosObj.transform.localPosition.y;
                pos.z = m_NextPosObj.transform.localPosition.z;

                scale.x = 0.5f;
                scale.y = 0.5f;

                regd2d.isKinematic = true;
                pzlTip.m_State = CPzlTips.TipState.Career;
                gameObj.transform.localPosition = pos;
                gameObj.transform.localScale = scale;
                pzlTip.SetLayer(LayerMask.NameToLayer("PickUpLayer"));
                pzlTip.SetOrder(5, 2);
                pzlTip.SetTipsStatus( this ,tipObj.tipsEntity);

                TipsData careerTip = new TipsData();
                careerTip.SetData(tipObj.name, tipObj.tipsEntity, gameObj);
                m_CareerTips = careerTip;
            }
            else
            {
                print("error career tips[" + tipObj.name + "]\n");
            }
        }
    }
    //販売チップを用意
    private void AddStoreTips(string tip_name)
    {
        {
            TipsData tipObj = GetTipsData(tip_name);
            // プレハブからインスタンスを生成
            GameObject gameObj = Instantiate<GameObject>(tipObj.tips, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, tipObj.tipsEntity.StRot), m_ParentObj.transform);

            if (gameObj != null)
            {
                Rigidbody2D regd2d = gameObj.GetComponent<Rigidbody2D>();
                CPzlTips pzlTip = gameObj.GetComponentInChildren<CPzlTips>();

                Vector3 pos = Vector3.zero;
                pos.x = m_StoreObj.transform.localPosition.x;
                pos.y = m_StoreObj.transform.localPosition.y;
                pos.z = m_StoreObj.transform.localPosition.z;

                regd2d.isKinematic = true;
                pzlTip.m_State = CPzlTips.TipState.StorePickUp;
                gameObj.transform.localPosition = pos;
                pzlTip.SetLayer(LayerMask.NameToLayer("PickUpLayer"));
                pzlTip.SetOrder(5, 2);
                pzlTip.SetTipsStatus( this ,tipObj.tipsEntity);


                TipsData storeTip = new TipsData();
                storeTip.SetData(tipObj.name, tipObj.tipsEntity, gameObj);
                m_StoreTips.Add(storeTip);
            }
            else
            {
                print("error store tips[" + tipObj.name + "]\n");
            }
        }
    }
    //
    private void AddResultTipsData(string tip_name,string tip_name_detail,int num)
    {
        {
            string result_name = GetResultTipName(tip_name, tip_name_detail);
            ResultTip tipObj = GetResultTipsData(result_name);
            // プレハブからインスタンスを生成
            GameObject gameObj = Instantiate<GameObject>(tipObj.tips, new Vector3(0, 0, 0), Quaternion.identity, m_ParentResultObj.transform);

            if (gameObj != null)
            {
                Vector3 pos = Vector3.zero;
                pos.x = m_ResultList.Count * 100;
                pos += m_ResultPosObj.transform.localPosition;
                gameObj.transform.localPosition = pos;

                CResultTip cResultTip = gameObj.GetComponentInChildren<CResultTip>();
                cResultTip.SetResultData(result_name, num);

                ResultTip resTip = new ResultTip();
                resTip.SetData(tipObj.name, tipObj.compTipsEntitiy, gameObj, num);
                m_ResultList.Add(resTip);
            }
            else
            {
                print("error result tips[" + tipObj.name + "]\n");
            }
        }
    }
    //指定のgameObjのTipDataを得ます。
    public TipsData GetAreaTips(GameObject gameObj)
    {
        for (int i = 0; i < m_AreaTips.Count; i++)
        {
            if (m_AreaTips[i].tips == gameObj)
            {
                return m_AreaTips[i];
            }
        }
        TipsData null_tips = new TipsData();
        null_tips.Init();
        return null_tips;
    }
    //指定のgameObjのTipDataを得ます。
    TipsData GetNextTips(GameObject gameObj)
    {
        for(int i = 0; i < m_NextTips.Count; i++)
        {
            if(m_NextTips[i].tips == gameObj)
            {
                return m_NextTips[i];
            }
        }
        TipsData null_tips = new TipsData();
        null_tips.Init();
        return null_tips;
    }
    //指定のgameObjのTipDataを得ます
    TipsData GetStoreTips(GameObject gameObj)
    {
        for (int i = 0; i < m_StoreTips.Count; i++)
        {
            if (m_StoreTips[i].tips == gameObj)
            {
                return m_StoreTips[i];
            }
        }
        TipsData null_tips = new TipsData();
        null_tips.Init();
        return null_tips;
    }

    int GetPickUpTipsCategory(GameObject gameObj)
    {
        TipsData nxTipsData = GetNextTips(gameObj);
        if (nxTipsData.tips)
        {
            return nxTipsData.tipsEntity.Category;
        }
        TipsData stTipsData = GetStoreTips(gameObj);
        if (stTipsData.tips)
        {
            return stTipsData.tipsEntity.Category;
        }
        return -1;
    }
    //
    private int GetNextPickUpObjNo()
    {
        for (int i = 0; i < m_NextTips.Count; i++)
        {
            if (m_NextTips[i].tips)
            {
                CPzlTips pzlTip = m_NextTips[i].pzlTips;
                if (pzlTip.m_State == CPzlTips.TipState.NextPickUp)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    private TipsData GetNextPickUpTips()
    {
        float pos_x = 0;
        int NextNo = -1;
        for (int i = 0; i < m_NextTips.Count; i++)
        {
            if (m_NextTips[i].tips)
            {
                CPzlTips pzlTip = m_NextTips[i].pzlTips;
                if (pzlTip.m_State == CPzlTips.TipState.Next)
                {
                    if (pos_x < m_NextTips[i].tips.transform.position.x)
                    {
                        NextNo = i;
                        pos_x = m_NextTips[i].tips.transform.position.x;
                    }
                }
            }
        }
        if (NextNo != -1)
        {
            return m_NextTips[NextNo];
        }
        TipsData null_tips = new TipsData();
        null_tips.Init();
        return null_tips;
    }

    //Next登録からエリア登録への切り替え
    public void SetAreaRemoveNextTips(GameObject gameObj,CPzlTips.TipState state)
    {
        switch (state)
        {
            case CPzlTips.TipState.NextPickUp:
                {
                    TipsData Tips = GetNextTips(gameObj);
                    m_AreaTips.Add(Tips);
                    m_NextTips.Remove(Tips);
                }
                break;
            case CPzlTips.TipState.StorePickUp:
                {
                    TipsData Tips = GetStoreTips(gameObj);
                    m_AreaTips.Add(Tips);
                    m_StoreTips.Remove(Tips);
                }
                break;
            default:break;
        }

    }

    /// <summary>
    /// 売れるかどうか
    /// </summary>
    /// <returns></returns>
    public bool IsSellItem()
    {
        return m_TipMonney.IsSellItem();
    }
    /// <summary>
    /// アイテム売った
    /// </summary>
    public void SetSellItem()
    {
        m_TipMonney.SellItem();
    }
    /// <summary>
    /// ピックアップした時の設定
    /// </summary>
    /// <param name="nextTips"></param>
    void SetNextPicupTips(TipsData nextTips)
    {
        CPzlTips pzlTip = nextTips.pzlTips;
        pzlTip.m_State = CPzlTips.TipState.NextPickUp;


        Rigidbody2D rig2d = nextTips.tips.GetComponent<Rigidbody2D>();
        rig2d.velocity = Vector2.zero;
        rig2d.angularVelocity = 0;
        rig2d.isKinematic = true;

    }

    /// <summary>
    /// ステージスタート
    /// </summary>
    public void StartStage()
    {
        m_TipGauge.SetStartGame();
        m_StageMonney = m_TipMonney.m_Monney;
        m_bCheckTriggerObj = false;//最初だけにする
        m_bGamePause = false;//ここではポーズはかかっていない
        m_StageClear.transform.gameObject.SetActive(false);
        m_StageClearWait = 0;
        m_ComboCount = 0;
    }
    /// <summary>
    /// ステージのアップデート
    /// </summary>
    public CPzlGameFlow.enGameFlow UpdateStage()
    {
        if (m_TipCareer.IsCareerEnd())
        {
            //運び終わった
            GameObject gameObj = m_CareerTips.tips;
            Rigidbody2D rig2d = gameObj.GetComponent<Rigidbody2D>();
            rig2d.isKinematic = false;
            CPzlTips pzlTip = m_CareerTips.pzlTips;
            pzlTip.m_State = CPzlTips.TipState.Next;


            m_NextTips.Add(m_CareerTips);

            m_TipCareer.SetStartPos();
            string tip_name = GetRandomNextTipsName();
            AddCareerTips(tip_name);
            m_TipCareer.SetCareerTipObj(m_CareerTips.tips);
        }


        if (m_NextTips.Count > 0)
        {
            //
            int nextPickUpObjNo = GetNextPickUpObjNo();
            if (nextPickUpObjNo == -1)
            {
                //次のピックアップオブジェクトがない

                TipsData nextTips = GetNextPickUpTips();

                CPzlTips pzlTip = nextTips.pzlTips;

                if (pzlTip.m_State != CPzlTips.TipState.NextPickUp)
                {
                    SetNextPicupTips(nextTips);
                }
            }
            else
            {
                //現在のピックアップオブジェクトを書字版の場所に配置
                TipsData nextTips = m_NextTips[nextPickUpObjNo];
                CPzlTips pzlTip = nextTips.pzlTips;
                if (pzlTip.m_bDrag == false)
                {
                    Vector3 pos = Vector3.zero;
                    Vector3 scale = Vector3.one;
                    pos.x = m_NextObj.transform.localPosition.x;
                    pos.y = m_NextObj.transform.localPosition.y;
                    pos.z = m_NextObj.transform.localPosition.z;

                    nextTips.tips.transform.localPosition += (pos - nextTips.tips.transform.localPosition) / 4.0f;
                    nextTips.tips.transform.localScale += (scale - nextTips.tips.transform.localScale) / 4.0f;
                }

            }
        }
        if (m_bStoreChange == true)
        {
            if (m_StoreTips.Count > 0 ){
                if (m_StoreTips[0].pzlTips.m_bDrag == false)
                {
                    m_StoreTips[0].pzlTips.ClearCollisionObject();
                    Destroy(m_StoreTips[0].tips);
                    m_StoreTips.Clear();
                }
            }
            m_bStoreChange = false;
        }
        if (m_StoreTips.Count == 0)
        {
            //購入できるアイテムのピックアップ
            NowNeedTips storeTipData = GetNowNeedTipsName();
            string tip_name = storeTipData.name;
            if (tip_name != "")
            {
                AddStoreTips(tip_name);
                TipsData storeData = m_StoreTips[0];

                m_TipMonney.SetStoreCost(tip_name, storeData.tipsEntity.StoreMonney, storeTipData.needPoint);
            }
            else
            {
                m_TipMonney.SetStoreMonney(0, 0);
            }

        }
        else
        {
            //ストアにあるアイテムの位置を定位置へ
            TipsData storeData = m_StoreTips[0];
            CPzlTips pzlTip = storeData.pzlTips;
            if (pzlTip.m_bDrag == false)
            {
                Vector3 pos = Vector3.zero;
                pos.x = m_StoreObj.transform.localPosition.x;
                pos.y = m_StoreObj.transform.localPosition.y;
                pos.z = m_StoreObj.transform.localPosition.z;

                storeData.tips.transform.localPosition += (pos - storeData.tips.transform.localPosition) / 4.0f;
            }

        }

        //納品物の残りチェック
        if (IsResultRestOver())
        {
            if(m_StageClear.transform.gameObject.active==false)
            {
                PlaySound.PlayBGM("result_win");
            }
            m_StageClear.transform.gameObject.SetActive(true);
            m_StageClearWait += Time.deltaTime;
            if (m_bCoProcess == false && m_StageClearWait > GlobalObjectPzl.m_GlobalParam.STAGE_CLEAR_WAIT)
            {

                m_StageClear.transform.gameObject.SetActive(false);
                if (m_StageParam.booktype == "End")
                {
                    //ステージ全終了
                    return CPzlGameFlow.enGameFlow.End;
                }
                //
                //ステージクリア
                return CPzlGameFlow.enGameFlow.NextStage;
            }
        }
        else
        {
            //ゲージ最大チェック
            if (m_TipGauge.IsGaugeMax())
            {
                if (m_bCoProcess == false)
                {
                    //ゲームオーバー
                    SetPause(true);//ポーズ
                    return CPzlGameFlow.enGameFlow.GameOverInit;

                }
            }

        }


        return CPzlGameFlow.enGameFlow.Null;

    }

    public void SetPause(bool enable)
    {
        //各チップのシミュレート
        foreach(TipsData tipData in m_AreaTips)
        {
            tipData.pzlTips.SetPause(enable);
        }
        foreach (TipsData tipData in m_NextTips)
        {
            tipData.pzlTips.SetPause(enable);
        }
        foreach (TipsData tipData in m_StoreTips)
        {
            tipData.pzlTips.SetPause(enable);
        }

        //運び屋
        m_TipCareer.SetPause(enable);

        //ゲージ
        m_TipGauge.SetPause(enable);

        m_bGamePause = enable;
    }
    public bool IsGamePause()
    {
        return m_bGamePause;
    }
    /// <summary>
    /// 残りの納品数が０となった？
    /// </summary>
    /// <returns></returns>
    bool IsResultRestOver()
    {
        int restCount = 0;
        for (int cnt = 0; cnt < m_ResultList.Count; cnt++)
        {
            restCount += m_ResultList[cnt].resTips.GetRestCount();
        }
        if (restCount <= 0)
        {
            return true;
        }

        return false;

    }

    public void SetCategoryPickUp(CPzlTips tips)
    {
        int nowCategory = -1;
        string exist_name = "";
        if (tips != null)
        {
            nowCategory = tips.GetCategory();
            exist_name = tips.GetTipsName();
        }
        for (int ii = 0; ii < m_AreaTips.Count; ii++)
        {
            TipsData tipData = m_AreaTips[ii];
            tipData.pzlTips.SetNowCategory(nowCategory, exist_name);
        }
    }

    /// <summary>
    /// 今一番必要なチップをチェック
    /// </summary>
    private struct NowNeedTips
    {
        public string name;
        public int needPoint;

    }
    private NowNeedTips GetNowNeedTipsName()
    {
        NowNeedTips ret;
        ret.name = "";
        ret.needPoint = 0;

        int needPoint = 0;
        int restCountMax = 999;
        int restNo = -1;
        int restCountCnt = 0;
        for(int i=0;i< m_ResultList.Count; i++)
        {
            ResultTip resTip =  m_ResultList[i];
            CResultTip cresTip =  resTip.resTips;
            int restCount = cresTip.GetRestCount();
            if (restCountMax > restCount && restCount!=0)
            {
                restCountMax = restCount;
                restNo = i;
            }
            restCountCnt += restCount;
        }

        if (restCountCnt > 0)
        {
            needPoint += (int)(GlobalObjectPzl.m_GlobalParam.REST_POINT_RESULT_CNT / restCountCnt);
        }

        if (restNo != -1)
        {
            ResultTip resTip = m_ResultList[restNo];
            switch (resTip.compTipsEntitiy.ResultData)
            {
                case "Variation":
                    {
                        string detail_name = resTip.GetBaseTipsName(Random.Range(0, resTip.compTipsEntitiy.BaseTipNum));
                        resTip = GetResultTipsData(GetResultTipName(resTip.compTipsEntitiy.CompResult, detail_name));
                    }
                    break;
                case "Tips":
                case "Result":
                default:
                    break;

            }

            string[] tipData = new string[11];
            tipData[0] = resTip.compTipsEntitiy.BaseTip0;
            tipData[1] = resTip.compTipsEntitiy.BaseTip1;
            tipData[2] = resTip.compTipsEntitiy.BaseTip2;
            tipData[3] = resTip.compTipsEntitiy.BaseTip3;
            tipData[4] = resTip.compTipsEntitiy.BaseTip4;
            tipData[5] = resTip.compTipsEntitiy.BaseTip5;
            tipData[6] = resTip.compTipsEntitiy.BaseTip6;
            tipData[7] = resTip.compTipsEntitiy.BaseTip7;
            tipData[8] = resTip.compTipsEntitiy.BaseTip8;
            tipData[9] = resTip.compTipsEntitiy.BaseTip9;
            tipData[10] = resTip.compTipsEntitiy.BaseTip10;

            int numTipMAX = 999;
            int areaTipsNum = 0;
            int nextTipsNum = 0;
            int tipNo = -1;
            for (int i=0;i< resTip.compTipsEntitiy.BaseTipNum; i++)
            {
                int areaNum = GetAreaTipsCount(tipData[i]);
                int nextNum = GetNextTipsCount(tipData[i]);
                if (numTipMAX > areaNum+nextNum)
                {
                    numTipMAX = areaNum + nextNum;
                    areaTipsNum = areaNum;
                    nextTipsNum = nextNum;
                    tipNo = i;
                }

            }

            if (tipNo != -1)
            {
                if (areaTipsNum == 0)
                {
                    needPoint += (int)(GlobalObjectPzl.m_GlobalParam.REST_POINT_AREATIPS_NUM_0);
                }
                else
                {
                    needPoint += (int)(GlobalObjectPzl.m_GlobalParam.REST_POINT_AREATIPS_NUM / areaTipsNum);
                }
                if (nextTipsNum == 0)
                {
                    needPoint += (int)(GlobalObjectPzl.m_GlobalParam.REST_POINT_NEXTTIPS_NUM_0);
                }
                else
                {
                    needPoint += (int)(GlobalObjectPzl.m_GlobalParam.REST_POINT_NEXTTIPS_NUM / nextTipsNum);
                }
                ret.name = tipData[tipNo];
                ret.needPoint = needPoint;
                return ret;
            }
        }

        return ret;
    }
    private int GetAreaTipsCount(string tip_name)
    {
        int num = 0;

        foreach(TipsData tipData in m_AreaTips)
        {
            if (tipData.name == tip_name)
            {
                num++;
            }
        }

        return num;
    }
    private int GetNextTipsCount(string tip_name)
    {
        int num = 0;

        foreach(TipsData tipData in m_NextTips)
        {
            if(tipData.name == tip_name)
            {
                num++;
            }
        }
        return num;
    }
    public bool IsPickupDragItem()
    {
        foreach (TipsData tipData in m_NextTips)
        {
            if (tipData.pzlTips.m_bDrag == true)
            {
                return true;
            }
        }
        foreach (TipsData tipData in m_StoreTips)
        {
            if (tipData.pzlTips.m_bDrag == true)
            {
                return true;
            }
        }

        return false;
    }
    public void ResetStage()
    {
        
        foreach (TipsData tipData in m_AreaTips)
        {
            tipData.pzlTips.ClearCollisionObject();
            Destroy(tipData.tips);
        }
        foreach (TipsData tipData in m_NextTips)
        {
            tipData.pzlTips.ClearCollisionObject();
            Destroy(tipData.tips);
        }
        foreach (TipsData tipData in m_StoreTips)
        {
            tipData.pzlTips.ClearCollisionObject();
            Destroy(tipData.tips);
        }
        if (m_CareerTips.tips != null)
        {
            m_CareerTips.pzlTips.ClearCollisionObject();
            Destroy(m_CareerTips.tips);
        }
        m_AreaTips.Clear();
        m_NextTips.Clear();
        m_StoreTips.Clear();
        m_CareerTips.Init();


        foreach (ResultTip resData in m_ResultList)
        {

            Destroy(resData.tips);
        }
        m_ResultList.Clear();

    }

    private void RemoveAreaTips(GameObject gameObj)
    {
        foreach (TipsData tipData in m_AreaTips)
        {
            if(tipData.tips == gameObj)
            {
                m_AreaTips.Remove(tipData);
                return;
            }
        }
    }

    public bool IsReset()
    {
        return true;
    }

    public void InitCombo()
    {
        m_ComboCount = 0;
    }
    

    /// <summary>
    /// 合成開始フラグ用のチェックオブジェクト設定
    /// </summary>
    /// <param name="gameObj"></param>
    public void SetCurrentTips(GameObject gameObj)
    {
        m_CheckTriggerObj = gameObj;　//オブジェクト指定
        //m_bCheckTriggerObj = false;   //一度有効になったらそのままにするので、コメント
    }
    public void CombinationStart()
    {
        m_bCheckTriggerObj = true;
    }
    /// <summary>
    /// チップの合成チェックのためのルーチン
    /// </summary>
    private struct CheckTips
    {
        public bool check;
        public GameObject obj;
    }
    public bool CheckCompTips(GameObject pzlTipGameObj)
    {
        if (m_bCheckProcess == true)
        {
            return false;
        }
        m_bCheckProcess = true;
       //ゲームオブジェクトからチェックエリアにある該当のTipData情報を取得
        TipsData tipData = GetAreaTips(pzlTipGameObj);
        CPzlTips pzlTips =  tipData.pzlTips;

        //チェックは有効？
        bool bTrigger = true;   //トリガーは通常は有効

        if (m_CheckTriggerObj != null && m_bCheckTriggerObj == false)
        {
            //基本的に無効状態（チェックして合成されたら有効）
            bTrigger = false;
        }

        //指定のGameObjectを全ての合成組み合わせにかかわるかをチェック
        foreach (ResultTip resTip in m_ResultBaseData)
        {
            bool bCheckStart = false;
            CheckTips[] checkBaseData = new CheckTips[resTip.compTipsEntitiy.BaseTipNum];
            for (int i = 0; i < resTip.compTipsEntitiy.BaseTipNum; i++) {
                //初期化
                checkBaseData[i].check = false;
                checkBaseData[i].obj = null;

                //今回のGameObjectが組み合わせに必要な場合に登録(※アイテムが削除予定でないこと前提）
                string baseName = resTip.GetBaseTipsName(i);
                if (tipData.name == baseName && pzlTips.m_bDestory==false)
                {
                    //チェック用のフラグとオブジェクトを設定
                    checkBaseData[i].check = true;
                    checkBaseData[i].obj = pzlTipGameObj;

                    bCheckStart = true; //チェックスタートさせる
                }
            }


            if (bCheckStart == true )
            {
                //引っかかったので、指定のゲームオブジェクトからのつながりをチェックする
                bool ret = CheckCollitionTip(tipData, resTip, ref checkBaseData);
                if (ret == true)
                {
                    //全て埋まったので合成可能
                    if (bTrigger == false) { 
                        //トリガーオブジェクトが有効になる前は合成しないので、無効時のフラグ更新処理
                        //※ユーザーが運んできたアイテムが合成されたらトリガー有効

                        for (int i = 0; i < checkBaseData.Length && m_CheckTriggerObj!=null ; i++)
                        {
                            if(checkBaseData[i].obj == m_CheckTriggerObj)
                            {
                                m_CheckTriggerObj = null;   //トリガーオブジェクトをnullにして通常通り有効に
                                m_bCheckTriggerObj = true;  //以後、次のオブジェクト設定（ドラッグ開始）まではフラグを有効にする
                                bTrigger = true;    //トリガーを有効にして合成開始
                            }
                        }
                    }

                    if (bTrigger == false)
                    {
                        //まだトリガー出来ないので次のチェックへ
                        continue;
                    }

                    //---------------ここから合成開始
                    //削除予約しておく
                    for (int i = 0; i < resTip.compTipsEntitiy.BaseTipNum; i++)
                    {
                        CPzlTips destroyPzlTip = checkBaseData[i].obj.GetComponentInChildren<CPzlTips>();
                        destroyPzlTip.m_bDestory = true;

                    }

                    //成立したアイテムの納品処理スタート
                    StartCoroutine(ResultCoRoutineStart(resTip, checkBaseData));
                    m_bCheckProcess = false;
                    return true;
                }
            }

        }
        m_bCheckProcess = false;
        return bTrigger;
    }
    private bool CheckCollitionTip(TipsData tipData, ResultTip resTip,ref CheckTips[] checkData)
    {
        CPzlTips pzlTips = tipData.pzlTips;
        int colCount = pzlTips.GetCollisionCount();
        for(int i = 0; i < colCount; i++)
        {
            GameObject colObj=pzlTips.GetCollisionObj(i);
            if (colObj == null)
            {
                continue;//既に削除されたオブジェクト
            }
            TipsData colTipData = GetAreaTips(colObj);
            if (colTipData.pzlTips.m_bDestory == true)
            {
                continue;//既に破棄予定のオブジェクト
            }
            bool resAllCheck = true;
            for (int j = 0;j < resTip.compTipsEntitiy.BaseTipNum; j++){
                if (checkData[j].check == true)
                {
                    continue;
                }
                resAllCheck = false;
                string baseName = resTip.GetBaseTipsName(j);
                if (baseName == colTipData.name)
                {
                    checkData[j].check = true;
                    checkData[j].obj = colObj;
                    bool ret = CheckCollitionTip(colTipData, resTip,ref checkData);
                    if (ret == false)
                    {
                        checkData[j].check = false;
                        checkData[j].obj = null;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            if (resAllCheck==true)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator ResultCoRoutineStart(ResultTip resTip, CheckTips[] checkBaseData)
    {
        m_bCoProcess = true;
        m_nProcessCnt++;
        //成立
        m_ComboCount++;
        int ComboCnt = m_ComboCount;//コルーチンでの遅延対応のためのローカル設定

        //中央位置を取得
        Vector3 area_pos = Vector3.zero;
        for (int i = 0; i < resTip.compTipsEntitiy.BaseTipNum; i++)
        {
            if (checkBaseData[i].check == false)
            {
                Debug.Assert(true, "Instance Delete Error\n");
            }

            area_pos += checkBaseData[i].obj.transform.localPosition;

        }
        area_pos /= resTip.compTipsEntitiy.BaseTipNum;//平均化した位置


        bool bTipItem = false; //別のチップに代わる？
        switch (resTip.compTipsEntitiy.ResultData)
        {
            case "Tips":
                {
                    bTipItem = true;
                }
                break;
            case "Variation":
            case "Result":
            default:
                break;
        }

        bool bDeliveryItem = false; //納品物？
        ResultTip nowResultTip = GetNowResultTipsData(resTip.name);
        int restResultCnt = 0;
        if (nowResultTip.resTips != null)
        {
            restResultCnt = nowResultTip.resTips.GetRestCount();
            //今回の納品物に含まれていて、残りもある
            if (restResultCnt > 0)
            {
                bDeliveryItem = true;
            }
        }

        //

        if (bDeliveryItem)
        {
            //アニメーションは納品物の時だけ
            yield return StartCoroutine(ResultAnimCoRoutine(resTip.compTipsEntitiy.Category, checkBaseData, nowResultTip.resTips));
        }
        //
        yield return StartCoroutine(ResultCoRoutine(checkBaseData)); //オブジェクト膨らませる

        //該当のインスタンスをすべて削除
        for (int i = 0; i<resTip.compTipsEntitiy.BaseTipNum; i++)
        {
            //エリアから外す
            RemoveAreaTips(checkBaseData[i].obj);
            //コリジョンをクリア
            CPzlTips pzlTip = checkBaseData[i].obj.GetComponentInChildren<CPzlTips>();
            if (pzlTip != null)
            {
                pzlTip.ClearCollisionObject();
            }
            //デストロイ
            Destroy(checkBaseData[i].obj);
        }

        StartCoroutine(ComboDispCoRoutine(area_pos, ComboCnt));

        if (bTipItem==true)
        {
            //チップの場合はアイテムを追加
            AddAreaList(resTip.compTipsEntitiy.CompResult, true, area_pos);
        }
        else
        {
            //それ以外はアイテムを納品（今回の納品物に限らない処理）
            yield return StartCoroutine(ResultDeliverCoRoutine(resTip, area_pos));//結果のアイテムを納品

        }

        if (bDeliveryItem == true)
        {
            //エフェクト
            StartCoroutine(OrderDispCoRoutine(nowResultTip, restResultCnt));

            //リザルト結果を反映して納品物をカウントダウン
            nowResultTip.resTips.DecResultData();//納品
            m_bStoreChange = true;

        }


        //yield return StartCoroutine(ResultGuildCardCoRoutine());//ギルドカード合わせ

        //お金もらう
        m_TipMonney.AddMonney(resTip.compTipsEntitiy.GetMonney);

        m_nProcessCnt--;
        m_bCoProcess = false;

    }
    IEnumerator ResultAnimCoRoutine(int categoryNo, CheckTips[] checkBaseData,CResultTip resTips)
    {
        if (m_nProcessCnt > 1)
        {
            //被らないように処理(終わるまでリターン）
            yield return null;
        }
        if (resTips.GetRestCount() > 0)
        {
            //まだ残りがあればアニメーション処理する

            for (int i = 0; i < checkBaseData.Length; i++)
            {
                CPzlTips tipObj = checkBaseData[i].obj.GetComponentInChildren<CPzlTips>();
                tipObj.SetActivateAddImage(true);
            }

            ResultAnim resAnim =m_ResultAnim[categoryNo];
            resAnim.obj.SetActive(true);
            if (resAnim.off_obj != null)
            {
                resAnim.off_obj.SetActive(false);
            }
        
            resAnim.anim.Play("action");
            while (resAnim.anim.GetCurrentAnimatorStateInfo(0).normalizedTime<1.0f)
            {
                //アニメーション処理中
                yield return null;
            }

            resAnim.obj.SetActive(false);
            if (resAnim.off_obj != null)
            {
                resAnim.off_obj.SetActive(true);
            }
        }

    }
    IEnumerator ResultCoRoutine(CheckTips[] checkBaseData)
    {
        for (int i = 0; i < checkBaseData.Length; i++)
        {
            CPzlTips tipObj = checkBaseData[i].obj.GetComponentInChildren<CPzlTips>();
            tipObj.SetActivateAddImage(true);
        }


        float scale_f = 1.0f;
        while (scale_f < GlobalObjectPzl.m_GlobalParam.RESULT_SCALE_PLS_X_MAX)
        {

            Vector3 scale = Vector3.one;

            scale.x = scale_f;
            scale.y = scale_f;
            scale_f += GlobalObjectPzl.m_GlobalParam.RESULT_SCALE_PLS_X;

            for (int i = 0; i < checkBaseData.Length; i++)
            {
                CPzlTips tipObj = checkBaseData[i].obj.GetComponentInChildren<CPzlTips>();
                checkBaseData[i].obj.transform.localScale = scale;

                float img_alpha = 1.0f - (scale_f / GlobalObjectPzl.m_GlobalParam.RESULT_SCALE_PLS_X_MAX);
                if (img_alpha < 0) img_alpha = 0.0f;
                tipObj.SetImageAlphaColor(img_alpha);

            }
            yield return null;
        }

    }

    IEnumerator ResultDeliverCoRoutine(ResultTip resTip,Vector3 st_pos)
    {
        Vector3 vecPos = m_ResultGeneralPosObj.transform.localPosition;
        
        //納品物の位置を取得
        ResultTip resNowTip = GetNowResultTipsData(resTip.name);
        if (resNowTip.tips != null)
        {
            vecPos = resNowTip.tips.transform.localPosition;
        }
        //
        // プレハブからインスタンスを生成
        GameObject gameObj = Instantiate<GameObject>(resTip.tips, Vector3.zero , Quaternion.identity, m_ParentResultObj.transform);
        gameObj.transform.localPosition = st_pos;
        CResultTip resGameTip = gameObj.GetComponent<CResultTip>();
        resGameTip.SetDeliverData();//納品用に変更



        float vecOffsAng = 0.0f;
        float VEC_OffsAddMAX = 0.125f;
        if (Random.Range(-1, 1) < 0)
        {
            VEC_OffsAddMAX *= -1;
        }

        float vecLength = 1.0f;
        float vecAddLength = 0.0f;
        float vecAddLengthS = GlobalObjectPzl.m_GlobalParam.DELIVER_POS_DEV;

        float chk_min = GlobalObjectPzl.m_GlobalParam.DELIVER_POS_CHK_MIN;
        while ((gameObj.transform.localPosition-vecPos).sqrMagnitude > (chk_min* chk_min))
        {
            vecAddLength += vecAddLengthS;
            vecLength += vecAddLength;
            float nowLength = (vecPos - gameObj.transform.localPosition).magnitude;
            if (vecLength > nowLength)
            {
                vecLength = nowLength;
            }

            Vector3 angOffsVec = vecPos - gameObj.transform.localPosition;
            float offs_ang = Mathf.Atan2(angOffsVec.y, angOffsVec.x);

            vecOffsAng += VEC_OffsAddMAX;
            if(vecOffsAng > Mathf.PI)
            {
                vecOffsAng = Mathf.PI;
            }
            if(vecOffsAng < -Mathf.PI)
            {
                vecOffsAng = -Mathf.PI;
            }

            Vector3 offsVec = Vector3.zero;
            offsVec.x = Mathf.Cos(UtilityFunction.Func.RadAngNormalize( offs_ang + vecOffsAng + Mathf.PI));
            offsVec.y = Mathf.Sin(UtilityFunction.Func.RadAngNormalize( offs_ang + vecOffsAng + Mathf.PI));

            //移動中
            //resGameTip.SetDeliverLocalPosition(gameObj.transform.localPosition + (vecPos - gameObj.transform.localPosition).normalized * vecLength);
            resGameTip.SetDeliverLocalPosition(gameObj.transform.localPosition + offsVec * vecLength);
            yield return null;
        }

        //到達したら作成したインスタンスを消す
        Destroy(gameObj);
    }

    IEnumerator ResultGuildCardCoRoutine()
    {
        yield return null;

    }
    IEnumerator ComboDispCoRoutine(Vector3 st_pos,int ComboCnt)
    {
        if (ComboCnt <= 1)
        {
            yield break;
        }

        // プレハブからインスタンスを生成
        GameObject gameObj = Instantiate<GameObject>(m_BonsCoinObjBase, Vector3.zero, Quaternion.identity, m_ParentEffectObj.transform);
        gameObj.transform.localPosition = st_pos;

        CPzlTipsEffect tipsEffect = gameObj.GetComponent<CPzlTipsEffect>();


        Color out_col = Color.red;
        switch (ComboCnt)
        {
            case 1:
                out_col=Color.yellow;
                break;
            case 2:
                out_col=new Color(1.0f,0.5f, 0.0f);
                break;
            case 3:
                out_col=new Color(1.0f,0.25f,0.0f);
                break;
            default:
                break;
        }
        tipsEffect.SetTextOutlineColor(string.Format("x{0}COMBO!", ComboCnt), out_col);
        tipsEffect.SetEmissionParticleCount(ComboCnt * 5);

        //お金もらう
        m_TipMonney.AddMonney(ComboCnt*500);
        float col_alpha_f = 1.0f;
        float dec_col_alpha_f = 0.05f;

        float pos_y = 0;
        float dec_y_add = 5.0f;
        float dec_y_add_f = 0.25f;

        while (col_alpha_f > 0)
        {
            pos_y += dec_y_add;
            dec_y_add -= dec_y_add_f;

            if (dec_y_add < 0)
            {
                col_alpha_f -= dec_col_alpha_f;
                if (col_alpha_f <= 0.0f)
                {
                    col_alpha_f = 0.0f;
                }

            }

            tipsEffect.SetTextAlpha(col_alpha_f);

            Vector3 pos = Vector3.zero;
            pos.y = pos_y;
            tipsEffect.SetTextObjPos(pos);

            yield return null;
        }

        //到達したら作成したインスタンスを消す
        Destroy(gameObj);

    }
    IEnumerator OrderDispCoRoutine(ResultTip resTip,int restCnt)
    {
        if (restCnt <= 0)
        {
            yield break;
        }

        // プレハブからインスタンスを生成
        GameObject gameObj = Instantiate<GameObject>(m_BonsBookObjBase, Vector3.zero, Quaternion.identity, m_ParentEffectObj.transform);
        gameObj.transform.localPosition = resTip.tips.transform.localPosition;

        int monnyBonus = (int)(resTip.compTipsEntitiy.GetMonney * 0.5f);

        CPzlTipsEffect tipsEffect = gameObj.GetComponent<CPzlTipsEffect>();

        Color out_col = Color.red;
        switch (resTip.compTipsEntitiy.Category)
        {
            case 0:
                out_col=Color.blue;
                break;
            case 1:
                out_col=Color.yellow;
                break;
            case 2:
                out_col=Color.red;
                break;
            case 3:
                out_col=Color.magenta;
                break;
            default:
                break;
        }
        tipsEffect.SetTextOutlineColor(string.Format("SUPPLY!"), out_col);
        tipsEffect.SetEmissionParticleCount(monnyBonus / 500);

        //お金もらう
        m_TipMonney.AddMonney(monnyBonus);
        float col_alpha_f = 1.0f;
        float dec_col_alpha_f = 0.05f;

        float pos_y = 0;
        float dec_y_add = 5.0f;
        float dec_y_add_f = 0.25f;

        while (col_alpha_f > 0)
        {
            pos_y += dec_y_add;
            dec_y_add -= dec_y_add_f;

            if (dec_y_add < 0)
            {
                col_alpha_f -= dec_col_alpha_f;
                if (col_alpha_f <= 0.0f)
                {
                    col_alpha_f = 0.0f;
                }

            }

            tipsEffect.SetTextAlpha(col_alpha_f);

            Vector3 pos = Vector3.zero;
            pos.y = pos_y;
            tipsEffect.SetTextObjPos(pos);

            yield return null;
        }

        //到達したら作成したインスタンスを消す
        Destroy(gameObj);

    }
}
