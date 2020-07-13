using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Helpers;

namespace RoadCover.GenerateToken.Coupon.Service
{
    public class CouponService
    {
        public static string GetCoupon()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, 6)
                                   .Select(s => s[random.Next(s.Length)])
                                   .ToArray());
            return result;
        }

        public static bool AccessCoupon(string token)
        {
            bool coupon = false;
            using (var http = new HttpClient())
            {
                var url = "https://staging-panic.aura.services/panic-api/coupon/2/PJH";
                http.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = http.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                dynamic jsonObject = Json.Decode(result);

                foreach (var coupons in jsonObject.coupons)
                    Console.WriteLine(coupons.couponCode);

                if (response.Result.IsSuccessStatusCode == true)
                {
                    result.ToArray();
                    coupon = true;
                }
                return coupon;
            }
        }
    }
}