using JsonToPowershellClass.Core.Enums;
using JsonToPowershellClass.Core.Helpers;
using JsonToPowershellClass.Core.Services;
using Newtonsoft.Json.Linq;

namespace JsonToPowershellClass.Core;

public class JsonType
{
    public JsonTypeEnum Type { get; }
    public JsonType InternalType { get; private init; }
    public string AssignedName { get; private set; }


    private readonly IJsonClassGeneratorService _generator;

    public JsonType(IJsonClassGeneratorService generator, JToken token)
    {
        _generator = generator;

        Type = GetFirstTypeEnum(token);

        if (Type != JsonTypeEnum.Array) 
            return;

        var array = (JArray)token;

        InternalType = GetCommonType(generator, array.ToArray());
    }

    private JsonType(IJsonClassGeneratorService generator, JsonTypeEnum type)
    {
        _generator = generator;

        Type = type;
    }

    private static JsonType GetCommonType(IJsonClassGeneratorService generator, IReadOnlyList<JToken> tokens)
    {
        if (!tokens.Any()) 
            return new JsonType(generator, JsonTypeEnum.NonConstrained);

        var common = new JsonType(generator, tokens[0]);

        for (int i = 1; i < tokens.Count; i++)
        {
            var current = new JsonType(generator, tokens[i]);

            common = common.GetCommonType(current);
        }

        return common;
    }

    internal static JsonType GetNull(IJsonClassGeneratorService generator) =>
        new(generator, JsonTypeEnum.NullableSomething);

    public void AssignName(string name) => AssignedName = name;

    public string GetTypeName() => JsonTypeHelper.GetTypeName(this);
    
    public JsonType GetCommonType(JsonType type2)
    {
        var commonType = GetCommonTypeEnum(Type, type2.Type);

        if (commonType is JsonTypeEnum.Array)
        {
            if (type2.Type is JsonTypeEnum.NullableSomething) 
                return this;

            if (Type is JsonTypeEnum.NullableSomething) 
                return type2;

            var commonInternalType = InternalType
                .GetCommonType(type2.InternalType);
            
            if (commonInternalType != InternalType) 
                return new JsonType(_generator, JsonTypeEnum.Array)
                {
                    InternalType = commonInternalType
                };
        }

        return Type == commonType
            ? this
            : new JsonType(_generator, commonType);
    }

    /// <summary>
    /// Whether type is JsonTypeEnum.NullableSomething
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsNull(JsonTypeEnum type) => type is JsonTypeEnum.NullableSomething;

