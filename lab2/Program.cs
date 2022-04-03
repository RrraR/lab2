using System;
using System.Collections.Generic;
using System.Net;

namespace lab2
{
    static class Program
    {
        private static void GetIp(string addr)
        {
            if (addr != null)
            {
                IPAddress[] addresslist = Dns.GetHostAddresses(addr);

                foreach (IPAddress theaddress in addresslist)
                {
                    Console.WriteLine(theaddress.ToString());
                }
            }
        }

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Enter service name or ip address");
                var destenation = Console.ReadLine();
                string ip = "";
                if (destenation != null)
                {
                    var parseResult = IPAddress.TryParse(destenation, out var Ipaddr);
                    if (!parseResult)
                    {
                        Console.WriteLine("You entered " + destenation);
                        GetIp(destenation);
                        while (true)
                        {
                            Console.WriteLine("Enter ip to trace");
                            var temp = Console.ReadLine();
                            if (temp != null)
                            {
                                var secparseResult = IPAddress.TryParse(temp, out Ipaddr);
                                if (!secparseResult)
                                {
                                    Console.WriteLine("invalid input, try again");
                                }
                                else
                                {
                                    ip = temp;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        ip = destenation;
                    }

                    Console.WriteLine("Tracing route to {0}: \n", ip);

                    Tracert tr = new Tracert(ip);
                    List<TracertFrame> res = tr.Run();

                    if (!((res[res.Count - 1].Success) && (res[res.Count - 1].RemoteIP.Contains(ip))))
                        Console.WriteLine("\nError: trace failed");
                    else
                    {
                        Console.WriteLine("\nТrace complete.");
                        break;
                    }

                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("invalid input, try again");
                }
            }
        }
    }
}