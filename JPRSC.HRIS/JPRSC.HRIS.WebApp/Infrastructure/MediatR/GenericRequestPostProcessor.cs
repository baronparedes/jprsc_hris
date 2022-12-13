using MediatR.Pipeline;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.WebApp.Infrastructure.MediatR
{
    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/GenericRequestPostProcessor.cs
    public class GenericRequestPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly TextWriter _writer;

        public GenericRequestPostProcessor(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            _writer.WriteLine("- All Done");
            return Task.FromResult(0);
        }
    }
}