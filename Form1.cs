using System.Windows.Forms;

namespace FileMD5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // 允许窗体接收拖拽的文件  
            this.AllowDrop = true;

            // 添加拖拽事件处理器  
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;

            // 设置窗口置顶  
            //this.TopMost = true;

            // 监听键盘按键事件
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private bool TopMostFlag = false;
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // 检测是否按下F1键
            if (e.KeyCode == Keys.F1)
            {
                if (TopMostFlag == false)
                {
                    this.TopMost = true;
                    TopMostFlag = true;
                    this.Text = "File MD5 Calc (置顶模式)";
                }
                else
                {
                    this.TopMost = false;
                    TopMostFlag = false;
                    this.Text = "File MD5 Calc";
                }
            }
        }

        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            // 如果拖拽的是文件，则改变鼠标样式  
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            // 检查 e.Data 是否为 null 并进行类型转换  
            if (e.Data != null && e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                if (files.Length == 1)
                {
                    FindFile(files[0]);
                }
                else
                {
                    foreach (string file in files)
                    {
                        FindFiles(file);
                    }
                    MessageBox.Show(
                        this,
                        $"计算完毕: 共计算 {files.Length} 个文件\n计算结果请查看*.md5文件",
                        "文件信息",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void FindFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            string FileName = Path.GetFileName(path); // 获取文件名，不带路径    
            string? FilePath = Path.GetDirectoryName(path); // 获取文件路径    

            if (FilePath == null)
            {
                MessageBox.Show("无法获取文件路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 计算文件的MD5值    
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    string md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                    Clipboard.SetData(DataFormats.Text, (Object)md5Hash);

                    string outStr = "";
                    outStr += md5Hash + " *" + FileName;

                    // 将outStr写入文件  
                    string outputFilePath = Path.Combine(FilePath, FileName + ".md5");
                    File.WriteAllText(outputFilePath, outStr);

                    // 创建一个MessageBox并设置其父窗口为当前窗  
                    MessageBox.Show(
                        this,
                        $"文件名: {FileName}\n路径: {FilePath}\n\nMD5: {md5Hash}\n\n结果已复制到剪贴板\n结果已保存到文件: {outputFilePath}",
                        "文件信息",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
        private void FindFiles(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            string FileName = Path.GetFileName(path); // 获取文件名，不带路径    
            string? FilePath = Path.GetDirectoryName(path); // 获取文件路径    

            if (FilePath == null)
            {
                return;
            }

            // 计算文件的MD5值    
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    string md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                    string outStr = "";
                    outStr += md5Hash + " *" + FileName;

                    // 将outStr写入文件  
                    string outputFilePath = Path.Combine(FilePath, FileName + ".md5");
                    File.WriteAllText(outputFilePath, outStr);
                }
            }
        }
    }
}
