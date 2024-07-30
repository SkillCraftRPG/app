using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace SkillCraft.Application.Accounts.Events;

internal record UserSignedInEvent(Session Session) : INotification;
