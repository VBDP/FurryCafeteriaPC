using UnityEditor;

namespace VRCCoffeeSet
{
    [CustomEditor(typeof(Object_Ade))]
    public class Object_Ade_Editor : Editor
    {
        Object_Ade script;

        void Awake()
        {
            script = target as Object_Ade;
        }

        public override void OnInspectorGUI()
        {
            // info box
            EditorGUILayout.HelpBox(
                "This is a Coffee Set DLC object\n" +
                "Do not duplicate this object"
                , MessageType.Info
            );
            // base.OnInspectorGUI();
        }
    }
}
