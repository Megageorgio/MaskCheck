using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaskCheckML.Model;

namespace MaskCheck
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog()
            {
                Title = "Open Image",
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png"
            })
            {
                if (dlg.ShowDialog() is DialogResult.OK)
                {
                    browseButton.Enabled = false;
                    ProcessImage(dlg.FileName);
                }
            }
        }

        private void ProcessImage(string path)
        {
            resultLabel.Text = "Wait...";
            Task.Run(() => CheckMask(path));
            pictureBox.Image = Image.FromFile(path);
        }

        private void CheckMask(string path)
        {
            var predictionResult = ConsumeModel.Predict(new ModelInput() {ImageSource = path});

            if (resultLabel.InvokeRequired)
            {
                resultLabel.BeginInvoke((MethodInvoker) (() => SetResultLabel(predictionResult.Prediction)));
            }
            else
            {
                SetResultLabel(predictionResult.Prediction);
            }
        }

        private void SetResultLabel(string prediction)
        {
            if (prediction is "with_mask")
            {
                resultLabel.ForeColor = Color.Lime;
                resultLabel.Text = "With mask!";
            }
            else
            {
                resultLabel.ForeColor = Color.Red;
                resultLabel.Text = "Without mask!";
            }

            browseButton.Enabled = true;
        }
    }
}