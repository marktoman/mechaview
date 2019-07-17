using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using IO = System.IO;
using VM = Mecha.ViewModel.Elements;
using Mecha.ViewModel;
using Mecha.ViewModel.Attributes;
using Forms = System.Windows.Forms;

namespace Mecha.View.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for PathControl.xaml
    /// </summary>
    public partial class PathInput : UserControl, ILabeled
    {
        bool isSetup;

        VM.PathInput vm;
        internal VM.PathInput ViewModel
        {
            get { return vm; }
            set
            {
                if (isSetup)
                    throw new InvalidOperationException("ViewModel can be set only once.");
                isSetup = true;

                vm = value;

                vm.OnValidation += Validate;
                DataContext = vm;
                label.DataContext = vm;
                txtBox.DataContext = vm;

                var binding = new Binding(vm.Path)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnExceptions = true,
                };
                binding.ValidationRules.Add(new MandatoryInputRule(vm));
                txtBox.SetBinding(TextBox.TextProperty, binding);
            }
        }

        public double LabelWidth
        {
            get { return label.ActualWidth; }
            set { label.Width = value; }
        }

        public PathInput()
        {
            InitializeComponent();

            browseBtn.Click += BrowseBtn_Click;
        }

        void Validate(object vm, VM.ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(txtBox.Text))
                txtBox.Text = null;
            
            result.HasError = txtBox.GetBindingExpression(TextBox.TextProperty).HasError;
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Search display words for a file extension via this.GetFilter()
            
            string initDir = null;
            if (!string.IsNullOrWhiteSpace(txtBox.Text))
            {
                try { initDir = IO.Path.GetDirectoryName(txtBox.Text); }
                catch { }
            }            

            var attrib = ViewModel.PathAttribute;
            var pathType = ViewModel.PathType;

            if (pathType == PathType.Folder)
            {
                var dlg = new Forms.FolderBrowserDialog
                {
                    RootFolder = Environment.SpecialFolder.Desktop,
                    ShowNewFolderButton = true,                    
                };

                if (dlg.ShowDialog() == Forms.DialogResult.OK)
                    ViewModel.Value = dlg.SelectedPath;
            }
            else
            {
                var filter = attrib?.Ext?.Length > 0
                ? GetFilter(attrib.Ext)
                : null;

                var all = attrib == null || attrib.All || !(attrib?.Ext?.Length > 0);

                if (all)
                {
                    filter = filter != null
                        ? $"{filter}|All Files|*.*"
                        : "All Files|*.*";
                }

                var dlg = pathType == PathType.Save
                    ? (FileDialog) new SaveFileDialog
                    {
                        InitialDirectory = initDir,
                        Filter = filter,
                        FileName = attrib?.FileName,
                    }
                    : (FileDialog) new OpenFileDialog
                    {
                        InitialDirectory = initDir,
                        Filter = filter,
                        FileName = attrib?.FileName,
                    };

                if (dlg.ShowDialog() == true)
                    ViewModel.Value = dlg.FileName;
            }
        }

        static string GetFilter(params string[] ext)
        {
            if (ext == null || ext.Length == 0)
                throw new ArgumentNullException(nameof(ext));

            return string.Join("|", ext
                    .Select(e => e.TrimStart('.'))
                    .Select(e => string.Format("{0} (*.{1})|*.{1}", GetFileTypeDescription(e), e)));
        }

        public static string GetFileTypeDescription(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentNullException(nameof(fileExtension));

            fileExtension = "." + fileExtension.TrimStart('.');

            try
            {
                SHFILEINFO shfi;
                var res = SHGetFileInfo("." + fileExtension,
                    (uint)FileInfoParam.FILE_ATTRIBUTE_NORMAL,
                    out shfi,
                    (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                    (uint)FileInfoParam.SHGFI_USEFILEATTRIBUTES | (uint)FileInfoParam.SHGFI_TYPENAME);

                return res != IntPtr.Zero
                    ? shfi.szTypeName
                    : fileExtension.TrimStart('.').ToUpper();
            }
            catch
            {
                return fileExtension.TrimStart('.').ToUpper();
            }
        }

        [DllImport("shell32")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        enum FileInfoParam : uint
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,
            FILE_ATTRIBUTE_VIRTUAL = 0x00010000,

            SHGFI_ICON = 0x000000100,     // get icon
            SHGFI_DISPLAYNAME = 0x000000200,     // get display name
            SHGFI_TYPENAME = 0x000000400,     // get type name
            SHGFI_ATTRIBUTES = 0x000000800,     // get attributes
            SHGFI_ICONLOCATION = 0x000001000,     // get icon location
            SHGFI_EXETYPE = 0x000002000,     // return exe type
            SHGFI_SYSICONINDEX = 0x000004000,     // get system icon index
            SHGFI_LINKOVERLAY = 0x000008000,     // put a link overlay on icon
            SHGFI_SELECTED = 0x000010000,     // show icon in selected state
            SHGFI_ATTR_SPECIFIED = 0x000020000,     // get only specified attributes
            SHGFI_LARGEICON = 0x000000000,     // get large icon
            SHGFI_SMALLICON = 0x000000001,     // get small icon
            SHGFI_OPENICON = 0x000000002,     // get open icon
            SHGFI_SHELLICONSIZE = 0x000000004,     // get shell size icon
            SHGFI_PIDL = 0x000000008,     // pszPath is a pidl
            SHGFI_USEFILEATTRIBUTES = 0x000000010,     // use passed dwFileAttribute
        }
    }
}
