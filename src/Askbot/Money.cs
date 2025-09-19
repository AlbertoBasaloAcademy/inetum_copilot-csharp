using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AskBot; // Fix for IpApi reference

namespace Askbot
{
        public class Money
        {
            public static async Task<string> GetMoneyReportAsync(IpApi ip)
            {
                string country = ip?.Country ?? "Unknown";
                string countryCode = ip?.CountryCode ?? "";
                CurrencyInfo currency = null;
                if (!string.IsNullOrEmpty(countryCode))
                    currency = GetCurrencyInfo(countryCode);
                if (currency == null && !string.IsNullOrEmpty(countryCode))
                    currency = await GetCurrencyInfoFallbackAsync(countryCode);

                string currencyCode = currency?.Code ?? "EUR";
                var rates = await FetchExchangeRatesAsync(currencyCode);
                return FormatRatesOutput(country, countryCode, currency, rates);
            }

            private static readonly string[] ReferenceCurrencies = new[] { "EUR", "USD", "GBP", "CHF" };

            public class ExchangeRatesResult
            {
                public string BaseCurrency { get; set; }
                public Dictionary<string, decimal> Rates { get; set; } = new();
                public string Date { get; set; }
                public string Error { get; set; }
            }

            public static async Task<ExchangeRatesResult> FetchExchangeRatesAsync(string baseCurrency)
            {
                var result = new ExchangeRatesResult { BaseCurrency = baseCurrency };
                try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                    var symbols = string.Join(",", ReferenceCurrencies);
                    var url = $"https://api.frankfurter.app/latest?base={baseCurrency}&symbols={symbols}";
                    var resp = await client.GetAsync(url);
                    if (!resp.IsSuccessStatusCode)
                    {
                        result.Error = $"Exchange rate API error: {resp.StatusCode}";
                        return result;
                    }
                    var json = await resp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    result.Date = root.GetProperty("date").GetString();
                    var rates = root.GetProperty("rates");
                    foreach (var refCur in ReferenceCurrencies)
                    {
                        if (refCur == baseCurrency)
                        {
                            result.Rates[refCur] = 1.00m;
                            continue;
                        }
                        if (rates.TryGetProperty(refCur, out var val))
                            result.Rates[refCur] = val.GetDecimal();
                    }
                }
                catch (Exception ex)
                {
                    result.Error = $"Exchange rate fetch failed: {ex.Message}";
                }
                return result;
            }

            public static string FormatRatesOutput(string country, string countryCode, CurrencyInfo currency, ExchangeRatesResult rates)
            {
                var lines = new List<string>();
                lines.Add($"# {country} ({countryCode})");
                if (currency != null)
                    lines.Add($"Currency: {currency.Name} ({currency.Code}) {currency.Symbol}");
                else
                    lines.Add($"Currency: Unknown");

                if (!string.IsNullOrEmpty(rates.Error))
                {
                    lines.Add($"> {rates.Error}");
                    return string.Join("\n", lines);
                }

                lines.Add($"Exchange rates (as of {rates.Date}):");
                // Format: 1 <CUR> = X <REF> (symbol)  (aligned)
                int colWidth = 8;
                foreach (var refCur in ReferenceCurrencies)
                {
                    if (!rates.Rates.TryGetValue(refCur, out var value))
                        continue;
                    string refSymbol = CountryCurrencyMap.Values.FirstOrDefault(c => c.Code == refCur)?.Symbol ?? refCur;
                    string valStr = value.ToString("0.####");
                    lines.Add($"  1 {currency?.Code ?? rates.BaseCurrency,-3} = {valStr,8} {refCur} {refSymbol}");
                }
                return string.Join("\n", lines);
            }

            public class CurrencyInfo
            {
                public string Code { get; set; }
                public string Name { get; set; }
                public string Symbol { get; set; }
            }

            // Minimal ISO country code to currency info map
            private static readonly Dictionary<string, CurrencyInfo> CountryCurrencyMap = new()
            {
                { "US", new CurrencyInfo { Code = "USD", Name = "US Dollar", Symbol = "$" } },
                { "GB", new CurrencyInfo { Code = "GBP", Name = "Pound Sterling", Symbol = "£" } },
                { "DE", new CurrencyInfo { Code = "EUR", Name = "Euro", Symbol = "€" } },
                { "FR", new CurrencyInfo { Code = "EUR", Name = "Euro", Symbol = "€" } },
                { "ES", new CurrencyInfo { Code = "EUR", Name = "Euro", Symbol = "€" } },
                { "IT", new CurrencyInfo { Code = "EUR", Name = "Euro", Symbol = "€" } },
                { "CH", new CurrencyInfo { Code = "CHF", Name = "Swiss Franc", Symbol = "Fr" } },
                { "JP", new CurrencyInfo { Code = "JPY", Name = "Yen", Symbol = "¥" } },
                { "CN", new CurrencyInfo { Code = "CNY", Name = "Yuan Renminbi", Symbol = "¥" } },
                { "IN", new CurrencyInfo { Code = "INR", Name = "Indian Rupee", Symbol = "₹" } },
                { "RU", new CurrencyInfo { Code = "RUB", Name = "Russian Ruble", Symbol = "₽" } },
                { "BR", new CurrencyInfo { Code = "BRL", Name = "Brazilian Real", Symbol = "R$" } },
                { "MX", new CurrencyInfo { Code = "MXN", Name = "Mexican Peso", Symbol = "$" } },
                { "CA", new CurrencyInfo { Code = "CAD", Name = "Canadian Dollar", Symbol = "$" } },
                { "AU", new CurrencyInfo { Code = "AUD", Name = "Australian Dollar", Symbol = "$" } },
                // Add more as needed
            };

            public static CurrencyInfo GetCurrencyInfo(string countryCode)
            {
                if (CountryCurrencyMap.TryGetValue(countryCode, out var info))
                    return info;
                return null;
            }

            public static async Task<CurrencyInfo> GetCurrencyInfoFallbackAsync(string countryCode)
            {
                try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                    var url = $"https://restcountries.com/v3.1/alpha/{countryCode}";
                    var resp = await client.GetStringAsync(url);
                    using var doc = JsonDocument.Parse(resp);
                    var root = doc.RootElement[0];
                    var currencyProp = root.GetProperty("currencies").EnumerateObject().First();
                    var code = currencyProp.Name;
                    var name = currencyProp.Value.GetProperty("name").GetString();
                    var symbol = currencyProp.Value.TryGetProperty("symbol", out var s) ? s.GetString() : code;
                    return new CurrencyInfo { Code = code, Name = name, Symbol = symbol };
                }
                catch
                {
                    return null;
                }
            }
        }
}
