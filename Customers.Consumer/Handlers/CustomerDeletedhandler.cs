using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers
{
    public class CustomerDeletedhandler : IRequestHandler<CustomerDeleted>
    {
        private readonly ILogger<CustomerDeletedhandler> _logger;
        public CustomerDeletedhandler(ILogger<CustomerDeletedhandler> logger)
        {
            _logger = logger;
        }
        public Task<Unit> Handle(CustomerDeleted request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(request.Id.ToString());
            return Unit.Task;
        }
    }
}
