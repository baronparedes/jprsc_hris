using MediatR.Pipeline;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JPRSC.HRIS.Infrastructure.MediatR
{
    // https://github.com/jbogard/MediatR/blob/master/samples/MediatR.Examples/GenericRequestPreProcessor.cs
    public class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly TextWriter _writer;

        public GenericRequestPreProcessor(TextWriter writer)
        {
            _writer = writer;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            _writer.WriteLine("- Starting Up");
            return Task.FromResult(0);
        }
    }
}