using BrailleToolkit.Data;
using GenerateYaml;

var engTable = EnglishBrailleTable.GetInstance();
var yaml = BrailleXmlToYaml.ToYamlString(engTable);
string filePath = @"d:/temp/english.yaml";
File.WriteAllText(filePath, yaml);
Console.WriteLine($"Output: {filePath}");

var chnTable = ChineseBrailleTable.GetInstance();
yaml = BrailleXmlToYaml.ToYamlString(chnTable);
filePath = @"d:/temp/chinese.yaml";
File.WriteAllText(filePath, yaml);
Console.WriteLine($"Output: {filePath}");
