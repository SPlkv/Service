using System;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAMEDPIPESERVERSERVICE
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = true;
            this.AutoLog = true;
        }
        public async void Go()
        {
            await Task.Run(()=>
            {
                while (true)
                {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("pipe", PipeDirection.In))
                    {
                        pipeServer.WaitForConnection();
                        byte[] clientData = new byte[1024 * 5000];
                        int receivedBytesLen = pipeServer.Read(clientData, 0, clientData.Length);
                        int fileNameLen = BitConverter.ToInt32(clientData, 0);
                        string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);
                        fileName = Path.GetFileName(fileName);
                        BinaryWriter bWrite = new BinaryWriter(File.Open(@"C:\Users\User\Desktop\NAMEDPIPESERVERSERVICE\NAMEDPIPESERVERSERVICE\bin\Debug" + fileName, FileMode.Create));
                        bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);
                        bWrite.Close();
                    }
                }
            });
        }

        protected override void OnStart(string[] args)
        {
            Thread.Sleep(1000);
            Go();
        }

        protected override void OnStop()
        {
            
        }
    }
}
