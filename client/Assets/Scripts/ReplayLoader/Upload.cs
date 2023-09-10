using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// User should upload the json replay file
/// </summary>
public class Upload
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenFileName
    {
        public int StructSize = 0;
        public IntPtr DlgOwner = IntPtr.Zero;
        public IntPtr Instance = IntPtr.Zero;
        public String Filter = null;
        public String CustomFilter = null;
        public int MaxCustFilter = 0;
        public int FilterIndex = 0;
        public String File = null;
        public int MaxFile = 0;
        public String FileTitle = null;
        public int MaxFileTitle = 0;
        public String InitialDir = null;
        public String Title = null;
        public int Flags = 0;
        public short FileOffset = 0;
        public short FileExtension = 0;
        public String DefExt = null;
        public IntPtr CustData = IntPtr.Zero;
        public IntPtr Hook = IntPtr.Zero;
        public String TemplateName = null;
        public IntPtr ReservedPtr = IntPtr.Zero;
        public int ReservedInt = 0;
        public int FlagsEx = 0;
    }

    /// <summary>
    /// Open a json file 
    /// </summary>
    /// <returns>
    /// OpenFileName: The file to be opened
    /// </returns>
    public OpenFileName UploadNcLevel()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.StructSize = Marshal.SizeOf(openFileName);
        openFileName.Filter = "nclevel文件(*.nclevel)\0*.nclevel";
        openFileName.File = new string(new char[256]);
        openFileName.MaxFile = openFileName.File.Length;
        openFileName.FileTitle = new string(new char[64]);
        openFileName.MaxFileTitle = openFileName.FileTitle.Length;
        openFileName.InitialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.Title = "选择回放nclevel文件";
        openFileName.Flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetOpenFileName(openFileName))
        {
            Debug.Log(openFileName.File);
            Debug.Log(openFileName.FileTitle);
            return openFileName;
        }
        return null;
    }

    private class LocalDialog
    {
        //链接指定系统函数       打开文件对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
        public static bool GetOFN([In, Out] OpenFileName ofn)
        {
            return GetOpenFileName(ofn);
        }

        //链接指定系统函数        另存为对话框
        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
        public static bool GetSFN([In, Out] OpenFileName ofn)
        {
            return GetSaveFileName(ofn);
        }
    }
}

