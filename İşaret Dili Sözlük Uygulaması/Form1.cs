using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using WMPLib;

namespace İşaret_Dili_Sözlük_Uygulaması
{
    public partial class Form1 : Form
    {
        internal bool canKeyboard;
        internal List<Word> words;
        internal List<Word> words_sorted;
        internal string videoPath;
        internal bool inLoop;
        internal double playBackSpeed;
        internal int exampleSentenceCount;
        List<TextBox> example_textbox;
        List<TextBox> example_transcript;
        List<Button> example_videoButton;
        internal int selectedExampleIndex;
        private void CheckDirectories()
        {
            if (!Directory.Exists("xml"))
            {
                Directory.CreateDirectory("xml");
            }
            if (!Directory.Exists("videos"))
            {
                Directory.CreateDirectory("videos");
            }
        }
        private void CheckXMLFiles()
        {

            if (!File.Exists("xml/kelimeler.xml"))
            {
                File.Create("xml/kelimeler.xml");
                MessageBox.Show("Program açıldığında kelimeleri içeren XML dosyası bulunamadı. Yeni XML dosyası oluşturuldu.", "Dosya bulunamadı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public Form1()
        {
            InitializeComponent();
            tabControl1.DrawItem += tabControl1_DrawItem;
            tabControl1.Parent = panel4;
            tabPage1.ForeColor = Color.Black;
            tabPage1.BackColor = Color.Gold;
            //tabControl1.Dock = DockStyle.Fill;
            //Panel panel = new Panel()
            //{
            //    Dock = DockStyle.Fill,
            //    BackColor = Color.Transparent
            //};
            //tabControl1.Parent = panel;
            //Controls.Add(panel);
            //tabControl1.Parent = new Panel()
            //{
            //    BackColor = Color.Transparent
            //};
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Transparent, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.Visible = false;
            panel_words.Visible = false;
            panelControls.Visible = false;
            cb_keyboard.Checked = true;
            canKeyboard = cb_keyboard.Checked;
            axWindowsMediaPlayer2.uiMode = "full";
            axWindowsMediaPlayer2.Visible = false;
            panel2.Visible = false;
            tabPage1.BackColor = Color.Transparent; tabPage2.BackColor = Color.Transparent;
            CheckDirectories();
            CheckXMLFiles();
            words = XMLReader.ReadWordFile();
            words_sorted = words.OrderBy(x => x.Name).ToList();
            exampleSentenceCount = 0;
            foreach (var word in words_sorted)
            {
                exampleSentenceCount += word.Examples.Count;
            }
            //label7.Text = String.Format("Toplam: {0} kelime ve {1} örnek cümle", words.Count, exampleSentenceCount);
            foreach (Word word in words_sorted)
            {
                lb_words.Items.Add(word.Name);
            }
            playBackSpeed = 1.0;
            rb_playback3.Checked = true;
            example_textbox = new List<TextBox>
            {
                tb_example1, tb_example2, tb_example3
            };
            example_transcript = new List<TextBox>
            {
                tb_transcript1, tb_transcript2, tb_transcript3
            };
            example_videoButton = new List<Button>
            {
                btn_exvideo1, btn_exvideo2, btn_exvideo3
            };
            foreach(var button in example_videoButton)
            {
                button.Visible = false;
            }
            btn_max.Text = "O";
        }

        private void btn_all_Click(object sender, EventArgs e)
        {
            if (txt_word.Text.Length == 0)
                panel_words.Visible = !panel_words.Visible;
            if (lb_words.Items.Count < words_sorted.Count)
            {
                lb_words.Items.Clear();
                foreach (var word in words_sorted)
                {
                    lb_words.Items.Add(word.Name);
                }
            }
        }

        private void cb_keyboard_CheckedChanged(object sender, EventArgs e)
        {
            canKeyboard = cb_keyboard.Checked;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !canKeyboard;
        }

        private void LetterButtonClick(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            txt_word.Text += button.Text;
        }

        private void txt_word_TextChanged(object sender, EventArgs e)
        {
            if (txt_word.Text.Length == 1)
                panel_words.Visible = true;
            else if (txt_word.Text.Length == 0)
            {
                panel_words.Visible = false;
                panelControls.Visible = false;
                axWindowsMediaPlayer2.Visible = false;
                panel2.Visible = false;
            }
            if (txt_word.Text.Length > 0)
            {
                List<Word> filteredWords = words_sorted.FindAll(word => word.Name.StartsWith(txt_word.Text, StringComparison.OrdinalIgnoreCase));
                lb_words.Items.Clear();
                foreach (Word word in filteredWords)
                {
                    lb_words.Items.Add(word.Name);
                }
            }
            else
            {
                lb_words.Items.Clear();
                foreach (Word word in words_sorted)
                {
                    lb_words.Items.Add(word.Name);
                }
            }

        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            txt_word.Text = string.Empty;
            axWindowsMediaPlayer2.Ctlcontrols.stop();
            tabControl1.Visible = false;
            axWindowsMediaPlayer2.Visible = false;
            panelControls.Visible = false;
            panel2.Visible = false;
            for (int i = 0; i < 3; i++)
            {
                example_textbox[i].Text = string.Empty;
            }
        }

        private void btn_random_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, words.Count);
            txt_word.Text = words_sorted[randomIndex].Name;
            panel_words.Visible = true;
            axWindowsMediaPlayer2.Ctlcontrols.stop();
        }

        private void lb_words_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_word.Text = lb_words.Items[lb_words.SelectedIndex].ToString();
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            lb_words.SelectedIndex = 0;
            tb_description.Text = words_sorted[index].Description;
            for (int i = 0; i < 3; i++)
            {
                example_textbox[i].Text = string.Empty;
                example_transcript[i].Text = string.Empty;
                example_videoButton[i].Visible = false;
            }
            for (int i = 0; i < Math.Min(words_sorted[index].Examples.Count, 3); i++)
            {
                example_textbox[i].Text = words_sorted[index].Examples[i].Sentence;
                example_transcript[i].Text = words_sorted[index].Examples[i].Transcript;
                if (words_sorted[index].Examples[i].Video != null || words_sorted[index].Examples[i].Video != string.Empty)
                {
                    example_videoButton[i].Visible = true;
                }
                
            }
        }

