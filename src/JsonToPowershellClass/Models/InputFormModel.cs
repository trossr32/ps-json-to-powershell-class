﻿namespace JsonToPowershellClass.Core.Models;

public class InputFormModel
{
    public string Json { get; set; } = Constants.Constants.ExampleJson;
    public string Url { get; set; }
    public string ClassName { get; set; }
    public bool Pascal { get; set; }
    public bool AddExampleFunctions { get; set; } = true;
}