namespace SchnorryNotSorry
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            bGGeneration = new Button();
            label2 = new Label();
            tBoxGGenerated = new TextBox();
            label1 = new Label();
            tBoxPValue = new TextBox();
            pBarGenerating = new ProgressBar();
            label3 = new Label();
            tBoxHValue = new TextBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(tBoxHValue);
            groupBox1.Controls.Add(pBarGenerating);
            groupBox1.Controls.Add(bGGeneration);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(tBoxGGenerated);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(tBoxPValue);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(356, 298);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Main Values";
            // 
            // bGGeneration
            // 
            bGGeneration.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
            bGGeneration.Location = new Point(6, 22);
            bGGeneration.Name = "bGGeneration";
            bGGeneration.Size = new Size(338, 28);
            bGGeneration.TabIndex = 4;
            bGGeneration.Text = "GENERATE VALUES";
            bGGeneration.UseVisualStyleBackColor = true;
            bGGeneration.Click += bGGeneration_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 116);
            label2.Name = "label2";
            label2.Size = new Size(45, 15);
            label2.TabIndex = 3;
            label2.Text = "q value";
            // 
            // tBoxGGenerated
            // 
            tBoxGGenerated.Enabled = false;
            tBoxGGenerated.Location = new Point(69, 113);
            tBoxGGenerated.Name = "tBoxGGenerated";
            tBoxGGenerated.Size = new Size(275, 23);
            tBoxGGenerated.TabIndex = 2;
            tBoxGGenerated.TextAlign = HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 87);
            label1.Name = "label1";
            label1.Size = new Size(45, 15);
            label1.TabIndex = 1;
            label1.Text = "p value";
            // 
            // tBoxPValue
            // 
            tBoxPValue.Enabled = false;
            tBoxPValue.Location = new Point(69, 84);
            tBoxPValue.Name = "tBoxPValue";
            tBoxPValue.Size = new Size(275, 23);
            tBoxPValue.TabIndex = 0;
            tBoxPValue.TextAlign = HorizontalAlignment.Center;
            // 
            // pBarGenerating
            // 
            pBarGenerating.Location = new Point(6, 54);
            pBarGenerating.Name = "pBarGenerating";
            pBarGenerating.Size = new Size(338, 24);
            pBarGenerating.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 145);
            label3.Name = "label3";
            label3.Size = new Size(45, 15);
            label3.TabIndex = 7;
            label3.Text = "h value";
            // 
            // tBoxHValue
            // 
            tBoxHValue.Enabled = false;
            tBoxHValue.Location = new Point(69, 142);
            tBoxHValue.Name = "tBoxHValue";
            tBoxHValue.Size = new Size(275, 23);
            tBoxHValue.TabIndex = 6;
            tBoxHValue.TextAlign = HorizontalAlignment.Center;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(382, 326);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private TextBox tBoxPValue;
        private Label label2;
        private TextBox tBoxGGenerated;
        private Button bGGeneration;
        private ProgressBar pBarGenerating;
        private Label label3;
        private TextBox tBoxHValue;
    }
}
