using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System.Collections;
using UnityEngine.Events;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using Photon.Pun.LobbySystemPhoton;


namespace ChiliGames.VROffice
{
    public class PlatformManager : MonoBehaviourPunCallbacks
    {
        public string nameText1;
        GameObject player1;
        bool muteon = false;
        public Text mutebtn;
        [SerializeField] GameObject whiteboard;
        bool boardon = false;
        public Text btnBoard;

        [SerializeField] GameObject simpleCamera;

        [SerializeField] GameObject vrRig;
        public GameObject screenRig;
        [SerializeField] Transform[] startingPositions;
        [SerializeField] TeleportationArea floor;

        [SerializeField] private GameObject vrBody;
        private VRBody localVrBody;

        [HideInInspector] public Transform[] vrRigParts;
        public Transform[] screenRigParts;

        [SerializeField] GameObject screenBody;

        Hashtable h = new Hashtable();
        bool initialized;
        bool seated;
        int actorNum;

        public enum Mode { VR, Screen };
        [Tooltip("Choose the mode before building")]
        public Mode mode;

        public UnityEvent onSpawned;
        [HideInInspector] public int spawnPosIndex;

        public static PlatformManager instance;

        void Awake()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("IsConnected False");
            }
            instance = this;
            actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

            if (mode == Mode.Screen)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            vrRigParts = new Transform[3];
            vrRigParts[0] = vrRig.transform.GetChild(0).GetChild(0); //Set camera
            vrRigParts[1] = vrRig.transform.GetChild(0).GetChild(1); //Set left hand
            vrRigParts[2] = vrRig.transform.GetChild(0).GetChild(2); //Set right hand

            floor.teleportationProvider = vrRig.GetComponent<TeleportationProvider>();
        }

        private void Start()
        {

            string playerName = Template.instance.PlayerNameInput.text;
            nameText1 = playerName;

            if (PhotonNetwork.IsMasterClient && !initialized)
            {
                InitializePositionsList();
            }


            if (mode == Mode.VR)
            {
                vrRig.SetActive(true);
                CreateVRBody();
                if (PhotonNetwork.CurrentRoom.CustomProperties["Initialized"] != null)
                {
                    SetPosition(GetFreePosition());
                }
            }
            else if (mode == Mode.Screen)
            {
                screenRig.SetActive(true);
                CreateScreenBody();
                if (PhotonNetwork.CurrentRoom.CustomProperties["Initialized"] != null)
                {
                    SetPosition(GetFreePosition());
                }
            }
        }

        void CreateVRBody()
        {
            player1 = PhotonNetwork.Instantiate(vrBody.name, transform.position, transform.rotation);
            localVrBody = player1.GetComponent<VRBody>();

            localVrBody.SendMessage("Setnickname", nameText1,
                 SendMessageOptions.DontRequireReceiver);
        }

        void CreateScreenBody()
        {
            Debug.Log("Delivery : " + nameText1);
            player1 = PhotonNetwork.Instantiate
                (screenBody.name, transform.position, transform.rotation, 0, new object[] 
                { nameText1 });
            player1.SendMessage("Setnickname", nameText1,
                 SendMessageOptions.DontRequireReceiver);
        }
        public void MuteClicked()
        {
            muteon = !muteon;

            localVrBody.SendMessage("Muteplayer", muteon,
                 SendMessageOptions.DontRequireReceiver);

            
            Debug.Log("mute" + mutebtn);
            Debug.Log("mute : " + mutebtn.GetComponent<Text>());

            if (muteon == true)
            {
                mutebtn.GetComponent<Text>().text = "Mute";
            }
            else
            {
                mutebtn.GetComponent<Text>().text = "Speaking";
            }

        }

        public void WhiteBoardClicked()
        {
            boardon = !boardon;
            
            whiteboard.SendMessage("WhiteBoardstart", boardon,
                 SendMessageOptions.DontRequireReceiver);
            simpleCamera.SendMessage("WhiteBoardstart", boardon,
                 SendMessageOptions.DontRequireReceiver);

            Debug.Log("Board" + btnBoard);

            if (boardon == true)
            {
                btnBoard.GetComponent<Text>().text = "WhiteBoardOn";
            }
            else
            {
                btnBoard.GetComponent<Text>().text = "WhiteBoardOff";
            }

        }


        public void TeleportEffect()
        {
            if (localVrBody != null)
            {
                localVrBody.GetComponent<PhotonView>().RPC("RPC_TeleportEffect", RpcTarget.Others);
            }
        }


        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

        void InitializePositionsList()
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["Initialized"] == null)
            {
                h.Add("Initialized", actorNum);
                for (int i = 0; i < startingPositions.Length; i++)
                {
                    h.Add("" + i, 0);
                }
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);
        }

        int GetFreePosition()
        {
            for (int i = 0; i < startingPositions.Length; i++)
            {
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties["" + i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        void SetPosition(int n)
        {
            if (n == -1)
            {
                Debug.LogError("No positions available");
            }
            Debug.Log("Spawning user in position number: " + n);
            if (mode == Mode.VR)
            {
                vrRig.transform.position = startingPositions[n].position;
                vrRig.transform.rotation = startingPositions[n].rotation;
            }
            else if (mode == Mode.Screen)
            {
                screenRig.transform.position = startingPositions[n].position;
                screenRig.transform.rotation = startingPositions[n].rotation;
            }
            seated = true;

            h["" + n] = actorNum;
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);

            spawnPosIndex = n;
            onSpawned.Invoke();
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (propertiesThatChanged.ContainsKey("Initialized") && !initialized)
            {
                Debug.Log("Positions list initialized");
                initialized = true;

                if (!seated)
                {
                    SetPosition(GetFreePosition());
                }
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < startingPositions.Length; i++)
                {
                    if ((int)PhotonNetwork.CurrentRoom.CustomProperties["" + i] == otherPlayer.ActorNumber)
                    {
                        h["" + i] = 0;
                        PhotonNetwork.CurrentRoom.SetCustomProperties(h);
                        Debug.Log("User " + otherPlayer.ActorNumber + " left room, freeing up seat " + i);
                        return;
                    }
                }
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            if (newMasterClient.ActorNumber == actorNum)
            {
                h = PhotonNetwork.CurrentRoom.CustomProperties;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.Log("OnDisconnected121212");
            Connexion.Fromoffice = true;
            GoToScene(0);
        }

        void GoToScene(int n)
        {
            StartCoroutine(LoadScene(n));
        }

        IEnumerator LoadScene(int n)
        {
            yield return new WaitForSeconds(0.5f);

            AsyncOperation async = SceneManager.LoadSceneAsync(n);
            async.allowSceneActivation = false;

            yield return new WaitForSeconds(1);
            async.allowSceneActivation = true;
            if (n == 0) 
            {
                Destroy(gameObject);
            }
        }
    }
}
