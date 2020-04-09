using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Client_Jokenpo
{
    public partial class Jokempo : Form
    {
        private NetworkStream clientStream;
        private EnviarJogada enviarJogada;
        private ReceberJogada receberJogada;

        private string nome { get; set; }
        private int p1 = 0;
        private int p2 = 0;

        public Jokempo(string nome) //inicio
        {
            this.nome = nome;
            InitializeComponent();
            StartConaction();
            StartThreadReceber();
            lbl_Nome_Jogador.Text = this.nome;
            this.btn_Papel.Enabled = false;
            this.btn_Pedra.Enabled = false;
            this.btn_Tesoura.Enabled = false;
        }
        private void Start2()
        {
            StartConaction();
            txt_Conversas.Clear();
            txt_History.Clear();
            txt_Message.Clear();
            StartThreadReceber();
            this.btn_Papel.Enabled = false;
            this.btn_Pedra.Enabled = false;
            this.btn_Tesoura.Enabled = false;
        }
        private void StartThreadReceber() // start receber msg
        {
            this.receberJogada = new ReceberJogada(this.clientStream);
            receberJogada.OnMessage += ReceberJogada_OnJogada;
            receberJogada.Start();
        }
        private void ReceberJogada_OnJogada(object sender, EventArgs e)
        {
            JogadaEvent jogadaEvent = e as JogadaEvent;

            if (jogadaEvent != null)
            {

                switch (jogadaEvent.jogada.Type)
                {
                    case 1:
                        txt_Conversas.AppendText($"{jogadaEvent.jogada.Text}" + System.Environment.NewLine.ToString());
                        break;
                    case 2:
                        txt_History.AppendText($"{jogadaEvent.jogada.Text}" + System.Environment.NewLine.ToString());

                        switch (jogadaEvent.jogada.Text)
                        {
                            case "Empate":
                                lbl_result.Text = "Empate";
                                break;
                            case "Vitória do jogador 1":
                                lbl_result.Text = "Vitória do jogador 1";
                                p1++;
                                lbl_Jogador1.Text = Convert.ToString(p1);
                                break;
                            case "Vitória do jogador 2":
                                lbl_result.Text = "Vitória do jogador 2";
                                p2++;
                                lbl_Jogador2.Text = Convert.ToString(p2);
                                break;
                        }

                        this.btn_Papel.Enabled = true;
                        this.btn_Pedra.Enabled = true;
                        this.btn_Tesoura.Enabled = true;
                        break;
                    case 3:
                        txt_Conversas.Text = ("Erro na conexão com o outro jogador reinicie o jogo.");
                        txt_History.Text = ("Erro na conexão com o outro jogador reinicie o jogo.");
                        txt_Message.Text = ("Erro na conexão com o outro jogador reinicie o jogo.");

                        DialogResult result = MessageBox.Show("Erro Jogador Indisponivel, Deseja Recomeçar", "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        switch (result)
                        {
                            case DialogResult.Yes:
                                this.p1 = 0;
                                this.p2 = 0;
                                lbl_result.Text = null;
                                lbl_Jogador1.Text = Convert.ToString(p1);
                                lbl_Jogador2.Text = Convert.ToString(p2);
                                this.receberJogada = null;
                                this.clientStream = null;
                                Start2();
                                break;
                            case DialogResult.No:
                                this.Close();
                                break;
                        }
                        break;
                    case 4:
                        lbl_QuemSouEu.Text = $"Jogador ({jogadaEvent.jogada.Text})";
                        break;
                }
            }
        }
        private void btn_Pedra_Click(object sender, EventArgs e)
        {
            this.btn_Papel.Enabled = false;
            this.btn_Pedra.Enabled = false;
            this.btn_Tesoura.Enabled = false;
            txt_History.AppendText("Enviado > Pedra" + System.Environment.NewLine.ToString());
            Enviar(2, "1"); //pedra

        }
        private void btn_Papel_Click(object sender, EventArgs e)
        {
            this.btn_Papel.Enabled = false;
            this.btn_Pedra.Enabled = false;
            this.btn_Tesoura.Enabled = false;
            txt_History.AppendText("Enviado > Papel" + System.Environment.NewLine.ToString());
            Enviar(2, "2"); //papel
        }
        private void btn_Tesoura_Click(object sender, EventArgs e)
        {
            this.btn_Papel.Enabled = false;
            this.btn_Pedra.Enabled = false;
            this.btn_Tesoura.Enabled = false;
            txt_History.AppendText("Enviado > Tesoura" + System.Environment.NewLine.ToString());
            Enviar(2, "3"); //tesoura
        }
        private void btn_Enviar_Click(object sender, EventArgs e)
        {
            if (txt_Message.Text != "")
            {
                string mensagem = Convert.ToString($"{this.nome}: {txt_Message.Text}");
                Enviar(1, mensagem);
                txt_Conversas.AppendText($"{mensagem}" + System.Environment.NewLine.ToString());
                txt_Message.Clear();
            }
        }
        private void Enviar(int type, string text)
        {
            BinaryWriter binaryWriter = new BinaryWriter(this.clientStream);
            this.enviarJogada = new EnviarJogada(binaryWriter, new Message() { Type = type, Text = text });
            enviarJogada.OnEnvioErro += EnviarJogada_OnEnvioErro;
            enviarJogada.Start();
        }

        private void EnviarJogada_OnEnvioErro(object sender, EventArgs e)
        {
            EnvirErroEvent envirErroEvent = e as EnvirErroEvent;
            if (envirErroEvent != null)
            {
                DialogResult result = MessageBox.Show("Erro Servidor Indisponivel, Deseja Recomeçar", "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (result)
                {
                    case DialogResult.Yes:
                        this.p1 = 0;
                        this.p2 = 0;
                        lbl_result.Text = null;
                        lbl_Jogador1.Text = Convert.ToString(p1);
                        lbl_Jogador2.Text = Convert.ToString(p2);
                        this.receberJogada = null;
                        this.clientStream = null;
                        Start2();
                        break;
                    case DialogResult.No:
                        this.Close();
                        break;
                }
            }
        }

        private void StartConaction()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 5000;

            TcpClient client = new TcpClient();//socket()
            try
            {
                client.Connect(ipAddress, port);//connect()
            }
            catch
            {
                DialogResult result = MessageBox.Show("Erro Servidor Indisponivel, Deseja Recomeçar", "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                switch (result)
                {
                    case DialogResult.Yes:
                        this.receberJogada = null;
                        this.clientStream = null;
                        Start2();
                        break;
                    case DialogResult.No:
                        this.Close();
                        break;
                }
            }
            this.clientStream = client.GetStream();
        }
        protected override void OnClosing(CancelEventArgs e) => Close();
    }
}
