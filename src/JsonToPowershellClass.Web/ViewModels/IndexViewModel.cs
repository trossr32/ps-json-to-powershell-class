namespace JsonToPowershellClass.Web.ViewModels;

public class IndexViewModel
{
    public string ClassName { get; set; }
    public string Json { get; set; }
    public string CodeObjects { get; set; }
    
    public bool Pascal { get; set; }
    public bool AddExampleFunctions { get; set; }
    
    public bool Error { get; set; }
    public int ErrorNo { get; set; }
}