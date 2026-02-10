using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildCore.HumanResources.Infrastructure.Persistence.Interceptors
{
    //Domain event üretilir, SaveChanges commit olur, Interceptor publish eder.
    public sealed class DomainEventInterceptor : SaveChangesInterceptor
    {
        private readonly IPublisher _publisher;

        public DomainEventInterceptor(IPublisher publisher)
        {
            _publisher = publisher;
        }
    }
}
