namespace ShootClient
{
    partial class OpenGLTableScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                //canvas.DestroyContexts();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            canvas = new OpenTK.GLControl();
            OpenTK.Graphics.GraphicsMode g = canvas.GraphicsMode;
            this.SuspendLayout();
            // 
            // OpenGLTableScreen
            // 
            this.ClientSize = new System.Drawing.Size(1200, 699);
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "OpenGLTableScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shoot the Moon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosingHandler);
            this.ResumeLayout(false);
        }

        #endregion

    }
}