namespace SkillCraft.Contracts.Aspects;

public interface IAttributeSelection
{
  Attribute? Mandatory1 { get; }
  Attribute? Mandatory2 { get; }
  Attribute? Optional1 { get; }
  Attribute? Optional2 { get; }
}
