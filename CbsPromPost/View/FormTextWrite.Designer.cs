namespace CbsPromPost.View
{
    sealed partial class FormTextWrite
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
            richTextBoxMain = new RichTextBox();
            buttonCancel = new Button();
            buttonSend = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
            button11 = new Button();
            button12 = new Button();
            button13 = new Button();
            button14 = new Button();
            button15 = new Button();
            button16 = new Button();
            button17 = new Button();
            button18 = new Button();
            button19 = new Button();
            button20 = new Button();
            button21 = new Button();
            button22 = new Button();
            button23 = new Button();
            button24 = new Button();
            button25 = new Button();
            button26 = new Button();
            button27 = new Button();
            button28 = new Button();
            button29 = new Button();
            button30 = new Button();
            button31 = new Button();
            button32 = new Button();
            button33 = new Button();
            button34 = new Button();
            button35 = new Button();
            label1 = new Label();
            button36 = new Button();
            SuspendLayout();
            // 
            // richTextBoxMain
            // 
            richTextBoxMain.Dock = DockStyle.Bottom;
            richTextBoxMain.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBoxMain.Location = new Point(0, 466);
            richTextBoxMain.Name = "richTextBoxMain";
            richTextBoxMain.Size = new Size(1150, 145);
            richTextBoxMain.TabIndex = 0;
            richTextBoxMain.Text = "";
            richTextBoxMain.KeyDown += RichTextBoxMain_KeyDown;
            // 
            // buttonCancel
            // 
            buttonCancel.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCancel.ForeColor = Color.Maroon;
            buttonCancel.Location = new Point(12, 421);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(183, 39);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "ОТМЕНИТЬ";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonSend
            // 
            buttonSend.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            buttonSend.ForeColor = Color.FromArgb(0, 64, 0);
            buttonSend.Location = new Point(955, 421);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(183, 39);
            buttonSend.TabIndex = 2;
            buttonSend.Text = "ПОДТВЕРДИТЬ";
            buttonSend.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button1.ForeColor = Color.Red;
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(183, 49);
            button1.TabIndex = 3;
            button1.Text = "TBS: неверная распиновка";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button2.ForeColor = Color.Red;
            button2.Location = new Point(201, 12);
            button2.Name = "button2";
            button2.Size = new Size(183, 49);
            button2.TabIndex = 4;
            button2.Text = "TBS: оторваны провода (плохая пайка)";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button3.ForeColor = Color.Red;
            button3.Location = new Point(766, 12);
            button3.Name = "button3";
            button3.Size = new Size(183, 49);
            button3.TabIndex = 5;
            button3.Text = "TBS: плохая пайка (капли и волосы на плате, лишний флюс)";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button4.ForeColor = Color.Red;
            button4.Location = new Point(955, 12);
            button4.Name = "button4";
            button4.Size = new Size(183, 49);
            button4.TabIndex = 6;
            button4.Text = "TBS: не доделано";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button5.ForeColor = Color.FromArgb(0, 0, 192);
            button5.Location = new Point(12, 67);
            button5.Name = "button5";
            button5.Size = new Size(183, 49);
            button5.TabIndex = 7;
            button5.Text = "РЕГУЛЬ: КЗ между контактами на моторах";
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button6.ForeColor = Color.FromArgb(0, 0, 192);
            button6.Location = new Point(201, 67);
            button6.Name = "button6";
            button6.Size = new Size(183, 49);
            button6.TabIndex = 8;
            button6.Text = "РЕГУЛЬ: нет шлейфа / не правильно вставлен шлейф";
            button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button7.ForeColor = Color.FromArgb(0, 0, 192);
            button7.Location = new Point(390, 67);
            button7.Name = "button7";
            button7.Size = new Size(183, 49);
            button7.TabIndex = 9;
            button7.Text = "РЕГУЛЬ: не правильно припаян конденсатор";
            button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button8.ForeColor = Color.FromArgb(0, 0, 192);
            button8.Location = new Point(579, 67);
            button8.Name = "button8";
            button8.Size = new Size(183, 49);
            button8.TabIndex = 10;
            button8.Text = "РЕГУЛЬ: не правильной стороной распаян регулятор";
            button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            button9.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button9.ForeColor = Color.Red;
            button9.Location = new Point(390, 12);
            button9.Name = "button9";
            button9.Size = new Size(183, 49);
            button9.TabIndex = 11;
            button9.Text = "TBS: КЗ на TBS";
            button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button10.ForeColor = Color.FromArgb(0, 0, 192);
            button10.Location = new Point(955, 67);
            button10.Name = "button10";
            button10.Size = new Size(183, 49);
            button10.TabIndex = 12;
            button10.Text = "РЕГУЛЬ: не доделано";
            button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            button11.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button11.ForeColor = Color.FromArgb(0, 0, 192);
            button11.Location = new Point(766, 67);
            button11.Name = "button11";
            button11.Size = new Size(183, 49);
            button11.TabIndex = 13;
            button11.Text = "РЕГУЛЬ: плохая пайка (капли и волосы на плате, лишний флюс)";
            button11.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            button12.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button12.ForeColor = Color.FromArgb(192, 0, 192);
            button12.Location = new Point(12, 122);
            button12.Name = "button12";
            button12.Size = new Size(183, 49);
            button12.TabIndex = 14;
            button12.Text = "ПОЛЕТНИК: КЗ между между припаянными контактами";
            button12.UseVisualStyleBackColor = true;
            // 
            // button13
            // 
            button13.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button13.ForeColor = Color.FromArgb(192, 0, 192);
            button13.Location = new Point(201, 122);
            button13.Name = "button13";
            button13.Size = new Size(183, 49);
            button13.TabIndex = 15;
            button13.Text = "ПОЛЕТНИК: замят , не правильно вставлен шлейф";
            button13.UseVisualStyleBackColor = true;
            // 
            // button14
            // 
            button14.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button14.ForeColor = Color.FromArgb(192, 0, 192);
            button14.Location = new Point(766, 122);
            button14.Name = "button14";
            button14.Size = new Size(183, 49);
            button14.TabIndex = 16;
            button14.Text = "ПОЛЕТНИК: плохая пайка (капли и волосы на плате, лишний флюс)";
            button14.UseVisualStyleBackColor = true;
            // 
            // button15
            // 
            button15.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button15.ForeColor = Color.FromArgb(192, 0, 192);
            button15.Location = new Point(390, 122);
            button15.Name = "button15";
            button15.Size = new Size(183, 49);
            button15.TabIndex = 17;
            button15.Text = "ПОЛЕТНИК: не правильная распиновка контактов пайки";
            button15.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            button16.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button16.ForeColor = Color.FromArgb(192, 0, 192);
            button16.Location = new Point(955, 122);
            button16.Name = "button16";
            button16.Size = new Size(183, 49);
            button16.TabIndex = 18;
            button16.Text = "ПОЛЕТНИК: не доделано";
            button16.UseVisualStyleBackColor = true;
            // 
            // button17
            // 
            button17.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button17.ForeColor = Color.FromArgb(128, 64, 0);
            button17.Location = new Point(12, 177);
            button17.Name = "button17";
            button17.Size = new Size(183, 49);
            button17.TabIndex = 19;
            button17.Text = "БАРОМЕТР: не правильная распиновка";
            button17.UseVisualStyleBackColor = true;
            // 
            // button18
            // 
            button18.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button18.ForeColor = Color.FromArgb(128, 64, 0);
            button18.Location = new Point(201, 177);
            button18.Name = "button18";
            button18.Size = new Size(183, 49);
            button18.TabIndex = 20;
            button18.Text = "БАРОМЕТР: оторваны провода (плохая пайка)";
            button18.UseVisualStyleBackColor = true;
            // 
            // button19
            // 
            button19.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button19.ForeColor = Color.FromArgb(128, 64, 0);
            button19.Location = new Point(390, 177);
            button19.Name = "button19";
            button19.Size = new Size(183, 49);
            button19.TabIndex = 21;
            button19.Text = "БАРОМЕТР: КЗ на барометре";
            button19.UseVisualStyleBackColor = true;
            // 
            // button20
            // 
            button20.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button20.ForeColor = Color.FromArgb(128, 64, 0);
            button20.Location = new Point(579, 177);
            button20.Name = "button20";
            button20.Size = new Size(183, 49);
            button20.TabIndex = 22;
            button20.Text = "БАРОМЕТР: перегрев";
            button20.UseVisualStyleBackColor = true;
            // 
            // button21
            // 
            button21.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button21.ForeColor = Color.FromArgb(128, 64, 0);
            button21.Location = new Point(766, 177);
            button21.Name = "button21";
            button21.Size = new Size(183, 49);
            button21.TabIndex = 23;
            button21.Text = "БАРОМЕТР: плохая пайка (капли и волосы на плате, лишний флюс)";
            button21.UseVisualStyleBackColor = true;
            // 
            // button22
            // 
            button22.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button22.ForeColor = Color.FromArgb(128, 64, 0);
            button22.Location = new Point(955, 177);
            button22.Name = "button22";
            button22.Size = new Size(183, 49);
            button22.TabIndex = 24;
            button22.Text = "БАРОМЕТР: не доделано";
            button22.UseVisualStyleBackColor = true;
            // 
            // button23
            // 
            button23.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button23.ForeColor = Color.Green;
            button23.Location = new Point(12, 232);
            button23.Name = "button23";
            button23.Size = new Size(183, 49);
            button23.TabIndex = 25;
            button23.Text = "ВИДЕО: не правильная распиновка";
            button23.UseVisualStyleBackColor = true;
            // 
            // button24
            // 
            button24.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button24.ForeColor = Color.Green;
            button24.Location = new Point(201, 232);
            button24.Name = "button24";
            button24.Size = new Size(183, 49);
            button24.TabIndex = 26;
            button24.Text = "ВИДЕО: оторваны провода (плохая пайка)";
            button24.UseVisualStyleBackColor = true;
            // 
            // button25
            // 
            button25.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button25.ForeColor = Color.Green;
            button25.Location = new Point(390, 232);
            button25.Name = "button25";
            button25.Size = new Size(183, 49);
            button25.TabIndex = 27;
            button25.Text = "ВИДЕО: КЗ на видео";
            button25.UseVisualStyleBackColor = true;
            // 
            // button26
            // 
            button26.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button26.ForeColor = Color.Green;
            button26.Location = new Point(766, 232);
            button26.Name = "button26";
            button26.Size = new Size(183, 49);
            button26.TabIndex = 28;
            button26.Text = "ВИДЕО: плохая пайка (капли и волосы на плате, лишний флюс)";
            button26.UseVisualStyleBackColor = true;
            // 
            // button27
            // 
            button27.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button27.ForeColor = Color.Green;
            button27.Location = new Point(955, 232);
            button27.Name = "button27";
            button27.Size = new Size(183, 49);
            button27.TabIndex = 29;
            button27.Text = "ВИДЕО: не доделано";
            button27.UseVisualStyleBackColor = true;
            // 
            // button28
            // 
            button28.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button28.ForeColor = Color.FromArgb(0, 192, 192);
            button28.Location = new Point(12, 287);
            button28.Name = "button28";
            button28.Size = new Size(183, 49);
            button28.TabIndex = 30;
            button28.Text = "СБОРКА: перевернутая TBS";
            button28.UseVisualStyleBackColor = true;
            // 
            // button29
            // 
            button29.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button29.ForeColor = Color.FromArgb(0, 192, 192);
            button29.Location = new Point(201, 287);
            button29.Name = "button29";
            button29.Size = new Size(183, 49);
            button29.TabIndex = 31;
            button29.Text = "СБОРКА: провода за стойкой";
            button29.UseVisualStyleBackColor = true;
            // 
            // button30
            // 
            button30.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button30.ForeColor = Color.FromArgb(0, 192, 192);
            button30.Location = new Point(390, 287);
            button30.Name = "button30";
            button30.Size = new Size(183, 49);
            button30.TabIndex = 32;
            button30.Text = "СБОРКА: перебиты провода";
            button30.UseVisualStyleBackColor = true;
            // 
            // button31
            // 
            button31.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button31.ForeColor = Color.FromArgb(0, 192, 192);
            button31.Location = new Point(579, 287);
            button31.Name = "button31";
            button31.Size = new Size(183, 49);
            button31.TabIndex = 33;
            button31.Text = "СБОРКА: барометр не правильно приклеен";
            button31.UseVisualStyleBackColor = true;
            // 
            // button32
            // 
            button32.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button32.ForeColor = Color.FromArgb(0, 192, 192);
            button32.Location = new Point(955, 287);
            button32.Name = "button32";
            button32.Size = new Size(183, 49);
            button32.TabIndex = 34;
            button32.Text = "СБОРКА: не дособрано";
            button32.UseVisualStyleBackColor = true;
            // 
            // button33
            // 
            button33.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button33.ForeColor = Color.Gray;
            button33.Location = new Point(12, 342);
            button33.Name = "button33";
            button33.Size = new Size(183, 49);
            button33.TabIndex = 35;
            button33.Text = "РАМА/ЛУЧИ: не протянуты болты";
            button33.UseVisualStyleBackColor = true;
            // 
            // button34
            // 
            button34.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button34.ForeColor = Color.Gray;
            button34.Location = new Point(955, 342);
            button34.Name = "button34";
            button34.Size = new Size(183, 49);
            button34.TabIndex = 36;
            button34.Text = "РАМА/ЛУЧИ: не дособрано";
            button34.UseVisualStyleBackColor = true;
            // 
            // button35
            // 
            button35.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button35.ForeColor = Color.Gray;
            button35.Location = new Point(201, 342);
            button35.Name = "button35";
            button35.Size = new Size(183, 49);
            button35.TabIndex = 37;
            button35.Text = "РАМА/ЛУЧИ: не верные метизы";
            button35.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(446, 423);
            label1.Name = "label1";
            label1.Size = new Size(213, 37);
            label1.TabIndex = 38;
            label1.Text = "СВОЙ ВАРИАНТ";
            // 
            // button36
            // 
            button36.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            button36.ForeColor = Color.Green;
            button36.Location = new Point(577, 232);
            button36.Name = "button36";
            button36.Size = new Size(183, 49);
            button36.TabIndex = 39;
            button36.Text = "ВИДЕО: нет изображения, но есть телеметрия";
            button36.UseVisualStyleBackColor = true;
            // 
            // FormTextWrite
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1150, 611);
            Controls.Add(button36);
            Controls.Add(label1);
            Controls.Add(button35);
            Controls.Add(button34);
            Controls.Add(button33);
            Controls.Add(button32);
            Controls.Add(button31);
            Controls.Add(button30);
            Controls.Add(button29);
            Controls.Add(button28);
            Controls.Add(button27);
            Controls.Add(button26);
            Controls.Add(button25);
            Controls.Add(button24);
            Controls.Add(button23);
            Controls.Add(button22);
            Controls.Add(button21);
            Controls.Add(button20);
            Controls.Add(button19);
            Controls.Add(button18);
            Controls.Add(button17);
            Controls.Add(button16);
            Controls.Add(button15);
            Controls.Add(button14);
            Controls.Add(button13);
            Controls.Add(button12);
            Controls.Add(button11);
            Controls.Add(button10);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(buttonSend);
            Controls.Add(buttonCancel);
            Controls.Add(richTextBoxMain);
            Name = "FormTextWrite";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormTextWrite";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox richTextBoxMain;
        private Button buttonCancel;
        private Button buttonSend;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button10;
        private Button button11;
        private Button button12;
        private Button button13;
        private Button button14;
        private Button button15;
        private Button button16;
        private Button button17;
        private Button button18;
        private Button button19;
        private Button button20;
        private Button button21;
        private Button button22;
        private Button button23;
        private Button button24;
        private Button button25;
        private Button button26;
        private Button button27;
        private Button button28;
        private Button button29;
        private Button button30;
        private Button button31;
        private Button button32;
        private Button button33;
        private Button button34;
        private Button button35;
        private Label label1;
        private Button button36;
    }
}