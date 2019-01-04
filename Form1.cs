using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Office.Interop.Word;
using Color = System.Drawing.Color;
using Path = System.IO.Path;
using Point = System.Drawing.Point;

namespace Kelime_Arama
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            label3.Text = "";
        }




        OpenFileDialog file = new OpenFileDialog();
        OpenFileDialog file2 = new OpenFileDialog(); // sadece word için


        //+++++++++++++++++++++++++++++++ Dosya Seçme İşlemi ++++++++++++++++++++++++++++++++++++++++++++++++++++
        public void button2_Click(object sender, EventArgs e)
        {



            file.Title = "Kelime Aranacak Dosyayı Seçiniz..";
            file.Filter = @"Tüm Dosyalar|*.txt;*.docx;*.doc;*.pdf|Metin Belgeleri (.txt)|*.txt|Word Dosyaları (.docx ,.doc)|*.docx;*.doc|PDF Dosyaları (.pdf)|*.pdf";
            file.FilterIndex = 1;//ilk önce TÜM arasın. index 3
            file.Multiselect = true;



            if (file.ShowDialog() == DialogResult.OK)
            {
                // dosya seçildi ise
                listView1.Clear();
                label1.BackColor = Color.GreenYellow;
                label1.Text = "Dosya Seçildi";


                string[] dosyaIsimler = file.SafeFileNames; //Çoklu seçimdeki dosyaların ismi
                string[] dosyalar = file.FileNames;
                listView1.Items.Add("\n");
                for (int i = 0; i < dosyaIsimler.Length; i++)
                {
                    listView1.Items.Add((dosyaIsimler[i]) + "\n");
                }


            }

            if (label1.Text == "Dosya Seçildi")
            {
                button2.BackColor = Color.LawnGreen;
            }

        }


        //+++++++++++++++++++++++++++++++ Dosyaları Aktarma  İşlemi +++++++++++++++++++++++++++++++++++++++++++++++

        private void button1_Click(object sender, EventArgs e)
        {


            string[] dosyalar = file.FileNames; // seçilen tüm dosyalar
            string uzanti= file.FileName; //uzantı kontrolü için aldık

            int l = 0;
            try
            {


                richTextBox1.Clear(); // önce temizlensin
                string[] dosyaIsimler = file.SafeFileNames; //Çoklu seçimdeki dosyaların ismi

                int isim=0;


                foreach (var dosya in dosyalar)
                {


                    if (dosya.EndsWith(".txt"))
                    {


                        string etiket = "\n\n#############################################> "+dosyaIsimler[isim]+" <###########################################\n";


                        string metin = System.IO.File.ReadAllText(dosya,Encoding.GetEncoding("windows-1254"));
                        richTextBox1.Text = richTextBox1.Text + " " + etiket + " " + metin;

                        isim++;
                        l++;
                    }


                    else if (dosya.EndsWith(".docx") || dosya.EndsWith(".doc"))
                    {
                        string etiket = "\n\n#############################################> "+dosyaIsimler[isim]+" <###########################################\n";
                        DocumentFormat.OpenXml.Packaging.WordprocessingDocument doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(dosya, false);

                        string yazilar = doc.MainDocumentPart.Document.Body.InnerText.ToString();


                        richTextBox1.Text = richTextBox1.Text + etiket + yazilar;


                        isim++;
                        l++;


                    }


                    else if (dosya.EndsWith(".pdf"))
                    {
                        string yol= dosya;

                        using (PdfReader reader = new PdfReader(yol))
                        {
                            StringBuilder text = new StringBuilder();

                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                            }
                            string etiket = "\n\n#############################################> "+dosyaIsimler[isim]+" <###########################################\n";
                            string yazilar = text.ToString();

                            richTextBox1.Text = richTextBox1.Text + " " + etiket + " " + yazilar;
                            isim++;
                            l++;
                            //EN SON PDF OKUMA PARÇALAMA YAPILDI..
                        }

                    }


                }



            }

            catch (Exception)
            {

                MessageBox.Show("Hata Oluştu \nDosya bozuk olabilir.\nDosya zaten kullanılıyor olabilir.\nSeçtiğiniz dosyanın kapalı olduğundan emin olun!\n\nCTRL Shift ve ESC tuşlarına aynı anda basın\nÇalışan Word Dosyalarını Kapatın.\nTekrar Deneyin...\n\n Hata Oluşturan Dosya =" + dosyalar[l],
     "Uyarı",
     MessageBoxButtons.OK,
     MessageBoxIcon.Exclamation,
     MessageBoxDefaultButton.Button1);
            }



        }





        //+++++++++++++++++++++++++++++++ Temizleme İşlemi +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }




        //+++++++++++++++++++++++++++++++ Kelime Arama İşlemi ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Text != "")
                {
                    backgroundWorker1.RunWorkerAsync();

                    progressBar1.Style = ProgressBarStyle.Marquee;
                }

                else
                {
                    MessageBox.Show("Lütfen Önce Dosyaları Aktarın", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    progressBar1.Value = 0;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Beklenmedik bir hata meydana geldi", "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
            

        }



        //+++++++++++++++++++++++++++++++ Word  Arama  ve Seçme İşlemi ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void button6_Click(object sender, EventArgs e)
        {

            file2.Title = "Kelime Aranacak Dosyayı Seçiniz..";
            file2.Filter = @"Word Dosyaları (.docx ,.doc)|*.docx;*.doc";
            file2.FilterIndex = 1;
            file2.Multiselect = true;

            if (file2.ShowDialog() == DialogResult.OK)
            {
                // dosya seçildi ise


                string[] worddosyaIsimler = file2.SafeFileNames; //Çoklu seçimdeki dosyaların ismi
                string[] worddosyalar = file2.FileNames;
                string yaziolarak ="";
                for (int i = 0; i < worddosyaIsimler.Length; i++)
                {
                    yaziolarak = yaziolarak + worddosyaIsimler[i] + "\n";
                }

                MessageBox.Show("Seçilen Dosyalar : \n------------------------\n" + yaziolarak,
    "Seçim Özet");

                button5.BackColor = Color.Green;
                button5.ForeColor = Color.White;

            }

        }


        //+++++++++++++++++++++++++++++++ pdf oluşturma İşlemi ++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                backgroundWorker2.RunWorkerAsync();


            }
            catch (Exception)
            {

                MessageBox.Show("Lütfen Bekleyin!", "Uygulama Meşgul...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string[] worddosyalar = file2.FileNames;
            string[] wordisimler = file2.SafeFileNames;
            progressBar2.Visible = true;
            progressBar2.Maximum = worddosyalar.Length; // progres barın maximum değeri seçilen dosya sayısı kadar olsun.
        }



        // +-+-+-+-++-+-+-+-+-+-+BACKGROUND WORKER 1  ÇALIŞMA YERİ  - KELİME ARA   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int sonuc=0;

            int start = 0;
            int end = richTextBox1.Text.LastIndexOf(textBox1.Text);

            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = Color.White;


            while (start < end)
            {
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    label3.Text = "Bulunan Sonuç : 0" ;
                    return;
                }
                else
                {
                    richTextBox1.Find(textBox1.Text, start, richTextBox1.TextLength, RichTextBoxFinds.MatchCase);

                    richTextBox1.SelectionBackColor = Color.Red;
                    sonuc++;

                    start = richTextBox1.Text.IndexOf(textBox1.Text, start) + 1;
                    backgroundWorker1.ReportProgress(sonuc);
                }

                
            }

            label3.Text = "Bulunan Sonuç : " + sonuc;

        }
        // +-+-+-+-+-+-+-+--+-+-+-+-+BACKGROUND WORKER 1  ÇALIŞIRKEN DEĞİŞECEK YERLER +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {

            progressBar1.Value = e.ProgressPercentage;

            label3.Text = "Bulunan Sonuç : " + e.ProgressPercentage.ToString();

        }
        // +-+-+-+-+-+-+-+-+-+-+-+-+herşey Tamamlanınca +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                MessageBox.Show("İptal Edildi !", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 0;
            }
            else
            {
                MessageBox.Show("Arama İşlemi Tamamlandı !", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar1.Style = ProgressBarStyle.Continuous;
                progressBar1.Value = 0;
            }

            
           
        }




        // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+BACKGROUND WORKER 2 ÇALIŞMA YERİ  - PDF ÇEVİR   +-+-+-+-+-+-+-+-+-+
        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string[] worddosyalar = file2.FileNames;
            string[] wordisimler = file2.SafeFileNames;

            try
            {
                //Dosya Varmı Yokmu ? - Yoksa Oluştur..
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!Directory.Exists(Path.GetDirectoryName(path + @"\Yeni_Pdf_Dosyalari\")))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path + @"\Yeni_Pdf_Dosyaları\"));

                }


                //PDF çevirme kodlarımız.
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();


                int i = 0;
                if (worddosyalar.Length > 0) //Dosya Seçilmiş mi ? 
                {


                    foreach (var dosya in worddosyalar)
                    {
                        wordDocument = appWord.Documents.Open(dosya);
                        wordDocument.ExportAsFixedFormat(path + @"\Yeni_Pdf_Dosyaları\" + wordisimler[i] + ".pdf",
                            WdExportFormat.wdExportFormatPDF);
                        i++;

                        backgroundWorker2.ReportProgress(i);//progres bar'a rapor..
                    }
                    MessageBox.Show("Dönüştürme Başarılı bir şekilde yapıldı\n Konum:\n\n" + path + @"\Yeni_Pdf_Dosyaları", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }

                else
                {
                    MessageBox.Show("Lütfen ilk önce dosyaları seçin !", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            catch (Exception)
            {

                MessageBox.Show("Hata Oluştu \nDosya bozuk olabilir.\nDosya kullanılıyor olabilir.\nSeçtiğiniz dosyanın PDF'i zaten aynı konumda olabilir. \nTekrar Deneyin...\n\nCTRL Shift ve Esc Tuşlarına aynı anda basın\nTüm Word Dosyalarını Kapatın!",
                     "Uyarı",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Exclamation,
                     MessageBoxDefaultButton.Button1);
            }


        }
        public Microsoft.Office.Interop.Word.Document wordDocument { get; set; }


        // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+BACKGROUND WORKER 2 ÇALIŞMA YERİ  - değişecek yerler +-+-+-+-+-+-+-
        private void backgroundWorker2_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            button2.Enabled = false;
            button3.Enabled = false;
            button6.Enabled = false;
        }

        // +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+BACKGROUND WORKER 2 ÇALIŞMA YERİ  - Herşey tamamlanınca +-+-+-+-+-+-
        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progressBar2.Value = 0;
            button2.Enabled = true;
            button3.Enabled = true;
            button6.Enabled = true;
        }




        //İPTAL ETME YERİ
        private void button7_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }
    }
}


