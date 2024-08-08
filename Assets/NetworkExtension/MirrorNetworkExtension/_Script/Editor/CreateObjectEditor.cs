using UnityEditor;
using UnityEngine;

namespace CizaMirrorNetworkExtension.Editor
{
    public static class CreateObjectEditor
    {
        public const string MirrorExtensionPath = "MirrorNetworkExtension/";

        public const string MirrorNetworkManager = "MirrorNetworkManager";

        [MenuItem("GameObject/Ciza/Network/MirrorNetworkManager", false, -10)]
        public static void CreateMirrorNetworkManager()
        {
            CreateObject(MirrorNetworkManager);
        }

        public const string MirrorNetworkPlayer = "MirrorNetworkPlayer";

        [MenuItem("GameObject/Ciza/Network/MirrorNetworkPlayer", false, -10)]
        public static void CreateMirrorNetworkPlayer()
        {
            CreateObject(MirrorNetworkPlayer);
        }

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(MirrorExtensionPath + dataId);
            var obj = Object.Instantiate(prefab, Selection.activeTransform);
            obj.name = dataId;
        }
    }
}