namespace JsonToPowershellClass.Core.Constants;

public static class Constants
{
    public const long MaxFileSize = 1024 * 1024 * 50;

    public const string ExampleJson = """
                                      {
                                        "id": 69,
                                        "title": "The Hitchhiker's Guide to the Galaxy",
                                        "author": "Douglas Adams",
                                        "publisher": "Megadodo Publications",
                                        "description": "A wholly remarkable book",
                                        "price": 73.57,
                                        "discount": 13.37,
                                        "stock": 420,
                                        "category": "ebook",
                                        "tags": [
                                          "mostly harmless",
                                          "ultimate question",
                                          "ultimate answer"
                                        ]
                                      }
                                      """;

    public const string PowershellCommentAppDescription = """
                                                          <#
                                                          .SYNOPSIS
                                                              Online converter of JSON to Powershell classes.

                                                          .DESCRIPTION
                                                              To convert a JSON object to a Powershell class,
                                                              either paste JSON into the JSON code editor or
                                                              enter the URL of an API endpoint into the Url
                                                              text box and click the 'Generate class' button.

                                                          .PARAMETER Url
                                                              Used to convert a JSON object returned from an
                                                              API endpoint.

                                                          .PARAMETER ClassName
                                                              By default, the parent class will be named
                                                              "RootObject". You can change this by entering
                                                              a new name in the text box.

                                                          .PARAMETER PascalCase
                                                              If selected, the class name(s) will be converted
                                                              to Pascal case.

                                                          .PARAMETER AddExampleFunctions
                                                              If selected, example functions will be added to
                                                              the generated code to show how to use the class.
                                                          #>
                                                          """;
}