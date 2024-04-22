using System;
namespace WebGate.Azure.TableUtils.Test;

public class ParentPoco
{
    public string? Id { get; set; }
    public SimplePoco? Child { set; get; }

    public static ParentPoco CreateParentWithChild()
    {
        ParentPoco pp = new ParentPoco
        {
            Child = SimplePoco.CreateInitializedPoco(),
            Id = "02830128101"
        };
        return pp;
    }
}
