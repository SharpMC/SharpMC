namespace SharpMC.Plugin.API
{
    public interface IPlugin
    {
        /// <summary>
        /// This function will be called on plugin initialization
        /// </summary>
        void OnEnable(IPluginContext context);

        /// <summary>
        /// This function will be called when the plugin will be disabled
        /// </summary>
        void OnDisable();
    }
}