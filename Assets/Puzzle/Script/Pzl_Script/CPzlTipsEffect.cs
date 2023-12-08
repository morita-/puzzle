using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPzlTipsEffect : MonoBehaviour
{
    [SerializeField] private GameObject m_TextObj;
    [SerializeField] private GameObject m_ParticleObj;
    public HUDTextMeshPro m_Text;
    public ParticleSystem m_Particle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTextObjPos(Vector3 localPos)
    {
        m_TextObj.transform.localPosition = localPos;
    }

    public void SetTextOutlineColor(string text,Color col)
    {
        m_Text.SetText(text);
        m_Text.SetTextOutlineColor(col);
    }
    public void SetTextAlpha(float alpha)
    {
        Color col = Color.white;
        col.a = alpha;
        m_Text.SetTextColor(col);
    }
    public void SetEmissionParticleCount(int count)
    {
        if (m_Particle.emission.burstCount > 0)
        {
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[m_Particle.emission.burstCount];
            m_Particle.emission.GetBursts(bursts);
            bursts[0].count = count;
        }
    }
}
