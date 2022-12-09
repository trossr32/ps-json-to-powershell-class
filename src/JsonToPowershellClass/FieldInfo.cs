using JsonToPowershellClass.Core.Helpers;

namespace JsonToPowershellClass.Core;

public class FieldInfo
{
    public string MemberName { get; }
    public JsonType Type { get; }

    public FieldInfo(string jsonMemberName, JsonType type, bool usePascalCase)
    {
        MemberName = jsonMemberName;

        if (usePascalCase) 
            MemberName = MemberName.ToTitleCase();
        
        Type = type;
    }
}