using UnityEngine;
using UnityEditor;
class Block24Editor : Editor
{
    float size = 1f;

    protected virtual void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Block24 target24 = ((Block24)target);
            Transform transform = ((Block24)target).transform;
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(
                0,
                transform.position + target24.position,
                transform.rotation * Quaternion.LookRotation(Vector3.right),
                size,
                EventType.Repaint
            );
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(
                0,
                transform.position + target24.position,
                transform.rotation * Quaternion.LookRotation(Vector3.up),
                size,
                EventType.Repaint
            );
            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(
                0,
                transform.position + target24.position,
                transform.rotation * Quaternion.LookRotation(Vector3.forward),
                size,
                EventType.Repaint
            );
        }
    }
}