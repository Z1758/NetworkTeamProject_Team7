using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

using Photon.Realtime;
using System;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class GameScene : MonoBehaviourPunCallbacks
{
    [SerializeField] Button gameOverButton;
    [SerializeField] TMP_Text countDownText;


    private static GameScene instance;

    [SerializeField] GameObject characterSelectUI;
    [SerializeField] GameObject startPoint;
    [SerializeField] Transform[] endPoint;
    [SerializeField] GameObject resultCamera;
    [SerializeField] GameObject uiCanvas;
    [SerializeField] GameObject resultCanvas;

    // 최대 몬스터 수
    [SerializeField] int monsterCount;

    public List<PlayerController> players = new List<PlayerController>();

 
    List<int> monsterPrefabsNumber = new List<int>();
    Queue<int> monsterOrderQueue = new Queue<int>();

    [SerializeField] TimelineBoss timeline;

    GameObject currentBoss;

    public int readyPlayer = 0;
    public int currentStage;

    public static GameScene Instance
    {
        get
        {
            return instance;

        }
    }

    private void Awake()
    {
        
        if (instance == null)
        {

            instance = this;

        }
        else
        {
            Destroy(this);
        }

        Time.timeScale = 1.0f;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            
            if (resultCanvas.activeSelf)
            {
              
                Cursor.lockState = CursorLockMode.Locked;

                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;

                Cursor.visible = true;
            }

            resultCanvas.SetActive(!resultCanvas.activeSelf);

        }
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);
        StartCoroutine(StartDelayRoutine());
    }


    Coroutine setMonsterCoroutine;

    IEnumerator StartDelayRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return waitForSeconds; // 네트워크 준비에 필요한 시간 살짝 주기

            bool allLoaded = CheckAllLoad();
            Debug.Log($"모든 플레이어가 로딩 완료되었는가 : {allLoaded}");
            if (allLoaded)
            {
                if (setMonsterCoroutine == null)
                {
                    setMonsterCoroutine = StartCoroutine(SetMonsterDelay());

                    break;
                }
            }
        }
       

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetGameOverButton();
    }

    public void SetGameOverButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameOverButton.interactable = true;
        }
        else
        {
            gameOverButton.interactable = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    public void GameOver()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        Time.timeScale = 1.0f;
        PhotonNetwork.DestroyAll();
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }

    public void LeaveRoom()
    {
        //  PhotonNetwork.DestroyAll();
        Time.timeScale = 1.0f;
        PhotonNetwork.LeaveRoom();
    }



   
    private void SetMonster()
    {
        for (int i = 0; i < monsterCount; i++)
        {
            monsterPrefabsNumber.Add(i + 1);
        }



        Debug.Log("셋업");
        // 랜덤 순서 초기화

        if (photonView.IsMine)
        {

            while (monsterPrefabsNumber.Count > 0)
            {


                int ran = UnityEngine.Random.Range(0, monsterPrefabsNumber.Count);

                photonView.RPC(nameof(SetMonsterOrder), RpcTarget.All, ran);



            }
        }


    }

    IEnumerator SetMonsterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        SetMonster();
        characterSelectUI.SetActive(true);
    }

    [PunRPC]
    public void SetMonsterOrder(int num)
    {
        monsterOrderQueue.Enqueue(monsterPrefabsNumber[num]);
        monsterPrefabsNumber.Remove(monsterPrefabsNumber[num]);
    }


    // 버튼 연동
    public void PlayerSpawn(int num)
    {
        
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, UnityEngine.Random.Range(-5f, 5f));



        PhotonNetwork.Instantiate($"GameObject/Player{num}", randomPos, Quaternion.identity);


        characterSelectUI.SetActive(false);

    }


    private bool CheckAllLoad()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
           
            if (player.GetLoad() == false)
            {
                
                return false;   
            }

          
        }
        return true;
    }



    public void StartStage()
    {
        readyPlayer = 0;
        currentStage++;
        startPoint.SetActive(false);
        if (currentBoss is not null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }
        if (monsterOrderQueue.Count > 0)
        {
          
            int orderNum = monsterOrderQueue.Dequeue();
            AudioManager.GetInstance().PlayBGM(currentStage);
            timeline.StartTimeline(orderNum);

        }
    }



    IEnumerator DelayRemoveBoss()
    {
        yield return new WaitForSecondsRealtime(7.0f);
        Vector3 vec = currentBoss.transform.position;

        vec.x = UnityEngine.Random.Range(-7f, 7f);
        vec.z = UnityEngine.Random.Range(-7f, 7f);
   
        WHS_ItemManager.Instance.SpawnChest(vec);
        RemoveBoss();
        startPoint.SetActive(true);
    }

    void RemoveBoss()
    {
        if (currentBoss is not null)
        {
            Destroy(currentBoss);
            currentBoss = null;

        }

    }
    public void ClearBoss(GameObject obj)
    {

        AudioManager.GetInstance().StopBGM();

        currentBoss = obj;



        if (currentStage < 5)
        {

            StartCoroutine(DelayRemoveBoss());
        }
        else
        {

            Debug.Log("클리어");
            GameClear();
        }
    }

    public void GameClear()
    {
        AudioManager.GetInstance().PlayClearBGM();
        resultCamera.SetActive(true);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Victory(endPoint[i]);

        }
        RemoveBoss();


        uiCanvas.SetActive(false);
        OnResultButton();

      


    }

    public void OnResultButton()
    {
        resultCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayer++;
            /*
            //임시 방편
            if (monsterPrefabsNumber.Count > 0)
            {
                if (photonView.IsMine)
                    SetMonster();
            }
            */
            Debug.Log($"준비 {readyPlayer}/{PhotonNetwork.PlayerList.Count()} ");

            // 스테이지 시작 호출
            if (readyPlayer >= PhotonNetwork.PlayerList.Count())
            {
                StartStage();

            }


        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayer--;



            Debug.Log($"준비 {readyPlayer}/{PhotonNetwork.PlayerList.Count()} ");
        }



    }

    private void OnDisable()
    {
        readyPlayer = 0;
    }

   
}
