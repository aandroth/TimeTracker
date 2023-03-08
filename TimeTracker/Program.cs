using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;

namespace TimeTracker
{
    internal class Program
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        static int lastHour = DateTime.Now.Minute;
        static string filePath = "C:\\Users\\aa1106\\Desktop\\HourlyLog.txt";
        static System.Timers.Timer aTimer = new System.Timers.Timer(1000);

        static string currentTime = DateTime.Now.TimeOfDay.Hours.ToString() + ":" + (DateTime.Now.TimeOfDay.Minutes < 10 ? "0" : "") + DateTime.Now.TimeOfDay.Minutes.ToString();

        private static async void Async_OnTimedEventEveryCheckedTimePeriod(int minutes)
        {
            while (true)
            {
                Process process = Process.GetCurrentProcess();
                SetForegroundWindow(process.MainWindowHandle);
                await Task.Delay(100);
                SetForegroundWindow(process.MainWindowHandle);
                Console.WriteLine("How have you used your very precious work time since " + currentTime + "?");
                LogToFile();
                currentTime = DateTime.Now.TimeOfDay.Hours.ToString() + ":" + (DateTime.Now.TimeOfDay.Minutes < 10 ? "0" : "") + DateTime.Now.TimeOfDay.Minutes.ToString();
                await Task.Delay(minutes * 60 *1000);
            }
        }

        private static void OnTimedEventEveryClockHour(object source, ElapsedEventArgs e)
        {
            if (lastHour < DateTime.Now.Hour || (lastHour == 23 && DateTime.Now.Hour == 0))
            {
                lastHour = DateTime.Now.Hour;
                Console.WriteLine(DateTime.Now + " OnTimedEventEveryClockHour tick");
                LogToFile();
            }
        }

        private static void Call_LogToFile(object source, ElapsedEventArgs e)
        {
            LogToFile();
        }

        private static void LogToFile()
        {
            aTimer.Stop();
            Process process = Process.GetCurrentProcess();
            Console.WriteLine(process.ProcessName);
            SetForegroundWindow(process.MainWindowHandle);

            string thisLog = "";
            thisLog = Console.ReadLine();

            string prevLogs = File.ReadAllText(filePath);

            File.WriteAllText(filePath, DateTime.Now + " | " + thisLog + "\n\n" + prevLogs);
            aTimer.Start();
        }

        static void Main(string[] args)
        {
            string choiceResponse = "";
            string minutesResponse = "";
            bool keepLooping = true;
            string breaker = "\r\n/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\r\n";
            string prevLogs = File.ReadAllText(filePath);
            File.WriteAllText(filePath, breaker + prevLogs);
            string I_WILL_CONFIRMATION = "";

            Console.WriteLine("Have you recited your I WILL statement?");
            I_WILL_CONFIRMATION = Console.ReadLine();
            while (I_WILL_CONFIRMATION.ToLower() != "yes")
            {
                Console.WriteLine("Recite your I WILL statement");
                I_WILL_CONFIRMATION = Console.ReadLine();
            }

            Console.WriteLine("0) Check every time period from now.");
            Console.WriteLine("1) Check every hour on the hour.");
            choiceResponse = Console.ReadLine();

            while (keepLooping)
            {
                switch (choiceResponse)
                {
                    case "0":
                        Console.WriteLine("Enter number of minutes per time period.");
                        minutesResponse = Console.ReadLine();
                        if(!int.TryParse(minutesResponse, out int n))
                            continue;
                        Async_OnTimedEventEveryCheckedTimePeriod(int.Parse(minutesResponse));
                        keepLooping = false;
                        break;

                    case "1":
                        File.WriteAllText(filePath, breaker + prevLogs);
                        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEventEveryClockHour);
                        aTimer.Interval = 1000;
                        aTimer.Start();
                        keepLooping = false;
                        break;

                    default:
                        keepLooping = true;
                        break;
                }
            }

            choiceResponse = "";
            minutesResponse = "";

            while (true) { }
        }
    }
}
