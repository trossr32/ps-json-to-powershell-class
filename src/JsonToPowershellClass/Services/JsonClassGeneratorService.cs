using System.Text;
using JsonToPowershellClass.Core.Enums;
using JsonToPowershellClass.Core.Helpers;
using JsonToPowershellClass.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonToPowershellClass.Core.Services;

public interface IJsonClassGeneratorService
{
    /// <summary>
    /// Generate classes from the supplied input form model
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    string GenerateClasses(InputFormModel model);

    /// <summary>
    /// Generate classes from the supplied json
    /// </summary>
    /// <param name="json"></param>
    /// <param name="source">How the JSON was passed</param>
    /// <param name="rootObjectClassName">Root class name. If null or whitespace then defaults to 'RootObject'</param>
    /// <param name="usePascalCase">Whether to use pascal case for property names</param>
    /// <param name="addExampleFunctions">whether to include example function output</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    string GenerateClasses(string json, JsonSourceWrapper source, string rootObjectClassName, bool usePascalCase = false, bool addExampleFunctions = false);
}

public class JsonClassGeneratorService : IJsonClassGeneratorService
{
    private const string RootObject = nameof(RootObject);

    private List<JsonType> _types = new();
    private List<string> _classNames = new();
    private StringBuilder _stringBuilder = new();
    private List<string> _writtenClasses = new();

    /// <inheritdoc/>
    public string GenerateClasses(InputFormModel model) =>
        GenerateClasses(model.Json, new JsonSourceWrapper { Source = InputSource.FromString }, model.ClassName, model.Pascal, model.AddExampleFunctions);

    /// <inheritdoc/>
    public string GenerateClasses(string json, JsonSourceWrapper source, string rootObjectClassName, bool usePascalCase = false, bool addExampleFunctions = false)
    {
        // initialise
        _types = new List<JsonType>();
        _classNames = new List<string>();
        _stringBuilder = new StringBuilder();
        _writtenClasses = new List<string>();
        
        JObject[] objects;

        using (var reader = new StringReader(json))
        using (var jsonReader = new JsonTextReader(reader))
        {
            objects = JToken.ReadFrom(jsonReader) switch
            {
                JArray array => array.Cast<JObject>().ToArray(),
                JObject jObject => new[] { jObject },
                _ => throw new Exception("Sample JSON must be either a JSON array, or a JSON object.")
            };
        }
        
        var rootType = new JsonType(this, objects[0]);

        rootType.AssignName(string.IsNullOrWhiteSpace(rootObjectClassName) ? RootObject : rootObjectClassName);

        GenerateClass(objects, rootType, usePascalCase);

        foreach (var type in _types)
        {
            WriteClass(type);
        }

        if (addExampleFunctions)
            WriteExampleFunctions(source);

        return _stringBuilder.ToString();
    }

    private void GenerateClass(IReadOnlyCollection<JObject> objects, JsonType type, bool usePascalCase)
    {
        var jsonFields = new Dictionary<string, JsonType>();

        var first = true;

        foreach (var obj in objects)
        {
            foreach (var prop in obj.Properties())
            {
                var currentType = new JsonType(this, prop.Value);
                var propName = prop.Name;

                if (jsonFields.TryGetValue(propName, out var fieldType))
                {
                    var commonType = fieldType.GetCommonType(currentType);

                    jsonFields[propName] = commonType;
                }
                else
                {
                    var commonType = currentType;

                    commonType = first
                        ? commonType
                        : commonType.GetCommonType(JsonType.GetNull(this));

                    jsonFields.Add(propName, commonType);
                }
            }

            first = false;
        }
        
        foreach (var (key, fieldType) in jsonFields)
        {
            List<JObject> nestedObjects;

            if (fieldType.Type == JsonTypeEnum.Object)
            {
                nestedObjects = new List<JObject>(objects.Count);

                foreach (var obj in objects)
                {
                    if (!obj.TryGetValue(key, out var value))
                        continue;

                    if (value.Type == JTokenType.Object)
                        nestedObjects.Add((JObject)value);
                }

                fieldType.AssignName(key.ToTitleCase());

                if (!_classNames.Contains(key))
                {
                    GenerateClass(nestedObjects.ToArray(), fieldType, usePascalCase);

                    _classNames.Add(key);
                }
            }

            if (fieldType.InternalType is not { Type: JsonTypeEnum.Object })
                continue;

            nestedObjects = new List<JObject>(objects.Count);

            foreach (var obj in objects)
            {
                if (!obj.TryGetValue(key, out var value))
                    continue;

                switch (value.Type)
                {
                    case JTokenType.Array:
                        {
                            foreach (var item in (JArray)value)
                            {
                                if (item is not JObject jObject)
                                    throw new NotSupportedException("Arrays of non-objects are not supported yet.");

                                nestedObjects.Add(jObject);
                            }

                            break;
                        }

                    case JTokenType.Object:
                        {
                            foreach (var item in (JObject)value)
                            {
                                if (item.Value is not JObject jObject)
                                    throw new NotSupportedException("Arrays of non-objects are not supported yet.");

                                nestedObjects.Add(jObject);
                            }

                            break;
                        }
                }
            }

            fieldType.InternalType.AssignName(key.ToTitleCase(true));

            GenerateClass(nestedObjects.ToArray(), fieldType.InternalType, usePascalCase);
        }

        type.Fields = jsonFields
            .Select(x => new FieldInfo(x.Key, x.Value, usePascalCase))
            .ToArray();

        _types.Add(type);
    }
    
