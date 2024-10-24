using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.Application.Languages.Queries;

/// <exception cref="PermissionDeniedException"></exception>
public record ReadLanguageQuery(Guid Id) : Activity, IRequest<LanguageModel?>;

internal class ReadLanguageQueryHandler : IRequestHandler<ReadLanguageQuery, LanguageModel?>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly IPermissionService _permissionService;

  public ReadLanguageQueryHandler(ILanguageQuerier languageQuerier, IPermissionService permissionService)
  {
    _languageQuerier = languageQuerier;
    _permissionService = permissionService;
  }

  public async Task<LanguageModel?> Handle(ReadLanguageQuery query, CancellationToken cancellationToken)
  {
    await _permissionService.EnsureCanPreviewAsync(query, EntityType.Language, cancellationToken);

    return await _languageQuerier.ReadAsync(query.GetWorldId(), query.Id, cancellationToken);
  }
}
