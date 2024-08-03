﻿using Logitar.Portal.Contracts.Users;
using MediatR;
using SkillCraft.Contracts.Accounts;

namespace SkillCraft.Application.Accounts.Commands;

public record SaveProfileCommand(SaveProfilePayload Payload) : Activity, IRequest<User>;
