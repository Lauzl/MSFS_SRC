using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KeyCounterApp
{
    public partial class Form1 : Form
    {
        private int counter = 0;
        private GlobalKeyboardHook globalKeyboardHook;
        private Keys upKey = Keys.Up;
        private Keys downKey = Keys.Down;
        private Keys resetKey = Keys.R;
        private MenuStrip menuStrip;
        private OverlayForm overlay;

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
            SetupGlobalHook();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "SimRate Controller";
            this.Size = new Size(300, 200);
            this.MinimumSize = new Size(300, 200);

            menuStrip = new MenuStrip();
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("Settings");
            ToolStripMenuItem upKeyItem = new ToolStripMenuItem("Simrate UP Shortcut");
            ToolStripMenuItem downKeyItem = new ToolStripMenuItem("Simrate DOWN Shortcut");
            ToolStripMenuItem resetKeyItem = new ToolStripMenuItem("RESET Shortcut");

            upKeyItem.Click += (s, e) => ConfigureKey("UP", k => upKey = k);
            downKeyItem.Click += (s, e) => ConfigureKey("DOWN", k => downKey = k);
            resetKeyItem.Click += (s, e) => ConfigureKey("RESET", k => resetKey = k);

            settingsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                upKeyItem, downKeyItem, resetKeyItem
            });
            menuStrip.Items.Add(settingsMenu);
            this.Controls.Add(menuStrip);

            overlay = new OverlayForm();
            overlay.Show();
            overlay.UpdateValue(counter);
        }

        private void ConfigureKey(string keyName, Action<Keys> setKey)
        {
            using (var form = new KeyConfigForm($"{keyName}-Simrate Shortcut"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    setKey(form.SelectedKey);
                }
            }
        }

        private void SetupGlobalHook()
        {
            globalKeyboardHook = new GlobalKeyboardHook();
            globalKeyboardHook.KeyDown += GlobalKeyboardHook_KeyDown;
        }

        private void GlobalKeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == upKey)
            {
                counter++;
                overlay.UpdateValue(counter);
            }
            else if (e.KeyCode == downKey)
            {
                counter--;
                overlay.UpdateValue(counter);
            }
            else if (e.KeyCode == resetKey)
            {
                counter = 0;
                overlay.UpdateValue(counter);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (overlay != null && !overlay.IsDisposed)
            {
                overlay.Close();
            }
        }
    }

    public class OverlayForm : Form
    {
        private Label titleLabel;
        private Label valueLabel;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public OverlayForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(100, 60);
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width - 10, 10);
            this.BackColor = Color.Black;
            this.Opacity = 0.7;

            titleLabel = new Label
            {
                Text = "SimRate",
                ForeColor = Color.White,
                Font = new Font("Arial", 12f, FontStyle.Bold),
                AutoSize = false,
                Width = this.Width,
                Height = 25,
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            valueLabel = new Label
            {
                Text = "0",
                ForeColor = Color.White,
                Font = new Font("Arial", 12f, FontStyle.Bold),
                AutoSize = false,
                Width = this.Width,
                Height = 25,
                Location = new Point(0, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.AddRange(new Control[] { titleLabel, valueLabel });

            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);
        }

        public void UpdateValue(int value)
        {
            if (valueLabel.InvokeRequired)
            {
                valueLabel.Invoke(new Action(() => valueLabel.Text = value.ToString()));
            }
            else
            {
                valueLabel.Text = value.ToString();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80000;
                return cp;
            }
        }
    }

    public class KeyConfigForm : Form
    {
        public Keys SelectedKey { get; private set; }
        private Label keyLabel;
        private bool isListening = false;

        public KeyConfigForm(string title = "Button")
        {
            this.Text = title;
            this.Size = new Size(250, 120);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            keyLabel = new Label { Text = "Save Shortcut" };
            Button listenButton = new Button { Text = "Bind" };
            listenButton.Click += (s, e) =>
            {
                isListening = true;
                keyLabel.Text = "Push your Button";
            };

            Button okButton = new Button { Text = "OK", DialogResult = DialogResult.OK };
            okButton.Click += (s, e) => this.Close();

            layout.Controls.Add(keyLabel, 0, 0);
            layout.Controls.Add(listenButton, 1, 0);
            layout.Controls.Add(okButton, 1, 1);

            this.Controls.Add(layout);
            this.KeyPreview = true;
            this.KeyDown += KeyConfigForm_KeyDown;
        }

        private void KeyConfigForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (isListening)
            {
                SelectedKey = e.KeyCode;
                keyLabel.Text = $"Ausgewählte Taste: {e.KeyCode}";
                isListening = false;
                e.Handled = true;
            }
        }
    }

    public class GlobalKeyboardHook
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookDelegate callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private delegate IntPtr KeyboardHookDelegate(int nCode, IntPtr wParam, IntPtr lParam);
        private KeyboardHookDelegate hook;
        private IntPtr hookHandle = IntPtr.Zero;

        public event KeyEventHandler KeyDown;

        public GlobalKeyboardHook()
        {
            hook = new KeyboardHookDelegate(HookCallback);
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, hook, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                KeyEventArgs e = new KeyEventArgs((Keys)vkCode);
                KeyDown?.Invoke(this, e);

                if (e.Handled)
                    return (IntPtr)1;
            }

            return CallNextHookEx(hookHandle, nCode, (int)wParam, lParam);
        }

        ~GlobalKeyboardHook()
        {
            if (hookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookHandle);
            }
        }
    }
}