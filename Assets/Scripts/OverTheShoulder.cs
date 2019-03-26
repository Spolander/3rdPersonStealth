using UnityEngine;
using Cinemachine;

/// <summary>
/// An add-on module for Cinemachine Virtual Camera that offsets the camera's position
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class OverTheShoulder : CinemachineExtension
{
    [Tooltip("Offset the camera's position by this much (camera space)")]
    public Vector3 m_Offset = Vector3.zero;



    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Apply after the camera has been aimed.
        // To apply offset before the camera aims, change this to Body
        if (stage == CinemachineCore.Stage.Aim)
        {
            state.PositionCorrection += state.FinalOrientation * m_Offset;
        }
    }
}