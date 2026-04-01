using Microsoft.JSInterop;

namespace Orders.Frontend.Helpers;

public static class JSRuntimeExtensionMethods
{
    public static ValueTask<object> SetLocalStorage(this IJSRuntime js, string key, string content)
    {
        return js.InvokeAsync<object>("localStorage.setItem", key, content);
    }

    public static ValueTask<object> GetLocalStorage(this IJSRuntime jS, string key)
    {
        return jS.InvokeAsync<object>("localStorage.getItem", key);
    }

    public static ValueTask<object> RemoveLocalStorage(this IJSRuntime js, string key)
    {
        return js.InvokeAsync<object>("localStorage.removeItem", key);
    }
}