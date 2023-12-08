using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPzlMapTip : MonoBehaviour
{
    [SerializeField] private Image m_BookImage;
    [SerializeField] private Image m_OpenImage;
    [SerializeField] private GameObject m_SpriteMaskObj;

    private Vector3 m_start_scale;
    private float m_open_scale=1.0f;

    private float m_now_open_scale=0.0f;
    private float m_cross_fade_x=0.0f;
    private bool m_bSpriteOpen=false;

    private void Awake()
    {
        m_start_scale.x = 100.0f;
        m_start_scale.y = 100.0f;
        m_start_scale.z = 1.0f;
        // m_SpriteMaskObj.transform.localScale;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_bSpriteOpen == true)
        {
            if (m_now_open_scale < m_open_scale)
            {
                m_now_open_scale += GlobalObjectPzl.m_GlobalParam.MAPTIP_OPEN_SCALE_PLS;
                if (m_now_open_scale > m_open_scale)
                {
                    m_now_open_scale = m_open_scale;
                    m_bSpriteOpen = false;
                }
                Vector3 scale = m_SpriteMaskObj.transform.localScale;
                scale.x = m_start_scale.x * m_now_open_scale;
                scale.y = m_start_scale.y * m_now_open_scale;
                scale.z = 1.0f;
                m_SpriteMaskObj.transform.localScale = scale;
            }
        }

        if (m_OpenImage.transform.gameObject.activeSelf == true)
        {
            if (m_cross_fade_x < 1.0f)
            {
                m_cross_fade_x += GlobalObjectPzl.m_GlobalParam.MAPTIP_OPEN_FADE_PLS;
                if (m_cross_fade_x >= 1.0f)
                {
                    m_cross_fade_x = 1.0f;
                    m_BookImage.transform.gameObject.SetActive(false);
                }

                Color color_b = m_BookImage.color;
                color_b.a = 1.0f - m_cross_fade_x;
                m_BookImage.color = color_b;

                Color color_o = m_OpenImage.color;
                color_o.a = m_cross_fade_x;
                m_OpenImage.color = color_o;

            }
        }
    }

    public void SetImageAlpha(float alpha)
    {
        Color color_op = m_OpenImage.color;
        Color color_b = m_BookImage.color;
        if (color_op.a > alpha)
        {
            color_op.a = alpha;
            m_OpenImage.color = color_op;
        }
        if(color_b.a > alpha)
        {
            color_b.a = alpha;
            m_BookImage.color = color_b;
        }
    }

    public bool IsAnimationEnd()
    {
        if(m_BookImage.transform.gameObject.activeSelf==false && m_bSpriteOpen == false)
        {
            return true;
        }
        return true;
    }

    public void SetPositionScale(float px,float py,float scale_x)
    {
        Vector3 pos = transform.localPosition;
        pos.x = px;
        pos.y = py;
        pos.z = 0.0f;
        transform.localPosition = pos;

        m_open_scale = scale_x;
    }
    public void StageStart()
    {
        m_bSpriteOpen = true;
        m_now_open_scale = 0.0f;

    }
    public void SetOpen()
    {
        m_OpenImage.transform.gameObject.SetActive(true);
        m_cross_fade_x = 0.0f;
    }
    public void SetClose()
    {
        m_cross_fade_x = 0.0f;

        m_BookImage.transform.gameObject.SetActive(true);
        m_OpenImage.transform.gameObject.SetActive(false);
        m_SpriteMaskObj.transform.localScale = m_start_scale;
    }
}
