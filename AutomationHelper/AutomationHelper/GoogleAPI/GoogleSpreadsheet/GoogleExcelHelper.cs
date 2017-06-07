﻿/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHelper.GoogleAPI.GoogleSpreadsheet
{
    public class GoogleExcelHelper
    {
        public ServiceAccountCredential GetAccountCredential(byte[] key, string password, string serviceAccountEmail)
        {
            var certificate = new X509Certificate2(key, password, X509KeyStorageFlags.Exportable);

            var serviceAccountCredentialInitializer =
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = new[]
                    {
                        "https://spreadsheets.google.com/feeds", 
                        DriveService.Scope.Drive
                    }
                }.FromCertificate(certificate);

            var credential = new ServiceAccountCredential(serviceAccountCredentialInitializer);

            Wait.UntilNoException(() =>
            {
                if (!credential.RequestAccessTokenAsync(System.Threading.CancellationToken.None).Result)
                    throw new InvalidOperationException("Access token request failed.");
            }, 60 * 1000);
            return credential;
        }
    }
}*/