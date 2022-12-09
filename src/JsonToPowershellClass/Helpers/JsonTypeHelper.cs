using JsonToPowershellClass.Core.Enums;

namespace JsonToPowershellClass.Core.Helpers;

public static class JsonTypeHelper
{
    public static string GetTypeName(this JsonType type, bool noBrackets = false)
    {
        //var arraysAsLists = !service.ExplicitDeserialization;

        return type.Type switch
        {
            JsonTypeEnum.Anything => "[object]",
            JsonTypeEnum.Array => $"{(noBrackets ? "" : "[")}" +
                                  type.InternalType.GetTypeName(true)
                                      .Replace("[", "")
                                      .Replace("]", "") +
                                  $"[]{(noBrackets ? "" : "]")}",
            //JsonTypeEnum.Dictionary => $"Dictionary<string, {GetTypeName(type.InternalType, config)}>",
            JsonTypeEnum.Boolean => "[bool]",
            JsonTypeEnum.Float => "[double]",
            JsonTypeEnum.Integer => "[int]",
            JsonTypeEnum.Long => "[long]",
            JsonTypeEnum.Date => "[DateTime]",
            JsonTypeEnum.NonConstrained => "[object]",
            JsonTypeEnum.NullableBoolean => "[bool]",
            JsonTypeEnum.NullableFloat => "[double]",
            JsonTypeEnum.NullableInteger => "[int]",
            JsonTypeEnum.NullableLong => "[long]",
            JsonTypeEnum.NullableDate => "[DateTime]",
            JsonTypeEnum.NullableSomething => "[object]",
            JsonTypeEnum.Object => $"[{type.AssignedName}]",
            JsonTypeEnum.String => "[string]",
            _ => throw new NotSupportedException("Unsupported json type")
        };
    }
}