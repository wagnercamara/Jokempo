using System.IO;
using System.Net.Sockets;


namespace Client_Jokenpo
{
    public class Message
    {
        public int Type { set; get; }//1 - texto, 2 - opções
        public string Text { set; get; }

        public static byte[] Serialize(Message message)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

                binaryWriter.Write(message.Type);
                binaryWriter.Write(message.Text);

                return memoryStream.ToArray();
            }
        }

        public static Message Deserialize(NetworkStream networkStream)
        {
            Message message = new Message();

            BinaryReader binaryReader = new BinaryReader(networkStream);

            message.Type = binaryReader.ReadInt32();
            message.Text = binaryReader.ReadString();

            return message;
        }
    }
}
