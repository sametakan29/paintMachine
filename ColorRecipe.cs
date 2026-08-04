namespace paintMachine
{
    public class ColorRecipe
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }//null olabilir o yüzden:?
        public string? HexCode { get; set; }//null olabilir o yüzden:?
        public double Servo1Ml { get; set; }
        public double Servo2Ml { get; set; }

        // ComboBox'ta sadece renk adının görünmesi için
        public override string ToString()
        {
            return ColorName ?? string.Empty;//null olabilir o yüzden ??
        }
    }
}