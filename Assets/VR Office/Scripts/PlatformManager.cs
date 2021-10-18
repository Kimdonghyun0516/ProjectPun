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
    //This script handles the different modes: VR or Screen (pc, tablet, phone, etc)
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

        //Seats
        Hashtable h = new Hashtable();
        bool initialized;
        bool seated;
        int actorNum;

        //Modes
        public enum Mode { VR, Screen };
        [Tooltip("Choose the mode before building")]
        public Mode mode;

        //For setting color in VRBody.cs
        public UnityEvent onSpawned;
        [HideInInspector] public int spawnPosIndex;

        //Singleton to access this script from everywhere.
        public static PlatformManager instance;

        void Awake()
        {
            //If not connected go to lobby to connect
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("IsConnected False");
            }
            instance = this;
            actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

            //If student connecting from phone, limit the fps to save battery. Also avoid sleep.
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

            //if this is the first player to connect, initialize the students list
            if (PhotonNetwork.IsMasterClient && !initialized)
            {
                InitializePositionsList();
            }


            //if this is the teacher, activate its rig and create the body
            if (mode == Mode.VR)
            {
                vrRig.SetActive(true);
                CreateVRBody();
                if (PhotonNetwork.CurrentRoom.CustomProperties["Initialized"] != null)
                {
                    SetPosition(GetFreePosition());
                }
            }
            //if it's a student, create it's body and sit in right position if the student list already exists
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

        /*        public void SetMaleAvatar()
                {
                    photonView.RPC("ChangeTeacherAvatar", RpcTarget.AllBuffered, "male");
                    localVrBody.GetComponent<PhotonView>().RPC("SetAvatarFollow", RpcTarget.AllBuffered);
                }

                public void SetFemaleAvatar()
                {
                    photonView.RPC("ChangeTeacherAvatar", RpcTarget.AllBuffered, "female");
                    localVrBody.GetComponent<PhotonView>().RPC("SetAvatarFollow", RpcTarget.AllBuffered);
                }*/
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
            //Debug.Log("mute : " + btnBoard.GetComponent<Text>());

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


        //So we stop loading scenes if we quit app
        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

        //This creates an empty list of positions matching the number of spawn positions
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

        //Gets the first sit that is free (that has a value of 0)
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

        //Puts the user in the correspondant spawning position
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

            //Store in room properties what actor number was set in the position
            h["" + n] = actorNum;
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);

            spawnPosIndex = n;
            onSpawned.Invoke();
        }

        //This is called when the room properties are updated, for example, when the positions list is created
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

        //This is called when a player leaves the room, so we can free the student's place
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            //get the seat number of plaer that left the room, and update room properties with the free seat (value to 0)
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

        //If the new master client is this client, get a copy of the room properties
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            if (newMasterClient.ActorNumber == actorNum)
            {
                h = PhotonNetwork.CurrentRoom.CustomProperties;
            }
        }

        //If disconnected from server, return to lobby to reconnect.
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            GoToScene(0);
        }

        //Class to load scenes async
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
            if (n == 0) //if going back to menu destroy instance
            {
                Destroy(gameObject);
            }
        }
    }
}
