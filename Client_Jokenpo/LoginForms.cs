using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_Jokenpo
{
    public partial class LoginForms : Form
    {
        private string nome { get; set; }
        public event EventHandler OnNomeClient;
        public LoginForms()
        {
            InitializeComponent();
        }

        private void btn_Logar_Click(object sender, EventArgs e)
        {
            nome = this.txt_Result.Text;
            if (nome == "")
            {
                MessageBox.Show("Nome não especificado");
            }
            else
            {
                this.OnNomeClient.Invoke(this, new NomeEvent() {nome = this.nome });
                this.Close();
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            this.nome = null;
        }

    }
}
