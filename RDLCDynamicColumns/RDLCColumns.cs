using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace RDLCDynamicColumns
{
    public class RDLCColumns
    {
        private readonly string _tableName;
        private readonly List<bool> _columnVisibilities;

        private readonly XElement _element;
        private readonly Lazy<List<RDLCColumn>> _columns;

        public RDLCColumns(string rdlcXml, string tableName, params bool[] columnVisibilities)
        {
            _tableName = tableName;
            _columnVisibilities = new List<bool>(columnVisibilities);

            if (!_columnVisibilities.Any(b => b))
                throw new Exception("There is no visible column");

            _element = XElement.Parse(rdlcXml);
            _columns = new Lazy<List<RDLCColumn>>(() => Columns(_element).ToList());
        }

        private IEnumerable<RDLCColumn> Columns(XElement element)
        {
            string tablePath = $"//Tablix[@Name='{_tableName}']".ConvertToInterpretation(XmlInterpretation.WithoutNamespace);
            XElement table = element.XPathSelectElements(tablePath).FirstOrDefault();

            string columnPath = ".//TablixColumn/Width".ConvertToInterpretation(XmlInterpretation.WithoutNamespace);
            List<XElement> columnsWidthElements = table.XPathSelectElements(columnPath).ToList();

            string columnHierarchyPath = ".//TablixColumnHierarchy/TablixMembers/TablixMember".ConvertToInterpretation(XmlInterpretation.WithoutNamespace);
            List<XElement> columnMembers = table.XPathSelectElements(columnHierarchyPath).ToList();

            if (columnsWidthElements.Count != columnMembers.Count)
                throw new Exception("Column width count found does not match column member count");

            if (columnsWidthElements.Count != _columnVisibilities.Count)
                throw new Exception("Columns count in table does not match the amount of visibilities in constructor");

            for (int i = 0; i < columnsWidthElements.Count; i++)
                yield return new RDLCColumn(columnsWidthElements[i], columnMembers[i], _columnVisibilities[i]);
        }

        public string UpdatedXml()
        {
            SetVisibility();
            SpreadInvisibleColumnWidths();
            UpdateXElementWidths();

            return _element.ToString();
        }

        private void SetVisibility()
        {
            foreach (RDLCColumn column in _columns.Value)
                column.SetVisibility();
        }

        private void SpreadInvisibleColumnWidths()
        {
            List<RDLCColumn> visibleColumns = _columns.Value.Where(c => c.IsVisible()).ToList();
            int visibleColumnCount = visibleColumns.Count;

            foreach (RDLCColumn column in _columns.Value)
            {
                if (!column.IsVisible())
                {
                    decimal width = column.Width();
                    decimal partialWidth = width / visibleColumnCount;

                    foreach (RDLCColumn visibleColumn in visibleColumns)
                        visibleColumn.AddWidth(partialWidth);
                }
            }
        }

        private void UpdateXElementWidths()
        {
            foreach (RDLCColumn column in _columns.Value)
                column.UpdateElementWidth();
        }
    }
}