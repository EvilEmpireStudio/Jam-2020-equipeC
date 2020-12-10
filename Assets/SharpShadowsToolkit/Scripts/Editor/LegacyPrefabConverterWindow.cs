using UnityEngine;
using UnityEditor;
using System.Linq;

namespace SharpShadowsToolkit
{
    public class LegacyPrefabConverterWindow : EditorWindow
    {
        public const string Name = "Sharp Shadows Legacy Prefab Converter";

        [MenuItem("Window/Sharp Shadows Toolkit/Legacy Prefab Converter (Preview)")]
        public static void Initialize()
        {
            var window = GetWindow<LegacyPrefabConverterWindow>(Name, true);
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        public bool applyPrefabInstances = true;

        [SerializeField]
        protected Vector2 scrollPosition;

        public void OnSelectionChange()
        {
            Repaint();
        }

        public void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "Please select one or multiple prefabs or game objects that for some reason have lost their connection to the source model prefabs used " +
                "to create them. This tool will find the source model prefab and add its Sharp Shadow components to the game object in the selection. This can " +
                "be useful when integrating sharp shadows into an already existing project. However, one should ideally use Prefab Variants which automatically " +
                "update the prefabs used in a scene when the source model changes. Please see the README for more information.", MessageType.Info);

            var objs = Selection.gameObjects;

