using UnityEngine;
using UnityEngine.Android;

namespace ChiliGames
{
    public class AskForMic : MonoBehaviour
    {
        void Start()
        {
    #if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
    #endif
            string mic = Microphone.devices[0];

        }

}
}
