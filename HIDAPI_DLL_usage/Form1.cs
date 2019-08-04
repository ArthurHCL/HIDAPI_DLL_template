using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using USERHIDAPI;

namespace HIDAPI_DLL_usage
{
    public partial class Form1 : Form
    {
        private const ushort VIDtest = 0x3412;
        private const ushort PIDtest = 0x7856;

        private const int USB_COMMUNICATION_TIMEOUT_IN_MILLISECONDS = 1000;

        private byte[] HID_write_buffer = new byte[UserHIDAPI.USB_COMMUNICATION_TX_DATA_LENGTH_MAX];
        private byte[] HID_read_buffer = new byte[UserHIDAPI.USB_COMMUNICATION_RX_DATA_LENGTH_MAX];

        private volatile bool is_USB_device_connected = false;
        private volatile IntPtr target_hid_device = IntPtr.Zero;

        public Form1()
        {
            InitializeComponent();

            Thread USB_Background_Thread = new Thread(new ThreadStart(USB_Background_Receive));
            USB_Background_Thread.Start();
        }

        private delegate void RichTextBox_Show_Func(string message);
        private void RichTextBox_Show(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                RichTextBox_Show_Func richTextBox_Show_Func = new RichTextBox_Show_Func(RichTextBox_Show);
                richTextBox1.Invoke(richTextBox_Show_Func, message);
            }
            else
            {
                richTextBox1.Text = message;
            }
        }

        private void USB_Background_Receive()
        {
            int ret = 0;
            bool is_thread_close_needed = false;

        retry:
            if (0 != UserHIDAPI.HID_Init())
            {
                ret = -1;

                goto exit_0;
            }

            IntPtr target_hid_device_info = UserHIDAPI.HID_Enumerate(VIDtest, PIDtest);
            if (IntPtr.Zero == target_hid_device_info)
            {
                ret = -2;

                goto exit_1;
            }

            target_hid_device = UserHIDAPI.HID_Open(VIDtest, PIDtest, IntPtr.Zero);
            if (IntPtr.Zero == target_hid_device)
            {
                ret = -3;

                goto exit_2;
            }

            is_USB_device_connected = true;

            while (true)
            {
                int read_hid_data_amount = UserHIDAPI.HID_Read_Timeout(target_hid_device, HID_read_buffer, HID_read_buffer.Length, USB_COMMUNICATION_TIMEOUT_IN_MILLISECONDS);
                if (0 > read_hid_data_amount)
                {
                    ret = -5;

                    goto exit_3;
                }
                else if (0 == read_hid_data_amount)
                {
                    continue;
                }

                string read_hid_data = String.Empty;
                for (int i = 0; i < HID_read_buffer.Length; i++)
                {
                    read_hid_data += HID_read_buffer[i].ToString("X2") + " ";
                    if (15 == (i % 16))
                    {
                        read_hid_data += "\n";
                    }
                }

                try
                {
                    RichTextBox_Show(read_hid_data);
                }
                catch
                {
                    is_thread_close_needed = true;

                    goto exit_3;
                }
            }

        exit_3:
            UserHIDAPI.HID_Close(target_hid_device);

        exit_2:
            UserHIDAPI.HID_Free_Enumeration(target_hid_device_info);

        exit_1:
            if (0 != UserHIDAPI.HID_Exit())
            {
                ret = -4;
            }

        exit_0:
            string stringresult = "ret = " + ret + ": ";
            switch (ret)
            {
                case -1:
                    stringresult += "fail HID_Init()";
                    break;
                case -2:
                    stringresult += "fail HID_Enumerate()";
                    break;
                case -3:
                    stringresult += "fail HID_Open()";
                    break;
                case -4:
                    stringresult += "fail HID_Exit()";
                    break;
                case -5:
                    stringresult += "fail HID_Read_Timeout()";
                    break;
                default:
                    stringresult += "fail unknown";
                    break;
            }
            stringresult += "\n";

            is_USB_device_connected = false;

            if (is_thread_close_needed)
            {
                return;
            }

            try
            {
                RichTextBox_Show(stringresult);
            }
            catch
            {
                return;
            }

            goto retry;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (is_USB_device_connected)
            {
                HID_write_buffer[0] = 0x00; /* must equal to 0x00. */

                HID_write_buffer[1] = 0x55;
                HID_write_buffer[2] = 0xAA;
                UserHIDAPI.HID_Write(target_hid_device, HID_write_buffer, HID_write_buffer.Length);
            }
            else
            {
                MessageBox.Show("USB device is not connected!");
            }
        }
    }
}