        private void lb_words_DoubleClick(object sender, EventArgs e)
        {
            inLoop = false;
            tabControl1.Visible = true;
            panelControls.Visible = true;
            axWindowsMediaPlayer2.Visible = true;
            panel2.Visible = true;
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            if (index != -1)
            {
                videoPath = String.Format("videos/{0}", words_sorted[index].Video);
                if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
                {
                    axWindowsMediaPlayer2.URL = videoPath;
                    axWindowsMediaPlayer2.settings.rate = playBackSpeed;
                    axWindowsMediaPlayer2.Ctlcontrols.play();
                }
            }

        }

        private void btn_replay_Click(object sender, EventArgs e)
        {
            inLoop = false;
            axWindowsMediaPlayer2.settings.autoStart = inLoop;
            if (axWindowsMediaPlayer2.playState != WMPLib.WMPPlayState.wmppsStopped)
            {
                axWindowsMediaPlayer2.Ctlcontrols.stop();
            }
            axWindowsMediaPlayer2.settings.rate = playBackSpeed;
            axWindowsMediaPlayer2.Ctlcontrols.play();
        }

        private void btn_replay2_Click(object sender, EventArgs e)
        {
            inLoop = true;
            axWindowsMediaPlayer2.settings.autoStart = inLoop;
            axWindowsMediaPlayer2.settings.setMode("loop", true);
            if (axWindowsMediaPlayer2.playState != WMPLib.WMPPlayState.wmppsStopped)
            {
                axWindowsMediaPlayer2.Ctlcontrols.stop();
                axWindowsMediaPlayer2.settings.rate = playBackSpeed;
                axWindowsMediaPlayer2.Ctlcontrols.play();
            }
            else
            {
                axWindowsMediaPlayer2.URL = videoPath;
                axWindowsMediaPlayer2.settings.rate = playBackSpeed;
                axWindowsMediaPlayer2.Ctlcontrols.play();
            }

        }

