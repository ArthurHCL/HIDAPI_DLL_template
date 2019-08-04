using System;
using System.Runtime.InteropServices;

namespace USERHIDAPI
{
    public static class UserHIDAPI
    {
        public const int USB_COMMUNICATION_TX_DATA_LENGTH_MAX = 1 + 64;
        public const int USB_COMMUNICATION_RX_DATA_LENGTH_MAX = 64;

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_init")]
        public static extern int HID_Init();

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_enumerate")]
        public static extern IntPtr HID_Enumerate(ushort vendor_id, ushort product_id);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_open")]
        public static extern IntPtr HID_Open(ushort vendor_id, ushort product_id, IntPtr serial_number);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_close")]
        public static extern void HID_Close(IntPtr device);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_free_enumeration")]
        public static extern void HID_Free_Enumeration(IntPtr devs);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_exit")]
        public static extern int HID_Exit();

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_write")]
        public static extern int HID_Write(IntPtr device, byte[] data, int length);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_read_timeout")]
        public static extern int HID_Read_Timeout(IntPtr dev, byte[] data, int length, int milliseconds);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_read")]
        public static extern int HID_Read(IntPtr device, byte[] data, int length);

        [DllImport("hidapi.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hid_set_nonblocking")]
        public static extern int HID_Set_Nonblocking(IntPtr device, int nonblock);
    }
}