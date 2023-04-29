﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace ASAP_Project
{
    public class GoogleDrive
    {
        //Solution Explorerda bulunan credentials dosyaları ile adlarını değiştirin    
        static IDataStore tokenStorage = new FileDataStore("C:\\Users\\hayre\\Source\\Repos\\ceng-407-408-2022-2023-ASAP-Academic-and-Student-Assessment-Platform-\\ASAP_Project\\SendedAccountCredential\\", false);

        public static async void UploadFile()
        {
            
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.ShowDialog();

                           

                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { ClientId = "714044421228-cugq90i34shjhu5ifs9lmh06fop801ro.apps.googleusercontent.com", ClientSecret = "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" },
                    new[] { DriveService.Scope.Drive },
                    "user",
                    System.Threading.CancellationToken.None,
                    tokenStorage).Result;

                // Create the Drive service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ASAP Project"
                });

                //Upload the selected file to Google Drive.
                var fileMetadata = new Google.Apis.Drive.v3.Data.File();
                fileMetadata.Name = System.IO.Path.GetFileName(openFileDialog.FileName);
                var filePath = openFileDialog.FileName;
                using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                {
                    var uploadRequest = service.Files.Create(fileMetadata, stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    uploadRequest.Upload();
                }
               
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading file to Google Drive: {ex.Message}");
            }
        }

        public static async void GetFile()
        {


            try
            {

                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { ClientId = "714044421228-cugq90i34shjhu5ifs9lmh06fop801ro.apps.googleusercontent.com", ClientSecret = "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" },
                    new[] { DriveService.Scope.Drive },
                    "user",
                    System.Threading.CancellationToken.None,
                    tokenStorage).Result;

                // Create the Drive service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "ASAP Project"
                });

                var request = service.Files.List();
                request.Q = "name='" + "Lesson1.xlsx" + "' and trashed = false";
                request.Fields = "nextPageToken, files(id)";
                var results = request.Execute().Files;

                if (results == null || results.Count == 0)
                {
                    MessageBox.Show("No files found.");
                }

                var file = service.Files.Get(results[0].Id).Execute();

                var downloadfile = service.Files.Get(results[0].Id);
                var stream = new MemoryStream();
                downloadfile.Download(stream);

                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ASAP");
                using (var fileStream = new FileStream(appDataPath, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(fileStream);
                }
            }


            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file to Google Drive: {ex.Message}");
            }
        }
    }
}
