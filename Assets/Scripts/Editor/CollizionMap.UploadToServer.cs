#if UNITY_EDITOR
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentFTP;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public partial class CollisionMap
    {
        private const string FTP_SERVER = "apisfs.ru";
        private const string FTP_USER = "daniil";
        private const string FTP_USERPASSWORD = "giftrola05";
        private const string FTP_PATH = "/home/daniil/collisions.json";

        private static async void UploadToServer(CollisionMapPayload payload)
        {
            var json = JsonUtility.ToJson(payload);

            var path = EditorUtility.SaveFilePanel("Save Collision Map", "", "collisions.json", "json");

            if (string.IsNullOrEmpty(path))
                return;

            await File.WriteAllTextAsync(path, json);
            
            await UploadJsonToServer(path);

            File.Delete(path);

            EditorUtility.DisplayDialog("Collision Map", "Collision map uploaded successfully", "OK");
        }

        private static async Task UploadJsonToServer(string path)
        {
            var client = new AsyncFtpClient(FTP_SERVER, new NetworkCredential(FTP_USER, FTP_USERPASSWORD));

            client.Config.EncryptionMode = FtpEncryptionMode.None;
            client.Config.DataConnectionType = FtpDataConnectionType.PASV;
            client.Config.DataConnectionEncryption = false;
            client.Config.SslProtocols = System.Security.Authentication.SslProtocols.None;
            client.Config.ValidateAnyCertificate = true;

            client.Config.LogToConsole = true;
            client.Config.LogHost = true;
            client.Config.LogUserName = true;
            client.Config.LogPassword = false;

            await client.Connect();
            await client.UploadFile(path, FTP_PATH);
            await client.Disconnect();
        }
    }
}
#endif
