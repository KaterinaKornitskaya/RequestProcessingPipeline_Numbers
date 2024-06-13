using Microsoft.AspNetCore.Builder;
using System.Runtime.CompilerServices;

namespace RequestProcessingPipeline
{
    public static class FromHundredToThousandExtension
    {
        public static IApplicationBuilder UseFromHundredToThousand( this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FromHundredToThousandMiddleware>();
        }
    }
}
