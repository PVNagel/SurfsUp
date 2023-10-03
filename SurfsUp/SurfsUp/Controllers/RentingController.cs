using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SurfsUpClassLibrary.Models;

namespace SurfsUp.Controllers
{
    public class RentingController
    {

        public async Task Get()
        {
            HttpClient client = new HttpClient();

            string url = "https://localhost:7022/RentingsAPI";

            var renting = await client.GetFromJsonAsync<Renting[]>(url);

            return;

        }


    }
}
