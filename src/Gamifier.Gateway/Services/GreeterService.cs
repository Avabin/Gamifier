using Grpc.Core;
using Gamifier.Gateway;

namespace Gamifier.Gateway.Services;

/// <summary>
/// Represents a service that handles greeting operations.
/// </summary>
public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;


    /// <summary>
    /// Represents a service that handles greeting operations.
    /// </summary>
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Responds to a client's greeting by saying hello.
    /// </summary>
    /// <param name="request">The request containing the client's name</param>
    /// <param name="context">The context of the server call</param>
    /// <returns>The response message with a greeting</returns>
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}