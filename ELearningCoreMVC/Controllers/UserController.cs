using ELearningCoreMVC.Data;
using ELearningCoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace ELearningCoreMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly ElearningCoreContext db;
        public UserController(ElearningCoreContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public PaymentPlace PaymentPlaces { get; set; }

        public IActionResult Index(Course c)
        {
            var viewModel = new CombinedViewModel
            {
                Courses = db.Courses.GroupBy(c => c.Cname).Select(g => g.First()).ToList(),
                ContactForm = new ContactFormModel()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(CombinedViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create a new MailMessage object
                    string recipient = viewModel.ContactForm.Email;
                    string myemail = "piyushmaurya7798@gmail.com";
                    string subject = viewModel.ContactForm.Subject;
                    string body = viewModel.ContactForm.Message;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(recipient);
                    mail.To.Add(myemail);
                    mail.Subject = subject;
                    mail.Body = body;

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("piyushmaurya7798@gmail.com", "qbposjoyllyywcld");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mail);

                    ViewBag.Message = "Message sent successfully!";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = $"Error: {ex.Message}";
                }
            }

            // Reload the courses to keep the list populated
            viewModel.Courses = db.Courses.ToList();
            return View(viewModel);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Course(Course c)
        {
            var data = db.Courses.ToList();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Course(int dropdownCourse, string keyword)
        {
            var filteredCourses = await db.Courses.Where(x => x.Id == dropdownCourse).ToListAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                filteredCourses = filteredCourses.Where(x => x.Cname.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return View(filteredCourses);
        }

        [HttpPost]
        public IActionResult CourseDetail(int id)
        {
            var course = db.Courses.Find(id);
            var obj = new Course()
            {
                Cname = course.Cname,
                Subname = course.Subname,
                Price = course.Price
            };
            db.Courses.Add(obj);
            db.SaveChanges();
            return RedirectToAction("CourseDetail");
        }

        public IActionResult EnrollCourse(PaymentPlace p)
        {
            string KeyId = "rzp_test_4DrJJevYZkd0No";
            string KeySecret = "3oX1Dvxlpy2wB3BGnDPxGtH8";

            Random random = new Random();
            string TransactionId = random.Next(0,100000).ToString();

            Dictionary<string, object> input = new Dictionary<string, object>();

            input.Add("amount",Convert.ToDecimal(PaymentPlaces.Price)*100);
            input.Add("currency", "INR");
            input.Add("receipt", TransactionId);

            RazorpayClient client = new RazorpayClient(KeyId, KeySecret);
            Razorpay.Api.Order order = client.Order.Create(input);

            ViewBag.orderId = order["Pid"].ToString();

            return View("Payment", db.PaymentPlaces);
        }

        public IActionResult 

        public IActionResult AddToCart()
        {
            return View();
        }
        public IActionResult MyCourse(Course c)
        {
            var data = db.Courses.ToList();
            return View(data);
        }
    }
}
