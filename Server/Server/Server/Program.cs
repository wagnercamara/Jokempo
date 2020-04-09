using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{

    class Program
    {
        static int clientNumber = 0;
        static List<ThreadClient> threadClients = new List<ThreadClient>();
        static Dictionary<Int32, Int32> optionClients = new Dictionary<Int32, Int32>();

        static void StartGame()
        {
            foreach (ThreadClient threadClient in threadClients)
            {
                threadClient.Write(new Message()
                {
                    Type = 4,
                    Text = $"{threadClient.GetNumber()}"
                });
            }
            foreach (ThreadClient threadClient in threadClients)
            {
                threadClient.Write(new Message()
                {
                    Type = 2,
                    Text = $"Olá jogador {threadClient.GetNumber()}, favor escolher a sua opção"
                });
            }
        }

        static void EndGame()
        {
            string messageText = String.Empty;

            int optionClient1 = optionClients[1];
            int optionClient2 = optionClients[2];

            if (optionClient1 == optionClient2)
            {
                messageText = "Empate";
            }
            else
            {
                switch (optionClient1)
                {
                    case 1://Pedra
                        switch (optionClient2)
                        {
                            case 2://Papel
                                messageText = "Vitória do jogador 2";
                                break;
                            case 3://Tesoura
                                messageText = "Vitória do jogador 1";
                                break;
                        }
                        break;
                    case 2://Papel
                        switch (optionClient2)
                        {
                            case 1://Pedra
                                messageText = "Vitória do jogador 1";
                                break;
                            case 3://Tesoura
                                messageText = "Vitória do jogador 2";
                                break;
                        }
                        break;
                    case 3://Tesoura
                        switch (optionClient2)
                        {
                            case 1://Pedra
                                messageText = "Vitória do jogador 2";
                                break;
                            case 2://Papel
                                messageText = "Vitória do jogador 1";
                                break;
                        }
                        break;
                }
            }

            optionClients.Clear();

            foreach (ThreadClient threadClient in threadClients)
            {
                threadClient.Write(new Message()
                {
                    Type = 2,
                    Text = messageText
                });
            }
        }
        static void ErroDisconect(ThreadClient threadClientSender)
        {
            Console.WriteLine($"Removendo >>>>>>>>>>>>>> {threadClients.Count}");
            threadClients.Remove(threadClientSender);
            clientNumber = 0;
            Console.WriteLine($"removido >>>>>>>>>>>>>> {threadClients.Count}");
            ThreadClient thr = null;

            if (threadClients.Count >= 1)
            {
                foreach (ThreadClient threadClient in threadClients)
                {
                    Console.WriteLine("ENVIAR para o outro jogador");
                    thr = threadClient;

                    threadClient.Write(new Message()
                    {
                        Type = 3,
                        Text = "Remove"
                    });

                }
                threadClients.Remove(thr);
            }

        }
        static void EnviarMessage(ThreadClient threadClientRem, string messageText)
        {

            foreach (ThreadClient threadClient in threadClients)
            {
                if (threadClient != threadClientRem)
                {
                    threadClient.Write(new Message()
                    {
                        Type = 1,
                        Text = messageText
                    });
                }
            }
        }
        static void Main(string[] args)
        {
            bool isRunning = true;

            IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            TcpListener tcpListener = new TcpListener(iPAddress, port);

            tcpListener.Start();

            Socket socket;
            ThreadClient threadClient;
            while (isRunning == true)
            {
                socket = tcpListener.AcceptSocket();
                clientNumber++;
                threadClient = new ThreadClient(clientNumber, socket);

                threadClient.OnMessage += ThreadClient_OnMessage;

                threadClient.OnKill += ThreadClient_OnKill;

                threadClient.Sleep();
                threadClients.Add(threadClient);
                Console.WriteLine($"Adicionando a lista {threadClients.Count}");

                threadClient.Start();

                if (clientNumber == 2)
                {
                    StartGame();
                }
                else
                {
                    threadClient.Write(new Message()
                    {
                        Type = 1,
                        Text = "Entrando na partida..."
                    });
                }
                Console.WriteLine("Qauntidade de cliente :" + clientNumber);

            }
        }

        private static void ThreadClient_OnKill(object sender, EventArgs e)
        {
            ThreadClient killEvent = sender as ThreadClient;
            Console.WriteLine("CASE 3");
            ErroDisconect(killEvent);
        }

        private static void ThreadClient_OnMessage(object sender, EventArgs e)
        {
            ThreadClient threadClient = sender as ThreadClient;
            MessageEventArgs messageEventArgs = e as MessageEventArgs;

            if (threadClient != null && messageEventArgs != null)
            {
                Console.WriteLine(messageEventArgs.Message.Type);
                switch (messageEventArgs.Message.Type)
                {

                    case 1://Texto

                        if (threadClients.Count == 2)
                        {
                            Console.WriteLine("CASE 1");
                            EnviarMessage(threadClient, Convert.ToString((messageEventArgs.Message.Text)));
                        }
                        break;

                    case 2://Opção
                        optionClients.Add(threadClient.GetNumber(), Convert.ToInt32(messageEventArgs.Message.Text));
                        Console.WriteLine("CASE 2");
                        if (optionClients.Count == 2)
                        {
                            EndGame();
                        }
                        break;
                    case 3:
                        if (optionClients.Count > 0)
                        {
                            Console.WriteLine("CASE 3");
                            ErroDisconect(threadClient);
                        }
                        break;

                }
            }
        }
    }
}
