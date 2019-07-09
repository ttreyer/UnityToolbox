using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovement))]
public class GroundCheckEditor : Editor {
    private bool handleEnabled = false;

    override public void OnInspectorGUI() {
        DrawDefaultInspector();

        bool newHandleEnabled = GUILayout.Toggle(handleEnabled, "Edit Ground Check");
        if (newHandleEnabled != handleEnabled) {
            handleEnabled = newHandleEnabled;
            EditorUtility.SetDirty(target);
        }
    }

    public void OnSceneGUI() {
        if (!handleEnabled)
            return;

        PlayerMovement playerMovement = (target as PlayerMovement);

        Handles.color = Color.green;

        Vector3 playerPosition = playerMovement.transform.position;
        Vector3 groundCheckPosition = playerPosition + (Vector3)playerMovement.groundCheck.offset;
        float groundCheckRadius = playerMovement.groundCheck.radius;

        EditorGUI.BeginChangeCheck();
        Vector3 newGroundCheckPosition = Handles.PositionHandle(groundCheckPosition, Quaternion.identity);
        float newGroundCheckRadius = Handles.RadiusHandle(Quaternion.identity, groundCheckPosition, groundCheckRadius);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed Player's Ground Check");
            playerMovement.groundCheck.offset = newGroundCheckPosition - playerPosition;
            playerMovement.groundCheck.radius = newGroundCheckRadius;
        }

        Handles.DrawLine(playerPosition, groundCheckPosition);

        // Handles.DrawWireDisc(groundCheckPosition, Vector3.forward, groundCheckRadius);
    }
}