    private static JsonTypeEnum GetCommonTypeEnum(JsonTypeEnum type1, JsonTypeEnum type2)
    {
        if (type1 is JsonTypeEnum.NonConstrained) 
            return type2;

        if (type2 is JsonTypeEnum.NonConstrained) 
            return type1;

        switch (type1)
        {
            case JsonTypeEnum.Boolean:
                if (IsNull(type2)) 
                    return JsonTypeEnum.NullableBoolean;

                if (type2 is JsonTypeEnum.Boolean) 
                    return type1;

                break;

            case JsonTypeEnum.NullableBoolean:
                if (IsNull(type2)) 
                    return type1;

                if (type2 is JsonTypeEnum.Boolean) 
                    return type1;

                break;

            case JsonTypeEnum.Integer:
                if (IsNull(type2)) 
                    return JsonTypeEnum.NullableInteger;

                return type2 switch
                {
                    JsonTypeEnum.Float => JsonTypeEnum.Float,
                    JsonTypeEnum.Long => JsonTypeEnum.Long,
                    JsonTypeEnum.Integer => type1,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.NullableInteger:
                if (IsNull(type2)) 
                    return type1;

                return type2 switch
                {
                    JsonTypeEnum.Float => JsonTypeEnum.NullableFloat,
                    JsonTypeEnum.Long => JsonTypeEnum.NullableLong,
                    JsonTypeEnum.Integer => type1,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.Float:
                if (IsNull(type2)) 
                    return JsonTypeEnum.NullableFloat;

                return type2 switch
                {
                    JsonTypeEnum.Float => type1,
                    JsonTypeEnum.Integer => type1,
                    JsonTypeEnum.Long => type1,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.NullableFloat:
                if (IsNull(type2)) 
                    return type1;

                return type2 switch
                {
                    JsonTypeEnum.Float => type1,
                    JsonTypeEnum.Integer => type1,
                    JsonTypeEnum.Long => type1,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.Long:
                if (IsNull(type2)) 
                    return JsonTypeEnum.NullableLong;

                return type2 switch
                {
                    JsonTypeEnum.Float => JsonTypeEnum.Float,
                    JsonTypeEnum.Integer => type1,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.NullableLong:
                if (IsNull(type2)) 
                    return type1;

                return type2 switch
                {
                    JsonTypeEnum.Float => JsonTypeEnum.NullableFloat,
                    JsonTypeEnum.Integer => type1,
                    JsonTypeEnum.Long => type1,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.Date:
                if (IsNull(type2)) 
                    return JsonTypeEnum.NullableDate;

                if (type2 == JsonTypeEnum.Date) 
                    return JsonTypeEnum.Date;

                break;

            case JsonTypeEnum.NullableDate:
                if (IsNull(type2)) 
                    return type1;

                if (type2 == JsonTypeEnum.Date) 
                    return type1;

                break;

            case JsonTypeEnum.NullableSomething:
                if (IsNull(type2)) 
                    return type1;

                return type2 switch
                {
                    JsonTypeEnum.String => JsonTypeEnum.String,
                    JsonTypeEnum.Integer => JsonTypeEnum.NullableInteger,
                    JsonTypeEnum.Float => JsonTypeEnum.NullableFloat,
                    JsonTypeEnum.Long => JsonTypeEnum.NullableLong,
                    JsonTypeEnum.Boolean => JsonTypeEnum.NullableBoolean,
                    JsonTypeEnum.Date => JsonTypeEnum.NullableDate,
                    JsonTypeEnum.Array => JsonTypeEnum.Array,
                    JsonTypeEnum.Object => JsonTypeEnum.Object,
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.Object:
                if (IsNull(type2)) 
                    return type1;

                return type2 switch
                {
                    JsonTypeEnum.Object => type1,
                    JsonTypeEnum.Dictionary => throw new ArgumentException(),
                    _ => JsonTypeEnum.Anything
                };

            case JsonTypeEnum.Dictionary:
                throw new ArgumentException();

            case JsonTypeEnum.Array:
                if (IsNull(type2)) 
                    return type1;

                if (type2 == JsonTypeEnum.Array) 
                    return type1;

                break;

            case JsonTypeEnum.String:
                if (IsNull(type2)) 
                    return type1;

                if (type2 == JsonTypeEnum.String) 
                    return type1;

                break;
        }

        return JsonTypeEnum.Anything;
    }
    
    private static JsonTypeEnum GetFirstTypeEnum(JToken token)
    {
        var type = token.Type;

        if (type == JTokenType.Integer)
            return ((long?)((JValue)token).Value ?? 0) < int.MaxValue 
                ? JsonTypeEnum.Integer 
                : JsonTypeEnum.Long;

        return type switch
        {
            JTokenType.Array => JsonTypeEnum.Array,
            JTokenType.Boolean => JsonTypeEnum.Boolean,
            JTokenType.Float => JsonTypeEnum.Float,
            JTokenType.Null => JsonTypeEnum.NullableSomething,
            JTokenType.Undefined => JsonTypeEnum.NullableSomething,
            JTokenType.String => JsonTypeEnum.String,
            JTokenType.Object => JsonTypeEnum.Object,
            JTokenType.Date => JsonTypeEnum.Date,
            _ => JsonTypeEnum.Anything
        };
    }

    public IList<FieldInfo> Fields { get; internal set; }
    //public bool IsRoot { get; internal init; }
}