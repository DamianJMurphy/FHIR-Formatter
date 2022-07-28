namespace FHIR_Formatter
{
    public partial class MainForm : Form
    {
        private bool needsSaving = false;
        private string inputForm = string.Empty;
        private string inputFileName = string.Empty;
        private string lastConversion = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void JSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                return;
            }
            string output = Converter.ToJson(InputTextBox.Text, inputForm);
            if (string.IsNullOrEmpty(output))
            {
                OutputTextBox.Text = "No FHIR resource found";
            }
            else
            {
                OutputTextBox.Text = output;
            }
            needsSaving = true;
            lastConversion = "json";
        }

        private void XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                return;
            }
            string output = Converter.ToXml(InputTextBox.Text, inputForm);
            if (string.IsNullOrEmpty(output))
            {
                OutputTextBox.Text = "No FHIR resource found";
            } else
            {
                OutputTextBox.Text = output;
            }
            needsSaving = true;
            lastConversion = "xml";
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputForm = string.Empty;
            using OpenFileDialog openFile = new();
            openFile.Multiselect = false;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFile.FileName;
                inputFileName = filePath;

                using StreamReader reader = new(filePath);
                InputTextBox.Text = reader.ReadToEnd();
                if (InputTextBox.Text.StartsWith("{"))
                {
                    inputForm = "json";
                }
                else
                {
                    if (InputTextBox.Text.StartsWith("<"))
                    {
                        inputForm = "xml";
                    }
                    else
                    {
                        MessageBox.Show("Cannot determine content type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            OutputTextBox.Text = String.Empty;
        }

        private static void HandleDirectory(string inpath, string outpath)
        {
            DirectoryInfo directory = new(inpath);
            if (MessageBox.Show("Convert all XML in " + inpath + " to JSON ?", "Convert directory", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                IEnumerable<FileInfo> files = directory.EnumerateFiles();
                foreach (FileInfo f in files)
                {
                    if (f.FullName.EndsWith(".xml") || f.FullName.EndsWith(".XML"))
                    {
                        using StreamReader reader = new(f.FullName);
                        string s = reader.ReadToEnd();
                        string j = Converter.ToJson(s, "xml");

                        string outname = RetypeFileName(f.Name, "json");
                        using StreamWriter streamWriter = new(outpath + Path.DirectorySeparatorChar + outname);
                        streamWriter.Write(j);
                    }
                }
            }
        }

        private static string RetypeFileName(string name, string type)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            string checkExtension = name.ToLower();
            if (!checkExtension.EndsWith("json") && !checkExtension.EndsWith("xml"))
            {
                return name + "." + type;
            }
            int dot = name.LastIndexOf('.');
            if (dot == -1)
            {
                return name;
            }
            string body = name[..(dot + 1)];
            return body + type;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(OutputTextBox.Text))
            {
                return;
            }
            using (SaveFileDialog saveFile = new())
            {
                saveFile.Filter = lastConversion + " files (*." + lastConversion + ") | *." + lastConversion;
                saveFile.FileName = RetypeFileName(inputFileName, lastConversion);
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    using StreamWriter sw = new(saveFile.FileName);
                    sw.WriteLine(OutputTextBox.Text);
                }
            }
            needsSaving = false;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (needsSaving)
            {
                if (MessageBox.Show("Exit ?", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    return;
                }
            }
            Application.Exit();
        }

        private void DirectoryMenuItem_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog openFile = new();
            openFile.ShowNewFolderButton = false;
            openFile.Description = "Source directory";
            openFile.UseDescriptionForTitle = true;
            if (openFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string filePath = openFile.SelectedPath;
            if (!Directory.Exists(filePath))
            {
                return;
            }
            using FolderBrowserDialog saveFile = new();
            saveFile.ShowNewFolderButton = true;
            saveFile.Description = "Output directory";
            saveFile.UseDescriptionForTitle = true;
            if (saveFile.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string outPath = saveFile.SelectedPath;

            this.Cursor = Cursors.WaitCursor;
            HandleDirectory(filePath, outPath);
            this.Cursor = Cursors.Default;
        }
    }
}