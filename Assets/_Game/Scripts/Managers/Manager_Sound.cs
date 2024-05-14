using UnityEngine;

public class Manager_Sound : MonoBehaviour
{
    [SerializeField] private AudioSource m_pop1_sound = null;
    [SerializeField] private AudioSource m_pop2_sound = null;
    [SerializeField] private AudioSource m_pop3_sound = null;
    [SerializeField] private AudioSource m_pop4_sound = null;
    [SerializeField] private AudioSource m_pop5_sound = null;
    [SerializeField] private AudioSource m_pop6_sound = null;
    [SerializeField] private AudioSource m_pop7_sound = null;
    [SerializeField] private AudioSource m_explosion_sound = null;
    [SerializeField] private AudioSource m_bombPlanted_Sound = null;
    [SerializeField] private AudioSource m_playerReady_Sound = null;
    [SerializeField] private AudioSource m_wrongTap_Sound = null;

    
    private void OnEnable()
    {
        Option.OnSendOptionState += OnSendOptionState;
    }

    private void OnDisable()
    {
        Option.OnSendOptionState -= OnSendOptionState;
    }


    private void OnSendOptionState(OptionType optionType, bool state)
    {
        if (optionType == OptionType.Sound)
        {
            if (state)
                SubscribeToEvents();
            else
                UnsubscribeToEvents();
        }
    }

    private void SubscribeToEvents()
    {
        Figure.OnFigureClicked += OnFigureClicked;
        LevelFiguresController.OnPopFigures += OnPopFigures;
        UI_BombConfirmationPanel.OnConfirmBombSelection += OnConfirmBombSelection;
        LevelFiguresController.OnClickFigureBomb += OnClickFigureBomb;
        LevelFiguresController.OnPlayerCanInteractWithFigures += OnPlayerCanInteractWithFigures;
        Click_Controller.OnClick += OnClick;
    }

    private void UnsubscribeToEvents()
    {
        Figure.OnFigureClicked -= OnFigureClicked;
        LevelFiguresController.OnPopFigures -= OnPopFigures;
        UI_BombConfirmationPanel.OnConfirmBombSelection -= OnConfirmBombSelection;
        LevelFiguresController.OnClickFigureBomb -= OnClickFigureBomb;
        LevelFiguresController.OnPlayerCanInteractWithFigures -= OnPlayerCanInteractWithFigures;
        Click_Controller.OnClick -= OnClick;
    }

    private void OnPlayerCanInteractWithFigures(bool canInteract)
    {
        /*
         if(canInteract)
            m_playerReady_Sound.Play();
        */
    }

    private void OnClick(Vector3 cursorPosition)
    {
        m_pop1_sound.Play();
    }

    private void OnFigureClicked(Figure clickedFigure)
    {
        /*
         if (Manager_ClickMode.Instance.CurrentClickMode == ClickMode.Pop)
            m_pop1_sound.Play();
        */
    }

    private void OnClickFigureBomb(Figure selectedFigure)
    {
        //m_bombPlanted_Sound.Play();
    }

    private void OnConfirmBombSelection()
    {
        m_explosion_sound.Play();
    }

    private void OnPopFigures(int count)
    {
        if (count == 1)
            m_wrongTap_Sound.Play();
        else if (count == 2)
            m_pop2_sound.Play();
        else if (count == 3)
            m_pop3_sound.Play();
        else if (count == 4)
            m_pop4_sound.Play();
        else if (count == 5)
            m_pop5_sound.Play();
        else if (count == 6)
            m_pop6_sound.Play();
        else if (count >= 7)
            m_pop7_sound.Play();
    }
}