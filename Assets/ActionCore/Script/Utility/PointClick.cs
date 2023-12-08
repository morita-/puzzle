using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PointClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject m_clickParticleObj = null;
    [SerializeField] private GameObject m_dragParticleObj = null;
    [SerializeField] float m_decTimerSt = 1.0f;
    [SerializeField] float m_decTimer = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickPos(eventData.position);
    }
    public void OnClickPos(Vector2 pos)
    {

        OnClick();
        if (m_clickParticleObj != null)
        {
            //クリックエフェクトをCanvas上に表示
            StartCoroutine(ClickEffect(pos, m_clickParticleObj));
        }
    }
    public void OnClick()
    {
        PlaySound.PlayMenuSE("common_click");

    }

    public void OnDragPos(Vector2 pos)
    {
        if(m_dragParticleObj != null)
        {
            m_decTimer -= Time.deltaTime;
            if (m_decTimer < 0)
            {
                StartCoroutine(ClickEffect(pos, m_dragParticleObj));
                m_decTimer = m_decTimerSt;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 click_pos = Input.mousePosition;
            OnClickPos(click_pos);
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 click_pos = Input.mousePosition;
            OnDragPos(click_pos);
        }

    }
    IEnumerator ClickEffect(Vector2 pos,GameObject effectObj)
    {
        // Canvasにセットされているカメラを取得
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        var camera = canvas.worldCamera;

        // Overlayの場合はScreenPointToLocalPointInRectangleにnullを渡さないといけないので書き換える
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            camera = null;
        }

        // クリック位置に対応するRectTransformのlocalPositionを計算する
        var localPoint = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pos, camera, out localPoint);


        GameObject gameObj = GameObject.Instantiate(effectObj, this.transform);
        gameObj.transform.localPosition = localPoint;

        ParticleSystem particle = gameObj.GetComponent<ParticleSystem>();
        while (particle.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObj);
    }
}
