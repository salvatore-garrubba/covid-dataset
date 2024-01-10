using Microsoft.EntityFrameworkCore;
using Store_files.Data;
using Store_files.Data.Models;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Store_files
{
    public partial class StoreFilesForm : Form
    {
        // TODO: add logger.
        private DatabaseContext? _db;
        private string _filePath = "";

        public StoreFilesForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this._db = new DatabaseContext();
            this._db.Database.EnsureCreated();
            this._db.Files.LoadAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            this._db?.Dispose();
            this._db = null;
        }

        private void browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = @"C:\",
                Title = "Select file",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "csv",
                Filter = "csv files (*.csv)|*.csv",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            // TODO: add check if file is 'csv' format.
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _filePath = openFileDialog.FileName;
                selectedFileTextbox.Text = _filePath;
                try
                {
                    resultTextbox.Text = StreamToString(_filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void import_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_filePath))
            {
                try
                {
                    _db?.Files.Add(new Filedoc { FileBytes = StreamToByteArray(_filePath), UploadedDateTime = DateTime.UtcNow });
                    _db?.SaveChanges();
                    MessageBox.Show("Success!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                MessageBox.Show("Stream was empty.");
        }

        private static string StreamToString(string filePath)
        {
            using StreamReader reader = new(filePath);
            return reader.ReadToEnd();
        }

        private static byte[] StreamToByteArray(string filePath)
        {

            var stream = File.OpenRead(filePath);
            using BinaryReader reader = new(stream);
            return reader.ReadBytes((int)stream.Length);
        }
    }
}