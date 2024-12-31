using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.Core.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>, ICommonRequest;
