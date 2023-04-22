using System.Timers;
using Timer = System.Timers.Timer;

namespace JsonToPowershellClass.Blazor.Services;

public class ToastService : IDisposable
{
    public event Action<string, string, ToastLevel> OnShow;
    public event Action OnHide;
    private Timer _countdown;
    private const int ConfirmDuration = 1500;
    private const int AlertDuration = 3000;

    /// <summary>
    /// Show an informational toast message
    /// </summary>
    /// <param name="message">Friendly message</param>
    /// <param name="detail">Detailed error, e.g. Newtonsoft exception message</param>
    public void ShowInfo(string message, string detail = null) => ShowToast(message, detail, ToastLevel.Info);

    /// <summary>
    /// Show a success toast message
    /// </summary>
    /// <param name="message">Friendly message</param>
    /// <param name="detail">Detailed error, e.g. Newtonsoft exception message</param>
    public void ShowSuccess(string message, string detail = null) => ShowToast(message, detail, ToastLevel.Success);

    /// <summary>
    /// Show a warning toast message
    /// </summary>
    /// <param name="message">Friendly message</param>
    /// <param name="detail">Detailed error, e.g. Newtonsoft exception message</param>
    public void ShowWarning(string message, string detail = null) => ShowToast(message, detail, ToastLevel.Warning);

    /// <summary>
    /// Show an error toast message
    /// </summary>
    /// <param name="message">Friendly message</param>
    /// <param name="detail">Detailed error, e.g. Newtonsoft exception message</param>
    public void ShowError(string message, string detail = null) => ShowToast(message, detail, ToastLevel.Error);

    /// <summary>
    /// Show a toast message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="detail"></param>
    /// <param name="level"></param>
    private void ShowToast(string message, string detail, ToastLevel level)
    {
        OnShow?.Invoke(message, detail, level);

        StartCountdown(level);
    }

    /// <summary>
    /// Start the countdown to hide the toast message
    /// </summary>
    /// <param name="level"></param>
    private void StartCountdown(ToastLevel level)
    {
        SetCountdown(level);

        if (!_countdown.Enabled)
        {
            _countdown.Start();

            return;
        }

        _countdown.Stop();
        _countdown.Start();
    }

    /// <summary>
    /// Set the countdown timer
    /// </summary>
    /// <param name="level"></param>
    private void SetCountdown(ToastLevel level)
    {
        if (_countdown != null) 
            return;
        
        _countdown = new Timer(level is ToastLevel.Success ? ConfirmDuration : AlertDuration);
        _countdown.Elapsed += HideToast;
        _countdown.AutoReset = false;
    }

    /// <summary>
    /// Hide the toast message
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void HideToast(object source, ElapsedEventArgs args) 
        => OnHide?.Invoke();

    /// <inheritdoc/>
    public void Dispose()
    {
        _countdown?.Dispose();

        GC.SuppressFinalize(this);
    }
}