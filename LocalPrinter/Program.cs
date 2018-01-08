using Fleck;
using Newtonsoft.Json;
using PrintCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalPrinter
{
    class Program
    {
        static void Main(string[] args)
        {
            var printer = PrinterFactory.GetPrinter("Microsoft XPS Document Writer", PaperWidth.Paper80mm);

            var server = new WebSocketServer("ws://0.0.0.0:2399");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("连接socket成功");
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("断开socket成功");
                };
                socket.OnMessage = msg =>
                {
                    var printData = JsonConvert.DeserializeObject<List<PrintUnit>>(msg);
                    foreach (var item in printData)
                    {
                        switch (item.Type)
                        {
                            case "text":
                                Console.WriteLine("打印文字");
                                printer.PrintText(item.Content);
                                break;
                            case "finish":
                                Console.WriteLine("结束打印输出文档");
                                printer.Finish();
                                break;
                            default:
                                break;
                        }
                    }
                };
            });

            Console.ReadKey();
        }
    }
}
