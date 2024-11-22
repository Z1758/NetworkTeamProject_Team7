#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(Hitbox))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        Hitbox fow = (Hitbox)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.radius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.angle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.angle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.radius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.radius);

    }
}

#endif