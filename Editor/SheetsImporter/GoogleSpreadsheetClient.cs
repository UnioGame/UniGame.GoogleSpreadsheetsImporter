namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.Sheets.v4;
    using Google.Apis.Util.Store;
    using GoogleSpreadsheets.Editor.SheetsImporter;
    using UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UnityEngine;

    public class GoogleSpreadsheetClient : IGoogleSpreadsheetClient
    {
        private readonly string                            _appName;
        private readonly string[]                          _scope;
        
        private          List<GoogleSpreadsheetConnection> _connections = new List<GoogleSpreadsheetConnection>();
        private          LifeTimeDefinition                _lifeTime    = new LifeTimeDefinition();
        
        private string                  _credentialsPath;
        private string                  _user;
        private SheetsService           _sheetService;
        private GoogleSpreadsheetClientStatus _clientStatus;
        private SpreadsheetData         _spreadsheetData;

        public GoogleSpreadsheetClient(string user, string credentialsPath,string appName, string[] scope)
        {
            _user            = user;
            _credentialsPath = credentialsPath;
            _appName         = appName;
            _scope           = scope;
            _spreadsheetData = new SpreadsheetData(GetSheets());
            _clientStatus    = new GoogleSpreadsheetClientStatus(GetSheets());
        }

        public bool IsConnectionRefused { get; protected set; }

        public bool IsConnected => !IsConnectionRefused;

        public ISpreadsheetData SpreadsheetData => _spreadsheetData;

        public IGooglsSpreadsheetClientStatus Status => _clientStatus;

        public bool HasSheet(string id) => _spreadsheetData.HasSheet(id);
        
        public void Dispose() => _lifeTime.Terminate();

        public void Disconnect() => _lifeTime.Release();
        
        public SheetsService SheetsService {
            get {
                if (_sheetService != null)
                    return _sheetService;

                _sheetService = LoadSheetService(GoogleSheetImporterConstants.ApplicationName, GoogleSpreadsheetConnection.WriteScope);
                _lifeTime.AddDispose(_sheetService);
                _lifeTime.AddCleanUpAction(() => _sheetService = null);
                return _sheetService;
            }
        }
        
        public IEnumerable<SheetData> GetSheets()
        {
            return _connections.SelectMany(connection => connection.Sheets);
        }

        public bool Upload(string sheetId)
        {
            var connection = _connections.FirstOrDefault(x => x.HasSheet(sheetId));
            if (connection == null)
                return false;
            connection.Upload(sheetId);
            return true;
        }

        public bool Upload(SheetData sheet)
        {
            var id         = sheet.Name;
            var connection = _connections.FirstOrDefault(x => x.HasSheet(id));
            if (connection == null)
                return false;
            connection.UpdateData(sheet);
            return true;
        }
        
        public bool Connect()
        {
            Disconnect();
            
            _lifeTime.AddCleanUpAction(() => _connections.Clear());

            _sheetService    = LoadSheetService(_appName, _scope);

            _lifeTime.AddCleanUpAction(() => IsConnectionRefused = true);
            
            return IsConnected;
        }
        
        public bool Connect(string user,string credentialsPath)
        {
            _user            = user;
            _credentialsPath = credentialsPath;

            return Connect();
        }

        public void ReloadAll() => _connections.ForEach(x => x.Reload());

        public void Reload(string sheetId) => _connections.FirstOrDefault(x => x.Id == sheetId)?.Reload();

        public bool ConnectToSpreadsheet(string spreadsheetId)
        {
            if (_connections.Any(x => x.Id == spreadsheetId))
                return true;
            try {
                var client = new GoogleSpreadsheetConnection(SheetsService, spreadsheetId);
                client.Reload();
                _connections.Add(client);
            }
            catch (Exception e) {
                Debug.LogException(e);
                return false;
            }

            return true;
        }


        /// <summary>
        /// create sheet service
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="scope">target and permissions scope. User GoogleSheetClient.*[Scope] constants</param>
        /// <returns></returns>
        private SheetsService LoadSheetService(string applicationName, string[] scope)
        {
            UserCredential credential;

            using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read)) {
                var cancelationSource = new CancellationTokenSource();
                cancelationSource.CancelAfter(TimeSpan.FromSeconds(30f));
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scope,
                    _user,
                    cancelationSource.Token,
                    new FileDataStore(GoogleSheetImporterConstants.TokenKey, true)).Result;
                Debug.Log("Credential file saved to: " + GoogleSheetImporterConstants.TokenKey);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(
                new BaseClientService.Initializer() {
                    HttpClientInitializer = credential,
                    ApplicationName       = applicationName,
                }).AddTo(_lifeTime);

            IsConnectionRefused = false;

            _lifeTime.AddCleanUpAction(() => IsConnectionRefused = true);

            return service;
        }
    }
}