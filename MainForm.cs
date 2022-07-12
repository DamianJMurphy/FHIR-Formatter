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
            OutputTextBox.Text = Converter.ToJson(InputTextBox.Text, inputForm);
            needsSaving = true;
            lastConversion = "json";
        }

        private void XMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputTextBox.Text))
            {
                return;
            }
            OutputTextBox.Text = Converter.ToXml(InputTextBox.Text, inputForm);
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
            string body = name.Substring(0, dot + 1);
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
    }
}