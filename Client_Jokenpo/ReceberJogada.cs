using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;


namespace Client_Jokenpo
{
    public class ReceberJogada
    {
        private NetworkStream clientStream;
        private Thread thread;
        public event EventHandler OnMessage = null;
        private bool isRunning;


        public ReceberJogada(NetworkStream clientStream)
        {
            this.isRunning = false;
            this.clientStream = clientStream;
            this.thread = new Thread(this.Run);
        }

        public void Start()
        {
            this.isRunning = true;
            this.thread.Start();
        }
        public void Run()
        {
            BinaryReader clientInput = new BinaryReader(this.clientStream);

            if (this.OnMessage != null)
            {
                while (this.isRunning)
                {
                    try
                    {
                        Message message = Message.Deserialize(this.clientStream);

                        this.OnMessage.Invoke(this, new JogadaEvent()
                        {
                            jogada = message
                        });
                    }
                    catch
                    {
                        this.isRunning = false;
                    }
                }
            }
        }

    }
}
