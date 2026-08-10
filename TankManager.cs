using System;

namespace paintMachine
{
    //Depo seviyelerini ve donanım tetikleme sürelerini yöneten iş mantığı (Business Logic) sınıfımız
    public class TankManager
    {
        public const double MaxCapacity = 1000.0;     //Maksimum depo kapasitesi (1000 ml)
        public const int FixedPressTimeMs = 1000;    //10 ml döküm için servoların basılı tutulma süresi (1 saniye)

        //private set:Depo seviyeleri dışarıdan doğrudan değiştirilemez, sadece bu sınıfın metotları güncelleyebilir (Kapsülleme)
        //Servo sayısını dinamik yapmak için List<Tank> kullanılabilir
        public double Servo1TankMl { get; private set; } = MaxCapacity;
        public double Servo2TankMl { get; private set; } = MaxCapacity;

        //Veritabanından okunan mevcut tank seviyelerini iş modeline yükler
        public void SetLevels(double s1, double s2)
        {
            //Math.Clamp: Değerleri 0 ile MaxCapacity (1000) arasında sınırlar
            Servo1TankMl = Math.Clamp(s1, 0, MaxCapacity);
            Servo2TankMl = Math.Clamp(s2, 0, MaxCapacity);
        }

        //1. Hazneye boya ekler; dökülen miktarı ve taşma durumu olup olmadığını Tuple (eklenenMiktar, tasmaVarMi) olarak döner
        public (double actualAdded, bool overflow) AddPaintToTank1(double amount)
        {
            double availableSpace = MaxCapacity - Servo1TankMl;
            double actualAdded = Math.Min(amount, availableSpace);
            Servo1TankMl += actualAdded;
            return (actualAdded, amount > availableSpace);
        }

        //2. Hazneye boya ekler
        public (double actualAdded, bool overflow) AddPaintToTank2(double amount)
        {
            double availableSpace = MaxCapacity - Servo2TankMl;
            double actualAdded = Math.Min(amount, availableSpace);
            Servo2TankMl += actualAdded;
            return (actualAdded, amount > availableSpace);
        }

        //Boyama yapıldığında depodaki boya miktarını harcanan ml kadar eksiltir
        public void ConsumePaint(double s1Ml, double s2Ml)
        {
            Servo1TankMl = Math.Max(0, Servo1TankMl - s1Ml);
            Servo2TankMl = Math.Max(0, Servo2TankMl - s2Ml);
        }

        //Reçetedeki ml miktarlarına göre (10ml veya 0ml) Arduino'ya gönderilecek sabit kalma sürelerini (ms) hesaplar.
        public (int s1Ms, int s2Ms) CalculateDurations(double s1Ml, double s2Ml)
        {
            int s1Ms = s1Ml > 0 ? FixedPressTimeMs : 0;
            int s2Ms = s2Ml > 0 ? FixedPressTimeMs : 0;
            return (s1Ms, s2Ms);
        }
    }
}