using Microsoft.AspNetCore.Builder;

namespace Wcs;

public class ApiLogPipeline
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ApiLogMidware>();
    }
}