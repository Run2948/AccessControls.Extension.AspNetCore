namespace AccessControls.Extension.AspNetCore
{
    /// <summary>
    /// view component access strategy
    /// </summary>
    public interface IControlAccessStrategy
    {
        /// <summary>
        /// view component access strategy
        /// </summary>
        bool IsControlCanAccess(string accessKey);
    }
}
