using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelToMariaDBConverter
{
    public partial class MainForm : Form
    {
        private string _inputFilePath;
        private HttpClient _httpClient;

        public MainForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            btnConvert.Click += BtnConvert_Click;
            AllowDrop = true;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private async void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0 && Path.GetExtension(files[0]).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                _inputFilePath = files[0];
                lblStatus.Text = $"File Ready to Convert";
            }
            else
            {
                MessageBox.Show("Please drop a valid .xlsx file.", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnConvert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_inputFilePath))
            {
                MessageBox.Show("Please drop a valid .xlsx file before converting.", "No File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog1.Filter = "ZIP Files (*.zip)|*.zip";
            saveFileDialog1.DefaultExt = "zip";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string outputFilePath = saveFileDialog1.FileName;

                try
                {
                    progressBar.Value = 0;
                    progressBar.Visible = true;

                    await ConvertExcelToMariaDB(_inputFilePath, outputFilePath);
                    MessageBox.Show($"Conversion successful. Output saved to {outputFilePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error converting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progressBar.Visible = false;
                }
            }
        }

        private async Task ConvertExcelToMariaDB(string inputFilePath, string outputFilePath)
        {
            using var fileStream = new FileStream(inputFilePath, FileMode.Open);
            var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            var progressContent = new ProgressableStreamContent(fileContent, 4096, reportUploadProgress);
            content.Add(progressContent, "files[]", Path.GetFileName(inputFilePath));

            var response = await _httpClient.PostAsync("https://www.rebasedata.com/api/v1/convert?outputFormat=mariadb&errorResponse=zip", content);

            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                using var outputStream = new FileStream(outputFilePath, FileMode.Create);

                var responseStreamContent = new StreamContent(responseStream);
                var responseContent = new ProgressableStreamContent(responseStreamContent, 4096, reportDownloadProgress);
                await responseContent.CopyToAsync(outputStream);
            }
            else
            {
                throw new Exception($"Conversion failed with status code {response.StatusCode}");
            }
        }


        private void reportUploadProgress(long bytesSent, long totalBytes)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => reportUploadProgress(bytesSent, totalBytes)));
                return;
            }

            progressBar.Value = (int)((double)bytesSent / totalBytes * 50); // 50% for upload
        }

        private void reportDownloadProgress(long bytesReceived, long totalBytes)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => reportDownloadProgress(bytesReceived, totalBytes)));
                return;
            }

            progressBar.Value = 50 + (int)((double)bytesReceived / totalBytes * 50); // 50% for download
        }
    }
}