        private void rb_playback1_CheckedChanged(object sender, EventArgs e)
        {
            playBackSpeed = 0.25;
            axWindowsMediaPlayer2.settings.rate = playBackSpeed;
        }

        private void rb_playback2_CheckedChanged(object sender, EventArgs e)
        {
            playBackSpeed = 0.5;
            axWindowsMediaPlayer2.settings.rate = playBackSpeed;
        }

        private void rb_playback3_CheckedChanged(object sender, EventArgs e)
        {
            playBackSpeed = 1.0;
            axWindowsMediaPlayer2.settings.rate = playBackSpeed;
        }

        private void rb_playback4_CheckedChanged(object sender, EventArgs e)
        {
            playBackSpeed = 1.5;
            axWindowsMediaPlayer2.settings.rate = playBackSpeed;
        }

        private void rb_playback5_CheckedChanged(object sender, EventArgs e)
        {
            playBackSpeed = 2.0;
            axWindowsMediaPlayer2.settings.rate = playBackSpeed;
        }

        private void txt_word_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer2.Ctlcontrols.stop();
            axWindowsMediaPlayer2.Visible = false;
            panel2.Visible = false;
        }

        //private void PlayExampleVideo(object sender, EventArgs e)
        //{
        //    txt_word.Text = lb_words.Items[lb_words.SelectedIndex].ToString();
        //    int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
        //    tabControl1.SelectedTab = tabControl1.TabPages["tabPage1"];
        //    videoPath = String.Format("videos/{0}", words_sorted[index].Video);
        //}

        private void btn_exvideo1_Click(object sender, EventArgs e)
        {
            selectedExampleIndex = 0;
            panelControls.Visible = true;
            axWindowsMediaPlayer2.Visible = true;
            panel2.Visible = true;
            txt_word.Text = lb_words.Items[lb_words.SelectedIndex].ToString();
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            tabControl1.SelectedTab = tabControl1.TabPages["tabPage1"];
            if (words_sorted[index].Examples[0].Transcript == null)
                cb_transcript.Enabled = false;
            else
                cb_transcript.Enabled = true;

            if (words_sorted[index].Examples[0].Sentence == null)
                cb_meaning.Enabled = false;
            else
                cb_meaning.Enabled = true;
            if (words_sorted[index].Examples[0].Video != null)
            {
                videoPath = String.Format("videos/{0}", words_sorted[index].Examples[0].Video);
                axWindowsMediaPlayer2.URL = videoPath;
                axWindowsMediaPlayer2.settings.rate = playBackSpeed;
                axWindowsMediaPlayer2.Ctlcontrols.play();
                if (cb_transcript.Checked == true)
                {
                    if (words_sorted[index].Examples[0].Transcript != null)
                        tb_exDesc.Text = words_sorted[index].Examples[0].Transcript;
                }
                if (cb_meaning.Checked == true)
                {
                    if (words_sorted[index].Examples[0].Sentence != null)
                        tb_exMean.Text = words_sorted[index].Examples[0].Sentence;
                }
            }

        }

