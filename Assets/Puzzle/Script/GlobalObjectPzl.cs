using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

[DefaultExecutionOrder((int)UtilityFunction.Func.ActionLayer.normal)]
public class GlobalObjectPzl : GlobalObjectBase
{
    [System.Serializable]
    public class GlobalParamater
    {
        [Tooltip("バージョン情報")]
        public float VERSION = 0.36f;
        [Tooltip("コリジョンヒットからのリミット時間(s)")]
        public float LINK_TIME = 0.5f;
        [Tooltip("キャリアーの角度更新補正値")]
        public float CAREER_ANG_SPEED = 2.0f;
        [Tooltip("キャリアーの傾き角度最大")]
        public float CAREER_ANG_MAX = 45.0f;
        [Tooltip("キャリアーの移動スピード最大")]
        public float CAREER_MOVE_SPEED = 3.0f;
        [Tooltip("ゲージのスピード")]
        public float GAUGE_SPEED = 1.0f;
        [Tooltip("ゲージの上昇率補正")]
        public float GAUGE_UP = 0.0001f;
        [Tooltip("ゲージの上昇スピード")]
        public float GAUGE_UP_SPEED = 0.001f;
        [Tooltip("鼓動の速さ")]
        public float GAUGE_ANG_SPEED = 10.0f;
        [Tooltip("鼓動の間隔")]
        public float GAUGE_ANG_TIME_MAX = 20.0f;
        [Tooltip("鼓動の大きさ")]
        public float GAUGE_ANG_RATE_MAX = 0.03f;
        [Tooltip("必要アイテムが減ってきたら増えるポイント")]
        public float REST_POINT_RESULT_CNT = 100.0f;
        [Tooltip("エリアにあるチップが無くなったら増えるポイント")]
        public float REST_POINT_AREATIPS_NUM_0 = 200.0f;
        [Tooltip("エリアにあるチップが減ってきたら増えるポイント")]
        public float REST_POINT_AREATIPS_NUM = 100.0f;
        [Tooltip("次にあるチップが無くなったら増えるポイント")]
        public float REST_POINT_NEXTTIPS_NUM_0 = 300.0f;
        [Tooltip("次にあるチップが減ってきたら増えるポイント")]
        public float REST_POINT_NEXTTIPS_NUM = 200.0f;
        [Tooltip("値段が高くなるポイントの閾値")]
        public float NEED_POINT_UPPER_COST = 400.0f;
        [Tooltip("値段が安くなるポイントの閾値")]
        public float NEED_POINT_DISCOUNT_COST = 200.0f;
        [Tooltip("結果がはっきりした後のスケール加算値")]
        public float RESULT_SCALE_PLS_X = 0.1f;
        [Tooltip("結果がはっきりした後のスケール加算値最大")]
        public float RESULT_SCALE_PLS_X_MAX = 3.0f;
        [Tooltip("納品物の移動スピード")]
        public float DELIVER_POS_DEV = 0.1f;
        [Tooltip("納品物納品の距離")]
        public float DELIVER_POS_CHK_MIN = 10.0f;
        [Tooltip("アドバタイズボタンで得る金額")]
        public int GET_ADS_MONNEY = 5000;
        [Tooltip("タイトルで落としてくる本の数")]
        public int TITLE_BOOKS_NUM = 50;
        [Tooltip("タイトルで落としてくる本のスケール")]
        public float TITLE_BOOKS_SCALE = 2.0f;
        [Tooltip("コンティニュー時の身食いゲージの減少率")]
        public float CONTINUE_DEC_GAUGE_RATE = 0.5f;
        [Tooltip("コンティニュー時の魔石の値段")]
        public int CONTINUE_MASEKI_PRICE = 20000;
        [Tooltip("汗の表示秒数")]
        public float SWEAT_CNT = 1.0f;
        [Tooltip("タップして動く量")]
        public float SWEAT_MOVE = 0.1f;
        [Tooltip("マップチップでスケールがかかる量")]
        public float MAPTIP_OPEN_SCALE_PLS = 0.4f;
        [Tooltip("マップチップでフェードされる量")]
        public float MAPTIP_OPEN_FADE_PLS = 0.2f;
        [Tooltip("エンディングでマスクが移動するスピード")]
        public float ENDING_CLEAR_FLOW_SPD = 1.0f;
        [Tooltip("エンディングでチップが消えるスピード")]
        public float ENDING_CEAR_ALPA_SPD = 0.01f;
        [Tooltip("ステージをクリアした時のウェイト秒数")]
        public float STAGE_CLEAR_WAIT = 3.0f;
        [Tooltip("アドバタイズボタン押した後のウェイト")]
        public float ADVERTIZE_TICK_WAIT = 1.0f;

        public GlobalParamater Clone()
        {
            return (GlobalParamater)MemberwiseClone();
        }

    }

    [System.Serializable]
    public class DebugParamater
    {
        [Tooltip("アドバタイズテストモード(ServiceのTestモード有効化も必要)")]
        public bool ADVERTIZE_TEST_MODE = true;
        [Tooltip("コリジョンライン表示")]
        public bool DEBUG_DRAW_COLLISION_LINE = false;
    }

    [SerializeField] private Pzl_StageParams m_StageParamData;
    [SerializeField] private Pzl_TipsParams m_TipsParamData;
    [SerializeField] static public Pzl_StageParams m_StageParam;
    [SerializeField] static public Pzl_TipsParams m_TipsParam;

    [SerializeField] public GlobalParamater m_GlobalParamData;
    [SerializeField] static public GlobalParamater m_GlobalParam;


    //デバッグフラグ
    public DebugParamater m_DebugParam;


    static public GlobalObjectPzl GetInstance()
    {
        return (GlobalObjectPzl)singleton;
    }

    void Awake()
    {
        base.Awake();

        m_StageParam = m_StageParamData.Clone();
        m_TipsParam = m_TipsParamData.Clone();

        m_GlobalParam = m_GlobalParamData.Clone();

    }
    // Use this for initialization
    void Start()
    {
        base.Start();
        //Advertisement.Initialize("", true);
        
#if UNITY_ANDROID
            Advertisement.Initialize("3105093", m_DebugParam.ADVERTIZE_TEST_MODE);
#elif UNITY_IOS
            Advertisement.Initialize("3105092",m_DebugParam.ADVERTIZE_TEST_MODE);
#else
#endif
        
    }

    // Update is called once per frame
    void Update()
    {

    }


}
