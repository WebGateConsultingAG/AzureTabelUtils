using System;
namespace WebGate.Azure.TableUtils.Test;

public class MainWithParent
{
    public string? Id { set; get; }
    public ParentPoco? Parent { set; get; }

    public SimplePoco? Child { set; get; }

    public static MainWithParent CreateMainWithParent()
    {
        MainWithParent mwp = new MainWithParent
        {
            Child = SimplePoco.CreateInitializedPoco(),
            Parent = ParentPoco.CreateParentWithChild(),
            Id = "01820281"
        };
        return mwp;
    }
}
