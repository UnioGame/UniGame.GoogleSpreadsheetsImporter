using System;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    [Serializable]
    public class GetRequestData<TRequest,TValue>
    {
        public TRequest Request;
        public Action<TValue> Result;
        public TValue Value;
        public bool isReady;
    }
}