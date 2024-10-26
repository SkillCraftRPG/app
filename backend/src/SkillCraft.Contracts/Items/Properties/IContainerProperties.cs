namespace SkillCraft.Contracts.Items.Properties;

public interface IContainerProperties
{
  double? Capacity { get; } // TODO(fpion): nullability
  double? Volume { get; } // TODO(fpion): nullability
}
