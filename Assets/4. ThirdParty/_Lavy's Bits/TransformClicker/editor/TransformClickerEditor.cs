using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class TransformClickerEditor
{
    private static Transform selectedTransform;

    static TransformClickerEditor()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    static void DuringSceneGUI(SceneView sceneView)
    {
        if (Event.current != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.B)
        {
            if (Selection.activeTransform != null)
            {
                selectedTransform = Selection.activeTransform;

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Undo.RecordObject(selectedTransform, "Place On Mesh");
                    selectedTransform.position = hit.point;
                    Event.current.Use();
                }
            }
        }
    }
}