    private void WriteClass(JsonType type)
    {
        // prevent writing multiple instances of the same class
        if (_writtenClasses.Any(wc => wc == type.AssignedName))
            return;

        _stringBuilder.AppendLine($"class {type.AssignedName}");
        _stringBuilder.AppendLine("{");

        WriteClassMembers(type, "    ");

        _stringBuilder.AppendLine("}");
        _stringBuilder.AppendLine();

        _writtenClasses.Add(type.AssignedName);
    }

    private void WriteClassMembers(JsonType type, string prefix)
    {
        foreach (var field in type.Fields)
        {
            _stringBuilder.AppendLine($"{prefix}{field.Type.GetTypeName()} ${field.MemberName};");
        }
    }

    private void WriteExampleFunctions(JsonSourceWrapper jsonSource)
    {
        // root class should be the last class written to the _writtenClasses collection
        var rootClass = _writtenClasses.Last();

        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("<#");
        _stringBuilder.AppendLine("    .SYNOPSIS");
        _stringBuilder.AppendLine($"    Deserialize a supplied json string into an object of type [{rootClass}].");
        _stringBuilder.AppendLine("    Response will be either an instance of the class or an array of the class, depending on the JSON input.");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("    .DESCRIPTION");
        _stringBuilder.AppendLine("    Uses ConvertFrom-Json to deserialize a json string into a [pscustomobject] object graph.");
        _stringBuilder.AppendLine("    Parses the [pscustomobject] graph into a strongly typed object graph based on the generated classes.");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("    .PARAMETER Json");
        _stringBuilder.AppendLine("    The json string to be deserialized.");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("    .EXAMPLE");

        switch (jsonSource.Source)
        {
            case InputSource.FromString:
                _stringBuilder.AppendLine($"    Get-{rootClass}Class -Json $json");
                break;

            case InputSource.FromFile:
                _stringBuilder.AppendLine($"    Get-{rootClass}Class -Json ([System.IO.File]::ReadAllText('{jsonSource.FileName}'))");
                break;

            case InputSource.FromUrl:
                _stringBuilder.AppendLine($"    Get-{rootClass}Class -Json ([System.Net.WebClient]::new()).DownloadString('{jsonSource.Url}')");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(jsonSource), jsonSource, null);
        }

        _stringBuilder.AppendLine("#>");
        _stringBuilder.AppendLine($"function Get-{rootClass}Class {{");
        _stringBuilder.AppendLine("    [CmdletBinding()]");
        _stringBuilder.AppendLine("    Param(");
        _stringBuilder.AppendLine("        [Parameter(Mandatory = $true, Position = 0, HelpMessage = \"String representation of json to be deserialized.\")]");
        _stringBuilder.AppendLine("        [string] $Json");
        _stringBuilder.AppendLine("    )");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("    Begin {}");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("    Process {");
        _stringBuilder.AppendLine("        $obj = ConvertFrom-Json $Json");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("        if ($obj -is [array])");
        _stringBuilder.AppendLine("        {");
        _stringBuilder.AppendLine("            $outArr = @()");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("            foreach ($o in $obj) {"); 
        _stringBuilder.AppendLine($"                $outArr + ([{rootClass}] $o)");
        _stringBuilder.AppendLine("            }");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("            return $outArr");
        _stringBuilder.AppendLine("        }");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine($"        return [{rootClass}] (ConvertFrom-Json $Json)");
        _stringBuilder.AppendLine("    }");
        _stringBuilder.AppendLine("");
        _stringBuilder.AppendLine("    End {}");
        _stringBuilder.AppendLine("}");
    }
}