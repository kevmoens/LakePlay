using Microsoft.JSInterop;

namespace TriviaForCheeseHeads.WebUtil
{
    public class JsConsole
    {
        private readonly IJSRuntime JsRuntime;
        public JsConsole(IJSRuntime jSRuntime)
        {
            this.JsRuntime = jSRuntime;
        }

        public async Task LogAsync(string message)
        {
            await this.JsRuntime.InvokeVoidAsync("console.log", message);
        }
    }
}
