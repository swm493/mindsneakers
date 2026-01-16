using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class CutsceneController : MonoBehaviour
{
    public PlayableDirector director;

    private void OnEnable()
    {
        InputManager.Instance.UI.Click.performed += OnNextInput;
        InputManager.Instance.UI.Submit.performed += OnNextInput;
    }

    private void OnDisable()
    {
        InputManager.Instance.UI.Click.performed -= OnNextInput;
        InputManager.Instance.UI.Submit.performed -= OnNextInput;
    }

    private void OnNextInput(InputAction.CallbackContext context)
    {
        ResumeTimeline();
    }

    public void PauseTimeline()
    {
        director.Pause();
    }

    public void ResumeTimeline()
    {
        if (director.state == PlayState.Paused)
        {
            director.Play();
        }
    }
}