            EditorGUILayout.LabelField(objs.Length == 0 ? "Selection: none" : "Selection:");
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUI.indentLevel++;
            for (int i = 0; i < objs.Length; i++)
            {
                EditorGUILayout.LabelField(objs[i].name);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            applyPrefabInstances = EditorGUILayout.Toggle(
                new GUIContent(
                    "Apply Prefab Instances",
                    "When this property is enabled, prefab instances in the selection will update their respective prefab asset once processing is complete"),
                applyPrefabInstances);

            EditorGUI.BeginDisabledGroup(objs.Length == 0);
            if (GUILayout.Button("Update sharp shadows for selection"))
            {
                for (int i = 0; i < objs.Length; i++)
                {
                    UpdateSharpShadowsForGameObject(objs[i], applyPrefabInstances);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        protected void UpdateSharpShadowsForGameObject(GameObject rootObject, bool applyPrefabInstances)
        {
            if (PrefabUtility.IsPartOfModelPrefab(rootObject))
            {
                Debug.Log("'" + rootObject.name + "' is a model prefab or model prefab instance, doing nothing...");
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(rootObject);
            if (!string.IsNullOrEmpty(assetPath))
            {
                // Prefab
                Debug.Log("Updating sharp shadows for prefab at '" + assetPath + "'");
                rootObject = PrefabUtility.LoadPrefabContents(assetPath);
            }
            else
            {
                // Prefab instance or ordinary game object
                Debug.Log("Updating sharp shadows for game object '" + rootObject.name + "'");
            }

            var meshFilters = rootObject.GetComponentsInChildren<MeshFilter>(true);
            var skinnedMeshRenderers = rootObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            var totalCount = meshFilters.Length + skinnedMeshRenderers.Length;
            var count = 0;

            foreach (var meshFilter in meshFilters)
            {
                EditorUtility.DisplayProgressBar(Name, "Updating mesh filter for '" + meshFilter.name + "'", (float)count++ / totalCount);

                if (meshFilter.sharedMesh == null)
                {
                    continue;
                }
                var meshPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                if (string.IsNullOrEmpty(meshPath))
                {
                    continue;
                }
                var model = AssetDatabase.LoadMainAssetAtPath(meshPath) as GameObject;
                if (model == null)
                {
                    continue;
                }
                var matchingMeshFilter = model.GetComponentsInChildren<MeshFilter>().Where(f => f.sharedMesh == meshFilter.sharedMesh).FirstOrDefault();
                if (matchingMeshFilter == null)
                {
                    continue;
                }
                var sourceShadow = matchingMeshFilter.GetComponent<SharpShadow>();
                if (sourceShadow == null)
                {
                    continue;
                }

                var targetObj = meshFilter.gameObject;

                var shadow = targetObj.GetComponent<SharpShadow>();
                if (shadow == null)
                {
                    shadow = Undo.AddComponent<SharpShadow>(targetObj);
                }
                Undo.RecordObject(shadow, "Update Sharp Shadow Component");
                shadow.shadowAsset = sourceShadow.shadowAsset;
            }

            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                EditorUtility.DisplayProgressBar(Name, "Updating skinned mesh renderer for '" + skinnedMeshRenderer.name + "'", (float)count++ / totalCount);

                if (skinnedMeshRenderer.sharedMesh == null)
                {
                    continue;
                }
                var meshPath = AssetDatabase.GetAssetPath(skinnedMeshRenderer.sharedMesh);
                if (string.IsNullOrEmpty(meshPath))
                {
                    continue;
                }
                var model = AssetDatabase.LoadMainAssetAtPath(meshPath) as GameObject;
                if (model == null)
                {
                    continue;
                }
                var matchingSkinnedMeshRenderer = model.GetComponentsInChildren<SkinnedMeshRenderer>().Where(r => r.sharedMesh == skinnedMeshRenderer.sharedMesh).FirstOrDefault();
                if (matchingSkinnedMeshRenderer == null)
                {
                    continue;
                }
                var sourceShadows = matchingSkinnedMeshRenderer.GetComponentsInChildren<SharpShadow>(true);
                if (sourceShadows == null)
                {
                    continue;
                }

                var targetObj = skinnedMeshRenderer.gameObject;

                foreach (var sourceShadow in sourceShadows)
                {
                    var targetChild = targetObj.transform.Find(sourceShadow.gameObject.name)?.gameObject;
                    if (targetChild == null)
                    {
                        targetChild = new GameObject(sourceShadow.gameObject.name);
                        targetChild.transform.parent = targetObj.transform;
                        targetChild.transform.localPosition = Vector3.zero;
                        targetChild.transform.localRotation = Quaternion.identity;
                        targetChild.transform.localScale = Vector3.one;
                        Undo.RegisterCreatedObjectUndo(targetChild, "Create Sharp Shadow Game Object");
                    }

                    var shadowRenderer = targetChild.GetComponent<SkinnedMeshRenderer>();
                    if (shadowRenderer == null)
                    {
                        shadowRenderer = Undo.AddComponent<SkinnedMeshRenderer>(targetChild);
                    }
                    Undo.RecordObject(shadowRenderer, "Update Skinned Mesh Renderer of Sharp Shadow Game Object");
                    shadowRenderer.sharedMesh = sourceShadow.shadowAsset.shadowMesh;
                    shadowRenderer.rootBone = skinnedMeshRenderer.rootBone;
                    shadowRenderer.bones = skinnedMeshRenderer.bones;

                    var shadow = targetChild.GetComponent<SharpShadow>();
                    if (shadow == null)
                    {
                        shadow = Undo.AddComponent<SharpShadow>(targetChild);
                    }
                    Undo.RecordObject(shadow, "Update Sharp Shadow Component");
                    shadow.shadowAsset = sourceShadow.shadowAsset;
                }
            }

            EditorUtility.DisplayProgressBar(Name, "Finalizing...", 1.0f);

            if (!string.IsNullOrEmpty(assetPath))
            {
                // Prefab
                PrefabUtility.SaveAsPrefabAsset(rootObject, assetPath);
                PrefabUtility.UnloadPrefabContents(rootObject);
            }
            else
            {
                // Prefab instance?
                var nearestPrefabInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(rootObject);
                if (nearestPrefabInstanceRoot != null)
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(rootObject);

                    if (applyPrefabInstances)
                    {
                        Debug.Log("Applying prefab instance settings for '" + nearestPrefabInstanceRoot.name + "'");
                        PrefabUtility.ApplyPrefabInstance(nearestPrefabInstanceRoot, InteractionMode.UserAction);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
