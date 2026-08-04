using System;

namespace paintMachine
{
    public class PaintLog
    {
        public int Id { get; set; }
        public string? ColorName { get; set; }//null olabilir o yüzden:?
        public double Servo1UsedMl { get; set; }
        public double Servo2UsedMl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}