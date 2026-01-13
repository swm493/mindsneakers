using UnityEngine;

public abstract class BaseUIController<TView, TModel> : MonoBehaviour
    where TView : MonoBehaviour, IView
    where TModel : IModel
{
    [SerializeField] protected TView view;
    protected TModel model;

    protected bool isInitialized = false;

    protected virtual void Awake()
    {
        if (isInitialized) return;

        if (view != null)
        {
            OnInitialize();
            isInitialized = true;
        }
        else
        {
            MyDebug.LogWarning($"[{name}] View component missing! Initialize failed.");
        }
    }

    public virtual void Initialize(TView view, TModel model)
    {
        if (isInitialized) return;

        this.view = view;
        this.model = model;

        OnInitialize();
        isInitialized = true;
    }

    protected virtual void OnInitialize() { }

    protected virtual void Release() { }

    protected virtual void OnDestroy()
    {
        Release();
    }
}