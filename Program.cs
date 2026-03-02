using System;
using System.Windows.Forms;
using TiendaApp.Views; // Importamos la carpeta de vistas

namespace TiendaApp
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // AQUÍ ESTÁ EL CAMBIO CLAVE: 
            // Le decimos al sistema que arranque la aplicación abriendo el Menú Principal
            Application.Run(new frmMenuPrincipal());
        }
    }
}
