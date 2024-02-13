using BrailleToolkit.Data;
using GenerateYaml;

var engTable = EnglishBrailleTable.GetInstance();
var yaml = BrailleXmlToYaml.ToYamlString(engTable);
Console.WriteLine(yaml);

string filePath = @"d:/temp/english.yaml";
File.WriteAllText(filePath, yaml);
Console.WriteLine($"Output: {filePath}");
