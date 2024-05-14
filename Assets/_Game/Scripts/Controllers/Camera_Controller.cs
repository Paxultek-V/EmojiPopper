using Cinemachine;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Cam ortho parameters")]
    [SerializeField] private CinemachineVirtualCamera m_vCam = null;

    [SerializeField] private float m_defaultSize = 8.75f;

    [SerializeField] private float m_baseScreenRatio = 1.77777f;
    
    
    [Header("Cam position parameters")]
    [SerializeField] private Transform m_cameraTargetPosition = null;

    [SerializeField] private float m_yPositionRatio = 3.5f;
    
    
    // 1080 x 1920 -> 1,777777 ratio
    // 1290 x 2792 -> 2,164341 ratio 
    // 1,217472
    
    void Awake()
    {
        AdaptCameraOrthoSize();
    }

    private void AdaptCameraTarget()
    {
        float yPos = m_yPositionRatio * ((float)Screen.height / Screen.width);
        Vector3 desiredPosition = m_cameraTargetPosition.position;
        desiredPosition.y = yPos;
        m_cameraTargetPosition.position = desiredPosition;
    }

    private void AdaptCameraOrthoSize()
    {
        float currentScreeRatio = (float)Screen.height / Screen.width;
        
        float newRatio = currentScreeRatio / m_baseScreenRatio;
        
        m_vCam.m_Lens.OrthographicSize = m_defaultSize * newRatio;
    }
}
