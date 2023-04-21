using Microsoft.JSInterop;

namespace JsonToPowershellClass.Blazor.Services;

public class BrowserService
{
    private IJSRuntime _js;
    public event EventHandler<bool> ThemeChanged;
    private bool _isLight; 
    
    public async void Init(IJSRuntime js)
    {
        // enforce single invocation            
        if (_js is not null) 
            return;

        _js = js;
        await _js.InvokeAsync<string>("prefersColorSchemeListener", DotNetObjectReference.Create(this));
    }

    [JSInvokable]
    public void ChangeTheme(bool jsIsLight)
    {
        _isLight = jsIsLight;
        ThemeChanged?.Invoke(this, jsIsLight);
    }
}