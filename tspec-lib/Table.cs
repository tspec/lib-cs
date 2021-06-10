using System.Collections.Generic;
using System.Text;

namespace TSpec.Lib
{
    public class Table
    {
        private readonly List<Row> _rows = new List<Row>();
        
        public IEnumerable<Row> GetTableRows() => _rows;

        public string[] Columns { get; set; }

        public void AddRow(string[] values) => _rows.Add(new Row(Columns, values));

        public override string ToString()
        {
            var builder = new StringBuilder();

            if (Columns == null) return "[]";
            
            builder.Append("[|");
            builder.Append(string.Join("|", Columns));
            builder.AppendLine("|]");

            foreach (var row in _rows)
            {
                builder.Append("[|");
                foreach (var column in Columns) builder.Append($"{row.GetCell(column)}|]");
                builder.AppendLine();
            }
            
            return builder.ToString();
        }
    }
}