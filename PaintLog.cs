using System;

namespace paintMachine
{
    // Veritabanındaki geçmiş boyama kayıtlarını (log) temsil eden model sınıfımız
    // DataGridView tablosunda geçmiş boyama geçmişini göstermek için kullanılır
    public class PaintLog
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }     //Dökülen boyanın adı (Null alabilir)
        public double Servo1UsedMl { get; set; }   //1.Servodan harcanan boya miktarı (ml)
        public double Servo2UsedMl { get; set; }   //2.Servodan harcanan boya miktarı (ml)
        public DateTime CreatedDate { get; set; }  //İşlemin gerçekleştiği tarih ve saat
    }
}