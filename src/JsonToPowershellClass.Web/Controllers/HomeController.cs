using System.Net;
using Flurl.Http;
using JsonToPowershellClass.Core.Enums;
using JsonToPowershellClass.Core.Models;
using JsonToPowershellClass.Core.Services;
using Microsoft.AspNetCore.Mvc;
using JsonToPowershellClass.Web.ViewModels;

namespace JsonToPowershellClass.Web.Controllers;

public class HomeController : Controller
{
    private readonly IJsonClassGeneratorService _jsonClassGeneratorSvc;

    public HomeController(IJsonClassGeneratorService jsonClassGeneratorSvc)
    {
        _jsonClassGeneratorSvc = jsonClassGeneratorSvc;
    }

    [HttpGet]
    public async Task<IActionResult> Index() => View(new IndexViewModel());

    [HttpPost]
    public async Task<IActionResult> Index(IndexViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ClassName))
        {
            model.ClassName = "RootObject";
        }
        else if (string.IsNullOrWhiteSpace(model.Json))
        {
            model.Error = true;
            model.ErrorNo = 2;
        }

        //todo - add description to index to say url can be used or create separate URL text box
        if (model.Json?.ToLower().StartsWith("http") ?? false)
        {
            try
            {
                model.Json = await model.Json.GetStringAsync();
            }
            catch (Exception)
            {
                model.Error = true;
                model.ErrorNo = 4;
            }
        }

        if (model.Error)
            return View(model);
            
        try
        {
            model.CodeObjects = WebUtility.HtmlEncode(Prepare(model.Json, model.ClassName, model.Pascal, model.AddExampleFunctions));
        }
        catch (Exception)
        {
            model.Error = true;
            model.ErrorNo = 3;               
        }
            
        return View(model);
    }

    private string Prepare(string json, string classname, bool pascal, bool addExampleFunctions) =>
        string.IsNullOrWhiteSpace(json) 
            ? null 
            : _jsonClassGeneratorSvc.GenerateClasses(json, new JsonSourceWrapper{Source = InputSource.FromString}, classname, pascal, addExampleFunctions);

    //private string Result { get; set; }

    //public void R(object d)
    //{
    //    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(d))
    //    {
    //        string name = descriptor.Name;
    //        object value = descriptor.GetValue(d);
    //        Result += $"{name}={value}";

    //        if (value is JToken {Type: JTokenType.Object} token)
    //            R(token);
    //    }
    //}
}