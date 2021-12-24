using System.Globalization;
using System.Xml.Linq;

namespace RDLCDynamicColumns
{
    public class RDLCColumn
    {
        private readonly XElement _widthElement;
        private readonly XElement _memberElement;
        private readonly bool _visibility;
        private decimal _addedWidth;

        public RDLCColumn(XElement widthElement, XElement memberElement, bool visibility)
        {
            _widthElement = widthElement;
            _memberElement = memberElement;
            _visibility = visibility;
            _addedWidth = 0m;
        }

        public void AddWidth(decimal value)
            => _addedWidth += value;

        public bool IsVisible()
            => _visibility;

        public decimal Width()
        {
            string value = _widthElement.Value;
            value = value.Remove(value.Length - 2, 2);
            value = value.Replace(",", ".");

            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        public void UpdateElementWidth()
        {
            if (!_visibility)
                _widthElement.SetValue("0cm");

            if (_addedWidth == 0m)
                return;

            decimal newWidth = Width() + _addedWidth;
            _widthElement.SetValue($"{newWidth.ToString("N6").Replace(",", ".")}cm");
        }

        public void SetVisibility()
        {
            if (!_visibility)
                _memberElement.Add(HideElement(_memberElement.Name.Namespace));
        }

        private static XElement HideElement(XNamespace ns)
        {
            return new XElement(ns + "Visibility",
                new XElement(ns + "Hidden", "true")
            );
        }
    }
}