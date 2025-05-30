﻿using MediatR;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.Core.Abstraction.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;
