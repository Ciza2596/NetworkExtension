using Mirror;
using UnityEngine;

namespace CizaMirrorExtension.Example
{
    public class PlayerCountHandler : MonoBehaviour
    {
        [SerializeField]
        private int _playerCount;

        [SerializeField]
        private NetworkManager _networkManager;

        void Update()
        {
            _playerCount = _networkManager.numPlayers;
        }
    }
}