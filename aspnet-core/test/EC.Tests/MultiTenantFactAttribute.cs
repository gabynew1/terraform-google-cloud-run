using Xunit;

namespace EC.Tests
{
    public sealed class MultiTenantFactAttribute : FactAttribute
    {
        public MultiTenantFactAttribute()
        {
#pragma warning disable CS0162 // Unreachable code — intentional, MultiTenancyEnabled is a compile-time constant
            if (!ECConsts.MultiTenancyEnabled)
            {
                Skip = "MultiTenancy is disabled.";
                return;
            }
#pragma warning restore CS0162
        }
    }
}
