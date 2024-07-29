using UnityEditor;
using UnityEngine;

namespace CizaMirrorExtension.Editor
{
    public static class CreateObjectEditor
    {
        public const string MirrorExtensionPath = "MirrorExtension/";

        public const string MirrorNetworkManager = "MirrorNetworkManager";

        [MenuItem("GameObject/Ciza/Network/MirrorNetworkManager", false, -10)]
        public static void CreateMirrorNetworkManager()
        {
            CreateObject(MirrorNetworkManager);
        }

        private static void CreateObject(string dataId)
        {
            var prefab = Resources.Load<GameObject>(MirrorExtensionPath + dataId);
            var obj = Object.Instantiate(prefab, Selection.activeTransform);
            obj.name = dataId;
        }
    }
}