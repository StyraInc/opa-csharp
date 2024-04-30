
namespace Styra.OpenApi.Hooks
{
    /// <summary>
    /// This file is only ever generated once on the first generation and then is free to be modified.
    /// Any hooks you wish to add should be registered in the InitHooks function. Feel free to define them
    /// in this file or in separate files in the Hooks folder.
    /// </summary>
    public static class HookRegistration
    {
        /// <summary>
        /// Add hooks by calling `Styra.OpenApi.Hooks.Register<HookInterface>(myHook);`
        /// where `I<HookInterface>` is one of the following interfaces defined in HookTypes.cs:
        ///   - ISDKInitHook
        ///   - IBeforeRequestHook
        ///   - IAfterSuccess
        ///   - IAfterError
        /// and `myHook` an instance that implements that specific interface.
        /// </summary>
        public static void InitHooks(IHooks hooks)
        {
            // var myHook = new MyHook();

            // hooks.RegisterSDKInitHook(myHook);
            // hooks.RegisterBeforeRequestHook(myHook);
            // hooks.RegisterAfterSuccessHook(myHook);
            // hooks.RegisterAfterErrorHook(myHook;
        }
    }
}