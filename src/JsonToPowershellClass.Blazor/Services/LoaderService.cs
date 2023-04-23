namespace JsonToPowershellClass.Blazor.Services;

public class LoaderService : IDisposable
{
    public event Action OnShow;
    public event Action OnHide;

    /// <summary>
    /// Show loader
    /// </summary>
    public void ShowLoader() => OnShow?.Invoke();

    /// <summary>
    /// Hide loader
    /// </summary>
    public void HideLoader() => OnHide?.Invoke();

    /// <inheritdoc/>
    public void Dispose() => GC.SuppressFinalize(this);
}