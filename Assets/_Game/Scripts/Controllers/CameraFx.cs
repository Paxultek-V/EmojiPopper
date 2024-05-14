using System;
using UnityEngine;

public class CameraFx : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_clickFx;

    [SerializeField] private float m_distanceZ;

    
    private Plane m_plane;
    private Vector3 m_distanceFromCamera;

    
    private void OnEnable()
    {
        Click_Controller.OnClick += OnClick;
    }

    private void OnDisable()
    {
        Click_Controller.OnClick -= OnClick;
    }

    private void Start()
    {
        Initialize();
    }

    
    private void Initialize()
    {
        m_clickFx.Stop();

        m_distanceFromCamera = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,
            Camera.main.transform.position.z + m_distanceZ);

        m_plane = new Plane(Vector3.back, m_distanceFromCamera);
    }

    private void OnClick(Vector3 cursorPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);

        float enter = 0.0f;

        if (m_plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            PlayFxAtPosition(hitPoint);
        }
    }

    private void PlayFxAtPosition(Vector3 position)
    {
        m_clickFx.gameObject.transform.position = position;

        if (m_clickFx.isPlaying)
            m_clickFx.Stop();

        m_clickFx.Play();
    }
}