using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ellipsoid)), CanEditMultipleObjects]
public class PositionHandleExampleEditor : Editor {
    protected virtual void OnSceneGUI() {
        Ellipsoid example = (Ellipsoid)target;

        Tools.hidden = Selection.activeGameObject == example.gameObject ? true : Tools.hidden;

        EditorGUI.BeginChangeCheck();
        Vector3 focus1Handle = Handles.PositionHandle(example.focus1.position, Quaternion.identity);
        Vector3 focus2Handle = Handles.PositionHandle(example.focus2.position, Quaternion.identity);

        Vector3 boundsPositionHandle = Handles.PositionHandle(example.intersectionBounds.center, Quaternion.identity);
        Vector3 boundsScaleHandle    = Handles.ScaleHandle(example.intersectionBounds.size, example.intersectionBounds.center, Quaternion.identity, 0.05f);

        Handles.DrawWireCube(example.intersectionBounds.center, example.intersectionBounds.size);

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(example, "Change " + example.gameObject.name + " property");
            example.focus1.position = focus1Handle;
            example.focus2.position = focus2Handle;

            example.intersectionBounds.center = boundsPositionHandle;
            example.intersectionBounds.size   = boundsScaleHandle;

            example.Update();
        }
    }
}