        private void btn_exvideo2_Click(object sender, EventArgs e)
        {
            selectedExampleIndex = 1;
            panelControls.Visible = true;
            axWindowsMediaPlayer2.Visible = true;
            panel2.Visible = true;
            txt_word.Text = lb_words.Items[lb_words.SelectedIndex].ToString();
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            tabControl1.SelectedTab = tabControl1.TabPages["tabPage1"];
            if (words_sorted[index].Examples[1].Transcript == null)
                cb_transcript.Enabled = false;
            else
                cb_transcript.Enabled = true;

            if (words_sorted[index].Examples[1].Sentence == null)
                cb_meaning.Enabled = false;
            else
                cb_meaning.Enabled = true;
            if (words_sorted[index].Examples[1].Video != null)
            {
                videoPath = String.Format("videos/{0}", words_sorted[index].Examples[1].Video);
                axWindowsMediaPlayer2.URL = videoPath;
                axWindowsMediaPlayer2.settings.rate = playBackSpeed;
                axWindowsMediaPlayer2.Ctlcontrols.play();
                if (cb_transcript.Checked == true)
                {
                    if (words_sorted[index].Examples[1].Transcript != null)
                        tb_exDesc.Text = words_sorted[index].Examples[1].Transcript;
                }
                if (cb_meaning.Checked == true)
                {
                    if (words_sorted[index].Examples[1].Sentence != null)
                        tb_exMean.Text = words_sorted[index].Examples[1].Sentence;
                }
            }
        }

        private void btn_exvideo3_Click(object sender, EventArgs e)
        {
            selectedExampleIndex = 2;
            panelControls.Visible = true;
            axWindowsMediaPlayer2.Visible = true;
            panel2.Visible = true;
            txt_word.Text = lb_words.Items[lb_words.SelectedIndex].ToString();
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            tabControl1.SelectedTab = tabControl1.TabPages["tabPage1"];
            if (words_sorted[index].Examples[2].Transcript == null)
                cb_transcript.Enabled = false;
            else
                cb_transcript.Enabled = true;

            if (words_sorted[index].Examples[2].Sentence == null)
                cb_meaning.Enabled = false;
            else
                cb_meaning.Enabled = true;
            if (words_sorted[index].Examples[2].Video != null)
            {
                videoPath = String.Format("videos/{0}", words_sorted[index].Examples[2].Video);
                axWindowsMediaPlayer2.URL = videoPath;
                axWindowsMediaPlayer2.settings.rate = playBackSpeed;
                axWindowsMediaPlayer2.Ctlcontrols.play();
                if (cb_transcript.Checked == true)
                {
                    if (words_sorted[index].Examples[2].Transcript != null)
                        tb_exDesc.Text = words_sorted[index].Examples[2].Transcript;
                }
                if (cb_meaning.Checked == true)
                {
                    if (words_sorted[index].Examples[2].Sentence != null)
                        tb_exMean.Text = words_sorted[index].Examples[2].Sentence;
                }
            }
        }

        private void cb_transcript_CheckedChanged(object sender, EventArgs e)
        {
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            if (cb_transcript.Checked == true)
            {
                tb_exDesc.Text = words_sorted[index].Examples[selectedExampleIndex].Transcript;
            }
            else
            {
                tb_exDesc.Text = string.Empty;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            int index = words_sorted.FindIndex(word => word.Name == txt_word.Text);
            if (cb_meaning.Checked == true)
            {
                tb_exMean.Text = tb_exMean.Text = words_sorted[index].Examples[selectedExampleIndex].Sentence;
            }
            else
            {
                tb_exMean.Text = string.Empty;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer2.playState != WMPLib.WMPPlayState.wmppsStopped)
            {
                axWindowsMediaPlayer2.Ctlcontrols.stop();
                //panelControls.Visible = false;
                //axWindowsMediaPlayer2.Visible = false;
                //panel2.Visible = false;
            }
        }

        private void btn_x_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_max_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                // Maximize the form
                WindowState = FormWindowState.Maximized;
                btn_max.Text = "o";
            }
            else
            {
                // Restore the form to normal size
                WindowState = FormWindowState.Normal;
                btn_max.Text = "O";
            }
        }

        private void btn_min_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
