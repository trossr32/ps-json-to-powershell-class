using System.Management.Automation;
using Flurl.Http;
using JsonToPowershellClass.Core.Enums;
using JsonToPowershellClass.Core.Models;
using JsonToPowershellClass.Core.Services;
using TextCopy;

namespace PsJsonToPowershellClass.Cmdlets;

/// <summary>
/// <para type="synopsis">
/// Convert JSON to Powershell classes
/// </para>
/// <para type="description">
/// Convert JSON to Powershell classes.
/// JSON can be supplied as a string, a file that will be read or a URL that will be downloaded.
/// </para>
/// <para type="description">
/// Optionally create functions showing example usage.
/// </para>
/// <para type="description">
/// Optionally write an output file.
/// </para>
/// <para type="description">
/// Optionally copy the output to clipboard.
/// </para>
/// <example>
///     <para>Example 1: Convert a JSON file to Powershell classes including usage examples and copy the output to clipboard.</para>
///     <code>PS C:\> Convert-JsonToPowershellClass -JsonFile 'C:\Temp\a-json-file.json' -CopyToClipboard -IncludeExamples</code>
/// </example>
/// <para type="link" uri="(https://github.com/trossr32/ps-json-to-powershell-class)">[Github]</para>
/// </summary>
[Cmdlet(VerbsData.Convert, "JsonToPowershellClass", HelpUri = "https://github.com/trossr32/ps-json-to-powershell-class")]
public class ConvertJsonToPowershellClassCmdlet : PSCmdlet
{
    /// <summary>
    /// <para type="description">
    /// JSON string
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
    public string Json { get; set; }

    /// <summary>
    /// <para type="description">
    /// JSON file name relative to current location, or relative path and JSON file name, or full path of JSON file
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipeline = false, ValueFromPipelineByPropertyName = true, Position = 1)]
    public string JsonFile { get; set; }

    /// <summary>
    /// <para type="description">
    /// URL to download JSON from
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipeline = false, ValueFromPipelineByPropertyName = true, Position = 2)]
    public string Url { get; set; }

    /// <summary>
    /// <para type="description">
    /// The root object class name. If not supplied this will default to 'RootObject'
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipeline = false, ValueFromPipelineByPropertyName = true, Position = 3)]
    public string RootObjectClassName { get; set; } = "RootObject";

    /// <summary>
    /// <para type="description">
    /// If supplied, the result will be written to this output file. The function expects a *.ps1 file, so if a
    /// path is supplied that doesn't end with '.ps1' then '.ps1' will be appended to the end of the parameter.
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false, ValueFromPipeline = false, ValueFromPipelineByPropertyName = true, Position = 4)]
    public string OutputFile { get; set; }

    /// <summary>
    /// <para type="description">
    /// If supplied the data uri will be saved to the clipboard.
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false)]
    public SwitchParameter CopyToClipboard { get; set; }

    /// <summary>
    /// <para type="description">
    /// If supplied the output classes will have example functions showing usages appended to the bottom
    /// </para>
    /// </summary>
    [Parameter(Mandatory = false)]
    public SwitchParameter IncludeExamples { get; set; }

    private string _json;
    private JsonSourceWrapper _jsonSourceWrapper;

    /// <summary>
    /// Implements the <see cref="BeginProcessing"/> method for <see cref="ConvertJsonToPowershellClassCmdlet"/>.
    /// </summary>
    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        if (!string.IsNullOrWhiteSpace(Json))
        {
            _json = Json;
            _jsonSourceWrapper = new JsonSourceWrapper
            {
                Source = InputSource.FromString
            };

            return;
        }

        if (!string.IsNullOrWhiteSpace(JsonFile))
        {
            if (!Path.GetExtension(JsonFile).Equals(".json", StringComparison.OrdinalIgnoreCase))
                ThrowTerminatingError(new ErrorRecord(new Exception("Input file name must be in the *.json format."), null, ErrorCategory.InvalidArgument, null));

            // ensure file path is correct
            JsonFile = File.Exists(JsonFile)
                ? JsonFile
                : Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, JsonFile.TrimStart('.', '/'));

            if (!File.Exists(JsonFile))
                ThrowTerminatingError(new ErrorRecord(new Exception($"Unable to find file: {JsonFile}"), null, ErrorCategory.ObjectNotFound, null));

            _json = File.ReadAllText(JsonFile);

            _jsonSourceWrapper = new JsonSourceWrapper
            {
                Source = InputSource.FromFile,
                FileName = JsonFile
            };

            return;
        }

        if (!string.IsNullOrWhiteSpace(Url))
        {
            try
            {
                _json = Url.GetStringAsync().Result;

                _jsonSourceWrapper = new JsonSourceWrapper
                {
                    Source = InputSource.FromUrl,
                    Url = Url
                };

                return;
            }
            catch (Exception e)
            {
                ThrowTerminatingError(new ErrorRecord(new Exception($"Unable to get json from URL: {Url} error: {e}", e), null, ErrorCategory.InvalidArgument, null));
            }
        }

        ThrowTerminatingError(new ErrorRecord(new Exception("Please supply at least one input parameter."), null, ErrorCategory.InvalidArgument, null));
    }

    /// <summary>
    /// Implements the <see cref="ProcessRecord"/> method for <see cref="ConvertJsonToPowershellClassCmdlet"/>.
    /// </summary>
    protected override void ProcessRecord()
    {

    }

    /// <summary>
    /// Implements the <see cref="EndProcessing"/> method for <see cref="ConvertJsonToPowershellClassCmdlet"/>.
    /// Retrieve all torrents
    /// </summary>
    protected override void EndProcessing()
    {
        string result = null;

        try
        {
            result = new JsonClassGeneratorService()
                .GenerateClasses(_json, _jsonSourceWrapper, RootObjectClassName, false, IncludeExamples.IsPresent);

            if (result is null)
                ThrowTerminatingError(new ErrorRecord(new Exception("Failed to convert json to classes"), null, ErrorCategory.OperationStopped, null));
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(new Exception($"Failed to convert json to classes with error: {e.Message}", e), null, ErrorCategory.OperationStopped, null));
        }
        
        try
        {
            if (!string.IsNullOrWhiteSpace(OutputFile))
            {
                OutputFile = Path.GetExtension(OutputFile).Equals(".ps1", StringComparison.OrdinalIgnoreCase)
                    ? OutputFile
                    : $"{OutputFile}.ps1";

                File.WriteAllText(OutputFile, result);
            }

            if (CopyToClipboard.IsPresent)
                ClipboardService.SetText(result);

            WriteObject(result);

            if (!string.IsNullOrWhiteSpace(OutputFile))
            {
                WriteObject("");
                WriteObject($"Output file created successfully: {OutputFile}");
            }
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(new Exception($"Failed to convert json to classes with error: {e.Message}", e), null, ErrorCategory.OperationStopped, null));
        }
    }
}