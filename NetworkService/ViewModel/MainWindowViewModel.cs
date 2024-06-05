using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel
    {
        private int id;
        private int value;
        //private bool file;
        private readonly Uri path = new Uri("LogFile.txt", UriKind.Relative);

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25675);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(Database.Database.Entities.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            string[] split = incomming.Split('_', ':');
                            id = Int32.Parse(split[1]);
                            value = Int32.Parse(split[2]);
                            Console.WriteLine($"Changing the {id} to {value}");
                            if (Database.Database.Entities.Count() > 0 && (Database.Database.Entities.Count() > id) && value <= 5 && value >= 1)
                            {
                                Database.Database.Entities[id].Value = value;
                                WriteLog(Database.Database.Entities[id].Id, Database.Database.Entities[id].Value);
                            }
                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        private void WriteLog(int id, double value)
        {
            StreamWriter wr;
            using (wr = new StreamWriter(path.ToString(), true))
            {
                wr.WriteLine("Date Time:\t" + DateTime.Now.ToString() + "\tObject_" + id + "\tValue:\t" + value);
            }
        }
    }
}
