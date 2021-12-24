# RDLCDynamicColumns

I've not found a solution online to simulate dynamic columns for RDLC table/matrix.
So I've decided to make a code example on how to achieve that by reading the XML from the RDLC and manipulating it before inserting it into the reportView.

This code assumes that you have all column visibilities set to "show".
Based on the param ColumnVisibilities for RDLCColumns it sets them to hidden and redistributes their width to the visible columns.

Example:
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
