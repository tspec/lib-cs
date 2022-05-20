using System.Collections.Generic;

namespace Tspec.Core
{
    public class Row
    {
        private readonly Dictionary<string, string> _cells = new Dictionary<string, string>();
        
        public Row(string[] columns, string[] values)
        {
            if (columns == null) return;
            if (values == null) return;
            for (var i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                if (i > values.Length - 1) break;
                _cells[column] = values[i];
            }
        }

        public string GetCell(string title)
        {
            if (_cells.TryGetValue(title ?? "", out var cell))
            {
                return cell;
            }

            return "";
        }
    }
}