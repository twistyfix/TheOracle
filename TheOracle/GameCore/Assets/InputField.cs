namespace TheOracle.GameCore.Assets
{
    public class InputField
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public override string ToString()
        {
            return Title;
        }
    }
}