namespace CBElectionTickerControl
{
    partial class Form1
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
            this.btnConnectLocal = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnPreviewXml = new System.Windows.Forms.Button();
            this.txtXmlPreview = new System.Windows.Forms.TextBox();
            this.btnSendLocalTest = new System.Windows.Forms.Button();
            this.btnShowPage34 = new System.Windows.Forms.Button();
            this.btnShowPage12 = new System.Windows.Forms.Button();
            this.chkAutoPageSwitch = new System.Windows.Forms.CheckBox();
            this.btnRecreateLocalElement = new System.Windows.Forms.Button();
            this.btnReadTickerState = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnConnectLocal
            // 
            this.btnConnectLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnectLocal.Location = new System.Drawing.Point(12, 12);
            this.btnConnectLocal.Name = "btnConnectLocal";
            this.btnConnectLocal.Size = new System.Drawing.Size(215, 79);
            this.btnConnectLocal.TabIndex = 0;
            this.btnConnectLocal.Text = "YEREL TEST SERVİSİNE BAĞLAN";
            this.btnConnectLocal.UseVisualStyleBackColor = true;
            this.btnConnectLocal.Click += new System.EventHandler(this.btnConnectLocal_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(12, 94);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(109, 18);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "BAĞLI DEĞİL";
            // 
            // btnPreviewXml
            // 
            this.btnPreviewXml.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreviewXml.Location = new System.Drawing.Point(588, 193);
            this.btnPreviewXml.Name = "btnPreviewXml";
            this.btnPreviewXml.Size = new System.Drawing.Size(128, 46);
            this.btnPreviewXml.TabIndex = 2;
            this.btnPreviewXml.Text = "XML ÖNİZLE";
            this.btnPreviewXml.UseVisualStyleBackColor = true;
            this.btnPreviewXml.Click += new System.EventHandler(this.btnPreviewXml_Click);
            // 
            // txtXmlPreview
            // 
            this.txtXmlPreview.Location = new System.Drawing.Point(485, 12);
            this.txtXmlPreview.Multiline = true;
            this.txtXmlPreview.Name = "txtXmlPreview";
            this.txtXmlPreview.ReadOnly = true;
            this.txtXmlPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtXmlPreview.Size = new System.Drawing.Size(303, 175);
            this.txtXmlPreview.TabIndex = 3;
            this.txtXmlPreview.WordWrap = false;
            // 
            // btnSendLocalTest
            // 
            this.btnSendLocalTest.Enabled = false;
            this.btnSendLocalTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendLocalTest.Location = new System.Drawing.Point(576, 245);
            this.btnSendLocalTest.Name = "btnSendLocalTest";
            this.btnSendLocalTest.Size = new System.Drawing.Size(158, 65);
            this.btnSendLocalTest.TabIndex = 4;
            this.btnSendLocalTest.Text = "YEREL TEST TICKER\'A GÖNDER";
            this.btnSendLocalTest.UseVisualStyleBackColor = true;
            this.btnSendLocalTest.Click += new System.EventHandler(this.btnSendLocalTest_Click);
            // 
            // btnShowPage34
            // 
            this.btnShowPage34.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowPage34.Location = new System.Drawing.Point(67, 382);
            this.btnShowPage34.Name = "btnShowPage34";
            this.btnShowPage34.Size = new System.Drawing.Size(144, 56);
            this.btnShowPage34.TabIndex = 5;
            this.btnShowPage34.Text = "3/4 GÖSTER";
            this.btnShowPage34.UseVisualStyleBackColor = true;
            this.btnShowPage34.Click += new System.EventHandler(this.btnShowPage34_Click);
            // 
            // btnShowPage12
            // 
            this.btnShowPage12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowPage12.Location = new System.Drawing.Point(67, 319);
            this.btnShowPage12.Name = "btnShowPage12";
            this.btnShowPage12.Size = new System.Drawing.Size(144, 56);
            this.btnShowPage12.TabIndex = 6;
            this.btnShowPage12.Text = "1/2 GÖSTER";
            this.btnShowPage12.UseVisualStyleBackColor = true;
            this.btnShowPage12.Click += new System.EventHandler(this.btnShowPage12_Click);
            // 
            // chkAutoPageSwitch
            // 
            this.chkAutoPageSwitch.AutoSize = true;
            this.chkAutoPageSwitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoPageSwitch.Location = new System.Drawing.Point(15, 289);
            this.chkAutoPageSwitch.Name = "chkAutoPageSwitch";
            this.chkAutoPageSwitch.Size = new System.Drawing.Size(247, 24);
            this.chkAutoPageSwitch.TabIndex = 7;
            this.chkAutoPageSwitch.Text = "Sol Otomasyon (5 SANİYE)";
            this.chkAutoPageSwitch.UseVisualStyleBackColor = true;
            this.chkAutoPageSwitch.CheckedChanged += new System.EventHandler(this.chkAutoPageSwitch_CheckedChanged);
            // 
            // btnRecreateLocalElement
            // 
            this.btnRecreateLocalElement.Enabled = false;
            this.btnRecreateLocalElement.Location = new System.Drawing.Point(658, 405);
            this.btnRecreateLocalElement.Name = "btnRecreateLocalElement";
            this.btnRecreateLocalElement.Size = new System.Drawing.Size(130, 42);
            this.btnRecreateLocalElement.TabIndex = 8;
            this.btnRecreateLocalElement.Text = "YEREL ELEMENTİ BİR KEZ YENİLE";
            this.btnRecreateLocalElement.UseVisualStyleBackColor = true;
            this.btnRecreateLocalElement.Click += new System.EventHandler(this.btnRecreateLocalElement_Click);
            // 
            // btnReadTickerState
            // 
            this.btnReadTickerState.Enabled = false;
            this.btnReadTickerState.Location = new System.Drawing.Point(658, 357);
            this.btnReadTickerState.Name = "btnReadTickerState";
            this.btnReadTickerState.Size = new System.Drawing.Size(130, 42);
            this.btnReadTickerState.TabIndex = 9;
            this.btnReadTickerState.Text = "TICKER DURUMUNU OKU";
            this.btnReadTickerState.UseVisualStyleBackColor = true;
            this.btnReadTickerState.Click += new System.EventHandler(this.btnReadTickerState_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnReadTickerState);
            this.Controls.Add(this.btnRecreateLocalElement);
            this.Controls.Add(this.chkAutoPageSwitch);
            this.Controls.Add(this.btnShowPage12);
            this.Controls.Add(this.btnShowPage34);
            this.Controls.Add(this.btnSendLocalTest);
            this.Controls.Add(this.txtXmlPreview);
            this.Controls.Add(this.btnPreviewXml);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConnectLocal);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnectLocal;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnPreviewXml;
        private System.Windows.Forms.TextBox txtXmlPreview;
        private System.Windows.Forms.Button btnSendLocalTest;
        private System.Windows.Forms.Button btnShowPage34;
        private System.Windows.Forms.Button btnShowPage12;
        private System.Windows.Forms.CheckBox chkAutoPageSwitch;
        private System.Windows.Forms.Button btnRecreateLocalElement;
        private System.Windows.Forms.Button btnReadTickerState;
    }
}

