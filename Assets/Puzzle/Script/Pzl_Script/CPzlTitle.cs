using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPzlTitle : MonoBehaviour
{
    [SerializeField] private GameObject m_TitleCanvasObj;
    [SerializeField] private GameObject m_ParentObj;
    [SerializeField] private Text m_VerText;

    private GameObject[] m_bk_prefab;
    private List<GameObject> m_bk_list;

    private bool m_bStartGame;

    // Start is called before the first frame update
    void Start()
    {
        m_bk_prefab = new GameObject[3];
        m_bk_list = new List<GameObject>();
        m_bStartGame = false;
        m_VerText.text = string.Format("ver {0:0.00}", GlobalObjectPzl.m_GlobalParam.VERSION);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActivateMenu(bool enable)
    {
        m_TitleCanvasObj.SetActive(enable);
    }

    public bool IsStartGame()
    {
        return m_bStartGame;
    }

    public void OnButtoneClick()
    {
        PlaySound.PlayMenuSE("common_dicide");
        m_bStartGame = true;
    }

    public void CreateLoadBooks()
    {
        //プレハブを取得
        string[] tip_name = new string[m_bk_prefab.Length];

        tip_name[0] = string.Format("_Prefab/Pzl_Tip/Tip_9_{0}", "Book_n");
        m_bk_prefab[0] = (GameObject)Resources.Load(tip_name[0]);

        tip_name[1] = string.Format("_Prefab/Pzl_Tip/Tip_9_{0}", "Book_e");
        m_bk_prefab[1] = (GameObject)Resources.Load(tip_name[1]);

        tip_name[2] = string.Format("_Prefab/Pzl_Tip/Tip_9_{0}", "Book_o");
        m_bk_prefab[2] = (GameObject)Resources.Load(tip_name[2]);


        for(int cnt = 0; cnt < GlobalObjectPzl.m_GlobalParam.TITLE_BOOKS_NUM; cnt++)
        {
            int typeNo = (int)Random.Range(0, 3);
            if (typeNo >= 3) typeNo = 0;

            GameObject gameObj = Instantiate(m_bk_prefab[typeNo], new Vector3(0, 0, 0), Quaternion.identity,m_ParentObj.transform);


            Vector3 pos = gameObj.transform.localPosition;
            pos.y = 1000+Random.Range(-150, 0);
            pos.x = 150-300*(cnt%2)+Random.Range(-50,50);
            pos.z = 0;

            Vector3 scale = gameObj.transform.localScale;
            scale.x = GlobalObjectPzl.m_GlobalParam.TITLE_BOOKS_SCALE;
            scale.y = GlobalObjectPzl.m_GlobalParam.TITLE_BOOKS_SCALE;
            scale.z = GlobalObjectPzl.m_GlobalParam.TITLE_BOOKS_SCALE;

            gameObj.transform.localPosition = pos;
            gameObj.transform.localScale = scale;
            CPzlTips pzlTip = gameObj.GetComponentInChildren<CPzlTips>();

            pzlTip.SetLayer( LayerMask.NameToLayer("TitleTips") );

            Rigidbody2D regd2d = gameObj.GetComponent<Rigidbody2D>();
            regd2d.isKinematic = false;

            m_bk_list.Add(gameObj);
        }

        m_bStartGame = false;

    }
    public void ReleaseBooks()
    {
        for(int cnt = 0; cnt < m_bk_list.Count; cnt++)
        {
            Destroy(m_bk_list[cnt]);
        }
        m_bk_list.Clear();
    }
}
