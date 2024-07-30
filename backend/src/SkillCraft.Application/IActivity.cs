namespace SkillCraft.Application;

public interface IActivity
{
  IActivity Anonymize();
  void Contextualize(ActivityContext context);
}
