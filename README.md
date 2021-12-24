# RDLCDynamicColumns
Code example on how to show/hide columns and have others fill the gap

```
string reportXml = File.ReadAllText("./SomeReport.rdlc");

string tablixName = "SomeTable";
bool[] visibilityForEachColumn = { true, false, true };

var columns = new RDLCColumns(reportXml, tablixName, visibilityForEachColumn);
string updatedXml = columns.UpdatedXml();

TextReader reader = new StringReader(updatedXml);
reportView1.LocalReport.LoadReportDefinition(reader);
...
Other C# stuff for parameters and such
...
reader.Close();
```
