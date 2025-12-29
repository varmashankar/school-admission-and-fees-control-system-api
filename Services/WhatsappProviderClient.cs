using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SchoolErpAPI.Services
{
    public class WhatsappSendResult
    {
        public bool ok { get; set; }
        public string providerMessageId { get; set; }
        public string error { get; set; }
        public HttpStatusCode? statusCode { get; set; }
    }

    internal class WhatsappProviderClient
    {
        private readonly string _baseUrl;
        private readonly string _token;

        public WhatsappProviderClient(string baseUrl, string token)
        {
            _baseUrl = baseUrl;
            _token = token;
        }

        public WhatsappSendResult SendText(string phone, string message)
        {
            if (string.IsNullOrWhiteSpace(_baseUrl))
                return new WhatsappSendResult { ok = false, error = "ProviderBaseUrl is not configured." };

            if (string.IsNullOrWhiteSpace(phone))
                return new WhatsappSendResult { ok = false, error = "Phone is empty." };

            if (string.IsNullOrWhiteSpace(message))
                return new WhatsappSendResult { ok = false, error = "Message is empty." };

            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    if (!string.IsNullOrWhiteSpace(_token))
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);

                    var payload = new
                    {
                        phone = phone,
                        message = message
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var url = _baseUrl.TrimEnd('/');
                    var resp = client.PostAsync(url, content).GetAwaiter().GetResult();
                    var body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!resp.IsSuccessStatusCode)
                    {
                        return new WhatsappSendResult
                        {
                            ok = false,
                            statusCode = resp.StatusCode,
                            error = string.IsNullOrWhiteSpace(body) ? ("Provider HTTP " + ((int)resp.StatusCode).ToString()) : body
                        };
                    }

                    string providerMsgId = null;
                    try
                    {
                        dynamic obj = JsonConvert.DeserializeObject(body);
                        if (obj != null)
                        {
                            if (obj.messageId != null) providerMsgId = Convert.ToString(obj.messageId);
                            else if (obj.id != null) providerMsgId = Convert.ToString(obj.id);
                            else if (obj.data != null && obj.data.messageId != null) providerMsgId = Convert.ToString(obj.data.messageId);
                        }
                    }
                    catch { }

                    return new WhatsappSendResult
                    {
                        ok = true,
                        statusCode = resp.StatusCode,
                        providerMessageId = providerMsgId
                    };
                }
            }
            catch (Exception ex)
            {
                return new WhatsappSendResult { ok = false, error = ex.Message };
            }
        }
    }
}
