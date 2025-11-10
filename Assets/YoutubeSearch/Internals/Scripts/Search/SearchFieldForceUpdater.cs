
using UdonSharp;
using VRC.SDK3.Components;

namespace Rinvo{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SearchFieldForceUpdater : UdonSharpBehaviour
    {
        public VRCUrlInputField inputField;
        public YoutubeSearchManager searchManager;


        void OnEnable(){
            inputField.SetUrl(searchManager.GetDefaultUrl());
        }
    }
}