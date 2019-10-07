using System;

namespace NotifyIRPAppointment {
    class Program {
        private static void Main(string[] args)
        {
            var getter = new WebGetter();
            var timer = new TimerProcess(getter.GetAppointments);
            timer.Start();

            Console.Read();
        }
    }
}