using CizaMirrorExtension.Implement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CizaMirrorExtension.Example
{
    public class MirrorCreateRoom : MonoBehaviour
    {
        [SerializeField]
        private GameObject _start;

        [SerializeField]
        private Button _hostButton;

        [SerializeField]
        private Button _clientButton;

        [Space]
        [SerializeField]
        private GameObject _stop;

        [SerializeField]
        private Button _stopButton;

        [Space]
        [SerializeField]
        private TMP_Text _playerCountText;


        [Space]
        [SerializeField]
        private MirrorNetworkHandlerConfig _mirrorNetworkHandlerConfig;

        public MirrorNetworkHandler MirrorNetworkHandler { get; private set; }

        public void OnEnable()
        {
            MirrorNetworkHandler = new MirrorNetworkHandler(_mirrorNetworkHandlerConfig);
            MirrorNetworkHandler.Initialize();

            _hostButton.onClick.AddListener(OnHostButtonClick);
            _clientButton.onClick.AddListener(OnClientButtonClick);

            _stopButton.onClick.AddListener(OnStopButtonClick);

            MirrorNetworkHandler.OnConnect += OnConnected;
            MirrorNetworkHandler.OnDisconnect += OnDisconnected;

            OnDisconnected(0);
        }

        public void OnDisable()
        {
            _hostButton.onClick.RemoveListener(OnHostButtonClick);
            _clientButton.onClick.RemoveListener(OnClientButtonClick);

            _stopButton.onClick.RemoveListener(OnStopButtonClick);

            MirrorNetworkHandler.OnConnect -= OnConnected;
            MirrorNetworkHandler.OnDisconnect -= OnDisconnected;

            MirrorNetworkHandler.Release();
            MirrorNetworkHandler = null;
        }

        private void LateUpdate()
        {
            if (MirrorNetworkHandler == null)
                return;

            _playerCountText.text = MirrorNetworkHandler.PlayerCount.ToString();
            MirrorNetworkHandler.Tick(Time.deltaTime);
        }

        private void OnHostButtonClick()
        {
            MirrorNetworkHandler.StartHost();
        }

        private void OnClientButtonClick()
        {
            MirrorNetworkHandler.StartClient();
        }

        private void OnStopButtonClick()
        {
            MirrorNetworkHandler.StopHost();
        }

        private void OnConnected(int playerIndex)
        {
            _start.SetActive(false);
            _stop.SetActive(true);
        }

        private void OnDisconnected(int playerIndex)
        {
            _start.SetActive(true);
            _stop.SetActive(false);
        }
    }
}