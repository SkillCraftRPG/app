﻿using CsvHelper;
using CsvHelper.Configuration.Attributes;
using MediatR;
using SkillCraft.Application.Worlds.Commands;
using SkillCraft.Contracts.Worlds;
using System.Globalization;

namespace SkillCraft.Tools.ETL.Commands;

internal record ImportWorlds : INotification;

internal class ImportWorldsHandler : INotificationHandler<ImportWorlds>
{
  private readonly IApiClient _client;

  public ImportWorldsHandler(IApiClient client)
  {
    _client = client;
  }

  public async Task Handle(ImportWorlds _, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<World> worlds = Extract();
    IReadOnlyCollection<SaveWorldCommand> commands = Transform(worlds);
    await LoadAsync(commands, cancellationToken);
  }

  private static IReadOnlyCollection<World> Extract()
  {
    StreamReader reader = new("data/worlds.csv");
    CsvReader csv = new(reader, CultureInfo.InvariantCulture);
    IEnumerable<World> records = csv.GetRecords<World>();
    return records.ToArray().AsReadOnly();
  }

  private static IReadOnlyCollection<SaveWorldCommand> Transform(IEnumerable<World> worlds) => worlds.Select(world =>
  {
    SaveWorldPayload payload = new(world.Slug)
    {
      Name = world.Name,
      Description = world.Description
    };
    return new SaveWorldCommand(world.Id, payload, Version: null);
  }).ToArray().AsReadOnly();

  private async Task LoadAsync(IEnumerable<SaveWorldCommand> commands, CancellationToken cancellationToken)
  {
    foreach (SaveWorldCommand command in commands)
    {
      await _client.SaveWorldAsync(command, cancellationToken);
    }
  }
}

internal record World
{
  [Name("id")]
  public Guid Id { get; set; }

  [Name("slug")]
  public string Slug { get; set; } = string.Empty;

  [Name("name")]
  public string? Name { get; set; }

  [Name("description")]
  public string? Description { get; set; }
}