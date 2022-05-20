namespace Tspec.Core
{
    public class SpecDef
    {
        public string Text { get; set; }
        public Table Table { get; set; }
        
        public override string ToString()
        {
            return $"[text:{Text}, hasTable:{Table != null}]";
        }
    }
}