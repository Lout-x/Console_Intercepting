using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

// ver 1.2
namespace MyProcessSample
{
    class MyProcess
    {

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        
        [DllImport("user32.dll")]

        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        public static void Main()
        {
            Process _process = new Process();
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;      // 35; 80; 115
            Console.Clear();
            Console.SetWindowSize(55, 5);

            Console.WriteLine("Введите путь к программе ltacons.exe: ");
            Console.WriteLine("например: C:\\Users\\metrolog_calib\\Desktop\\ltacons.exe");
            string? pathToExe = Console.ReadLine();

            Console.SetWindowSize(24, 5);
            _process.StartInfo.FileName = pathToExe;                    //"C:\\Users\\romanov\\Desktop\\ltacons.exe"
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.Start();
            
            char identif = '>';
            char needless = ' ';
            
            Thread myThread = new Thread(TimerToRefresh);
            myThread.Start();

            while (true)
            {
                try
                {
                    Thread.Sleep(500);
                    // задержка между "запросами" (при задержке меньше 500 не работает)
                    string? text = _process.StandardOutput.ReadLine();
                    // проверка на непустые значения
                    if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text) || text == null)
                    {
                        keybd_event((byte)0x0D, 0, 0x0001 | 0, 0);                 // нажимается Enter
                        //Thread.Sleep(500);
                        continue;
                    }
                    else
                    {
                        // отрезаем лишнюю информацию и меняем . на ,
                        int indexOfCharIdentif = text.IndexOf(identif);
                        int indexOfCharNeedless = text.IndexOf(needless);
                        if (indexOfCharIdentif != -1 && text != "")
                        {
                            text = text.Substring(7);
                        }
                        if (indexOfCharNeedless != -1 && text != "")
                        {
                            text = text.Remove(text.IndexOf(' '), text.Length - text.IndexOf(' '));
                        }
                        Console.WriteLine(text);
                        // запись в файл
                        try
                        {
                            using StreamWriter file = new("c:\\OSSY-NG\\Runtime\\Schema\\WriteLines.xml", append: true);
                            WriteToFileAsync(file, text);
                        }
                        catch { }
                    }
                }
                catch
                {
                    keybd_event((byte)0x0D, 0, 0x0001 | 0, 0);                 // нажимается Enter
                }
            }
        }


        //static void TimerToRefresh()
        //{
        //    int q = 0;
        //    while (true)
        //    {
        //        if (q >= 20 && ShowWindow(GetConsoleWindow(), 0) == true)
        //        {
        //            q = 0;
        //            ShowWindow(GetConsoleWindow(), 2);
        //            Thread.Sleep(500);
        //            ShowWindow(GetConsoleWindow(), 1);
        //            keybd_event((byte)0x0D, 0, 0x0001 | 0, 0);                 // нажимается Enter
        //        }
        //        q++;
        //        ShowWindow(GetConsoleWindow(), 1);
        //        Thread.Sleep(500);
        //    }
        //}


        static void TimerToRefresh()
        {
            for (int q = 0; q < 100; q++)
            {
                if (q == 20)
                {
                    q = 0;
                    ShowWindow(GetConsoleWindow(), 2);
                    Thread.Sleep(500);
                    ShowWindow(GetConsoleWindow(), 1);
                    keybd_event((byte)0x0D, 0, 0x0001 | 0, 0);                 // нажимается Enter
                }
                ShowWindow(GetConsoleWindow(), 1);
                Thread.Sleep(500);
            }
        }

        static async Task WriteToFileAsync(StreamWriter file, string text)
        {
            await file.WriteLineAsync(text);
        }
    }
}



