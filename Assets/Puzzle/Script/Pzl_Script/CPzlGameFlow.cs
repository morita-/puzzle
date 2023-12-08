using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPzlGameFlow : MonoBehaviour
{
    //ステージをまたいだデータ設定を保持
    private List<Pzl_StageParamsEntity> m_StageList;

    private int m_nowStageNo;
    private Pzl_StageParamsEntity m_nowStageParam;

    //ステータス情報
    [SerializeField] private CPzlStatus m_StageStatus;
    [SerializeField] private CPzlTitle m_StageTitle;
    [SerializeField] private CPzlGameOver m_StageGameOver;
    [SerializeField] private CPzlMapGame m_NextStageMap;

    //フローステータス
    public enum enGameFlow{
        Null,
        TitleInit,
        TitleUpdate,
        TitleEnd,
        Init,
        Start,
        Update,
        End,
        Ending,
        NextStage,
        NextStageNextCheck,
        NextStageNextSet,
        GameOverInit,
        GameOverUpdate,
        GameOverRestart,
        GameOverContinue,
        Reset,
        ResetChk,

    }

    enGameFlow m_GameFlow;
    bool m_AdvertizePause;

    // Start is called before the first frame update
    void Start()
    {
        m_StageList = GlobalObjectPzl.m_StageParam.Map0;
        m_NextStageMap.InitMapTipBaseData();
        m_NextStageMap.SetAllStegeMapTips(m_StageList);
        m_NextStageMap.SetNextStage(0, -1);

        SetStageStart(0);
        m_GameFlow = enGameFlow.TitleInit;
        m_AdvertizePause = false;
    }

    void SetStageStart(int nowStageNo)
    {
        m_nowStageNo = nowStageNo;
        m_nowStageParam = GlobalObjectPzl.m_StageParam.GetStageParam(m_nowStageNo);
        m_StageStatus.SetStageEntity(m_nowStageParam);
        m_GameFlow = enGameFlow.Reset;
    }



    // Update is called once per frame
    void Update()
    {
        switch (m_GameFlow)
        {
            case enGameFlow.TitleInit:
                {
                    m_StageTitle.SetActivateMenu(true);
                    m_StageTitle.CreateLoadBooks();
                    m_GameFlow = enGameFlow.TitleUpdate;
                    PlaySound.PlayBGM("title");
                }
                break;
            case enGameFlow.TitleUpdate:
                if (m_StageTitle.IsStartGame())
                {
                    m_GameFlow = enGameFlow.TitleEnd;
                }
                break;
            case enGameFlow.TitleEnd:
                {
                    if (m_StageStatus.m_bStart == false)
                    {
                        break;
                    }
                    m_StageTitle.ReleaseBooks();
                    m_StageTitle.SetActivateMenu(false);
                    m_GameFlow = enGameFlow.Init;

//                    m_StageStatus.InitStage();
//                    m_GameFlow = enGameFlow.Start;
                }
                break;
            case enGameFlow.Init:
                {
                    if (m_StageStatus.m_bStart == false)
                    {
                        break;
                    }
                    PlaySound.PlayBGM("main");
                    m_StageStatus.InitStage();
                    m_GameFlow = enGameFlow.Start;
                }
                break;
            case enGameFlow.Start:
                {
                    m_StageStatus.StartStage();
                    m_GameFlow = enGameFlow.Update;
                }
                break;
            case enGameFlow.Update:
                {
                    if (m_AdvertizePause != m_StageStatus.IsGamePause())
                    {
                        m_StageStatus.SetPause(m_AdvertizePause);
                    }
                    enGameFlow ret = m_StageStatus.UpdateStage();
                    if (ret != enGameFlow.Null)
                    {
                        m_GameFlow = ret;
                    }
                }
                break;
            case enGameFlow.End:
                {
                    m_NextStageMap.SetActivateMenu(true);
                    m_NextStageMap.SetNextStage(m_nowStageNo + 1, m_nowStageNo);
                    m_NextStageMap.SetStartEnding();
                    m_GameFlow = enGameFlow.Ending;
                }
                break;
            case enGameFlow.Ending:
                {

                }
                break;
            case enGameFlow.NextStage:
                {
                    if (true)
                    {
                        //金銭徴収の為、Nextステージ画面へ
                        m_StageStatus.SetPause(true);
                        m_NextStageMap.SetActivateMenu(true);
                        m_NextStageMap.SetMapNowMonney(m_StageStatus.GetNowMonney());
                        m_NextStageMap.SetNextStage(m_nowStageNo + 1, m_nowStageNo);
                        m_GameFlow = enGameFlow.NextStageNextCheck;
                        PlaySound.PlayBGM("next");
                    }
                    else
                    {
                        //強制的に次のステージへ
                        SetStageStart(m_nowStageNo + 1);
                        m_GameFlow = enGameFlow.Reset;
                    }
                }
                break;
            case enGameFlow.NextStageNextCheck:
                {
                    if (m_NextStageMap.IsCheck(m_AdvertizePause) ==true)
                    {
                        m_StageStatus.SetPause(false);
                        m_NextStageMap.SetActivateMenu(false);
                        m_StageStatus.SetNowMonney(m_NextStageMap.GetMapNowMonney());
                        SetStageStart(m_nowStageNo + 1);
                        m_GameFlow = enGameFlow.Reset;
                    }
                }
                break;
            case enGameFlow.NextStageNextSet:
                {
                    //再度ページをスタートする
                    SetStageStart(m_nowStageNo);
                    m_GameFlow = enGameFlow.Reset;
                }
                break;
            case enGameFlow.GameOverInit:
                {
                    m_StageGameOver.SetActivateMenu(true);
                    m_StageGameOver.InitGameOver(m_StageStatus.GetNowMonney());
                    m_GameFlow = enGameFlow.GameOverUpdate;
                    PlaySound.PlayBGM("result_lose");
                }
                break;
            case enGameFlow.GameOverUpdate:
                {
                    enGameFlow ret = m_StageGameOver.UpdateGameOver();
                    if (ret != enGameFlow.Null)
                    {
                        m_GameFlow = ret;
                    }
                }
                break;
            case enGameFlow.GameOverRestart:
                {
                    m_StageStatus.SetRestartMonney();
                    m_GameFlow = enGameFlow.Reset;
                }
                break;
            case enGameFlow.GameOverContinue:
                {
                    //続きから
                    m_StageStatus.SetNowMonney(m_StageGameOver.GetGameOverNowMonney());
                    m_StageStatus.SetDecGauge(GlobalObjectPzl.m_GlobalParam.CONTINUE_DEC_GAUGE_RATE);
                    m_StageStatus.SetPause(false);
                    m_StageGameOver.SetActivateMenu(false);
                    m_GameFlow = enGameFlow.Update;
                }
                break;
            case enGameFlow.Reset:
                {
                    //最初から
                    m_StageStatus.ResetStage();
                    m_StageGameOver.SetActivateMenu(false);
                    m_StageStatus.SetPause(false);
                    m_GameFlow = enGameFlow.ResetChk;
                }
                break;
            case enGameFlow.ResetChk:
                {
                    if (m_StageStatus.IsReset())
                    {
                        m_GameFlow = enGameFlow.Init;
                    }
                }
                break;
            default:break;
        }


    }
    public void SetGameAdvertizePause(bool enable)
    {
        m_AdvertizePause = enable;

    }
}
