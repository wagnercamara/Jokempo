using System;
using System.IO;
using System.Threading;

namespace Client_Jokenpo
{
    public class EnviarJogada
    {
        private BinaryWriter binaryWriter;
        private Message message;
        private Thread thread;
        public event EventHandler OnEnvioErro = null;
        protected bool isRunning;

        public EnviarJogada(BinaryWriter binaryWriter, Message message)
        {
            this.message = message;
            this.binaryWriter = binaryWriter;
            this.thread = new Thread(this.Run);
        }

        public void Start()
        {
            this.thread.Start();
        }
        public void Run()
        {
            try
            {
                if (this.message != null)
                {
                    this.binaryWriter.Write(Message.Serialize(this.message));
                    this.message = null;
                }
            }
            catch
            {
                this.OnEnvioErro.Invoke(this, new EnvirErroEvent() { message = "Erro" });
            }
               
        }

    }
}
