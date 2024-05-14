using UnityEngine;

public class LifeSpan : MonoBehaviour
{
    [SerializeField] private float m_lifeSpan = 5f;

    private float m_timer;

    private void Update()
    {
        m_timer += Time.deltaTime;

        if (m_timer > m_lifeSpan)
            Destroy(gameObject);
    }
}