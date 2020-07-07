using WebApplication.Models;
using System.Linq;
using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private ShopDBEntities dataBase = new ShopDBEntities();

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Index()
        {
            var Items = dataBase.Cars;
            return View(Items);
        }

        public ActionResult CarPage(int item_id)
        {
            var Item = dataBase.Cars.FirstOrDefault(x => x.Id == item_id);

            if (Item == null)
            {
                return Content("<h1>Page not found</h1>");
            }
            return View(Item);
        }
        
        [ChildActionOnly] 
        public ActionResult Navigation()
        {
            var Items = dataBase.Cars;
            string result = "";
            foreach (var item in Items)
            {
                result += "<li><a href='/Home/CarPage/?item_id=" + item.Id + "' title='" + item.Title + "'>" + item.Title + "</a></li>";
            }
            return Content(result);
        }

        [HttpGet]
        public ActionResult Form(int itemId = 0)
        {
            ViewBag.Item = itemId;
            return PartialView();
        }

        [ChildActionOnly]
        public string FormOptions(int itemId = 0)
        {
            var Items = dataBase.Cars;
            string str = "";

            foreach (var item in Items)
            {
                if (itemId == item.Id)
                {
                    str += "<option value=" + item.Id + " selected>" + item.Title + "</option>";
                }
                else
                {
                    str += "<option value=" + item.Id + ">" + item.Title + "</option>";
                }
            }
            return str;
        }

        [HttpPost]
        public string Form(string Name, string Tel, int Car)
        {
            var car = dataBase.Cars.FirstOrDefault(x => x.Id == Car);
            if (car.Count > 0)
            {
                Order order = new Order
                {
                    UserName = Name,
                    UserTel = Tel,
                    CarId = Car,
                    Status = "Создана"
                };

                var checkId = dataBase.Orders.ToList().Count;

                if (checkId == 0)
                {
                    order.Id = 1;
                }
                else
                {
                    order.Id = checkId + 1;
                }

                dataBase.Orders.Add(order);

                car.Count--;

                dataBase.SaveChanges();

                return $"{order.UserName}, Ваша заявка на автомобиль {car.Title} была принята. Спасибо за заказ.";
            }
            else return $"Извините, автомобиль {car.Title} недоступен, так как автомобили данной модели были распроданы";
        }
    }
}