using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTipGauge : MonoBehaviour
{
    [SerializeField] private RectTransform m_Mask;
    [SerializeField] private RectTransform m_Base;
    [SerializeField] private RectTransform m_SubGauge;
    [SerializeField] private Image m_AlartImage;

    [SerializeField] private float m_GaugeRate;  //ゲージの進み
    [SerializeField] private float m_NowGaugeRate;   //実際のゲージの進み具合

    private float m_Speed;  //スピード
    private float m_UpGaugeRate; //ゲージのアップサイズ
    private float m_AngRate;    //鼓動分のプラスゲージ分
    private float m_AngleTime;  //鼓動の角度の進み具合
    private float m_SubGagueRate;  //サブゲージの進み具合
    private bool m_bUpdate; //アップデートフラグ

    // Start is called before the first frame update
    void Start()
    {
        m_bUpdate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bUpdate == false)
        {
            return;
        }
        m_SubGagueRate += Time.deltaTime * m_Speed * GlobalObjectPzl.m_GlobalParam.GAUGE_SPEED;
        if (m_SubGagueRate >= 1.0f)
        {
            m_GaugeRate += m_UpGaugeRate * GlobalObjectPzl.m_GlobalParam.GAUGE_UP;
            m_SubGagueRate = 0.0f;
            if (m_GaugeRate > 1.0f)
            {
                m_GaugeRate = 1.0f;
            }
        }

        //鼓動
        m_AngleTime+=Time.deltaTime * GlobalObjectPzl.m_GlobalParam.GAUGE_ANG_SPEED;
        bool bGaugeUpdate=true;
        float angleTime = m_AngleTime;
        if (m_AngleTime > Mathf.PI*2)
        {
            angleTime = Mathf.PI * 2;
            bGaugeUpdate = false;
        }
        if(m_AngleTime > GlobalObjectPzl.m_GlobalParam.GAUGE_ANG_TIME_MAX || m_GaugeRate < m_NowGaugeRate)
        {
            m_AngleTime = 0;
        }
        m_AngRate = (-Mathf.Cos(angleTime) / Mathf.PI) * GlobalObjectPzl.m_GlobalParam.GAUGE_ANG_RATE_MAX + GlobalObjectPzl.m_GlobalParam.GAUGE_ANG_RATE_MAX;



        //ゲージ量更新
        if (m_GaugeRate > m_NowGaugeRate && bGaugeUpdate)
        {
            m_NowGaugeRate += GlobalObjectPzl.m_GlobalParam.GAUGE_UP_SPEED;
            if (m_GaugeRate < m_NowGaugeRate)
            {
                m_NowGaugeRate = m_GaugeRate;
            }
        }
        if(m_GaugeRate < m_NowGaugeRate)
        {
            m_NowGaugeRate -= GlobalObjectPzl.m_GlobalParam.GAUGE_UP_SPEED;
            if(m_GaugeRate > m_NowGaugeRate)
            {
                m_NowGaugeRate = m_GaugeRate;
            }
        }

        //データ更新
        Vector3 pos3 = m_SubGauge.localPosition;
        pos3.y = m_Base.sizeDelta.y * m_SubGagueRate;
        m_SubGauge.localPosition = pos3;
        
        Vector2 rect = m_Mask.sizeDelta;
        rect.y = m_Base.sizeDelta.y * (m_NowGaugeRate+m_AngRate);
        m_Mask.sizeDelta = rect;

        Color col = m_AlartImage.color;
        col.a = m_GaugeRate * (m_AngRate/ GlobalObjectPzl.m_GlobalParam.GAUGE_ANG_RATE_MAX);
        m_AlartImage.color = col;
    }
    public void SetStartGame()
    {
        m_bUpdate = true;
        m_GaugeRate = m_UpGaugeRate * GlobalObjectPzl.m_GlobalParam.GAUGE_UP;
        m_NowGaugeRate = m_GaugeRate;
        m_SubGagueRate = 0;
    }
    public void SetPause(bool enable)
    {
        m_bUpdate = !enable;
    }
    public void SetGaugeUpSpeed(float speed,float up_rate)
    {
        m_Speed = speed;
        m_UpGaugeRate = up_rate;
    }
    public void DecGage(float dec)
    {
        m_GaugeRate -= dec;
        m_NowGaugeRate -= dec / 10.0f;
    }
    public bool IsGaugeMax()
    {
        if (m_NowGaugeRate >= 1.0f)
        {
            return true;
        }
        return false;
    }
}
