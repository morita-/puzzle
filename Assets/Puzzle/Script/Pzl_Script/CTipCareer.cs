using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTipCareer : MonoBehaviour
{
    [SerializeField] private GameObject m_CareerPos;
    [SerializeField] private Image m_Sweat;
    private GameObject m_nextPosObj;
    private float m_Speed;

    private float m_AngleSpeed;
    private float m_AngleMax;
    private Vector3 m_StartPos;
    private GameObject m_CareerTipObj=null;
    private float m_SweatCnt;
    private bool m_bPause;

    private void Awake()
    {
        m_StartPos = transform.localPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_bPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bPause)
        {
            return;
        }
        Vector3 pos = transform.localPosition;
        pos.x += Time.deltaTime * m_Speed * GlobalObjectPzl.m_GlobalParam.CAREER_MOVE_SPEED;

        if (IsCareerEnd())
        {
            m_AngleMax -= Time.deltaTime;
            if (m_AngleMax < 0)
            {
                m_AngleMax = 0;
            }
        }

        m_AngleSpeed += Time.deltaTime * GlobalObjectPzl.m_GlobalParam.CAREER_ANG_SPEED;
        float rot = (Mathf.Sin(m_AngleSpeed)/Mathf.PI) * m_AngleMax * GlobalObjectPzl.m_GlobalParam.CAREER_ANG_MAX;

        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(0, 0, rot);

        if (m_CareerTipObj != null)
        {
            m_CareerTipObj.transform.position = m_CareerPos.transform.position;
        }

        if (m_SweatCnt > 0.0f)
        {
            m_SweatCnt -= Time.deltaTime;
            if (m_SweatCnt < 0.0f)
            {
                m_SweatCnt = 0;
            }
            Color color = m_Sweat.color;
            color.a = m_SweatCnt / GlobalObjectPzl.m_GlobalParam.SWEAT_CNT;
            m_Sweat.color = color;
        }

    }
    public void SetPause(bool enable)
    {
        m_bPause = enable;
    }
    public void SetCareerSpeed(float speed,GameObject nextPosObj)
    {
        m_Speed = speed;
        m_nextPosObj = nextPosObj;
        m_AngleMax = 1;
    }
    public bool IsCareerEnd()
    {
        if (transform.localPosition.x > m_nextPosObj.transform.localPosition.x)
        {
            return true;
        }
        return false;
    }
    public void SetStartPos()
    {
        m_AngleMax = 1;
        transform.localPosition = m_StartPos;

    }
    public void SetCareerTipObj(GameObject tipObj)
    {
        m_CareerTipObj = tipObj;
    }
    public void OnPointerClick()
    {
        m_SweatCnt = GlobalObjectPzl.m_GlobalParam.SWEAT_CNT;
        Quaternion qrot = m_Sweat.transform.localRotation;
        qrot.SetEulerAngles(0, 0, Random.Range(-0.1f, 1.0f));
        m_Sweat.transform.localRotation = qrot;

        //ちょっとだけ移動
        Vector3 pos = transform.localPosition;
        pos.x += GlobalObjectPzl.m_GlobalParam.SWEAT_MOVE;
        transform.localPosition = pos;
    }
}
