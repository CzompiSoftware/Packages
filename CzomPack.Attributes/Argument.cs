namespace CzomPack.Attributes
{
    [ToString]
    public partial class Argument
    {
        
        public string Name { get; internal set; }
        public string? Value { get; internal set; }
    }
}
