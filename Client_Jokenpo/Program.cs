using System;
using System.Windows.Forms;

namespace Client_Jokenpo
{
    static class Program
    {
        static string nomeClient;
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            nomeClient = null;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForms loginForms = new LoginForms();
            loginForms.OnNomeClient += LoginForms_OnNomeClient;
            Application.Run(loginForms);

            if (nomeClient != null)
            {
                Jokempo jokempo = new Jokempo(nomeClient);
                Application.Run(jokempo);
            }

        }

        private static void LoginForms_OnNomeClient(object sender, EventArgs e)
        {
            NomeEvent nomeEvent = e as NomeEvent;
            if (nomeEvent != null)
            {
                nomeClient = nomeEvent.nome;
            }
        }

    }
}
