namespace PubSub.Model
{
    public class ConfigurationModel
    {
        public bool IgnoreCallbackException { get; set; } = false;
        public bool FireAndForgetCallback { get; set; } = false;
        public bool InvokeCallbackFunctionsSimultaneously { get; set; }
    }
}
