﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CSharpFunctionalExtensions;
using FakeSurveyGenerator.Application.Common.Errors;
using FakeSurveyGenerator.Application.Common.Persistence;
using FakeSurveyGenerator.Application.Users.Models;
using FakeSurveyGenerator.Domain.AggregatesModel.UserAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FakeSurveyGenerator.Application.Users.Queries.GetUser;

public sealed record GetUserQuery(int Id) : IRequest<Result<UserModel, Error>>;

public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserModel, Error>>
{
    private readonly ISurveyContext _surveyContext;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(ISurveyContext context, IMapper mapper)
    {
        _surveyContext = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<UserModel, Error>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _surveyContext.Users
            .AsNoTracking()
            .ProjectTo<UserModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        return user is null ? Errors.General.NotFound(nameof(User), request.Id) : user;
    }
}