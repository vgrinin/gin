namespace Gin.Editors
{
    public class ComboBoxItem
    {
        public string Display { get; set; }
        public object Value { get; set; }
        public ComboBoxItem()
        {
            Display = "";
            Value = new object();
        }
        public override string ToString()
        {
            return Display;
        }
    }
}