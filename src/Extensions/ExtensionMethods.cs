using System.Reflection;

namespace diggcordslash.Extensions;

public static class ExtensionMethods
{
    public static async Task<object> InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
    {
        if (methodInfo != null)
        {
            dynamic? awaitable = methodInfo.Invoke(obj, parameters);
            if (awaitable != null)
            {
                await awaitable;
                return awaitable.GetAwaiter().GetResult();
            }
        }
        // Return the interaction we received - probably incorrect but we really, really, should not end up here
        return Task.FromResult(obj);
    }
}