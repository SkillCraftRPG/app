﻿using Moq;
using SkillCraft.Application.Characters.Commands;
using SkillCraft.Application.Educations;
using SkillCraft.Application.Permissions;
using SkillCraft.Contracts;
using SkillCraft.Contracts.Characters;
using SkillCraft.Domain;
using SkillCraft.Domain.Educations;

namespace SkillCraft.Application.Characters.Creation;

[Trait(Traits.Category, Categories.Unit)]
public class ResolveEducationQueryHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IEducationRepository> _educationRepository = new();
  private readonly Mock<IPermissionService> _permissionService = new();

  private readonly ResolveEducationQueryHandler _handler;

  private readonly WorldMock _world = new();
  private readonly Education _education;
  private readonly CreateCharacterCommand _activity = new(Id: null, new CreateCharacterPayload());

  public ResolveEducationQueryHandlerTests()
  {
    _handler = new(_educationRepository.Object, _permissionService.Object);

    _education = new(_world.Id, new Name("Champs de bataille"), _world.OwnerId);
    _educationRepository.Setup(x => x.LoadAsync(_education.Id, _cancellationToken)).ReturnsAsync(_education);

    _activity.Contextualize(_world);
  }

  [Fact(DisplayName = "It should return the found education.")]
  public async Task It_should_return_the_found_education()
  {
    ResolveEducationQuery query = new(_activity, _education.EntityId);

    Education education = await _handler.Handle(query, _cancellationToken);
    Assert.Same(_education, education);

    _permissionService.Verify(x => x.EnsureCanPreviewAsync(_activity, EntityType.Education, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw EducationNotFoundException when the education could not be found.")]
  public async Task It_should_throw_EducationNotFoundException_when_the_education_could_not_be_found()
  {
    ResolveEducationQuery query = new(_activity, Guid.NewGuid());

    var exception = await Assert.ThrowsAsync<EducationNotFoundException>(async () => await _handler.Handle(query, _cancellationToken));
    Assert.Equal(_world.Id.ToGuid(), exception.WorldId);
    Assert.Equal(query.Id, exception.EducationId);
    Assert.Equal("EducationId", exception.PropertyName);
  }
}
