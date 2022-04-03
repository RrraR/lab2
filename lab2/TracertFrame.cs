using System.Collections.Generic;

namespace lab2
{
    public class TracertFrame
    {
        public string RemoteIP { get; set; }
        public int HopNumber { get; set; }
        public bool Success { get; set; }
        public List<int> Attempts { get; set; }


        public TracertFrame()
        {
            Success = false;
            Attempts = new List<int>();
            RemoteIP = "EMPTY_STR";
        }
    }
}