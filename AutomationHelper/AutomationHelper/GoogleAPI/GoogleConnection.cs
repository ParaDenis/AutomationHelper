using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using AutomationHelper.Waiters;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace AutomationHelper.GoogleAPI
{
    public class GoogleConnection
    {
        #region SpreadsheetsService
        private ServiceAccountCredential GetAccountCredential(byte[] p12key, string password, string serviceAccountEmail)
        {
            var certificate = new X509Certificate2(p12key, password, X509KeyStorageFlags.Exportable);

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
        private GDataRequestFactory GetGDataRequestFactory(byte[] p12key, string password, string serviceAccountEmail)
        {
            var requestFactory = new GDataRequestFactory(null);
            requestFactory.CustomHeaders.Add("Authorization: Bearer " + GetAccountCredential(p12key, password, serviceAccountEmail).Token.AccessToken);
            return requestFactory;
        }
        /// <summary>
        /// private byte[] KEY = Properties.Resources.key;
        /// private string password = "notasecret";
        /// private string serviceAccountEmail = "atproautomation-1281@appspot.gserviceaccount.com";
        /// </summary>
        /// <param name="p12key"></param>
        /// <param name="password"></param>
        /// <param name="serviceAccountEmail"></param>
        /// <returns></returns>
        public SpreadsheetsService GetSpreadsheetsService(byte[] p12key, string password, string serviceAccountEmail)
        {
            return new SpreadsheetsService(null) { RequestFactory = GetGDataRequestFactory(p12key, password, serviceAccountEmail) };
        }
        #endregion


        #region DriveService
        //jsonKey (should be generate on google develop console):
        /*
         * e.g.
        {
          "type": "service_account",
          "project_id": "atproautomation-1281",
          "private_key_id": "be1881327174cda21dd9b3f8e738c4df6c7215e5",
          "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQC/yEyyob3nczMN\noDQUmfkMfphkgcn9KSR4j35+xnFknGd0D3sPO7cejKRs/Bk1cfNqjVIoc1Snv1Gx\nT0PmLsaRn7xc97X6dalgI7s3NOL8HVkHGV1QEN3Osl9wYo+EcjYMZ/jHZ8KBmsr1\nedAshxKmzNzyFvA+KP8J1/xFhBUH08OJrq+GJyZOJlJs7ReBNzBVm40Gn4a5hXD4\nfYQxvyQryeCXZCF2Me4fNJeiT+JkE1hQ25+OuJYaRLhkyNcBdiwm4erxajvWxdH5\nj8mN58Yj10Fhqoy1/0m6KHicWNbEeaTjXQtC4jEMMSxqRgd9DwWPJAcrokSpD2xe\ncvO5epdPAgMBAAECggEBAJkDtfYcSElfR4+Kj6MUSnnmk641Q7TCW7/5NULvD8/n\nXj9ijpvT2EH+kr4F0hMrTLxp7vApPJTud76RcKo6DeJoYUCHv2EK+c2kFJ5yitv+\nima4nRPhsSdWeAFSEhHpkigJ2Js4tR71IxQCUc8FIiOFdo+NL7dYvEvdUQEh5ims\nL7j3tmQz62vBEB7f1AgXqAZG1wiD9bUS33FtsK0cq+ApWQUsF2e2Ibx1z8wfxB/P\nb5Yi6k21zICcVu+5LNCBqGgnUGY4H/xFz4/rmP0+2NS+ulmddzx6Z2QesExyKtJ6\n+1y906WoQvUPUT2WHWcZ6vtvqPDVI73d6XyK3P2PtrkCgYEA88nAVo/jdGN2x5FE\ndn5BZgmT9KwbvDWF3vgOqk7h9p60tXQX5RvOXosF7xKNZYEEaLrd+xldlKPa0iNf\n2uI3uiWMrL+EUQDyhPuohHICSE785TJEyavWRRKtAj7VWAc32LGRsspEGkF6b1Co\npJT+hPhyGXBbYAqGdaCcAw3PGO0CgYEAyWOljxOPTb+3m0Twjzrr4WLf29djinj9\nVJOgaVYIXAsHHJCzRrf2A32QyPj22RGDMjN/iFLeVhhscA3lLjpYWsy6btPt3gGS\nKrybP1PlbDBR2SGCQu4HxB6GX4pDtd0Tg0vg6jV+4nkZLuyvBXVBOv8/fuWA3fm9\n5/BqxyvPlasCgYEA3QmXH6UUySmJv8aBuaId78NNaDFIcGxi9dgZi+c7z9zz4fXh\n2HROt7/7YFmE5HyHH+IWdJ36EFREifVS6uXjOx7inpqDAOMIAPUgRP4N/bvIZpMe\ndXAdNcGKJhgMXeUliI7vmJj2CIxH03fhgGArKSsOYOxTnQ/N/AbkjYxOsr0CgYEA\ngkHucqVvGhW31zj2811j9iryqgodexIYoNy/c4JO8+G6HtLE8ifIeqd7v1Gwr+Pw\n3MaMZYtb/YnckXmbU5QvU8N5jneLMf4IIZZOP96u1JRITE2tKEc3RLWNqjwO4ZF1\nroWkmzgwsqErFbvG1JvrrhIsapJdELYZC+zn051VrfMCgYEAn6rEG1RRwvnv59Y+\nK449Bj3+5LBr69vhh2UqTA5Z7bEbt31b50qnjQranrHaW4bIS90uS+mVIXwcGoZk\nkhWh0f57GHKiPFDhfcfkjHH9NqFa94z33wgJZdFbkxDUU2AQTLVOzGnk8MgFQd80\nmVJHhXtdzTUhkyxuQiarALUpIgo=\n-----END PRIVATE KEY-----\n",
          "client_email": "gcaccount@atproautomation-1281.iam.gserviceaccount.com",
          "client_id": "102929300149907606848",
          "auth_uri": "https://accounts.google.com/o/oauth2/auth",
          "token_uri": "https://accounts.google.com/o/oauth2/token",
          "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
          "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/gcaccount%40atproautomation-1281.iam.gserviceaccount.com"
        }
         */

        public DriveService GetDriveService(string applicationName, byte[] jsonKey)
        {
            GoogleCredential credential;

            credential = GoogleCredential.FromStream(new MemoryStream(jsonKey));
            credential = credential.CreateScoped(new[]
            {
                DriveService.Scope.Drive,
                "https://spreadsheets.google.com/feeds",
                "https://docs.google.com/feeds"
            });

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
            
        }
        #endregion
    }
}
