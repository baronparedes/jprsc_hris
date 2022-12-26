using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Infrastructure.MediatR
{
    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/GenericPipelineBehavior.cs
    public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly TextWriter _writer;

        public GenericPipelineBehavior(TextWriter writer)
        {
            _writer = writer;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _writer.WriteLine("-- Handling Request");
            var response = await next();
            _writer.WriteLine("-- Finished Request");
            return response;
        }
    }
}