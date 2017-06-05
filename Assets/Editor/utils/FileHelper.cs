using foundation;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class FileHelper
{
    public static byte[] getBytes(string fullPath)
    {
        FileStream fs = File.OpenRead(fullPath);

        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    public static void autoCreateDirectory(string fullPath)
    {
        FileInfo fi = new FileInfo(fullPath);
        DirectoryInfo di = fi.Directory;
        if (di.Exists == false)
        {
            Directory.CreateDirectory(di.FullName);
        }
    }

    public static object getAMF(string fullPath)
    {
        if (File.Exists(fullPath) == false)
        {
            return null;
        }

        byte[] bytes;

        using (FileStream fileStream = File.OpenRead(fullPath))
        {
            bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);

        }

        if (bytes != null)
        {
            ByteArray bytesArray = new ByteArray(bytes);
            try
            {
                bytesArray.Inflate();
            }
            catch (Exception)
            {
                bytesArray.Position = 0;
            }

            return bytesArray.ReadObject();
        }

        return null;
    }

    public static void saveAMF(object vo, string fullPath)
    {
        ByteArray bytesArray = new ByteArray();
        bytesArray.WriteObject(vo);
        bytesArray.Deflate();

        byte[] bytes = bytesArray.ToArray();

        autoCreateDirectory(fullPath);
        using (FileStream fileStream = File.Open(fullPath, FileMode.OpenOrCreate))
        {
            fileStream.Write(bytes, 0, bytes.Length);
        }
    }

    /// <summary>
    /// 递归查找指定目录下指定后缀名的所有文件（含所有子级目录）
    /// </summary>
    /// <param name="dirPath">根目录</param>
    /// <param name="exNameArr">要查找的后缀名,写法如 new string[] { "*.prefab"} 或 new string[] { "*.*"}</param>
    /// <param name="list">查找到的所有文件的完整路径</param>
    public static List<string> FindFile(string dirPath, List<string> exNameArr)
    {
        return FindFile(dirPath, exNameArr.ToArray());
    }

    public static List<string> FindFile(string dirPath, string[] exNameArr)
    {
        List<string> list = new List<string>();
        DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

        FileInfo[] fileInfoArr = new FileInfo[] { };

        for (int i = 0; i < exNameArr.Length; i++)
        {
            FileInfo[] fileInfoArr2 = dirInfo.GetFiles(exNameArr[i], SearchOption.AllDirectories);

            foreach (FileInfo filePath in fileInfoArr2)
            {
                string fileFullPath = filePath.FullName.Replace("\\", "/");
                list.Add(fileFullPath);
            }
        }

        return list;
    }

    public static DirectoryInfo[] FinDirectory(string dirPath)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
        if (dirInfo.Exists == false)
        {
            Debug.Log(dirPath + ":不存在");
            return new DirectoryInfo[0];
        }
        return dirInfo.GetDirectories();
    }

    public static void copy(string sourceFileName, string destFileName, bool overrideIt = true)
    {
        if (overrideIt == false)
        {
            if (File.Exists(destFileName) == true)
            {
                return;
            }
        }
        autoCreateDirectory(destFileName);
        File.Copy(sourceFileName, destFileName, overrideIt);
    }

    public static string getDirectory(string path, bool isFull = true)
    {

        FileInfo fileInfo = new FileInfo(path);

        if (isFull)
        {
            return fileInfo.Directory.FullName;
        }

        return fileInfo.DirectoryName;
    }

    public static void saveBytes(byte[] bytes, string itemPath)
    {
        autoCreateDirectory(itemPath);
        using (FileStream fileStream = File.Open(itemPath, FileMode.OpenOrCreate))
        {
            fileStream.Write(bytes, 0, bytes.Length);
        }
    }
}
