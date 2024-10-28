using MediatR;
using SkillCraft.Application.Storages;
using SkillCraft.Domain.Items;

namespace SkillCraft.Application.Items.Commands;

internal record SaveItemCommand(Item Item) : IRequest;

internal class SaveItemCommandHandler : IRequestHandler<SaveItemCommand>
{
  private readonly IItemRepository _itemRepository;
  private readonly IStorageService _storageService;

  public SaveItemCommandHandler(IItemRepository itemRepository, IStorageService storageService)
  {
    _itemRepository = itemRepository;
    _storageService = storageService;
  }

  public async Task Handle(SaveItemCommand command, CancellationToken cancellationToken)
  {
    Item item = command.Item;

    EntityMetadata entity = item.GetMetadata();
    await _storageService.EnsureAvailableAsync(entity, cancellationToken);

    await _itemRepository.SaveAsync(item, cancellationToken);

    await _storageService.UpdateAsync(entity, cancellationToken);
  }
}
