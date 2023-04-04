using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;


namespace unpackPicture
{
    public struct TileObject
    {
        public string FileName;
        public Rectangle rect;
        //图片图集保存位置
        public string SavePath;
        //原始图片位置
        public string TexturePackName;
        public TileObject(string _TexturePackName, string _SavePath, string _filename, Rectangle _rect)
        {
            TexturePackName = _TexturePackName;
            SavePath = _SavePath;
            FileName = _filename;
            rect = _rect;
        }
    }

    public partial class Form1 : Form
    {
        Bitmap myBitmap = null;
        List<TileObject> TileObjects;
        string SavePath;

        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //创建FolderBrowserDialog对象
            var dialog = new FolderBrowserDialog();

            //设置初始文件夹
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;

            //设定对话框的标题       
            dialog.Description = "请选择要处理的文件夹";

            //弹出对话框选择文件夹
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //获取用户选择的文件夹路径
                var folderPath = dialog.SelectedPath;

                //在这里可以根据选中的文件夹路径执行相应的操作
                //...
                string[] pngFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories); // 获取所有后缀为png的文件路径
                TileObjects = new List<TileObject>();
                foreach (string file in pngFiles)
                {
                    // 在这里可以根据 png 文件的路径执行相应的操作，例如读取文件内容等等

                    if (comboBox1.SelectedItem.ToString() == "tpsheet")
                    {
                        //有后缀的
                        //string name = Path.GetFileName(file);
                        //无有后缀的
                        string name = Path.GetFileNameWithoutExtension(file);
                        string filepath =Path.GetDirectoryName(file);
                        string path = "";
                        SavePath = filepath + "\\unpack" + name + "\\";
                        path = filepath + "\\" + name + ".tpsheet";

                        if (File.Exists(path))
                        {
                            
                            string[] lines = System.IO.File.ReadAllLines(path);


                            myBitmap = new Bitmap(file);
                            Point pSize = new Point(myBitmap.Width, myBitmap.Height);
                            for (int i = 0; i < lines.Length; i++)
                            {
                                string[] temp = lines[i].Split(';');
                                if (temp.Length == 7)
                                {
                                    int w = int.Parse(temp[3]);
                                    int h = int.Parse(temp[4]);
                                    int x = int.Parse(temp[1]);
                                    int y = pSize.Y - int.Parse(temp[2]) - h;

                                    TileObject tileObj = new TileObject(file, SavePath, _filename: temp[0], _rect: new Rectangle(x, y, w, h));
                                    TileObjects.Add(tileObj);
                                }
                            }

                        }
                        else
                        {
                            // 如果文件存在，进入下一次循环
                            continue;
                        }

                       
                    }


                }

                //最后释放对话框对象
                dialog.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "tpsheet")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "BMP File| *.png";
                openFileDialog1.Title = "取得影像檔";
                openFileDialog1.FilterIndex = 3;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    myBitmap = new Bitmap(openFileDialog1.FileName);
                }

                string name = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                string file = Path.GetDirectoryName(openFileDialog1.FileName);
                string path = "";

                SavePath = file + "\\unpack" + name + "\\";
                path = file + "\\" + name + ".tpsheet";



                TileObjects = new List<TileObject>();
                string[] lines = System.IO.File.ReadAllLines(path);

                Point pSize = new Point(myBitmap.Width, myBitmap.Height);

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] temp = lines[i].Split(';');
                    if (temp.Length == 7)
                    {
                        int w = int.Parse(temp[3]);
                        int h = int.Parse(temp[4]);
                        int x = int.Parse(temp[1]);
                        int y = pSize.Y - int.Parse(temp[2]) - h;

                        TileObject tileObj = new TileObject(openFileDialog1.FileName,SavePath,_filename: temp[0], _rect: new Rectangle(x, y, w, h));
                        TileObjects.Add(tileObj);
                    }
                }
            }
            else if (comboBox1.SelectedItem.ToString() == "font")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = "c:\\";
                openFileDialog1.Filter = "BMP File| *.png";
                openFileDialog1.Title = "取得影像檔";
                openFileDialog1.FilterIndex = 3;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    myBitmap = new Bitmap(openFileDialog1.FileName);
                }

                string name = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                string file = Path.GetDirectoryName(openFileDialog1.FileName);
                string path = "";

                SavePath = file + "\\unpack" + name + "\\";
                path = file + "\\" + name + ".fnt";
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlNode main = doc.SelectSingleNode("font/chars");//選擇節點
                if (main == null)
                    return;

                XmlElement element = (XmlElement)main;

                TileObjects = new List<TileObject>();
                for (int i = 0; i < element.ChildNodes.Count; i++)
                {
                    int x = Convert.ToInt32(element.ChildNodes[i].Attributes["x"].Value);
                    int y = Convert.ToInt32(element.ChildNodes[i].Attributes["y"].Value);
                    int width = Convert.ToInt32(element.ChildNodes[i].Attributes["width"].Value);
                    int height = Convert.ToInt32(element.ChildNodes[i].Attributes["height"].Value);

                    TileObject tileObj = new TileObject(openFileDialog1.FileName, SavePath,_filename: i.ToString(), _rect: new Rectangle(x, y, width, height));
                    TileObjects.Add(tileObj);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < TileObjects.Count; i++)
                {
                    if (!System.IO.Directory.Exists(TileObjects[i].SavePath))
                    {
                        System.IO.Directory.CreateDirectory(TileObjects[i].SavePath);                     
                    }
                    Bitmap TexturePackName = new Bitmap(TileObjects[i].TexturePackName);
                    Bitmap imageOutput = TexturePackName.Clone(TileObjects[i].rect,
                           System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    imageOutput.Save(TileObjects[i].SavePath + TileObjects[i].FileName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    imageOutput.Dispose();
                }
                MessageBox.Show("文件夹裁剪完毕");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
