using System.Text.RegularExpressions;

namespace GestaoEventos.API.Helpers
{
    public static class Helpers
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneBR(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var cleanPhone = Regex.Replace(phone, @"[^\d]", "");

            return cleanPhone.Length == 10 || cleanPhone.Length == 11;
        }

        public static string FormatPhoneBR(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return string.Empty;

            var cleanPhone = Regex.Replace(phone, @"[^\d]", "");

            return cleanPhone.Length switch
            {
                10 => $"({cleanPhone[..2]}) {cleanPhone.Substring(2, 4)}-{cleanPhone.Substring(6, 4)}",
                11 => $"({cleanPhone[..2]}) {cleanPhone.Substring(2, 5)}-{cleanPhone.Substring(7, 4)}",
                _ => phone
            };
        }

        public static bool IsFutureDate(DateTime date)
        {
            return date > DateTime.Now;
        }

        public static int DaysDifference(DateTime startDate, DateTime endDate)
        {
            return (endDate.Date - startDate.Date).Days;
        }

        public static string GenerateTicketCode(int eventoId, int ingressoId)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            return $"EVT{eventoId:D4}ING{ingressoId:D6}{timestamp}";
        }

        public static decimal CalculateOccupancyPercentage(int ingressosVendidos, int capacidadeMaxima)
        {
            if (capacidadeMaxima == 0)
                return 0;

            return Math.Round((decimal)ingressosVendidos / capacidadeMaxima * 100, 2);
        }

        public static bool CanCancelEvent(DateTime eventDate)
        {
            return eventDate > DateTime.Now.AddHours(24);
        }

        public static bool CanRefundTicket(DateTime eventDate)
        {
            return eventDate > DateTime.Now.AddHours(2);
        }

        public static string SanitizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input.Trim()
                        .Replace("  ", " ")
                        .Replace("\n", " ")
                        .Replace("\r", " ")
                        .Replace("\t", " ");
        }

        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var words = input.ToLower().Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i][1..];
                }
            }
            return string.Join(" ", words);
        }
    }
}