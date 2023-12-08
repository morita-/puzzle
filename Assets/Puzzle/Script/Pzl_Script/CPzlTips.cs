using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPzlTips : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_BackImg;
    [SerializeField] private SpriteRenderer m_BaseImg;
    [SerializeField] private SpriteRenderer m_AddImg;
    [SerializeField] private CPzlTipsCollision m_TipsCollision;
    [SerializeField] private GameObject m_MainObj;

    private RectTransform m_CanvasRect;

    Vector3 m_StartPos;
    Vector3 m_ClickPos;
    public bool m_bDrag;
    public bool m_bDestory=false;

    private CPzlStatus m_TipsStatus=null;
    private Pzl_TipsParamsTipsEntity m_TipEntity = null;

    public enum TipState
    {
        Area,
        NextPickUp,
        Next,
        StorePickUp,
        Career,
    }

    public TipState m_State=TipState.Area;


    // Start is called before the first frame update
    void Start()
    {
        m_bDrag = false;
        SetOrder(5, 2);
    }

    // Update is called once per frame
    void Update()
    {

    }
    //現在コントロールできるかどうかのチェック
    private bool IsCtrlTips()
    {
        if (m_State != TipState.NextPickUp && m_State != TipState.StorePickUp)
        {
            //ピックアップアイテム以外は操作不可能
            return false;
        }
        return true;
    }
    private bool IsDecideTips()
    { 
        if (m_State == TipState.StorePickUp && m_TipsStatus.IsSellItem() == false)
        {
            //ストアのピックアップアイテム且つ売り物ではない場合は決定できない
            return false;
        }
        return true;
    }
    private bool IsSellEnableTips()
    {
        if (m_State == TipState.StorePickUp && m_TipsStatus.IsSellItem() == true)
        {
            //ストアのピックアップアイテム且つ購入可能な場合はTrue
            return true;
        }
        return false;
    }
    private bool IsTipTaps(GameObject nextObj)
    {
        //ピックアップしたアイテムをすぐに話してしまった時のチェック
        if (m_State != TipState.NextPickUp)
        {
            return false;
        }
        if (nextObj != null)
        {
            RectTransform rectTr = nextObj.GetComponent<RectTransform>();
            float min_x = rectTr.position.x - rectTr.rect.width / 2;
            float max_x = rectTr.position.x + rectTr.rect.width / 2;
            float min_y = rectTr.position.y - rectTr.rect.height / 2;
            float max_y = rectTr.position.y + rectTr.rect.height / 2;
            Vector2 pos = transform.position;
            if (pos.x>min_x && pos.x<max_x && pos.y>min_y && pos.y < max_y)
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// チップのステータスとエンティティ情報を設定
    /// </summary>
    /// <param name="status"></param>
    /// <param name="entity"></param>
    public void SetTipsStatus(CPzlStatus status,Pzl_TipsParamsTipsEntity entity)
    {
        m_TipsStatus = status;
        m_TipEntity = entity;
        m_TipsCollision.SetPzlStatus(status,this);
        m_CanvasRect = m_TipsStatus.GetCanvasRect();
    }
    public int GetCategory()
    {
        return m_TipEntity.Category;
    }
    public string GetTipsName()
    {
        return m_TipEntity.name;
    }
    /// <summary>
    /// レイヤー設定
    /// </summary>
    /// <param name="layerNo"></param>
    public void SetLayer(int layerNo)
    {
        m_MainObj.layer = layerNo;
        m_TipsCollision.transform.gameObject.layer = layerNo;

    }
    /// <summary>
    /// 描画優先順位
    /// </summary>
    /// <param name="BaseOrder"></param>
    /// <param name="BackOrder"></param>
    public void SetOrder(int baseOrder,int backOrder)
    {
        if(m_AddImg != null)
        {
            m_AddImg.sortingOrder = baseOrder;
        }
        if (m_BaseImg != null)
        {
            m_BaseImg.sortingOrder = baseOrder;
        }
        if (m_BackImg != null)
        {
            m_BackImg.sortingOrder = backOrder;
        }

    }
    public void SetActivateAddImage(bool enable)
    {
        if(m_AddImg!=null)
        {
            m_AddImg.transform.gameObject.SetActive(enable);
        }
    }
    public void SetImageAlphaColor(float alpha)
    {
        if (m_AddImg != null)
        {
            Color col = m_AddImg.color;
            col.a = alpha;
            m_AddImg.color = col;
        }
        if (m_BaseImg != null)
        {
            Color col = m_BaseImg.color;
            col.a = alpha;
            m_BaseImg.color = col;

        }
        if (m_BackImg != null)
        {
            Color col = m_BackImg.color;
            col.a = alpha;
            m_BackImg.color = col;
        }
    }
    /// <summary>
    /// ポーズ設定
    /// </summary>
    /// <param name="enable"></param>
    public void SetPause(bool enable)
    {
        Rigidbody2D reg2D = m_MainObj.GetComponent<Rigidbody2D>();
        if (enable == true)
        {
            reg2D.simulated = false;
        }
        else
        {
            reg2D.simulated = true;
        }

    }
    /// <summary>
    /// 現在選ばれているカテゴリによってカラーを変更する
    /// </summary>
    /// <param name="category"></param>
    /// <param name="my_category"></param>
    public void SetNowCategory(int category,string exist_name)
    {
        if (m_BackImg == null || m_BaseImg == null)
        {
            return;
        }
        if (category == -1 || m_State!=TipState.Area)
        {
            m_BackImg.color = Color.white;
            m_BaseImg.color = Color.white;
        }
        else
        {
            if(category== GetCategory())
            {
                if (m_TipEntity.name != exist_name)
                {
                    m_BackImg.color = Color.white;
                    m_BaseImg.color = Color.white;
                }
                else
                {
                    m_BackImg.color = Color.gray;
                    m_BaseImg.color = Color.gray;
                }
            }
            else
            {
                m_BackImg.color = Color.gray;
                m_BaseImg.color = Color.gray;
            }
        }
    }

    /// <summary>
    /// チップをエリアに配置
    /// </summary>
    /// <param name="picup_state"></param>
    private void SetAreaDropDown(TipState picup_state)
    {
        SetLayer(LayerMask.NameToLayer("UI"));
        SetOrder(5, 2);

        Rigidbody2D rig2d = m_MainObj.GetComponent<Rigidbody2D>();
        rig2d.isKinematic = false;

        m_TipsStatus.InitCombo();
        m_TipsStatus.SetAreaRemoveNextTips(m_MainObj, picup_state);
        m_TipsStatus.SetCategoryPickUp(null);
        m_TipsStatus.CombinationStart();//合成開始する

        m_State = TipState.Area;
    }

    public GameObject GetGameObject()
    {
        return m_MainObj;
    }

    /// <summary>
    /// コリンジョンにヒットしている数
    /// </summary>
    /// <returns></returns>
    public int GetCollisionCount()
    {
        return m_TipsCollision.m_LinkList.Count;
    }
    /// <summary>
    /// ヒットしているチップの情報
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public GameObject GetCollisionObj(int no)
    {
        return m_TipsCollision.m_LinkList[no].link_tips;
    }

    public void ClearCollisionObject()
    {
        m_TipsCollision.m_LinkList.Clear();
    }
    
    /// <summary>
    /// チップをクリックしたときの挙動
    /// </summary>
    public void OnPointerClick()
    {
        if (IsCtrlTips() == false) return;
        Vector3 mousePosition = Input.mousePosition;
        if (m_CanvasRect != null)
        {
            mousePosition.x = mousePosition.x * (m_CanvasRect.sizeDelta.x/Screen.width);
            mousePosition.y = mousePosition.y * (m_CanvasRect.sizeDelta.y/Screen.height);
        }
        m_StartPos = mousePosition;
        m_ClickPos = m_MainObj.transform.localPosition;

        Rigidbody2D rig2d = m_MainObj.GetComponent<Rigidbody2D>();
        rig2d.isKinematic = true;

        if (IsSellEnableTips()==true)
        {
            m_TipsStatus.SetSellItem();
        }

        if (IsDecideTips() == true)
        {
            m_TipsStatus.SetCurrentTips(m_MainObj);
        }

    }
    /// <summary>
    /// チップを配置したときの挙動
    /// </summary>
    public void OnPointerUp()
    {
        if (IsCtrlTips() == false) return;
        if (IsTipTaps(m_TipsStatus.GetNextBoxObj()) == true)
        {
            Rigidbody2D rig2d = m_MainObj.GetComponent<Rigidbody2D>();
            rig2d.isKinematic = false;
            return;
        }


        SetOrder(5, 2);

        if (IsDecideTips() == false)
        {
            m_TipsStatus.SetCategoryPickUp(null);
            m_bDrag = false;
            return;
        }

        SetAreaDropDown(m_State);


        m_bDrag = false;
    }
    /// <summary>
    /// チップを移動させた時の挙動
    /// </summary>
    public void OnPointerDrag()
    {
        if (IsCtrlTips() == false) return;
        float x_pos = 1.0f;
        if (IsDecideTips() == false)
        {
            x_pos = 0.1f;
        }
        Vector3 mousePosition = Input.mousePosition;
        if (m_CanvasRect != null)
        {
            mousePosition.x = mousePosition.x * (m_CanvasRect.sizeDelta.x / Screen.width);
            mousePosition.y = mousePosition.y * (m_CanvasRect.sizeDelta.y / Screen.height);
        }
        m_MainObj.transform.localPosition = ((mousePosition - m_StartPos) * x_pos) + m_ClickPos;
        m_TipsStatus.SetCategoryPickUp(this);
        SetOrder(8, 7);
        m_bDrag = true;
    }
    public void OnPointerEnter()
    {
        print("pointer enter\n");
    }

}
