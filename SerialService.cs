using System;
using System.IO.Ports;

namespace paintMachine
{
    //Bilgisayar ile Arduino arasındaki Seri Port (COM) haberleşmesini yöneten servis sınıfımız
    public class SerialService
    {
        private SerialPort? _serialPort;

        //Seri portun bağlı ve açık olup olmadığını kontrol eden salt okunur özellik
        public bool IsConnected => _serialPort != null && _serialPort.IsOpen;

        //Belirtilen COM portuna (Varsayılan: COM7) ve Baud Rate hızına (9600) bağlanmayı dener.
        public bool Connect(string portName = "COM7", int baudRate = 9600)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate)
                {
                    WriteTimeout = 1000, //Yazma zaman aşımı süresi (1 saniye)
                    ReadTimeout = 1000   //Okuma zaman aşımı süresi (1 saniye)
                };
                _serialPort.Open();
                return true;
            }
            catch
            {
                //Bağlantı başarısız olursa (port bulunamadı veya başka program kullanıyor) false döner
                return false;
            }
        }

        //Arduino'ya servoların bekleme sürelerini virgülle ayrılmış metin olarak gönderir (Örn: "1000,1000\n")
        public bool SendDurations(int servo1Ms, int servo2Ms)
        {
            if (!IsConnected) return false;

            try
            {
                //Satır sonu karakteri (\n), Arduino'daki Serial.readStringUntil('\n') komutunun veriyi bitirmesini sağlar
                _serialPort!.Write($"{servo1Ms},{servo2Ms}\n");
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Seri port bağlantısını güvenli bir şekilde kapatır ve sistem kaynaklarını serbest bırakır
        public void Disconnect()
        {
            if (IsConnected)
            {
                _serialPort?.Close();
                _serialPort?.Dispose();
            }
        }
    }
}