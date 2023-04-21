using System.ComponentModel.DataAnnotations;

namespace JsonToPowershellClass.Blazor.Models;

public class InputFormModel
{
    [Required]
    public string Json { get; set; }
    public string Url { get; set; }
    public string ClassName { get; set; }
    public bool Pascal { get; set; }
    public bool AddExampleFunctions { get; set; } = true;

    //public bool Error { get; set; }
    //public int ErrorNo { get; set; }
}