namespace paintMachine
{
    //Veritabanındaki renk reçetelerini nesne olarak temsil eden model sınıfımız
    public class ColorRecipe
    {
        public int Id { get; set; }

        //Bu alanın null değer olabileceğini derleyiciye bildirir.
        public string? ColorName { get; set; }  //Renk adı (Kırmızı, Sarı, Turuncu)
        public string? HexCode { get; set; }    //Arayüzdeki renk önizleme kutusu için HEX kodu (#FF0000 vb.)

        // Hangi hazneden kaç ml döküleceğini belirten miktarlar
        public double Servo1Ml { get; set; }
        public double Servo2Ml { get; set; }

        //C#'taki temel Object sınıfının varsayılan ToString() metodunu ezer.
        // Böylece ComboBox bu nesneyi listelerken sınıf adı yerine doğrudan renk adını (ColorName) gösterir.
        public override string ToString()
        {
            //??: ColorName null ise hata vermesini önleyip boş metin ("") döndürür.
            return ColorName ?? string.Empty;
        }
    }
}