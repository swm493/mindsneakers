using System;
using UnityEngine;

public abstract class BaseDialogueController<TView, TModel> : MonoBehaviour, IDialogueController
    where TView : MonoBehaviour, IView
    where TModel : IModel
{
    [SerializeField] protected TView view;
    protected TModel model;

    protected bool isInitialized = false;

    public event Action OnDialogueComplete;

    protected virtual void Awake()
    {
        if (isInitialized) return;

        if (view != null)
        {
            Initialize();
            isInitialized = true;
        }
        else
        {
            MyDebug.LogWarning($"[{name}] View component missing! Initialize failed.");
        }
    }

    protected virtual void OnDestroy()
    {
        Release();
    }

    protected virtual void Initialize() { }

    protected virtual void Release() { }

    public virtual void ShowDialogue()
    {
        view.Show();
    }

    public void CloseDialogue()
    {
        OnDialogueComplete?.Invoke();
    }

    public abstract void SetDialogueId(int id);
}