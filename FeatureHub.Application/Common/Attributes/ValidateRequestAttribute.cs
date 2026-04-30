using FeatureHub.Application.Common.Middleware;
using Paramore.Brighter;

namespace FeatureHub.Application.Common.Attributes;

public class ValidateRequestAttribute : RequestHandlerAttribute
{
    public ValidateRequestAttribute(int step, HandlerTiming timing = HandlerTiming.Before) : base(step, timing)
    {
    }

    public override Type GetHandlerType()
    {
        return typeof(ValidationHandler<>);
    }
}
