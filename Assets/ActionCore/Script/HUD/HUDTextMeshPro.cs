using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDTextMeshPro : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMeshPro;
    [SerializeField] private Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetText(string text)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.text = text;
        }
        else
        {
            m_Text.text = text;
        }
    }
    public void SetTextColor(Color col)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.color = col;
        }
        else
        {
            m_Text.color = col;
        }
    }
    public Color GetTextColor()
    {
        if (m_TextMeshPro != null)
        {
            return m_TextMeshPro.color;
        }
        else
        {
            return m_Text.color;
        }
    }
    public void SetTextFaceColor(Color col)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.faceColor = col;
        }
        else
        {
            m_Text.color = col;
        }
    }
    public void SetTextOutlineColor(Color col)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.outlineColor = col;
        }
        else
        {
            m_Text.color = col;
        }
    }
    public void SetTextActivate(bool _active)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.gameObject.SetActive(_active);
        }
        else
        {
            m_Text.gameObject.SetActive(_active);
        }
    }

    public void SetTextScale(float scale)
    {
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.transform.localScale = new Vector3(scale, scale, 0.0f);
        }
        else
        {
            m_Text.transform.localScale = new Vector3(scale, scale, 0.0f);
        }
    }

    public bool IsActivate()
    {
        return gameObject.activeSelf;
    }
    public void SetActivate(bool _active)
    {
        gameObject.SetActive(_active);
    }
}
