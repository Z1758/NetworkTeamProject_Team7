using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class TestGameScene : MonoBehaviourPunCallbacks
{
    private static TestGameScene instance;


    public static TestGameScene Instance 
    {
        get
        {
            return instance;

        }
    }

    public const string RoomName = "TestRoom";
    [SerializeField] GameObject characterSelectUI;
    [SerializeField] GameObject startPoint;
    [SerializeField] Transform[] endPoint;
    [SerializeField] GameObject resultCamera;
    [SerializeField] GameObject uiCanvas;
 
    public List<PlayerController> players = new List<PlayerController>();

    [SerializeField] int monsterCount;
     List<int> monsterPrefabsNumber = new List<int>();
    Queue<int> monsterOrderQueue = new Queue<int>();

    [SerializeField] TimelineBoss timeline;

    GameObject currentBoss;
 
    public int readyPlayer = 0;
    public int currentStage;

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
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            BossSpawn();
        }
      
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();

        for (int i = 0; i < monsterCount; i++) {
            monsterPrefabsNumber.Add(i+1);
        }



    }

    // 모든 플레이어가 로딩 완료 됐을때 호출 하도록 변경
    private void SetMonster()
    {
        // 랜덤 순서 초기화
       
        if (photonView.IsMine)
        {
            
            while (monsterPrefabsNumber.Count >0)
            {


                int ran = Random.Range(0, monsterPrefabsNumber.Count);
                
                photonView.RPC(nameof(SetMonsterOrder), RpcTarget.All, ran);



            }
        }

       
    }

    [PunRPC]
    public void SetMonsterOrder(int num)
    {
        monsterOrderQueue.Enqueue(monsterPrefabsNumber[num]);
        monsterPrefabsNumber.Remove(monsterPrefabsNumber[num]);
    }



    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }



    public override void OnJoinedRoom()
    {
        StartCoroutine(StartDelayRoutine());
    }

    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f); // 네트워크 준비에 필요한 시간 살짝 주기
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("게임 시작");
        characterSelectUI.SetActive(true);
    

        // 방장만 진행하는 코드

        if (PhotonNetwork.IsMasterClient == false)
            return;

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
           
        }
    }
  
   
    public void PlayerSpawn(int num)
    {
      
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

   

        PhotonNetwork.Instantiate($"GameObject/Player{num}", randomPos, Quaternion.identity);

        characterSelectUI.SetActive(false);
        
    }

    private void BossSpawn()
    {

       
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));


        PhotonNetwork.InstantiateRoomObject("GameObject/Boss6", randomPos, Quaternion.identity);

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

    public void ClearBoss(GameObject obj)
    {
     
        AudioManager.GetInstance().StopBGM();
       
        currentBoss = obj;


        if (currentStage < 5)
        {
            WHS_ItemManager.Instance.SpawnChest(obj.transform.position);
            startPoint.SetActive(true);
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
        for (int i = 0; i<players.Count ; i++)
        {
            players[i].Victory(endPoint[i]);

        }
        if (currentBoss is not null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }

        uiCanvas.SetActive(false);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayer++;

            //임시 방편
            if (monsterPrefabsNumber.Count > 0)
            {
                if(photonView.IsMine)
                    SetMonster();
            }
            Debug.Log($"준비 {readyPlayer}/{PhotonNetwork.PlayerList.Count()} ");

            // 스테이지 시작 호출
            if(readyPlayer >= PhotonNetwork.PlayerList.Count())
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
