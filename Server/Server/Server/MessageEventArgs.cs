using System;

namespace Server
{
	public class MessageEventArgs : EventArgs
	{
		public Message Message { set; get; }
	}
}
