using UnityEditor;
using UnityEngine;

namespace SharpShadowsToolkit
{
    [CustomPropertyDrawer(typeof(SingleLayer))]
    public class SingleLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var layerIndex = property.FindPropertyRelative("layerIndex");
            if (layerIndex != null)
            {
                EditorGUI.BeginChangeCheck();
                int newValue = EditorGUI.LayerField(position, label, layerIndex.intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    layerIndex.intValue = newValue;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}
