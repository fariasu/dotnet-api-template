﻿using AutoMapper;
using TaskManager.Application.Validators.Users;
using TaskManager.Communication.DTOs.Users.Requests;
using TaskManager.Domain.Repositories.Db;
using TaskManager.Domain.Repositories.Users;
using TaskManager.Domain.Services;
using TaskManager.Exception.ExceptionsBase;

namespace TaskManager.Application.UseCases.Users.Update.Profile;

public class UpdateProfileUseCase(
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IUserRepositoryReadOnly userRepositoryReadOnly,
    ILoggedUserService loggedUserService)
    : IUpdateProfileUseCase
{
    
    public async Task Execute(RequestUpdateProfileJson request)
    {
        await Validate(request);

        var loggedUser = await loggedUserService.GetUserAsync();
        var userEntity = await userRepositoryReadOnly.GetActiveUserWithEmail(loggedUser.Email);
        
        mapper.Map(request, userEntity);

        await unitOfWork.CommitAsync();
    }

    private async Task Validate(RequestUpdateProfileJson request)
    {
        var result = await new UpdateProfileValidator().ValidateAsync(request);

        if (result.IsValid is false)
        {
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errors);
        }
    }
}