using System;
using UnityEngine;

public class ExplosionFx : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_bombFx = null;

    [SerializeField] private float m_zPositionOffset = -1f;
    
    private Vector3 m_bombPosition;
    
    private void OnEnable()
    {
        LevelFiguresController.OnClickFigureBomb += OnClickFigureBomb;
        LevelFiguresController.OnBombExplode += OnBombExplode;
    }

    private void OnDisable()
    {
        LevelFiguresController.OnClickFigureBomb -= OnClickFigureBomb;
        LevelFiguresController.OnBombExplode -= OnBombExplode;
    }

    private void OnClickFigureBomb(Figure selectedFigure)
    {
        m_bombPosition.x = selectedFigure.XPos;
        m_bombPosition.y = selectedFigure.YPos;
        m_bombPosition.z = m_zPositionOffset;
    }

    private void OnBombExplode()
    {
        m_bombFx.transform.position = m_bombPosition;
        PlayBombFx();
    }
    
    private void PlayBombFx()
    {
        if (m_bombFx.isPlaying)
            m_bombFx.Stop();

        m_bombFx.Play();
    }
}