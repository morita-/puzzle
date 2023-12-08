using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPzlTipsCollision : MonoBehaviour
{
    public struct LinkTips{
        public float count;
        public float all_count;
        public GameObject link_tips;
        public bool tip_check;
    }
    public List<LinkTips> m_LinkList;

    
    private CPzlTips m_PzlTips=null;
    private CPzlStatus m_PzlStatus=null;

    // Start is called before the first frame update
    void Start()
    {
        m_LinkList = new List<LinkTips>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_LinkList.Count; i++)
        {
            LinkTips tips = m_LinkList[i];
            tips.count -= Time.deltaTime;
            tips.all_count += Time.deltaTime;

            if (tips.tip_check == false && tips.count > 0)
            {
                //有効なつながりで、まだ合成チェックしていないので、チェックさせる
                tips.tip_check = m_PzlStatus.CheckCompTips(tips.link_tips); //チェック！
            }
            m_LinkList[i] = tips;
        }

        //カウントが0以下になったら削除
        for (int idx = m_LinkList.Count-1; idx >=0 ; idx--)
        {
            LinkTips tips = m_LinkList[idx];
            if (tips.count <= 0)
            {
                m_LinkList.RemoveAt(idx);
            }
        }

        if (GlobalObjectPzl.GetInstance().m_DebugParam.DEBUG_DRAW_COLLISION_LINE==true)
        {
            foreach (LinkTips tips in m_LinkList)
            {
                if (tips.link_tips != null)
                {
                    int myCategoryNo = m_PzlTips.GetCategory();
                    CPzlStatus.TipsData tipData = m_PzlStatus.GetAreaTips(tips.link_tips);
                    if(tipData.tipsEntity.Category== myCategoryNo)
                    {
                        Color col = Color.black;
                        switch (myCategoryNo)
                        {
                            case 0:col = Color.blue;break;
                            case 1: col = Color.yellow; break;
                            case 2: col = Color.red; break;
                            case 3: col = Color.magenta; break;
                            default: break;
                        }
                        Debug.DrawLine(m_PzlTips.GetGameObject().transform.position, tipData.tips.transform.position,col);
                    }

                }

            }
        }
    }

    void SetLinkList(GameObject gameObj)
    {
        for(int i=0;i<m_LinkList.Count;i++){
            LinkTips tips = m_LinkList[i];
        
            if (tips.link_tips == gameObj)
            {
                tips.count = GlobalObjectPzl.m_GlobalParam.LINK_TIME;
                return;
            }
        }

        if (m_PzlStatus.IsPickupDragItem() == true)
        {
            //ドラッグ中
            return;
        }

        LinkTips newtips = new LinkTips();
        newtips.count = GlobalObjectPzl.m_GlobalParam.LINK_TIME;
        newtips.all_count = 0;
        newtips.link_tips = gameObj;

        //合成可能中にチェックされた？
        newtips.tip_check = m_PzlStatus.CheckCompTips(gameObj); //チェック！

        m_LinkList.Add(newtips);
    }

    public void SetPzlStatus(CPzlStatus status,CPzlTips pzlTips)
    {
        m_PzlStatus = status;
        m_PzlTips = pzlTips;
    }

    void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.tag != "PzlTips")  //PzlTipsタグ以外は対象外
        {
            return;
        }
        if(collision.gameObject.layer != LayerMask.NameToLayer("UI"))   //UIレイヤー以外は利用しないので取らない
        {
            return;
        }
        SetLinkList(collision.gameObject);

 //      Debug.Log("OnCollisionStay2D: " + collision.gameObject.name);
    }

}
