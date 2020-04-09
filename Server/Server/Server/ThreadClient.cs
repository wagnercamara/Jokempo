using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
	public class ThreadClient
	{
		private Thread thread;
		private Socket socket;
		private int number;
		private bool isRunning;
		public event EventHandler OnMessage;
		public event EventHandler OnKill;

		public ThreadClient(int number, Socket socket)
		{
			this.number = number;
			this.socket = socket;
			this.thread = new Thread(this.Run);
		}

		public void Sleep()
		{
			Thread.Sleep(1000);
		}
		private void Run()
		{
			if (this.OnMessage != null)
			{
				NetworkStream networkStream = new NetworkStream(this.socket);

				Message message;

				while (this.isRunning == true)
				{
					try
					{
						message = Message.Deserialize(networkStream);
						this.OnMessage.Invoke(this, new MessageEventArgs()
						{
							Message = message
						});
					}
					catch
					{
						this.OnKill.Invoke(this, new KillEvent() { threadClient = this });
						this.isRunning = false;
					}
				}
			}
		}

		public void Start()
		{
			this.isRunning = true;
			this.thread.Start();
		}

		public void Write(Message message)
		{
			try
			{
				NetworkStream networkStream = new NetworkStream(this.socket);
				BinaryWriter binaryWriter = new BinaryWriter(networkStream);

				binaryWriter.Write(Message.Serialize(message));
			}
			catch
			{
				this.isRunning = false;
			}
		}

		public int GetNumber()
		{
			return this.number;
		}
	}
}
