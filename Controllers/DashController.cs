using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dtacms.Models;

namespace dtacms.Controllers
{
    public class DashController : Controller
    {
        public ActionResult Index(){
            return View();
        }
    }
}