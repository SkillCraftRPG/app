using MediatR;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts.Languages;

namespace SkillCraft.Application.Languages.Queries;

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
    LanguageModel? language = await _languageQuerier.ReadAsync(query.Id, cancellationToken);
    if (language != null)
    {
      await _permissionService.EnsureCanPreviewAsync(query, language.GetMetadata(), cancellationToken);
    }

    return language;
  }
}
