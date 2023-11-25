using Microsoft.AspNetCore.Mvc;
using static CrudApp1.Controllers.Enum;

namespace CrudApp1.Controllers
{
    public abstract class BaseController : Controller
    {


        public void Alert(string message, NotificationType notificationType)
        {
       

            var msg2 = @"
            
        Swal.fire({

            icon: '"+ notificationType.ToString() + @"',
            title: '"+ message + @"',
            showConfirmButton: false,
            timer: 1500
        })
            ";
            
            TempData["notification"] = msg2;
        }
    }

    public class Enum
    {
        public enum NotificationType
        {
            error,
            success,
            warning,
            info
        }

    }
}
