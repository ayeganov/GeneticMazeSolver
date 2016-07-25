namespace MazeSolver
{
    partial class MazeSolverForm
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
            this.m_MazeSurface = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_MazeSurface)).BeginInit();
            this.SuspendLayout();
            // 
            // m_MazeSurface
            // 
            this.m_MazeSurface.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_MazeSurface.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_MazeSurface.Location = new System.Drawing.Point(2, 1);
            this.m_MazeSurface.Name = "m_MazeSurface";
            this.m_MazeSurface.Size = new System.Drawing.Size(789, 760);
            this.m_MazeSurface.TabIndex = 0;
            this.m_MazeSurface.TabStop = false;
            // 
            // MazeSolverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 761);
            this.Controls.Add(this.m_MazeSurface);
            this.Name = "MazeSolverForm";
            this.Text = "MazeSolver";
            ((System.ComponentModel.ISupportInitialize)(this.m_MazeSurface)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_MazeSurface;
    }
}

