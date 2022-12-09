using JsonToPowershellClass.Core.Enums;

namespace JsonToPowershellClass.Core.Models;

public class JsonSourceWrapper
{
    public InputSource Source { get; set; }
    public string FileName { get; set; }
    public string Url { get; set; }
}