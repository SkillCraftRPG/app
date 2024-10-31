using Bogus;
using Logitar;
using Logitar.Data;
using Logitar.Data.PostgreSQL;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillCraft.Application;
using SkillCraft.Application.Actors;
using SkillCraft.Application.Logging;
using SkillCraft.Contracts.Worlds;
using SkillCraft.Domain;
using SkillCraft.Domain.Worlds;
using SkillCraft.EntityFrameworkCore;
using SkillCraft.EntityFrameworkCore.SqlServer;
using SkillCraft.Infrastructure;
using SkillCraft.Logging;

namespace SkillCraft;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;
  private readonly User _user;

  protected Actor Actor { get; }
  protected Faker Faker { get; } = new();
  protected UserId UserId { get; }
  protected World World { get; }

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }
  protected IRequestPipeline Pipeline { get; }
  protected EventContext EventContext { get; }
  protected SkillCraftContext SkillCraftContext { get; }

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);

    string connectionString;
    string database = GetType().Name;
    _databaseProvider = Configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (_databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        connectionString = Configuration.GetValue<string>("POSTGRESQLCONNSTR_SkillCraft")?.Replace("{Database}", database) ?? string.Empty;
        services.AddSkillCraftWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = Configuration.GetValue<string>("SQLCONNSTR_SkillCraft")?.Replace("{Database}", database) ?? string.Empty;
        services.AddSkillCraftWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    DateTime now = DateTime.UtcNow;
    _user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      CreatedOn = now,
      UpdatedOn = now,
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true,
        VerifiedOn = now
      },
      IsConfirmed = true,
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      FullName = Faker.Person.FullName,
      Locale = new Locale(Faker.Locale),
      TimeZone = "America/Montreal",
      Picture = Faker.Person.Avatar,
      AuthenticatedOn = now
    };
    Actor = new(_user);
    _user.CreatedBy = Actor;
    _user.UpdatedBy = Actor;
    _user.Email.VerifiedBy = Actor;
    UserId = new(_user.Id);

    World = new(new Slug("ungar"), UserId);
    WorldModel worldModel = new(Actor, World.Slug.Value)
    {
      Id = World.Id.ToGuid(),
      Version = World.Version,
      CreatedBy = Actor,
      CreatedOn = World.CreatedOn.AsUniversalTime(),
      UpdatedBy = Actor,
      UpdatedOn = World.UpdatedOn.AsUniversalTime(),
      Name = World.Name?.Value,
      Description = World.Description?.Value
    };

    ActivityContext activityContext = new(ApiKey: null, Session: null, _user, worldModel);
    services.AddSingleton<IActivityContextResolver>(new TestActivityContextResolver(activityContext));
    services.AddSingleton<ILogRepository, FakeLogRepository>();

    ServiceProvider = services.BuildServiceProvider();

    Pipeline = ServiceProvider.GetRequiredService<IRequestPipeline>();
    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    SkillCraftContext = ServiceProvider.GetRequiredService<SkillCraftContext>();
  }

  public virtual async Task InitializeAsync()
  {
    await MigrateAsync();
    await EmptyDatabaseAsync();
    await SeedDatabaseAsync();
  }

  private async Task MigrateAsync()
  {
    await EventContext.Database.MigrateAsync();
    await SkillCraftContext.Database.MigrateAsync();
  }

  private async Task EmptyDatabaseAsync()
  {
    StringBuilder statement = new();
    TableId[] tables =
    [
      EntityFrameworkCore.SkillCraftDb.Comments.Table,
      EntityFrameworkCore.SkillCraftDb.Characters.Table,
      EntityFrameworkCore.SkillCraftDb.Aspects.Table,
      EntityFrameworkCore.SkillCraftDb.Castes.Table,
      EntityFrameworkCore.SkillCraftDb.Educations.Table,
      EntityFrameworkCore.SkillCraftDb.Items.Table,
      EntityFrameworkCore.SkillCraftDb.Languages.Table,
      EntityFrameworkCore.SkillCraftDb.Lineages.Table,
      EntityFrameworkCore.SkillCraftDb.Parties.Table,
      EntityFrameworkCore.SkillCraftDb.Personalities.Table,
      EntityFrameworkCore.SkillCraftDb.Customizations.Table,
      EntityFrameworkCore.SkillCraftDb.StorageDetails.Table,
      EntityFrameworkCore.SkillCraftDb.StorageSummaries.Table,
      EntityFrameworkCore.SkillCraftDb.Talents.Table,
      EntityFrameworkCore.SkillCraftDb.Worlds.Table,
      EntityFrameworkCore.SkillCraftDb.Users.Table,
      EventDb.Events.Table
    ];
    foreach (TableId table in tables)
    {
      statement.Append(CreateDeleteBuilder(table).Build().Text);
    }
    await SkillCraftContext.Database.ExecuteSqlRawAsync(statement.ToString());
  }
  private IDeleteBuilder CreateDeleteBuilder(TableId table)
  {
    return _databaseProvider switch
    {
      DatabaseProvider.EntityFrameworkCorePostgreSQL => new PostgresDeleteBuilder(table),
      DatabaseProvider.EntityFrameworkCoreSqlServer => new SqlServerDeleteBuilder(table),
      _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
    };
  }

  private async Task SeedDatabaseAsync()
  {
    IActorService actorService = ServiceProvider.GetRequiredService<IActorService>();
    await actorService.SaveAsync(_user);

    IWorldRepository worldRepository = ServiceProvider.GetRequiredService<IWorldRepository>();
    await worldRepository.SaveAsync(World);
  }

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
