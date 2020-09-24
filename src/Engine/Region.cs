namespace Engine
{
    public class Region
    {
        public static Region Ru => new Region("ru");

        public static Region Eu => new Region("eu");

        public static Region Na => new Region("na");

        public static Region Asia => new Region("asia");

        private Region(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}