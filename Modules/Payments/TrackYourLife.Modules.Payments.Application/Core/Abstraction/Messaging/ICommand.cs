using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;

public interface ICommand : IRequest<Result>, IPaymentsRequest { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IPaymentsRequest { }
